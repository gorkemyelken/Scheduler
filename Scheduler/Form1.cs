
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Scheduler.CFS;
using Scheduler.SRT;
using Scheduler.HRRN;
using Scheduler.RR;
using Scheduler.PRIORITY;
using Scheduler.FCFS;
using Scheduler.SJF;

namespace Scheduler
{
    public partial class Form1 : Form
    {
        Scheduler_CFS scheduler_cfs;
        Scheduler_SRT scheduler_srt;
        Scheduler_SJF scheduler_sjf;
        Scheduler_HRRN scheduler_hrrn;
        Scheduler_RR scheduler_rr;
        Scheduler_PRIORITY scheduler_priority;
        Scheduler_FCFS scheduler_fcfs;

        Random random = new Random();

        public Form1()
        {
            InitializeComponent();

            scheduler_cfs = new Scheduler_CFS(cfs_task_info_grid, cfs_rb_tree_grid, cfs_txt_output, flow, cfs_nmbr_period, (int)cfs_nmbr_period.Value);
            scheduler_srt = new Scheduler_SRT(srt_task_info_grid, srt_ready_queue_grid, flow, srt_txt_output);
            scheduler_sjf = new Scheduler_SJF(sjf_task_info_grid, sjf_ready_queue_grid, flow, sjf_txt_output);
            scheduler_hrrn = new Scheduler_HRRN(hrrn_task_info_grid, hrrn_ready_queue_grid, flow, hrrn_txt_output);
            scheduler_rr = new Scheduler_RR(rr_task_info_grid, rr_ready_queue_grid, flow, rr_txt_output);
            scheduler_priority = new Scheduler_PRIORITY(priority_task_info_grid, priority_ready_queue_grid, flow, priority_txt_output);
            scheduler_fcfs = new Scheduler_FCFS(fcfs_task_info_grid, fcfs_ready_queue_grid, flow, fcfs_txt_output);

            /*scheduler_rr.add_task(new Task(24, 0));
            scheduler_rr.add_task(new Task(3, 0));
            scheduler_rr.add_task(new Task(3, 0));*/

            /*scheduler_hrrn.add_task(new Scheduler_HRRN_Task(3, 0));
            scheduler_hrrn.add_task(new Scheduler_HRRN_Task(5, 2));
            scheduler_hrrn.add_task(new Scheduler_HRRN_Task(4, 4));
            scheduler_hrrn.add_task(new Scheduler_HRRN_Task(1, 6));
            scheduler_hrrn.add_task(new Scheduler_HRRN_Task(2, 8));*/

            /*scheduler_srt.add_task(new Task(6, 2));
            scheduler_srt.add_task(new Task(2, 5));
            scheduler_srt.add_task(new Task(8, 1));
            scheduler_srt.add_task(new Task(3, 0));
            scheduler_srt.add_task(new Task(4, 4));*/

            /*scheduler_cfs.add_task(new Scheduler_CFS_Task(3,1,0));
            scheduler_cfs.add_task(new Scheduler_CFS_Task(4,2,-2));
            scheduler_cfs.add_task(new Scheduler_CFS_Task(3,2,2));*/
        }

        /****************************************************************************************************************/

        private void cfs_btn_add_task_Click(object sender, EventArgs e)
        {
            Scheduler_CFS_Task task = new Scheduler_CFS_Task((int)cfs_nmbr_burst_time.Value, (int)cfs_nmbr_arr_time.Value, (int)cfs_nmbr_nice.Value); 

            scheduler_cfs.add_task(task);
        }

        private void cfs_btn_add_task_random_Click(object sender, EventArgs e)
        {
            Scheduler_CFS_Task task = new Scheduler_CFS_Task(random.Next(1, 10), random.Next(0, 10), random.Next(-19, 21));
            scheduler_cfs.add_task(task);
        }

        private void cfs_btn_run_Click(object sender, EventArgs e)
        {
            flow.Controls.Clear();

            scheduler_cfs.start();
        }

        private void cfs_txt_output_TextChanged(object sender, EventArgs e)
        {
            // set the current caret position to the end
            cfs_txt_output.SelectionStart = cfs_txt_output.Text.Length;
            // scroll it automatically
            cfs_txt_output.ScrollToCaret();
        }

        private void cfs_nmbr_period_ValueChanged(object sender, EventArgs e)
        {
            this.scheduler_cfs.period = (int)cfs_nmbr_period.Value;
        }

        /****************************************************************************************************************/

        private void flow_ControlAdded(object sender, ControlEventArgs e)
        {
            flow.HorizontalScroll.Value = flow.HorizontalScroll.Maximum;
        }

        /****************************************************************************************************************/
        private void srt_txt_output_TextChanged(object sender, EventArgs e)
        {
            // set the current caret position to the end
            srt_txt_output.SelectionStart = srt_txt_output.Text.Length;
            // scroll it automatically
            srt_txt_output.ScrollToCaret();
        }

        private void srt_btn_run_Click(object sender, EventArgs e)
        {
            flow.Controls.Clear();

            scheduler_srt.start();
        }

        private void srt_btn_add_task_random_Click(object sender, EventArgs e)
        {
            Task task = new Task(random.Next(1, 10), random.Next(0, 10));
            scheduler_srt.add_task(task);
        }

        private void srt_btn_add_task_Click(object sender, EventArgs e)
        {
            Task task = new Task((int)srt_nmbr_burst_time.Value, (int)srt_nmbr_arr_time.Value);
            scheduler_srt.add_task(task);
        }

