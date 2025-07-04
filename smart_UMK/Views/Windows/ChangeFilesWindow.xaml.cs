using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using DocumentFormat.OpenXml.ExtendedProperties;
using Microsoft.Office.Interop.Word;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Linq;
using smart_UMK.ViewModels;
using WordApp = Microsoft.Office.Interop.Word.Application;
using System.Windows.Controls;

namespace smart_UMK.Views.Windows
{
    public partial class ChangeFilesWindow : System.Windows.Window
    {

        private ObservableCollection<FileViewModel> filesCollection;

        public ChangeFilesWindow(ObservableCollection<FileViewModel> files)
        {
            InitializeComponent();
            filesCollection = files;
        }

        // проверка открыт ли файл ворд
        private bool IsFileLocked(string filePath)
        {
            try
            {
                using (FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite))
                {
                    return false; // Файл доступен
                }
            }
            catch (IOException)
            {
                MessageBoxResult result = MessageBox.Show($"Файл открыт в другой программе: {filePath}\n\nХотите закрыть Word?",
                                                          "Ошибка",
                                                          MessageBoxButton.YesNo,
                                                          MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    CloseWordProcesses(); // Закрываем Word
                    return false; // Теперь файл доступен
                }

                return true; // Файл всё ещё заблокирован
            }
        }

        private void CloseWordProcesses()
        {
            foreach (var process in System.Diagnostics.Process.GetProcessesByName("WINWORD"))
            {
                process.Kill(); // Закрывает все процессы Word
            }
            MessageBox.Show("Word закрыт. Попробуйте снова!", "Готово", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Button_Save_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(DisciplineNameTextBox.Text))
            {
                MessageBox.Show("Введите название дисциплины!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (filesCollection == null || filesCollection.Count == 0)
            {
                MessageBox.Show("Нет загруженных файлов!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var dialog = new CommonOpenFileDialog { IsFolderPicker = true };
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                string saveFolderPath = dialog.FileName;
                string newDisciplineName = DisciplineNameTextBox.Text;

                // Получаем реальный список загруженных файлов
                List<string> filePaths = filesCollection.Select(f => f.FilePath).ToList();

                foreach (string filePath in filePaths)
                {
                    ReplaceContentControlText(filePath, newDisciplineName, saveFolderPath);
                }
                MessageBox.Show("Файлы успешно сохранены!", "Готово", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void ReplaceContentControlText(string filePath, string newDisciplineName, string saveFolderPath)
        {
            if (IsFileLocked(filePath))
            {
                return; // Если файл заблокирован, выходим
            }

            WordApp wordApp = new WordApp { Visible = false };
            Microsoft.Office.Interop.Word.Document doc = null;

            try
            {
                doc = wordApp.Documents.Open(filePath);

                // Словарь для хранения всех значений из ComboBox и TextBox
                var replacementValues = new Dictionary<string, string>
        {
            { "DisciplineName", newDisciplineName }
        };

                // Собираем данные из всех ComboBox для направлений (1-5)
                for (int i = 1; i <= 5; i++)
                {
                    var specialityComboBox = this.FindName($"Speciality{i}ComboBox") as ComboBox;
                    if (specialityComboBox != null && specialityComboBox.SelectedItem is ComboBoxItem specialityItem)
                    {
                        replacementValues[$"Direction{i}"] = specialityItem.Content.ToString();
                    }

                    var formatComboBox = this.FindName($"EducationFormat{i}ComboBox") as ComboBox;
                    if (formatComboBox != null && formatComboBox.SelectedItem is ComboBoxItem formatItem)
                    {
                        replacementValues[$"FormDirection{i}"] = formatItem.Content.ToString();
                    }
                }

                // Заменяем содержимое в документе
                foreach (Microsoft.Office.Interop.Word.ContentControl control in doc.ContentControls)
                {
                    if (!string.IsNullOrEmpty(control.Title) && replacementValues.TryGetValue(control.Title, out string value))
                    {
                        control.Range.Text = value;
                    }
                }

                // Формируем новое имя файла
                string newFileName = Path.GetFileNameWithoutExtension(filePath) + "_изменённый.docx";
                string savePath = Path.Combine(saveFolderPath, newFileName);

                // Сохраняем как .docx
                doc.SaveAs2(savePath, Microsoft.Office.Interop.Word.WdSaveFormat.wdFormatDocumentDefault);
                doc.Close();

                MainWindow mainWindow = System.Windows.Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
                if (mainWindow != null)
                {
                    mainWindow.SavedFolderPath = saveFolderPath; // Сохраняем путь в MainWindow
                }
            }
            finally
            {
                wordApp.Quit();
            }
        }



        private void Button_Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
