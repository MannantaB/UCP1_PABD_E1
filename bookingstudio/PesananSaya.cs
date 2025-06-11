using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using static bookingstudio.Booking;

namespace bookingstudio
{
    public partial class PesananSaya : Form
    {
        private int PelangganID;
        private string connectionString = "Data Source=DESKTOP-JNH7B7M\\MANNANTA;Initial Catalog=BookingStudio;Integrated Security=True";

        // ✅ Constructor diperbaiki agar menerima PelangganID
        public PesananSaya(int pelangganID)
        {
            InitializeComponent();
            this.PelangganID = pelangganID;
        }

        private void PesananSaya_Load(object sender, EventArgs e)
        {
            LoadPesanan();
        }

        private void LoadPesanan()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetPesananAktif", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                // ✅ Pastikan ID pelanggan valid
                if (PelangganID <= 0)
                {
                    MessageBox.Show("User belum login atau PelangganID tidak valid.");
                    return;
                }

                cmd.Parameters.AddWithValue("@PelangganID", PelangganID);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                dataGridView1.DataSource = dt;
                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                if (dataGridView1.Columns.Contains("BookingID"))
                    dataGridView1.Columns["BookingID"].Visible = false;
            }
        }

        private int GetSelectedBookingID()
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Pilih pesanan terlebih dahulu.");
                return -1;
            }

            return Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["BookingID"].Value);
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Pilih riwayat booking yang ingin diedit.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];
            int bookingID = Convert.ToInt32(selectedRow.Cells["BookingID"].Value);
            EditBooking(bookingID);
        }

        private void EditBooking(int bookingID)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetBookingDetail", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@BookingID", bookingID);

                try
                {
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        string nama = reader["NamaPelanggan"].ToString();
                        string studio = reader["NamaStudio"].ToString();
                        int paketID = Convert.ToInt32(reader["PaketID"]);
                        string namaPaket = reader["NamaPaket"].ToString();
                        decimal harga = Convert.ToDecimal(reader["Harga"]);
                        DateTime tanggal = Convert.ToDateTime(reader["Tanggal"]);
                        TimeSpan jam = (TimeSpan)reader["Jam"];

                        string paketFormatted = $"{paketID} | {namaPaket} | {harga}";

                        // ✅ Pastikan ID pelanggan dikirim ke form Booking
                        Booking formBooking = new Booking(PelangganID)
                        {
                            IsEditMode = true,
                            BookingIDToEdit = bookingID,
                            SelectedData = new BookingData
                            {
                                Nama = nama,
                                Studio = studio,
                                Paket = paketFormatted,
                                Tanggal = tanggal,
                                Jam = jam.TotalHours
                            }
                        };

                        this.Hide();
                        formBooking.Show();
                    }
                    else
                    {
                        MessageBox.Show("Data booking tidak ditemukan.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Gagal mengambil data booking: " + ex.Message);
                }
            }
        }

        private void btnBatal_Click(object sender, EventArgs e)
        {
            int bookingID = GetSelectedBookingID();
            if (bookingID == -1) return;

            DialogResult confirm = MessageBox.Show("Yakin ingin membatalkan pesanan ini?", "Konfirmasi", MessageBoxButtons.YesNo);
            if (confirm == DialogResult.Yes)
            {
                UpdateStatus(bookingID, "Dibatalkan");
                LoadPesanan();
            }
        }

        private void UpdateStatus(int bookingID, string status)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("spUpdateStatusBooking", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@BookingID", bookingID);
                cmd.Parameters.AddWithValue("@Status", status);

                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    MessageBox.Show($"Status berhasil diubah menjadi {status}.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Gagal update status: " + ex.Message);
                }
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            main mainForm = new main();
            mainForm.Show();
            this.Close();
        }
    }
}
