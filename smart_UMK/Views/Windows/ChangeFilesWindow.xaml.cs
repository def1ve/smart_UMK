using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using System.Linq;
using smart_UMK.ViewModels;

namespace smart_UMK.Views.Windows
{
    public partial class ChangeFilesWindow : Window
    {
        private readonly ObservableCollection<FileViewModel> _filesCollection;
        private int _directionCount = 1;
        private const int MaxDirections = 10;
        private const int MinDirections = 1;
        private readonly string _specialitiesFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Specialities.txt");
        private string[] _availableSpecialities;

        public ChangeFilesWindow(ObservableCollection<FileViewModel> files)
        {
            InitializeComponent();
            _filesCollection = files ?? throw new ArgumentNullException(nameof(files));

            // Создаем файл при необходимости и загружаем направления
            CreateSpecialitiesFileIfNotExists();
            LoadSpecialitiesFromFile();

            AddDirectionControls();
            DisciplineTxtBox.Focus();
        }

        private void CreateSpecialitiesFileIfNotExists()
        {
            if (!File.Exists(_specialitiesFilePath))
            {
                try
                {
                    // Создаем файл с одной строкой-инструкцией
                    File.WriteAllText(_specialitiesFilePath, "Добавьте направления");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Не удалось создать файл направлений: {ex.Message}",
                                  "Ошибка",
                                  MessageBoxButton.OK,
                                  MessageBoxImage.Error);
                }
            }
        }

        private void LoadSpecialitiesFromFile()
        {
            try
            {
                _availableSpecialities = File.ReadAllLines(_specialitiesFilePath)
                    .Where(line => !string.IsNullOrWhiteSpace(line))
                    .ToArray();

                // Если файл пустой или содержит только инструкцию
                if (_availableSpecialities.Length == 0 ||
                    (_availableSpecialities.Length == 1 && _availableSpecialities[0] == "Добавьте направления"))
                {
                    _availableSpecialities = new[] { "Добавьте направления в файл Specialities.txt" };
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки файла направлений: {ex.Message}",
                              "Ошибка",
                              MessageBoxButton.OK,
                              MessageBoxImage.Error);
                _availableSpecialities = new[] { "Ошибка загрузки направлений" };
            }
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
                Margin = new Thickness(0, 5, 0, 0),
                Tag = _directionCount
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
                Margin = new Thickness(0, 5, 0, 10),
                Tag = _directionCount
            };
            InitializeFormatComboBox(formatComboBox);
            panel.Children.Add(formatComboBox);

            ContentStackPanel.Children.Add(panel);
            _directionCount++;

            RemoveDirectionBtn.IsEnabled = _directionCount > MinDirections + 1;
        }

        private void InitializeSpecialitiesComboBox(ComboBox comboBox)
        {
            comboBox.Items.Clear();
            foreach (var speciality in _availableSpecialities)
            {
                comboBox.Items.Add(new ComboBoxItem { Content = speciality });
            }

            if (comboBox.Items.Count > 0)
                comboBox.SelectedIndex = 0;
        }

        private void InitializeFormatComboBox(ComboBox comboBox)
        {
            comboBox.Items.Clear();
            comboBox.Items.Add(new ComboBoxItem { Content = "Очное" });
            comboBox.Items.Add(new ComboBoxItem { Content = "Заочное" });
            comboBox.Items.Add(new ComboBoxItem { Content = "Очно-заочное" });
            comboBox.SelectedIndex = 0;
        }

        private void RemoveDirectionButton_Click(object sender, RoutedEventArgs e)
        {
            if (ContentStackPanel.Children.Count <= MinDirections + 1)
                return;

            ContentStackPanel.Children.RemoveAt(ContentStackPanel.Children.Count - 1);
            _directionCount--;

            RemoveDirectionBtn.IsEnabled = _directionCount > MinDirections + 1;
            UpdateDirectionsNumbers();
        }

        private void UpdateDirectionsNumbers()
        {
            int currentNumber = 1;
            foreach (var child in ContentStackPanel.Children)
            {
                if (child is StackPanel panel)
                {
                    if (panel.Children[0] is TextBlock dirTextBlock)
                        dirTextBlock.Text = $"Название направления {currentNumber}:";

                    if (panel.Children[2] is TextBlock formatTextBlock)
                        formatTextBlock.Text = $"Формат обучения {currentNumber}:";

                    currentNumber++;
                }
            }
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

            MessageBox.Show("Данные сохранены", "Готово", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}