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
            _textBlock[0] = "Для начала работы в программе\r\nнеобходимо открыть проект.\r\nВ появившемся окне необходимо\r\nвыбрать папку, сгенерированную\r\nв программе VargenMM.\r\nВ этой папке должна находиться\r\nбаза данных .sqlite,\r\nтекстовый файл с информацией,\r\nа также изображение объекта .png.\r\n";
            _textBlock[1] = "На вкладке Первый уровень декомпозиции\r\nпользователь может таблицы фазовых \r\nкоординат, мониторинга состояния объекта,\r\nа также просмотреть графики.";
            _textBlock[2] = "При нажатии по любому из графиков \r\nможно включить режим отображения\r\nна весь экран";
            _textBlock[3] = "Для начала работы на втором\r\nуровне декомпозиции необходимо\r\nраспределить точки по блокам объекта.\r\nТочки можно перемещать между двумя\r\nсписками, нажимая по ним.\r\n";
            _textBlock[4] = "После этого во вкладке Расчеты и графики\r\nможно просмотреть таблицы с вычислениями,\r\nа также графики для выбранного блока.\r\n";
            _textBlock[5] = "Для работы на вкладке \r\nЧетвертый уровень декомпозиции\r\nсначала нужно выполнить распределение\r\nточек на втором уровне.\r\nНа этой вкладке нужно выбрать блок и\r\nможно отметить точки для отображения\r\nна графике\r\n";
            _textBlock[6] = "Изменяя коэффиент эксп. сглаживания\r\nили погрешность измеререния, а также после \r\nизменения количества строчек программа\r\nзаново проводит все вычисления.\r\nПри изменении погрешности измерений\r\nна вкладке программа сообщает\r\nо том, что статус изменился на Не сохранено.\r\n";
            _textBlock[7] = "Чтобы сохранить новое значение\r\nпогрешности измерений необходимо \r\nнажать Файл --> Сохранить.\r\nТогда новое значение запишется в\r\nтекстовый файл и программа укажет\r\nстатус Сохранено.\r\n";

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
                buttonBack.Enabled = true;
            }

            if (_userLocation < (_headersArray.Count() - 1))
            {
                buttonForward.Enabled = true;
            }

            if (_userLocation > 0)
            {
                buttonBack.Enabled = true;
            }
            updateForm();
        }

        private void buttonBack_Click(object sender, EventArgs e)
        {
            _userLocation--;
            
            if (_userLocation == 0)
            {
                buttonBack.Enabled=false;
                buttonForward.Enabled=true;
            }

            if (_userLocation < (_headersArray.Count() - 1))
            {
                buttonForward.Enabled = true;
            }
            updateForm();
        }
    }
}
