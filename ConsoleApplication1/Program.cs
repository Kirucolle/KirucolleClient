using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenCvSharp;
using ConsoleApplication1;
using OpenCvSharp.Extensions;
using System.Drawing;

namespace WebCamConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            KiruColleClient kcc = new KiruColleClient("COM4", 9600, "id001");
            kcc.startCapture(300, 100, "http://webapplication27309.azurewebsites.net", "[accesskey]");
        }
    }
}