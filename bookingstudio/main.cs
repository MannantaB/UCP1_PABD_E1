﻿using bookingstudio;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace bookingstudio
{

    public partial class main : Form
    {
        koneksi kn = new koneksi();
        public main()
        {
            InitializeComponent();
        }

        private void main_Load(object sender, EventArgs e)
        {
            EnsureIndexes(); // hanya dijalankan sekali saat form utama terbuka
        }


        private void button1_Click(object sender, EventArgs e)
        {
            profil formprofil = new profil(this);
            formprofil.Show();
            this.Hide();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Booking formBooking = new Booking(this, null, SessionUser.PelangganID); // atau ID login valid
            formBooking.Show();
            this.Hide();
        }



        private void button3_Click_1(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
             "Apakah Anda yakin ingin keluar?",
             "Konfirmasi",
             MessageBoxButtons.YesNo,
             MessageBoxIcon.Question
            );

            if (result == DialogResult.Yes)
            {

                Login formLogin = new Login();
                formLogin.Show();
                this.Hide();
            }
        }

        private void btnRiwayat_Click(object sender, EventArgs e)
        {
            Riwayat riwayatForm = new Riwayat(this, SessionUser.PelangganID);
            riwayatForm.Show();
            this.Hide();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            PesananSaya pesananForm = new PesananSaya(this, SessionUser.PelangganID);
            pesananForm.Show();
            this.Hide(); // opsional, kalau kamu mau sembunyikan form utama
        }

        private void EnsureIndexes()
        {
            using (SqlConnection conn = new SqlConnection(kn.ConnectionString()))
            {
                conn.Open();
                string query = @"
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'idx_Pelanggan_Email')
                BEGIN
                    CREATE NONCLUSTERED INDEX idx_Pelanggan_Email ON Pelanggan(Email);
                END;

                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'idx_Studio_NamaStudio')
                BEGIN
                    CREATE NONCLUSTERED INDEX idx_Studio_NamaStudio ON Studio(NamaStudio);
                END;

                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'idx_Booking_PelangganID')
                BEGIN
                    CREATE NONCLUSTERED INDEX idx_Booking_PelangganID ON Booking(PelangganID);
                END;

                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'idx_Booking_StudioID')
                BEGIN
                    CREATE NONCLUSTERED INDEX idx_Booking_StudioID ON Booking(StudioID);
                END;

                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'idx_Booking_PaketID')
                BEGIN
                    CREATE NONCLUSTERED INDEX idx_Booking_PaketID ON Booking(PaketID);
                END;

                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'idx_Pembayaran_BookingID')
                BEGIN
                    CREATE NONCLUSTERED INDEX idx_Pembayaran_BookingID ON Pembayaran(BookingID);
                END;
            ";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.ExecuteNonQuery();
            }
        }
    }
}
