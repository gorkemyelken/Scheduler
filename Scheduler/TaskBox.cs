using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Scheduler
{
    class TaskBox<T> : Panel
    {
        T task;

        private Label label;

        public TaskBox(string txt)
        {
            Random rnd = new Random();

            //
            this.BorderStyle = BorderStyle.FixedSingle;
            this.Margin = new Padding(10);

            this.label = new Label();
            this.Label_text = txt;
            this.label.AutoSize = false;
            this.label.TextAlign = ContentAlignment.MiddleCenter;
            this.label.Size = new Size(225, 225);
            this.label.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            this.label.Location = new Point((this.Width - this.label.Width) / 2, (this.Height - this.label.Height) / 2);
            this.label.Font = new Font(this.label.Font.FontFamily, 15);
            this.Controls.Add(this.label);
        }

        public TaskBox(T task)
        {
            this.task = task;

            Random rnd = new Random();

            //
            this.Size = new System.Drawing.Size(100, 100);
            this.BackColor = Color.FromArgb(rnd.Next(256), rnd.Next(256), rnd.Next(256));
            this.BorderStyle = BorderStyle.FixedSingle;

            this.label = new Label();
            this.label.AutoSize = false;
            this.label.TextAlign = ContentAlignment.MiddleCenter;
            this.label.BorderStyle = BorderStyle.FixedSingle;
            this.label.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            this.label.Font = new Font(this.label.Font.FontFamily, 48F);
            this.label.Location = new Point((this.Width - this.label.Width) / 2, (this.Height - this.label.Height) / 2);

            this.Controls.Add(this.label);
        }

        public string Label_text { get => this.label.Text; set => this.label.Text = value; }
    }
}
