using ODBenchmark.Azure;
using ODBenchmark.Frequency;
using ODBenchmark.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Drawing;

namespace ODBenchmark
{
    public class Controller
    {
        public Panel ODControlPanel;

        private AzureObjectDetectionPanel _azurePanel;
        private FrequencyObjectDetectionPanel _frequencyPanel;

        public Controller()
        {
            _azurePanel = new AzureObjectDetectionPanel();
            _frequencyPanel = new FrequencyObjectDetectionPanel();
        }

        public void AzurePanelButtonClick()
        {
            ODControlPanel.Children.Clear();
            _azurePanel.ShowOnPanel(ODControlPanel);
        }

        public void FrequencyPanelButtonClick()
        {
            ODControlPanel.Children.Clear();
            _frequencyPanel.ShowOnPanel(ODControlPanel);
        }

        public void StartProcess(string inputFolderPath, string outputDirectoryPath)
        {
            if (_azurePanel.PreprocessSetup() && _frequencyPanel.PreprocessSetup())
            {
                //Read from image collection
                var imageFiles = Directory.GetFiles(inputFolderPath).Where(path => path.ToLower().EndsWith(".jpg") || path.ToLower().EndsWith(".jpeg") || path.ToLower().EndsWith(".png") || path.ToLower().EndsWith(".bmp")).ToArray();

                //var azureResults = new List<RecognitionResult>(imageFiles.Length);
                var freqResults = new List<RecognitionResult>(imageFiles.Length);

                foreach (var imagePath in imageFiles)
                {
                    var img = System.Drawing.Image.FromFile(imagePath);
                    var tasks = new Task<RecognitionResult>[1];
                    //tasks[0] = _azurePanel.Recogise(img);
                    tasks[0] = Task.Run(() => _frequencyPanel.Recogise(img));
                    Task.WaitAll(tasks);
                    //azureResults.Add(tasks[0].Result);
                    freqResults.Add(tasks[0].Result);
                }
                //Save results
                //File.WriteAllLines(Path.Combine(outputDirectoryPath, "azure.txt"), azureResults.Select(res => res.ToString()));
                File.WriteAllLines(Path.Combine(outputDirectoryPath, "freq.txt"), freqResults.Select(res => res.ToString()));
            }
            else
                MessageBox.Show("There was an error in preprocess setup!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
