using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ODBenchmark.Frequency
{
    public class HarrisCornerDetection
    {
        private float[] kernel;
        public float sigma = 1.2f;
        public bool orginalMeasure = true;
        public float threshold = 2000f;
        public int r = 3;

        public void Start()
        {
            createKernel();
        }

        public List<IntPoint> ProcessImage(byte[] image, int imageWidth, int imageHeight)
        {
            createKernel();

            // get source image size
            int width = imageWidth;
            int height = imageHeight;


            // 1. Calculate partial differences
            float[,] diffx = new float[height, width];
            float[,] diffy = new float[height, width];
            float[,] diffxy = new float[height, width];

            //Fixed
            byte[] src = image;

            // for each line
            for (int y = 1; y < height - 1; y++)
            {
                // for each pixel
                for (int x = 1; x < width - 1; x++)
                {
                    int centerIndex = (y * width) + x;
                    // Retrieve the pixel neighborhood
                    byte a11 = src[centerIndex + width - 1], a12 = src[centerIndex + width], a13 = src[centerIndex + width + 1];
                    byte a21 = src[centerIndex - 1], /*  a22    */  a23 = src[centerIndex + 1];
                    byte a31 = src[centerIndex - width - 1], a32 = src[centerIndex - width], a33 = src[centerIndex - width + 1];

                    // Convolution with horizontal differentiation kernel mask
                    float h = ((a11 + a12 + a13) - (a31 + a32 + a33)) * 0.166666667f;

                    // Convolution with vertical differentiation kernel mask
                    float v = ((a11 + a21 + a31) - (a13 + a23 + a33)) * 0.166666667f;

                    // Store squared differences directly
                    diffx[y, x] = h * h;
                    diffy[y, x] = v * v;
                    diffxy[y, x] = h * v;
                }

                // Skip last column
            }

            // 2. Smooth the diff images

            if (sigma > 0.0)
            {
                float[,] temp = new float[height, width];

                // Convolve with Gaussian kernel
                convolve(diffx, temp, kernel);
                convolve(diffy, temp, kernel);
                convolve(diffxy, temp, kernel);
            }


            // 3. Compute Harris Corner Response Map
            float[,] map = new float[height, width];

            float M, A, B, C;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    A = diffx[y, x];
                    B = diffy[y, x];
                    C = diffxy[y, x];

                    if (orginalMeasure)
                    {
                        // Original Harris corner measure
                        M = (A * B - C * C) - (0.04f * ((A + B) * (A + B)));
                    }
                    else
                    {
                        // Harris-Noble corner measure
                        M = (A * B - C * C) / (A + B + 1.192093E-07f);
                    }

                    if (M > threshold)
                    {
                        map[y, x] = M; // insert value in the map
                    }
                }
            }


            // 4. Suppress non-maximum points
            List<IntPoint> cornersList = new List<IntPoint>();

            for (int y = r, maxY = height - r; y < maxY; y++)
            {
                // for each pixel
                for (int x = r, maxX = width - r; x < maxX; x++)
                {
                    float currentValue = map[y, x];

                    // for each windows' row
                    for (int i = -r; (currentValue != 0) && (i <= r); i++)
                    {
                        // for each windows' pixel
                        for (int j = -r; j <= r; j++)
                        {
                            if (map[y + i, x + j] > currentValue)
                            {
                                currentValue = 0;
                                break;
                            }
                        }
                    }
                    // check if this point is really interesting
                    if (currentValue != 0)
                    {
                        cornersList.Add(new IntPoint(y, x));
                    }
                }
            }
            return cornersList;
        }

        public void createKernel()
        {
            kernel = new float[r];
            for (var i = 0; i < r; i++)
            {
                kernel[i] = (float)(1.0f / (Math.Sqrt(2 * Math.PI) * sigma))
                    * (float)Math.Exp(-((r * r) / (2.0f * sigma * sigma)));
            }
        }

        private void convolve(float[,] image, float[,] temp, float[] kernel)
        {
            int width = image.GetLength(1);
            int height = image.GetLength(0);
            int radius = kernel.Length / 2;

            for (int y = 0; y < height; y++)
            {
                for (int x = radius; x < width - radius; x++)
                {
                    float v = 0;
                    for (int k = 0; k < kernel.Length; k++)
                        v += image[y, x + k - radius] * kernel[k];
                    temp[y, x] = v;
                }
            }

            for (int x = 0; x < width; x++)
            {
                for (int y = radius; y < height - radius; y++)
                {
                    float v = 0;
                    for (int k = 0; k < kernel.Length; k++)
                        v += temp[y + k - radius, x] * kernel[k];
                    image[y, x] = v;
                }
            }
        }
    }

    public class IntPoint
    {
        public int X { get; set; }
        public int Y { get; set; }

        public IntPoint(int y, int x)
        {
            X = x;
            Y = y;
        }

        public float Distance(IntPoint p)
        {
            return (float)Math.Sqrt((Y - p.Y) * (X - p.X));
        }

        public IntPoint CalculateMoveVector(IntPoint p)
        {
            return new IntPoint(p.Y - Y, p.X - X);
        }

        public float VectorNormalized()
        {
            return (float)((Math.Atan2(Y, X) / Math.PI) + 2.0f) % 2.0f;
        }

        public float CalculateScaleFromMoveVectors(IntPoint p)
        {
            var s1 = (float)Math.Sqrt(X * X + Y * Y);
            var s2 = (float)Math.Sqrt(p.X * p.X + p.Y * p.Y);
            if (s1 == 0.0f)
                s1 = 1.0f;
            return Math.Abs(s2 / s1);
        }
    }
}
