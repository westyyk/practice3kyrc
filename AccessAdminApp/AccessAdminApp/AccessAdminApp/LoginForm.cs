using System;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Windows.Forms;

namespace AccessAdminApp
{
    public partial class LoginForm : Form
    {
        private const string ConnectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=E:\123455\db.accdb;Persist Security Info=False;";

        private TextBox txtFirstName, txtLastName, txtUsername, txtPassword;
        private Button btnLogin, btnRegister;

        public LoginForm()
        {
            InitializeForm();
        }

        private void InitializeForm()
        {
            this.Text = "Авторизация";
            this.Size = new Size(420, 330);
            this.StartPosition = FormStartPosition.CenterScreen;

            Label lblInfo = new Label
            {
                Text = "Вход для администратора или ученика",
                Location = new Point(50, 20),
                AutoSize = true
            };
            Controls.Add(lblInfo);

            Label lblFirst = new Label { Text = "Имя:", Location = new Point(50, 60), AutoSize = true };
            Label lblLast = new Label { Text = "Фамилия:", Location = new Point(50, 90), AutoSize = true };
            Label lblUser = new Label { Text = "Логин (для админа):", Location = new Point(50, 140), AutoSize = true };
            Label lblPass = new Label { Text = "Пароль (для админа):", Location = new Point(50, 170), AutoSize = true };

            txtFirstName = new TextBox { Location = new Point(180, 60), Width = 160 };
            txtLastName = new TextBox { Location = new Point(180, 90), Width = 160 };
            txtUsername = new TextBox { Location = new Point(180, 140), Width = 160 };
            txtPassword = new TextBox { Location = new Point(180, 170), Width = 160, UseSystemPasswordChar = true };

            btnLogin = new Button { Text = "Войти", Location = new Point(50, 220), Width = 120 };
            btnRegister = new Button { Text = "Регистрация (админ)", Location = new Point(190, 220), Width = 150 };

            btnLogin.Click += BtnLogin_Click;
            btnRegister.Click += BtnRegister_Click;

            Controls.AddRange(new Control[] {
                lblFirst, lblLast, lblUser, lblPass,
                txtFirstName, txtLastName, txtUsername, txtPassword,
                btnLogin, btnRegister
            });
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            string firstName = txtFirstName.Text.Trim();
            string lastName = txtLastName.Text.Trim();
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text.Trim();

            // 1️⃣ Проверяем как ученика
            if (!string.IsNullOrEmpty(firstName) && !string.IsNullOrEmpty(lastName))
            {
                using (OleDbConnection conn = new OleDbConnection(ConnectionString))
                {
                    try
                    {
                        conn.Open();
                        string query = "SELECT StudentID FROM Students WHERE FirstName=@f AND LastName=@l";
                        OleDbCommand cmd = new OleDbCommand(query, conn);
                        cmd.Parameters.AddWithValue("@f", firstName);
                        cmd.Parameters.AddWithValue("@l", lastName);

                        object result = cmd.ExecuteScalar();
                        if (result != null)
                        {
                            int studentId = Convert.ToInt32(result);
                            this.Hide();
                            new MainForm(studentId, firstName, lastName).Show();
                            return;
                        }
                        else
                        {
                            MessageBox.Show("Ученик не найден!");
                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ошибка: " + ex.Message);
                        return;
                    }
                }
            }

            // 2️⃣ Проверяем как администратора
            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
            {
                using (OleDbConnection conn = new OleDbConnection(ConnectionString))
                {
                    try
                    {
                        conn.Open();
                        string query = "SELECT Role FROM Users WHERE Username=@u AND [Password]=@p";
                        OleDbCommand cmd = new OleDbCommand(query, conn);
                        cmd.Parameters.AddWithValue("@u", username);
                        cmd.Parameters.AddWithValue("@p", password);

                        object roleObj = cmd.ExecuteScalar();

                        if (roleObj != null && roleObj.ToString() == "admin")
                        {
                            this.Hide();
                            new AdminForm(username, "admin").Show();
                            return;
                        }
                        else
                        {
                            MessageBox.Show("Неверный логин или пароль администратора!");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ошибка: " + ex.Message);
                    }
                }
            }
            else
            {
                MessageBox.Show("Введите данные для входа!");
            }
        }

        private void BtnRegister_Click(object sender, EventArgs e)
        {
            this.Hide();
            new RegisterForm().Show();
        }
    }
}
