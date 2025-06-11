using bookingstudio;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Runtime.Caching; // <--- untuk MemoryCache

namespace bookingstudio
{
    public partial class Booking : Form
    {
        private string connectionString = "Data Source=DESKTOP-JNH7B7M\\MANNANTA;Initial Catalog=BookingStudio;Integrated Security=True";
        private int currentPelangganID;

        public bool IsEditMode { get; set; } = false;
        public int BookingIDToEdit { get; set; } = 0;
        public BookingData SelectedData { get; set; }

        public Booking(int pelangganID)
        {
            InitializeComponent();
            currentPelangganID = pelangganID;
            datePickerTanggal.Value = DateTime.Now;

            LoadComboBoxData();

            if (SelectedData != null)
            {
                cmbStudio.SelectedItem = SelectedData.Studio;
                cmbPaket.SelectedItem = SelectedData.Paket;
                datePickerTanggal.Value = SelectedData.Tanggal;
                numericJam.Value = (int)SelectedData.Jam;
            }
        }

        public class BookingData
        {
            public string Nama { get; set; }
            public string Studio { get; set; }
            public string Paket { get; set; }
            public DateTime Tanggal { get; set; }
            public double Jam { get; set; }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Apakah Anda yakin ingin keluar?", "Konfirmasi", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            main formmain = new main();
            formmain.Show();
            this.Hide();
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            string selectedStudio = cmbStudio.Text;
            string selectedPaket = cmbPaket.SelectedItem?.ToString();
            DateTime selectedDate = datePickerTanggal.Value;
            string selectedTime = numericJam.Value.ToString();

            if (string.IsNullOrEmpty(selectedStudio) || string.IsNullOrEmpty(selectedPaket))
            {
                MessageBox.Show("Semua field harus diisi!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (currentPelangganID <= 0)
            {
                MessageBox.Show("User belum login atau PelangganID tidak valid.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string[] paketParts = selectedPaket.Split('|');
            if (paketParts.Length != 3)
            {
                MessageBox.Show("Format paket tidak valid!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int paketID = Convert.ToInt32(paketParts[0].Trim());
            string paketName = paketParts[1].Trim();
            decimal harga = Convert.ToDecimal(paketParts[2].Trim());

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    SqlTransaction transaction = conn.BeginTransaction();

                    try
                    {
                        int pelangganID = currentPelangganID;

                        int studioID = 0;
                        using (SqlCommand cmdGetStudioID = new SqlCommand("SELECT StudioID FROM Studio WHERE NamaStudio = @NamaStudio", conn, transaction))
                        {
                            cmdGetStudioID.Parameters.AddWithValue("@NamaStudio", selectedStudio);
                            object result = cmdGetStudioID.ExecuteScalar();
                            if (result == null || result == DBNull.Value)
                            {
                                MessageBox.Show("Studio tidak ditemukan!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                transaction.Rollback();
                                return;
                            }
                            studioID = Convert.ToInt32(result);
                        }

                        using (SqlCommand cmdCheckPaket = new SqlCommand("SELECT COUNT(1) FROM Paket WHERE PaketID = @PaketID", conn, transaction))
                        {
                            cmdCheckPaket.Parameters.AddWithValue("@PaketID", paketID);
                            int paketExists = (int)cmdCheckPaket.ExecuteScalar();

                            if (paketExists == 0)
                            {
                                MessageBox.Show("Paket tidak valid atau tidak ditemukan!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                transaction.Rollback();
                                return;
                            }
                        }

                        if (IsEditMode)
                        {
                            using (SqlCommand cmdEditBooking = new SqlCommand("spUpdateBooking", conn, transaction))
                            {
                                cmdEditBooking.CommandType = CommandType.StoredProcedure;
                                cmdEditBooking.Parameters.AddWithValue("@BookingID", BookingIDToEdit);
                                cmdEditBooking.Parameters.AddWithValue("@StudioID", studioID);
                                cmdEditBooking.Parameters.AddWithValue("@PaketID", paketID);
                                cmdEditBooking.Parameters.AddWithValue("@Tanggal", selectedDate.Date);
                                cmdEditBooking.Parameters.AddWithValue("@Jam", TimeSpan.Parse(selectedTime + ":00"));

                                cmdEditBooking.ExecuteNonQuery();
                            }

                            // 🔥 Hapus cache setelah edit
                            ClearRiwayatCache();

                            transaction.Commit();
                            MessageBox.Show("Booking berhasil diperbarui!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            this.Close();
                        }
                        else
                        {
                            int bookingID = 0;
                            using (SqlCommand cmdInsertBooking = new SqlCommand("spAddBookingStudio", conn, transaction))
                            {
                                cmdInsertBooking.CommandType = CommandType.StoredProcedure;
                                cmdInsertBooking.Parameters.AddWithValue("@PelangganID", pelangganID);
                                cmdInsertBooking.Parameters.AddWithValue("@StudioID", studioID);
                                cmdInsertBooking.Parameters.AddWithValue("@PaketID", paketID);
                                cmdInsertBooking.Parameters.AddWithValue("@Tanggal", selectedDate.Date);
                                cmdInsertBooking.Parameters.AddWithValue("@Jam", TimeSpan.Parse(selectedTime + ":00"));

                                object result = cmdInsertBooking.ExecuteScalar();
                                if (result == null || result == DBNull.Value)
                                {
                                    transaction.Rollback();
                                    MessageBox.Show("Booking gagal dibuat!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    return;
                                }

                                bookingID = Convert.ToInt32(result);
                            }

                            // 🔥 Hapus cache setelah insert
                            ClearRiwayatCache();

                            transaction.Commit();
                            MessageBox.Show("Booking berhasil disimpan!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            ClearForm();
                            pembayaran formpembayaran = new pembayaran(bookingID);
                            formpembayaran.Show();
                            this.Hide();
                        }
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        MessageBox.Show("Gagal menyimpan: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Koneksi gagal: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void ClearForm()
        {
            cmbStudio.SelectedIndex = -1;
            cmbPaket.SelectedIndex = -1;
            datePickerTanggal.Value = DateTime.Now;
            numericJam.Value = 1;
        }

        private void LoadComboBoxData()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand("SELECT NamaStudio FROM Studio", conn))
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        cmbStudio.Items.Clear();
                        while (reader.Read())
                        {
                            cmbStudio.Items.Add(reader["NamaStudio"].ToString());
                        }
                    }

                    using (SqlCommand cmd = new SqlCommand("SELECT PaketID, NamaPaket, Harga FROM Paket", conn))
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        cmbPaket.Items.Clear();
                        while (reader.Read())
                        {
                            int paketID = Convert.ToInt32(reader["PaketID"]);
                            string namaPaket = reader["NamaPaket"].ToString();
                            decimal harga = Convert.ToDecimal(reader["Harga"]);
                            cmbPaket.Items.Add($"{paketID} | {namaPaket} | {harga}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Gagal load combo: " + ex.Message);
                }
            }
        }

        private void ClearRiwayatCache()
        {
            MemoryCache.Default.Remove($"RiwayatBooking_{currentPelangganID}");
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
            string query = @"
                SELECT b.BookingID, s.NamaStudio, p.NamaPaket, b.Tanggal, b.Jam
                FROM Booking b
                JOIN Studio s ON b.StudioID = s.StudioID
                JOIN Paket p ON b.PaketID = p.PaketID
                WHERE b.PelangganID = 1";
            AnalyzeQuery(query);
        }

        
    }
}
