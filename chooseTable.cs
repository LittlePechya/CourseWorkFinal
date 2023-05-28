using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CourseWorkFinal
{
    public partial class chooseTable : Form
    {
        // Свойство для имени таблицы
        public string SelectedTableName { get; set; }
        public chooseTable(List<string> tableNames)
        {
            InitializeComponent();
            buttonAccept.Enabled = false;
            comboBoxChooseTable.Items.AddRange(tableNames.ToArray());
        }

        private void buttonAccept_Click(object sender, EventArgs e)
        {
            SelectedTableName = comboBoxChooseTable.SelectedItem.ToString();
            this.Close();
        }

        private void comboBoxChooseTable_SelectedIndexChanged(object sender, EventArgs e)
        {
            buttonAccept.Enabled = true;
        }
    }
}
