using System.Diagnostics;
using System.Drawing.Imaging;

namespace ParallelEvaluationDemo
{
    internal class BitmapFactory
    {
        private ThreadLocal<RGBCalculator> Calculators { get; }

        public BitmapFactory(string redFormula, string greenFormula, string blueFormula)
        {
            Calculators = new ThreadLocal<RGBCalculator>(() => new RGBCalculator(redFormula, greenFormula, blueFormula));
        }

        public Bitmap CreateBitmap(int width, int height, out long elapsedMilliseconds)
        {
            var bitmap = new Bitmap(width, height, PixelFormat.Format24bppRgb);
            var rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);

            // lock the bitmap data
            var bmpData = bitmap.LockBits(rect, ImageLockMode.ReadWrite, bitmap.PixelFormat);

            // get the address of the first line
            var ptr = bmpData.Scan0;

            // create an array that will contain the RGB values
            // Stride, or pitch, is described at http://msdn.microsoft.com/en-us/library/windows/desktop/aa473780(v=vs.85).aspx
            var byteCount = bmpData.Stride * bitmap.Height;

            var rgbValues = new byte[byteCount];

            // prepare consts / stats
            const double mult = 2.0d * Math.PI / 256.0d;

            var stopWatch = new Stopwatch();
            stopWatch.Start();

            for (var y = 0; y < bitmap.Height; ++y)
            {
                var y1 = y;

                Parallel.ForEach(
                  Enumerable.Range(0, bitmap.Width),
                  () => y1,
                  (x, loop, yv) =>
                  {
                      var yValue = (yv - 128.0d) * mult;
                      var offset = yv * bmpData.Stride;

                      // get a thread specific calculator
                      var calculator = Calculators.Value;

                      calculator.SetX((x - 128.0d) * mult);
                      calculator.SetY(yValue);

                      // data is stored in BGR order (not RGB)
                      var xOffset = offset + x * 3;
                      rgbValues[xOffset] = calculator.BlueValue;
                      rgbValues[xOffset + 1] = calculator.GreenValue;
                      rgbValues[xOffset + 2] = calculator.RedValue;

                      return yv;
                  },
                  finalResult => { });
            }

            stopWatch.Stop();
            elapsedMilliseconds = stopWatch.ElapsedMilliseconds;

            // Copy the RGB values back to the bitmap
            System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, ptr, byteCount);

            bitmap.UnlockBits(bmpData);

            return bitmap;
        }
    }
}
