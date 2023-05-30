using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CourseWorkFinal.Decomposition;

namespace CourseWorkFinal
{
    public partial class MainForm : Form
    {

        // TODO: если класс decomposition только 1 (все описано в нем) -> создаешь 1 экземпляр класса в Program.cs и передаешь
        // в конструктор MainForm`ы
        // public MainForm(Decomposition decomposition) {...}

        // file: Program.cs, Decomposition decomposition = new Decomposition();
        // Application.Run(new MainForm(decomposition));

        int epochCount;
        FileManager fileManager;
        string[] objectData;
        DataTable dt = new DataTable();
        Database db;
        List<DataGridView> dataGridViewList = new List<DataGridView>();
        double defaultAlpha = 0.9;
        double measurmentError = 0;

        /// <summary>
        /// Конструктор основной формы проекта MainForm
        /// Здесь иницализируются все компоненты
        /// </summary>
        public MainForm()
        {
            InitializeComponent();
            SetStatusToFormComponents(false); // Отключает элементы формы, пока пользователь не начнет работу с проектом
            // tabPage1

            // tabControl заполняет весь экран
            tabControl.Dock = DockStyle.Fill;

            // Заполнение списка таблиц с координатами, которые находятся на форме
            dataGridViewList.Add(dataGridViewZCoordinate);
            //dataGridViewList.Add(dataGridViewFirstLevelCoordinates);
            //dataGridViewList.Add(dataGridViewSecondLevelCoordinates);

            // Настройка длины элементов для корректного отображения
            int dataSplitterWidth = 300;
            int decompositionSplitterWidth = 380;
            int decomposition4lvlSplitterWidth = 150;
            splitContainerDataPage.Dock = DockStyle.Fill;
            splitContainerDataPage.SplitterDistance = dataSplitterWidth;
            splitContainerFirstLevel.SplitterDistance = decompositionSplitterWidth;
            splitContainerSecondLevel.SplitterDistance = decompositionSplitterWidth;
            splitContainerFourthLevel.SplitterDistance = decomposition4lvlSplitterWidth;

            //
            showSaveStatus(false);
        }

        /// <summary>
        /// Метод отображает статус сохранения
        /// </summary>
        /// <param name="saveStatus"> true, если сохранено, false, если не сохранено </param>
        private void showSaveStatus(bool saveStatus)
        {
            if (saveStatus) { toolStripLabelSaveStatus.ForeColor = System.Drawing.Color.LimeGreen; }
            else { toolStripLabelSaveStatus.ForeColor = System.Drawing.Color.DarkRed; }
        }
        private void SetStatusToFormComponents(bool status)
        {
            buttonAddLoop.Enabled = status;
            buttonAddLoop.Enabled = status;
            buttonDeleteSelectedLoops.Enabled = status;
            numericUpDownMeasurementError.Enabled = status;
            numericUpDownSmoothingFactor.Enabled = status;
        }

        private void OpenProjectButton_Click(object sender, EventArgs e)
        {
            //
            // Работа со вкладкой "Данные"
            //
            string PathToFile = "";

            fileManager = new FileManager();
            FolderBrowserDialog selectedFolder = new FolderBrowserDialog();

            // Указываем путь к папке с проектом
            if (selectedFolder.ShowDialog() == DialogResult.OK)
            {
                PathToFile = selectedFolder.SelectedPath;
            }
            try
            {
                if (PathToFile != null || !PathToFile.Equals(""))
                {
                    fileManager.GetFilesPath(PathToFile);
                }

                if (fileManager.pathToObjectPicture != null || !fileManager.pathToObjectPicture.Equals(""))
                {
                    pictureBoxObject.Load(fileManager.pathToObjectPicture);
                    pictureBoxObject.SizeMode = PictureBoxSizeMode.StretchImage;
                }

                objectData = new string[4];
                if (fileManager.pathToTextFile != null || !fileManager.pathToTextFile.Equals(""))
                {
                    objectData = fileManager.GetDataFromTextFile();
                }

                objectData[3] = defaultAlpha.ToString();

                placeTextDataToFormElements(objectData);
                openDataBaseTable();
                SetStatusToFormComponents(true);



                StartDecomposition();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка");
                MessageBox.Show(ex.StackTrace, "Путь к ошибке");

            }
        }

        private void StartDecomposition()
        {
            FirstLevel();
            //Second...
        }

        // Первый уровень декомпозиции
        public void FirstLevel()
        {
            FirstLevelDecomposition decomposition = new FirstLevelDecomposition(defaultAlpha, measurmentError, 
                dataGridViewZCoordinate, dataGridViewFirstLevelPhaseCoordinates, dataGridViewFirstLevelObjectStatus, 
                chartFirstLevelExponentialSmooth, chartFirstLevelResponseFunction, dt);
        }

