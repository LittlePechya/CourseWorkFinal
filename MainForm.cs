using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using CourseWorkFinal.Decomposition;
using SharpCompress;
using ChartControl = System.Windows.Forms.DataVisualization.Charting.Chart;

namespace CourseWorkFinal
{
    public partial class MainForm : Form
    {
        private int _epochCount;
        private int _blockCount;
        private int _pointsCount;
        private FileManager _fileManager;
        private string[] _objectData;
        private DataTable _dt = new DataTable();
        private Database _db;
        private List<DataGridView> dataGridViewList = new List<DataGridView>();
        private double _smoothingFactor = 0.9;
        private double _measurmentError = 0;
        private List<List<string>> _points = new List<List<string>>();

        // Этот флаг снимается, после того как пользователь открывает проект, иначе вызывается метод valueChanged у numericUpDown
        private bool _flagFirstOpen = true;
        private FirstLevelDecomposition _decompositionFirst;
        private SecondLevelDecomposition _decompositionSecond;
        private FourthLevelDecomposition _decompositionFourth;

        // Имя таблицы, с которой мы работаем в БД
        private string _tableName;

        // Список всех чекбоксов на втором уровне
        private List <CheckBox> _checkBoxSecondLevelList = new List<CheckBox>();
        /// <summary>
        /// Конструктор основной формы проекта MainForm
        /// Здесь иницализируются все компоненты
        /// </summary>
        public MainForm()
        {
            InitializeComponent();
            // Добавляем чекбоксы со второго уровня в список
            _checkBoxSecondLevelList.Add(checkBoxSecondLevelMBottom);
            _checkBoxSecondLevelList.Add(checkBoxSecondLevelMOriginal);
            _checkBoxSecondLevelList.Add(checkBoxSecondLevelMTop);
            _checkBoxSecondLevelList.Add(checkBoxSecondLevelABottom);
            _checkBoxSecondLevelList.Add(checkBoxSecondLevelAOriginal);
            _checkBoxSecondLevelList.Add(checkBoxSecondLevelATop);
            _checkBoxSecondLevelList.Add(checkBoxSecondLevelResponseFunctionBottom);
            _checkBoxSecondLevelList.Add(checkBoxSecondLevelResponseFunctionOriginal);
            _checkBoxSecondLevelList.Add(checkBoxSecondLevelResponseFunctionTop);
            SetStatusToFormComponents(false); // Отключает элементы формы, пока пользователь не начнет работу с проектом
            // tabPage1

            // tabControl заполняет весь экран
            anotherTabControl.Dock = DockStyle.Fill;

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

            // tabPage9 с расчетами на втором уровне должен быть заблокирован, пока пользователь не распределит точки по блокам на втором уровне
            // tapPage5 с 4 уровнем декомпозиции также заблокирован, пока пользователь не распределит точки по блокам
            tabPage9.Enabled = false;
            tabPage5.Enabled = false;

            // Выставляем статус "Не сохранено"
            showSaveStatus(false);
        }

