using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace bookingstudio
{
    public partial class Riwayat : Form
    {
        private int PelangganID;
        private string connectionString = "Data Source=DESKTOP-JNH7B7M\\MANNANTA;Initial Catalog=BookingStudio;Integrated Security=True";
        private main _mainForm;

        public Riwayat(main mainForm, int PelangganId)
        {
            InitializeComponent();
            _mainForm = mainForm;   
            PelangganID = PelangganId;
        }

        private void Riwayat_Load(object sender, EventArgs e)
        {
            LoadRiwayatBooking();
        }

        private void LoadRiwayatBooking()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("spRiwayatBookingUser", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@PelangganID", PelangganID);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    if (dt.Rows.Count == 0)
                    {
                        MessageBox.Show("Tidak ada data riwayat booking untuk user ini.");
                        // Tetap tampilkan DataGridView kosong
                        dataGridView1.DataSource = dt;
                        return;
                    }

                    dataGridView1.DataSource = dt;
                    dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                    // Format kolom
                    FormatDataGridView();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Gagal memuat riwayat: " + ex.Message);
                }
            }
        }

        private void FormatDataGridView()
        {
            if (dataGridView1.Columns.Contains("BookingID"))
                dataGridView1.Columns["BookingID"].Visible = false;

            if (dataGridView1.Columns.Contains("NamaStudio"))
                dataGridView1.Columns["NamaStudio"].HeaderText = "Studio";

            if (dataGridView1.Columns.Contains("NamaPaket"))
                dataGridView1.Columns["NamaPaket"].HeaderText = "Paket";

            if (dataGridView1.Columns.Contains("Tanggal"))
                dataGridView1.Columns["Tanggal"].HeaderText = "Tanggal Booking";

            if (dataGridView1.Columns.Contains("Jam"))
                dataGridView1.Columns["Jam"].HeaderText = "Jam Foto";

            if (dataGridView1.Columns.Contains("Status"))
                dataGridView1.Columns["Status"].HeaderText = "Status Booking";
        }


        private void btnCetak_Click(object sender, EventArgs e)
        {
            reportviewer laporan = new reportviewer(PelangganID);
            laporan.Show();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            _mainForm.Show();
            this.Close();
        }

    }
}
