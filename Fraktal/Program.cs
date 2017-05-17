using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.Threading;
using System.Numerics;
using Akka;
using Akka.Actor;
using Akka.Configuration;
using Akka.Routing;
using System.Diagnostics;

namespace Fraktal
{
    public static class Program
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
                if (z.Imaginary * z.Imaginary + z.Real * z.Real >= 4.0) return iterations; 
                //jeśli warunek spełniony to oznacza że |z| rozbiegnie się do ∞ , lub gdy osiągniemy maksymalną liczbę iteracji.
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
                
                row[i] = (char)(255 - calculatePixel(x, y)); 
                //Jasność piksela równa 255 - liczba iteracji (proporcjonalna do liczby iteracji pętli)
            }
            return row;
        }
        
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

        public static Bitmap useAkka(int height, int width, string mode)
        {
            //Initialize
            FractalBitmapHolder.isReady = false;
            ActorSystem actorSystem = ActorSystem.Create("local");
            IActorRef fractalActor = actorSystem.ActorOf(Props.Create(() => new FractalActor(height, width, 4, mode)), "FractalActor");
            fractalActor.Tell("Start");
            while (!FractalBitmapHolder.isReady)
            {
                //wait
            }
            return FractalBitmapHolder.fractalBitmap;             
        }

        public static Bitmap runLocal(int height, int width, string ip, string mode)
        {
            if (ip == "")
            {
                ip = "localhost";
            }
            FractalBitmapHolder.isReady = false;

            string localIP = ip;
            Config configlocal  = ConfigurationFactory.ParseString(@"
            akka {
                actor {
                    provider = ""Akka.Remote.RemoteActorRefProvider, Akka.Remote""
                    serializers {
                          hyperion = ""Akka.Serialization.HyperionSerializer, Akka.Serialization.Hyperion""
                    }
                    serialization-bindings {
                      ""System.Object"" = hyperion
                    }
                }
                remote {
                    dot-netty.tcp {
                        port = 8090
                        hostname = " + localIP + @"
                        maximum-frame-size = 30000000
                    }
                }
            }");

            var localSystem = ActorSystem.Create("localsystem", configlocal);
            localSystem.ActorOf(Props.Create(() => new MasterActor()), "localmaster");
            IActorRef fractalActor = localSystem.ActorOf(Props.Create(() => new FractalActor(height, width, 4, mode)), "FractalActor");
            fractalActor.Tell("Start");

            while (!FractalBitmapHolder.isReady)
            {
                //wait
            }
            return FractalBitmapHolder.fractalBitmap;

        }

        public static void runRemote(string ip)
        {
            if (ip == "")
            {
                ip = "localhost";
            }
            string remoteIP = ip;
            Config configremote = ConfigurationFactory.ParseString(@"
            akka {
                actor {
                    provider = ""Akka.Remote.RemoteActorRefProvider, Akka.Remote""
                    serializers {
                          hyperion = ""Akka.Serialization.HyperionSerializer, Akka.Serialization.Hyperion""
                    }
                    serialization-bindings {
                      ""System.Object"" = hyperion
                    }
                }
                remote {
                    dot-netty.tcp {
                        port = 8080
                        hostname = " + remoteIP + @"
                        maximum-frame-size = 30000000
                    }
                }
            }");
            var remoteSystem = ActorSystem.Create("remotesystem", configremote);
            remoteSystem.ActorOf(Props.Create(() => new MasterActor()), "remotemaster");
            //remoteSystem.ActorOf(Props.Create(() => new RowActor()).WithRouter(new RoundRobinPool(4)), "RowCalculatorsRemote");
        }
    }
}