        private void showSaveStatus(bool saveStatus)
        {
            if (saveStatus) { toolStripLabelSaveStatus.ForeColor = System.Drawing.Color.LimeGreen; toolStripLabelSaveStatus.Text = "Статус: сохранено"; }
            else { toolStripLabelSaveStatus.ForeColor = System.Drawing.Color.DarkRed; toolStripLabelSaveStatus.Text = "Статус: не сохранено"; }
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

            _fileManager = new FileManager();
            FolderBrowserDialog selectedFolder = new FolderBrowserDialog();

            // Указываем путь к папке с проектом
            if (selectedFolder.ShowDialog() == DialogResult.OK)
            {
                PathToFile = selectedFolder.SelectedPath;
            }

            // Если пользователь закрыл окно, то выходим из метода, ничего не происходит
            else
            {
                return;
            }
           
            try
            {
                if (PathToFile != null || !PathToFile.Equals(""))
                {
                    _fileManager.GetFilesPath(PathToFile);
                }

                if (_fileManager.pathToObjectPicture != null || !_fileManager.pathToObjectPicture.Equals(""))
                {
                    pictureBoxObject.Load(_fileManager.pathToObjectPicture);
                    pictureBoxObject.SizeMode = PictureBoxSizeMode.StretchImage;
                    pictureBoxSecondLevelDecomposition.Load(_fileManager.pathToObjectPicture);
                    pictureBoxSecondLevelDecomposition.SizeMode = PictureBoxSizeMode.StretchImage;
                }

                _objectData = new string[4];
                if (_fileManager.pathToTextFile != null || !_fileManager.pathToTextFile.Equals(""))
                {
                    _objectData = _fileManager.GetDataFromTextFile();
                }

                _objectData[3] = _smoothingFactor.ToString();

                placeTextDataToFormElements(_objectData);
                
                // Здесь используется немного другой метод openDataBase, потому что нужно получить ссылку на dataTable
                _dt = openDataBaseTableDt();
                _db.ChangeCommasToDots(_dt, _tableName);
                _db.UpdateFillTable(_dt, dataGridViewZCoordinate, _tableName);
               
                SetStatusToFormComponents(true);
                showSaveStatus(true);


                StartDecomposition();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка");
                MessageBox.Show(ex.StackTrace, "Путь к ошибке");
            }
        }

        /// <summary>
        /// Метод производит необходимые для декомпозиции вычисления на 1, 2 и 4 уровнях
        /// </summary>
        private void StartDecomposition()
        {
            FirstLevel();
            SecondLevel();
            FourthLevel();
        }

        /// <summary>
        /// Первый уровень декомпозиции
        /// </summary>
        public void FirstLevel()
        {
            _decompositionFirst = new FirstLevelDecomposition(_smoothingFactor, _measurmentError,
                dataGridViewZCoordinate, dataGridViewFirstLevelPhaseCoordinates, dataGridViewFirstLevelObjectStatus,
                chartFirstLevelM, chartFirstLevelA, chartFirstLevelResponseFunction, _dt);
        }

        /// <summary>
        /// Второй уровень декомпозиции
        /// </summary>
        public void SecondLevel()
        {


            // Блокируем чекбоксы для отображения графиков, чтобы не словить эксепшены
            EnableSecondLevelCheckBoxes(false);

            string str = labelBlockCount.Text;
            _blockCount = Int32.Parse(new String(str.Where(Char.IsDigit).ToArray()));
            str = labelPointCount.Text;
            _pointsCount = Int32.Parse(new String(str.Where(Char.IsDigit).ToArray()));
            _decompositionSecond = new SecondLevelDecomposition(_smoothingFactor, _measurmentError, dataGridViewZCoordinate, _blockCount, _pointsCount, listBoxAllPointsOfTheObject, listBoxPointsOnTheBlock, labelPointsOfTheSelectedBlock,
                chartSecondLevelResponseFunction, chartSecondLevelM, chartSecondLevelA, comboBoxSecondLevelChooseBlock, dataGridViewSecondLevelObjectStatus, dataGridViewSecondLevelPhaseCoordinates);

            if (_decompositionSecond.GetPoints() != null)
            {
                _points = _decompositionSecond.GetPoints();
            }
        }

        /// <summary>
        /// Включает или выключает чекбоксы на втором уровне
        /// </summary>
        /// <param name="enabled"></param>
        private void EnableSecondLevelCheckBoxes(bool enabled)
        {
            checkBoxSecondLevelABottom.Enabled = enabled;
            checkBoxSecondLevelAOriginal.Enabled = enabled;
            checkBoxSecondLevelATop.Enabled = enabled;

            checkBoxSecondLevelMBottom.Enabled = enabled;
            checkBoxSecondLevelMOriginal.Enabled = enabled;
            checkBoxSecondLevelMTop.Enabled = enabled;

            checkBoxSecondLevelResponseFunctionBottom.Enabled = enabled;
            checkBoxSecondLevelResponseFunctionOriginal.Enabled = enabled;
            checkBoxSecondLevelResponseFunctionTop.Enabled = enabled;
        }

