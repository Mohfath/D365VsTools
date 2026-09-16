using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using D365VsTools.CodeGenerator.Helpers;
using D365VsTools.CodeGenerator.Model;
using Microsoft.Xrm.Sdk.Metadata;

namespace D365VsTools.Forms
{
    public partial class EntityPickerForm : Form
    {
        private readonly Dictionary<string, EntityMetadata> allEntitiesByLogicalName;
        private readonly MappingSettings mappingSettings;

        /// <summary>
        /// The mapping settings passed into the constructor, mutated in place by Add/Remove.
        /// Only meaningful to persist when ShowDialog() returns DialogResult.OK.
        /// </summary>
        public MappingSettings MappingSettings => mappingSettings;

        public EntityPickerForm(IEnumerable<EntityMetadata> allEntities, MappingSettings mappingSettings)
        {
            InitializeComponent();

            allEntitiesByLogicalName = allEntities
                .Where(e => !string.IsNullOrEmpty(e.LogicalName))
                .GroupBy(e => e.LogicalName)
                .ToDictionary(g => g.Key, g => g.First());

            this.mappingSettings = mappingSettings ?? new MappingSettings();
            if (this.mappingSettings.Entities == null)
                this.mappingSettings.Entities = new Dictionary<string, EntityMappingSetting>();
        }

        private void EntityPickerForm_Load(object sender, EventArgs e)
        {
            RefreshLists();
        }

        private string GetLabel(string logicalName) =>
            allEntitiesByLogicalName.TryGetValue(logicalName, out var metadata)
                ? metadata.DisplayName?.UserLocalizedLabel?.Label ?? ""
                : "";

        private IEnumerable<string> AvailableLogicalNames =>
            allEntitiesByLogicalName.Keys
                .Where(n => !mappingSettings.Entities.ContainsKey(n))
                .OrderBy(n => n, StringComparer.OrdinalIgnoreCase);

        private void RefreshLists()
        {
            RefreshAvailableList();
            RefreshSelectedList();
        }

        private void RefreshAvailableList()
        {
            var filter = txtFilter.Text?.Trim() ?? "";
            var items = AvailableLogicalNames
                .Where(n => filter.Length == 0
                    || n.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0
                    || GetLabel(n).IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0)
                .ToArray();

            RepopulatePreservingScroll(lstAvailable, items);
        }

        private void RefreshSelectedList()
        {
            var items = mappingSettings.Entities.Keys.OrderBy(n => n, StringComparer.OrdinalIgnoreCase).ToArray();

            RepopulatePreservingScroll(lstSelected, items);
        }

        /// <summary>
        /// Clears and repopulates a ListView without resetting its scroll position back to the top -
        /// ListView.Items.Clear() otherwise loses the current TopItem, which is jarring when adding or
        /// removing a single entity from a list you're scrolled through.
        /// </summary>
        private void RepopulatePreservingScroll(ListView listView, string[] logicalNames)
        {
            var topIndex = listView.TopItem?.Index ?? -1;

            listView.BeginUpdate();
            listView.Items.Clear();
            foreach (var logicalName in logicalNames)
                listView.Items.Add(new ListViewItem(new[] { logicalName, GetLabel(logicalName) }));

            if (topIndex >= 0 && listView.Items.Count > 0)
                listView.TopItem = listView.Items[Math.Min(topIndex, listView.Items.Count - 1)];

            listView.EndUpdate();
        }

        private void txtFilter_TextChanged(object sender, EventArgs e)
        {
            RefreshAvailableList();
        }

        private void lstAvailable_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (lstAvailable.HitTest(e.Location).Item != null)
                AddSelectedEntities();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            AddSelectedEntities();
        }

        private void AddSelectedEntities()
        {
            var selected = lstAvailable.SelectedItems.Cast<ListViewItem>().Select(i => i.Text).ToArray();
            foreach (var logicalName in selected)
            {
                if (mappingSettings.Entities.ContainsKey(logicalName))
                    continue;

                if (!allEntitiesByLogicalName.TryGetValue(logicalName, out var metadata))
                    continue;

                var label = metadata.DisplayName?.UserLocalizedLabel?.Label;
                var codeName = !string.IsNullOrWhiteSpace(label)
                    ? Naming.GetCodeNameFromLabel(label)
                    : Naming.GetCodeNameFromLabel(!string.IsNullOrWhiteSpace(metadata.SchemaName) ? metadata.SchemaName : logicalName);

                var mapping = new EntityMappingSetting
                {
                    CodeName = codeName,
                    Attributes = new Dictionary<string, string>()
                };

                // The only field a freshly-added entity needs is its primary key, mapped to "Id"
                // (matches this codebase's existing convention for the primary key CodeName).
                if (!string.IsNullOrWhiteSpace(metadata.PrimaryIdAttribute))
                    mapping.Attributes[metadata.PrimaryIdAttribute] = "Id";

                mappingSettings.Entities[logicalName] = mapping;
            }

            RefreshLists();
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            var selected = lstSelected.SelectedItems.Cast<ListViewItem>().Select(i => i.Text).ToArray();
            foreach (var logicalName in selected)
                mappingSettings.Entities.Remove(logicalName);

            RefreshLists();
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
    }
}
