using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    /** 画像処理用のクラス */
    class MyImageConverter
    {
        /** マスク用の背景画像 */
        private Bitmap bg = null;

        private bool useData = false;

        /** マスク用の背景画像を設定するコンストラクタ */
        public MyImageConverter(Bitmap back)
        {
            this.bg = back;  
        }

        /** 画像に含まれる平均的な色を取得するメソッド
         *  閾値を設定することができる
         *  @param src 調査したい画像
         *  @param threshold 閾値
         *  @param du データアップロード用クラス
         *  @return 平均的な色に染めた画像
         */
        public Color PickColor(Bitmap src, double threshold,  DataUploader du)
        {
            int r = 0;
            int g = 0;
            int b = 0;
            int count = 0;
            // 全ての画素に対して
            for (int y = 0; y < bg.Height; y++)
            {
                for (int x = 0; x < bg.Width; x++)
                {
                    // 画像をグレースケールに
                    double src_gray = (src.GetPixel(x, y).R + src.GetPixel(x, y).G + src.GetPixel(x, y).B) / 3.0f;

                    // 採用する色
                    if (threshold < src_gray)
                    {
                        r += src.GetPixel(x, y).R;
                        g += src.GetPixel(x, y).G;
                        b += src.GetPixel(x, y).B;
                        count++;
//                        src.SetPixel(x, y, Color.FromArgb((int)src_gray, (int)src_gray, (int)src_gray));
                    }
                }
            }
            if (count != 0)
            {
                r /= count;
                g /= count;
                b /= count;
            }
            else 
            {
//                Console.WriteLine((int)r + "," + (int)g + "," + (int)b);
                useData = false;
                Console.WriteLine("Error");
                return Color.FromArgb(0, 0, 0);
            }

            // 平均的な色
//            Console.WriteLine(r + "," + g + "," + b);
            useData = true;
            du.setColorData(r,g,b);
            return Color.FromArgb(r,g,b);
        }

        public bool canUseData() 
        {
            return useData;
        }

        /** 
         * 指定した色で画像を染める
         * @param src 画像データ
         * @param col 染めたい色
         * @return 染めた画像のデータ
         */
        public Bitmap makeColorImage(Bitmap src, Color col)
        {
            // 画像を指定の色で染めあげる
            for (int y = 0; y < bg.Height; y++)
            {
                for (int x = 0; x < bg.Width; x++)
                {
                    src.SetPixel(x, y, col);
                }
            }
            return src;
        }

        /** 
         * 背景画像を基に、変化のある部分を抽出する
         * @param src 画像データ
         * @param threshold 閾値
         * @return 背景を取り除いた画像データ
         */
        public Bitmap makeMaskImage(Bitmap src, float threshold)
        {
            // 全ての画素に対して
            for (int y = 0; y < bg.Height; y++)
            {
                for (int x = 0; x < bg.Width; x++)
                {
                    // 画像をグレースケールに
                    double bg_gray = (bg.GetPixel(x, y).R + bg.GetPixel(x, y).G + bg.GetPixel(x, y).B)/3.0f;
                    double src_gray = (src.GetPixel(x, y).R + src.GetPixel(x, y).G + src.GetPixel(x, y).B)/3.0f;

                    // 色の距離が閾値よりも遠ければ黒にする
                    if (threshold > Math.Abs(bg_gray - src_gray))
                    {
                        src.SetPixel(x, y, Color.Black);
                    }
                }
            }
            return src;
        }
    }
}
