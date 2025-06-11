using System;
using System.Data;
using System.Windows.Forms;
using Microsoft.Reporting.WinForms;

namespace bookingstudio
{
    public partial class reportviewer : Form
    {
        private int _pelangganID; // simpan ID user

        public reportviewer(int PelangganID)
        {
            InitializeComponent();
            _pelangganID = PelangganID;
        }

        private void reportviewer_Load(object sender, EventArgs e)
        {
          
        }

        private void reportViewer1_Load(object sender, EventArgs e)
        {
            try
            {
                // Ambil data dari TableAdapter dengan parameter UserID
                var adapter = new RiwayatBookingSetTableAdapters.DataTable1TableAdapter();
                DataTable dt = adapter.GetData(SessionUser.PelangganID); // Kirim parameter UserID

                // Buat DataSource Report
                ReportDataSource rds = new ReportDataSource("DataSet1", dt);
                reportViewer1.LocalReport.DataSources.Clear();
                reportViewer1.LocalReport.ReportPath = "RiwayatBookingReport.rdlc";
                reportViewer1.LocalReport.DataSources.Add(rds);
                reportViewer1.RefreshReport();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal memuat laporan: " + ex.Message);
            }
        }
    }
}
