using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ODBenchmark.Interfaces
{
    public class RecognitionResult
    {
        public bool MatchFound { get; set; }
        public int StartX { get; set; }
        public int StartY { get; set; }
        public int EndX { get; set; }
        public int EndY { get; set; }
        public float Confidence { get; set; }

        public override string ToString()
        {
            return $"{MatchFound};{StartX};{StartY};{EndX};{EndY};{Confidence}";
        }
    }
}