        public void FourthLevel()
        {
            _decompositionFourth = new FourthLevelDecomposition(_smoothingFactor, _measurmentError, dataGridViewZCoordinate, _blockCount, _points, comboBoxFourthLevelChooseBlock,
                checkedListBoxFourthLevelAvailablePoints, chartFourthLevel);
        }

        /// <summary>
        /// Полученные из текстового файла значения располагаются на соответствующих элементах формы
        /// </summary>
        /// <param name="objectData"></param>
        public void placeTextDataToFormElements(string[] objectData)
        {
            // 0 точность измерений 1 количество блоков 2 количество точек 3 погрешность 
            _measurmentError = double.Parse(objectData[0]);
            numericUpDownMeasurementError.Value = decimal.Parse(objectData[0]);
            labelBlockCount.Text = "Количество блоков на объекте: " + objectData[1].ToString();
            labelPointCount.Text = "Количество марок на объекте: " + objectData[2].ToString();
            _smoothingFactor = double.Parse(objectData[3]);
            numericUpDownSmoothingFactor.Value = decimal.Parse(objectData[3]);
        }

        private void openDataBaseTable()
        {
            //Если пользователь не указал путь к БД, то происходит выход из метода
            if (_fileManager.pathToDataBaseTable == null || _fileManager.pathToDataBaseTable.Equals(""))
            {
                return;
            }

            clearDataGridViewZCoordinate();
            _db = new Database(_fileManager.pathToDataBaseTable);
            // Установка соединения с БД
            _db.GetDataBaseConnection();
            // Заполнение таблиц
            FillAllTables(dataGridViewList, _dt, _db);
            setConnectionStatus(true);
        }

        private void UpdateDataBaseTable()
        {
            //Если пользователь не указал путь к БД, то происходит выход из метода
            if (_fileManager.pathToDataBaseTable == null || _fileManager.pathToDataBaseTable.Equals(""))
            {
                return;
            }

            clearDataGridViewZCoordinate();
            _db = new Database(_fileManager.pathToDataBaseTable);
            // Установка соединения с БД
            _db.GetDataBaseConnection();
            // Заполнение таблиц
            UpdateAllTables(dataGridViewList, _dt, _db);
            setConnectionStatus(true);
        }
        private DataTable openDataBaseTableDt()
        {
            //Если пользователь не указал путь к БД, то происходит выход из метода
            if (_fileManager.pathToDataBaseTable == null || _fileManager.pathToDataBaseTable.Equals(""))
            {
                return null;
            }

            clearDataGridViewZCoordinate();
            _db = new Database(_fileManager.pathToDataBaseTable);
            // Установка соединения с БД
            _db.GetDataBaseConnection();
            // Заполнение таблиц
            FillAllTables(dataGridViewList, _dt, _db);
            setConnectionStatus(true);

            return _dt;
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
                _tableName = db.FillTable(dt, dgw);
            }
        }

