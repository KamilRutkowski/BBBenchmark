using ODBenchmark.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ODBenchmark.Azure
{
    public class AzureObjectDetection : IObjectDetection
    {
        public RecognitionResult Recognise(Image img)
        {
            //throw new NotImplementedException();
            return new RecognitionResult();
        }
    }
}
