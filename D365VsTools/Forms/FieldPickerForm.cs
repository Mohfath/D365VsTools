using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using D365VsTools.CodeGenerator.Helpers;
using D365VsTools.CodeGenerator.Model;
using D365VsTools.Xrm;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;

namespace D365VsTools.Forms
{
    public partial class FieldPickerForm : Form
    {
        private readonly IOrganizationService service;
        private readonly MappingSettings mappingSettings;
        private readonly Dictionary<string, EntityMetadata> entityMetadataCache = new Dictionary<string, EntityMetadata>(StringComparer.OrdinalIgnoreCase);

        private AttributeMetadata[] currentAttributes = new AttributeMetadata[0];
        private string currentEntityLogicalName;
        private bool isRefreshing;
        private int sortColumn = -1;
        private SortOrder sortOrder = SortOrder.Ascending;

        /// <summary>
        /// The mapping settings passed into the constructor, mutated in place as fields are checked/unchecked.
        /// Only meaningful to persist when ShowDialog() returns DialogResult.OK.
        /// </summary>
        public MappingSettings MappingSettings => mappingSettings;

        public FieldPickerForm(IOrganizationService service, MappingSettings mappingSettings)
        {
            InitializeComponent();

            this.service = service;
            this.mappingSettings = mappingSettings ?? new MappingSettings();
            if (this.mappingSettings.Entities == null)
                this.mappingSettings.Entities = new Dictionary<string, EntityMappingSetting>();
        }

        private void FieldPickerForm_Load(object sender, EventArgs e)
        {
            cboEntity.Items.AddRange(mappingSettings.Entities.Keys.OrderBy(n => n, StringComparer.OrdinalIgnoreCase).ToArray());
            if (cboEntity.Items.Count > 0)
                cboEntity.SelectedIndex = 0;
        }

        private void cboEntity_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentEntityLogicalName = cboEntity.SelectedItem as string;
            if (string.IsNullOrEmpty(currentEntityLogicalName))
            {
                currentAttributes = new AttributeMetadata[0];
                RefreshFieldList();
                return;
            }

            if (!entityMetadataCache.TryGetValue(currentEntityLogicalName, out var metadata))
            {
                Cursor.Current = Cursors.WaitCursor;
                metadata = service.GetEntityMetadata(currentEntityLogicalName);
                Cursor.Current = Cursors.Default;
                if (metadata != null)
                    entityMetadataCache[currentEntityLogicalName] = metadata;
            }

            // AttributeOf != null marks internal helper attributes (e.g. lookup Name/Type shadow fields),
            // which aren't meaningful to map directly - same filter CodeGenerator's Mapper applies.
            currentAttributes = metadata?.Attributes?.Where(a => a.AttributeOf == null).ToArray() ?? new AttributeMetadata[0];
            RefreshFieldList();
        }

        private void Filter_Changed(object sender, EventArgs e)
        {
            RefreshFieldList();
        }

        private void RefreshFieldList()
        {
            if (currentEntityLogicalName == null)
            {
                lvFields.Items.Clear();
                return;
            }

            mappingSettings.Entities.TryGetValue(currentEntityLogicalName, out var entityMapping);
            var mappedAttributes = entityMapping?.Attributes ?? new Dictionary<string, string>();

            var filter = txtFilter.Text?.Trim() ?? "";
            var showSystem = cbShowSystemFields.Checked;
            var showManaged = cbShowManagedFields.Checked;

            var items = currentAttributes.Where(a =>
            {
                if (!showSystem && a.IsCustomAttribute != true)
                    return false;
                if (!showManaged && a.IsManaged == true)
                    return false;
                if (filter.Length == 0)
                    return true;

                var label = a.DisplayName?.UserLocalizedLabel?.Label ?? "";
                return a.LogicalName.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0
                    || label.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0;
            });

            isRefreshing = true;
            lvFields.BeginUpdate();
            lvFields.Items.Clear();
            foreach (var attribute in items)
            {
                var label = attribute.DisplayName?.UserLocalizedLabel?.Label ?? "";
                var type = attribute.AttributeType?.ToString() ?? "Unknown";

                var item = new ListViewItem(new[] { attribute.LogicalName, label, type })
                {
                    Tag = attribute,
                    Checked = mappedAttributes.ContainsKey(attribute.LogicalName)
                };
                lvFields.Items.Add(item);
            }
            SortFieldList();
            lvFields.EndUpdate();
            isRefreshing = false;
        }

        private void SortFieldList()
        {
            if (sortColumn < 0)
                return;

            lvFields.ListViewItemSorter = new ListViewItemComparer(sortColumn, sortOrder);
            lvFields.Sort();
        }

        private void lvFields_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            if (sortColumn == e.Column)
                sortOrder = sortOrder == SortOrder.Ascending ? SortOrder.Descending : SortOrder.Ascending;
            else
            {
                sortColumn = e.Column;
                sortOrder = SortOrder.Ascending;
            }

            SortFieldList();
        }

        private void lvFields_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            if (isRefreshing || currentEntityLogicalName == null)
                return;

            if (!mappingSettings.Entities.TryGetValue(currentEntityLogicalName, out var entityMapping))
            {
                entityMapping = new EntityMappingSetting { Attributes = new Dictionary<string, string>() };
                mappingSettings.Entities[currentEntityLogicalName] = entityMapping;
            }
            if (entityMapping.Attributes == null)
                entityMapping.Attributes = new Dictionary<string, string>();

            var attribute = (AttributeMetadata)e.Item.Tag;
            if (e.Item.Checked)
            {
                var label = attribute.DisplayName?.UserLocalizedLabel?.Label;
                var codeName = !string.IsNullOrWhiteSpace(label)
                    ? Naming.GetCodeNameFromLabel(label)
                    : Naming.GetCodeNameFromLabel(attribute.LogicalName);

                entityMapping.Attributes[attribute.LogicalName] = codeName;
            }
            else
            {
                entityMapping.Attributes.Remove(attribute.LogicalName);
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private class ListViewItemComparer : System.Collections.IComparer
        {
            private readonly int column;
            private readonly SortOrder order;

            public ListViewItemComparer(int column, SortOrder order)
            {
                this.column = column;
                this.order = order;
            }

            public int Compare(object x, object y)
            {
                var itemX = (ListViewItem)x;
                var itemY = (ListViewItem)y;
                var result = string.Compare(itemX.SubItems[column].Text, itemY.SubItems[column].Text, StringComparison.OrdinalIgnoreCase);
                return order == SortOrder.Descending ? -result : result;
            }
        }
    }
}
