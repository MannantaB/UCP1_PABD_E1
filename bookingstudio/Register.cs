using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace bookingstudio
{
    public partial class Register : Form
    {
        koneksi kn = new koneksi();
        public Register()
        {
            InitializeComponent();
        }

        private void btnDaftar_Click(object sender, EventArgs e)
        {
            string nama = txtNama.Text.Trim();
            string email = txtEmail.Text.Trim();
            string password = txtPassword.Text.Trim();
            string telepon = txtTelepon.Text.Trim();

            if (string.IsNullOrEmpty(nama) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(telepon))
            {
                MessageBox.Show("Semua field wajib diisi!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (password.Length < 8)
            {
                MessageBox.Show("Password minimal 8 karakter!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!password.Any(c => "%[!@#$%^&*()]%".Contains(c)))
            {
                MessageBox.Show("Password harus mengandung minimal 1 karakter spesial (!@#$%^&*())", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SqlConnection conn = new SqlConnection(kn.ConnectionString()))
            {
                try
                {
                    conn.Open();

                    string cekEmail = "SELECT COUNT(*) FROM Pelanggan WHERE Email = @Email";
                    SqlCommand cekCmd = new SqlCommand(cekEmail, conn);
                    cekCmd.Parameters.AddWithValue("@Email", email);

                    int count = (int)cekCmd.ExecuteScalar();
                    if (count > 0)
                    {
                        MessageBox.Show("Email sudah terdaftar!", "Gagal", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // Panggil Stored Procedure spAddRegister
                    SqlCommand cmd = new SqlCommand("spAddRegister", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Nama", nama);
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@Password", password);
                    cmd.Parameters.AddWithValue("@Telepon", telepon);

                    int result = cmd.ExecuteNonQuery();

                    if (result > 0)
                    {
                        MessageBox.Show("Pendaftaran berhasil!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        // Reset form
                        txtNama.Clear();
                        txtEmail.Clear();
                        txtPassword.Clear();
                        txtTelepon.Clear();
                    }
                    else
                    {
                        MessageBox.Show("Pendaftaran gagal!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Terjadi kesalahan: " + ex.Message, "Kesalahan", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }


        private void button1_Click(object sender, EventArgs e)
        {
            Login mainLogin = new Login();
            mainLogin.Show();
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
            string query = "SELECT COUNT(*) FROM Pelanggan WHERE Email = 'user@example.com'";
            AnalyzeQuery(query);
        }
    }
}
