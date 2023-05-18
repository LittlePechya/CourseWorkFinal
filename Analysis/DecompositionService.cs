using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CourseWorkFinal.Analysis
{
    public class DecompositionService
    {
        List<List<Double>> MValuesLists;
        List<List<Double>> SmoothMValues = new List<List<Double>>();
        List<List<Double>> SmoothAValues = new List<List<Double>>();
        Calculations calculations = new Calculations();

        public List<List<Double>> FirstLevelMValues(DataGridView elevationTable, DataTable dataTable, Double T, Double A)
        {

            MValuesLists = new List<List<Double>>();

            DataGridView bottomTable = calculations.BottomOrTopTable(dataTable, T, elevationTable, "-");
            DataGridView topTable = calculations.BottomOrTopTable(dataTable, T, elevationTable, "+");

            List<Double> listOfBottomMValues = calculations.MValues(bottomTable);
            List<Double> listOfTopMValues = calculations.MValues(topTable);
            List<Double> smoothTopMValues = calculations.SmoothValue(listOfTopMValues, A);
            List<Double> smoothBottomMValues = calculations.SmoothValue(listOfBottomMValues, A);
            List<Double> listOfMValues = calculations.MValues(elevationTable);
            List<Double> smoothMValues = calculations.SmoothValue(listOfMValues, A);

            //Комментариями подписаны индексы, где что хранится, чтобы потом удобно было заполнять таблицу или строить график
            MValuesLists.Add(listOfBottomMValues); //0
            MValuesLists.Add(listOfTopMValues); //1
            MValuesLists.Add(smoothTopMValues); //2 
            MValuesLists.Add(smoothBottomMValues); //3
            MValuesLists.Add(listOfMValues); //4
            MValuesLists.Add(smoothMValues); //5

            SmoothMValues.Add(calculations.SmoothValue(listOfBottomMValues, A)); //0
            SmoothMValues.Add(calculations.SmoothValue(listOfTopMValues, A)); //1
            SmoothMValues.Add(calculations.SmoothValue(listOfMValues, A)); //2


            return MValuesLists;
        }

        public List<List<Double>> SecondLevelMValues(DataGridView elevationTable, DataTable dataTable, Double T, Double A, List<String> marks)
        {
            MValuesLists = new List<List<Double>>();

            DataGridView bottomLineTable = calculations.BottomOrTopTableToSecondLevel(elevationTable, marks, T, "-");
            DataGridView topLineTable = calculations.BottomOrTopTableToSecondLevel(elevationTable, marks, T, "+");

            List<Double> listOfBottomMValues = calculations.MValues(bottomLineTable);
            List<Double> listOfTopMValues = calculations.MValues(topLineTable);
            List<Double> forecastTopMValue = calculations.SmoothValue(listOfTopMValues, A);
            List<Double> forecastBottomMValue = calculations.SmoothValue(listOfBottomMValues, A);
            List<Double> listOfMValues = calculations.MValues(elevationTable, marks);
            List<Double> forecastMValue = calculations.SmoothValue(listOfMValues, A);

            //Комментариями подписаны индексы, где что хранится, чтобы потом удобно было заполнять таблицу или строить график
            MValuesLists.Add(listOfBottomMValues); //0
            MValuesLists.Add(listOfTopMValues); //1
            MValuesLists.Add(forecastTopMValue); //2
            MValuesLists.Add(forecastBottomMValue); //3
            MValuesLists.Add(listOfMValues); //4
            MValuesLists.Add(forecastMValue); //5

            SmoothMValues.Clear();

            SmoothMValues.Add(calculations.SmoothValue(listOfBottomMValues, A)); //0
            SmoothMValues.Add(calculations.SmoothValue(listOfTopMValues, A)); //1
            SmoothMValues.Add(calculations.SmoothValue(listOfMValues, A)); //2

            return MValuesLists;
        }

        public List<List<Double>> FirstLevelAValues(DataGridView elevationTable, DataTable dataTable, Double T, Double A, DataGridView dataGridViewFirstLevelPhaseCoordinates)
        {
            DataGridView bottomTable = calculations.BottomOrTopTable(dataTable, T, elevationTable, "-");
            DataGridView topTable = calculations.BottomOrTopTable(dataTable, T, elevationTable, "+");

            List<List<Double>> AValuesLists = new List<List<Double>>();
            List<Double> listOfBottomAValues = calculations.AValues(bottomTable, MValuesLists[0]);
            List<Double> listOfTopAValues = calculations.AValues(topTable, MValuesLists[1]);
            List<Double> forecastTopAValue = calculations.SmoothValue(listOfTopAValues, A);
            List<Double> forecastBottomAValue = calculations.SmoothValue(listOfBottomAValues, A);
            List<Double> listOfAValues = calculations.AValues(elevationTable, MValuesLists[4]);
            List<Double> forecastAValue = calculations.SmoothValue(listOfAValues, A);

            //Комментариями подписаны индексы, где что хранится, чтобы потом удобно было заполнять таблицу или строить график
            AValuesLists.Add(listOfBottomAValues); //0
            AValuesLists.Add(listOfTopAValues); //1
            AValuesLists.Add(forecastTopAValue); //2
            AValuesLists.Add(forecastBottomAValue); //3
            AValuesLists.Add(listOfAValues); //4
            AValuesLists.Add(forecastAValue); //5

            SmoothAValues.Add(calculations.SmoothValue(listOfBottomAValues, A));
            SmoothAValues.Add(calculations.SmoothValue(listOfTopAValues, A));
            SmoothAValues.Add(calculations.SmoothValue(listOfAValues, A));

            FillPhaseCoordinatesTable(elevationTable, MValuesLists, AValuesLists, dataGridViewFirstLevelPhaseCoordinates);

            return AValuesLists;
        }

        public List<List<Double>> SecondLevelAValues(DataGridView elevationTable, DataTable dataTable, Double T, Double A, List<String> marks, DataGridView dataGridViewSecondLevelCoordinates)
        {
            List<List<Double>> AValuesLists = new List<List<Double>>();

            DataGridView bottomTable = calculations.BottomOrTopTableToSecondLevel(elevationTable, marks, T, "-");
            DataGridView topTable = calculations.BottomOrTopTableToSecondLevel(elevationTable, marks, T, "+");
            List<Double> listOfBottomAValues = calculations.AValues(bottomTable, MValuesLists[0]);
            List<Double> listOfTopAValues = calculations.AValues(topTable, MValuesLists[1]);
            List<Double> forecastTopAValue = calculations.SmoothValue(listOfTopAValues, A);
            List<Double> forecastBottomAValue = calculations.SmoothValue(listOfBottomAValues, A);
            List<Double> listOfAValues = calculations.AValues(elevationTable, MValuesLists[4], marks);
            List<Double> forecastAValue = calculations.SmoothValue(listOfAValues, A);

            //Комментариями подписаны индексы, где что хранится, чтобы потом удобно было заполнять таблицу или строить график
            AValuesLists.Add(listOfBottomAValues); //0
            AValuesLists.Add(listOfTopAValues); //1
            AValuesLists.Add(forecastTopAValue); //2
            AValuesLists.Add(forecastBottomAValue); //3
            AValuesLists.Add(listOfAValues); //4
            AValuesLists.Add(forecastAValue); //5

            SmoothAValues.Clear();

            SmoothAValues.Add(calculations.SmoothValue(listOfBottomAValues, A));
            SmoothAValues.Add(calculations.SmoothValue(listOfTopAValues, A));
            SmoothAValues.Add(calculations.SmoothValue(listOfAValues, A));

            FillPhaseCoordinatesTable(elevationTable, MValuesLists, AValuesLists, dataGridViewSecondLevelCoordinates);

            return AValuesLists;
        }

        private void FillPhaseCoordinatesTable(DataGridView elevationTable, List<List<Double>> Mvalues, List<List<Double>> Avalues, DataGridView table)
        {

            table.Columns.Clear();

            for (int i = 0; i < 13; i++)
            {
                table.Columns.Add(new DataGridViewTextBoxColumn());
            }

            table.Columns[0].Name = "Эпоха";
            table.Columns[1].Name = "М(t)";
            table.Columns[2].Name = "М(t)+";
            table.Columns[3].Name = "М(t)-";
            table.Columns[4].Name = "A(M)";
            table.Columns[5].Name = "A(M)+";
            table.Columns[6].Name = "A(M)-";
            table.Columns[7].Name = "Сглаженное M(t)";
            table.Columns[8].Name = "Сглаженное М(t)+";
            table.Columns[9].Name = "Сглаженное М(t)-";
            table.Columns[10].Name = "Сглаженное А(М)";
            table.Columns[11].Name = "Сглаженное A(M)+";
            table.Columns[12].Name = "Сглаженное A(M)-";

            for (int i = 0; i < elevationTable.Rows.Count - 1; i++)
            {
                table.Rows.Add();
                table.Rows[i].Cells[0].Value = elevationTable.Rows[i].Cells[0].Value;
                table.Rows[i].Cells[1].Value = Mvalues[4][i];
                table.Rows[i].Cells[2].Value = Mvalues[0][i];
                table.Rows[i].Cells[3].Value = Mvalues[1][i];
                table.Rows[i].Cells[4].Value = Avalues[4][i];
                table.Rows[i].Cells[5].Value = Avalues[0][i];
                table.Rows[i].Cells[6].Value = Avalues[1][i];
            }
            table.Rows.Add();
            int lastIndex = table.Rows.GetLastRow(0x0);
            table.Rows[lastIndex - 1].Cells[0].Value = Convert.ToDouble(elevationTable.Rows[lastIndex - 2].Cells[0].Value) + 1;

            for (int i = 0; i < elevationTable.Rows.Count; i++)
            {
                table.Rows[i].Cells[7].Value = SmoothMValues[2][i];
                table.Rows[i].Cells[8].Value = SmoothMValues[1][i];
                table.Rows[i].Cells[9].Value = SmoothMValues[0][i];
                table.Rows[i].Cells[10].Value = SmoothAValues[2][i];
                table.Rows[i].Cells[11].Value = SmoothAValues[1][i];
                table.Rows[i].Cells[12].Value = SmoothAValues[0][i];
            }
        }

        public DataGridView FillObjectStatusTable(DataGridView dataTable, List<List<Double>> lists, DataGridView elevatorTable)
        {
            dataTable.Rows.Clear();
            dataTable.Columns.Clear();

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
            dataTable.Rows[elevatorTable.Rows.Count - 1].Cells[1].Value = lists[3].Last();
            //Заполняем таблицу значениями M(вверх) + прогноз
            for (int i = 0; i < lists[1].Count; i++)
            {
                dataTable.Rows[i].Cells[3].Value = lists[1][i];

            }
            dataTable.Rows[elevatorTable.Rows.Count - 1].Cells[3].Value = lists[2].Last();
            //Заполняем таблицу значениями M(исходное) + прогноз
            for (int i = 0; i < lists[4].Count; i++)
            {
                dataTable.Rows[i].Cells[2].Value = lists[4][i];

            }
            dataTable.Rows[elevatorTable.Rows.Count - 1].Cells[2].Value = lists[5].Last();
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
    }
}
