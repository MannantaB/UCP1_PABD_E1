using System;
using System.Data;
using System.Windows.Forms;
using Microsoft.Reporting.WinForms;

namespace bookingstudio
{
    public partial class reportviewer : Form
    {
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
            try
            {
                // Ambil data dari TableAdapter dengan parameter BookingID
                var adapter = new RiwayatBookingSetTableAdapters.DataTable1TableAdapter();

                // Pastikan method GetData punya parameter int bookingID
                DataTable dt = adapter.GetData(_bookingID);

                // Siapkan data source untuk ReportViewer
                ReportDataSource rds = new ReportDataSource("DataSet1", dt);

                // Bersihkan dan tambahkan data source
                reportViewer1.LocalReport.DataSources.Clear();
                reportViewer1.LocalReport.DataSources.Add(rds);

                // Pastikan path file .rdlc sesuai
                reportViewer1.LocalReport.ReportPath = "D:\\coolyeaah!!\\smt 4\\pabd\\bookingstudio\\bookingstudio\\InvoiceReport.rdlc";

                // Refresh tampilan report
                reportViewer1.RefreshReport();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal memuat invoice: " + ex.Message);
            }
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
