using System;
#nullable disable
using System.Collections.Generic;
using System.Data.SQLite;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.IO;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Globalization;
using System.ComponentModel;
using System.Linq;
using System.Diagnostics.CodeAnalysis;


namespace OOP_Cursovaya
{
    /// <summary>
    /// ������� ����� ���������� ��� ������ � ����� ������ ������
    /// </summary>
    public partial class ClientForm : Form
    {
        private DatabaseContext _dbContext;
        private ListSortDirection _currentSortDirection = ListSortDirection.Ascending;
        private string _currentSortColumn = "";
        private int _totalItemsCount = 0;
        private int _filteredItemsCount = 0;

        /// <summary>
        /// ����������� �����, �������������� ���������� � ����
        /// </summary>
        public ClientForm()
        {
            InitializeComponent();
            InitializeMenu();
            dataGridView.SelectionChanged += dataGridView_SelectionChanged;

        }

        /// <summary>
        /// ������������� ��������� ������� ����
        /// </summary>
        private void InitializeMenu()
        {
            deleteDatabaseToolStripMenuItem.Enabled = false; // �������� ����������
            openDatabaseToolStripMenuItem.Enabled = true;    // �������� ��������
            createDatabaseToolStripMenuItem.Enabled = true;  // �������� ��������

        }

        /// <summary>
        /// ���������� �������� �����
        /// </summary>
        private void ClientForm_Load(object sender, EventArgs e)
        {
            comboBoxFilter.SelectedIndex = 0;

            // ��������� ������ ������ ���� ���� ������ �������
            if (_dbContext != null)
            {
                LoadData();
            }
        }

        /// <summary>
        /// ��������� ������� ��������������� ���������
        /// </summary>
        /// <param name="isFiltered">����, ����������� �������� �� ������</param>
        /// <param name="count">���������� ���������</param>
        private void UpdateFilterCount(bool isFiltered, int count)
        {
            if (isFiltered)
            {
                lblFilteredCount.Text = $"�������������: {count} �� {_totalItemsCount}";
            }
            else
            {
                lblFilteredCount.Text = $"����� ���������: {count}";
            }
        }