        // Данные

        /// <summary>
        /// Полученные из текстового файла значения располагаются на соответствующих элементах формы
        /// </summary>
        /// <param name="objectData"></param>
        public void placeTextDataToFormElements(string[] objectData)
        {
            // 0 точность измерений 1 количество блоков 2 количество точек 3 погрешность 
            measurmentError = double.Parse(objectData[0]);
            numericUpDownMeasurementError.Value = decimal.Parse(objectData[0]);
            labelBlockCount.Text = "Количество блоков на объекте: " + objectData[1].ToString();
            labelPointCount.Text = "Количество марок на объекте: " + objectData[2].ToString();
            defaultAlpha = double.Parse(objectData[3]);
            numericUpDownSmoothingFactor.Value = decimal.Parse(objectData[3]);
        }

        private void openDataBaseTable()
        {
            //Если пользователь не указал путь к БД, то происходит выход из метода
            if (fileManager.pathToDataBaseTable == null || fileManager.pathToDataBaseTable.Equals(""))
            {
                return;
            }

            clearDataGridViewZCoordinate();
            db = new Database(fileManager.pathToDataBaseTable);
            // Установка соединения с БД
            db.GetDataBaseConnection();
            // Заполнение таблиц
            FillAllTables(dataGridViewList, dt, db);
            // Подсчет нынешнего количества эпох
            epochCount = Convert.ToInt32(dataGridViewZCoordinate.RowCount)-1;
            setConnectionStatus(true);

        }

        /// <summary>
        /// Заполнение всех dataGridView, хранящих фазовые координаты
        /// </summary>
        /// <param name="dataGridViewList"></param>
        /// <param name="dt"></param>
        /// <param name="db"></param>
        private void FillAllTables(List<DataGridView> dataGridViewList, DataTable dt, Database db)
        {
            foreach (DataGridView dgw in dataGridViewList)
            {
                db.FillTable(dt, dgw);
            }
        }
        private void clearDataGridViewZCoordinate()
        {
            dataGridViewZCoordinate.Rows.Clear();
            dataGridViewZCoordinate.Columns.Clear();
        }

        private void setConnectionStatus(bool connectionStatus)
        {
            if (connectionStatus)
            {
                textBoxConnection.Text = "Связь с базой данных установлена";
                textBoxConnection.BackColor = Color.LawnGreen;
            }
        }

        private void ToolStripMenuItemOpen_Click(object sender, EventArgs e)
        {
            OpenProjectButton_Click(sender, e);
        }

        private void buttonAddLoop_Click(object sender, EventArgs e)
        {
            int newRowIndex = checkNewRowIndex();
            // Добавление новой строки и запись её номера в первую колонку
            dataGridViewZCoordinate.Rows[newRowIndex].Cells[0].Value = findMaxEpochInTable();
            // Добавление строки в базу данных
            // Расчет значений новой строки
            db.CalculateNewRowValues(dataGridViewZCoordinate, db, newRowIndex);
            //db.AddNewRowQuery(epochCount + 1);


            db.AddNewRowQuery(newRowIndex);
            // Глобальный счетчик строк увеличивается на 1
            epochCount++;
            // Необходимо снова открыть таблицу, чтобы увидеть изменения
            openDataBaseTable();

        }

        private int checkNewRowIndex()
        {
            return dataGridViewZCoordinate.RowCount - 1;
        }

        private int findMaxEpochInTable()
        {
            int max = Convert.ToInt32(dataGridViewZCoordinate.Rows[0].Cells[0].Value);
            foreach (DataGridViewRow row in  dataGridViewZCoordinate.Rows) 
            {
                int number = Convert.ToInt32(row.Cells[0].Value);
                if (number > max)
                    max = number;
            }
            return max+1;
        }

        private void buttonDeleteSelectedLoops_Click(object sender, EventArgs e)
        {
            if(areTableValuesSelected())
            {
                for (int i = 0; i < dataGridViewZCoordinate.SelectedRows.Count; i++)
                {
                  db.DeleteRowQuery(dataGridViewZCoordinate.Rows[dataGridViewZCoordinate.SelectedRows[i].Index].Cells[0].Value.ToString());
                }
            }

            openDataBaseTable();
        }

        private bool areTableValuesSelected()
        {
            if (dataGridViewZCoordinate.SelectedRows.Count > 0)
                return true;
            else 
                return false;
        }

       
    }

}
