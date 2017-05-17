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
using System.Diagnostics;

namespace Fraktal
{
    public class RowActor : ReceiveActor
    {
        public RowActor()
        {
            this.Receive<RowData>(data =>
            {
                //Stopwatch watch = Stopwatch.StartNew();
                int[] row = Program.calculateRow(data.w, data.h, data.rowNum);
                this.Sender.Tell(new CalculatedRowData(data.rowNum, row));
                
                //watch.Stop();
                //var elapsedMs = watch.ElapsedMilliseconds;
                //Console.WriteLine("RowActor finished in " + elapsedMs + "ms.");
            });
        }
    }
}
