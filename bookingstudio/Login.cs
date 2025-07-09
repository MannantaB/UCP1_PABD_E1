using bookingstudio;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace bookingstudio
{
    public partial class Login : Form
    {
        koneksi kn = new koneksi();
        public Login()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ClearForm();
            txtPassword.PasswordChar = '*';
        }

        private void ClearForm()
        {
            txtEmail.Clear();
            txtPassword.Clear();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string email = txtEmail.Text.Trim();
            string password = txtPassword.Text.Trim();

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Email dan Password tidak boleh kosong!");
                return;
            }

            using (SqlConnection conn = new SqlConnection(kn.ConnectionString()))
            {
                try
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("spLoginUser", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@Password", password);

                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        // ✅ Ambil data dari hasil query login
                        SessionUser.PelangganID = reader.GetInt32(0);         // PelangganID
                        SessionUser.Nama = reader["Nama"].ToString();        // Nama
                        SessionUser.Email = reader["Email"].ToString();      // Email

                        // ✅ Buka form utama
                        main mainForm = new main();
                        mainForm.Show();
                        this.Hide();
                    }
                    else
                    {
                        MessageBox.Show("Email atau Password salah.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Terjadi kesalahan: " + ex.Message);
                }
            }
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            Register formRegistrasi = new Register();
            formRegistrasi.Show();
            this.Hide();
        }

        private void AnalyzeQuery(string sqlQuery)
        {
            using (var conn = new SqlConnection(kn.ConnectionString()))
            {
                conn.InfoMessage += (s, e) => MessageBox.Show(e.Message, "STATISTICS INFO");
                conn.Open();

                var wrapped = $@"
                    SET STATISTICS IO ON;
                    SET STATISTICS TIME ON;
                    {sqlQuery};
                    SET STATISTICS IO OFF;
                    SET STATISTICS TIME OFF;";

                using (var cmd = new SqlCommand(wrapped, conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void btnAnalyze_Click_1(object sender, EventArgs e)
        {
            string query = "SELECT PelangganID FROM Pelanggan WHERE Email = 'user@example.com'";
            AnalyzeQuery(query);
        }
    }
}
