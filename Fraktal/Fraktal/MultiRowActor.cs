using Akka;
using Akka.Actor;
using Akka.Routing;
using System.Diagnostics;

namespace Fraktal
{
    class MultiRowActor : ReceiveActor
    {
        public MultiRowActor()
        {
            this.Receive<MultiRowData>(data =>
            {
                //Stopwatch watch = Stopwatch.StartNew();

                int[][] rows = new int[data.rowTo - data.rowFrom][];
                for (int i = 0; i < data.rowTo - data.rowFrom; i++)
                {
                    rows[i] = Program.calculateRow(data.w, data.h, data.rowFrom + i);
                }
                this.Sender.Tell(new CalculatedMultiRowData(data.rowFrom, data.rowTo, rows));
                //watch.Stop();
                //var elapsedMs = watch.ElapsedMilliseconds;
                //Console.WriteLine("RowActor finished in " + elapsedMs + "ms.");
            });
        }
    }

}
