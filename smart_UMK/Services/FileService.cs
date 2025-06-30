using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using smart_UMK.ViewModels;

namespace smart_UMK.Services
{
    internal class FileService
    {
        /// <summary>
        /// Открывает документ в стороннем приложении (например, Word).
        /// </summary>
        /// <param name="filePath">Путь к файлу.</param>
        public void OpenDocumentInExternalApp(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("Файл не найден.", filePath);

            Process.Start(new ProcessStartInfo
            {
                FileName = filePath,
                UseShellExecute = true // Открывает файл с помощью приложения по умолчанию
            });
        }

        /// <summary>
        /// Заменяет указанный текст во всех предоставленных файлах.
        /// </summary>
        /// <param name="files">Коллекция файлов для обработки.</param>
        /// <param name="oldText">Текст, который нужно найти.</param>
        /// <param name="newText">Текст, который нужно вставить.</param>
        public void ReplaceTextInDocuments(ObservableCollection<FileViewModel> files, string oldText, string newText)
        {
            foreach (var file in files)
            {
                ReplaceTextInDocumentAndCreateCopy(file.FilePath, oldText, newText);
            }
        }

        /// <summary>
        /// Создаёт копию документа, в которой заменены все вхождения старого текста на новый.
        /// </summary>
        /// <param name="filePath">Путь к оригинальному файлу.</param>
        /// <param name="oldText">Текст, который нужно найти.</param>
        /// <param name="newText">Текст для замены.</param>
        /// <returns>Путь к новому файлу.</returns>
        public string ReplaceTextInDocumentAndCreateCopy(string filePath, string oldText, string newText)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("Файл не найден.", filePath);

            // Получаем имя и расширение файла
            string fileName = Path.GetFileName(filePath);
            string directory = Path.GetDirectoryName(filePath);

            // Создаём новый путь для копии
            string newFilePath = Path.Combine(directory, $"Modified_{fileName}");

            // Копируем оригинальный файл
            File.Copy(filePath, newFilePath, true);

            // Открываем копию для внесения изменений
            using (var wordDoc = WordprocessingDocument.Open(newFilePath, true))
            {
                var body = wordDoc.MainDocumentPart.Document.Body;

                if (body != null)
                {
                    // Ищем и заменяем старый текст на новый
                    foreach (var text in body.Descendants<Text>())
                    {
                        if (text.Text.Contains(oldText))
                        {
                            text.Text = text.Text.Replace(oldText, newText);
                        }
                    }

                    // Сохраняем изменения в новой копии
                    wordDoc.MainDocumentPart.Document.Save();
                }
            }

            return newFilePath;
        }
    }
}
