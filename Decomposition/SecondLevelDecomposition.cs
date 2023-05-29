﻿using CourseWorkFinal.Analysis;
using CourseWorkFinal.Chart;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ChartControl = System.Windows.Forms.DataVisualization.Charting.Chart;

namespace CourseWorkFinal.Decomposition
{
    public class SecondLevelDecomposition
    {
        // Поля для расчетов
        private double _smoothingFactor;
        private double _measurementErorr;
        private List<List<double>> _AValuesLists;
        private List<List<double>> _MValuesLists;
        private List<List<string>> _points;
        private Dictionary<Int32, string> _blocksName;
        private int _blockCount;
        private int _needPointsCount;
        private int _pointsCount;
        private int _currentBlock = 0;
        private List<Int32> _epochList;

        // Поля-компоненты формы, передающиеся из MainForm
        private ListBox _listBoxAllPointsOfTheObject;
        private ListBox _listBoxPointsOnTheBlock;
        private Label _labelPointsOfTheSelectedBlock;
        private ComboBox _comboBoxSecondLevelChooseBlock;
        private DataGridView _dataGridViewSecondLevelPhaseCoordinates;
        private DataGridView _dataGridViewSecondLevelObjectStatus;
        private ChartControl _chartSecondLevelResponseFunction;
        private ChartControl _chartSecondLevelM;
        private ChartControl _chartSecondLevelA;

        private DataGridView _coordinatesTableZ;
        private DataTable _dataTable;

        // Создание объекта для расчетов
        private Calculations _calculations = new Calculations();
        private DecompositionService _decompositionService;

        // Логическое поле, которое включается только после того, когда пользователь распределил все точки по блокам
        public bool pointsAreDistributed = false;
        // Логическое поле, нужно чтобы списки не очищались повторно после того, как уже были очищены
        public bool resetFlag = false;

        public SecondLevelDecomposition(double smoothingFactor, double measurementError, DataGridView coordinatesTableZ, int BlockCount, int pointsCount,
            ListBox listBoxAllPointsOfTheObject, ListBox listBoxPointsOnTheblock, Label labelPointsOfTheSelectedBlock,
            ChartControl chartSecondLevelResponseFunction, ChartControl chartSecondLevelM, ChartControl chartSecondLevelA,
            ComboBox comboBoxSecondLevelChooseBlock, DataGridView dataGridViewSecondLevelObjectStatus, DataGridView dataGridViewSecondLevelPhaseCoordinates)
        {

            _smoothingFactor = smoothingFactor;
            _measurementErorr = measurementError;
            _coordinatesTableZ = coordinatesTableZ;
            _blockCount = BlockCount;
            _pointsCount = pointsCount;

            _listBoxAllPointsOfTheObject = listBoxAllPointsOfTheObject;
            _listBoxPointsOnTheBlock = listBoxPointsOnTheblock;
            _labelPointsOfTheSelectedBlock = labelPointsOfTheSelectedBlock;

            _chartSecondLevelResponseFunction = chartSecondLevelResponseFunction;
            _chartSecondLevelM = chartSecondLevelM;
            _chartSecondLevelA = chartSecondLevelA;

            _dataGridViewSecondLevelObjectStatus = dataGridViewSecondLevelObjectStatus;
            _dataGridViewSecondLevelPhaseCoordinates = dataGridViewSecondLevelPhaseCoordinates;

            _comboBoxSecondLevelChooseBlock = comboBoxSecondLevelChooseBlock;

            // Заполняем словарь, сопоставляющий индекс блока с буквой
            _blocksName = new Dictionary<Int32, string>
            {
                { 0, "A" },
                { 1, "Б" },
                { 2, "В" },
                { 3, "Г" },
                { 4, "Д" },
                { 5, "Е" }
            };

            SecondLevelDecompositionLoad();

        }

        private void SecondLevelDecompositionLoad()
        {
            // Настройка графиков
            ChartService.SetResponseFunctionSettings(_chartSecondLevelResponseFunction);
            ChartService.SetExponentialSmoothSettings(_chartSecondLevelM);
            ChartService.SetExponentialSmoothSettings(_chartSecondLevelA);

            // Создание объекта для декомпозиции
            _decompositionService = new DecompositionService();

            // Устанавливается начальное значение подписи для лейбла
            _labelPointsOfTheSelectedBlock.Text = "Точки блока: " + _blocksName[_currentBlock];
            _points = new List<List<string>>();

            // Очистка листбоксов
            _listBoxAllPointsOfTheObject.Items.Clear();
            _listBoxPointsOnTheBlock.Items.Clear();

            // Заполнение листбокса всеми точками
            // Начинаем с 1, так как в 0 записано слово "эпоха"
            for (int i = 1; i < _coordinatesTableZ.Columns.Count; i++) 
            {
                _listBoxAllPointsOfTheObject.Items.Add(_coordinatesTableZ.Columns[i].Name);
            }

            // Расчет необходимого количества точек на каждом блоке (количество должно быть равным)
            int requiredPointsCount = _pointsCount;
            while (requiredPointsCount % _blockCount !=0)
            {
                requiredPointsCount--;
            }
            _needPointsCount = requiredPointsCount / _blockCount;
        }

