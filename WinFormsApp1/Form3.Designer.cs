namespace WinFormsApp1
{
    partial class Form3
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
            dateTimePicker1 = new DateTimePicker();
            listView1 = new ListView();
            monthCalendar1 = new MonthCalendar();
            progressBar1 = new ProgressBar();
            comboBox1 = new ComboBox();
            SuspendLayout();
            // 
            // dateTimePicker1
            // 
            dateTimePicker1.Location = new Point(28, 26);
            dateTimePicker1.Name = "dateTimePicker1";
            dateTimePicker1.Size = new Size(250, 27);
            dateTimePicker1.TabIndex = 0;
            // 
            // listView1
            // 
            listView1.Location = new Point(28, 242);
            listView1.Name = "listView1";
            listView1.Size = new Size(598, 183);
            listView1.TabIndex = 1;
            listView1.UseCompatibleStateImageBehavior = false;
            // 
            // monthCalendar1
            // 
            monthCalendar1.Location = new Point(385, 26);
            monthCalendar1.Name = "monthCalendar1";
            monthCalendar1.TabIndex = 2;
            // 
            // progressBar1
            // 
            progressBar1.Location = new Point(28, 95);
            progressBar1.Name = "progressBar1";
            progressBar1.Size = new Size(250, 29);
            progressBar1.TabIndex = 3;
            // 
            // comboBox1
            // 
            comboBox1.FormattingEnabled = true;
            comboBox1.Location = new Point(25, 183);
            comboBox1.Name = "comboBox1";
            comboBox1.Size = new Size(253, 28);
            comboBox1.TabIndex = 4;
            // 
            // Form3
            // 
            AutoScaleDimensions = new SizeF(9F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(comboBox1);
            Controls.Add(progressBar1);
            Controls.Add(monthCalendar1);
            Controls.Add(listView1);
            Controls.Add(dateTimePicker1);
            Name = "Form3";
            Text = "Form3";
            ResumeLayout(false);
        }

        #endregion

        private DateTimePicker dateTimePicker1;
        private ListView listView1;
        private MonthCalendar monthCalendar1;
        private ProgressBar progressBar1;
        private ComboBox comboBox1;
    }
}