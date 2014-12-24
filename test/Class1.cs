using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using ControlTreeView;

namespace test
{
    class Class1 : NodeControl
    {
        private Button button1;
        private CheckBox checkBox1;

        public Class1(string text)
            : base()
        {
            InitializeComponent();
            button1.Text = Name = text;
        }
    
        private void InitializeComponent()
        {
            this.button1 = new System.Windows.Forms.Button();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(3, 3);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 0;
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(84, 8);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(15, 14);
            this.checkBox1.TabIndex = 1;
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // Class1
            // 
            this.BackColor = System.Drawing.Color.PaleTurquoise;
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.button1);
            this.Name = "Class1";
            this.Size = new System.Drawing.Size(102, 30);
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.Class1_MouseClick);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked) BackColor = Color.PaleGreen;
            else BackColor = Color.PaleTurquoise;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Button_Click!");
        }

        private void Class1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {

            }
        }
    }
}
