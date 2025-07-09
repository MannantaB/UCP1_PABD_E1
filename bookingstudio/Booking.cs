using bookingstudio;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Runtime.Caching;

namespace bookingstudio
{
    public partial class Booking : Form
    {
        koneksi kn = new koneksi();
        private int currentPelangganID;
        private main _mainForm;
        private Form _pesananSayaForm;
        private main main;
        private int pelangganID;
        public bool IsEditMode { get; set; } = false;
        public int BookingIDToEdit { get; set; } = 0;
        public BookingData SelectedData { get; set; }
        public event Action OnBookingUpdated;

        public Booking(int pelangganID)
        {
            InitializeComponent();
            currentPelangganID = pelangganID;
            datePickerTanggal.Value = DateTime.Now;
            datePickerTanggal.MinDate = DateTime.Today; 
            numericJam.Maximum = 22;
            numericJam.Minimum = 8;
            LoadComboBoxData();
            if (SelectedData != null)
            {
                cmbStudio.SelectedItem = SelectedData.Studio;
                cmbPaket.SelectedItem = SelectedData.Paket;
                datePickerTanggal.Value = SelectedData.Tanggal;
                numericJam.Value = (int)SelectedData.Jam;
            }
        }

        public Booking(main mainForm, Form pesananSayaForm, int pelangganId)
        {
            InitializeComponent();
            _mainForm = mainForm;
            _pesananSayaForm = pesananSayaForm;
            currentPelangganID = pelangganId;
            datePickerTanggal.Value = DateTime.Now;
            LoadComboBoxData();
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
            if (result == DialogResult.Yes)
            {
                if (_pesananSayaForm != null)
                {
                    _pesananSayaForm.Show(); // kembali ke form PesananSaya
                }
                else if (_mainForm != null)
                {
                    _mainForm.Show(); // kembali ke form main jika tidak dari PesananSaya
                }
                this.Close();
            }
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            string selectedStudio = cmbStudio.Text;
            string selectedPaket = cmbPaket.SelectedItem?.ToString();
            DateTime selectedDate = datePickerTanggal.Value.Date;
            int selectedTime = (int)numericJam.Value;

            if (string.IsNullOrEmpty(selectedStudio) || string.IsNullOrEmpty(selectedPaket))
            {
                MessageBox.Show("Semua field harus diisi!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Validasi jika tanggal adalah hari ini, maka jam tidak boleh < jam sekarang
            if (selectedDate == DateTime.Today.Date && selectedTime < DateTime.Now.Hour)
            {
                MessageBox.Show("Tidak dapat memesan dengan jam yang sudah lewat untuk hari ini.",
                    "Validasi Jam", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Validasi jam berada dalam rentang yang benar
            if (selectedTime < numericJam.Minimum || selectedTime > numericJam.Maximum)
            {
                MessageBox.Show($"Nilai jam harus antara {numericJam.Minimum} dan {numericJam.Maximum}.",
                    "Validasi Jam", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
            decimal harga = Convert.ToDecimal(paketParts[2].Trim());

            using (SqlConnection conn = new SqlConnection(kn.ConnectionString()))
            {
                try
                {
                    conn.Open();
                    SqlTransaction transaction = conn.BeginTransaction();

                    try
                    {
                        int studioID;
                        using (SqlCommand cmdGetStudioID = new SqlCommand("SELECT StudioID FROM Studio WHERE NamaStudio = @NamaStudio", conn, transaction))
                        {
                            cmdGetStudioID.Parameters.AddWithValue("@NamaStudio", selectedStudio);
                            object result = cmdGetStudioID.ExecuteScalar();
                            if (result == null)
                            {
                                MessageBox.Show("Studio tidak ditemukan!");
                                transaction.Rollback();
                                return;
                            }
                            studioID = Convert.ToInt32(result);
                        }

                        using (SqlCommand cmdCheckPaket = new SqlCommand("SELECT COUNT(1) FROM Paket WHERE PaketID = @PaketID", conn, transaction))
                        {
                            cmdCheckPaket.Parameters.AddWithValue("@PaketID", paketID);
                            if ((int)cmdCheckPaket.ExecuteScalar() == 0)
                            {
                                MessageBox.Show("Paket tidak valid!");
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
                                cmdEditBooking.Parameters.AddWithValue("@Jam", TimeSpan.Parse($"{selectedTime}:00"));
                                cmdEditBooking.ExecuteNonQuery();
                            }

                            ClearRiwayatCache();
                            transaction.Commit();
                            MessageBox.Show("Booking berhasil diperbarui!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            OnBookingUpdated?.Invoke(); 
                            this.Close();
                        }
                        else
                        {
                            int bookingID;
                            using (SqlCommand cmdInsertBooking = new SqlCommand("spAddBookingStudio", conn, transaction))
                            {
                                cmdInsertBooking.CommandType = CommandType.StoredProcedure;
                                cmdInsertBooking.Parameters.AddWithValue("@PelangganID", currentPelangganID);
                                cmdInsertBooking.Parameters.AddWithValue("@StudioID", studioID);
                                cmdInsertBooking.Parameters.AddWithValue("@PaketID", paketID);
                                cmdInsertBooking.Parameters.AddWithValue("@Tanggal", selectedDate.Date);
                                cmdInsertBooking.Parameters.AddWithValue("@Jam", TimeSpan.Parse($"{selectedTime}:00"));

                                object result = cmdInsertBooking.ExecuteScalar();
                                if (result == null)
                                {
                                    transaction.Rollback();
                                    MessageBox.Show("Gagal membuat booking!");
                                    return;
                                }

                                bookingID = Convert.ToInt32(result);
                            }

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
                        MessageBox.Show("Gagal menyimpan: " + ex.Message);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Koneksi gagal: " + ex.Message);
                }
            }
        }

        private void ClearForm()
        {
            cmbStudio.SelectedIndex = -1;
            cmbPaket.SelectedIndex = -1;
            datePickerTanggal.Value = DateTime.Now;
            numericJam.Value = numericJam.Minimum; 
        }

        private void LoadComboBoxData()
        {
            using (SqlConnection conn = new SqlConnection(kn.ConnectionString()))
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

        private void AnalyzeQuery(string sqlQuery, SqlParameter[] parameters = null)
        {
            if (string.IsNullOrWhiteSpace(sqlQuery))
            {
                MessageBox.Show("Query kosong atau tidak valid.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string trimmedQuery = sqlQuery.Trim();
            if (!trimmedQuery.StartsWith("SELECT", StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show("AnalyzeQuery hanya mendukung query SELECT.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (var conn = new SqlConnection(kn.ConnectionString()))
                {
                    conn.InfoMessage += (s, e) =>
                    {
                        foreach (var message in e.Message.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries))
                        {
                            MessageBox.Show(message, "STATISTICS INFO", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    };

                    conn.Open();

                    string wrappedQuery = $@"
                SET STATISTICS IO ON;
                SET STATISTICS TIME ON;
                {sqlQuery};
                SET STATISTICS IO OFF;
                SET STATISTICS TIME OFF;";

                    using (var cmd = new SqlCommand(wrappedQuery, conn))
                    {
                        if (parameters != null)
                        {
                            cmd.Parameters.AddRange(parameters);
                        }

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show("SQL Error: " + ex.Message, "SQL Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnAnalyze_Click_1(object sender, EventArgs e)
        {
            string query = @"
        SELECT b.BookingID, s.NamaStudio, p.NamaPaket, b.Tanggal, b.Jam
        FROM Booking b
        JOIN Studio s ON b.StudioID = s.StudioID
        JOIN Paket p ON b.PaketID = p.PaketID
        WHERE b.PelangganID = @PelangganID";

            SqlParameter[] parameters =
            {
        new SqlParameter("@PelangganID", currentPelangganID)
    };

            AnalyzeQuery(query, parameters);
        }
        

    }
}