        private void UpdateAllTables(List<DataGridView> dataGridViewList, DataTable dt, Database db)
        {
            foreach (DataGridView dgw in dataGridViewList)
            {
                db.UpdateFillTable(dt, dgw, _tableName);
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
            ResetFormAfterSmoothingFactorChanged();
        }

        private void buttonAddLoop_Click(object sender, EventArgs e)
        {
            // Определяем индекс новой строки как количество строк в таблице - 1
            // RowsCount = 9 (потому что учитывается последняя пустая строчка), индекс последнего элемента 8
            // Этот индекс используется, для добавления строки в таблицу dataGridView
            // Тогда индекс нового элемента = 9, то есть ровс каунт
            int newRowIndex = dataGridViewZCoordinate.RowCount;
            // Добавляем новую строку в саму БД
            // В базе данных он должен быть записан под индексом 9
            //db.AddNewRowQuery(newRowIndex);
            // Определеяем имя максимальной эпохи, чтобы записать его в новой строчке
            int maxEpoch = findMaxEpochInTable();
            // Индекс в БД определяется сам, а нам нужно только указать имя для эпохи
            _db.AddNewRowQuery(maxEpoch + 1, _tableName);
            // Добавлением значения в новую строку с индексом 9
            _db.CalculateNewRowValues(dataGridViewZCoordinate, _db, newRowIndex, maxEpoch + 1, _tableName);
            // Это нам надо, чтобы числа в табличку выводились в правильном виде double
            _db.ChangeCommasToDots(_dt, _tableName);
            UpdateDataBaseTable();
            // Заново считаем декомпозицию
            ResetFormAfterSmoothingFactorChanged();
        }

        /// <summary>
        /// Метод проходится по всем ячейкам, содержащим эпоху, и возвращает значение максимальной эпохи
        /// </summary>
        /// <returns></returns>
        private int findMaxEpochInTable()
        {
            int max = Convert.ToInt32(dataGridViewZCoordinate.Rows[0].Cells[0].Value);
            foreach (DataGridViewRow row in  dataGridViewZCoordinate.Rows) 
            {
                int number = Convert.ToInt32(row.Cells[0].Value);
                if (number > max)
                    max = number;
            }
            return max;
        }

        private void buttonDeleteSelectedLoops_Click(object sender, EventArgs e)
        {
            // Здесь используется 3, так как помним про пустую строчку
            if (dataGridViewZCoordinate.Rows.Count > 3)
            {
                if(areTableValuesSelected())
                {
                    for (int i = 0; i < dataGridViewZCoordinate.SelectedRows.Count; i++)
                    {
                      _db.DeleteRowQuery(dataGridViewZCoordinate.Rows[dataGridViewZCoordinate.SelectedRows[i].Index].Cells[0].Value.ToString(), _tableName);
                    }

                    // Обновляем строки в БД после удаления
                    UpdateDataBaseTable();
                    // Заново считаем декомпозицию, так как изменилось количество строк
                    ResetFormAfterSmoothingFactorChanged();
                }
            }
            else
            {
                MessageBox.Show("Невозможно выполнить удаление, для работы таблица должна содержать хотя бы 2 строки");
            }
        }

        private bool areTableValuesSelected()
        {
            if (dataGridViewZCoordinate.SelectedRows.Count > 0)
                return true;
            else 
                return false;
        }

        private void checkBoxResponseFunctionBottom_CheckedChanged(object sender, EventArgs e)
        {
            _decompositionFirst.CheckBoxResponseFunctionChange(checkBoxFirstLevelResponseFunctionBottom, chartFirstLevelResponseFunction, "нижняя");
        }

        private void checkBoxResponseFunctionOriginal_CheckedChanged(object sender, EventArgs e)
        {
            _decompositionFirst.CheckBoxResponseFunctionChange(checkBoxFirstLevelResponseFunctionOriginal, chartFirstLevelResponseFunction, "исходное");
        }

        private void checkBoxResponseFunctionTop_CheckedChanged(object sender, EventArgs e)
        {
            _decompositionFirst.CheckBoxResponseFunctionChange(checkBoxFirstLevelResponseFunctionOriginal, chartFirstLevelResponseFunction, "верхняя");
        }

        private void checkBoxFirstLevelMBottom_CheckedChanged(object sender, EventArgs e)
        {
            _decompositionFirst.CheckBoxMChange(chartFirstLevelM, "нижняя");
        }

        private void numericUpDownSmoothingFactor_ValueChanged(object sender, EventArgs e)
        {
            _smoothingFactor = Convert.ToDouble(numericUpDownSmoothingFactor.Value);
            if (_smoothingFactor > 0 & _smoothingFactor < 1) 
            {
                ResetFormAfterSmoothingFactorChanged();
            }
            
        }

        /// <summary>
        /// Этот метод нужен для того, чтобы очистить необходимые компоненты и заново рассчитать декомпозицию после изменения пользователем сглаживания
        /// </summary>
        public void ResetFormAfterSmoothingFactorChanged()
        {
            // Важно! Сначала отключаем чекбоксы, потом используем метод ResetLevel
            RemoveCheckFromCheckBoxes(_checkBoxSecondLevelList);

            checkBoxFirstLevelABottom.Checked = false;
            checkBoxFirstLevelAOriginal.Checked = false;
            checkBoxFirstLevelATop.Checked = false;
            checkBoxSecondLevelABottom.Checked = false;
            checkBoxSecondLevelAOriginal.Checked = false;
            checkBoxSecondLevelATop.Checked = false;
            
            checkBoxFirstLevelMBottom.Checked = false;
            checkBoxSecondLevelMBottom.Checked = false;
            checkBoxFirstLevelMOriginal.Checked = false;
            checkBoxSecondLevelMOriginal.Checked = false;
            checkBoxFirstLevelMTop.Checked = false;
            checkBoxSecondLevelMTop.Checked = false;

            comboBoxSecondLevelChooseBlock.Items.Clear();
            dataGridViewSecondLevelPhaseCoordinates.Rows.Clear();
            dataGridViewSecondLevelObjectStatus.Rows.Clear();
            _decompositionFirst.ResetFirstLevel(chartFirstLevelM, chartFirstLevelM);
            _decompositionSecond.resetFlag = true;
          
            comboBoxFourthLevelChooseBlock.Items.Clear();

            _decompositionFourth.ResetFourthLevel(_points, chartFourthLevel, checkedListBoxFourthLevelAvailablePoints, comboBoxFourthLevelChooseBlock);
            _decompositionSecond.ResetSecondLevel();

            // Табпейджи тоже обязательно выключать в самом конце, иначе невозможно отключить элементы формы
            tabPage5.Enabled = false;
            tabPage9.Enabled = false;
            listBoxAllPointsOfTheObject.Enabled = true;
            listBoxPointsOnTheBlock.Enabled = true;
            StartDecomposition();
        }

        private void checkBoxFirstLevelMBase_CheckedChanged(object sender, EventArgs e)
        {
            _decompositionFirst.CheckBoxMChange(chartFirstLevelM, "исходное");
        }

        private void checkBoxFirstLevelMTop_CheckedChanged(object sender, EventArgs e)
        {
            _decompositionFirst.CheckBoxMChange(chartFirstLevelM, "верхняя");
        }

        private void checkBoxFirstLevelABottom_CheckedChanged(object sender, EventArgs e)
        {
            _decompositionFirst.CheckBoxAChange(chartFirstLevelA, "нижняя");
        }

        private void checkBoxFirstLevelAOriginal_CheckedChanged(object sender, EventArgs e)
        {
            _decompositionFirst.CheckBoxAChange(chartFirstLevelA, "исходное");
        }

        private void checkBoxFirstLevelATop_CheckedChanged(object sender, EventArgs e)
        {
            _decompositionFirst.CheckBoxAChange(chartFirstLevelA, "верхняя");
        }

        private void listBoxAllPointsOfTheObject_DoubleClick(object sender, EventArgs e)
        {
            _decompositionSecond.AllPointsListBox_DoubleClick();
           
            if (_decompositionSecond.pointsAreDistributed == true)
            {
                tabPage5.Enabled = true;
                tabPage9.Enabled = true;
            }
        }

        private void listBoxPointsOnTheBlock_DoubleClick(object sender, EventArgs e)
        {
            _decompositionSecond.ListBoxPointsOnTheBlock_DoubleClick();
        }

        private void comboBoxSecondLevelChooseBlock_SelectedIndexChanged(object sender, EventArgs e)
        {
            _decompositionSecond.ComboBoxSecondLevelChooseBlock_SelectedIndexChanged();
            // Включаем чекбоксы
            EnableSecondLevelCheckBoxes(true);
            // Убираем чеки
            RemoveCheckFromCheckBoxes(_checkBoxSecondLevelList);
        }

        /// <summary>
        /// Убирает check для чекбоксов
        /// </summary>
        /// <param name="checkBoxes"> Список чекбоксов</param>
        private void RemoveCheckFromCheckBoxes(List<CheckBox> checkBoxes)
        {
            foreach (CheckBox checkBox in checkBoxes)
            {
                if (checkBox.Checked == true)
                {
                    checkBox.Checked = false;
                }
            }
        }

        private void checkBoxSecondLevelResponseFunctionBottom_CheckedChanged(object sender, EventArgs e)
        {
            _decompositionSecond.CheckBoxResponseFunctionChange(checkBoxSecondLevelResponseFunctionBottom, chartSecondLevelResponseFunction, "нижняя");
        }

        private void checkBoxSecondLevelResponseFunctionOriginal_CheckedChanged(object sender, EventArgs e)
        {
            _decompositionSecond.CheckBoxResponseFunctionChange(checkBoxSecondLevelResponseFunctionOriginal, chartSecondLevelResponseFunction, "исходное");
        }

        private void checkBoxSecondLevelResponseFunctionTop_CheckedChanged(object sender, EventArgs e)
        {
            _decompositionSecond.CheckBoxResponseFunctionChange(checkBoxSecondLevelResponseFunctionTop, chartSecondLevelResponseFunction, "верхняя");
        }

        private void checkBoxSecondMBottom_CheckedChanged(object sender, EventArgs e)
        {
            _decompositionSecond.CheckBoxMChange(chartSecondLevelM, "нижняя");
        }

        private void checkBoxSecondLevelMBase_CheckedChanged(object sender, EventArgs e)
        {
            _decompositionSecond.CheckBoxMChange(chartSecondLevelM, "исходное");
        }

        private void checkBoxSecondLevelMTop_CheckedChanged(object sender, EventArgs e)
        {
            _decompositionSecond.CheckBoxMChange(chartSecondLevelM, "верхняя");
        }

        private void checkBoxSecondLevelABottom_CheckedChanged(object sender, EventArgs e)
        {
            _decompositionSecond.CheckBoxAChange(chartSecondLevelA, "нижняя");
        }

        private void checkBoxSecondLevelAOriginal_CheckedChanged(object sender, EventArgs e)
        {
            _decompositionSecond.CheckBoxAChange(chartSecondLevelA, "исходное");
        }

        private void checkBoxSecondLevelATop_CheckedChanged(object sender, EventArgs e)
        {
            _decompositionSecond.CheckBoxAChange(chartSecondLevelA, "верхняя");
        }

        private void comboBoxFourthLevelChooseBlock_SelectedIndexChanged(object sender, EventArgs e)
        {    
            for (int i = 0; i < checkedListBoxFourthLevelAvailablePoints.Items.Count; i++)
            {
                if (checkedListBoxFourthLevelAvailablePoints.GetItemChecked(i) == true)
                {
                    checkedListBoxFourthLevelAvailablePoints.SetItemChecked(i, false);
                }
            }

            _decompositionFourth.ComboBoxFourthLevelChooseBlock_SelectedIndexChanged();
            chartFourthLevel.Series.Clear();
        }

        private void checkedListBoxFourthLevelAvailablePoints_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            _decompositionFourth.CheckedListBoxFourthLevelAvailablePoints_ItemCheck();
        }


