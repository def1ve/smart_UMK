using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using smart_UMK.ViewModels;

namespace smart_UMK.Views.Windows
{
    public partial class ChangeFilesWindow : Window
    {
        private readonly ObservableCollection<FileViewModel> _filesCollection;
        private int _directionCount = 1;
        private const int MaxDirections = 10;

        public ChangeFilesWindow(ObservableCollection<FileViewModel> files)
        {
            InitializeComponent();
            _filesCollection = files ?? throw new ArgumentNullException(nameof(files));
            AddDirectionControls();
            DisciplineTxtBox.Focus();
        }

        private void AddDirectionControls()
        {
            if (_directionCount > MaxDirections)
            {
                MessageBox.Show($"Максимальное количество направлений: {MaxDirections}",
                              "Ограничение",
                              MessageBoxButton.OK,
                              MessageBoxImage.Information);
                return;
            }

            var panel = new StackPanel { Margin = new Thickness(0, 10, 0, 0) };

            // Название направления
            var dirTextBlock = new TextBlock
            {
                Text = $"Название направления {_directionCount}:",
                FontWeight = FontWeights.Bold
            };
            panel.Children.Add(dirTextBlock);

            // ComboBox для направления
            var dirComboBox = new ComboBox
            {
                Name = $"DirComboBox{_directionCount}",
                Height = 25,
                Margin = new Thickness(0, 5, 0, 0)
            };
            InitializeSpecialitiesComboBox(dirComboBox);
            panel.Children.Add(dirComboBox);

            // Формат обучения
            var formatTextBlock = new TextBlock
            {
                Text = $"Формат обучения {_directionCount}:",
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 10, 0, 0)
            };
            panel.Children.Add(formatTextBlock);

            // ComboBox для формата
            var formatComboBox = new ComboBox
            {
                Name = $"FormatComboBox{_directionCount}",
                Height = 25,
                Width = 150,
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(0, 5, 0, 10)
            };
            InitializeFormatComboBox(formatComboBox);
            panel.Children.Add(formatComboBox);

            ContentStackPanel.Children.Add(panel);
            _directionCount++;
        }

        private void InitializeSpecialitiesComboBox(ComboBox comboBox)
        {
            // Здесь должен быть ваш список направлений
            // Для примера добавлены 2 варианта
            comboBox.Items.Add(new ComboBoxItem { Content = "01.03.02 Прикладная математика" });
            comboBox.Items.Add(new ComboBoxItem { Content = "09.03.02 Информационные системы" });
        }

        private void InitializeFormatComboBox(ComboBox comboBox)
        {
            comboBox.Items.Add(new ComboBoxItem { Content = "Очное" });
            comboBox.Items.Add(new ComboBoxItem { Content = "Заочное" });
            comboBox.Items.Add(new ComboBoxItem { Content = "Очно-заочное" });
        }

        private void AddDirectionButton_Click(object sender, RoutedEventArgs e)
        {
            AddDirectionControls();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(DisciplineTxtBox.Text))
            {
                MessageBox.Show("Введите название дисциплины", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Здесь логика сохранения
            MessageBox.Show("Данные сохранены", "Готово", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}