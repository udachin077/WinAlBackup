using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WinAlBackup.Models;
using WinAlBackup.ViewModels;

namespace WinAlBackup
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F5)
            {
                ((MainViewModel)DataContext).ReloadBackupFiles();
            }
        }

        private void DataGridRow_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var row = (DataGridRow)sender;
            var file = ((BackupFile)row.DataContext).FullName;
            Process.Start("explorer.exe", $"\"{file}\"");
        }
    }
}
