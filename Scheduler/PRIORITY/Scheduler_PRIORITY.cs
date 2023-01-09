using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Scheduler.PRIORITY
{
    class Scheduler_PRIORITY_Task : Task
    {
        int p;
        [DisplayName("Priority")]
        public int P { get => p; set => p = value; }

        public Scheduler_PRIORITY_Task(int burst_time, int arrival_time, int p) : base(burst_time, arrival_time)
        {
            this.p = p;
        }
    }

    class Scheduler_PRIORITY : Scheduler<Scheduler_PRIORITY_Task>
    {
        List<Scheduler_PRIORITY_Task> wait_list, ready_queue;

        Scheduler_PRIORITY_Task current_task;

        DataGridView datagrid_ready_queue;

        public int current_task_priority;


        /****************************************/

        public Scheduler_PRIORITY(DataGridView datagrid_task_list, DataGridView datagrid_ready_queue, FlowLayoutPanel flow, RichTextBox textbox_output) : base(datagrid_task_list, flow, textbox_output)
        {
            this.datagrid_ready_queue = datagrid_ready_queue;
        }

        /****************************************/

        public override void add_task(Scheduler_PRIORITY_Task task)
        {
            input_task_list.Add(task);

            datagrid_task_list.DataSource = null;
            datagrid_task_list.DataSource = input_task_list.ToList<Scheduler_PRIORITY_Task>();
            datagrid_task_list.Update();
        }

        public override void start()
        {
            current_task = null;

            wait_list = new List<Scheduler_PRIORITY_Task>(input_task_list.OrderBy(task => task.P));

            ready_queue = new List<Scheduler_PRIORITY_Task>();

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
            if (flag || (new_task_arrived))
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
                if (wait_list[i].Arrival_time <= total_elapsed_time_seconds && wait_list[i] == wait_list.ElementAt(0))
                {
                    new_task_arrived = false;

                    //Get task
                    Scheduler_PRIORITY_Task task = wait_list[i];
                    task.Wait_time = 0;

                    ready_queue.Add(task);
                    //Add task to ready queue
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
        }

        /****************************************/

        bool Check_Reschedule_Flag()
        {
            if (current_task == null)
                return true;

            if (current_task.Finished)
            {

                TaskBox<Task> taskBox = new TaskBox<Task>((current_task.Task_name + " Finished") +
                    ("\n Real Runtime:" + current_task.Execution_time / 1000.0) +
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
            Scheduler_PRIORITY_Task next_task;


            next_task = ready_queue.OrderBy(t => t.P).ElementAt(0);

            //Remove the task from ready queue
            ready_queue.Remove(next_task);

            //Initialize next task 
            next_task.First_response_time = (next_task.First_response_time == -1) ? total_elapsed_time_seconds : next_task.First_response_time;

            //If current task is not finished, put it back to ready queue 
            if (current_task != null && !current_task.Finished)
            {
                ready_queue.Add(current_task);
            }


            TaskBox<Task> taskBox = new TaskBox<Task>(next_task.Task_name + " Scheduled"
                + ("\nT=" + Math.Round((total_elapsed_time / 1000.0), 2).ToString())); flow.Controls.Add(taskBox);

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
            datagrid_task_list.DataSource = input_task_list.ToList<Scheduler_PRIORITY_Task>();
            datagrid_task_list.Update();

            if (current_task != null)
            {
                TaskBox<Task> taskBox = new TaskBox<Task>((current_task.Task_name) +
                    ("\nRuntime:" + current_task.Execution_time / 1000) +
                    ("\nT=" + total_elapsed_time_seconds)); flow.Controls.Add(taskBox);
            }
            else
            {
                TaskBox<Task> taskBox = new TaskBox<Task>("\nT=" + total_elapsed_time_seconds);
                flow.Controls.Add(taskBox);
            }
        }
    }
}