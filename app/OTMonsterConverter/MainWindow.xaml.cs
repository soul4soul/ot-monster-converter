using MonsterConverterInterface;
using MonsterConverterProcessor;
using System.Data;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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
                    monsterListDataTable.Rows.Add(new object[] { e.Source, e.Destination });
                    dataGridResults.ScrollIntoView(dataGridResults.Items[dataGridResults.Items.Count - 1]);
                });
            }
        }

        private void ValidateControls()
        {
            // Set enable state of input controls
            if (comboInputFormat.SelectedItem == null)
            {
                textBoxInputPath.IsEnabled = true;
                buttonInputPath.IsEnabled = true;
            }
            else
            {
                textBoxInputPath.IsEnabled = (((IMonsterConverter)comboInputFormat.SelectedItem).FileSource == FileSource.LocalFiles);
                buttonInputPath.IsEnabled = (((IMonsterConverter)comboInputFormat.SelectedItem).FileSource == FileSource.LocalFiles);
            }

            // Determine that all fields are set correctly before the convert button is enabled
            bool isInputOutputConfigured;
            if ((comboInputFormat.SelectedItem != null) && (((IMonsterConverter)comboInputFormat.SelectedItem).FileSource == FileSource.Web))
            {
                isInputOutputConfigured = ((textBoxOutputPath.Text != "") &&
                          (comboInputFormat.SelectedItem != null) &&
                          (comboOutputFormat.SelectedItem != null));
            }
            else
            {
                isInputOutputConfigured = ((textBoxInputPath.Text != "") &&
                          (textBoxOutputPath.Text != "") &&
                          (comboInputFormat.SelectedItem != null) &&
                          (comboOutputFormat.SelectedItem != null));
            }

            // Set enable state of otbm fields
            if ((ItemConversionMethod)comboItemConversion.SelectedItem == ItemConversionMethod.KeepSouceIds)
            {
                buttonOtbmFilePath.IsEnabled = false;
                textBoxOtbmFilePath.IsEnabled = false;
            }
            else
            {
                buttonOtbmFilePath.IsEnabled = true;
                textBoxOtbmFilePath.IsEnabled = true;
            }

            bool isOtbmConfigured = (((ItemConversionMethod)comboItemConversion.SelectedItem == ItemConversionMethod.KeepSouceIds) ||
                            (((ItemConversionMethod)comboItemConversion.SelectedItem != ItemConversionMethod.KeepSouceIds) && (textBoxOtbmFilePath.Text != "")));

            if (buttonConvert != null)
            {
                buttonConvert.IsEnabled = isInputOutputConfigured && isOtbmConfigured;
            }
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            PluginHelper plugins = await PluginHelper.Instance;
            foreach (var p in plugins.Converters)
            {
                if (p.IsReadSupported)
                    comboInputFormat.Items.Add(p);
                if (p.IsWriteSupported)
                    comboOutputFormat.Items.Add(p);
            }

            comboItemConversion.Items.Add(ItemConversionMethod.KeepSouceIds);
            comboItemConversion.Items.Add(ItemConversionMethod.UseServerIds);
            comboItemConversion.Items.Add(ItemConversionMethod.UseClientIds);
            comboItemConversion.SelectedItem = ItemConversionMethod.KeepSouceIds;

            ValidateControls();
            monsterListDataTable = new DataTable("MonsterList");
            monsterListDataTable.Columns.Add("Source", typeof(ConvertResultEventArgs));
            monsterListDataTable.Columns.Add("Destination", typeof(ConvertResultEventArgs));
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

            IMonsterConverter inputFormat = (IMonsterConverter)comboInputFormat.SelectedItem;
            string inputDir = textBoxInputPath.Text;
            IMonsterConverter outputFormat = (IMonsterConverter)comboOutputFormat.SelectedItem;
            string outputDir = textBoxOutputPath.Text;
            ItemConversionMethod conversionMethod = (ItemConversionMethod)comboItemConversion.SelectedItem;
            string otbPath = textBoxOtbmFilePath.Text;
            ProcessorScanError result = ProcessorScanError.Success;
            await Task.Run(() =>
            {
                result = fileProcessor.ConvertMonsterFiles(inputDir, inputFormat, outputDir, outputFormat, otbPath, conversionMethod, mirroredFolderStructure: true);
            });
            switch (result)
            {
                case ProcessorScanError.Success:
                    textBlockScanStatus.Text = "Completed successfully.";
                    break;
                case ProcessorScanError.NoMonstersFound:
                    textBlockScanStatus.Text = "Couldn't find any monster files.";
                    break;
                case ProcessorScanError.InvalidInputDirectory:
                    textBlockScanStatus.Text = "The selected input directory is invald.";
                    break;
                case ProcessorScanError.CouldNotCreateDirectory:
                    textBlockScanStatus.Text = "Couldn't create output directory.";
                    break;
                case ProcessorScanError.DirectoriesMatch:
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

        private void buttonOtbmFilePath_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog fileDialog = new Microsoft.Win32.OpenFileDialog();
            fileDialog.Filter = "Open Tibia Binary (.otb)|*.otb";
            fileDialog.DefaultExt = ".otb";
            fileDialog.Title = "Select an otb file";
            if (fileDialog.ShowDialog() == true)
            {
                textBoxOtbmFilePath.Text = fileDialog.FileName;
            }
            ValidateControls();
        }

        private void comboItemConversion_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ValidateControls();
        }
    }
}
