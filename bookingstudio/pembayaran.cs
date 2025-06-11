using System;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace bookingstudio
{
    public partial class pembayaran : Form
    {
        private int _bookingID;
        private string connectionString = "Data Source=DESKTOP-JNH7B7M\\MANNANTA;Initial Catalog=BookingStudio;Integrated Security=True";
        private string selectedFilePath;
        private string folderPath;

        public pembayaran(int bookingID)
        {
            InitializeComponent();
            _bookingID = bookingID;

            string projectRootPath = @"C:\VISUALSTUDIO2022";
            folderPath = Path.Combine(projectRootPath, "BuktiBayar");

            // Pastikan folder ada
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
        }

        private void btnUpload_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "All Files|.";


            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                selectedFilePath = openFileDialog.FileName;

                // Preview gambar
                pictureBox1.Image = Image.FromFile(selectedFilePath);
                pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;

                // Simpan gambar ke folder lokal
                string fileName = Path.GetFileName(selectedFilePath);
                string destinationPath = Path.Combine(folderPath, fileName);

                try
                {
                    File.Copy(selectedFilePath, destinationPath, true); // overwrite jika file sama
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Gagal menyimpan file ke folder: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlTransaction transaction = conn.BeginTransaction();

                    try
                    {
                        // Query insert pembayaran dengan transaction
                        string query = "INSERT INTO Pembayaran (BookingID, Tanggal, Bukti_Transfer) VALUES (@BookingID, @Tanggal, @Bukti)";
                        SqlCommand cmd = new SqlCommand(query, conn, transaction);
                        cmd.Parameters.AddWithValue("@BookingID", _bookingID);
                        cmd.Parameters.AddWithValue("@Tanggal", DateTime.Now.Date);
                        cmd.Parameters.AddWithValue("@Bukti", fileName);
                        cmd.ExecuteNonQuery();

                        // Untuk testing error rollback, uncomment baris berikut:
                        // throw new Exception("Testing error rollback pembayaran!");

                        transaction.Commit();

                        MessageBox.Show("Bukti pembayaran berhasil diupload!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);

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

        private void AnalyzeQuery(string sqlQuery)
        {
            using (var conn = new SqlConnection(connectionString))
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
            string query = "SELECT * FROM Pembayaran WHERE BookingID = 1";
            AnalyzeQuery(query);
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
    }
}