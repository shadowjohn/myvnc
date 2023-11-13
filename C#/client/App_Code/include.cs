using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
namespace utility
{
    public class myinclude
    {
        private Random rnd = new Random(DateTime.Now.Millisecond);

        public myinclude()
        {
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
        }
        public string b2s(byte[] input)
        {
            return System.Text.Encoding.UTF8.GetString(input);
        }
        public double arduino_map(double x, double inMin, double inMax, double outMin, double outMax)
        {
            // x 為輸入值
            // inMin、inMax 為輸入範圍
            // outMin、outMax 為輸出範圍
            return (x - inMin) * (outMax - outMin) / (inMax - inMin) + outMin;
        }
    }
}
