using OpenCvSharp;
using OpenCvSharp.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    /**
     * Webカメラを使用するためのクラス
     * static classなのでインスタンス生成をしなくて良い
     */
    class KiruColleClient
    {
        /** 確認の為にデータを出力する */
        private const bool DEBUG = true;
        /** センサからデータを受け取る */
        private SensorManager sensor = null;
        /** 画像処理用 */
        private MyImageConverter ic = null;
        /** データ送信用 */
        private DataUploader du = null;
        /** マスク画像作成用の背景画像が設定されているか */
        private bool preparedMask = false;
        /** プログラム起動中 */
        private bool running = false;
        /** 場所データ */
        private string id = "";
        /** 背景のデータ */

        /**
         * コンストラクタ
         **/
        public KiruColleClient(string serialPort, int baudRate, string id)
        {
            sensor = new SensorManager(serialPort, baudRate);
            du = new DataUploader();
            this.id = id;
            du.setLocale(id);
        }

        public void startCapture(double w, double h, string url, string accesskey)
        {
 //           Console.WriteLine("Started KiruColleClient.");
            using (var capture = Cv.CreateCameraCapture(0))
            {
                // センサーを初期化する
                sensor.initSensor();
                // 輝度を変更
                capture.SetCaptureProperty(CaptureProperty.Brightness, 100.0f);
//                capture.SetCaptureProperty(CaptureProperty.Fps, 15.0f);
                // 画面のサイズを設定する
                Cv.SetCaptureProperty(capture, CaptureProperty.FrameWidth, w);
                Cv.SetCaptureProperty(capture, CaptureProperty.FrameHeight, h);
                // 画面を用意（まだ表示しない）
                IplImage frame = Cv.QueryFrame(capture); 
                IplImage mask = new IplImage();
                IplImage pick = new IplImage();
                // 起動中
                running = true;
                // キー入力があるまで続ける
                while (running)
                {
                    /** ESCキーが押されたら終了する */
                    if (Cv.WaitKey(1) == 27)
                    {
                        break;
                    }
                    // カメラからフレームを取得
                    frame = Cv.QueryFrame(capture);
                    // マスク用の背景を設定する（preparedMask=trueで再設定）
                    if (!preparedMask)
                    {
//                        frame.SaveImage(bgPath);
                        ic = new MyImageConverter(BitmapConverter.ToBitmap(frame));
                        preparedMask = true;
                        Console.WriteLine("Prepare");
                    }
                    // デバッグ中なら途中経過の画像を表示する
                    if (DEBUG)
                    {
                        Cv.ShowImage("Capture", frame);
                    }
                    // 撮影のタイミングを待つ
                    if (isTriggered())
//                    if (Cv.WaitKey(1) == 32) // スペースキーが押されたら実行
                    {
                        Bitmap maskBit = ic.makeMaskImage(BitmapConverter.ToBitmap(frame), 40.0f);
                        mask = (OpenCvSharp.IplImage)BitmapConverter.ToIplImage(maskBit);
                        Color col = ic.PickColor(maskBit, 32, du);
                        Bitmap pickBit = ic.makeColorImage(maskBit, col);
                        pick = (OpenCvSharp.IplImage)BitmapConverter.ToIplImage(pickBit);

                        // 袖の長さを計測する
                        du.setSodeData(1);
                        // アップロードする
                        if(ic.canUseData())
                        {
                            du.sendMessage(url, accesskey);
                        }

                        // デバッグ中なら途中経過の画像を表示する
                        if (DEBUG)
                        {
                            Cv.ShowImage("Mask", mask);
                            Cv.ShowImage("Pick", pick);
                        }
                        //System.Threading.Thread.Sleep(2000);
                    }
                }
                // 作成した画面を破棄する
                Cv.DestroyWindow("Capture");
                Cv.DestroyWindow("Pick");
                Cv.DestroyWindow("Mask");
            }
        }

        private bool isAllowingPerson = false;
        private bool isTriggered()
        {
            try
            {
                string str = sensor.getData();
//                Console.WriteLine(str);
                string[] data = str.Split(',');
                double[] values = new double[4];
                for (int i = 0; i < values.Length; i++)
                {
                    values[i] = double.Parse(data[i]);
                }
                if (values[3] < 100.0f)
                {
                    if (!isAllowingPerson)
                    {
//                        Console.WriteLine("Triggered!");
                        du.setSensorData(values);
                        isAllowingPerson = true;
                        return true;
                    }
                }
                else 
                {
                    isAllowingPerson = false;
                }
            }
            catch (Exception err)
            {
                Console.WriteLine("Error" + err.ToString());
                return false;
            }
            return false;
        }
    }
}
