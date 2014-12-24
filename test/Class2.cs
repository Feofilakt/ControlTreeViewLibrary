using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ControlTreeView;
using System.Drawing;

namespace test
{
    class Class2 : NodeControl
    {
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.TextBox textBox1;

        public Class2(string text)
            : base()
        {
            InitializeComponent();
            textBox1.Text = Name = text;
        } 
    
        private void InitializeComponent()
        {
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(3, 3);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(53, 20);
            this.textBox1.TabIndex = 0;
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(3, 29);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(53, 20);
            this.textBox2.TabIndex = 1;
            // 
            // Class2
            // 
            this.BackColor = System.Drawing.Color.PaleGreen;
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.textBox1);
            this.Name = "Class2";
            this.Size = new System.Drawing.Size(59, 53);
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}
