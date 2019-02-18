using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using ODBenchmark.Interfaces;

namespace ODBenchmark.Frequency
{
    public class FrequencyObjectDetectionPanel: IObjectDetectionPanel
    {
        private Grid _frequencyPanel;        
        private TextBox _sigmaTB;
        private CheckBox _orginalMeasureCB;
        private TextBox _thresholdTB;
        private TextBox _radiusTB;
        private TextBox _accuracyTB;
        private TextBox _targetSizeXTB;
        private TextBox _targetSizeYTB;
        private TextBox _targetHistogramSizeTB;
        private TextBox _modelPathTB;

        private HarrisCornerDetection _harris;
        private RecognitionModelHistogram _model;

        public FrequencyObjectDetectionPanel()
        {
            _harris = new HarrisCornerDetection();
            #region Grid definition
            _frequencyPanel = new Grid();
            _frequencyPanel.ColumnDefinitions.Add(new ColumnDefinition());
            _frequencyPanel.ColumnDefinitions.Add(new ColumnDefinition());
            _frequencyPanel.RowDefinitions.Add(new RowDefinition());
            _frequencyPanel.RowDefinitions.Add(new RowDefinition());
            _frequencyPanel.RowDefinitions.Add(new RowDefinition());
            _frequencyPanel.RowDefinitions.Add(new RowDefinition());
            _frequencyPanel.RowDefinitions.Add(new RowDefinition());
            _frequencyPanel.RowDefinitions.Add(new RowDefinition());
            _frequencyPanel.RowDefinitions.Add(new RowDefinition());
            _frequencyPanel.RowDefinitions.Add(new RowDefinition());
            _frequencyPanel.RowDefinitions.Add(new RowDefinition());
            #endregion

            #region Labels
            var sigmaLabel = new Label();
            sigmaLabel.Content = "Sigma";
            Grid.SetColumn(sigmaLabel, 0);
            Grid.SetRow(sigmaLabel, 0);
            _frequencyPanel.Children.Add(sigmaLabel);

            var orginalMeasureLabel = new Label();
            orginalMeasureLabel.Content = "Orginal Measure";
            Grid.SetColumn(orginalMeasureLabel, 0);
            Grid.SetRow(orginalMeasureLabel, 1);
            _frequencyPanel.Children.Add(orginalMeasureLabel);

            var thresholdLabel = new Label();
            thresholdLabel.Content = "Threshold";
            Grid.SetColumn(thresholdLabel, 0);
            Grid.SetRow(thresholdLabel, 2);
            _frequencyPanel.Children.Add(thresholdLabel);

            var radiusLabel = new Label();
            radiusLabel.Content = "Radius";
            Grid.SetColumn(radiusLabel, 0);
            Grid.SetRow(radiusLabel, 3);
            _frequencyPanel.Children.Add(radiusLabel);

            var accuracyLabel = new Label();
            accuracyLabel.Content = "Accuracy";
            Grid.SetColumn(accuracyLabel, 0);
            Grid.SetRow(accuracyLabel, 4);
            _frequencyPanel.Children.Add(accuracyLabel);

            var targetResolutionXLabel = new Label();
            targetResolutionXLabel.Content = "Target resolution X";
            Grid.SetColumn(targetResolutionXLabel, 0);
            Grid.SetRow(targetResolutionXLabel, 5);
            _frequencyPanel.Children.Add(targetResolutionXLabel);

            var targetResolutionYLabel = new Label();
            targetResolutionYLabel.Content = "Target resolution Y";
            Grid.SetColumn(targetResolutionYLabel, 0);
            Grid.SetRow(targetResolutionYLabel, 6);
            _frequencyPanel.Children.Add(targetResolutionYLabel);

            var targetHistogramSizeLabel = new Label();
            targetHistogramSizeLabel.Content = "Target histogram size";
            Grid.SetColumn(targetHistogramSizeLabel, 0);
            Grid.SetRow(targetHistogramSizeLabel, 7);
            _frequencyPanel.Children.Add(targetHistogramSizeLabel);

            var modelPictureLabel = new Label();
            modelPictureLabel.Content = "Model picture";
            Grid.SetColumn(modelPictureLabel, 0);
            Grid.SetRow(modelPictureLabel, 8);
            _frequencyPanel.Children.Add(modelPictureLabel);


            #endregion

            #region Inputs
            _sigmaTB = new TextBox();
            _sigmaTB.Text = "3";
            Grid.SetColumn(_sigmaTB, 1);
            Grid.SetRow(_sigmaTB, 0);
            _frequencyPanel.Children.Add(_sigmaTB);

            _orginalMeasureCB = new CheckBox();
            _orginalMeasureCB.IsChecked = true;
            Grid.SetColumn(_orginalMeasureCB, 1);
            Grid.SetRow(_orginalMeasureCB, 1);
            _frequencyPanel.Children.Add(_orginalMeasureCB);

            _thresholdTB = new TextBox();
            _thresholdTB.Text = "50,0";
            Grid.SetColumn(_thresholdTB, 1);
            Grid.SetRow(_thresholdTB, 2);
            _frequencyPanel.Children.Add(_thresholdTB);

            _radiusTB = new TextBox();
            _radiusTB.Text = "3";
            Grid.SetColumn(_radiusTB, 1);
            Grid.SetRow(_radiusTB, 3);
            _frequencyPanel.Children.Add(_radiusTB);

            _accuracyTB = new TextBox();
            _accuracyTB.Text = "0.8";
            Grid.SetColumn(_accuracyTB, 1);
            Grid.SetRow(_accuracyTB, 4);
            _frequencyPanel.Children.Add(_accuracyTB);

            _targetSizeXTB = new TextBox();
            _targetSizeXTB.Text = "600";
            Grid.SetColumn(_targetSizeXTB, 1);
            Grid.SetRow(_targetSizeXTB, 5);
            _frequencyPanel.Children.Add(_targetSizeXTB);

            _targetSizeYTB = new TextBox();
            _targetSizeYTB.Text = "600";
            Grid.SetColumn(_targetSizeYTB, 1);
            Grid.SetRow(_targetSizeYTB, 6);
            _frequencyPanel.Children.Add(_targetSizeYTB);

            _targetHistogramSizeTB = new TextBox();
            _targetHistogramSizeTB.Text = "21";
            Grid.SetColumn(_targetHistogramSizeTB, 1);
            Grid.SetRow(_targetHistogramSizeTB, 7);
            _frequencyPanel.Children.Add(_targetHistogramSizeTB);
            
            WrapPanel wrapPanel = new WrapPanel();
            Grid.SetColumn(wrapPanel, 1);
            Grid.SetRow(wrapPanel, 8);
            _frequencyPanel.Children.Add(wrapPanel);

            _modelPathTB = new TextBox();
            _modelPathTB.Width = 225;
            wrapPanel.Children.Add(_modelPathTB);

            var pathButton = new Button();
            pathButton.Content = "Model image path";
            pathButton.Margin = new System.Windows.Thickness(3, 0, 0, 0);
            pathButton.Click += (sender, eventArgs) =>
            {
                var dlg = new Microsoft.Win32.OpenFileDialog();   
                dlg.Filter = "All Files|*.jpeg;*.png;*.jpg;*.bmp|JPEG Files (*.jpeg)|*.jpeg|PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg|Bitmap Files (*.bmp)|*.bmp";                
                var result = dlg.ShowDialog();
                if (result == true)
                {
                    string filename = dlg.FileName;
                    _modelPathTB.Text = filename;
                }
            };
            wrapPanel.Children.Add(pathButton);            
            #endregion
        }

