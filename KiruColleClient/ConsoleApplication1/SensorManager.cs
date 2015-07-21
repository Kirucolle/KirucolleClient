using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class SensorManager
    {
        /** シリアルポート */
        private SerialPort sp;
        /** センサの初期化 */
        public SensorManager(string serialPort, int baudRate)
        {
            sp = new SerialPort(serialPort);
            sp.BaudRate = baudRate;
            sp.Open();
        }

        /** センサから得た値を取得 */
        public string getData()
        {
            return sp.ReadLine();
        }

        public void initSensor() 
        {
            while(true)
            {
                try
                {
                    string[] data = sp.ReadLine().Split(',');
                    double[] values = new double[4];
                    for (int i = 0; i < values.Length; i++)
                    {
                        values[i] = double.Parse(data[i]);
                    }
                    if (values[3] > 100.0f)
                    {
                        break;
                    }
                }
                catch (Exception)
                {
                }
            }
        }
    }
}
