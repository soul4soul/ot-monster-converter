using System;
using System.Collections.Generic;
using System.Data;
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

namespace OTMonsterConverter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DataTable monsterListDataTable;
        private MonsterFileProcessor fileProcessor;

        public MainWindow()
        {
            InitializeComponent();
            fileProcessor = new MonsterFileProcessor();
            fileProcessor.OnMonsterConverted += fileProcessor_OnMonsterConverted;
        }

        private void fileProcessor_OnMonsterConverted(object sender, FileProcessorEventArgs e)
        {
            if (e != null)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    monsterListDataTable.Rows.Add(new object[] { System.IO.Path.GetFileName(e.SourceMonsterFile), e.DestinationFile, e.ConvertedSuccessfully });
                    dataGridResults.ScrollIntoView(dataGridResults.Items[dataGridResults.Items.Count - 1]);
                });
            }
        }

        private bool ValidateControls()
        {
            bool result = ((textBoxInputPath.Text != "") &&
                           (textBoxOutputPath.Text != "") &&
                           (comboInputFormat.SelectedItem != null) &&
                           (comboOutputFormat.SelectedItem != null));
            if (buttonConvert != null)
            {
                buttonConvert.IsEnabled = result;
            }
            return result;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ValidateControls();
            monsterListDataTable = new DataTable("MonsterList");
            monsterListDataTable.Columns.Add("Monster", typeof(string));
            monsterListDataTable.Columns.Add("Destination", typeof(string));
            monsterListDataTable.Columns.Add("Status", typeof(bool));
            dataGridResults.ItemsSource = monsterListDataTable.AsDataView();
        }

        private void buttonInputPath_Click(object sender, RoutedEventArgs e)
        {
            var folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            folderBrowserDialog.Description = "Select the source monsters folder.";
            System.Windows.Forms.DialogResult result = folderBrowserDialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                textBoxInputPath.Text = folderBrowserDialog.SelectedPath;
            }
            ValidateControls();
        }

        private void buttonOutputPath_Click(object sender, RoutedEventArgs e)
        {
            var folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            folderBrowserDialog.Description = "Select the output folder.";
            System.Windows.Forms.DialogResult result = folderBrowserDialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                textBoxOutputPath.Text = folderBrowserDialog.SelectedPath;
            }
            ValidateControls();
        }

        private async void buttonConvert_Click(object sender, RoutedEventArgs e)
        {
            buttonConvert.IsEnabled = false;
            Cursor = Cursors.Wait;
            textBlockScanStatus.Text = "scanning...";
            progressBarScan.Visibility = Visibility.Visible;
            taskBarProgressScan.ProgressState = System.Windows.Shell.TaskbarItemProgressState.Indeterminate;

            monsterListDataTable.Rows.Clear();

            string inputDir = textBoxInputPath.Text;
            string outputDir = textBoxOutputPath.Text;
            MonsterFormat inputFormat = GetMonsterFormatFromCombo(comboInputFormat);
            MonsterFormat outputFormat = GetMonsterFormatFromCombo(comboOutputFormat);
            ScanError result = ScanError.Success;
            await Task.Run(() =>
            {
                result = fileProcessor.ConvertMonsterFiles(inputDir, inputFormat, outputDir, outputFormat, true);
            });
            switch (result)
            {
                case ScanError.Success:
                    textBlockScanStatus.Text = "Completed successfully.";
                    break;
                case ScanError.NoMonstersFound:
                    textBlockScanStatus.Text = "Couldn't find any monster files.";
                    break;
                case ScanError.InvalidMonsterDirectory:
                    textBlockScanStatus.Text = "The selected project directory is invald.";
                    break;
                case ScanError.InvalidMonsterFormat:
                    textBlockScanStatus.Text = "The selected input or output monster format is invalid.";
                    break;
                case ScanError.CouldNotCreateDirectory:
                    textBlockScanStatus.Text = "Couldn't create destination directory.";
                    break;
                case ScanError.DirectoriesMatch:
                    textBlockScanStatus.Text = "Input and output directories can't be the same.";
                    break;
                default:
                    break;
            }

            taskBarProgressScan.ProgressState = System.Windows.Shell.TaskbarItemProgressState.None;
            progressBarScan.Visibility = Visibility.Hidden;
            Cursor = Cursors.Arrow;
            buttonConvert.IsEnabled = true;
        }

        private MonsterFormat GetMonsterFormatFromCombo(ComboBox comboBox)
        {
            string tag = (string)((Control)(comboBox.SelectedItem)).Tag;
            return (MonsterFormat)int.Parse(tag);
        }

        private void comboInputFormat_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ValidateControls();
        }

        private void comboOutputFormat_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ValidateControls();
        }

        private void buttonAbout_Click(object sender, RoutedEventArgs e)
        {
            var about = new AboutWindow();
            about.ShowDialog();
        }
    }
}
