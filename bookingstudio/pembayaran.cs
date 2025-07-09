using System;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace bookingstudio
{
    public partial class pembayaran : Form
    {
        koneksi kn = new koneksi();
        private int _bookingID;
        private string _mode; // "booking" atau "pesanan"
        private Form _PesananSayaForm;

        
        private string selectedFilePath;
        private string folderPath;

        private const long MaxFileSize = 2 * 1024 * 1024; // 2 MB

        public pembayaran(int bookingID, string mode = "booking", Form pesananSayaForm = null)
        {
            InitializeComponent();
            _bookingID = bookingID;
            _mode = mode;
            _PesananSayaForm = pesananSayaForm;

            string projectRootPath = @"C:\VISUALSTUDIO2022";
            folderPath = Path.Combine(projectRootPath, "BuktiBayar");

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            // Sembunyikan tombol "Bayar Nanti" kalau dibuka dari form PesananSaya
            if (_mode == "pesanan")
            {
                btnPayLater.Visible = false;
                btnExit.Visible = true;
            }
            else
            {
                btnExit.Visible = false;
            }
        }

        private void btnPayNow_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                selectedFilePath = openFileDialog.FileName;

                FileInfo fileInfo = new FileInfo(selectedFilePath);
                if (fileInfo.Length > MaxFileSize)
                {
                    MessageBox.Show("Ukuran file terlalu besar! Maksimal 2 MB.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Tampilkan preview gambar
                pictureBox1.Image = Image.FromFile(selectedFilePath);
                pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;

                string fileName = Path.GetFileName(selectedFilePath);
                string destinationPath = Path.Combine(folderPath, fileName);

                try
                {
                    File.Copy(selectedFilePath, destinationPath, true);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Gagal menyimpan file ke folder lokal: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Simpan data ke database
                using (SqlConnection conn = new SqlConnection(kn.ConnectionString()))
                {
                    conn.Open();
                    SqlTransaction transaction = conn.BeginTransaction();

                    try
                    {
                        // Insert pembayaran
                        string query = "INSERT INTO Pembayaran (BookingID, Tanggal, Bukti_Transfer) VALUES (@BookingID, @Tanggal, @Bukti)";
                        SqlCommand cmd = new SqlCommand(query, conn, transaction);
                        cmd.Parameters.AddWithValue("@BookingID", _bookingID);
                        cmd.Parameters.AddWithValue("@Tanggal", DateTime.Now.Date);
                        cmd.Parameters.AddWithValue("@Bukti", destinationPath);
                        cmd.ExecuteNonQuery();

                        // Update status booking jadi "Selesai"
                        string updateBooking = "UPDATE Booking SET Status = 'Selesai' WHERE BookingID = @BookingID";
                        SqlCommand updateCmd = new SqlCommand(updateBooking, conn, transaction);
                        updateCmd.Parameters.AddWithValue("@BookingID", _bookingID);
                        updateCmd.ExecuteNonQuery();

                        transaction.Commit();

                        MessageBox.Show("Bukti pembayaran berhasil diupload! Status booking menjadi 'Selesai'.", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        main formMain = new main();
                        formMain.Show();
                        this.Close();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        MessageBox.Show("Gagal upload ke database: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnPayLater_Click(object sender, EventArgs e)
        {
            // Update status booking jadi 'Diajukan'
            using (SqlConnection conn = new SqlConnection(kn.ConnectionString()))
            {
                conn.Open();

                string query = "UPDATE Booking SET Status = 'Diajukan' WHERE BookingID = @BookingID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@BookingID", _bookingID);
                cmd.ExecuteNonQuery();
            }

            MessageBox.Show("Silakan bayar nanti. Status booking telah diset menjadi 'Diajukan'.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);

            main formMain = new main();
            formMain.Show();
            this.Close();
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
            string query = "SELECT * FROM Pembayaran WHERE BookingID = @bookingID";
            query = query.Replace("@bookingID", _bookingID.ToString());
            AnalyzeQuery(query);
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            if (_PesananSayaForm != null)
            {
                _PesananSayaForm.Show();
            }

            this.Close();
        }
    }
}