        /// <summary>
        /// Метод проверяет, достаточно ли точек на блоке если да, то возвращает true
        /// </summary>
        /// <returns></returns>
        private bool CheckRequiredPointsOnBlock()
        {
            if (_listBoxPointsOnTheBlock.Items.Count == _needPointsCount)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Метод вызывается при двойном нажатии на элемент списка со всеми точками
        /// Этот элемент перебрасывается в список точек конкретного блока
        /// </summary>
        public void AllPointsListBox_DoubleClick()
        {
            // Если выбран не пустой элемент списка
            if (_listBoxAllPointsOfTheObject.SelectedItems != null)
            {
                // Добавляем точку в список блока и убираем из списка всех точек
                _listBoxPointsOnTheBlock.Items.Add(_listBoxAllPointsOfTheObject.SelectedItem);
                _listBoxAllPointsOfTheObject.Items.Remove(_listBoxAllPointsOfTheObject.SelectedItem);
            }

            // Проверка, достаточно ли точек на блоке
            if (CheckRequiredPointsOnBlock())
            {
                // Если да, то точки сохраняются в список
                _points.Add(new List<string>());
                // Заносим в созданый список все точки
                foreach (string str in _listBoxPointsOnTheBlock.Items)
                {
                    _points[_currentBlock].Add(str);
                }
                // Индекс текущего блока увеличивается на 1
                _currentBlock++;
                if (_currentBlock == _blockCount)
                {
                    _listBoxAllPointsOfTheObject.Enabled = false;
                }
                // Очищаем список распределенных по блоку точек
                _listBoxPointsOnTheBlock.Items.Clear();
                // Меняем подпись в соответствии со следующим блоком
                _labelPointsOfTheSelectedBlock.Text = "Точки блока: " + _blocksName[_currentBlock];
                _labelPointsOfTheSelectedBlock.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            }

            // Если все точки распределены, то заполняем комбоБокс для выбора блока
            if (_currentBlock == _blockCount)
            {
                for (int i = 0; i < _blockCount; i++)
                {
                    _comboBoxSecondLevelChooseBlock.Items.Add(_blocksName[i]);
                }

                // Эта строчка нужна, чтобы при перерасчете уровня декомпозиции comboBox был пустым
                _comboBoxSecondLevelChooseBlock.SelectedItem = -1;

                pointsAreDistributed = true;
                resetFlag = false;
            }
        }
        
        /// <summary>
        /// При двойном клике по элементу списка текущего блока этот элемент перемещается обратно в список всех точек объекта
        /// </summary>
        public void ListBoxPointsOnTheBlock_DoubleClick()
        {
            if (_listBoxPointsOnTheBlock.SelectedItem != null)
            {
                _listBoxAllPointsOfTheObject.Items.Add(_listBoxPointsOnTheBlock.SelectedItem);
                _listBoxPointsOnTheBlock.Items.Remove(_listBoxPointsOnTheBlock.SelectedItem);
            }
        }
        /// <summary>
        /// Если пользователь выбирает блок в комбоБоксе на 2 уровне, то выполняются расчеты для выбранного блока
        /// </summary>
        public void ComboBoxSecondLevelChooseBlock_SelectedIndexChanged()
        {
            // Расчет значений M
            _MValuesLists = _decompositionService.SecondLevelMValues(_coordinatesTableZ, _dataTable, _measurementErorr, 
                _smoothingFactor, _points[_comboBoxSecondLevelChooseBlock.SelectedIndex]);

            // Заполнение таблицы состояния блока
            _dataGridViewSecondLevelObjectStatus = _decompositionService.FillObjectStatusTable(_dataGridViewSecondLevelObjectStatus, _MValuesLists, _coordinatesTableZ);

            // Расчет значений A
            _AValuesLists = _decompositionService.SecondLevelAValues(_coordinatesTableZ, _dataTable, _measurementErorr, 
                _smoothingFactor, _points[_comboBoxSecondLevelChooseBlock.SelectedIndex], _dataGridViewSecondLevelPhaseCoordinates);

            // Расчет сглаженных значений для графиков сглаживания
            List<double> smoothMValues = _calculations.SmoothValue(_MValuesLists[4], _smoothingFactor);
            List<double> smoothAValues = _calculations.SmoothValue(_AValuesLists[4], _smoothingFactor);
            _MValuesLists.Add(smoothMValues);
            _AValuesLists.Add(smoothAValues);

            // Заполнение листа с эпохами
            _epochList = new List<Int32>();
            for (int i = 0; i < _dataGridViewSecondLevelObjectStatus.Rows.Count - 1; i++)
            {
                _epochList.Add(Convert.ToInt32(_dataGridViewSecondLevelObjectStatus.Rows[i].Cells[0].Value));
            }

            // Добавление прогнозной эпохи
            _epochList.Add(_epochList.Last() +1);

            Calculations.HighligteDangerRows(_dataGridViewSecondLevelObjectStatus);
        }

        public void ResetSecondLevel()
        {
            if (resetFlag == false)
            {
                _AValuesLists.Clear();
                _epochList.Clear();
                pointsAreDistributed = false;
                _chartSecondLevelA.Series.Clear();
                _chartSecondLevelM.Series.Clear();
                _chartSecondLevelResponseFunction.Series.Clear();
                _comboBoxSecondLevelChooseBlock.Items.Clear();
            }

            resetFlag = true;
        }

        public void CheckBoxResponseFunctionChange(CheckBox checkBoxResponseFunction, ChartControl responseFunctionChart, string location)
        {
            /*MValuesLists.Add(listOfBottomMValues); //0
            MValuesLists.Add(listOfTopMValues); //1
            MValuesLists.Add(smoothTopMValues); //2 
            MValuesLists.Add(smoothBottomMValues); //3
            MValuesLists.Add(listOfMValues); //4
            MValuesLists.Add(smoothMValues); //5*/

            /* AValuesLists.Add(listOfBottomAValues); //0
             AValuesLists.Add(listOfTopAValues); //1
             AValuesLists.Add(forecastTopAValue); //2
             AValuesLists.Add(forecastBottomAValue); //3
             AValuesLists.Add(listOfAValues); //4
             AValuesLists.Add(forecastAValue); //5*/
            if (_comboBoxSecondLevelChooseBlock.SelectedItem == null) return;
            
            switch (location)
            {
                case "нижняя":
                    ChartService.AddLineToChart(responseFunctionChart, "Функция отклика (нижняя граница)", "Прогнозное значение функции отклика (нижняя граница)", _MValuesLists[0], _AValuesLists[0], _MValuesLists[3], _AValuesLists[3], _epochList);
                    break;
                case "исходное":
                    ChartService.AddLineToChart(responseFunctionChart, "Функция отклика (исходное)", "Прогнозное значение функции отклика (исходное)", _MValuesLists[4], _AValuesLists[4], _MValuesLists[5], _AValuesLists[5], _epochList);
                    break;
                case "верхняя":
                    ChartService.AddLineToChart(responseFunctionChart, "Функция отклика (верхняя граница)", "Прогнозное значение функции отклика (верхняя граница)", _MValuesLists[1], _AValuesLists[1], _MValuesLists[2], _AValuesLists[2], _epochList);
                    break;
            }
        }

        public void CheckBoxMChange(ChartControl chartM, string location)
        {
            // 4-5 исходная граница
            // 0-3 нижняя граница
            // 1-2 верхняя граница
            switch (location)
            {
                case "нижняя":
                    ChartService.AddXYLineToChart(chartM, "Функция отклика (нижняя граница)", "Прогнозное значение функции отклика (нижняя граница)", _MValuesLists[0], _MValuesLists[3], _epochList);
                    break;
                case "исходное":
                    ChartService.AddXYLineToChart(chartM, "Функция отклика (исходное)", "Прогнозное значение функции отклика (исходное)", _MValuesLists[4], _MValuesLists[5], _epochList);
                    break;
                case "верхняя":
                    ChartService.AddXYLineToChart(chartM, "Функция отклика (верхняя граница)", "Прогнозное значение функции отклика (верхняя граница)", _MValuesLists[1], _MValuesLists[2], _epochList);
                    break;
            }
        }

        public void CheckBoxAChange(ChartControl chartA, string location)
        {
            // 4-5 исходная граница
            // 0-3 нижняя граница
            // 1-2 верхняя граница
            switch (location)
            {
                case "нижняя":
                    ChartService.AddXYLineToChart(chartA, "Функция отклика (нижняя граница)", "Прогнозное значение функции отклика (нижняя граница)", _AValuesLists[0], _AValuesLists[3], _epochList);
                    break;
                case "исходное":
                    ChartService.AddXYLineToChart(chartA, "Функция отклика (исходное)", "Прогнозное значение функции отклика (исходное)", _AValuesLists[4], _AValuesLists[5], _epochList);
                    break;
                case "верхняя":
                    ChartService.AddXYLineToChart(chartA, "Функция отклика (верхняя граница)", "Прогнозное значение функции отклика (верхняя граница)", _AValuesLists[1], _AValuesLists[2], _epochList);
                    break;
            }
        }

        /// <summary>
        /// Метод для получения заполненного списка точек по объектам
        /// </summary>
        /// <returns></returns>
        public List<List<string>> GetPoints()
        {
            return _points;
        }
    }
}
