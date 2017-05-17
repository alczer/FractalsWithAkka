using Akka.Actor;
using Akka.Routing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Fraktal
{
    public class MasterActor : ReceiveActor
    {
        private IActorRef superMaster;
        
        public int routees;
        public int height;
        public int width;
        public int startRow;
        public int endRow;
        private int rows;

        public MasterActor()
        {
            Receive<StartMessage>(data =>
            {
                this.Sender.Tell(data);
                this.startRow = data.startRow;
                this.endRow = data.endRow;
                this.width = data.width;
                this.height = data.height;
                superMaster = Sender;
                rows = endRow - startRow;
                

                if (data.mode == true)
                {
                    IActorRef calculateRowActors = Context.ActorOf(Props.Create(() => new MultiRowActor())
                                          .WithRouter(new RoundRobinPool(4)), "MultiRowCalculators");
                    for (int i = startRow; i < endRow; i += 10)
                    {
                        calculateRowActors.Tell(new MultiRowData(width, height, i, i + 10));
                    }
                }
                else
                {
                    IActorRef calculateRowActors = Context.ActorOf(Props.Create(() => new RowActor())
                                          .WithRouter(new RoundRobinPool(4)), "RowCalculators");
                    for (int i = startRow; i < endRow; i++)
                    {
                        calculateRowActors.Tell(new RowData(width, height, i));
                    }
                }

            });

            Receive<CalculatedMultiRowData>(data =>
            {
                this.superMaster.Tell(data);

                this.rows -= (data.rowTo - data.rowFrom);
                if (this.rows == 0)
                {
                    //FractalBitmapHolder.fractalBitmap = bitmap;
                    //FractalBitmapHolder.isReady = true;
                    Self.Ask(Kill.Instance);
                }
            });


            Receive<CalculatedRowData>(data =>
            {
                this.superMaster.Tell(data);

                this.rows--;
                if (this.rows == 0)
                {
                    //FractalBitmapHolder.fractalBitmap = bitmap;
                    //FractalBitmapHolder.isReady = true;
                    Self.Ask(Kill.Instance);
                }
            });
        }
    }
}
