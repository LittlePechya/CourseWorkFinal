using CourseWorkFinal.Analysis;
using CourseWorkFinal.Chart;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ChartControl = System.Windows.Forms.DataVisualization.Charting.Chart;

namespace CourseWorkFinal.Decomposition
{
    internal class FirstLevelDecomposition
    {
        // Поля-компоненты, которые передаются из основной формы
        private DataGridView _coordinatesTableZ;
        private DataGridView _phaseCoordinatesTable;
        private DataGridView _objectStatusTable;

        private ChartControl _chartFirstLevelM;
        private ChartControl _chartFirstLevelA;
        private ChartControl _responseFunctionChart;

        private DataTable _dataTable;

  

        // Поля для расчетов
        private double _smoothingFactor;
        private double _measurementErorr;
        private List<List<Double>> _AValues;
        private List<List<Double>> _MValues;
        private List<Int32> _epochList;

        // Создание объекта для расчетов
        private Calculations calculations = new Calculations();

        /// <summary>
        /// Конструктор класса
        /// </summary>
        /// <param name="smoothingFactor"> Значение сглаживания </param>
        /// <param name="measurementError"> Значение погрешности </param>
        /// <param name="coordinatesTableZ"> Таблица координат Z</param>
        /// <param name="phaseCoordinatesTable"> Таблица фазовых координат с первого уровня декомпозиции</param>
        /// <param name="objectStatusTable"> Таблица состояния объекта с первого уровня декомпозиции </param>
        /// /// <param name="responseFunctionChart"> График функции отклика </param>
        /// <param name="exponenthialSmoothChart"> График экпоненчиального сглаживания </param>
        /// <param name="dataTable"></param>
        public FirstLevelDecomposition(double smoothingFactor, double measurementError,
            DataGridView coordinatesTableZ, DataGridView phaseCoordinatesTable, 
            DataGridView objectStatusTable, ChartControl chartFirstLevelM, ChartControl chartFirstLevelA,
            ChartControl responseFunctionChart, DataTable dataTable)
        {
            this._smoothingFactor = smoothingFactor;
            this._measurementErorr = measurementError;
            this._coordinatesTableZ = coordinatesTableZ;
            this._phaseCoordinatesTable = phaseCoordinatesTable;
            this._objectStatusTable = objectStatusTable;
            this._chartFirstLevelM = chartFirstLevelM;
            this._chartFirstLevelA = chartFirstLevelA;
            this._responseFunctionChart = responseFunctionChart; 
            this._dataTable = dataTable;

            FirsLevelDecompositionLoad();
        }

        private void FirsLevelDecompositionLoad()
        {
            // Настройка графика функции отклика
            ChartService.SetResponseFunctionSettings(_responseFunctionChart);
            // Настройка графика со сглаживанием
            ChartService.SetExponentialSmoothSettings(_chartFirstLevelM);
            ChartService.SetExponentialSmoothSettings(_chartFirstLevelA);
            // Создание объекта класса decompositionService для расчета M и A
            DecompositionService decompositionService = new DecompositionService();
            // Расчет M
            _MValues = decompositionService.FirstLevelMValues(_coordinatesTableZ, _dataTable, _measurementErorr, _smoothingFactor);
            // Расчет alpha
            _AValues = decompositionService.FirstLevelAValues(_coordinatesTableZ, _dataTable, _measurementErorr, _smoothingFactor, _phaseCoordinatesTable);
            // На основе M заполням таблицу оценки состояния c помощью метода из decompositionService
            _objectStatusTable = decompositionService.FillObjectStatusTable(_objectStatusTable, _MValues, _coordinatesTableZ);
            // Расчет сглаженных значений для графиков сглаживания
            List<double> smoothMValues = calculations.SmoothValue(_MValues[4], _smoothingFactor);
            List<Double> smoothAValues = calculations.SmoothValue(_AValues[4], _smoothingFactor);
            _MValues.Add(smoothMValues);
            _AValues.Add(smoothAValues);
            // Заполнение листа с эпохами
            _epochList = new List<Int32>();
            fillEpochList(_epochList);
            // Добавление прогнозной эпохи
            _epochList.Add(_epochList.Last() + 1);

            // Тут закрашиваем 
            Calculations.HighligteDangerRows(_objectStatusTable);
        }

        private void fillEpochList(List<Int32> epochList)
        {
            for (int i = 0; i < _objectStatusTable.Rows.Count - 1; i++)
            {
                epochList.Add(Convert.ToInt32(_objectStatusTable.Rows[i].Cells[0].Value));
            }
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
            switch (location)
            {
                case "нижняя":
                    ChartService.AddLineToChart(responseFunctionChart, "Функция отклика (нижняя граница)", "Прогнозное значение функции отклика (нижняя граница)", _MValues[0], _AValues[0], _MValues[3], _AValues[3], _epochList);
                    break;
                case "исходное":
                    ChartService.AddLineToChart(responseFunctionChart, "Функция отклика (исходное)", "Прогнозное значение функции отклика (исходное)", _MValues[4], _AValues[4], _MValues[5], _AValues[5], _epochList);
                    break;
                case "верхняя":
                    ChartService.AddLineToChart(responseFunctionChart, "Функция отклика (верхняя граница)", "Прогнозное значение функции отклика (верхняя граница)", _MValues[1], _AValues[1], _MValues[2], _AValues[2], _epochList);
                    break;
            }
        }

        public void CheckBoxMChange(ChartControl chartM, string location)
        {
            // 4-5 исходная граница
            // 0-3 нижняя граница
            // 1-2 верхняя граница
            switch(location)
            {
                case "нижняя":
                    ChartService.AddXYLineToChart(chartM, "Функция отклика (нижняя граница)", "Прогнозное значение функции отклика (нижняя граница)", _MValues[0], _MValues[3],  _epochList);
                    break;
                case "исходное":
                    ChartService.AddXYLineToChart(chartM, "Функция отклика (исходное)", "Прогнозное значение функции отклика (исходное)", _MValues[4], _MValues[5], _epochList);
                    break;
                case "верхняя":
                    ChartService.AddXYLineToChart(chartM, "Функция отклика (верхняя граница)", "Прогнозное значение функции отклика (верхняя граница)", _MValues[1], _MValues[2], _epochList);
                    break;
            }
        }

        public void CheckBoxAChange(ChartControl chartM, string location)
        {
            // 4-5 исходная граница
            // 0-3 нижняя граница
            // 1-2 верхняя граница
            switch (location)
            {
                case "нижняя":
                    ChartService.AddXYLineToChart(chartM, "Функция отклика (нижняя граница)", "Прогнозное значение функции отклика (нижняя граница)", _AValues[0], _AValues[3], _epochList);
                    break;
                case "исходное":
                    ChartService.AddXYLineToChart(chartM, "Функция отклика (исходное)", "Прогнозное значение функции отклика (исходное)", _AValues[4], _AValues[5], _epochList);
                    break;
                case "верхняя":
                    ChartService.AddXYLineToChart(chartM, "Функция отклика (верхняя граница)", "Прогнозное значение функции отклика (верхняя граница)", _AValues[1], _AValues[2], _epochList);
                    break;
            }
        }

        public void ResetFirstLevel(ChartControl chartM, ChartControl chartA)
        {
            _AValues.Clear();
            _MValues.Clear();
            _epochList.Clear();
            //chartM.Series.Clear();
            //chartA.Series.Clear();
        }

    }
}

