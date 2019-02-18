using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ODBenchmark.Interfaces
{
    public interface IObjectDetection
    {
        RecognitionResult Recognise(Image img);
    }
}
