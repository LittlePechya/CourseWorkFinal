using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SharpCompress.Archives;
using SharpCompress.Archives.Zip;
using SharpCompress.Common;

namespace CourseWorkFinal
{
    /// <summary>
    /// Класс для открытия файла проекта
    /// </summary>
    internal class FileManager
    {
        public string pathToDataBaseTable { get; private set; }
        public string pathToObjectPicture { get; set; }
        public string pathToTextFile { get; private set; }
        public string pathToFolder { get; private set; }

        string dbSearchTemplate = "*.sqlite";
        string pngSearchTemplate = "*.png";
        string txtSearchTemplate = "*.txt";

        public void GetFilesPath(string pathToFolder)
        {
            this.pathToFolder = pathToFolder;
            
            if (this.pathToFolder.Equals(""))
            {
                //MessageBox.Show("Вы не указали путь к файлу", "Ошибка");
                throw new Exception("Вы не указали путь к файлу");
            }
            pathToDataBaseTable = "";
            pathToObjectPicture = "";
            pathToTextFile = "";


            int dbFilesCount = CountFilesOfSomeType(dbSearchTemplate);
            int pngFilesCount = CountFilesOfSomeType(pngSearchTemplate);
            int textFilesCount = CountFilesOfSomeType(txtSearchTemplate);

            // Если в папке находится только один файл определенного типа, то он выбирается автоматически
            // Иначе пользователь должен выбрать нужный файл

            OpenFileDialog chooseFile = new OpenFileDialog();
            chooseFile.InitialDirectory = pathToFolder;
            chooseFile.Multiselect = false;

            TryOpenFile(chooseFile, dbFilesCount, "Выберите таблицу координат Z", dbSearchTemplate);
            TryOpenFile(chooseFile, pngFilesCount, "Выберите изображените объекта", pngSearchTemplate);
            TryOpenFile(chooseFile, textFilesCount, "Выберите текстовый файл с данными", txtSearchTemplate);

        }

        public int CountFilesOfSomeType(string type)
        {
            int count = Directory.GetFiles(pathToFolder, type).Length;
            return count;
        }

        public void TryOpenFile(OpenFileDialog chooseFile, int fileCount, string title, string filter)
        {
            if (fileCount > 1 || fileCount == 0)
            {
                SetChooseFileTitleAndFilter(title, filter, chooseFile);
                if (chooseFile.ShowDialog() == DialogResult.OK)
                {
                    switch (filter)
                    {
                        case "*.sqlite":
                            pathToDataBaseTable = Path.GetFullPath(chooseFile.FileName);
                            break;
                        case "*.png":
                            pathToObjectPicture = Path.GetFullPath(chooseFile.FileName);
                            break;
                        case "*.txt":
                            pathToTextFile = Path.GetFullPath(chooseFile.FileName);
                            break;
                    }
                }
            }
            else
            {
                switch (filter)
                {
                    case "*.sqlite":
                        pathToDataBaseTable = Path.GetFullPath(Directory.GetFiles(pathToFolder, dbSearchTemplate)[0]);
                        break;
                    case "*.png":
                        pathToObjectPicture = Path.GetFullPath(Directory.GetFiles(pathToFolder, pngSearchTemplate)[0]);
                        break;
                    case "*.txt":
                        pathToTextFile = Path.GetFullPath(Directory.GetFiles(pathToFolder, txtSearchTemplate)[0]);
                        break;
                }
            }

        }
        public void SetChooseFileTitleAndFilter(string title, string filter, OpenFileDialog chooseFile)
        {
            chooseFile.Title = title;
            chooseFile.Filter = filter;
        }

        public string[] GetDataFromTextFile()
        {
            string[] result = new string[4];
            // В список заносятся все строки из текстового файла
            List<string> lines = File.ReadAllLines(pathToTextFile, Encoding.Unicode).ToList();

            foreach (string line in lines)
            {
                //Если начинается с точности измерения, записываем полученную точность в результат
                if (line.StartsWith("Точность измерений"))
                {
                    List<string> value = line.Split(' ').ToList();
                    result[0] = (value[2].Split('м')[0]);

                }
                if (line.StartsWith("Количество структурных блоков"))
                {
                    List<string> value = line.Split(' ').ToList();
                    result[1] = (value[3]);

                }
                if (line.StartsWith("Количество геодезических марок, закрепленных в теле объекта"))
                {
                    List<String> value = line.Split(' ').ToList();
                    result[2] = (value[7]);

                }
            }

            return result;
        }
    }
}