        public bool PreprocessSetup()
        {
            try
            {
                _harris.orginalMeasure = _orginalMeasureCB.IsChecked.Value;
                _harris.sigma = float.Parse(_sigmaTB.Text);
                _harris.r = int.Parse(_radiusTB.Text);
                _harris.threshold = float.Parse(_thresholdTB.Text);
                _harris.Start();
                _model = new RecognitionModelHistogram(int.Parse(_targetHistogramSizeTB.Text), int.Parse(_targetSizeYTB.Text), int.Parse(_targetSizeXTB.Text));
                SetModel();
                return true;
            }
            catch(Exception e)
            {
                return false;
            }
        }

        public async Task<RecognitionResult> Recogise(System.Drawing.Image img)
        {
            return await Task.Run(() => RecogniseImage(img));
        }

        public void ShowOnPanel(Panel panel)
        {
            panel.Children.Add(_frequencyPanel);
        }

        private async Task<RecognitionResult> RecogniseImage(System.Drawing.Image img)
        {
            return new RecognitionResult();
        }

        private byte[] ScaleImage(System.Drawing.Image img, int targetY, int targetX)
        {
            byte[] resultImage = new byte[targetX * targetY];
            byte[] source;
            using (var ms = new MemoryStream())
            {
                img.Save(ms, img.RawFormat);
                source = ms.ToArray();
            }            
            var xRatio = img.Width / targetX;
            var yRatio = img.Height / targetY;
            for (int y = 0; y < targetY; y++)
            {
                for (int x = 0; x < targetX; x++)
                {
                    var index = (int)((y * img.Width) * yRatio) + (int)(x * xRatio);
                    resultImage[y * targetX + x] = (byte)(/*R*/(source[index * 3] * 0.2989) + /*G*/(source[(index * 3) + 1] * 0.5870) + /*B*/(source[(index * 3) + 2] * 0.1140));
                }
            }
            return resultImage;
        }

        private void SetModel()
        {
            var modelImage = ScaleImage(System.Drawing.Image.FromFile(_modelPathTB.Text), int.Parse(_targetSizeYTB.Text), int.Parse(_targetSizeXTB.Text));
            var points = _harris.ProcessImage(modelImage, int.Parse(_targetSizeXTB.Text), int.Parse(_targetSizeYTB.Text));
            var histogramSize = int.Parse(_targetHistogramSizeTB.Text);
            List<IntPoint>[,] recognition = new List<IntPoint>[histogramSize, histogramSize];
            for (var y = 0; y < histogramSize; y++)
            {
                for (var x = 0; x < histogramSize; x++)
                {
                    recognition[y, x] = new List<IntPoint>();
                }
            }
            float histogramRatioX = (float)int.Parse(_targetSizeXTB.Text) / (float)histogramSize;
            float histogramRatioY = (float)int.Parse(_targetSizeYTB.Text) / (float)histogramSize;
            foreach (var p in points)
            {
                recognition[(int)(p.Y / histogramRatioX), (int)(p.X / histogramRatioY)].Add(p);
            }
            _model.SetModel(recognition, 0.0f);
            _model.CalculatePatern();
        }
    }
}