        /// <summary>
        /// ��������� ������ �� ���� ������ � DataGridView
        /// </summary>
        private void LoadData()
        {
            if (_dbContext == null)
            {
                dataGridView.Rows.Clear();
                UpdateFilterCount(false, 0);
                return;
            }

            try
            {
                // �������� ��� ������ �� ���� ������
                var furnitureList = _dbContext.GetAllFurniture();
                _totalItemsCount = furnitureList.Count;
                _filteredItemsCount = _totalItemsCount;

                dataGridView.Rows.Clear();

                // ��������� DataGridView �������
                foreach (var furniture in furnitureList)
                {
                    string weight = furniture.Weight.ToString("F2", CultureInfo.InvariantCulture);
                    string price = furniture.Price.ToString("F2", CultureInfo.InvariantCulture);
                    dataGridView.Rows.Add(furniture.Id, furniture.Category, weight, price);
                }

                UpdateFilterCount(false, _totalItemsCount);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"������ ��� �������� ������: {ex.Message}", "������", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// ���������� ����� ��� ���� ����, ��������� ������ ����� � �����
        /// </summary>
        private void txtWeight_KeyPress(object sender, KeyPressEventArgs e)
        {
            // ��������� �����, ����� � ����������� ������� (��������, Backspace)
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != '.' && !char.IsControl(e.KeyChar))
            {
                e.Handled = true; // ���������� ����
            }

            // ��������� ���� ����� ����� �����
            if (e.KeyChar == '.' && (sender as TextBox).Text.Contains("."))
            {
                e.Handled = true;
            }
        }

        /// <summary>
        /// ���������� ������ ���������� ����� ������
        /// </summary>
        private void btnAdd_Click(object sender, EventArgs e)
        {
            // ��������� ���������� ������������ �����
            if (string.IsNullOrEmpty(txtWeight.Text) || string.IsNullOrEmpty(txtPrice.Text))
            {
                MessageBox.Show("��������� ��� ����.", "������", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // ������� ����� ������ ������
                var furniture = new Furniture
                {
                    Category = comboBoxCategory.SelectedItem.ToString(),
                    Weight = double.Parse(txtWeight.Text, CultureInfo.InvariantCulture),
                    Price = double.Parse(txtPrice.Text, CultureInfo.InvariantCulture)
                };

                // ��������� ������ � ���� ������
                _dbContext.AddFurniture(furniture);
                LoadData(); // ��������� ����������� ������
            }
            catch (FormatException)
            {
                MessageBox.Show("������������ ������ �����. ����������� ����� ��� �����������.", "������", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"������ ��� ���������� ������: {ex.Message}", "������", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// ������������ ��������� ��������� ������ � DataGridView, �������� ��������������� �������� ����������.
        /// </summary>
        /// <param name="sender">�������� �������</param>
        /// <param name="e">������ �������</param>
        /// <remarks>
        /// ��������� �����-���� ��������� � ��������� ���� ������� �� ��������� ������.
        /// </remarks>
        private void dataGridView_SelectionChanged([NotNull] object sender, EventArgs e)
        {
            if (dataGridView.SelectedRows.Count == 0)
                return;

            var row = dataGridView.SelectedRows[0];

            if (row.Cells["Category"]?.Value is { } categoryValue)
                comboBoxCategory.SelectedItem = categoryValue.ToString();

            txtWeight.Text = row.Cells["Weight"]?.Value?.ToString();
            txtPrice.Text = row.Cells["Price"]?.Value?.ToString();
        }

        /// <summary>
        /// ������������ ������� ������ ��������������, �������� ��������� ������ � ���� ������.
        /// </summary>
        /// <param name="sender">�������� �������</param>
        /// <param name="e">������ �������</param>
        /// <remarks>
        /// ��������� ������������ ��������� ������ ����� ����������� ������.
        /// � ������ ������ ���������� ��������������� ���������.
        /// </remarks>
        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (dataGridView.SelectedRows.Count == 0)
            {
                MessageBox.Show("�������� ������ ��� ��������������.", "������", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var selectedRow = dataGridView.SelectedRows[0];

            if (!double.TryParse(txtWeight.Text.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out double weight))
            {
                MessageBox.Show("������������ ������ ����.", "������", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!double.TryParse(txtPrice.Text.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out double price))
            {
                MessageBox.Show("������������ ������ ����.", "������", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                var furniture = new Furniture
                {
                    Id = (int)selectedRow.Cells["Id"].Value,
                    Category = comboBoxCategory.SelectedItem?.ToString(),
                    Weight = weight,
                    Price = price
                };

                _dbContext.UpdateFurniture(furniture);
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"������ ��� ���������� ������: {ex.Message}", "������", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// ���������� ������ �������� ������
        /// </summary>
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dataGridView.SelectedRows.Count > 0)
            {
                var selectedRow = dataGridView.SelectedRows[0];

                // ���������, ��� ������ �� �������� ������ ��� ����������
                if (selectedRow.IsNewRow)
                {
                    MessageBox.Show("�������� ������ ��� ��������.", "������", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // �������� ID ������ ��� ��������
                int id = (int)selectedRow.Cells[0].Value; 

                // ������� ������ �� ���� ������
                _dbContext.DeleteFurniture(id);
                LoadData(); // ��������� ����������� ������
            }
            else
            {
                MessageBox.Show("�������� ������ ��� ��������.", "������", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        /// ���������� ������ ���������� ������
        /// </summary>
        private void btnFilter_Click(object sender, EventArgs e)
        {
            string filterValue = txtFilterValue.Text;
            string filterColumn = comboBoxFilter.SelectedItem.ToString();
            var filteredList = new List<Furniture>();

            try
            {
                // ��������� ������ � ����������� �� ���������� �������
                switch (filterColumn)
                {
                    case "ID":
                        if (checkBoxExactMatch.Checked)
                        {
                            if (int.TryParse(filterValue, out int id))
                            {
                                filteredList = _dbContext.GetAllFurniture()
                                    .Where(f => f.Id == id)
                                    .ToList();
                            }
                        }
                        else
                        {
                            filteredList = _dbContext.GetAllFurniture()
                                .Where(f => f.Id.ToString().Contains(filterValue))
                                .ToList();
                        }
                        break;

                    case "���������":
                        if (checkBoxExactMatch.Checked)
                        {
                            filteredList = _dbContext.GetAllFurniture()
                                .Where(f => f.Category.Equals(filterValue, StringComparison.OrdinalIgnoreCase))
                                .ToList();
                        }
                        else
                        {
                            filteredList = _dbContext.SearchFurnitureByCategory(filterValue);
                        }
                        break;

                    case "���":
                        if (checkBoxExactMatch.Checked)
                        {
                            if (double.TryParse(filterValue, NumberStyles.Any, CultureInfo.InvariantCulture, out double weight))
                            {
                                filteredList = _dbContext.GetAllFurniture()
                                    .Where(f => Math.Abs(f.Weight - weight) < 0.0001)
                                    .ToList();
                            }
                        }
                        else
                        {
                            filteredList = _dbContext.GetAllFurniture()
                                .Where(f => f.Weight.ToString(CultureInfo.InvariantCulture).Contains(filterValue))
                                .ToList();
                        }
                        break;

                    case "����":
                        if (checkBoxExactMatch.Checked)
                        {
                            if (double.TryParse(filterValue, NumberStyles.Any, CultureInfo.InvariantCulture, out double price))
                            {
                                filteredList = _dbContext.GetAllFurniture()
                                    .Where(f => Math.Abs(f.Price - price) < 0.0001)
                                    .ToList();
                            }
                        }
                        else
                        {
                            filteredList = _dbContext.GetAllFurniture()
                                .Where(f => f.Price.ToString(CultureInfo.InvariantCulture).Contains(filterValue))
                                .ToList();
                        }
                        break;
                }

                // ��������� ������� � ���������� ��������������� ������
                dataGridView.Rows.Clear();

                foreach (var furniture in filteredList)
                {
                    dataGridView.Rows.Add(
                        furniture.Id,
                        furniture.Category,
                        furniture.Weight.ToString("F2", CultureInfo.InvariantCulture),
                        furniture.Price.ToString("F2", CultureInfo.InvariantCulture)
                    );
                }

                _filteredItemsCount = filteredList.Count;
                UpdateFilterCount(true, _filteredItemsCount);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"������ ��� ����������: {ex.Message}", "������",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// ���������� ������ ������ ������
        /// </summary>
        private void ButtonSearch_Click(object sender, EventArgs e)
        {
            // �������� ��������� ������
            string filterColumnText = comboBoxFilter.SelectedItem?.ToString();
            string filterValue = txtFilterValue.Text.Trim();

            if (string.IsNullOrEmpty(filterColumnText) || string.IsNullOrEmpty(filterValue))
            {
                MessageBox.Show("�������� ������� ��� ���������� � ������� ��������.", "������", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // ���������� ���������� ���������
                foreach (DataGridViewRow row in dataGridView.Rows)
                {
                    foreach (DataGridViewCell cell in row.Cells)
                    {
                        cell.Style.BackColor = System.Drawing.Color.White;
                    }
                }

                // ���������� ��� ������� ��� ������
                string filterColumnName = null;

                switch (filterColumnText)
                {
                    case "ID":
                        filterColumnName = "Id";
                        break;
                    case "���������":
                        filterColumnName = "Category";
                        break;
                    case "���":
                        filterColumnName = "Weight";
                        break;
                    case "����":
                        filterColumnName = "Price";
                        break;
                    default:
                        MessageBox.Show($"������� '{filterColumnText}' �� ������ � �������.", "������", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                }

                int foundCount = 0; // ������� ��������� ���������

                // ��������� ����� � �������� ����������
                foreach (DataGridViewRow row in dataGridView.Rows)
                {
                    if (row.IsNewRow) continue; // ���������� ����� ������

                    var cellValue = row.Cells[filterColumnName]?.Value?.ToString() ?? string.Empty;
                    bool isMatch = false;

                    if (checkBoxExactMatch.Checked)
                    {
                        // ������ ����������

                        if ((filterColumnText == "���" || filterColumnText == "����") &&
                            double.TryParse(cellValue, NumberStyles.Any, CultureInfo.InvariantCulture, out double cellDouble) &&
                            double.TryParse(filterValue, NumberStyles.Any, CultureInfo.InvariantCulture, out double searchDouble))
                        {
                            // ���������� ����� � ������ ��������
                            isMatch = Math.Abs(cellDouble - searchDouble) < 0.0001;
                        }
                        else
                        {
                            // ��� ��������� ����� (��������, ID ��� Category) ������� ��������� ���������
                            isMatch = string.Equals(cellValue, filterValue, StringComparison.OrdinalIgnoreCase);
                        }
                    }
                    else
                    {
                        // ��������� ����������
                        isMatch = cellValue.IndexOf(filterValue, StringComparison.OrdinalIgnoreCase) >= 0;
                    }

                    // �������� ����������
                    if (isMatch)
                    {
                        foundCount++; // ����������� ������� ��������� ���������
                        foreach (DataGridViewCell cell in row.Cells)
                        {
                            cell.Style.BackColor = System.Drawing.Color.LightGreen;
                        }
                    }
                }

                // ��������� ���������� � ���������� ��������� ���������
                lblFilteredCount.Text = $"������� ���������: {foundCount} �� {_totalItemsCount}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"������ ��� ���������� ������: {ex.Message}", "������", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// ��������� ���������� ���������� � ���������� ��������
        /// </summary>
        private void UpdateSortGlyph()
        {
            foreach (DataGridViewColumn column in dataGridView.Columns)
            {
                column.HeaderCell.SortGlyphDirection = SortOrder.None;
            }

            if (!string.IsNullOrEmpty(_currentSortColumn))
            {
                var sortedColumn = dataGridView.Columns
                    .Cast<DataGridViewColumn>()
                    .FirstOrDefault(c => c.HeaderText == _currentSortColumn);

                if (sortedColumn != null)
                {
                    sortedColumn.HeaderCell.SortGlyphDirection = _currentSortDirection == ListSortDirection.Ascending
                        ? SortOrder.Ascending
                        : SortOrder.Descending;
                }
            }
        }

        /// <summary>
        /// ���������� ������ ������ ��������
        /// </summary>
        private void btnClearFilter_Click(object sender, EventArgs e)
        {
            // ���������� ��������� ����������
            _currentSortColumn = "";
            _currentSortDirection = ListSortDirection.Ascending;

            // ��������� �������� ������
            LoadData();
            UpdateSortGlyph(); // ��������� ���������� ����������
        }

        /// <summary>
        /// ���������� ������ ���� "������� ���� ������"
        /// </summary>
        private void OpenDatabaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "SQLite Database (*.sqlite)|*.sqlite|All Files (*.*)|*.*";
                openFileDialog.Title = "�������� ���� ���� ������";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string selectedFile = openFileDialog.FileName;

                    try
                    {
                        // ������� ����� �������� ���� ������
                        _dbContext = new DatabaseContext($"Data Source={selectedFile};Version=3;");
                        _dbContext.InitializeDatabase(); // ��������� ���������
                        LoadData(); // ��������� ������

                        UpdateDatabaseNameDisplay();

                        // ���������� �������� ����������
                        comboBoxCategory.Enabled = true;
                        txtWeight.Enabled = true;
                        txtPrice.Enabled = true;
                        btnAdd.Enabled = true;
                        btnEdit.Enabled = true;
                        btnDelete.Enabled = true;
                        comboBoxFilter.Enabled = true;
                        txtFilterValue.Enabled = true;
                        btnFilter.Enabled = true;
                        btnClearFilter.Enabled = true;
                        buttonSearch.Enabled = true;
                        deleteDatabaseToolStripMenuItem.Enabled = true;
                        checkBoxExactMatch.Enabled = true;
                    }
                    catch (Exception ex)
                    {
                        // �������� ������
                        string logPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "YourAppName", "error.log");
                        Directory.CreateDirectory(Path.GetDirectoryName(logPath));
                        File.AppendAllText(logPath, $"[{DateTime.Now}] Error: {ex}\n\n");

                        MessageBox.Show($"������: {ex.Message}\n����������� � ����: {logPath}", "������", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        _dbContext = null;
                    }
                }
            }
        }

        /// <summary>
        /// ���������� ������ ���� "������� ���� ������"
        /// </summary>
        private void CreateDatabaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "SQLite Database (*.sqlite)|*.sqlite|All Files (*.*)|*.*";
                saveFileDialog.Title = "�������� ����� ���� ���� ������";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string newDatabasePath = saveFileDialog.FileName;

                    try
                    {
                        // ������� ����� ���� ���� ������
                        File.Create(newDatabasePath).Close();
                        _dbContext = new DatabaseContext($"Data Source={newDatabasePath};Version=3;");
                        _dbContext.InitializeDatabase(); // �������������� ���������
                        LoadData(); // ��������� ������ (������ �������)

                        UpdateDatabaseNameDisplay();

                        // ���������� �������� ����������
                        comboBoxCategory.Enabled = true;
                        txtWeight.Enabled = true;
                        txtPrice.Enabled = true;
                        btnAdd.Enabled = true;
                        btnEdit.Enabled = true;
                        btnDelete.Enabled = true;
                        comboBoxFilter.Enabled = true;
                        txtFilterValue.Enabled = true;
                        btnFilter.Enabled = true;
                        btnClearFilter.Enabled = true;
                        buttonSearch.Enabled = true;
                        deleteDatabaseToolStripMenuItem.Enabled = true;
                        checkBoxExactMatch.Enabled = true;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"������ ��� �������� ���� ������: {ex.Message}", "������", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        _dbContext = null;
                    }
                }
            }
        }

        /// <summary>
        /// ���������� ������ ���� "��������� ���� ������ ���..."
        /// </summary>
        private void SaveAsDatabaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_dbContext == null || string.IsNullOrEmpty(_dbContext.ConnectionString))
            {
                MessageBox.Show("���� ������ �� �������.", "������", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (var saveFileDialog = new SaveFileDialog())
                {
                    saveFileDialog.Filter = "SQLite Database (*.sqlite)|*.sqlite|All Files (*.*)|*.*";
                    saveFileDialog.Title = "��������� ���� ������ ���";

                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        string newDatabasePath = saveFileDialog.FileName;

                        // �������� ���� � ������� ���� ������
                        var connectionStringBuilder = new SQLiteConnectionStringBuilder(_dbContext.ConnectionString);
                        string currentDatabasePath = connectionStringBuilder.DataSource;

                        // ��������� �����������
                        _dbContext.Dispose();
                        _dbContext = null;
                        SQLiteConnection.ClearAllPools();

                        // ������� �������
                        GC.Collect();
                        GC.WaitForPendingFinalizers();
                        System.Threading.Thread.Sleep(500);

                        // ��������� ���������� �����
                        if (IsFileLocked(currentDatabasePath, out string lockingProcessName))
                        {
                            MessageBox.Show($"���� ���� ������ ������������ ���������: {lockingProcessName}.", "������", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        // �������� ���� ������
                        File.Copy(currentDatabasePath, newDatabasePath, overwrite: true);

                        // ������� ������ ����, ���� ���������
                        if (File.Exists(currentDatabasePath))
                        {
                            File.Delete(currentDatabasePath);
                        }

                        // ��������������� ����������� � ����� ����
                        _dbContext = new DatabaseContext($"Data Source={newDatabasePath};Version=3;");
                        LoadData();

                        MessageBox.Show("���� ������ ������� ���������.", "�����", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"������ ��� ���������� ���� ������: {ex.Message}", "������", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// ���������� ������ ���� "��������� ������� ��� PDF"
        /// </summary>
        private void SaveTableAsPDFToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dataGridView.Rows.Count == 0)
            {
                MessageBox.Show("������� �����. ������ ���������.", "������", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (var saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "PDF Files (*.pdf)|*.pdf|All Files (*.*)|*.*";
                saveFileDialog.Title = "��������� ������� ��� PDF";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string pdfFilePath = saveFileDialog.FileName;

                    if (string.IsNullOrEmpty(pdfFilePath) || Path.GetInvalidPathChars().Any(pdfFilePath.Contains))
                    {
                        MessageBox.Show("������������ ���� � �����.", "������", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    try
                    {
                        if (File.Exists(pdfFilePath))
                        {
                            File.Delete(pdfFilePath);
                        }

                        QuestPDF.Settings.License = LicenseType.Community;

                        string dbName = "����������� ���� ������";
                        if (_dbContext != null && !string.IsNullOrEmpty(_dbContext.ConnectionString))
                        {
                            var builder = new SQLiteConnectionStringBuilder(_dbContext.ConnectionString);
                            dbName = Path.GetFileNameWithoutExtension(builder.DataSource) ?? dbName;
                        }

                        Document.Create(container =>
                        {
                            container.Page(page =>
                            {
                                page.Size(PageSizes.A4);
                                page.Margin(2, Unit.Centimetre);
                                page.PageColor(Colors.White);
                                page.DefaultTextStyle(x => x.FontSize(12));

                                page.Content().Column(column =>
                                {
                                    // ����� ��������� ��� ���������
                                    column.Item().PaddingBottom(10).AlignCenter().Text(text =>
                                    {
                                        text.Span($"������� �� ���� ������: {dbName}")
                                           .FontSize(20)
                                           .Bold();
                                    });

                                    column.Item().Border(1).Table(table =>
                                    {
                                        table.ColumnsDefinition(columns =>
                                        {
                                            foreach (DataGridViewColumn column in dataGridView.Columns)
                                            {
                                                columns.RelativeColumn();
                                            }
                                        });

                                        var headerStyle = TextStyle.Default.Bold();
                                        var cellStyle = TextStyle.Default;

                                        table.Header(header =>
                                        {
                                            foreach (DataGridViewColumn column in dataGridView.Columns)
                                            {
                                                // ����� ��������� ��� ���������� ��������
                                                header.Cell()
                                                    .Border(1)
                                                    .Background(Colors.Grey.Lighten3)
                                                    .Padding(5)
                                                    .AlignCenter()
                                                    .Text(text =>
                                                    {
                                                        text.Span(column.HeaderText)
                                                           .Style(headerStyle);
                                                    });
                                            }
                                        });

                                        foreach (DataGridViewRow row in dataGridView.Rows)
                                        {
                                            if (row.IsNewRow) continue;

                                            foreach (DataGridViewCell cell in row.Cells)
                                            {
                                                string cellValue = cell.Value?.ToString() ?? "";
                                                // ����� ��������� ��� ����� �������
                                                table.Cell()
                                                    .Border(1)
                                                    .Padding(5)
                                                    .AlignCenter()
                                                    .Text(text =>
                                                    {
                                                        text.Span(cellValue)
                                                           .Style(cellStyle);
                                                    });
                                            }
                                        }
                                    });
                                });
                            });
                        }).GeneratePdf(pdfFilePath);

                        MessageBox.Show("������� ������� ��������� � PDF.", "�����", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"������ ��� �������� PDF: {ex.Message}", "������", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        /// <summary>
        /// ���������� ������ ���� "������� ���� ������"
        /// </summary>
        private void DeleteDatabaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (_dbContext == null || string.IsNullOrEmpty(_dbContext.ConnectionString))
                {
                    MessageBox.Show("���� ������ �� �������.", "������", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // �������� ���� � ���� ������
                var connectionStringBuilder = new SQLiteConnectionStringBuilder(_dbContext.ConnectionString);
                string databasePath = connectionStringBuilder.DataSource;

                if (string.IsNullOrEmpty(databasePath) || !File.Exists(databasePath))
                {
                    MessageBox.Show("���� ���� ������ �� ������.", "������", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // ����������� ������������� ��������
                if (MessageBox.Show("�� �������, ��� ������ ������� ������� ���� ������?",
                    "�������������", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    try
                    {
                        // ��������� �����������
                        _dbContext.Dispose();
                        _dbContext = null;
                        SQLiteConnection.ClearAllPools();

                        // ������� �������
                        GC.Collect();
                        GC.WaitForPendingFinalizers();
                        System.Threading.Thread.Sleep(500);

                        // ��������� ���������� �����
                        if (IsFileLocked(databasePath, out string lockingProcessName) && !string.IsNullOrEmpty(lockingProcessName))
                        {
                            MessageBox.Show($"���� ���� ������ ������������ ���������: {lockingProcessName}.",
                                "������", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        // ���������� ���� � ������� ������ ��������
                        Microsoft.VisualBasic.FileIO.FileSystem.DeleteFile(
                            databasePath,
                            Microsoft.VisualBasic.FileIO.UIOption.OnlyErrorDialogs,
                            Microsoft.VisualBasic.FileIO.RecycleOption.SendToRecycleBin);

                        // ������� ���������
                        dataGridView.Rows.Clear();

                        // ��������� ��������� ����
                        deleteDatabaseToolStripMenuItem.Enabled = false;
                        openDatabaseToolStripMenuItem.Enabled = true;
                        createDatabaseToolStripMenuItem.Enabled = true;
                        UpdateDatabaseNameDisplay();

                        MessageBox.Show("���� ������ ���������� � �������.",
                            "�����", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (IOException ex)
                    {
                        MessageBox.Show($"�� ������� ������� ���� ���� ������: {ex.Message}",
                            "������", MessageBoxButtons.OK, MessageBoxIcon.Error);

                        if (!string.IsNullOrEmpty(databasePath))
                        {
                            _dbContext = new DatabaseContext($"Data Source={databasePath};Version=3;");
                            LoadData();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"������ ��� �������� ���� ������: {ex.Message}",
                    "������", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// ���������, ������������ �� ���� ������ ���������
        /// </summary>
        /// <param name="filePath">���� � ������������ �����</param>
        /// <param name="lockingProcessName">��� ��������, ������������ ���� (���� ����)</param>
        /// <returns>True, ���� ���� ������������, ����� False</returns>
        private bool IsFileLocked(string filePath, out string lockingProcessName) 
        {
            lockingProcessName = null;

            if (string.IsNullOrEmpty(filePath)) return false;

            try
            {
                using (FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                {
                    stream.Close();
                }
            }
            catch (IOException)
            {
                Process[] processes = Process.GetProcesses();
                foreach (Process process in processes)
                {
                    try
                    {
                        if (process.MainWindowHandle != IntPtr.Zero && !string.IsNullOrEmpty(process.MainWindowTitle))
                        {
                            foreach (ProcessModule module in process.Modules)
                            {
                                if (module.FileName?.Equals(filePath, StringComparison.OrdinalIgnoreCase) == true)
                                {
                                    lockingProcessName = process.ProcessName;
                                    return true;
                                }
                            }
                        }
                    }
                    catch
                    {
                        continue;
                    }
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// ��������� ������ � DataGridView �� ���������� ������� � ������ �������� ����������� ����������.
        /// </summary>
        /// <param name="columnHeaderText">�������� �������, �� �������� ����������� ���������� (������ ��������������� ������ �������� DataGridView).</param>
        /// <exception cref="ArgumentException">�������������, ���� ���������� ��� ������� �� ������������� ��������� ���������.</exception>

        private void SortData(string columnHeaderText)
        {
            if (_dbContext == null) return;

            try
            {
                // ���������� ����������� ����������
                if (_currentSortColumn == columnHeaderText)
                {
                    _currentSortDirection = _currentSortDirection == ListSortDirection.Ascending
                        ? ListSortDirection.Descending
                        : ListSortDirection.Ascending;
                }
                else
                {
                    _currentSortColumn = columnHeaderText;
                    _currentSortDirection = ListSortDirection.Ascending;
                }

                // �������� ��� ������
                var furnitureList = _dbContext.GetAllFurniture();

                // ��������� � ����������� �� �������
                switch (columnHeaderText)
                {
                    case "Id":
                        furnitureList = _currentSortDirection == ListSortDirection.Ascending
                            ? furnitureList.OrderBy(f => f.Id).ToList()
                            : furnitureList.OrderByDescending(f => f.Id).ToList();
                        break;

                    case "���������":
                        furnitureList = _currentSortDirection == ListSortDirection.Ascending
                            ? furnitureList.OrderBy(f => f.Category).ToList()
                            : furnitureList.OrderByDescending(f => f.Category).ToList();
                        break;

                    case "��� (��)":
                        furnitureList = _currentSortDirection == ListSortDirection.Ascending
                            ? furnitureList.OrderBy(f => f.Weight).ToList()
                            : furnitureList.OrderByDescending(f => f.Weight).ToList();
                        break;

                    case "���� (���)":
                        furnitureList = _currentSortDirection == ListSortDirection.Ascending
                            ? furnitureList.OrderBy(f => f.Price).ToList()
                            : furnitureList.OrderByDescending(f => f.Price).ToList();
                        break;

                    default:
                        throw new ArgumentException("�������� ��� �������");
                }

                // ��������� DataGridView
                dataGridView.Rows.Clear();
                foreach (var furniture in furnitureList)
                {
                    dataGridView.Rows.Add(
                        furniture.Id,
                        furniture.Category,
                        furniture.Weight.ToString("F2", CultureInfo.InvariantCulture),
                        furniture.Price.ToString("F2", CultureInfo.InvariantCulture)
                    );
                }

                UpdateSortGlyph();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"������ ��� ����������: {ex.Message}", "������",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// ������������ ���� �� ��������� ������� DataGridView � ��������� ������ �� ���������� �������.
        /// </summary>
        /// <param name="sender">������, ��������� ������� (DataGridView).</param>
        /// <param name="e">��������� �������, ���������� ���������� � ����� (������ ����, ������ �������).</param>
        /// <remarks>
        /// ���������� ����������� ������ ��� ����� ����� ������� ����.
        /// </remarks>
        private void dataGridView_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                string columnHeaderText = dataGridView.Columns[e.ColumnIndex].HeaderText;
                SortData(columnHeaderText);
            }
        }

        /// <summary>
        /// ��������� ����������� ����� ������� ���� ������ � �������� ���������� Label.
        /// </summary>
        /// <remarks>
        /// ���� ����������� � �� ����������� ��� �� �����������, ������������ "������� ��: �� �������".
        /// ��� SQLite ��� �� ����������� �� ���� � ConnectionString.
        /// </remarks>
        private void UpdateDatabaseNameDisplay()
        {
            if (_dbContext == null || string.IsNullOrEmpty(_dbContext.ConnectionString))
            {
                lblDatabaseName.Text = "������� ��: �� �������";
                return;
            }

            var builder = new SQLiteConnectionStringBuilder(_dbContext.ConnectionString);
            string dbName = Path.GetFileName(builder.DataSource);
            lblDatabaseName.Text = $"������� ��: {dbName}";
        }
    }
}