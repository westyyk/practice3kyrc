using System;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Windows.Forms;

namespace AccessAdminApp
{
    public partial class MainForm : Form
    {
        private const string ConnectionString =
            @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=E:\123455\db.accdb;Persist Security Info=False;";

        private OleDbConnection connection;
        private DataGridView dataGrid;
        private Button btnLogout;
        private int studentId;
        private string studentName;

        public MainForm(int studentId, string firstName, string lastName)
        {
            this.studentId = studentId;
            this.studentName = $"{lastName} {firstName}";
            BuildInterface();
            connection = new OleDbConnection(ConnectionString);
            LoadGrades();
        }

        private void BuildInterface()
        {
            this.Text = $"Оценки ученика: {studentName}";
            this.Size = new Size(900, 600);
            this.StartPosition = FormStartPosition.CenterScreen;

            btnLogout = new Button { Text = "Выйти", Location = new Point(20, 20), Width = 100 };
            btnLogout.Click += BtnLogout_Click;
            this.Controls.Add(btnLogout);

            dataGrid = new DataGridView
            {
                Location = new Point(20, 60),
                Size = new Size(840, 480),
                ReadOnly = true,
                AllowUserToAddRows = false
            };
            this.Controls.Add(dataGrid);
        }

        private void LoadGrades()
        {
            try
            {
                connection.Open();
                string query = @"
                    SELECT s.SubjectName AS [Предмет],
                           g.Grade AS [Оценка],
                           g.GradeDate AS [Дата]
                    FROM Grades g
                    INNER JOIN Subjects s ON g.SubjectID = s.SubjectID
                    WHERE g.StudentID = @sid";

                OleDbCommand cmd = new OleDbCommand(query, connection);
                cmd.Parameters.AddWithValue("@sid", studentId);

                OleDbDataAdapter adapter = new OleDbDataAdapter(cmd);
                DataTable table = new DataTable();
                adapter.Fill(table);

                dataGrid.DataSource = table;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки оценок: " + ex.Message);
            }
            finally
            {
                connection.Close();
            }
        }

        private void BtnLogout_Click(object sender, EventArgs e)
        {
            this.Hide();
            new LoginForm().Show();
        }
    }
}
