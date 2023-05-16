using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CourseWorkFinal
{
    /// <summary>
    /// Класс для работы с базой данных, позволяет получать, добавлять и обновлять значение в БД.
    /// Данные из базы данных записываются в DataGridView
    /// </summary>
    internal class Database
    {
        private string pathToDataBase;
        private SQLiteConnection sqlConnection;
        private string tableName;

        /// <summary>
        /// Конструктор класса 
        /// </summary>
        /// <param name="pathToDataBase"> Путь к базе данных</param>
        public Database(string pathToDataBase)
        {
            this.pathToDataBase = pathToDataBase;
        }

        /// <summary>
        /// Установка связи с базой данных
        /// </summary>
        /// <returns> SQLiteConnection </returns>
        public SQLiteConnection GetDataBaseConnection()
        {
            this.sqlConnection = new SQLiteConnection("Data Source=" + pathToDataBase + ";Version=3;");
            this.sqlConnection.Open();
            return this.sqlConnection;
        }

        /// <summary>
        /// Получение списка таблиц из базы данных
        /// </summary>
        public void GetTableNames()
        {
            string SQLQuery = "SELECT name FROM sqlite_master WHERE type='table' ORDER BY name;";
            SQLiteCommand command = new SQLiteCommand(SQLQuery, sqlConnection);
            SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                tableName = reader.GetString(0);
            }
        }

        /// <summary>
        /// Заполнение таблицы DataGridView из таблицы DataTable
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="coordinatesTable"></param>
        /// <returns></returns>
        public DataGridView FillTable(DataTable dt, DataGridView coordinatesTable)
        {
            GetTableNames();
            string SQLQuerySelectAll = "SELECT * FROM [" + tableName + "]";
            ClearDataTable(dt);
            SQLiteCommand command = new SQLiteCommand(sqlConnection);
            SQLiteDataAdapter adapter = new SQLiteDataAdapter(SQLQuerySelectAll, sqlConnection);
            adapter.Fill(dt);
            ChangeCommasToDots(dt);
            adapter = new SQLiteDataAdapter(SQLQuerySelectAll, sqlConnection);
            DataGridViewClear(coordinatesTable);
            coordinatesTable = FillRowsAndCols(dt, coordinatesTable);
            return coordinatesTable;
        }

        /// <summary>
        /// Отправление запроса на замену запятых на точки
        /// </summary>
        /// <param name="dt"> Таблица DataTable, получившая данные из базы данных</param>
        public void ChangeCommasToDots(DataTable dt)
        {
            for (int i = 1; i < dt.Columns.Count; i++)
            {
                string replaceCommasToDots = "UPDATE [" + tableName + "] SET[" + i + "] = REPLACE([" + i + "],',','.')";
                SQLiteCommand command = new SQLiteCommand(replaceCommasToDots, sqlConnection);
                command.ExecuteNonQuery();
                Thread.Sleep(5); // Если не поставить таймер, может возникнуть ошибка
            }
        }

        /// <summary>
        /// Очистка таблицы DataTable
        /// </summary>
        /// <param name="dt"></param>
        public void ClearDataTable(DataTable dt)
        {
            dt.Rows.Clear();
            dt.Columns.Clear();
        }

        /// <summary>
        /// Очистка таблицы DataGridView
        /// </summary>
        /// <param name="coordinatesTable"></param>
        public void DataGridViewClear(DataGridView coordinatesTable)
        {
            coordinatesTable.Rows.Clear();
            coordinatesTable.Columns.Clear();
        }

        /// <summary>
        /// Заполнение строк и столбцов таблицы dataGridView
        /// </summary>
        /// <param name="dataTable"></param>
        /// <param name="coordinatesTable"></param>
        public DataGridView FillRowsAndCols(DataTable dataTable, DataGridView coordinatesTable)
        {
            for (int column = 0; column < dataTable.Columns.Count; column++)
            {
                string columnName = dataTable.Columns[column].ColumnName;
                coordinatesTable.Columns.Add(columnName, columnName);
                coordinatesTable.Columns[column].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }

            for (int row = 0; row < dataTable.Rows.Count; row++)
            {
                coordinatesTable.Rows.Add(dataTable.Rows[row].ItemArray);
            }

            return coordinatesTable;
        }

        /// <summary>
        /// Рассчитывает значения по формулам для строки, добавленной пользователем.
        /// </summary>
        /// <param name="coordinatesTable"></param>
        /// <param name="db"></param>
        /// <param name="epochCount"></param>
        /// <returns></returns>
        public DataGridView CalculateNewRowValues(DataGridView coordinatesTable, Database db, int newRowIndex)
        {
            double delta = 0, averageDelta = 0, newCellValue = 0;
            /// new row number - podvoh
            int newRowNumber = coordinatesTable.RowCount - 1;
            Random random = new Random();

            // Цикл начинается с 1, чтобы пропустить колонку, содержащую номер эпохи
            for (int col = 1; col < coordinatesTable.Columns.Count; col++)
            {
                for (int rows = 0; rows < coordinatesTable.Rows.Count - 1; rows++)
                {
                    if (Convert.ToDouble(coordinatesTable.Rows[rows + 1].Cells[col].Value) != 0)
                    {
                        // Расчет delta между значением и предыдущим значением
                        delta = Math.Abs(Convert.ToDouble(coordinatesTable.Rows[rows].Cells[col].Value) - Convert.ToDouble(coordinatesTable.Rows[rows + 1].Cells[col].Value));
                    }


                    averageDelta += delta;
                    delta = 0;
                }

                // Расчет среднего значения averageData
                averageDelta /= coordinatesTable.Rows.Count - 1;
                // Генерация случайного значения в диапазоне от -averageDelta до +averageDelta
                newCellValue = random.NextDouble() * (averageDelta - (-averageDelta)) + averageDelta;
                // Округление до 4 знаков и добавление значения в новую эпоху
                coordinatesTable.Rows[newRowNumber].Cells[col].Value = 
                Math.Round(newCellValue + Convert.ToDouble(coordinatesTable.Rows[newRowNumber - 1].Cells[col].Value), 4);

                // TODO: nextEpochValue - следующий номер эпохи, которая должна быть добавлена (после добавления нужно ее увеличивать)
                // можно в параметры передать эту переменную по указателю из main формы (всегда будешь знать какую добавить следующую)
                // а можно из бд получать просто вообще-то так и  надо))))))))))
                // Пример:
                // public DataGridView CalculateNewRowValues(DataGridView coordinatesTable, Database db, int newRowIndex, ref int nextEpochValue)

                //db.AddValuesInNewRowQuery(col, nextEpochValue, Convert.ToDouble(coordinatesTable.Rows[newRowIndex].Cells[col].Value));
                // nextEpochValue++;
            }

            // TODO: понять какие значениия добавлять надо (row, column, value) db.AddValuesInNewRowQuery();

            coordinatesTable.Rows.Add();
            return coordinatesTable;
        }

        public void AddValuesInNewRowQuery(int column, int row, double value)
        {
            string SQLQuery = "UPDATE [" + tableName + "] SET \"" + column + "\" = \"" + value + "\" WHERE Эпоха = \'" + row + "\'";
            DoSQLQuery(SQLQuery);
        }
        /// <summary>
        /// Добавление новой строки в БД с помощью INSERT
        /// </summary>
        /// <param name="index"></param>
        public void AddNewRowQuery(int index)
        {
            index++;
            string SQLQuery = "INSERT INTO [" + tableName + "] (Эпоха) VALUES (\"" + index + "\")";
            DoSQLQuery(SQLQuery);
        }

        public void DeleteRowQuery(string index)
        {
            string SQLQuery = "DELETE FROM [" + tableName + "] WHERE Эпоха = \'" + index + "\'";
            DoSQLQuery(SQLQuery);
        }

        public void DoSQLQuery(string SQLQuery)
        {
            SQLiteCommand command = new SQLiteCommand(sqlConnection);
            command.CommandText = SQLQuery;
            command.ExecuteNonQuery();
        } 

    }
}



