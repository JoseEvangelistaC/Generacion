﻿namespace Generacion.Models.ION
{

    public class Revenue : DatosION
    {
        public string Date_Time { get; set; }
        public decimal kWhDelInt { get; set; }
        public decimal kVARhDelInt { get; set; }
        public decimal kWhRecInt { get; set; }
        public decimal kVARhRecInt { get; set; }
        public decimal VllAvg { get; set; }
        public decimal Freq { get; set; }

    }

    public class DatosION
    {
        public decimal KWDelInt { get; set; }
        public decimal KVARDelInt { get; set; }
        public decimal KWRecInt { get; set; }
        public decimal KVARRecInt { get; set; }
    }
}
