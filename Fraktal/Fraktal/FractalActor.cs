using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.Threading;
using System.Numerics;
using Akka;
using Akka.Actor;
using Akka.Routing;
using Akka.Configuration;
using System.Diagnostics;

namespace Fraktal
{
    public class FractalActor : ReceiveActor //COMMANDER!
    {
        public Bitmap bitmap;
        public int routees;
        public int height;
        public int width;
        public string localIP = "192.168.0.14";
        public string remoteIP = "192.168.0.17";
        
        //public Stopwatch watch;
        public FractalActor(int h, int w, int routees, string mode = "single") 
        {
            //setConfig();
            this.routees = routees;
            this.width = w;
            this.height = h;
            

            Receive<CalculatedRowData>(data =>
            {
                for (int j = 0; j < width; j++)
                {
                    bitmap.SetPixel(j, data.rowNum, Color.FromArgb(0, 0, data.rowData[j]));
                }
                this.height--;
                if (this.height == 0)
                {
                    //FractalBitmapHolder.fractalBitmap = bitmap;
                    //FractalBitmapHolder.isReady = true;
                    Self.Ask(Kill.Instance);
                }
            });

            Receive<CalculatedMultiRowData>(data =>
            {
                for (int i = data.rowFrom; i < data.rowTo; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        bitmap.SetPixel(j, i, Color.FromArgb(0, 0, data.rowsData[i - data.rowFrom][j]));
                    }
                    this.height--;
                }
                
                if (this.height == 0)
                {
                    Self.Ask(Kill.Instance);
                }
            });

            if (mode == "single")
            {
                RunSingle();
            }
            else if (mode == "local")
            {
                RunLocal();
            }
            else if (mode == "local2")
            {
                RunLocal2();
            }
            else if (mode == "single2")
            {
                RunSingle2();
            }
            //watch = new Stopwatch();
            //watch.Start();
            //Console.WriteLine("FractalActor Started");
        }
        
        public void RunSingle()
        {
            bitmap = new Bitmap(height, width);
            IActorRef calculateRowActors = Context.ActorOf(Props.Create(() => new RowActor())
                                          .WithRouter(new RoundRobinPool(routees)), "RowCalculators");

            for (int i = 0; i < height; i++)
            {
                calculateRowActors.Tell(new RowData(width, height, i));
            }
        }

        public void RunSingle2()
        {
            bitmap = new Bitmap(height, width);
            IActorRef calculateRowActors = Context.ActorOf(Props.Create(() => new MultiRowActor())
                                          .WithRouter(new RoundRobinPool(routees)), "RowCalculators");

            for (int i = 0; i < height; i+= 10)
            {
                calculateRowActors.Tell(new MultiRowData(width, height, i, i+10));
            }
        }


        public void RunLocal()
        {

            bitmap = new Bitmap(height, width);

            ActorSelection localSlave = Context.ActorSelection("akka.tcp://localsystem@" + localIP + ":8090/user/localmaster");
            ActorSelection remoteSlave = Context.ActorSelection("akka.tcp://remotesystem@" + remoteIP + ":8080/user/remotemaster");
            remoteSlave.Tell(new StartMessage(width, height, 0, height / 2, false));
            localSlave.Tell(new StartMessage(width, height, height / 2, height, false));
            
            
        }
        public void RunLocal2()
        {
            bitmap = new Bitmap(height, width);

            ActorSelection localSlave = Context.ActorSelection("akka.tcp://localsystem@" + localIP + ":8090/user/localmaster");
            ActorSelection remoteSlave = Context.ActorSelection("akka.tcp://remotesystem@" + remoteIP + ":8080/user/remotemaster");
            remoteSlave.Tell(new StartMessage(width, height, 0, height / 2, true));
            localSlave.Tell(new StartMessage(width, height, height / 2, height, true));


        }



        protected override void PostStop()
        {
            FractalBitmapHolder.fractalBitmap = bitmap;
            FractalBitmapHolder.isReady = true;
            //watch.Stop();
            //var elapsedMs = watch.ElapsedMilliseconds;
            //Console.WriteLine("FractalAgent finished in " + elapsedMs + "ms.");
        }
    }
}
