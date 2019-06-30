using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using ODBenchmark.Interfaces;
using CsvHelper;

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
        private TextBox _additionalDataPathTB;

        private int _targetX;
        private int _targetY;
        private int _targetHistogramSize;
        private float _sigma;
        private bool _orginalMeasure;
        private int _r;
        private float _threshold;
        private float _accuracy;
        private string _modelImgPath;
        private string _additionalDataPath;

        private HarrisCornerDetection _harris;
        private RecognitionModelHistogram _model;
        private List<ImageAdditionalData> _additionalImageDatas;

        public FrequencyObjectDetectionPanel()
        {
            _harris = new HarrisCornerDetection();
            _additionalImageDatas = new List<ImageAdditionalData>();

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

            var additionalModelData = new Label();
            additionalModelData.Content = "Additional data";
            Grid.SetColumn(additionalModelData, 0);
            Grid.SetRow(additionalModelData, 9);
            _frequencyPanel.Children.Add(additionalModelData);

            #endregion

            #region Inputs
            _sigmaTB = new TextBox();
            _sigmaTB.Text = "11";
            Grid.SetColumn(_sigmaTB, 1);
            Grid.SetRow(_sigmaTB, 0);
            _frequencyPanel.Children.Add(_sigmaTB);

            _orginalMeasureCB = new CheckBox();
            _orginalMeasureCB.IsChecked = true;
            Grid.SetColumn(_orginalMeasureCB, 1);
            Grid.SetRow(_orginalMeasureCB, 1);
            _frequencyPanel.Children.Add(_orginalMeasureCB);

            _thresholdTB = new TextBox();
            _thresholdTB.Text = "50.0";
            Grid.SetColumn(_thresholdTB, 1);
            Grid.SetRow(_thresholdTB, 2);
            _frequencyPanel.Children.Add(_thresholdTB);

            _radiusTB = new TextBox();
            _radiusTB.Text = "5";
            Grid.SetColumn(_radiusTB, 1);
            Grid.SetRow(_radiusTB, 3);
            _frequencyPanel.Children.Add(_radiusTB);

            _accuracyTB = new TextBox();
            _accuracyTB.Text = "0.6";
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

            WrapPanel wrapPanel2 = new WrapPanel();
            Grid.SetColumn(wrapPanel2, 1);
            Grid.SetRow(wrapPanel2, 9);
            _frequencyPanel.Children.Add(wrapPanel2);

            _additionalDataPathTB = new TextBox();
            _additionalDataPathTB.Width = 225;
            wrapPanel2.Children.Add(_additionalDataPathTB);

            var path2Button = new Button();
            path2Button.Content = "Model image path";
            path2Button.Margin = new System.Windows.Thickness(3, 0, 0, 0);
            path2Button.Click += (sender, eventArgs) =>
            {
                var dlg = new Microsoft.Win32.OpenFileDialog();
                dlg.Filter = "All Files|*.txt;*.csv;";
                var result = dlg.ShowDialog();
                if (result == true)
                {
                    string filename = dlg.FileName;
                    _additionalDataPathTB.Text = filename;
                }
            };
            wrapPanel2.Children.Add(path2Button);
            #endregion
        }

        public bool PreprocessSetup()
        {
            try
            {
                _targetX = int.Parse(_targetSizeXTB.Text);
                _targetY = int.Parse(_targetSizeYTB.Text);
                _targetHistogramSize = int.Parse(_targetHistogramSizeTB.Text);
                _threshold = float.Parse(_thresholdTB.Text, CultureInfo.InvariantCulture);
                _r = int.Parse(_radiusTB.Text);
                _orginalMeasure = _orginalMeasureCB.IsChecked.Value;
                _sigma = float.Parse(_sigmaTB.Text, CultureInfo.InvariantCulture);
                _accuracy = float.Parse(_accuracyTB.Text, CultureInfo.InvariantCulture);
                _modelImgPath = _modelPathTB.Text;
                _additionalDataPath = _additionalDataPathTB.Text;

                _additionalImageDatas = File.ReadAllLines(_additionalDataPath).Select(row => row.Split(';')).Select(dataInRow =>
                {
                    return new ImageAdditionalData
                    {
                        WorldModelRotation = float.Parse(dataInRow[0], CultureInfo.InvariantCulture),
                        WorldModelVerticalTilt = float.Parse(dataInRow[1], CultureInfo.InvariantCulture),
                        Tilt = float.Parse(dataInRow[2], CultureInfo.InvariantCulture),
                        Latitude = float.Parse(dataInRow[3], CultureInfo.InvariantCulture),
                        Longitude = float.Parse(dataInRow[4], CultureInfo.InvariantCulture),
                        FileName = dataInRow[5]
                    };
                }).ToList();

                _harris.orginalMeasure = _orginalMeasure;
                _harris.sigma = _sigma;
                _harris.r = _r;
                _harris.threshold = _threshold;
                _harris.Start();
                _model = new RecognitionModelHistogram(_targetHistogramSize, _targetY, _targetX);
                SetModel();
                return true;
            }
            catch(Exception e)
            {
                return false;
            }
        }

        public async Task<RecognitionResult> Recogise( string filePath)
        {
            return await RecogniseImage(filePath);
        }

        public void ShowOnPanel(Panel panel)
        {
            panel.Children.Add(_frequencyPanel);
        }

        private async Task<RecognitionResult> RecogniseImage(string filePath)
        {
            var fileName = Path.GetFileName(filePath);
            var additionalData = _additionalImageDatas.FirstOrDefault(data => data.FileName == fileName);
            if (additionalData == null)
                return new RecognitionResult();
            additionalData.NormalizeTilts();
            var recognitionPhase = _model.IsViableForImageRecognition(additionalData.WorldModelRotation, additionalData.WorldModelVerticalTilt, additionalData.Latitude, additionalData.Longitude);
            if (recognitionPhase != 0)
                return new RecognitionResult() { RecognitionPhase = recognitionPhase };
            //Process image
            var img = System.Drawing.Image.FromFile(filePath);
            var modelImage = ScaleImage(img, _targetY, _targetX);
            var points = _harris.ProcessImage(modelImage, _targetY, _targetX);
            SaveScaledImageWithPoints(modelImage, points, filePath);
            List<IntPoint>[,] recognition = new List<IntPoint>[_targetHistogramSize, _targetHistogramSize];
            for (var y = 0; y < _targetHistogramSize; y++)
            {
                for (var x = 0; x < _targetHistogramSize; x++)
                {
                    recognition[y, x] = new List<IntPoint>();
                }
            }
            float histogramRatioX = (float)_targetX / (float)_targetHistogramSize;
            float histogramRatioY = (float)_targetY / (float)_targetHistogramSize;
            foreach (var p in points)
            {
                recognition[(int)(p.Y / histogramRatioX), (int)(p.X / histogramRatioY)].Add(p);
            }

            //For recognition
            var recognitionResult = _model.Recognise(recognition, _accuracy, additionalData.Tilt);
            var result = new RecognitionResult();
            result.MatchFound = recognitionResult.ModelFound;
            result.Confidence = recognitionResult.RecognitionProb;
            if (recognitionResult.ModelFound)
            {
                result.StartX = (int)(recognitionResult.StartOfFoundModel.X * (img.Width / (float)_targetHistogramSize));
                result.StartY = (int)(recognitionResult.StartOfFoundModel.Y * (img.Height / (float)_targetHistogramSize));
                result.EndX = (int)(recognitionResult.EndOfFoundModel.X * (img.Width / (float)_targetHistogramSize));
                result.EndY = (int)(recognitionResult.EndOfFoundModel.Y * (img.Height / (float)_targetHistogramSize));
            }
            return result;
        }

        private byte[] ScaleImage(System.Drawing.Image img, int targetY, int targetX)
        {
            byte[] resultImage = new byte[targetX * targetY];
            byte[] source;
            using (var ms = new MemoryStream())
            {
                img.Save(ms, ImageFormat.Bmp);
                source = ms.ToArray();
            }
            var xRatio = (float)img.Width / (float)targetX;
            var yRatio = (float)img.Height / (float)targetY;
            for (int y = 0; y < targetY; y++)
            {
                for (int x = 0; x < targetX; x++)
                {
                    var index = ((int)(y * yRatio) * img.Width) + (int)(x * xRatio) + 20;//(source[10] / 8)/*BMP Header offset*/;
                    resultImage[(y * targetX) + x] = (byte)(/*R*/(source[index * 3] * 0.2989) + /*G*/(source[(index * 3) + 1] * 0.5870) + /*B*/(source[(index * 3) + 2] * 0.1140));
                }
            }
            return resultImage;
        }

        private void SaveScaledImageWithPoints(byte[] grayImg, List<IntPoint> points, string path)
        {
            path = path.Replace(Path.GetExtension(path), "out.jpg");
            var gray24 = new byte[grayImg.Length * 3];
            
            Bitmap bmp = new Bitmap(_targetX, _targetY, PixelFormat.Format24bppRgb);
            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            BitmapData bmpData = bmp.LockBits(rect, ImageLockMode.ReadWrite, bmp.PixelFormat);

            IntPtr ptr = bmpData.Scan0;
            System.Runtime.InteropServices.Marshal.Copy(ptr, gray24, 0, gray24.Length);
            for (int i = 0; i < gray24.Length; i++)
                gray24[i] = grayImg[i / 3];
            System.Runtime.InteropServices.Marshal.Copy(gray24, 0, ptr, gray24.Length);
            // Unlock the bits.
            bmp.UnlockBits(bmpData);
            bmp.Save(path);
        }

        private void SetModel()
        {
            var modelImage = ScaleImage(System.Drawing.Image.FromFile(_modelImgPath), _targetY, _targetX);
            string fileName = Path.GetFileName(_modelImgPath);
            var additionalData = _additionalImageDatas.FirstOrDefault(data => data.FileName == fileName);
            additionalData.NormalizeTilts();
            var points = _harris.ProcessImage(modelImage, _targetY, _targetX);
            List<IntPoint>[,] recognition = new List<IntPoint>[_targetHistogramSize, _targetHistogramSize];
            for (var y = 0; y < _targetHistogramSize; y++)
            {
                for (var x = 0; x < _targetHistogramSize; x++)
                {
                    recognition[y, x] = new List<IntPoint>();
                }
            }
            float histogramRatioX = (float)_targetX / (float)_targetHistogramSize;
            float histogramRatioY = (float)_targetY / (float)_targetHistogramSize;
            foreach (var p in points)
            {
                recognition[(int)(p.Y / histogramRatioX), (int)(p.X / histogramRatioY)].Add(p);
            }
            //For model setting
            _model.SetModel(recognition, additionalData.Tilt, additionalData.WorldModelRotation, additionalData.WorldModelVerticalTilt, additionalData.Latitude, additionalData.Longitude);
            _model.CalculatePatern();
        }
    }
}
