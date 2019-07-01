using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Drawing;

namespace ODBenchmark.Interfaces
{
    public interface IObjectDetectionPanel
    {
        void ShowOnPanel(Panel panel);
        bool PreprocessSetup();
        Task<RecognitionResult> Recogise(string fileName, string outPath);
    }
}
