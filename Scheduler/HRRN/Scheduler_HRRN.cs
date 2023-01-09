using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Scheduler.HRRN
{
    class Scheduler_HRRN_Task : Task
    {
        double hrrn_ratio;

        /****************************************/

        public Scheduler_HRRN_Task(int burst_time, int arrival_time) : base(burst_time, arrival_time)
        {
            Hrrn_ratio = 0;
        }

        /****************************************/

        [DisplayName("HRRN Ratio")]
        public double Hrrn_ratio { get => hrrn_ratio; set => hrrn_ratio = value; }
    }

    class Scheduler_HRRN : Scheduler<Scheduler_HRRN_Task>
    {
        List<Scheduler_HRRN_Task> wait_list, ready_queue;

        Scheduler_HRRN_Task current_task;

        DataGridView datagrid_ready_queue;

        public bool preemptive;

        /****************************************/

        public Scheduler_HRRN(DataGridView datagrid_task_list, DataGridView datagrid_ready_queue, FlowLayoutPanel flow, RichTextBox textbox_output) : base(datagrid_task_list, flow, textbox_output)
        {
            this.datagrid_ready_queue = datagrid_ready_queue;

            this.preemptive = true;
        }

        /****************************************/

        public override void add_task(Scheduler_HRRN_Task task)
        {
            input_task_list.Add(task);

            datagrid_task_list.DataSource = null;
            datagrid_task_list.DataSource = input_task_list.ToList<Task>();
            datagrid_task_list.Update();
        }

        public override void start()
        {
            current_task = null;

            wait_list = new List<Scheduler_HRRN_Task>(input_task_list);

            foreach (Task t in wait_list)
            {
                t.Wait_time = 0;
            }

            ready_queue = new List<Scheduler_HRRN_Task>();

            timer.Interval = 1000;

            update();

            timer.Start();
        }

        public override void update()
        {

            // Check for new arrived tasks and insert them to ready queue
            bool new_task_arrived = Check_For_New_Tasks();

            //Update current task
            Update_Current_Task();

            Update_Time_Chart();

            bool flag = Check_Reschedule_Flag();

            //If flag is set, reschdule
            if (flag || (preemptive && new_task_arrived))
            {
                Reschedule();

                Update_Time_Chart();
            }
        }

        /****************************************/

        bool Check_For_New_Tasks()
        {
            bool new_task_arrived = false;

            //For each task in wait list
            for (int i = 0; i < wait_list.Count; i++)
            {
                //If elapsed time has passed arrival time
                if (wait_list[i].Arrival_time <= total_elapsed_time_seconds)
                {
                    new_task_arrived = true;

                    //Get task
                    Scheduler_HRRN_Task task = wait_list[i];
                    task.Wait_time = 0;

                    ready_queue.Add(task); //Add task to ready queue
                    wait_list.RemoveAt(i--);
                }
            }

            return new_task_arrived;
        }

        /****************************************/

        void Update_Current_Task()
        {
            if (current_task == null)
                return;

            current_task.Execution_time += 1000;

            if (current_task.Execution_time >= current_task.Burst_time * 1000)
                current_task.Finished = true;

            foreach (Scheduler_HRRN_Task t in ready_queue)
            {
                if(total_elapsed_time_seconds != t.Arrival_time)
                    t.Wait_time += 1;
                
                if(preemptive)
                    t.Hrrn_ratio = (t.Wait_time / (t.Burst_time - (t.Execution_time / 1000)) ) + 1;
                else
                    t.Hrrn_ratio = (t.Wait_time / t.Burst_time) + 1;

                t.Hrrn_ratio = Math.Round(t.Hrrn_ratio, 3);
            }
        }

        /****************************************/

        bool Check_Reschedule_Flag()
        {
            if (current_task == null)
                return true;

            if (current_task.Finished)
            {

                TaskBox<Scheduler_HRRN_Task> taskBox = new TaskBox<Scheduler_HRRN_Task>((current_task.Task_name + " Finished") +
                    ("\n Real Runtime:" + current_task.Execution_time / 1000.0) +
                    ("\nHRRN Ratio:" + current_task.Hrrn_ratio) +
                    ("\nT=" + total_elapsed_time_seconds)); flow.Controls.Add(taskBox);

                current_task.Completion_time = total_elapsed_time_seconds;
                current_task.Turn_around_time = current_task.Completion_time - current_task.Arrival_time;
                current_task.Wait_time = current_task.Turn_around_time - current_task.Burst_time;

                current_task = null;

                return true;
            }

            return false;
        }

        /****************************************/

        void Reschedule()
        {
            if (ready_queue.Count == 0)
                return;

            //Get next task
            Scheduler_HRRN_Task next_task = null;

            if(preemptive == false)
            {
                next_task = ready_queue.OrderByDescending(x => (x.Wait_time / x.Burst_time) + 1).ElementAt(0);
            }             
            else
            {
                next_task = ready_queue.OrderByDescending(x => x.Wait_time / (x.Burst_time - (x.Execution_time / 1000.0)) + 1).ElementAt(0);
            }

            //Remove the task from ready queue
            ready_queue.Remove(next_task);

            //Initialize next task 
            next_task.First_response_time = (next_task.First_response_time == -1) ? total_elapsed_time_seconds : next_task.First_response_time;

            //If current task is not finished, put it back to ready queue 
            if (current_task != null && !current_task.Finished)
            {
                ready_queue.Add(current_task);
            }

            TaskBox<Scheduler_HRRN_Task> taskBox = new TaskBox<Scheduler_HRRN_Task>(next_task.Task_name + " Scheduled"
                + ("\nHRRN Ratio:" + next_task.Hrrn_ratio)
                + ("\nT=" + total_elapsed_time_seconds)); flow.Controls.Add(taskBox);

            //Update current task
            current_task = next_task;
        }

        /****************************************/
        void Update_Time_Chart()
        {

            datagrid_ready_queue.DataSource = null;
            datagrid_ready_queue.DataSource = ready_queue;
            datagrid_ready_queue.Update();

            datagrid_task_list.DataSource = null;
            datagrid_task_list.DataSource = input_task_list.ToList<Task>();
            datagrid_task_list.Update();

            if (current_task != null)
            {
                TaskBox<Scheduler_HRRN_Task> taskBox = new TaskBox<Scheduler_HRRN_Task>((current_task.Task_name) +
                    ("\nRuntime:" + current_task.Execution_time / 1000) +
                    ("\nHRRN Ratio:" + current_task.Hrrn_ratio) +
                    ("\nT=" + total_elapsed_time_seconds)); flow.Controls.Add(taskBox);
            }
            else
            {
                TaskBox<Scheduler_HRRN_Task> taskBox = new TaskBox<Scheduler_HRRN_Task>("\nT=" + total_elapsed_time_seconds);
                flow.Controls.Add(taskBox);
            }
        }
    }
}
