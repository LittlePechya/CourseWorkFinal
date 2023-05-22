using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.DataVisualization.Charting;
using ChartControl = System.Windows.Forms.DataVisualization.Charting.Chart;

namespace CourseWorkFinal.Chart
{
    public static class ChartService
    {
        /// <summary>
        /// Настройка графика функции отклика
        /// </summary>
        /// <param name="responseFunction"> Chart для функции отклика </param>
        /// <returns></returns>
        public static void SetResponseFunctionSettings(ChartControl responseFunction)
        {
            responseFunction.ChartAreas[0].AxisX.Title = "M";
            responseFunction.ChartAreas[0].AxisY.Title = "Alpha";
            responseFunction.ChartAreas[0].AxisY.IsStartedFromZero = false;
        }

        /// <summary>
        /// Настройка графика экспоненциального сглаживания
        /// </summary>
        /// <param name="expoentialSmooth"></param>
        /// <returns></returns>
        public static void SetExponentialSmoothSettings(ChartControl expoentialSmooth)
        {
            expoentialSmooth.ChartAreas[0].AxisX.Title = "Эпоха";
            expoentialSmooth.ChartAreas[0].AxisY.IsStartedFromZero = false;
        }

        public static void SetFourthLevelChartSetting(ChartControl fourthLevelChart)
        {
            fourthLevelChart.ChartAreas[0].AxisX.Title = "Эпоха";
            fourthLevelChart.ChartAreas[0].AxisY.Title = "Высота";
            fourthLevelChart.ChartAreas[0].AxisY.IsStartedFromZero = false;
        }

        /// <summary>
        /// Настройка отображения серии на графике
        /// </summary>
        /// <param name="chart"></param>
        /// <param name="serieName"></param>
        /// <returns></returns>
        public static void SerieSetSettings(ChartControl chart, string serieName)
        {
            //Включаем маркеры кружочки
            chart.Series[serieName].MarkerStyle = MarkerStyle.Circle;
            //Задаем размер 10
            chart.Series[serieName].MarkerSize = 10;
            //Цвет делаем такой же какой у основного графика
            chart.Series[serieName].MarkerColor = chart.Series[serieName].Color;
            //Вид графика линия
            chart.Series[serieName].ChartType = SeriesChartType.Line;
            // Отображение координат при наведении на точку
            chart.Series[serieName].ToolTip = "X = #VALX, Y = #VALY";
        }

        /// <summary>
        /// Добавление серии, используется на 1 и 4 уровне
        /// </summary>
        /// <param name="chart"></param>
        /// <param name="name"></param>
        /// <param name="forecastName"></param>
        /// <param name="XValue"></param>
        /// <param name="YValue"></param>
        /// <param name="XValue2"></param>
        /// <param name="YValue2"></param>
        /// <param name="epochList"></param>
        /// <returns></returns>
        public static void AddLineToChart(ChartControl chart, string name, string forecastName, List<Double> XValue, List<Double> YValue, List<Double> XValue2, List<Double> YValue2, List<Int32> epochList)
        {
            //Условие проверяет существует ли серия с таким названием, если существует то удаляет, если нет, то добавляем
            if (chart.Series.IndexOf(name) == -1)
            {
                //Добавляем серию по названию и устанавливаем настройки графика
                chart.Series.Add(name);
                //И ставим настройки графика
                SerieSetSettings(chart, name);
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
                chart.Series[forecastName].Points.AddXY(XValue2.Last(), YValue2.Last());
                //Добавляем подпись
                chart.Series[forecastName].Points.Last().Label = (epochList.Last() - 1).ToString();

                //Добавляем последнее значение из рассчитанных предсказанных (потому что нам нужна только последняя точка)
                //И ставим настройки графика
                SerieSetSettings(chart, forecastName);
            }
            else
            {
                //Тут удаляем график, если он был
                DeleteChart(chart, name, forecastName);
            }
        }

        /// <summary>
        /// Используется на втором уровне
        /// </summary>
        /// <param name="chart"></param>
        /// <param name="name"></param>
        /// <param name="smoothName"></param>
        /// <param name="YValue"></param>
        /// <param name="YValue2"></param>
        /// <param name="epochList"></param>
        public static void AddXYLineToChart(ChartControl chart, string name, string smoothName, List<Double> YValue, List<Double> YValue2, List<Int32> epochList)
        {
            //Условие проверяет существует ли серия с таким названием, если существует то удаляет, если нет, то добавляем
            if (chart.Series.IndexOf(name) == -1)
            {
                //Добавляем серию по названию и устанавливаем настройки графика
                chart.Series.Add(name);
                //И ставим настройки графика
                SerieSetSettings(chart, name);
                //Вот эта штука ниже нужна чтобы при наведении на точку появлялись её значения
                chart.Series[name].ToolTip = "X = #VALX, Y = #VALY";

                //В цикле добавляем точки по оси Х и У
                for (int i = 0; i < YValue.Count; i++)
                {

                    chart.Series[name].Points.AddXY(epochList[i], YValue[i]);
                    //Добавляем подпись
                    chart.Series[name].Points[i].Label = i.ToString();
                }
                //Тут добавляем точку прогнозного значения по названию графика
                chart.Series.Add(smoothName);

                //В цикле добавляем точки по оси Х и У
                for (int i = 0; i < YValue2.Count; i++)
                {

                    chart.Series[smoothName].Points.AddXY(epochList[i], YValue2[i]);
                    //Добавляем подпись
                    chart.Series[smoothName].Points[i].Label = i.ToString();
                }

                chart.Series[smoothName].ToolTip = "X = #VALX, Y = #VALY";

                //Добавляем последнее значение из рассчитанных предсказанных (потому что нам нужна только последняя точка)
                //И ставим настройки графика
                SerieSetSettings(chart, smoothName);
            }
            else
            {
                //Тут удаляем график, если он был
                DeleteChart(chart, name, smoothName);
            }
        }

        public static void AddLineToChartOnFourthLevel(ChartControl chart, string name, string forecastName, List<double> XValue, List<double> YValue, List<double> Xvalue2, List<double> YValue2)
        {
            // Если серия уже существует, то происходит удаление

            if (chart.Series.IndexOf(name) == -1)
            {
                // Добавление серии и настройка графика
                chart.Series.Add(name);
                SerieSetSettings(chart, name);

                // Добавление точек по оси Х и У
                for (int i = 0; i < XValue.Count; i++)
                {

                    chart.Series[name].Points.AddXY(XValue[i], YValue[i]);
                    chart.Series[name].Points[i].Label = i.ToString();
                }
                // Добавление прогнозной серии
                chart.Series.Add(forecastName);

                // Добавление прогнозного значения
                chart.Series[forecastName].Points.AddXY(Xvalue2.Last() + 1, YValue2.Last());
                chart.Series[forecastName].Points.Last().Label = (Xvalue2.Last() + 1).ToString();

                // Настройка графика
                SerieSetSettings(chart, forecastName);

            }
            else
            {
                //Тут удаляем график, если он был
                DeleteChart(chart, name, forecastName);
            }
        }

        private static void DeleteChart(ChartControl chart, string serieName, string forecastOrSmoothSerieName)
        {
            chart.Series[serieName].Points.Clear();
            chart.Series.Remove(chart.Series[serieName]);
            chart.Series[forecastOrSmoothSerieName].Points.Clear();
            chart.Series.Remove(chart.Series[forecastOrSmoothSerieName]);
        }
    }
}
