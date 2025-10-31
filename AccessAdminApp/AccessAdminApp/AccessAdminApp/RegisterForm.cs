using System;
using System.Data.OleDb;
using System.Drawing;
using System.Windows.Forms;

namespace AccessAdminApp
{
    public partial class RegisterForm : Form
    {
        private const string ConnectionString =
            @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=E:\123455\db.accdb;Persist Security Info=False;";

        private TextBox txtUsername, txtPassword;
        private Button btnRegister, btnBack;

        public RegisterForm()
        {
            InitializeForm();
        }

        private void InitializeForm()
        {
            this.Text = "Регистрация";
            this.Size = new Size(400, 250);
            this.StartPosition = FormStartPosition.CenterScreen;

            Label lblUser = new Label { Text = "Логин:", Location = new Point(50, 50), AutoSize = true };
            Label lblPass = new Label { Text = "Пароль:", Location = new Point(50, 90), AutoSize = true };

            txtUsername = new TextBox { Location = new Point(150, 50), Width = 180 };
            txtPassword = new TextBox { Location = new Point(150, 90), Width = 180, UseSystemPasswordChar = true };

            btnRegister = new Button { Text = "Зарегистрироваться", Location = new Point(50, 140), Width = 130 };
            btnBack = new Button { Text = "Назад", Location = new Point(210, 140), Width = 120 };

            btnRegister.Click += BtnRegister_Click;
            btnBack.Click += BtnBack_Click;

            Controls.AddRange(new Control[] { lblUser, lblPass, txtUsername, txtPassword, btnRegister, btnBack });
        }

        private void BtnRegister_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text.Trim();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Введите логин и пароль!");
                return;
            }

            using (OleDbConnection conn = new OleDbConnection(ConnectionString))
            {
                try
                {
                    conn.Open();
                    string query = "INSERT INTO Users (Username, [Password], Role) VALUES (@u, @p, 'user')";
                    OleDbCommand cmd = new OleDbCommand(query, conn);
                    cmd.Parameters.AddWithValue("@u", username);
                    cmd.Parameters.AddWithValue("@p", password);
                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Регистрация прошла успешно!");
                    this.Hide();
                    new LoginForm().Show();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка: " + ex.Message);
                }
            }
        }

        private void BtnBack_Click(object sender, EventArgs e)
        {
            this.Hide();
            new LoginForm().Show();
        }
    }
}
