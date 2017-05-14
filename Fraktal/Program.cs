using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.Threading;
using System;
using System.Numerics;
using Akka;
using Akka.Actor;
using Akka.Routing;



namespace Fraktal
{

    public static class FractalBitmapHolder
    {
        public static Bitmap fractalBitmap { get; set; }
        public static bool isReady {get; set;}
    }
    class ComplexNumber
    {
        public double Real;
        public double Imaginary;
        public ComplexNumber(double real,double imaginary)
        {
            Real = real;
            Imaginary = imaginary;
        }
        public ComplexNumber squared()
        {
            double newReal = Real * Real - Imaginary * Imaginary;
            double newImaginary = Real * Imaginary + Imaginary * Real;
            return new ComplexNumber(newReal, newImaginary);
        }
        public static ComplexNumber operator +(ComplexNumber c1, ComplexNumber c2)
        {
            return new ComplexNumber(c1.Real + c2.Real, c1.Imaginary + c2.Imaginary);
        }
    }

    public class FractalActor : ReceiveActor //COMMANDER!
    {
        public Bitmap bitmap;
        public int jobsLeft;
        public FractalActor(int height, int width)
        {
            jobsLeft = height;
            Receive<CalculatedRowData>(data =>
            {
                for (int j = 0; j < width; j++)
                {
                    bitmap.SetPixel(j, data.rowNum, Color.FromArgb(0, 0, data.rowData[j]));
                }
                jobsLeft--;
                if(jobsLeft == 0)
                {
                    Self.Ask(Kill.Instance);
                }
            });
            bitmap = new Bitmap(height, width);
            IActorRef calculateRowActors = Context.ActorOf(Props.Create(() => new RowActor())
                                          .WithRouter(new RoundRobinPool(4)), "RowCalculators");

            for (int i = 0; i < height; i++)
            {
                calculateRowActors.Tell(new RowData(width, height, i));
            }
        }
        protected override void PostStop()
        {
            FractalBitmapHolder.fractalBitmap = bitmap;
            FractalBitmapHolder.isReady = true;
        }
    }

    public struct CalculatedRowData
    {
        public int rowNum;
        public int[] rowData;
        public CalculatedRowData(int num, int[] data)
        {
            rowNum = num;
            rowData = data;
        }
    }

    public struct RowData
    {
        public int w;
        public int h;
        public int rowNum;
        public RowData(int _w, int _h, int _num)
        {
            w = _w;
            h = _h;
            rowNum = _num;
        }
    }

    public class RowActor : ReceiveActor
    {
        public RowActor()
        {
            this.Receive<RowData>(data =>
              {
                  int[] row = Program.calculateRow(data.w, data.h, data.rowNum);
                  this.Sender.Tell(new CalculatedRowData(data.rowNum, row));
              });
        }
    }

    static class Program
    {
        static int calculatePixel(double x, double y)
        {
            //Wzór to Zk+1 = Zk^2 +c
            //c to badany punkt na płaszczyźnie zespolonej
            ComplexNumber z = new ComplexNumber(0.0, 0.0);
            ComplexNumber zOld = new ComplexNumber(0.0, 0.0);
            ComplexNumber c = new ComplexNumber(x, y);
            int iterations;
            for (iterations = 0; iterations < 255; iterations++)
            {
                z = zOld.squared() + c;
                if (z.Imaginary * z.Imaginary + z.Real * z.Real >= 4.0) return iterations; //jeśli warunek spełniony to oznacza że |z| rozbiegnie się do ∞ , lub gdy osiągniemy maksymalną liczbę iteracji.
                zOld.Real = z.Real;
                zOld.Imaginary = z.Imaginary;
            }
            return iterations;
        }
        public static int[] calculateRow(int w, int h, int rowNbr)
        {
            int[] row = new int[w];
            double y = -2.0 + 4.0 * rowNbr / h;
            for (int i = 0; i < w; i++)
            {
                double x = -2.0 + 4.0 * i / w;
                
                row[i] = (char)(255 - calculatePixel(x, y)); //Jasność piksela równa 255 - liczba iteracji (proporcjonalna do liczby iteracji pętli)
            }
            return row;
        }
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]

        public static Bitmap generateFractal(int height, int width)
        {
            int[] holder = new int[width];
            Bitmap bitmap = new Bitmap(width, height, PixelFormat.Format24bppRgb);
            for (int i = 0; i < height; ++i)
            {
                holder = calculateRow(width, height, i);
                for (int j = 0; j < width; j++)
                {
                    bitmap.SetPixel(j, i, Color.FromArgb(0, 0, holder[j]));
                }
            }
            return bitmap;
        }
        public static Bitmap useAkka(int height, int width)
        {
            //Initialize
            FractalBitmapHolder.isReady = false;
            ActorSystem actorSystem = ActorSystem.Create("main");
            IActorRef fractalActor = actorSystem.ActorOf(Props.Create(() => new FractalActor(height, width)), "FractalActor");
            fractalActor.Tell("Start");
            while (!FractalBitmapHolder.isReady)
            {
                //
            }
            return FractalBitmapHolder.fractalBitmap;             
        }
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);      
            Application.Run(new Form1());
        }
    }
}