        private void chartFirstLevelResponseFunction_Click(object sender, EventArgs e)
        {
            OpenChartOnFullScreen(chartFirstLevelResponseFunction);
        }

        private ChartControl CloneChart(ChartControl chart)
        {
            MemoryStream stream = new MemoryStream();
            ChartControl clonedChart = chart;
            clonedChart.Serializer.Save(stream);
            clonedChart = new ChartControl();
            clonedChart.Serializer.Load(stream);
            return clonedChart;
        }

        private void OpenChartOnFullScreen(ChartControl chart)
        {
            fullScreenChart fullScreen = new fullScreenChart(CloneChart(chart));
            fullScreen.ShowDialog();
        }

        private void chartFirstLevelM_Click(object sender, EventArgs e)
        {
            OpenChartOnFullScreen(chartFirstLevelM);
        }

        private void chartFirstLevelA_Click(object sender, EventArgs e)
        {
            OpenChartOnFullScreen(chartFirstLevelA);
        }

        private void chartSecondLevelResponseFunction_Click(object sender, EventArgs e)
        {
            OpenChartOnFullScreen(chartSecondLevelResponseFunction);
        }

        private void chartSecondLevelM_Click(object sender, EventArgs e)
        {
            OpenChartOnFullScreen(chartSecondLevelM);
        }

