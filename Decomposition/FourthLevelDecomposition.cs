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
            ComboBox comboBoxFourthLevelChooseBlock, CheckedListBox checkedListBoxFourthLevelAvailablePoints, Button buttonFourthLevelReset, ChartControl chartFourthLevel)
        {
            _smoothingFactor = smoothingFactor;
            _measurementErorr = measurementError;
            _coordinatesTableZ = CoordinatesTableZ;
            _blockCount = blockCount;
            _points = points;
            _comboBoxFourthLevelChooseBlock = comboBoxFourthLevelChooseBlock;
            _checkedListBoxFourthLevelAvailablePoints = checkedListBoxFourthLevelAvailablePoints;
            _buttonFourthLevelReset = buttonFourthLevelReset;
            _chartFourthLevel = chartFourthLevel;

            _blocksName = new Dictionary<Int32, string>();
            _blocksName.Add(0, "A");
            _blocksName.Add(1, "Б");
            _blocksName.Add(2, "В");
            _blocksName.Add(3, "Г");
            _blocksName.Add(4, "Д");

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
    }
}
