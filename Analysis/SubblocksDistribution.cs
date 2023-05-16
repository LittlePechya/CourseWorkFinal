using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CourseWorkFinal.Analysis
{
    internal class SubblocksDistribution
    {
        // Необходимые компоненты формы для реализации класса
        ListBox allMarksListBox;
        ListBox marksOnBlockListBox;
        DataGridView dataGridViewZCoordinates;

        private Dictionary<Int32, String> blocksName;
        private int blockCount;
        private int needMarksCount;
        private int marksCount;
        private int currentBlock = 0;

        // 
        public SubblocksDistribution()
        {
            // Заполнение словаря для наименования подблоков
            blocksName = new Dictionary<Int32, String>();
            blocksName.Add(0, "A");
            blocksName.Add(1, "Б");
            blocksName.Add(2, "В");
            blocksName.Add(3, "Г");
            blocksName.Add(4, "Д");
            blocksName.Add(5, "E");
            blocksName.Add(6, "Ё");
        }

        /// <summary>
        /// Заполнение листбокса со всеми точками
        /// </summary>
        public void fillAllMarksList()
        {
            for (int i = 1; i < dataGridViewZCoordinates.ColumnCount; i++)
            {
                allMarksListBox.Items.Add(dataGridViewZCoordinates.Columns[i].Name);
            }
        }

        /// <summary>
        /// Метод для расчета количества точек на каждом подблоке (оно должно быть равным)
        /// </summary>
        public void countMarksOnOneBlock()
        {
            int requiredMarksCount = marksCount;
            while (requiredMarksCount % blockCount != 0)
            {
                requiredMarksCount--;
            }
            needMarksCount = requiredMarksCount / blockCount;
        }

        /// <summary>
        /// Метод для проверки количества точек на одном подблоке
        /// </summary>
        /// <returns></returns>
        private bool checkRequiredMarksOnOneBlock()
        {
            if (marksOnBlockListBox.Items.Count == needMarksCount)
            {
                return true;
            }
            else return false;
        }
    }
}
