using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CourseWorkFinal.Analysis
{
    public  class Calculations
    {
        public Calculations() { }

        public List<Double> MValues(DataGridView elevatorTable, List<String> marks)
        {
            Double M = 0;

            List<Double> values = new List<Double>();

            List<Double> listOfMValues = new List<Double>();

            for (int i = 0; i < elevatorTable.RowCount - 1; i++)
            {
                foreach (DataGridViewColumn col in elevatorTable.Columns)
                {
                    if (marks.Contains(col.Name))
                    {
                        values.Add(Convert.ToDouble(elevatorTable.Rows[i].Cells[col.Name].Value));

                    }

                }
                foreach (double c in values)
                {
                    M += (c * c);
                }
                listOfMValues.Add(Math.Sqrt(M));
                M = 0;
                values.Clear();

            }
            return listOfMValues;

        }

        public List<Double> MValues(DataGridView elevatorTable)
        {
            Double M = 0;
            //List<Double> values = new List<Double>();
            List<Double> listOfMValues = new List<Double>();
            int bottomEdge = 0;
            int leftEdge = 0;

            if (elevatorTable.Columns[0].Name.Equals("Эпоха"))
            {
                bottomEdge = elevatorTable.RowCount - 1;
                leftEdge = 1;
            }
            else bottomEdge = elevatorTable.RowCount - 2;

            for (int i = 0; i < bottomEdge; i++)
            {
                M = 0;
                for (int j = leftEdge; j < elevatorTable.ColumnCount; j++)
                {
                    //values.Add(Convert.ToDouble(elevatorTable.Rows[i].Cells[j].Value));

                    M += Math.Pow(Convert.ToDouble(elevatorTable.Rows[i].Cells[j].Value), 2);
                }

                /*foreach (double c in values)
                {
                    M += (c * c);
                }*/
                
                listOfMValues.Add(Math.Sqrt(M));
                
                //values.Clear();
            }

            return listOfMValues;

        }

        public List<Double> AValues(DataGridView elevatorTable, List<Double> listOfMValues, List<String> marks)
        {
            //Рассчитываем альфу также как в методе снизу, только не для всех колонок в таблице, а только для тех, которые переданы в листе marks
            Double calculateAcos = 0;
            Double calculateDegree = 0;
            Double summPr = 0;
            Double firstValue = 0;
            Double secondValue = 0;
            List<Double> listOfAlphaValues = new List<Double>();
            listOfAlphaValues.Add(0);

            for (int i = 0; i < elevatorTable.Rows.Count - 2; i++)
            {
                summPr = 0;
                foreach (DataGridViewColumn col in elevatorTable.Columns)
                {
                    if (marks.Contains(col.Name))
                    {
                        firstValue = Convert.ToDouble(elevatorTable.Rows[0].Cells[col.Name].Value);
                        secondValue = Convert.ToDouble(elevatorTable.Rows[i + 1].Cells[col.Name].Value);
                        summPr += firstValue * secondValue;

                    }
                }
                summPr /= listOfMValues[0];
                summPr /= listOfMValues[i + 1];
                summPr = Math.Round(summPr, 15);
                calculateAcos = Math.Acos(summPr);
                listOfAlphaValues.Add(calculateAcos);

            }
            return listOfAlphaValues;

        }

        public List<Double> AValues(DataGridView elevatorTable, List<Double> listOfMValues)
        {
            Double calculateAcos = 0;
            Decimal summPr = 0;
            Decimal firstValue = 0;
            Decimal secondValue = 0;

            //Добавляем нулевое значение в лист с Альфа
            List<Double> result = new List<Double>();
            result.Add(0);

            //Определяем нижнюю границу в таблице и левуюю границу с колонками откуда будем считать
            int bottomEdge = 0;
            int leftEdge = 0;

            //Если первая колонка называется Эпоха, то левой границе присваеваем 1, чтобы не брать в рассчеты колонку Эпоха
            //Нижней границы присваеваем Количество строчек таблицы - 2, изза пустых строчек внизу.
            if (elevatorTable.Columns[0].Name.Equals("Эпоха"))
            {
                leftEdge = 1;
                bottomEdge = elevatorTable.RowCount - 2;
            }
            else bottomEdge = elevatorTable.RowCount - 3;

            //Считаем строчки в цикле от 0 до нижней границы
            for (int i = 0; i < bottomEdge; i++)
            {
                summPr = 0;

                //Тут сумма произведений
                for (int j = leftEdge; j < elevatorTable.ColumnCount; j++)
                {
                    firstValue = Convert.ToDecimal(elevatorTable.Rows[0].Cells[j].Value);
                    secondValue = Convert.ToDecimal(elevatorTable.Rows[i + 1].Cells[j].Value);
                    summPr += firstValue * secondValue;
                }

                summPr /= Convert.ToDecimal(listOfMValues[0]);
                summPr /= Convert.ToDecimal(listOfMValues[i + 1]);

                //ОЧЕНЬ ВАЖНАЯ СТРОКА (тут округляем до 14 знаков после запятой, но в разных вариантах нужно пробовать разные значения, чтобы избежать
                //шумов 
                summPr = Math.Round(summPr, 14);
                //Считаем аркосинус
                calculateAcos = Math.Acos(Convert.ToDouble(summPr));
                //Добавляем в результат
                result.Add(calculateAcos);

            }
            return result;

        }

        public List<Double> SmoothValue(List<Double> listOfValues, Double a)
        {
            List<Double> forecastValues = new List<Double>();
            Double averageSumm = 0;
            Double value = 0;

            //Считаем среднее, если первое число нулевое, то пропускаем
            if (listOfValues[0] == 0)
            {
                for (int i = 1; i < listOfValues.Count; i++)
                {
                    averageSumm += listOfValues[i];
                }
                averageSumm /= listOfValues.Count;
                value = a * listOfValues[0] + (1 - a) * averageSumm;
                forecastValues.Add(value);
            }
            else
            {
                value = a * listOfValues[0] + (1 - a) * listOfValues.Average();
                forecastValues.Add(value);
            }

            //Тут считаем все остальные значения, кроме последнего
            for (int i = 1; i < listOfValues.Count; i++)
            {
                value = a * listOfValues[i] + (1 - a) * forecastValues[i - 1];
                forecastValues.Add(value);
            }

            //Последнее значение считаем по другой формуле
            value = a * forecastValues.Average() + (1 - a) * forecastValues.Last();
            forecastValues.Add(value);

            return forecastValues;
        }

        //Метод для расчета таблицы  нижней и верхней границы
        public DataGridView BottomOrTopTable(DataTable dataTable, Double T, DataGridView elevatorTable, String arithmeticSign)
        {
            //тут считаем нижнюю или верхнюю границу (тоесть пересчитываем всю таблицу)
            //Добавляем необходимые колонки
            DataGridView Table = FillNewTable(dataTable);

            for (int i = 0; i < elevatorTable.Rows.Count - 1; i++)
            {
                //Пропускаем колонку Эпоха , поэтому цикл от 1
                for (int j = 1; j < elevatorTable.ColumnCount; j++)
                {
                    //Если нужна нижняя граница то знак минус
                    if (arithmeticSign.Equals("-"))
                    {
                        Table.Rows[i].Cells[j].Value = Convert.ToDouble(elevatorTable.Rows[i].Cells[j].Value) - T;
                    }
                    //Если верхняя то плюс
                    else if (arithmeticSign.Equals("+"))
                    {
                        Table.Rows[i].Cells[j].Value = Convert.ToDouble(elevatorTable.Rows[i].Cells[j].Value) + T;
                    }

                }
            }
            return Table;
        }

        public DataGridView BottomOrTopTableToSecondLevel(DataGridView elevatorTable, List<String> marks, Double T, String arithmeticSign)
        {
            //Считается также как в методе выше, только включаем не все колонки, а только переданные в листе marks
            DataGridView Table = new DataGridView();

            for (int i = 0; i < marks.Count; i++)
            {
                Table.Columns.Add(new DataGridViewTextBoxColumn());
                Table.Columns[i].Name = marks[i];
            }

            for (int i = 0; i < elevatorTable.Rows.Count; i++)
            {
                Table.Rows.Add();
                foreach (DataGridViewColumn col in elevatorTable.Columns)
                {
                    if (marks.Contains(col.Name))
                    {
                        if (arithmeticSign.Equals("-"))
                        {
                            Table.Rows[i].Cells[col.Name].Value = Convert.ToDouble(elevatorTable.Rows[i].Cells[col.Name].Value) - T;
                        }
                        else if (arithmeticSign.Equals("+"))
                        {
                            Table.Rows[i].Cells[col.Name].Value = Convert.ToDouble(elevatorTable.Rows[i].Cells[col.Name].Value) + T;
                        }

                    }
                }
            }
            return Table;
        }

        private DataGridView FillNewTable(DataTable dataTable)
        {
            //Добавляем колонки из переданного dataTable
            DataGridView newTable = new DataGridView();
            for (int column = 0; column < dataTable.Columns.Count; column++)
            {
                String ColName = dataTable.Columns[column].ColumnName;
                newTable.Columns.Add(ColName, ColName);
                newTable.Columns[column].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }

            for (int row = 0; row < dataTable.Rows.Count; row++)
            {
                newTable.Rows.Add(dataTable.Rows[row].ItemArray);
            }
            return newTable;
        }
    }
}
