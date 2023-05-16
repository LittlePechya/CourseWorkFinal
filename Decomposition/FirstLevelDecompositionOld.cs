using CourseWorkFinal.Analysis;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace CourseWorkFinal
{
    internal class FirstLevelDecompositionOld
    {
        private double smoothingFactor;
        private double measurementError;
        private DataGridView coordinatesTable;
        private DataGridView phaseTable;
        private DataGridView statusTable;
        private Calculations calculations = new Calculations();
        private DataTable dataTable;
        private List<List<Double>> AValues;
        private List<List<Double>> MValues;
        private List<Int32> epochList;

        /// <summary>
        /// Конструктрор класса
        /// </summary>
        /// <param name="smoothingFactor"> Значение сглаживание </param>
        /// <param name="measureentError"> Значение погрешности </param>
        /// <param name="coordinatesTable"> Таблица координат Z</param>
        /// <param name="dt"> Таблица со значениями</param>
        public FirstLevelDecompositionOld(double smoothingFactor,
            double measureentError, DataGridView coordinatesTable, DataGridView phaseTable, DataGridView statusTable, DataTable dt)
        {
            this.smoothingFactor = smoothingFactor;
            this.measurementError = measureentError;
            this.coordinatesTable = coordinatesTable;
            this.phaseTable = phaseTable;
            this.statusTable = statusTable;
            this.dataTable = dt;
        }

        public void firstDecompositionLevel(System.Windows.Forms.DataVisualization.Charting.Chart responseFunctionChart, System.Windows.Forms.DataVisualization.Charting.Chart expSmoothChart)
        {
            // TODO: Все, что с графиком связано унести в Chart/ChartService.cs

            // ChartService.Function(..)

            // Для графиков необходимо отключить отсчет значений по Y с 0
            // Также здесь подписываются оси графиков
            responseFunctionChart.ChartAreas[0].AxisX.Title = "M";
            responseFunctionChart.ChartAreas[0].AxisY.Title = "Alpha";
            responseFunctionChart.ChartAreas[0].AxisY.IsStartedFromZero = false;
            expSmoothChart.ChartAreas[0].AxisX.Title = "Эпоха";
            expSmoothChart.ChartAreas[0].AxisY.IsStartedFromZero = false;

            
            // Создание экземпляра класса decompositionService для расчета M и a 

            // TODO: для расчета M и А есть класс Calculation -> его использовать тут надо
            DecompositionService decompositionService = new DecompositionService();

            // Расчет M
            MValues = decompositionService.FirstLevelMValues(coordinatesTable, dataTable, measurementError, smoothingFactor);

            // Заполнение таблицы оценки состояния на основе значений M
            statusTable = FillTable(statusTable, MValues, coordinatesTable);

            // Расчет значений a
            AValues = decompositionService.FirstLevelAValues(coordinatesTable, dataTable, measurementError, smoothingFactor, phaseTable);

            // Расчет сглаженных значений
            List<Double> smoothMValues = calculations.SmoothValue(MValues[4], smoothingFactor);
            List<Double> smoothAValues = calculations.SmoothValue(AValues[4], smoothingFactor);
            MValues.Add(smoothMValues);
            AValues.Add(smoothAValues);

            //Заполняем лист с эпохами
            epochList = new List<Int32>();
            for (int i = 0; i < statusTable.Rows.Count - 1; i++)
            {
                epochList.Add(Convert.ToInt32(statusTable.Rows[i].Cells[0].Value));
            }
            //Добавляем прогнозную эпоху
            epochList.Add(epochList.Last() + 1);

        }

        private DataGridView FillTable(DataGridView dataTable, List<List<Double>> lists, DataGridView elevatorTable)
        {
            dataTable.Rows.Clear();
            dataTable.Columns.Clear();

            // TODO: вот эта таблица с оценкой используется на 1 и 2 уровнях => можно вынести в отдельную функцкию создание
            // столбов и в качестве параметра передавать просто dataGridView нужный

            //Добавляем столбики
            dataTable.Columns.Add(new DataGridViewTextBoxColumn());
            dataTable.Columns.Add(new DataGridViewTextBoxColumn());
            dataTable.Columns.Add(new DataGridViewTextBoxColumn());
            dataTable.Columns.Add(new DataGridViewTextBoxColumn());
            dataTable.Columns.Add(new DataGridViewTextBoxColumn());
            dataTable.Columns.Add(new DataGridViewTextBoxColumn());
            dataTable.Columns.Add(new DataGridViewTextBoxColumn());
            dataTable.Columns[0].Name = "Эпоха";
            dataTable.Columns[1].Name = "М(нижнее)";
            dataTable.Columns[2].Name = "М";
            dataTable.Columns[3].Name = "М(верхнее)";
            dataTable.Columns[4].Name = "2E";
            dataTable.Columns[5].Name = "L";
            dataTable.Columns[6].Name = "Состояние";

            int forecastIndex = 0;
            //Записываем эпохи
            for (int i = 0; i < elevatorTable.RowCount; i++)
            {
                dataTable.Rows.Add();
                dataTable.Rows[i].Cells[0].Value = elevatorTable.Rows[i].Cells[0].Value;
                forecastIndex = i + 1;
            }
            dataTable.Rows[forecastIndex - 1].Cells[0].Value = Convert.ToDouble(elevatorTable.Rows[forecastIndex - 2].Cells[0].Value) + 1;
            //Заполняем таблицу значениями M(низ) + прогноз
            for (int i = 0; i < lists[0].Count; i++)
            {
                dataTable.Rows[i].Cells[1].Value = lists[0][i];

            }
            dataTable.Rows[coordinatesTable.Rows.Count - 1].Cells[1].Value = lists[3].Last();
            //Заполняем таблицу значениями M(вверх) + прогноз
            for (int i = 0; i < lists[1].Count; i++)
            {
                dataTable.Rows[i].Cells[3].Value = lists[1][i];

            }
            dataTable.Rows[coordinatesTable.Rows.Count - 1].Cells[3].Value = lists[2].Last();
            //Заполняем таблицу значениями M(исходное) + прогноз
            for (int i = 0; i < lists[4].Count; i++)
            {
                dataTable.Rows[i].Cells[2].Value = lists[4][i];

            }
            dataTable.Rows[coordinatesTable.Rows.Count - 1].Cells[2].Value = lists[5].Last();
            //Считаем значение 2Е (по модулю (М(верх) - М(низ)))
            for (int i = 0; i < dataTable.Rows.Count - 1; i++)
            {
                dataTable.Rows[i].Cells[4].Value = Math.Abs(Convert.ToDouble(dataTable.Rows[i].Cells[1].Value) - Convert.ToDouble(dataTable.Rows[i].Cells[3].Value));
            }
            //Считаем L (по модулю (M(0) - M(i)))
            for (int i = 0; i < dataTable.Rows.Count - 1; i++)
            {
                dataTable.Rows[i].Cells[5].Value = Math.Abs(Convert.ToDouble(dataTable.Rows[0].Cells[2].Value) - Convert.ToDouble(dataTable.Rows[i].Cells[2].Value));
            }

            // TODO: это тоже можно в отдельную функцию fillEstimationTableColumn()
            //Считаем есть ли выход за границу
            for (int i = 0; i < dataTable.Rows.Count - 1; i++)
            {
                if (Convert.ToDouble(dataTable.Rows[i].Cells[5].Value) < Convert.ToDouble(dataTable.Rows[i].Cells[4].Value) / 2)
                {
                    dataTable.Rows[i].Cells[6].Value = "В пределе";
                }
                else dataTable.Rows[i].Cells[6].Value = "Выход за границу";
            }
            return dataTable;
        }

        private void serieSetSettings(System.Windows.Forms.DataVisualization.Charting.Chart chart, String serieName)
        {
            //Включаем маркеры кружочки
            chart.Series[serieName].MarkerStyle = MarkerStyle.Circle;
            //Задаем размер 10
            chart.Series[serieName].MarkerSize = 10;
            //Цвет делаем такой же какой у основного графика
            chart.Series[serieName].MarkerColor = chart.Series[serieName].Color;
            //Вид графика линия
            chart.Series[serieName].ChartType = SeriesChartType.Line;
            //Это что отображается когда наводиштся на точку (ее координаты)
            chart.Series[serieName].ToolTip = "X = #VALX, Y = #VALY";
        }

        private void addLineToChart(System.Windows.Forms.DataVisualization.Charting.Chart chart, String name, String forecastName, List<Double> XValue, List<Double> YValue, List<Double> Xvalue2, List<Double> YValue2)
        {
            //Условие проверяет существует ли серия с таким названием, если существует то удаляет, если нет, то добавляем
            if (chart.Series.IndexOf(name) == -1)
            {
                //Добавляем серию по названию и устанавливаем настройки графика
                chart.Series.Add(name);
                //И ставим настройки графика
                serieSetSettings(chart, name);
                //Вот эта штука ниже нужна чтобы при наведении на точку появлялись её значения
                chart.Series[name].ToolTip = "X = #VALX, Y = #VALY";

                //В цикле добавляем точки по оси Х и У
                for (int i = 0; i < XValue.Count; i++)
                {

                    chart.Series[name].Points.AddXY(XValue[i], YValue[i]);
                    //Добавляем подпись
                    chart.Series[name].Points[i].Label = i.ToString();
                }
                //Тут добавляем точку прогнозного значения по названию графика
                chart.Series.Add(forecastName);

                //Добавляем её координаты на график
                chart.Series[forecastName].Points.AddXY(Xvalue2.Last(), YValue2.Last());
                //Добавляем подпись
                chart.Series[forecastName].Points.Last().Label = (epochList.Last() - 1).ToString();

                //Добавляем последнее значение из рассчитанных предсказанных (потому что нам нужна только последняя точка)
                //И ставим настройки графика
                serieSetSettings(chart, forecastName);
            }
            else
            {
                //Тут удаляем график, если он был
                chart.Series[name].Points.Clear();
                chart.Series.Remove(chart.Series[name]);
                chart.Series[forecastName].Points.Clear();
                chart.Series.Remove(chart.Series[forecastName]);
            }
        }

        private void initialFuntion_CheckedChanged(object sender, EventArgs e, System.Windows.Forms.DataVisualization.Charting.Chart responseFunctionChart)
        {
            addLineToChart(responseFunctionChart, "Функция отклика", "Прогнозное исходной ф-ии", MValues[4], AValues[4], MValues[5], AValues[5]);
        }

        private void topBorderFunction_CheckedChanged(object sender, EventArgs e, System.Windows.Forms.DataVisualization.Charting.Chart responseFunctionChart)
        {
            addLineToChart(responseFunctionChart, "Верхняя граница", "Прогнозное верхней границы", MValues[1], AValues[1], MValues[2], AValues[2]);
        }

        private void bottomBorderFunction_CheckedChanged(object sender, EventArgs e, System.Windows.Forms.DataVisualization.Charting.Chart responseFunctionChart)
        {
            addLineToChart(responseFunctionChart, "Нижняя граница", "Прогнозное нижней границы", MValues[0], AValues[0], MValues[3], AValues[3]);
        }

        private void smoothMChart_CheckedChanged(object sender, EventArgs e, System.Windows.Forms.DataVisualization.Charting.Chart expSmoothChart)
        {


            //Если серии с таким названием нет, то заходим внутрь условия и добавляем
            if (expSmoothChart.Series.IndexOf("М реальное") == -1)
            {
                //Меняем подпись оси
                expSmoothChart.ChartAreas[0].AxisY.Title = "Значение М";
                //Добавляем серию
                expSmoothChart.Series.Add("М реальное");
                //Применяем настройки
                serieSetSettings(expSmoothChart, "М реальное");
                //Добавляем точки на график
                for (int i = 0; i < MValues[4].Count; i++)
                {
                    expSmoothChart.Series["М реальное"].Points.AddXY(epochList[i], MValues[4][i]);
                    //Подписи точек
                    expSmoothChart.Series["М реальное"].Points[i].Label = i.ToString();
                }
                //Добавляем график сглаженного значения
                expSmoothChart.Series.Add("М сглаженное");
                //Применяем настройки
                serieSetSettings(expSmoothChart, "М сглаженное");
                //Добавляем точки на график
                for (int i = 0; i < MValues[6].Count; i++)
                {
                    expSmoothChart.Series["М сглаженное"].Points.AddXY(epochList[i], MValues[6][i]);
                    //Добавляем подпись
                    expSmoothChart.Series["М сглаженное"].Points[i].Label = i.ToString();
                }
            }
            //Если график есть, то удаляем серию
            else
            {
                expSmoothChart.Series["М реальное"].Points.Clear();
                expSmoothChart.Series.Remove(expSmoothChart.Series["М реальное"]);
                expSmoothChart.Series["М сглаженное"].Points.Clear();
                expSmoothChart.Series.Remove(expSmoothChart.Series["М сглаженное"]);
            }
            //Если включены оба графика, меняем подпись на значение
            if (expSmoothChart.Series.Count > 2)
            {
                expSmoothChart.ChartAreas[0].AxisY.Title = "Значение";
            }
        }
    }
}
