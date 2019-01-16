using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace SudokuPlayer
{
    internal static class PictureProcesser
    {
        static Bitmap[,] grid = new Bitmap[9, 9];
        internal static void cut()
        {
            var bitmap=new Bitmap(@"C:\Users\15835\Desktop\all.png");

            //Bitmap[,] grid = new Bitmap[9, 9];
            var xshift = 2;
            var yshift = 2;
            for (int j = 0; j < 9; j++)
            {
                for (var i = 0; i < 9; i++)
                {
                    var gb = new Bitmap(35, 35);
                    using (var g = Graphics.FromImage(gb))
                    {
                        g.DrawImage(bitmap, 0, 0, new Rectangle(xshift, yshift, 35, 35), GraphicsUnit.Point);
                        grid[i, j] = gb;
                        xshift += 35;
                    }
                }
                xshift = 2;
                yshift += 35;
            }
        }
        internal static void Test()
        {
            cut();
            Console.WriteLine(IsSame(PicResize(grid[0,1]), PicResize(grid[8,7])));
        }
        internal static float IsSame(Bitmap a,Bitmap b)
        {
            //very inaccuracy, because picture cut has a liitle shift, maybe i will change to compare count of non-bule pixel
            return GetResult(GetHisogram(a), GetHisogram(b));
        }
        
        private static Bitmap PicResize(Image img)=>new Bitmap(img, 16, 16);
        
        private static int[] GetHisogram(Bitmap img)

        {
            BitmapData data = img.LockBits(new Rectangle( 0 , 0 , img.Width , img.Height ),ImageLockMode.ReadWrite , PixelFormat.Format24bppRgb );
            int[] histogram = new int[256];
            unsafe
            {
                byte* ptr = (byte*)data.Scan0;
                int remain = data.Stride - data.Width * 3;
                for(int i=0;i<histogram.Length;i++)
                    histogram[ i ] = 0;
                for(int i=0;i<data.Height;i++)
                {
                    for(int j=0;j<data.Width;j++)
                    {
                        int mean = ptr[0]+ptr[1] + ptr[2];
                        mean /= 3;
                        histogram[mean] ++;
                        ptr += 3;
                    }
                    ptr += remain;
                }
            }
            img.UnlockBits( data );
            return histogram;

        }
        
        private static float GetResult(int[] firstNum,int[] scondNum)
        {
            if (firstNum.Length != scondNum.Length)
                return 0;
            else
            {
                float result = 0;
                int j = firstNum.Length;
                for (int i = 0; i < j; i++)
                {
                    result += 1 - GetAbs(firstNum[i], scondNum[i]);
                    //Console.WriteLine(i + "----" + result);
                }
                return result/j;
                
                float GetAbs(float firNum, float secNum)
                {
                    float abs = Math.Abs(firNum - secNum);
                    float resultAbs = Math.Max(firNum, secNum);
                    if (resultAbs == 0)
                        resultAbs = 1;
                    return abs / resultAbs;
                }
            }
        }
    }
}