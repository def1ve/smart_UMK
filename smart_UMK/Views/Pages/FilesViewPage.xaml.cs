using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using DocumentFormat.OpenXml.Packaging;
using smart_UMK.Services;
using smart_UMK.ViewModels;
using smart_UMK.Views.Windows;
using Microsoft.Win32;

namespace smart_UMK.Views.Pages
{
    /// <summary>
    /// Логика взаимодействия для FilesViewPage.xaml
    /// </summary>
    public partial class FilesViewPage : Page
    {
        public ObservableCollection<FileViewModel> WordsFiles { get; set; }

        public FilesViewPage(ObservableCollection<FileViewModel> filesCollection)
        {
            InitializeComponent();
            WordsFiles = filesCollection;
            FilesListBox.ItemsSource = WordsFiles;
            filesCollection.CollectionChanged += (s, e) =>
            {
                // Обновляем видимость кнопки при изменении коллекции
                ClearAllButton.Visibility = filesCollection.Count > 0
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            };

        }

        private async void FilesListBox_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            FileService fileService = new FileService();

            if (FilesListBox.SelectedItem is FileViewModel selectedFile)
            {
                // Открываем документ в стороннем приложении (например, Word)
                fileService.OpenDocumentInExternalApp(selectedFile.FilePath);

                // Открываем окно для поиска и замены текста
                SearchReplaceWindow searchReplaceWindow = new SearchReplaceWindow(WordsFiles);
                searchReplaceWindow.ShowDialog();
            }

            //if (FilesListBox.SelectedItem is FileViewModel selectedFile)
            //{

            //    //try
            //    //{

            //    //    using (WordprocessingDocument doc = WordprocessingDocument.Open(selectedFile.FilePath, true))
            //    //    {

            //    //    }
            //    //}
            //    //catch (Exception ex)
            //    //{
            //    //    MessageBox.Show($"Ошибка при обработке файла: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            //    //}
            //}
        }
        private void ClearAllButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
               "Вы уверены, что хотите удалить все прикрепленные файлы?",
               "Подтверждение удаления",
               MessageBoxButton.YesNo,
               MessageBoxImage.Warning);

            // Используем ItemsSource вместо Items
            if (FilesListBox.ItemsSource is System.Collections.IList collection)
            {
                collection.Clear();
            }
        }

        private void DeleteFileButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag != null)
            {
                var itemToRemove = button.Tag;

                // Используем ItemsSource вместо Items
                if (FilesListBox.ItemsSource is System.Collections.IList collection)
                {
                    collection.Remove(itemToRemove);
                }
            }
        }

    }
}
