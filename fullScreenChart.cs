using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using ChartControl = System.Windows.Forms.DataVisualization.Charting.Chart;

namespace CourseWorkFinal
{

    public partial class fullScreenChart : Form
    {
        // Почему-то не удается получить размер через свойства, использую для этого поля
        private int _formHeight = 1547;
        private int _formWidth = 1012;

        public fullScreenChart(ChartControl chartFromAnotherForm)
        {
            chartFromAnotherForm.Dock = DockStyle.Fill;
            this.Controls.Add(chartFromAnotherForm);
            InitializeComponent();
        }
    }
}
