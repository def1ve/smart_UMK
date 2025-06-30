using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Win32;
using smart_UMK.ViewModels;
using smart_UMK.Views.Pages;
using smart_UMK.Views.Windows;
using Microsoft.Office.Interop.Word;

namespace smart_UMK.Views
{
    public partial class MainWindow : System.Windows.Window
    {
        public string SavedFolderPath { get; set; }

        private ObservableCollection<FileViewModel> filesCollection = new ObservableCollection<FileViewModel>();

        public MainWindow()
        {
            InitializeComponent();
        }

        private Dictionary<string, List<string>> ExtractFormattedFields(List<string> filePaths)
        {
            Dictionary<string, List<string>> formattedFields = new Dictionary<string, List<string>>();
            Microsoft.Office.Interop.Word.Application wordApp = new Microsoft.Office.Interop.Word.Application();

            try
            {
                foreach (var filePath in filePaths)
                {
                    Microsoft.Office.Interop.Word.Document doc = wordApp.Documents.Open(filePath);
                    List<string> fields = new List<string>();

                    foreach (Microsoft.Office.Interop.Word.Paragraph paragraph in doc.Paragraphs)
                    {
                        fields.Add(paragraph.Range.Text.Trim());
                    }

                    formattedFields[filePath] = fields;
                    doc.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                wordApp.Quit();
            }

            return formattedFields;
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DragMove();
            }
        }

        private void Button_Click_Documents(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Multiselect = true,
                Filter = "Документы Word (*.docx;*.dotx)|*.docx;*.dotx"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                foreach (string filepath in openFileDialog.FileNames)
                {
                    var file = new FileViewModel
                    {
                        FileName = System.IO.Path.GetFileName(filepath),
                        FilePath = filepath
                    };

                    filesCollection.Add(file);
                }
            }

            PagesNavigation.Navigate(new FilesViewPage(filesCollection));
        }

        private void Button_Click_ChangeFiles(object sender, RoutedEventArgs e)
        {
            if (filesCollection == null || filesCollection.Count == 0)
            {
                MessageBox.Show("Нет загруженных файлов!", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            ChangeFilesWindow changeFilesWindow = new ChangeFilesWindow(filesCollection);
            changeFilesWindow.Show();
        }


        private void Button_Click_ChangedDocuments(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(SavedFolderPath) && Directory.Exists(SavedFolderPath))
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo()
                {
                    FileName = SavedFolderPath,
                    UseShellExecute = true
                });
            }
            else
            {
                MessageBox.Show("Папка с итоговыми документами не найдена!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }



        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Вы уверены, что хотите выйти?", "Выход", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                System.Windows.Application.Current.Shutdown();
            }
        }
    }
    
}
