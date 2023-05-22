using CourseWorkFinal.Analysis;
using CourseWorkFinal.Chart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ChartControl = System.Windows.Forms.DataVisualization.Charting.Chart;

namespace CourseWorkFinal.Decomposition
{
    public class FourthLevelDecomposition
    {
        // Поля-компоненты, которые передаются из основной формы
        ComboBox _comboBoxFourthLevelChooseBlock;
        CheckedListBox _checkedListBoxFourthLevelAvailablePoints;
        Button _buttonFourthLevelSelectAll;
        Button _buttonFourthLevelReset;
        ChartControl _chartFourthLevel;

        DataGridView _coordinatesTableZ;

        // Поля для расчетов
        private double _smoothingFactor;
        private double _measurementErorr;
        private List<List<string>> _points;
        private Dictionary<Int32, string> _blocksName;
        private int _blockCount;
        private List<double> _epochCount;

        //Объекты для расчетов

        public FourthLevelDecomposition(double smoothingFactor, double measurementError, DataGridView CoordinatesTableZ, int blockCount, List<List<string>> points,
            ComboBox comboBoxFourthLevelChooseBlock, CheckedListBox checkedListBoxFourthLevelAvailablePoints, Button buttonFourthLevelSelectAll, Button buttonFourthLevelReset, ChartControl chartFourthLevel)
        {
            _smoothingFactor = smoothingFactor;
            _measurementErorr = measurementError;
            _coordinatesTableZ = CoordinatesTableZ;
            _blockCount = blockCount;
            _points = points;
            _comboBoxFourthLevelChooseBlock = comboBoxFourthLevelChooseBlock;
            _checkedListBoxFourthLevelAvailablePoints = checkedListBoxFourthLevelAvailablePoints;
            _buttonFourthLevelReset = buttonFourthLevelSelectAll;
            _buttonFourthLevelReset = buttonFourthLevelReset;
            _chartFourthLevel = chartFourthLevel;

            _blocksName = new Dictionary<Int32, string>
            {
                { 0, "A" },
                { 1, "Б" },
                { 2, "В" },
                { 3, "Г" },
                { 4, "Д" }
            };

            FourthLevelDecompositionLoad();
        }

        private void FourthLevelDecompositionLoad()
        {
            // При загрузке формы получаем спислк эпох для построения графиков
            _epochCount = new List<double>();
            for (int i = 0; i < _coordinatesTableZ.Rows.Count - 1; i++)
            {
                _epochCount.Add(Convert.ToDouble(_coordinatesTableZ.Rows[i].Cells[0].Value));
            }

            // Настраиваем график 
            ChartService.SetFourthLevelChartSetting(_chartFourthLevel);

            // В список добавляются доступные для выбора блоки
            for (int i = 0; i < _blockCount; i++)
            {
                _comboBoxFourthLevelChooseBlock.Items.Add(_blocksName[i]);
            }

            // Эта строчка нужна, чтобы при перерасчете уровня декомпозиции comboBox был пустым
            _comboBoxFourthLevelChooseBlock.SelectedItem = -1;
        }

        /// <summary>
        /// Обновляет доступные для выбора точки после выбора блока из комбоБокса
        /// </summary>
        public void ComboBoxFourthLevelChooseBlock_SelectedIndexChanged()
        {
            _checkedListBoxFourthLevelAvailablePoints.Items.Clear();
            if (_points.Count > 0)
            {
                foreach (string str in _points[_comboBoxFourthLevelChooseBlock.SelectedIndex])
                {
                    _checkedListBoxFourthLevelAvailablePoints.Items.Add(str);
                }
            }

        }

        /// <summary>
        /// Метод рассчитывает значения и выводит их на график при нажатии галочки в чекБоксе
        /// </summary>
        public void CheckedListBoxFourthLevelAvailablePoints_ItemCheck()
        {
            Calculations calculations = new Calculations();
            List<double> pointsHeight = new List<double>();

            for (int i = 0; i < _coordinatesTableZ.Rows.Count - 1; i++)
            {
                // Заполнение списка высот точек
                pointsHeight.Add(Convert.ToDouble(_coordinatesTableZ.Rows[i].Cells[Convert.ToInt32(_checkedListBoxFourthLevelAvailablePoints.SelectedItem) + 1].Value));
            }

            // Получение прогнозных значений высот
            List<double> forecastPointsHeight = calculations.SmoothValue(pointsHeight, _smoothingFactor);

            // Добавление графика 
            ChartService.AddLineToChartOnFourthLevel(_chartFourthLevel, _checkedListBoxFourthLevelAvailablePoints.SelectedItem.ToString(),
                (_checkedListBoxFourthLevelAvailablePoints.SelectedItem.ToString() + " прогноз"), _epochCount, pointsHeight, _epochCount, forecastPointsHeight);
        }

        public void ResetFourthLevel(List<List<string>> points, ChartControl fourthLevelChart, CheckedListBox availablePoints, ComboBox chooseBlockComboBox)
        {
            points.Clear();
            fourthLevelChart.Series.Clear();
            availablePoints.Items.Clear();
            chooseBlockComboBox.Items.Clear();
            chooseBlockComboBox.SelectedItem = -1;

        }
    }
}