        private void srt_checkbox_preemptive_CheckedChanged(object sender, EventArgs e)
        {
            scheduler_srt.preemptive = srt_checkbox_preemptive.Checked;
        }

        /****************************************************************************************************************/

        private void hrrn_txt_output_TextChanged(object sender, EventArgs e)
        {
            // set the current caret position to the end
            hrrn_txt_output.SelectionStart = hrrn_txt_output.Text.Length;
            // scroll it automatically
            hrrn_txt_output.ScrollToCaret();
        }

        private void hrrn_btn_run_Click(object sender, EventArgs e)
        {
            flow.Controls.Clear();

            scheduler_hrrn.start();
        }

        private void hrrn_btn_add_task_Click(object sender, EventArgs e)
        {
            Scheduler_HRRN_Task task = new Scheduler_HRRN_Task((int)hrrn_nmbr_burst_time.Value, (int)hrrn_nmbr_arr_time.Value);
            scheduler_hrrn.add_task(task);
        }

        private void hrrn_btn_add_task_random_Click(object sender, EventArgs e)
        {
            Scheduler_HRRN_Task task = new Scheduler_HRRN_Task(random.Next(1, 10), random.Next(0, 10));
            scheduler_hrrn.add_task(task);
        }

        private void hrrn_checkbox_preemptive_CheckedChanged(object sender, EventArgs e)
        {
            scheduler_hrrn.preemptive = hrrn_checkbox_preemptive.Checked;
        }

        /****************************************************************************************************************/

        private void rr_txt_output_TextChanged(object sender, EventArgs e)
        {
            // set the current caret position to the end
            rr_txt_output.SelectionStart = rr_txt_output.Text.Length;
            // scroll it automatically
            rr_txt_output.ScrollToCaret();
        }

        private void rr_btn_add_task_Click(object sender, EventArgs e)
        {
            Task task = new Task((int)rr_nmbr_burst_time.Value, (int)rr_nmbr_arr_time.Value);
            scheduler_rr.add_task(task);
        }

        private void rr_btn_add_task_random_Click(object sender, EventArgs e)
        {
            Task task = new Task(random.Next(1, 10), random.Next(0, 10));
            scheduler_rr.add_task(task);
        }

        private void rr_btn_run_Click(object sender, EventArgs e)
        {
            flow.Controls.Clear();

            scheduler_rr.start();
        }

        private void rr_nmbr_quantum_ValueChanged(object sender, EventArgs e)
        {
            scheduler_rr.quantum = (int)rr_nmbr_quantum.Value;
        }

        /****************************************************************************************************************/

        private void priority_btn_add_task_Click(object sender, EventArgs e)
        {
            Scheduler_PRIORITY_Task task = new Scheduler_PRIORITY_Task((int)priority_nmbr_burst_time.Value, (int)priority_nmbr_arr_time.Value, (int)priority_nmbr_p.Value);

            scheduler_priority.add_task(task);
        }

        private void priority_btn_add_task_random_Click(object sender, EventArgs e)
        {
            Scheduler_PRIORITY_Task task = new Scheduler_PRIORITY_Task(random.Next(1, 10), random.Next(0, 10), random.Next(1, 10));
            scheduler_priority.add_task(task);
        }

        private void priprity_btn_run_Click(object sender, EventArgs e)
        {
            flow.Controls.Clear();

            scheduler_priority.start();
        }

        private void priority_txt_output_TextChanged(object sender, EventArgs e)
        {
            // set the current caret position to the end
            priority_txt_output.SelectionStart = priority_txt_output.Text.Length;
            // scroll it automatically
            priority_txt_output.ScrollToCaret();
        }

        /****************************************************************************************************************/

        private void fcfs_btn_run_Click(object sender, EventArgs e)
        {
            flow.Controls.Clear();

            scheduler_fcfs.start();
        }

        private void fcfs_btn_add_task_Click(object sender, EventArgs e)
        {
            Task task = new Task((int)fcfs_nmbr_burst_time.Value, (int)fcfs_nmbr_arr_time.Value);
            scheduler_fcfs.add_task(task);
        }

        private void fcfs_btn_add_task_random_Click(object sender, EventArgs e)
        {
            Task task = new Task(random.Next(1, 10), random.Next(0, 10));
            scheduler_fcfs.add_task(task);
        }

        private void fcfs_txt_output_TextChanged(object sender, EventArgs e)
        {
            fcfs_txt_output.SelectionStart = fcfs_txt_output.Text.Length;
            // scroll it automatically
            fcfs_txt_output.ScrollToCaret();
        }

        /****************************************************************************************************************/
        private void sjf_btn_add_task_Click(object sender, EventArgs e)
        {
            Task task = new Task((int)srt_nmbr_burst_time.Value, (int)srt_nmbr_arr_time.Value);
            scheduler_sjf.add_task(task);
        }

        private void sjf_btn_add_task_random_Click(object sender, EventArgs e)
        {
            Task task = new Task(random.Next(1, 10), random.Next(0, 10));
            scheduler_sjf.add_task(task);
        }

        private void sjf_btn_run_Click(object sender, EventArgs e)
        {
            flow.Controls.Clear();

            scheduler_sjf.start();
        }

        private void sjf_txt_output_TextChanged(object sender, EventArgs e)
        {
            sjf_txt_output.SelectionStart = sjf_txt_output.Text.Length;
            // scroll it automatically
            sjf_txt_output.ScrollToCaret();
        }
    }
}
