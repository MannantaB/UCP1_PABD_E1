using bookingstudio;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Caching;
using System.Windows.Forms;

namespace bookingstudio
{
    public partial class profil : Form
    {
        private string connectionString = "Data Source=DESKTOP-JNH7B7M\\MANNANTA;Initial Catalog=BookingStudio;Integrated Security=True";

        // 🔥 Inisialisasi cache
        private readonly MemoryCache _cache = MemoryCache.Default;
        private string CacheKey => $"Profil_{SessionUser.PelangganID}";
        private readonly CacheItemPolicy _policy = new CacheItemPolicy { AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(5) };

        private main _mainForm;

        public profil(main mainForm)
        {
            InitializeComponent();
            _mainForm = mainForm;
        }

        private void profil_Load(object sender, EventArgs e)
        {
            LoadDataUser();
        }

        private void ClearForm()
        {
            txtNama.Clear();
            txtEmail.Clear();
            txtPassword.Clear();
            txtTelepon.Clear();
            txtNama.Focus();
        }

        private void LoadDataUser()
        {
            DataTable dt;

            if (_cache.Contains(CacheKey))
            {
                dt = _cache.Get(CacheKey) as DataTable;
            }
            else
            {
                dt = new DataTable();
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    try
                    {
                        conn.Open();
                        SqlCommand cmd = new SqlCommand("SELECT Nama, Email, Password, Telepon FROM Pelanggan WHERE ID = @ID", conn);
                        cmd.Parameters.AddWithValue("@ID", SessionUser.PelangganID);
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        da.Fill(dt);

                        // Simpan ke cache
                        _cache.Set(CacheKey, dt, _policy);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Gagal memuat data user: " + ex.Message);
                        return;
                    }
                }
            }

            if (dt.Rows.Count > 0)
            {
                txtNama.Text = dt.Rows[0]["Nama"].ToString();
                txtEmail.Text = dt.Rows[0]["Email"].ToString();
                txtPassword.Text = dt.Rows[0]["Password"].ToString();
                txtTelepon.Text = dt.Rows[0]["Telepon"].ToString();
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (txtNama.Text == "" || txtEmail.Text == "" || txtPassword.Text == "" || txtTelepon.Text == "")
            {
                MessageBox.Show("Harap isi semua data!");
                return;
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("spUpdateProfil", conn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@PelangganID", SessionUser.PelangganID);
                    cmd.Parameters.AddWithValue("@Nama", txtNama.Text.Trim());
                    cmd.Parameters.AddWithValue("@Email", txtEmail.Text.Trim());
                    cmd.Parameters.AddWithValue("@Password", txtPassword.Text.Trim());
                    cmd.Parameters.AddWithValue("@Telepon", txtTelepon.Text.Trim());

                    int rows = cmd.ExecuteNonQuery();
                    if (rows > 0)
                    {
                        // 🔥 Hapus cache setelah update berhasil
                        _cache.Remove(CacheKey);
                        MessageBox.Show("Profil berhasil diperbarui!");
                    }
                    else
                    {
                        MessageBox.Show("Gagal memperbarui data!");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Terjadi kesalahan: " + ex.Message);
                }
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            _mainForm.Show();
            this.Close();
        }
    }
}
