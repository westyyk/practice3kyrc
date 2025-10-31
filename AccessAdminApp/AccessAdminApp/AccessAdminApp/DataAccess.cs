using System;
using System.Data;
using System.Data.OleDb;
using System.Windows.Forms;

namespace AccessAdminApp
{
    public static class DataAccess
    {
        private const string ConnectionString =
            @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=E:\123455\db.accdb;Persist Security Info=False;";

        public static bool TestConnection()
        {
            using (OleDbConnection conn = new OleDbConnection(ConnectionString))
            {
                try
                {
                    conn.Open();
                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка подключения: " + ex.Message);
                    return false;
                }
            }
        }

        public static string VerifyUser(string username, string password)
        {
            using (OleDbConnection conn = new OleDbConnection(ConnectionString))
            {
                conn.Open();
                string sql = "SELECT Role FROM Users WHERE Username=@u AND [Password]=@p";
                using (OleDbCommand cmd = new OleDbCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@u", username);
                    cmd.Parameters.AddWithValue("@p", password);
                    object result = cmd.ExecuteScalar();
                    return result?.ToString();
                }
            }
        }

        public static bool RegisterUser(string username, string password, string role)
        {
            using (OleDbConnection conn = new OleDbConnection(ConnectionString))
            {
                conn.Open();
                string checkSql = "SELECT COUNT(*) FROM Users WHERE Username=@u";
                using (OleDbCommand checkCmd = new OleDbCommand(checkSql, conn))
                {
                    checkCmd.Parameters.AddWithValue("@u", username);
                    int count = (int)checkCmd.ExecuteScalar();
                    if (count > 0)
                    {
                        MessageBox.Show("Такой пользователь уже существует!");
                        return false;
                    }
                }

                string sql = "INSERT INTO Users (Username, [Password], Role) VALUES (@u, @p, @r)";
                using (OleDbCommand cmd = new OleDbCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@u", username);
                    cmd.Parameters.AddWithValue("@p", password);
                    cmd.Parameters.AddWithValue("@r", role);
                    cmd.ExecuteNonQuery();
                    return true;
                }
            }
        }

        public static DataTable GetTable(string tableName)
        {
            using (OleDbConnection conn = new OleDbConnection(ConnectionString))
            {
                conn.Open();
                using (OleDbDataAdapter adapter = new OleDbDataAdapter($"SELECT * FROM [{tableName}]", conn))
                {
                    DataTable table = new DataTable();
                    adapter.Fill(table);
                    return table;
                }
            }
        }

        public static void SaveTable(DataTable table, string tableName)
        {
            using (OleDbConnection conn = new OleDbConnection(ConnectionString))
            {
                conn.Open();
                using (OleDbDataAdapter adapter = new OleDbDataAdapter($"SELECT * FROM [{tableName}]", conn))
                {
                    OleDbCommandBuilder builder = new OleDbCommandBuilder(adapter);
                    adapter.Update(table);
                }
            }
        }

        public static DataTable GetStudentGrades(int studentId)
        {
            using (OleDbConnection conn = new OleDbConnection(ConnectionString))
            {
                conn.Open();
                string sql = @"
            SELECT s.SubjectName AS [Предмет],
                   g.Grade AS [Оценка],
                   g.GradeDate AS [Дата]
            FROM Grades g
            INNER JOIN Subjects s ON g.SubjectID = s.SubjectID
            WHERE g.StudentID = @id";
                using (OleDbCommand cmd = new OleDbCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", studentId);
                    OleDbDataAdapter adapter = new OleDbDataAdapter(cmd);
                    DataTable table = new DataTable();
                    adapter.Fill(table);
                    return table;
                }
            }
        }

    }
}
