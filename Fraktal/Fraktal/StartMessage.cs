using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fraktal
{        
    class StartMessage
    {
        public int width;
        public int height;
        public int startRow;
        public int endRow;
        public bool mode;

        public StartMessage(int width, int height, int startRow, int endRow, bool mode)
        {
            this.width = width;
            this.height = height;
            this.startRow = startRow;
            this.endRow = endRow;
            this.mode = mode;
        }
    }
}
