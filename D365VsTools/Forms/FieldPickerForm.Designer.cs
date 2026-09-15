namespace D365VsTools.Forms
{
    partial class FieldPickerForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lbEntity = new System.Windows.Forms.Label();
            this.cboEntity = new System.Windows.Forms.ComboBox();
            this.lbFilter = new System.Windows.Forms.Label();
            this.txtFilter = new System.Windows.Forms.TextBox();
            this.cbShowSystemFields = new System.Windows.Forms.CheckBox();
            this.cbShowManagedFields = new System.Windows.Forms.CheckBox();
            this.lvFields = new System.Windows.Forms.ListView();
            this.colLogicalName = new System.Windows.Forms.ColumnHeader();
            this.colLabel = new System.Windows.Forms.ColumnHeader();
            this.colType = new System.Windows.Forms.ColumnHeader();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            //
            // lbEntity
            //
            this.lbEntity.AutoSize = true;
            this.lbEntity.Location = new System.Drawing.Point(13, 15);
            this.lbEntity.Name = "lbEntity";
            this.lbEntity.Size = new System.Drawing.Size(38, 13);
            this.lbEntity.TabIndex = 0;
            this.lbEntity.Text = "Entity:";
            //
            // cboEntity
            //
            this.cboEntity.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboEntity.FormattingEnabled = true;
            this.cboEntity.Location = new System.Drawing.Point(70, 12);
            this.cboEntity.Name = "cboEntity";
            this.cboEntity.Size = new System.Drawing.Size(300, 21);
            this.cboEntity.TabIndex = 1;
            this.cboEntity.SelectedIndexChanged += new System.EventHandler(this.cboEntity_SelectedIndexChanged);
            //
            // lbFilter
            //
            this.lbFilter.AutoSize = true;
            this.lbFilter.Location = new System.Drawing.Point(13, 46);
            this.lbFilter.Name = "lbFilter";
            this.lbFilter.Size = new System.Drawing.Size(33, 13);
            this.lbFilter.TabIndex = 2;
            this.lbFilter.Text = "Filter:";
            //
            // txtFilter
            //
            this.txtFilter.Location = new System.Drawing.Point(70, 43);
            this.txtFilter.Name = "txtFilter";
            this.txtFilter.Size = new System.Drawing.Size(300, 20);
            this.txtFilter.TabIndex = 3;
            this.txtFilter.TextChanged += new System.EventHandler(this.Filter_Changed);
            //
            // cbShowSystemFields
            //
            this.cbShowSystemFields.AutoSize = true;
            this.cbShowSystemFields.Location = new System.Drawing.Point(390, 45);
            this.cbShowSystemFields.Name = "cbShowSystemFields";
            this.cbShowSystemFields.Size = new System.Drawing.Size(115, 17);
            this.cbShowSystemFields.TabIndex = 4;
            this.cbShowSystemFields.Text = "Show system fields";
            this.cbShowSystemFields.UseVisualStyleBackColor = true;
            this.cbShowSystemFields.CheckedChanged += new System.EventHandler(this.Filter_Changed);
            //
            // cbShowManagedFields
            //
            this.cbShowManagedFields.AutoSize = true;
            this.cbShowManagedFields.Location = new System.Drawing.Point(530, 45);
            this.cbShowManagedFields.Name = "cbShowManagedFields";
            this.cbShowManagedFields.Size = new System.Drawing.Size(129, 17);
            this.cbShowManagedFields.TabIndex = 5;
            this.cbShowManagedFields.Text = "Show managed fields";
            this.cbShowManagedFields.UseVisualStyleBackColor = true;
            this.cbShowManagedFields.CheckedChanged += new System.EventHandler(this.Filter_Changed);
            //
            // lvFields
            //
            this.lvFields.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvFields.CheckBoxes = true;
            this.lvFields.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colLogicalName,
            this.colLabel,
            this.colType});
            this.lvFields.FullRowSelect = true;
            this.lvFields.GridLines = true;
            this.lvFields.HideSelection = false;
            this.lvFields.Location = new System.Drawing.Point(13, 75);
            this.lvFields.Name = "lvFields";
            this.lvFields.Size = new System.Drawing.Size(674, 330);
            this.lvFields.TabIndex = 6;
            this.lvFields.UseCompatibleStateImageBehavior = false;
            this.lvFields.View = System.Windows.Forms.View.Details;
            this.lvFields.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.lvFields_ColumnClick);
            this.lvFields.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.lvFields_ItemChecked);
            //
            // colLogicalName
            //
            this.colLogicalName.Text = "Logical Name";
            this.colLogicalName.Width = 220;
            //
            // colLabel
            //
            this.colLabel.Text = "Label";
            this.colLabel.Width = 260;
            //
            // colType
            //
            this.colType.Text = "Type";
            this.colType.Width = 150;
            //
            // btnOk
            //
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOk.Location = new System.Drawing.Point(532, 415);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 7;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            //
            // btnCancel
            //
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(613, 415);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 8;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            //
            // FieldPickerForm
            //
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(700, 448);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.lvFields);
            this.Controls.Add(this.cbShowManagedFields);
            this.Controls.Add(this.cbShowSystemFields);
            this.Controls.Add(this.txtFilter);
            this.Controls.Add(this.lbFilter);
            this.Controls.Add(this.cboEntity);
            this.Controls.Add(this.lbEntity);
            this.MinimizeBox = false;
            this.MaximizeBox = false;
            this.Name = "FieldPickerForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Field Picker";
            this.Load += new System.EventHandler(this.FieldPickerForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lbEntity;
        private System.Windows.Forms.ComboBox cboEntity;
        private System.Windows.Forms.Label lbFilter;
        private System.Windows.Forms.TextBox txtFilter;
        private System.Windows.Forms.CheckBox cbShowSystemFields;
        private System.Windows.Forms.CheckBox cbShowManagedFields;
        private System.Windows.Forms.ListView lvFields;
        private System.Windows.Forms.ColumnHeader colLogicalName;
        private System.Windows.Forms.ColumnHeader colLabel;
        private System.Windows.Forms.ColumnHeader colType;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
    }
}
