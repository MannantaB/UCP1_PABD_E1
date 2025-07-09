using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Security.AccessControl;
using System.Windows.Forms;
using Microsoft.Reporting.WinForms;

namespace bookingstudio
{
    public partial class reportviewer : Form
    {
        koneksi kn = new koneksi();
        private int _bookingID; // ID booking yang dipilih
        private Form _riwayatForm;

        public reportviewer(int bookingID, Form riwayatForm)
        {
            InitializeComponent();
            _bookingID = bookingID;
            _riwayatForm = riwayatForm;
        }

        private void reportviewer_Load(object sender, EventArgs e)
        {
            LoadInvoice();
        }

        private void LoadInvoice()
        {
            string connectionString = kn.ConnectionString();
            string query = @"
        SELECT B.BookingID, S.NamaStudio, P.NamaPaket, B.Tanggal, B.Jam, B.Status, B.PelangganID
        FROM Booking AS B 
        INNER JOIN Studio AS S ON B.StudioID = S.StudioID 
        LEFT OUTER JOIN Paket AS P ON B.PaketID = P.PaketID
        WHERE B.BookingID = @BookingID AND B.Status IN ('Selesai', 'Dibatalkan')";

            DataTable dataTable = new DataTable();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@BookingID", _bookingID);

                SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
                dataAdapter.Fill(dataTable);
            }

            ReportDataSource rds = new ReportDataSource("DataSet1", dataTable);

            reportViewer1.LocalReport.DataSources.Clear();
            reportViewer1.LocalReport.DataSources.Add(rds);

            string reportPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "InvoiceReport.rdlc");
            reportViewer1.LocalReport.ReportPath = reportPath;

            reportViewer1.RefreshReport();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            // Tampilkan kembali form Riwayat
            _riwayatForm.Show();

            // Tutup reportviewer
            this.Close();
        }
    }
}
