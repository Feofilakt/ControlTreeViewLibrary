using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ControlTreeView;

namespace test
{
    class Class4 : NodeControl
    {
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.Label label1;

        public Class4(string text)
        {
            InitializeComponent();
            this.label1.Text = text;
            this.label1.MouseDown += new MouseEventHandler((sender, e) => { OnMouseDown(e); });
        }
    
        private void InitializeComponent()
        {
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(0, 0);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(15, 14);
            this.checkBox1.TabIndex = 0;
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "label1";
            // 
            // Class4
            // 
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.label1);
            this.Name = "Class4";
            this.Size = new System.Drawing.Size(75, 13);
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}
