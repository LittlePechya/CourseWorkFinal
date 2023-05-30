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
        private List<string> tableNames;

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
        /// Получение списка таблиц из базы данных и выбор таблицы
        /// Работает, только если в БД одна таблица
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
        /// 
        /// </summary>
        /// <param name="databasePath"></param>
        /// <returns></returns>
        private List<string> GetTableNames(string databasePath)
        {
           tableNames = new List<string>();

            using (var connection = new SQLiteConnection($"Data Source={databasePath};Version=3;"))
            {
                connection.Open();
                var command = new SQLiteCommand("SELECT name FROM sqlite_master WHERE type='table';", connection);
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    tableNames.Add(reader.GetString(0));
                }
            }

            return tableNames;
        }

        /// <summary>
        /// Заполнение таблицы DataGridView из таблицы DataTable
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="coordinatesTable"></param>
        /// <returns></returns>
        public string FillTable(DataTable dt, DataGridView coordinatesTable)
        {
            tableNames = GetTableNames(pathToDataBase);
            if (tableNames.Count > 1)
            {

                chooseTable chooseTable = new chooseTable(tableNames);
                chooseTable.ShowDialog();
                tableName = chooseTable.SelectedTableName;
            }
            else
            {
                tableName = "Данные";
            }

            string SQLQuerySelectAll = "SELECT * FROM [" + tableName + "]";
            ClearDataTable(dt);
            SQLiteCommand command = new SQLiteCommand(sqlConnection);
            SQLiteDataAdapter adapter = new SQLiteDataAdapter(SQLQuerySelectAll, sqlConnection);
            adapter.Fill(dt);
            ChangeCommasToDots(dt, tableName);
            adapter = new SQLiteDataAdapter(SQLQuerySelectAll, sqlConnection);
            DataGridViewClear(coordinatesTable);
            coordinatesTable = FillRowsAndCols(dt, coordinatesTable);
            return tableName;
        }

        public DataGridView UpdateFillTable(DataTable dt, DataGridView coordinatesTable, string tableName)
        {
            string SQLQuerySelectAll = "SELECT * FROM [" + tableName + "]";
            ClearDataTable(dt);
            SQLiteCommand command = new SQLiteCommand(sqlConnection);
            SQLiteDataAdapter adapter = new SQLiteDataAdapter(SQLQuerySelectAll, sqlConnection);
            adapter.Fill(dt);
            ChangeCommasToDots(dt, tableName);
            adapter = new SQLiteDataAdapter(SQLQuerySelectAll, sqlConnection);
            DataGridViewClear(coordinatesTable);
            coordinatesTable = FillRowsAndCols(dt, coordinatesTable);
            return coordinatesTable;
        }
        /// <summary>
        /// Отправление запроса на замену запятых на точки
        /// </summary>
        /// <param name="dt"> Таблица DataTable, получившая данные из базы данных</param>
        public void ChangeCommasToDots(DataTable dt, string tableName)
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
        public void CalculateNewRowValues(DataGridView coordinatesTable, Database db, int newRowIndex, int maxEpochName, string tableName)
        {
            double delta = 0, averageDelta = 0, newCellValue = 0;
            Random random = new Random();
            // Мы передали newRowIndex = 9
            // Тут newROwIndex -1, потому что мы обращаемся к 9 строчке, которая восьмая по нумерации
            coordinatesTable.Rows[newRowIndex - 1].Cells[0].Value = maxEpochName;
            //AddNewRowQuery(Convert.ToDouble(maxEpoch + 1));
            // Цикл начинается с 1, чтобы пропустить колонку, содержащую номер эпохи
            for (int cols = 1; cols < coordinatesTable.Columns.Count; cols++)
            {

                for (int rows = 0; rows < coordinatesTable.Rows.Count - 1; rows++)
                {
                    if (Convert.ToDouble(coordinatesTable.Rows[rows + 1].Cells[cols].Value) != 0)
                    {
                        delta = Math.Abs(Convert.ToDouble(coordinatesTable.Rows[rows].Cells[cols].Value) - Convert.ToDouble(coordinatesTable.Rows[rows + 1].Cells[cols].Value));
                    }

                    averageDelta += delta;
                    delta = 0;
                }


                // newRowIndex = 9
                averageDelta /= coordinatesTable.Rows.Count;
                // Считаем значение, которое будем прибавлять или вычитать
                newCellValue = random.NextDouble() * (averageDelta - (-averageDelta)) + averageDelta;
                // Обращаемся к восьмой строке, это наша созданная строчка
                
                // Тут считаем с шансом 50% прибавится значение или убавится
                Random randomFiftyPercent = new Random();
                bool option1 = (randomFiftyPercent.NextDouble() < 0.5);

                if (option1)
                {
                    coordinatesTable.Rows[newRowIndex - 1].Cells[cols].Value = Math.Round(Convert.ToDouble(coordinatesTable.Rows[newRowIndex - 2].Cells[cols].Value) + newCellValue, 4);
                }
                else
                {
                    coordinatesTable.Rows[newRowIndex - 1].Cells[cols].Value = Math.Round(Convert.ToDouble(coordinatesTable.Rows[newRowIndex - 2].Cells[cols].Value) - newCellValue, 4);
                }

                AddValuesInNewRowQuery(cols, maxEpochName, Convert.ToDouble(coordinatesTable.Rows[newRowIndex - 1].Cells[cols].Value), tableName);
                averageDelta = 0;
            }
        }

        public void AddValuesInNewRowQuery(int column, int maxEpoch, double value, string tableName)
        {
            string SQLQuery = "UPDATE [" + tableName + "] SET \"" + column + "\" = \"" + value + "\" WHERE Эпоха = \'" + maxEpoch + "\'";
            DoSQLQuery(SQLQuery);
        }
        /// <summary>
        /// Добавление новой строки в БД с помощью INSERT
        /// </summary>
        /// <param name="index"></param>
        public void AddNewRowQuery(double index, string tableName)
        {
            string SQLQuery = "INSERT INTO [" + tableName + "] (Эпоха) VALUES (\"" + index + "\")";
            DoSQLQuery(SQLQuery);
        }

        public void DeleteRowQuery(string index, string tableName)
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



