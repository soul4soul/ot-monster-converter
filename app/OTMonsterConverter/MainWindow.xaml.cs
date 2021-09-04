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
            bool result = false;

            // Control file selection controls properly
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

            // Determine that all finds are set correctly before the convert button is enabled
            if ((comboInputFormat.SelectedItem != null) && (((IMonsterConverter)comboInputFormat.SelectedItem).FileSource == FileSource.Web))
            {
                result = ((textBoxOutputPath.Text != "") &&
                          (comboInputFormat.SelectedItem != null) &&
                          (comboOutputFormat.SelectedItem != null));
            }
            else
            {
                result = ((textBoxInputPath.Text != "") &&
                          (textBoxOutputPath.Text != "") &&
                          (comboInputFormat.SelectedItem != null) &&
                          (comboOutputFormat.SelectedItem != null));
            }
            if (buttonConvert != null)
            {
                buttonConvert.IsEnabled = result;
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

            string inputDir = textBoxInputPath.Text;
            string outputDir = textBoxOutputPath.Text;
            IMonsterConverter inputFormat = (IMonsterConverter)comboInputFormat.SelectedItem;
            IMonsterConverter outputFormat = (IMonsterConverter)comboOutputFormat.SelectedItem;
            ProcessorScanError result = ProcessorScanError.Success;
            await Task.Run(() =>
            {
                result = fileProcessor.ConvertMonsterFiles(inputDir, inputFormat, outputDir, outputFormat, mirroredFolderStructure: true);
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
    }
}
