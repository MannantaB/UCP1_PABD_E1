using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace SISTEM_BOOKING_FOTO_STUDIO
{
    public partial class Form1 : Form
    {
        private string connectionString = "Data Source=DESKTOP-JNH7B7M\\MANNANTA;Initial Catalog=BookingStudio;Integrated Security=True";
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadStudio();
            LoadPaket();
        }

        private void LoadStudio()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
                try
                {
                comboBox1.Items.Clear();
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT NamaStudio FROM Studio", conn);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    comboBox1.Items.Add(reader["NamaStudio"].ToString());
                }
                conn.Close();
                }
                catch (Exception ex)
                {
                MessageBox.Show("Error loading studios: " + ex.Message);
                }
        }

        private void LoadPaket()
        {
            comboBox2.Items.Clear();
            comboBox2.Items.Add("LolyPop");
            comboBox2.Items.Add("Marshmallow");
            comboBox2.Items.Add("CandyCane");
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (ValidasiForm())
            {
                string studio = comboBox1.SelectedItem.ToString();
                string paket = comboBox2.SelectedItem.ToString();
                DateTime tanggal = dateTimePicker1.Value.Date;
                TimeSpan jam = TimeSpan.Parse(numericUpDown1.Value.ToString() + ":00");
                decimal harga = 150000; // bisa disesuaikan berdasarkan paket
                string buktiTransfer = UploadGambar();

                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("INSERT INTO Booking (PelangganID, StudioID, Paket, Harga, Tanggal, Jam) VALUES (@PelangganID, @StudioID, @Paket, @Harga, @Tanggal, @Jam); SELECT SCOPE_IDENTITY();", conn);
                    cmd.Parameters.AddWithValue("@PelangganID", 1); // Sesuaikan login
                    cmd.Parameters.AddWithValue("@StudioID", GetStudioID(studio));
                    cmd.Parameters.AddWithValue("@Paket", paket);
                    cmd.Parameters.AddWithValue("@Harga", harga);
                    cmd.Parameters.AddWithValue("@Tanggal", tanggal);
                    cmd.Parameters.AddWithValue("@Jam", jam);
                    int bookingID = Convert.ToInt32(cmd.ExecuteScalar());

                    SqlCommand pembayaranCmd = new SqlCommand("INSERT INTO Pembayaran (BookingID, Tanggal, Jumlah, Bukti_Transfer) VALUES (@BookingID, @Tanggal, @Jumlah, @Bukti)", conn);
                    pembayaranCmd.Parameters.AddWithValue("@BookingID", bookingID);
                    pembayaranCmd.Parameters.AddWithValue("@Tanggal", tanggal);
                    pembayaranCmd.Parameters.AddWithValue("@Jumlah", harga);
                    pembayaranCmd.Parameters.AddWithValue("@Bukti", buktiTransfer);
                    pembayaranCmd.ExecuteNonQuery();

                    MessageBox.Show("Booking berhasil!");
                    conn.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Gagal simpan data: " + ex.Message);
                }
            }
        }

        private int GetStudioID(string namaStudio)
        {
            SqlCommand cmd = new SqlCommand("SELECT StudioID FROM Studio WHERE NamaStudio = @Nama", conn);
            cmd.Parameters.AddWithValue("@Nama", namaStudio);
            return (int)cmd.ExecuteScalar();
        }

        private string UploadGambar()
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Image Files|.jpg;.jpeg;*.png;";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    string fileName = Path.GetFileName(ofd.FileName);
                    string destPath = Path.Combine(Application.StartupPath, "uploads", fileName);
                    Directory.CreateDirectory(Path.GetDirectoryName(destPath));
                    File.Copy(ofd.FileName, destPath, true);
                    return destPath;
                }
            }
            return null;
        }

        private bool ValidasiForm()
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text) || comboBox1.SelectedIndex == -1 || comboBox2.SelectedIndex == -1)
            {
                MessageBox.Show("Harap lengkapi semua data!");
                return false;
            }
            return true;
        }
    }
}