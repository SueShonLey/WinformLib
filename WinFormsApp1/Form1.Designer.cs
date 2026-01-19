namespace WinFormsApp1
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            checkedListBox1 = new CheckedListBox();
            progressBar1 = new ProgressBar();
            SuspendLayout();
            // 
            // checkedListBox1
            // 
            checkedListBox1.FormattingEnabled = true;
            checkedListBox1.Location = new Point(181, 155);
            checkedListBox1.Name = "checkedListBox1";
            checkedListBox1.Size = new Size(230, 164);
            checkedListBox1.TabIndex = 1;
            // 
            // progressBar1
            // 
            progressBar1.Location = new Point(190, 107);
            progressBar1.Name = "progressBar1";
            progressBar1.Size = new Size(474, 29);
            progressBar1.TabIndex = 2;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(700, 370);
            Controls.Add(progressBar1);
            Controls.Add(checkedListBox1);
            Font = new Font("宋体", 9F, FontStyle.Regular, GraphicsUnit.Point);
            Margin = new Padding(3, 2, 3, 2);
            Name = "Form1";
            Text = "Form100";
            ResumeLayout(false);
        }

        #endregion

        private CheckedListBox checkedListBox1;
        private ProgressBar progressBar1;
    }
}
