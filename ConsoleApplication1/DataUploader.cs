using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class DataUploader
    {
        private bool[] reset = new bool[4];
        private string sensorData = "";
        private string colorData = "";
        private string sodeData = "";
        private string locale = "";
        public DataUploader()
        {
            allDataReset();
        }

        public void setSensorData(double[] data)
        {
            sensorData = "o=" + data[1] + "&si=" + data[0] + "&sy=" + data[2];
            reset[0] = true;
        }
        public void setColorData(int r, int g, int b)
        {
            colorData = "r=" + r + "&g=" + g + "&b=" + b;
            reset[1] = true;
        }
        public void setSodeData(int data)
        {
            sodeData = "sode=" + data;
            reset[2] = true;
        }
        public void setLocale(string data)
        {
            this.locale = "basyo=" + data;
            reset[3] = true;
        }

        public void allDataReset() 
        {
            for (int i = 0; i < reset.Length-1; i++)
            {
                reset[i] = false;
            }
        }

        public void sendMessage(string url, string accesskey)
        {
            foreach(bool val in reset)
            {
                if (!val)
                {
                    allDataReset();
                    return;
                }
            }
            string message = url + "/api/setjisseki?" + sensorData + "&" + colorData + "&" + locale + "&" + sodeData;
            Console.WriteLine(message);

//            System.Diagnostics.Process.Start(message);
            //            Console.WriteLine(url + "/api/setjisseki?" + sensorData + "&" + colorData + "&" + sodeData + "&" + locale);

            var client = new HttpClient();
//            client.DefaultRequestHeaders.Add("X-ZUMO-APPLICATION", accesskey);
//            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var request = new HttpRequestMessage(new HttpMethod("GET"), message);
            var result = client.SendAsync(request).Result;

//            Console.WriteLine("Uploaded.");

            allDataReset();
        }
    }
}
