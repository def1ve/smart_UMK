using System;
using System.Collections.ObjectModel;
using System.Windows;
using smart_UMK.Services;
using smart_UMK.ViewModels;

namespace smart_UMK.Views.Windows
{
    public partial class SearchReplaceWindow : Window
    {
        private readonly FileService _fileService;
        private readonly ObservableCollection<FileViewModel> _files;

        public SearchReplaceWindow(ObservableCollection<FileViewModel> files)
        {
            InitializeComponent();
            _fileService = new FileService();
            _files = files ?? throw new ArgumentNullException(nameof(files));
            SearchTxtBox.Focus();
        }

        private void OnReplaceButtonClick(object sender, RoutedEventArgs e)
        {
            string oldText = SearchTxtBox.Text.Trim();
            string newText = ReplaceTxtBox.Text.Trim();

            if (string.IsNullOrEmpty(oldText))
            {
                MessageBox.Show("Введите текст для поиска", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _fileService.ReplaceTextInDocuments(_files, oldText, newText);
            MessageBox.Show("Замена текста выполнена", "Готово", MessageBoxButton.OK, MessageBoxImage.Information);
            this.Close();
        }

        private void OnCancelButtonClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
