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
    public static class FractalBitmapHolder
    {
        public static Bitmap fractalBitmap { get; set; }
        public static bool isReady { get; set; }
    }
}
