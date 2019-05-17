using ODBenchmark.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Controls;

namespace ODBenchmark.Azure
{
    public class AzureObjectDetectionPanel : IObjectDetectionPanel
    {
        private AzureObjectDetection _azureObjectDetection;

        private Panel _azurePanel;

        public AzureObjectDetectionPanel()
        {
            _azureObjectDetection = new AzureObjectDetection();
            //Fill to create panel
            _azurePanel = new StackPanel();
        }

        public bool PreprocessSetup()
        {
            try
            {

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<RecognitionResult> Recogise(System.Drawing.Image img, string fileName)
        {
            return await Task.Run(() => _azureObjectDetection.Recognise(img));
        }

        public void ShowOnPanel(Panel panel)
        {
            panel.Children.Add(_azurePanel);
        }
    }
}
