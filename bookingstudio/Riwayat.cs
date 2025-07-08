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

                    dataGridView1.DataSource = dt;

                    // ✅ Properti DGV
                    dataGridView1.AutoGenerateColumns = true;
                    dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                    dataGridView1.MultiSelect = false;
                    dataGridView1.ReadOnly = true;
                    dataGridView1.AllowUserToAddRows = false;
                    dataGridView1.AllowUserToDeleteRows = false;
                    dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                    // Format
                    FormatDataGridView();

                    if (dt.Rows.Count == 0)
                    {
                        MessageBox.Show("Tidak ada data riwayat booking untuk user ini.");
                    }
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
                dataGridView1.Columns["Tanggal"].HeaderText = "Tanggal ";

            if (dataGridView1.Columns.Contains("Jam"))
                dataGridView1.Columns["Jam"].HeaderText = "Jam Foto";

            if (dataGridView1.Columns.Contains("Status"))
                dataGridView1.Columns["Status"].HeaderText = "Status Booking";
        }

        private void btnCetak_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Pilih riwayat booking yang ingin dicetak!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int bookingID = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["BookingID"].Value);

            reportviewer laporan = new reportviewer(bookingID);
            laporan.Show();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            _mainForm.Show();
            this.Close();
        }
    }
}
