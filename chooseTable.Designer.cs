namespace CourseWorkFinal
{
    partial class chooseTable
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
            this.label1 = new System.Windows.Forms.Label();
            this.comboBoxChooseTable = new System.Windows.Forms.ComboBox();
            this.buttonAccept = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.label1.Location = new System.Drawing.Point(16, 33);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(425, 40);
            this.label1.TabIndex = 0;
            this.label1.Text = "В базе данных обнаружено несколько таблиц.\r\nНеобходимо выбрать таблицу с Z-коорди" +
    "натами.";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // comboBoxChooseTable
            // 
            this.comboBoxChooseTable.FormattingEnabled = true;
            this.comboBoxChooseTable.Location = new System.Drawing.Point(157, 105);
            this.comboBoxChooseTable.Name = "comboBoxChooseTable";
            this.comboBoxChooseTable.Size = new System.Drawing.Size(121, 24);
            this.comboBoxChooseTable.TabIndex = 1;
            this.comboBoxChooseTable.SelectedIndexChanged += new System.EventHandler(this.comboBoxChooseTable_SelectedIndexChanged);
            // 
            // buttonAccept
            // 
            this.buttonAccept.Location = new System.Drawing.Point(130, 149);
            this.buttonAccept.Name = "buttonAccept";
            this.buttonAccept.Size = new System.Drawing.Size(172, 49);
            this.buttonAccept.TabIndex = 2;
            this.buttonAccept.Text = "Подтвердить";
            this.buttonAccept.UseVisualStyleBackColor = true;
            this.buttonAccept.Click += new System.EventHandler(this.buttonAccept_Click);
            // 
            // chooseTable
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(453, 243);
            this.ControlBox = false;
            this.Controls.Add(this.buttonAccept);
            this.Controls.Add(this.comboBoxChooseTable);
            this.Controls.Add(this.label1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "chooseTable";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "chooseTable";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboBoxChooseTable;
        private System.Windows.Forms.Button buttonAccept;
    }
}