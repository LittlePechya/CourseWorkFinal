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
            this.textAboutAuthor = new System.Windows.Forms.Label();
            this.logoPictureBox = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.logoPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // textAboutAuthor
            // 
            this.textAboutAuthor.AutoSize = true;
            this.textAboutAuthor.Location = new System.Drawing.Point(74, 83);
            this.textAboutAuthor.Name = "textAboutAuthor";
            this.textAboutAuthor.Size = new System.Drawing.Size(201, 16);
            this.textAboutAuthor.TabIndex = 0;
            this.textAboutAuthor.Text = "Здесь информация об авторе";
            // 
            // logoPictureBox
            // 
            this.logoPictureBox.Location = new System.Drawing.Point(182, 150);
            this.logoPictureBox.Name = "logoPictureBox";
            this.logoPictureBox.Size = new System.Drawing.Size(100, 50);
            this.logoPictureBox.TabIndex = 1;
            this.logoPictureBox.TabStop = false;
            // 
            // FormAboutAuthorcs
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(490, 290);
            this.Controls.Add(this.logoPictureBox);
            this.Controls.Add(this.textAboutAuthor);
            this.Name = "FormAboutAuthorcs";
            this.Text = "FormAboutAuthorcs";
            ((System.ComponentModel.ISupportInitialize)(this.logoPictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label textAboutAuthor;
        private System.Windows.Forms.PictureBox logoPictureBox;
    }
}