        private void chartSecondLevelA_Click(object sender, EventArgs e)
        {
            OpenChartOnFullScreen(chartSecondLevelA);
        }

        private void chartFourthLevel_Click(object sender, EventArgs e)
        {
            OpenChartOnFullScreen(chartFourthLevel);
        }

        private void ToolStripMenuItemSave_Click(object sender, EventArgs e)
        {
            _fileManager.ChangeDataInTextFile(numericUpDownMeasurementError.Value, numericUpDownSmoothingFactor.Value);
            showSaveStatus(true);
        }

        private void numericUpDownMeasurementError_ValueChanged(object sender, EventArgs e)
        {
            if (_flagFirstOpen)
            {
                _flagFirstOpen = false;
                return;
            }
            else
            {
                showSaveStatus(false);
                ResetFormAfterSmoothingFactorChanged();
            }
        }

        private void ButtonToCancelAllCheckboxes(params CheckBox[] checkBoxes)
        {
            foreach (CheckBox checkBox in  checkBoxes)
            {
                if (checkBox.Checked == true)
                {
                    checkBox.Checked = false;
                }
            }
        }

        private void buttonFirstLevelRemoveAllAlpha_Click(object sender, EventArgs e)
        {
            ButtonToCancelAllCheckboxes(checkBoxFirstLevelResponseFunctionBottom, checkBoxFirstLevelResponseFunctionOriginal, checkBoxFirstLevelResponseFunctionTop);
        }

