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
    public partial class Login : Form
    {
        private string connectionString = "Data Source=DESKTOP-JNH7B7M\\MANNANTA;Initial Catalog=BookingStudio;Integrated Security=True";
        public Login()
        {
            InitializeComponent();
        }

        private void Login_Load(object sender, EventArgs e)
        {
            txtEmail.Clear();
            txtPassword.Clear();

            txtEmail.Focus();

            txtPassword.PasswordChar = '*';
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            string email = txtEmail.Text.Trim();
            string password = txtPassword.Text.Trim(); // Bisa dihapus jika tidak memerlukan password

            // Validasi email
            if (string.IsNullOrWhiteSpace(email))
            {
                MessageBox.Show("Email tidak boleh kosong!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Menggunakan connection string yang sudah ditentukan
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    // Query untuk validasi email dan password
                    string query = "SELECT * FROM Pelanggan WHERE Email = @Email AND Password = @Password"; // Pastikan kolom Password ada
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@Password", password); // Hapus kalau tidak pakai password

                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        int pelangganID = Convert.ToInt32(reader["PelangganID"]);

                        // Mengarahkan ke halaman Edit Profil setelah login berhasil
                        Pelanggan Form1 = new Pelanggan(pelangganID); // Pastikan form EditProfil sudah dibuat
                        Form1.Show();
                        this.Hide(); // Menyembunyikan form login setelah berhasil login
                    }
                    else
                    {
                        MessageBox.Show("Email atau password salah!", "Login Gagal", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Terjadi kesalahan: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

        }
    }
}
