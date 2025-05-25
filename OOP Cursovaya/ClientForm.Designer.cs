namespace OOP_Cursovaya
{
    partial class ClientForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.DataGridView dataGridView;
        private System.Windows.Forms.TextBox txtWeight;
        private System.Windows.Forms.TextBox txtPrice;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnEdit;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.ComboBox comboBoxFilter;
        private System.Windows.Forms.TextBox txtFilterValue;
        private System.Windows.Forms.Button btnFilter;
        private System.Windows.Forms.Button btnClearFilter;
        private System.Windows.Forms.Label lblCategory;
        private System.Windows.Forms.Label lblPrice;
        private System.Windows.Forms.Label lblWeight;
        private System.Windows.Forms.ComboBox comboBoxCategory;
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openDatabaseToolStripMenuItem; 
        private System.Windows.Forms.ToolStripMenuItem createDatabaseToolStripMenuItem; 
        private System.Windows.Forms.ToolStripMenuItem deleteDatabaseToolStripMenuItem;
        private System.Windows.Forms.Label lblFilteredCount;

        /// <summary>
        /// Освобождает неуправляемые ресурсы, используемые формой, а при необходимости — также управляемые ресурсы.
        /// </summary>
        /// <param name="disposing">
        /// Значение <see langword="true"/> указывает, что следует освободить как управляемые, так и неуправляемые ресурсы;
        /// Значение <see langword="false"/> означает, что нужно освободить только неуправляемые ресурсы.
        /// </param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ClientForm));
            dataGridView = new DataGridView();
            Id = new DataGridViewTextBoxColumn();
            Category = new DataGridViewTextBoxColumn();
            Weight = new DataGridViewTextBoxColumn();
            Price = new DataGridViewTextBoxColumn();
            lblCategory = new Label();
            lblWeight = new Label();
            lblPrice = new Label();
            comboBoxCategory = new ComboBox();
            txtWeight = new TextBox();
            txtPrice = new TextBox();
            btnAdd = new Button();
            btnEdit = new Button();
            btnDelete = new Button();
            comboBoxFilter = new ComboBox();
            txtFilterValue = new TextBox();
            btnFilter = new Button();
            btnClearFilter = new Button();
            menuStrip = new MenuStrip();
            fileToolStripMenuItem = new ToolStripMenuItem();
            openDatabaseToolStripMenuItem = new ToolStripMenuItem();
            createDatabaseToolStripMenuItem = new ToolStripMenuItem();
            saveAsDatabaseToolStripMenuItem = new ToolStripMenuItem();
            SaveTableAsPDFToolStripMenuItem = new ToolStripMenuItem();
            deleteDatabaseToolStripMenuItem = new ToolStripMenuItem();
            labelFilterByValue = new Label();
            lblFilteredCount = new Label();
            buttonSearch = new Button();
            checkBoxExactMatch = new CheckBox();
            lblDatabaseName = new Label();
            ((System.ComponentModel.ISupportInitialize)dataGridView).BeginInit();
            menuStrip.SuspendLayout();
            SuspendLayout();
            // 
            // dataGridView
            // 
            dataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView.Columns.AddRange(new DataGridViewColumn[] { Id, Category, Weight, Price });
            dataGridView.Location = new Point(12, 45);
            dataGridView.Name = "dataGridView";
            dataGridView.ReadOnly = true;
            dataGridView.RowHeadersWidth = 51;
            dataGridView.Size = new Size(922, 300);
            dataGridView.TabIndex = 0;
            dataGridView.ColumnHeaderMouseClick += dataGridView_ColumnHeaderMouseClick;
            // 
            // Id
            // 
            Id.HeaderText = "Id";
            Id.MinimumWidth = 6;
            Id.Name = "Id";
            Id.ReadOnly = true;
            Id.Width = 190;
            // 
            // Category
            // 
            Category.HeaderText = "Категория";
            Category.MinimumWidth = 6;
            Category.Name = "Category";
            Category.ReadOnly = true;
            Category.Width = 190;
            // 
            // Weight
            // 
            Weight.HeaderText = "Вес (кг)";
            Weight.MinimumWidth = 6;
            Weight.Name = "Weight";
            Weight.ReadOnly = true;
            Weight.Width = 190;
            // 
            // Price
            // 
            Price.HeaderText = "Цена (руб)";
            Price.MinimumWidth = 6;
            Price.Name = "Price";
            Price.ReadOnly = true;
            Price.Width = 190;
            // 
            // lblCategory
            // 
            lblCategory.AutoSize = true;
            lblCategory.Location = new Point(13, 358);
            lblCategory.Name = "lblCategory";
            lblCategory.Size = new Size(81, 20);
            lblCategory.TabIndex = 1;
            lblCategory.Text = "Категория";
            // 
            // lblWeight
            // 
            lblWeight.AutoSize = true;
            lblWeight.Location = new Point(169, 358);
            lblWeight.Name = "lblWeight";
            lblWeight.Size = new Size(60, 20);
            lblWeight.TabIndex = 3;
            lblWeight.Text = "Вес (кг)";
            // 
            // lblPrice
            // 
            lblPrice.AutoSize = true;
            lblPrice.Location = new Point(325, 358);
            lblPrice.Name = "lblPrice";
            lblPrice.Size = new Size(84, 20);
            lblPrice.TabIndex = 5;
            lblPrice.Text = "Цена (руб)";
            // 
            // comboBoxCategory
            // 
            comboBoxCategory.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxCategory.Enabled = false;
            comboBoxCategory.FormattingEnabled = true;
            comboBoxCategory.Items.AddRange(new object[] { "Стул", "Стол", "Шкаф", "Диван" });
            comboBoxCategory.SelectedIndex = 0;
            comboBoxCategory.Location = new Point(13, 381);
            comboBoxCategory.Name = "comboBoxCategory";
            comboBoxCategory.Size = new Size(150, 28);
            comboBoxCategory.TabIndex = 2;
            // 
            // txtWeight
            // 
            txtWeight.Enabled = false;
            txtWeight.Location = new Point(169, 381);
            txtWeight.Name = "txtWeight";
            txtWeight.Size = new Size(150, 27);
            txtWeight.TabIndex = 4;
            txtWeight.KeyPress += txtWeight_KeyPress;
            // 
            // txtPrice
            // 
            txtPrice.Enabled = false;
            txtPrice.Location = new Point(325, 381);
            txtPrice.Name = "txtPrice";
            txtPrice.Size = new Size(150, 27);
            txtPrice.TabIndex = 6;
            txtPrice.KeyPress += txtWeight_KeyPress;
            // 
            // btnAdd
            // 
            btnAdd.Enabled = false;
            btnAdd.Location = new Point(481, 381);
            btnAdd.Name = "btnAdd";
            btnAdd.Size = new Size(125, 37);
            btnAdd.TabIndex = 7;
            btnAdd.Text = "Добавить";
            btnAdd.Click += btnAdd_Click;
            // 
            // btnEdit
            // 
            btnEdit.Enabled = false;
            btnEdit.Location = new Point(481, 422);
            btnEdit.Name = "btnEdit";
            btnEdit.Size = new Size(125, 40);
            btnEdit.TabIndex = 8;
            btnEdit.Text = "Редактировать";
            btnEdit.Click += btnEdit_Click;
            // 
            // btnDelete
            // 
            btnDelete.Enabled = false;
            btnDelete.Location = new Point(612, 381);
            btnDelete.Name = "btnDelete";
            btnDelete.Size = new Size(125, 37);
            btnDelete.TabIndex = 9;
            btnDelete.Text = "Удалить";
            btnDelete.Click += btnDelete_Click;
            // 
            // comboBoxFilter
            // 
            comboBoxFilter.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxFilter.Enabled = false;
            comboBoxFilter.FormattingEnabled = true;
            comboBoxFilter.Items.AddRange(new object[] { "ID", "Категория", "Вес", "Цена" });
            comboBoxFilter.Location = new Point(12, 499);
            comboBoxFilter.Name = "comboBoxFilter";
            comboBoxFilter.Size = new Size(150, 28);
            comboBoxFilter.TabIndex = 10;
            // 
            // txtFilterValue
            // 
            txtFilterValue.Enabled = false;
            txtFilterValue.Location = new Point(168, 500);
            txtFilterValue.Name = "txtFilterValue";
            txtFilterValue.Size = new Size(150, 27);
            txtFilterValue.TabIndex = 11;
            // 
            // btnFilter
            // 
            btnFilter.Enabled = false;
            btnFilter.Location = new Point(324, 499);
            btnFilter.Name = "btnFilter";
            btnFilter.Size = new Size(108, 37);
            btnFilter.TabIndex = 12;
            btnFilter.Text = "Фильтровать";
            btnFilter.Click += btnFilter_Click;
            // 
            // btnClearFilter
            // 
            btnClearFilter.Enabled = false;
            btnClearFilter.Location = new Point(196, 542);
            btnClearFilter.Name = "btnClearFilter";
            btnClearFilter.Size = new Size(108, 31);
            btnClearFilter.TabIndex = 13;
            btnClearFilter.Text = "Сбросить фильтр";
            btnClearFilter.Click += btnClearFilter_Click;
            // 
            // menuStrip
            // 
            menuStrip.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            menuStrip.Dock = DockStyle.None;
            menuStrip.ImageScalingSize = new Size(20, 20);
            menuStrip.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem });
            menuStrip.LayoutStyle = ToolStripLayoutStyle.HorizontalStackWithOverflow;
            menuStrip.Location = new Point(1119, 0);
            menuStrip.Name = "menuStrip";
            menuStrip.Size = new Size(67, 28);
            menuStrip.TabIndex = 14;
            menuStrip.Text = "menuStrip";
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownDirection = ToolStripDropDownDirection.BelowLeft;
            fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { openDatabaseToolStripMenuItem, createDatabaseToolStripMenuItem, saveAsDatabaseToolStripMenuItem, SaveTableAsPDFToolStripMenuItem, deleteDatabaseToolStripMenuItem });
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new Size(59, 24);
            fileToolStripMenuItem.Text = "Файл";
            // 
            // openDatabaseToolStripMenuItem
            // 
            openDatabaseToolStripMenuItem.Name = "openDatabaseToolStripMenuItem";
            openDatabaseToolStripMenuItem.Size = new Size(268, 26);
            openDatabaseToolStripMenuItem.Text = "Открыть БД";
            openDatabaseToolStripMenuItem.Click += OpenDatabaseToolStripMenuItem_Click;
            // 
            // createDatabaseToolStripMenuItem
            // 
            createDatabaseToolStripMenuItem.Name = "createDatabaseToolStripMenuItem";
            createDatabaseToolStripMenuItem.Size = new Size(268, 26);
            createDatabaseToolStripMenuItem.Text = "Создать БД";
            createDatabaseToolStripMenuItem.Click += CreateDatabaseToolStripMenuItem_Click;
            // 
            // saveAsDatabaseToolStripMenuItem
            // 
            saveAsDatabaseToolStripMenuItem.Name = "saveAsDatabaseToolStripMenuItem";
            saveAsDatabaseToolStripMenuItem.Size = new Size(268, 26);
            saveAsDatabaseToolStripMenuItem.Text = "Сохранить как";
            saveAsDatabaseToolStripMenuItem.Click += SaveAsDatabaseToolStripMenuItem_Click;
            // 
            // SaveTableAsPDFToolStripMenuItem
            // 
            SaveTableAsPDFToolStripMenuItem.Name = "SaveTableAsPDFToolStripMenuItem";
            SaveTableAsPDFToolStripMenuItem.Size = new Size(268, 26);
            SaveTableAsPDFToolStripMenuItem.Text = "Сохранить таблицу в PDF";
            SaveTableAsPDFToolStripMenuItem.Click += SaveTableAsPDFToolStripMenuItem_Click;
            // 
            // deleteDatabaseToolStripMenuItem
            // 
            deleteDatabaseToolStripMenuItem.Enabled = false;
            deleteDatabaseToolStripMenuItem.Name = "deleteDatabaseToolStripMenuItem";
            deleteDatabaseToolStripMenuItem.Size = new Size(268, 26);
            deleteDatabaseToolStripMenuItem.Text = "Удалить текущую БД";
            deleteDatabaseToolStripMenuItem.Click += DeleteDatabaseToolStripMenuItem_Click;
            // 
            // labelFilterByValue
            // 
            labelFilterByValue.AutoSize = true;
            labelFilterByValue.Location = new Point(12, 464);
            labelFilterByValue.Name = "labelFilterByValue";
            labelFilterByValue.Size = new Size(190, 20);
            labelFilterByValue.TabIndex = 17;
            labelFilterByValue.Text = "Фильтрация по значению";
            // 
            // lblFilteredCount
            // 
            lblFilteredCount.AutoSize = true;
            lblFilteredCount.Location = new Point(12, 545);
            lblFilteredCount.Name = "lblFilteredCount";
            lblFilteredCount.Size = new Size(141, 20);
            lblFilteredCount.TabIndex = 18;
            lblFilteredCount.Text = "Всего элементов: 0";
            // 
            // buttonSearch
            // 
            buttonSearch.Enabled = false;
            buttonSearch.Location = new Point(324, 542);
            buttonSearch.Name = "buttonSearch";
            buttonSearch.Size = new Size(108, 31);
            buttonSearch.TabIndex = 19;
            buttonSearch.Text = "Поиск";
            buttonSearch.Click += ButtonSearch_Click;
            // 
            // checkBoxExactMatch
            // 
            checkBoxExactMatch.AutoSize = true;
            checkBoxExactMatch.Enabled = false;
            checkBoxExactMatch.Location = new Point(438, 503);
            checkBoxExactMatch.Name = "checkBoxExactMatch";
            checkBoxExactMatch.Size = new Size(169, 24);
            checkBoxExactMatch.TabIndex = 20;
            checkBoxExactMatch.Text = "Точное совпадение";
            checkBoxExactMatch.UseVisualStyleBackColor = true;
            // 
            // lblDatabaseName
            // 
            lblDatabaseName.AutoSize = true;
            lblDatabaseName.Location = new Point(12, 9);
            lblDatabaseName.Name = "lblDatabaseName";
            lblDatabaseName.Size = new Size(93, 20);
            lblDatabaseName.TabIndex = 21;
            lblDatabaseName.Text = "Текущая БД:";
            // 
            // ClientForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1186, 618);
            Controls.Add(lblDatabaseName);
            Controls.Add(checkBoxExactMatch);
            Controls.Add(buttonSearch);
            Controls.Add(labelFilterByValue);
            Controls.Add(btnClearFilter);
            Controls.Add(btnFilter);
            Controls.Add(txtFilterValue);
            Controls.Add(comboBoxFilter);
            Controls.Add(btnDelete);
            Controls.Add(btnEdit);
            Controls.Add(btnAdd);
            Controls.Add(txtPrice);
            Controls.Add(lblPrice);
            Controls.Add(txtWeight);
            Controls.Add(lblWeight);
            Controls.Add(comboBoxCategory);
            Controls.Add(lblCategory);
            Controls.Add(dataGridView);
            Controls.Add(menuStrip);
            Controls.Add(lblFilteredCount);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MainMenuStrip = menuStrip;
            Name = "ClientForm";
            Text = "Курсовой проект, Степанов 23ВП2, Мебель";
            Load += ClientForm_Load;
            ((System.ComponentModel.ISupportInitialize)dataGridView).EndInit();
            menuStrip.ResumeLayout(false);
            menuStrip.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }
        private ToolStripMenuItem saveAsDatabaseToolStripMenuItem;
        private ToolStripMenuItem SaveTableAsPDFToolStripMenuItem;
        private Label labelFilterByValue;
        private Button buttonSearch;
        private CheckBox checkBoxExactMatch;
        private Label lblDatabaseName;
        private DataGridViewTextBoxColumn Id;
        private DataGridViewTextBoxColumn Category;
        private DataGridViewTextBoxColumn Weight;
        private DataGridViewTextBoxColumn Price;
    }
}