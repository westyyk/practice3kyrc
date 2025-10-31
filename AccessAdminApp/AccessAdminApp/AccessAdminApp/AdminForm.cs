using System;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;

namespace AccessAdminApp
{
    public partial class AdminForm : Form
    {
        private const string ConnectionString =
            @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=E:\123455\db.accdb;Persist Security Info=False;";

        private DataGridView dataGrid;
        private ComboBox tableSelector;
        private Button btnSave, btnAdd, btnDelete, btnLogout;

        public AdminForm(string username, string role)
        {
            InitializeComponentFixed();
        }

        private void InitializeComponentFixed()
        {
            this.Text = "Администратор — управление базой данных";
            this.Size = new Size(900, 600);
            this.StartPosition = FormStartPosition.CenterScreen;

            tableSelector = new ComboBox();
            tableSelector.Items.AddRange(new string[] { "Grades", "Students", "Subjects", "Users" });
            tableSelector.Location = new Point(20, 20);
            tableSelector.Width = 200;
            tableSelector.SelectedIndexChanged += TableSelector_SelectedIndexChanged;
            this.Controls.Add(tableSelector);

            btnSave = new Button { Text = "Сохранить", Location = new Point(240, 20) };
            btnSave.Click += BtnSave_Click;
            this.Controls.Add(btnSave);

            btnAdd = new Button { Text = "Добавить", Location = new Point(340, 20) };
            btnAdd.Click += BtnAdd_Click;
            this.Controls.Add(btnAdd);

            btnDelete = new Button { Text = "Удалить", Location = new Point(440, 20) };
            btnDelete.Click += BtnDelete_Click;
            this.Controls.Add(btnDelete);

            btnLogout = new Button { Text = "Выйти", Location = new Point(780, 20) };
            btnLogout.Click += BtnLogout_Click;
            this.Controls.Add(btnLogout);

            dataGrid = new DataGridView
            {
                Location = new Point(20, 60),
                Size = new Size(840, 480),
                AllowUserToAddRows = false
            };
            this.Controls.Add(dataGrid);
        }

        private void TableSelector_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tableSelector.SelectedItem != null)
                LoadTable(tableSelector.SelectedItem.ToString());
        }

        private void LoadTable(string tableName)
        {
            using (OleDbConnection connection = new OleDbConnection(ConnectionString))
            {
                try
                {
                    connection.Open();
                    string query = $"SELECT * FROM [{tableName}]";
                    OleDbDataAdapter adapter = new OleDbDataAdapter(query, connection);
                    DataTable table = new DataTable();
                    adapter.Fill(table);
                    dataGrid.DataSource = table;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка загрузки: " + ex.Message);
                }
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (tableSelector.SelectedItem == null)
            {
                MessageBox.Show("Выберите таблицу!");
                return;
            }

            string tableName = tableSelector.SelectedItem.ToString();

            DataTable table = dataGrid.DataSource as DataTable;
            if (table == null || table.Rows.Count == 0)
            {
                MessageBox.Show("Нет данных для сохранения!");
                return;
            }

            using (OleDbConnection connection = new OleDbConnection(ConnectionString))
            {
                try
                {
                    connection.Open();

                    string selectQuery = $"SELECT * FROM [{tableName}]";
                    OleDbDataAdapter adapter = new OleDbDataAdapter(selectQuery, connection);
                    OleDbCommandBuilder builder = new OleDbCommandBuilder(adapter);

                    builder.QuotePrefix = "[";
                    builder.QuoteSuffix = "]";

                    adapter.InsertCommand = builder.GetInsertCommand();
                    adapter.UpdateCommand = builder.GetUpdateCommand();
                    adapter.DeleteCommand = builder.GetDeleteCommand();

                    adapter.Update(table);

                    MessageBox.Show("Изменения успешно сохранены!");
                    LoadTable(tableName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка сохранения: " + ex.Message);
                }
            }
        }



        private void InsertRow(OleDbConnection connection, string tableName, DataRow row)
        {
            var columns = new System.Collections.Generic.List<string>();
            var values = new System.Collections.Generic.List<string>();
            var parameters = new System.Collections.Generic.List<OleDbParameter>();

            foreach (DataColumn column in row.Table.Columns)
            {
                // Пропускаем автоинкрементные поля
                if (!column.AutoIncrement && row[column] != DBNull.Value && !string.IsNullOrEmpty(row[column].ToString()))
                {
                    columns.Add($"[{column.ColumnName}]");

                    object value = row[column];
                    if (value is string || value is DateTime)
                    {
                        values.Add("?");
                        parameters.Add(new OleDbParameter($"@{column.ColumnName}", value));
                    }
                    else
                    {
                        values.Add(value.ToString());
                    }
                }
            }

            if (columns.Count > 0)
            {
                string insertSql = $"INSERT INTO [{tableName}] ({string.Join(", ", columns)}) VALUES ({string.Join(", ", values)})";

                using (OleDbCommand cmd = new OleDbCommand(insertSql, connection))
                {
                    // Добавляем параметры если есть
                    foreach (var param in parameters)
                    {
                        cmd.Parameters.Add(param);
                    }

                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            if (tableSelector.SelectedItem == null)
            {
                MessageBox.Show("Сначала выберите таблицу!");
                return;
            }

            if (dataGrid.DataSource is DataTable table)
            {
                DataRow newRow = table.NewRow();
                table.Rows.Add(newRow);
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (dataGrid.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите строки для удаления!");
                return;
            }

            if (MessageBox.Show("Удалить выбранные строки?", "Подтверждение",
                MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                foreach (DataGridViewRow row in dataGrid.SelectedRows)
                {
                    if (!row.IsNewRow)
                        dataGrid.Rows.Remove(row);
                }
            }
        }

        private void BtnLogout_Click(object sender, EventArgs e)
        {
            this.Hide();
            new LoginForm().Show();
        }
    }
}