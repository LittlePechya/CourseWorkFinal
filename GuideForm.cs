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
    public partial class GuideForm : Form
    {
        private string[] _headersArray = new string[8] { "Шаг 1", "Шаг 2", "Шаг 3", "Шаг 4", "Шаг 5", "Шаг 6", "Шаг 7", "Шаг 8"};
        private string[] _textBlock = new string[8];
        private int _userLocation = 0;
        List<Image> images = new List<Image>();
        public GuideForm()
        {
            InitializeComponent();
            setText();
            setImages();
            // Изначально кнопка "Назад" отключена, также она отключена, если _userLocation = 0
            buttonBack.Enabled = false;
        }

        public void updateForm()
        {
            header.Text = _headersArray[_userLocation];
            infoText.Text = _textBlock[_userLocation];
            pictureBoxGuide.Image = images[_userLocation];
        }

        public void setText()
        {
            _textBlock[0] = "Для начала работы в программе необходимо \r\nнажать на кнопку \"Открыть проект\".\r\nДалее в открывшемся окне необходимо \r\nвыбрать папку с вариантом, \r\nкоторая была сгенерирована в \r\nпрограмме VargenMM.\r\nПапка должна содержать картинку .png, \r\nтекстовый файл с информацией \r\nи файл базы данных \r\n.sqlite.\r\n\r\n";
        }

        public void setImages()
        {
            images.Add(Properties.Resources.Guide0);
            images.Add(Properties.Resources.Guide1);
            images.Add(Properties.Resources.Guide2);
            images.Add(Properties.Resources.Guide3);
            images.Add(Properties.Resources.Guide4);
            images.Add(Properties.Resources.Guide5);
            images.Add(Properties.Resources.Guide6);
            images.Add(Properties.Resources.Guide7);
        }

        private void buttonForward_Click(object sender, EventArgs e)
        {
            _userLocation++;
            if (_userLocation == _headersArray.Count() - 1) 
            { 
                buttonForward.Enabled = false; 
            }

            if (_userLocation > 0)
            {
                buttonBack.Enabled = true;
            }
            updateForm();
        }

        private void buttonBack_Click(object sender, EventArgs e)
        {

        }
    }
}
