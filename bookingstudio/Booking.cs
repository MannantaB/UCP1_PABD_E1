using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace bookingstudio
{
    public partial class Booking : Form
    {
        public Booking()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            comboBox2.Items.Add("Studio A - Rp200000");
            comboBox2.Items.Add("Studio B - Rp300000");
            comboBox2.Items.Add("Studio C - Rp400000");
            comboBox2.SelectedIndex = 0;
        }
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedText = comboBox2.SelectedItem.ToString();

            // Misah pake "-" (asumsi formatnya: Paket - Harga)
            string[] parts = selectedText.Split('-');

            if (parts.Length == 2)
            {
                string paket = parts[0].Trim(); // Studio A
                string hargaStr = parts[1].Replace("Rp", "").Trim(); // 200000
                int harga = int.Parse(hargaStr); // Ubah ke angka

                // Tampilkan (opsional)
                label4.Text = paket;
                label5.Text = harga.ToString();
            }
            private void btnUpload_Click(object sender, EventArgs e)
            {
                OpenFileDialog open = new OpenFileDialog();
                open.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";

                if (open.ShowDialog() == DialogResult.OK)
                {
                    pictureBox1.Image = new Bitmap(open.FileName);
                    pbBukti.Tag = open.FileName; // simpan path untuk nanti
                }
            }
        }
    }
}
