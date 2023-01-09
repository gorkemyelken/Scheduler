using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Scheduler
{
    abstract class Scheduler<T>
    {
        protected int total_elapsed_time;

        protected double total_elapsed_time_seconds { get => Math.Round(total_elapsed_time / 1000.0F, 2);}

        protected Timer timer;

        protected FlowLayoutPanel flow;

        protected DataGridView datagrid_task_list;

        protected RichTextBox textbox_output;

        protected List<T> input_task_list;

        protected Scheduler(DataGridView datagrid_task_list, FlowLayoutPanel flow, RichTextBox textbox_output)
        {
            this.input_task_list = new List<T>();
            this.timer = new System.Windows.Forms.Timer();
            this.timer.Interval = 1000;
            this.timer.Tick += Timer_Tick;
            this.datagrid_task_list = datagrid_task_list;
            this.flow = flow;
            this.textbox_output = textbox_output;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            total_elapsed_time += timer.Interval;

            this.update();

            bool is_finished = true;

            for (int i = 0; i < input_task_list.Count; i++)
            {
                if( (input_task_list[i] as Task).Finished == false )
                {
                    is_finished = false;
                    break;
                }
            }

            if (is_finished)
            {
                timer.Stop();

                double total_wait_time = 0;
                double total_turnaround_time = 0;
                int task_count = 0;

                for (int i = 0; i < input_task_list.Count; i++)
                {
                    Task t = input_task_list[i] as Task;

                    if (t.Finished == true)
                    {
                        total_wait_time += t.Wait_time;
                        total_turnaround_time += t.Turn_around_time;
                        task_count++;
                    }
                }

                textbox_output.Text += "\nTotal Wait Time:" + Math.Round(total_wait_time, 2) + "\n";
                textbox_output.Text += "\nTotal Turn Around Time:" + Math.Round(total_turnaround_time, 2) + "\n";
                textbox_output.Text += "\nAvg Wait Time:" + Math.Round(total_wait_time / task_count,2) + "\n";
                textbox_output.Text += "\nAvg Turn Around Time:" + Math.Round(total_turnaround_time / task_count, 2) + "\n";
            }
                             
        }

        public abstract void add_task(T task);

        public abstract void start();

        public abstract void update();
    }

}
