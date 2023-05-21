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
        }

        private void FirsLevelDecompositionLoad()
        {
            // Настройка графика функции отклика
            ChartService.SetResponseFunctionSettings(responseFunctionChart);
            // Настройка графика со сглаживанием
            ChartService.SetExponentialSmoothSettings(chartFirstLevelM);
            ChartService.SetExponentialSmoothSettings(chartFirstLevelA);
            // Создание объекта класса decompositionService для расчета M и A
            DecompositionService decompositionService = new DecompositionService();
            // Расчет M
            MValues = decompositionService.FirstLevelMValues(coordinatesTableZ, dataTable, measurementErorr, smoothingFactor);
            // Расчет alpha
            AValues = decompositionService.FirstLevelAValues(coordinatesTableZ, dataTable, measurementErorr, smoothingFactor, phaseCoordinatesTable);
            // На основе M заполням таблицу оценки состояния c помощью метода из decompositionService
            objectStatusTable = decompositionService.FillObjectStatusTable(objectStatusTable, MValues, coordinatesTableZ);
            // Расчет сглаженных значений для графиков сглаживания
            List<double> smoothMValues = calculations.SmoothValue(MValues[4], smoothingFactor);
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
                    ChartService.AddLineToChart(responseFunctionChart, "Функция отклика (нижняя граница)", "Прогнозное значение функции отклика (нижняя граница)", MValues[0], AValues[0], MValues[3], AValues[3], epochList);
                    break;
                case "исходное":
                    ChartService.AddLineToChart(responseFunctionChart, "Функция отклика (исходное)", "Прогнозное значение функции отклика (исходное)", MValues[4], AValues[4], MValues[5], AValues[5], epochList);
                    break;
                case "верхняя":
                    ChartService.AddLineToChart(responseFunctionChart, "Функция отклика (верхняя граница)", "Прогнозное значение функции отклика (верхняя граница)", MValues[1], AValues[1], MValues[2], AValues[2], epochList);
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
                    ChartService.AddXYLineToChart(chartM, "Функция отклика (нижняя граница)", "Прогнозное значение функции отклика (нижняя граница)", MValues[0], MValues[3],  epochList);
                    break;
                case "исходное":
                    ChartService.AddXYLineToChart(chartM, "Функция отклика (исходное)", "Прогнозное значение функции отклика (исходное)", MValues[4], MValues[5], epochList);
                    break;
                case "верхняя":
                    ChartService.AddXYLineToChart(chartM, "Функция отклика (верхняя граница)", "Прогнозное значение функции отклика (верхняя граница)", MValues[1], MValues[2], epochList);
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
                    ChartService.AddXYLineToChart(chartM, "Функция отклика (нижняя граница)", "Прогнозное значение функции отклика (нижняя граница)", AValues[0], AValues[3], epochList);
                    break;
                case "исходное":
                    ChartService.AddXYLineToChart(chartM, "Функция отклика (исходное)", "Прогнозное значение функции отклика (исходное)", AValues[4], AValues[5], epochList);
                    break;
                case "верхняя":
                    ChartService.AddXYLineToChart(chartM, "Функция отклика (верхняя граница)", "Прогнозное значение функции отклика (верхняя граница)", AValues[1], AValues[2], epochList);
                    break;
            }
        }

        public void ResetFirstLevel(ChartControl chartM, ChartControl chartA)
        {
            AValues.Clear();
            MValues.Clear();
            epochList.Clear();
            //chartM.Series.Clear();
            //chartA.Series.Clear();
        }

    }
}

