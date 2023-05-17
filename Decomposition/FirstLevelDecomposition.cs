using CourseWorkFinal.Analysis;
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
    internal class FirstLevelDecomposition
    {
        // Поля-компоненты, которые передаются из основной формы
        private DataGridView coordinatesTableZ;
        private DataGridView phaseCoordinatesTable;
        private DataGridView objectStatusTable;

        private ChartControl chartFirstLevelM;
        private ChartControl chartFirstLevelA;
        private ChartControl responseFunctionChart;

        private DataTable dataTable;

        private CheckBox checkBoxResponseFunctionBottom;
        private CheckBox checkBoxResponseFunctionOriginal;
        private CheckBox checkBoxResponseFunctionTop;

        // Поля для расчетов
        private double smoothingFactor;
        private double measurementErorr;
        private List<List<Double>> AValues;
        private List<List<Double>> MValues;
        private List<Int32> epochList;

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
            this.smoothingFactor = smoothingFactor;
            this.measurementErorr = measurementError;
            this.coordinatesTableZ = coordinatesTableZ;
            this.phaseCoordinatesTable = phaseCoordinatesTable;
            this.objectStatusTable = objectStatusTable;
            this.chartFirstLevelM = chartFirstLevelM;
            this.chartFirstLevelA = chartFirstLevelA;
            this.responseFunctionChart = responseFunctionChart; 
            this.dataTable = dataTable;

            FirsLevelDecompositionLoad();
            CheckBoxResponseFunctionBottomChange(checkBoxResponseFunctionBottom, responseFunctionChart);

        }

        private void FirsLevelDecompositionLoad()
        {
            // Настройка графика функции отклика
            responseFunctionChart = ChartService.SetResponseFunctionSettings(responseFunctionChart);
            // Настройка графика со сглаживанием
            chartFirstLevelM = ChartService.SetExponentialSmoothSettings(chartFirstLevelM);
            chartFirstLevelA = ChartService.SetExponentialSmoothSettings(chartFirstLevelA);
            // Создание объекта класса decompositionService для расчета M и A
            DecompositionService decompositionService = new DecompositionService();
            // Расчет M
            MValues = decompositionService.FirstLevelMValues(coordinatesTableZ, dataTable, measurementErorr, smoothingFactor);
            AValues = decompositionService.FirstLevelAValues(coordinatesTableZ, dataTable, measurementErorr, smoothingFactor, phaseCoordinatesTable);
            // На основе M заполням таблицу оценки состояния c помощью метода из decompositionService
            objectStatusTable = decompositionService.FillObjectStatusTable(objectStatusTable, MValues, coordinatesTableZ);
            // Расчет сглаженных значений для графика сглаживания
            List<Double> smoothMValues = calculations.SmoothValue(MValues[4], smoothingFactor);
            List<Double> smoothAValues = calculations.SmoothValue(AValues[4], smoothingFactor);
            MValues.Add(smoothMValues);
            AValues.Add(smoothAValues);
            // Заполнение листа с эпохами
            epochList = new List<Int32>();
            fillEpochList(epochList);
            // Добавление прогнозной эпохи
            epochList.Add(epochList.Last() + 1);
        }

        private void fillEpochList(List<Int32> epochList)
        {
            for (int i = 0; i < objectStatusTable.Rows.Count - 1; i++)
            {
                epochList.Add(Convert.ToInt32(objectStatusTable.Rows[i].Cells[0].Value));
            }
        }

        private void CheckBoxResponseFunctionBottomChange(CheckBox checkBoxResponseFunctionBottom, ChartControl responseFunctionChart)
        {
            ChartService.AddLineToChart(responseFunctionChart, "Функция отклика (нижняя граница)", "Прогнозное значение функции отклика (нижняя граница)", MValues[4], AValues[4], MValues[5], AValues[5], epochList);
        }
    }
}
