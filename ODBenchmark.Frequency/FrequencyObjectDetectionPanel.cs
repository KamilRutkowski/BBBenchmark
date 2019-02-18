﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using ODBenchmark.Interfaces;

namespace ODBenchmark.Frequency
{
    public class FrequencyObjectDetectionPanel: IObjectDetectionPanel
    {
        private FrequencyObjectDetection _frequencyObjectDetection;

        private Grid _frequencyPanel;        
        private TextBox _sigmaTB;
        private CheckBox _orginalMeasureCB;
        private TextBox _threshold;
        private TextBox _radiusTB;
        private TextBox _accuracyTB;
        private TextBox _targetSizeXTB;
        private TextBox _targetSizeYTB;
        private TextBox _targetHistogramSizeTB;
        private TextBox _modelPathTB;

        public FrequencyObjectDetectionPanel()
        {
            _frequencyObjectDetection = new FrequencyObjectDetection();

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
            _frequencyObjectDetection = new FrequencyObjectDetection();
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

            _threshold = new TextBox();
            _threshold.Text = "50";
            Grid.SetColumn(_threshold, 1);
            Grid.SetRow(_threshold, 2);
            _frequencyPanel.Children.Add(_threshold);

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

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<RecognitionResult> Recogise(System.Drawing.Image img)
        {
            return await Task.Run(() => _frequencyObjectDetection.Recognise(img));
        }

        public void ShowOnPanel(Panel panel)
        {
            panel.Children.Add(_frequencyPanel);
        }
    }
}