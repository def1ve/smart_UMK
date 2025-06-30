using System;
using System.Collections.ObjectModel;
using System.Windows;
using smart_UMK.Services;
using smart_UMK.ViewModels;

namespace smart_UMK.Views.Windows
{
    public partial class SearchReplaceWindow : Window
    {
        private FileService _fileService;
        private ObservableCollection<FileViewModel> _files;

        public SearchReplaceWindow(ObservableCollection<FileViewModel> files)
        {
            InitializeComponent();
            _fileService = new FileService();
            _files = files;
        }

        private void OnReplaceButtonClick(object sender, RoutedEventArgs e)
        {
            string oldText = SearchTextBox.Text;
            string newText = ReplaceTextBox.Text;

            if (string.IsNullOrEmpty(oldText) || string.IsNullOrEmpty(newText))
            {
                MessageBox.Show("Пожалуйста, укажите оба текста для поиска и замены.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Выполняем замену в документах
            _fileService.ReplaceTextInDocuments(_files, oldText, newText);
            MessageBox.Show("Замена завершена!");
            //this.Close(); // Закрываем окно после выполнения операции
        }
    }
}
