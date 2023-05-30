namespace CourseWorkFinal
{
    partial class FormAboutAuthor
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormAboutAuthor));
            this.textAboutAuthor = new System.Windows.Forms.Label();
            this.logoPictureBox = new System.Windows.Forms.PictureBox();
            this.header1 = new System.Windows.Forms.Label();
            this.header2 = new System.Windows.Forms.Label();
            this.textAboutProgram = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.logoPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // textAboutAuthor
            // 
            this.textAboutAuthor.AutoSize = true;
            this.textAboutAuthor.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.textAboutAuthor.Location = new System.Drawing.Point(10, 59);
            this.textAboutAuthor.Name = "textAboutAuthor";
            this.textAboutAuthor.Size = new System.Drawing.Size(265, 58);
            this.textAboutAuthor.TabIndex = 0;
            this.textAboutAuthor.Text = "Припоров Владислав, \r\nстудент группы БИ-21";
            // 
            // logoPictureBox
            // 
            this.logoPictureBox.Image = ((System.Drawing.Image)(resources.GetObject("logoPictureBox.Image")));
            this.logoPictureBox.Location = new System.Drawing.Point(363, 6);
            this.logoPictureBox.Name = "logoPictureBox";
            this.logoPictureBox.Size = new System.Drawing.Size(115, 111);
            this.logoPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.logoPictureBox.TabIndex = 1;
            this.logoPictureBox.TabStop = false;
            // 
            // header1
            // 
            this.header1.AutoSize = true;
            this.header1.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.header1.Location = new System.Drawing.Point(10, 18);
            this.header1.Name = "header1";
            this.header1.Size = new System.Drawing.Size(178, 29);
            this.header1.TabIndex = 2;
            this.header1.Text = "Разработчик:";
            // 
            // header2
            // 
            this.header2.AutoSize = true;
            this.header2.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.header2.Location = new System.Drawing.Point(12, 130);
            this.header2.Name = "header2";
            this.header2.Size = new System.Drawing.Size(189, 29);
            this.header2.TabIndex = 3;
            this.header2.Text = "О программе:";
            // 
            // textAboutProgram
            // 
            this.textAboutProgram.AutoSize = true;
            this.textAboutProgram.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.textAboutProgram.Location = new System.Drawing.Point(12, 174);
            this.textAboutProgram.Name = "textAboutProgram";
            this.textAboutProgram.Size = new System.Drawing.Size(414, 116);
            this.textAboutProgram.TabIndex = 4;
            this.textAboutProgram.Text = "Программа разработана в рамках \r\nкурсовой работы по дисциплине \r\n\"Моделирование с" +
    "истем\"\r\n\r\n";
            // 
            // FormAboutAuthor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(490, 290);
            this.Controls.Add(this.textAboutProgram);
            this.Controls.Add(this.header2);
            this.Controls.Add(this.header1);
            this.Controls.Add(this.logoPictureBox);
            this.Controls.Add(this.textAboutAuthor);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormAboutAuthor";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Об авторе";
            ((System.ComponentModel.ISupportInitialize)(this.logoPictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label textAboutAuthor;
        private System.Windows.Forms.PictureBox logoPictureBox;
        private System.Windows.Forms.Label header1;
        private System.Windows.Forms.Label header2;
        private System.Windows.Forms.Label textAboutProgram;
    }
}