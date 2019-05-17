using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ODBenchmark.Interfaces
{
    public class RecognitionResult
    {
        public string FileName { get; set; } = "";
        public bool MatchFound { get; set; } = false;
        public int StartX { get; set; } = 0;
        public int StartY { get; set; } = 0;
        public int EndX { get; set; } = 0;
        public int EndY { get; set; } = 0;
        public float Confidence { get; set; } = 0.0f;
        public int RecognitionPhase { get; set; } = 0;

        public override string ToString()
        {
            return $"{FileName};{MatchFound};{StartX};{StartY};{EndX};{EndY};{Confidence};{RecognitionPhase}";
        }
    }
}