        private void buttonFirstLevelRemoveAllM_Click(object sender, EventArgs e)
        {
            ButtonToCancelAllCheckboxes(checkBoxFirstLevelMBottom, checkBoxFirstLevelMOriginal, checkBoxFirstLevelMTop);
        }

        private void buttonFirstLevelRemoveAllAlpha_Click_1(object sender, EventArgs e)
        {
            ButtonToCancelAllCheckboxes(checkBoxFirstLevelABottom, checkBoxFirstLevelAOriginal, checkBoxFirstLevelATop);
        }

        private void buttonSecondLevelRemoveAllAlpha_Click(object sender, EventArgs e)
        {
            ButtonToCancelAllCheckboxes(checkBoxSecondLevelResponseFunctionBottom, checkBoxSecondLevelResponseFunctionOriginal, checkBoxSecondLevelResponseFunctionTop);
        }

        private void buttonSecondLevelRemoveAllM_Click(object sender, EventArgs e)
        {
            ButtonToCancelAllCheckboxes(checkBoxSecondLevelMBottom, checkBoxSecondLevelMOriginal, checkBoxSecondLevelMBottom);
        }

        private void buttonSecondLevelRemoveAllAlpha_Click_1(object sender, EventArgs e)
        {
            ButtonToCancelAllCheckboxes(checkBoxSecondLevelABottom, checkBoxSecondLevelAOriginal, checkBoxSecondLevelATop);
        }

        private void toolStripButtonAboutAuthor_Click(object sender, EventArgs e)
        {
            FormAboutAuthor formAboutAuthor = new FormAboutAuthor();
            formAboutAuthor.ShowDialog();
        }

        private void toolStripButtonGuideForm_Click(object sender, EventArgs e)
        {
            GuideForm guideForm = new GuideForm();
            guideForm.ShowDialog();
        }

    }

}
