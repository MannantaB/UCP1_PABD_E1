using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bookingstudio
{
    internal class koneksi

    {
        public string ConnectionString()
        {
            try
            {
                string connectStr = $"Server=192.168.43.141\\MANNANTA;Database=BookingStudio;User ID=sa;Password=Jaeminna130800#";
                return connectStr;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return string.Empty;
            }
        }

    }
}
