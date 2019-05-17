using ODBenchmark.Azure;
using ODBenchmark.Frequency;
using ODBenchmark.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;
using System.IO;

namespace ODBenchmark
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Controller _controller;

        public MainWindow()
        {
            InitializeComponent();
            _controller = new Controller()
            {
                ODControlPanel = ODPanel,
            };

            PanelCB.SelectionChanged += (sender, eventArgs) =>
            {
                ODPanel.Children.Clear();
                switch(((ComboBoxItem)PanelCB.SelectedItem).Name)
                {
                    case "Azure":
                        _controller.AzurePanelButtonClick();
                        break;
                    case "Frequency":
                        _controller.FrequencyPanelButtonClick();
                        break;
                    default:
                        break;
                }
            };

            StartProcessButton.Click += (sender, eventArgs) =>
            {
                _controller.StartProcess(InputFolderTB.Text, OutputFolderTB.Text);
                System.Windows.MessageBox.Show("Done");
            };

            InputFolderButton.Click += (sender, eventArgs) =>
            {
                using (var dialog = new FolderBrowserDialog())
                {
                    if (Directory.Exists(InputFolderTB.Text))
                        dialog.SelectedPath = InputFolderTB.Text;
                    DialogResult result = dialog.ShowDialog();
                    if (result == System.Windows.Forms.DialogResult.OK)
                        InputFolderTB.Text = dialog.SelectedPath;
                }                
            };

            OutputFolderButton.Click += (sender, eventArgs) =>
            {
                using (var dialog = new FolderBrowserDialog())
                {
                    if (Directory.Exists(OutputFolderTB.Text))
                        dialog.SelectedPath = OutputFolderTB.Text;
                    DialogResult result = dialog.ShowDialog();
                    if (result == System.Windows.Forms.DialogResult.OK)
                        OutputFolderTB.Text = dialog.SelectedPath;
                }
            };
        }
    }
}
