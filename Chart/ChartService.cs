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
        public static ChartControl SetResponseFunctionSettings(ChartControl responseFunction)
        {
            responseFunction.ChartAreas[0].AxisX.Title = "M";
            responseFunction.ChartAreas[0].AxisY.Title = "Alpha";
            responseFunction.ChartAreas[0].AxisY.IsStartedFromZero = false;
            return responseFunction;
        }

        /// <summary>
        /// Настройка графика экспоненциального сглаживания
        /// </summary>
        /// <param name="expoentialSmooth"></param>
        /// <returns></returns>
        public static ChartControl SetExponentialSmoothSettings(ChartControl expoentialSmooth)
        {
            expoentialSmooth.ChartAreas[0].AxisX.Title = "Эпоха";
            expoentialSmooth.ChartAreas[0].AxisY.IsStartedFromZero=false;
            return expoentialSmooth;
        }

        /// <summary>
        /// Настройка отображения серии на графике
        /// </summary>
        /// <param name="chart"></param>
        /// <param name="serieName"></param>
        /// <returns></returns>
        public static ChartControl SerieSetSettings(ChartControl chart, string serieName)
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
            return chart;
        }

        /// <summary>
        /// Добавление серии
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
        public static ChartControl AddLineToChart(ChartControl chart, string name, string forecastName, List<Double> XValue, List<Double> YValue, List<Double> XValue2, List<Double> YValue2, List<Int32> epochList)
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
                chart.Series[name].Points.Clear();
                chart.Series.Remove(chart.Series[name]);
                chart.Series[forecastName].Points.Clear();
                chart.Series.Remove(chart.Series[forecastName]);
            }
            return chart;
        }
    }
}
