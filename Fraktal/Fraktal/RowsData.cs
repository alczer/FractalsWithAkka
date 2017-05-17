using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fraktal
{
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

    public struct MultiRowData
    {
        public int w;
        public int h;
        public int rowFrom;
        public int rowTo;
        public MultiRowData(int _w, int _h, int _rowFrom, int _rowTo)
        {
            w = _w;
            h = _h;
            rowFrom = _rowFrom;
            rowTo = _rowTo;
        }

    }

    public struct CalculatedMultiRowData
    {
        public int rowFrom;
        public int rowTo;
        public int[][] rowsData;
        public CalculatedMultiRowData(int rowFrom, int rowTo, int[][] data)
        {
            this.rowFrom = rowFrom;
            this.rowTo = rowTo;
            rowsData = data;
        }
    }

}
