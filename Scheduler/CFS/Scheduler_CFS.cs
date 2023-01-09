using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Scheduler.CFS
{
    class Scheduler_CFS_Task : Task
    {
        int nice;

        double v_runtime, cpu_time_slice;
        
        public double current_time_slice_counter;

        /****************************************/

        public int Nice { get => nice; set => nice = value; }

        [DisplayName("Virtual Runtime")]
        public double V_runtime { get => v_runtime; set => v_runtime = value; }

        [DisplayName("CPU Time Slice")]
        public double CPU_time_slice { get => cpu_time_slice; set => cpu_time_slice = value; }

        public double Weight { get => 1024.0 / Math.Pow(1.25, nice); }

        /****************************************/
        public Scheduler_CFS_Task(int burst_time, int arrival_time, int nice) : base(burst_time, arrival_time)
        {
            this.nice = nice;
            this.v_runtime = -1;
            this.cpu_time_slice = -1;
        }

        /****************************************/
    }

    class Scheduler_CFS : Scheduler<Scheduler_CFS_Task>
    {
        /****************************************/

        DataGridView datagrid_rb_tree;

        NumericUpDown numeric_updown_period;

        List<Scheduler_CFS_Task> wait_list, ready_queue;

        Scheduler_CFS_Task current_task;

        double total_weight;

        public int period;

        /****************************************/

        public Scheduler_CFS(DataGridView datagrid_task_list, DataGridView datagrid_rb_tree, 
            RichTextBox textbox_output, FlowLayoutPanel flow, NumericUpDown numeric_updown_period, int period) : base(datagrid_task_list, flow, textbox_output)
        {
            this.numeric_updown_period = numeric_updown_period;

            this.datagrid_rb_tree = datagrid_rb_tree;

            this.period = period;
        }

        /****************************************/

        public override void add_task(Scheduler_CFS_Task task)
        {
            input_task_list.Add(task);

            datagrid_task_list.DataSource = null;
            datagrid_task_list.DataSource = input_task_list.ToList<Scheduler_CFS_Task>();
            datagrid_task_list.Update();
        }

        public override void start()
        {
            current_task = null;

            wait_list = new List<Scheduler_CFS_Task>(input_task_list);

            ready_queue = new List<Scheduler_CFS_Task>();

            timer.Interval = 10;

            update();

            timer.Start();
        }

        public override void update()
        {
           
            // Check for new arrived tasks and insert them to ready queue
            Check_For_New_Tasks();

            //Calculate cpu run time slices
            Update_Time_Slices();

            //Update current task
            Update_Current_Task();

            //Check schedule conditions
            bool flag = Check_Reschedule_Flag();

            //If flag is set, reschdule
            if (flag)
            {
                Reschedule();
            }                  

            if(total_elapsed_time % 1000 == 0)
            {
                Update_Time_Chart();
            }              
        }

        /****************************************/

        void Check_For_New_Tasks()
        {
            //For each task in wait list
            for (int i = 0; i < wait_list.Count; i++)
            {
                //If elapsed time has passed arrival time
                if (wait_list[i].Arrival_time <= total_elapsed_time_seconds)
                {
                    //Get task
                    Scheduler_CFS_Task task = wait_list[i];
                    task.Wait_time = 0;
                    task.V_runtime = (ready_queue.Count > 0) ? ready_queue[0].V_runtime : 0; //Set tasks v_runtime to minimum v_run time in the red black tree
                    ready_queue.Add(task); //Add task to ready queue

                    total_weight += task.Weight; //Update total weight

                    wait_list.RemoveAt(i--);
                }
            }

            ready_queue = ready_queue.OrderBy(t => t.V_runtime).ToList();
        }

        /****************************************/

        double calculate_time_slice(int period, double task_weight)
        {
            double slice = (task_weight * period) / total_weight; // slice = t_weight * period / total weight

            slice = slice / 1000.0; //Convert ms to seconds
                                  
            return Math.Round(slice, 2); //Round to decimal point
        }

        private void Update_Time_Slices()
        {
            int num_runable_tasks = ready_queue.Count + (current_task == null ? 0 : 1);

            int cur_period = num_runable_tasks < period / 1000 ? period : 1000 * num_runable_tasks;

            numeric_updown_period.Value = cur_period;

            if (current_task != null)
            {
                current_task.CPU_time_slice = calculate_time_slice(cur_period, current_task.Weight);
            }

            foreach (Scheduler_CFS_Task task in ready_queue)
            {
                task.CPU_time_slice = calculate_time_slice(cur_period, task.Weight);
            }
        }

        /****************************************/

        private void Update_Current_Task()
        {
            if (current_task == null)
                return;

            current_task.current_time_slice_counter += 10;

            current_task.Execution_time += 10;

            current_task.V_runtime += 10.24 / current_task.Weight;

            current_task.V_runtime = Math.Round(current_task.V_runtime, 3);

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
                    ("\n V Runtime:" + current_task.V_runtime) +
                    ("\n Real Runtime:" + current_task.Execution_time / 1000.0) +
                    ("\nT=" + total_elapsed_time_seconds)); flow.Controls.Add(taskBox);

                total_weight -= current_task.Weight;

                current_task.Completion_time = total_elapsed_time_seconds;
                current_task.Turn_around_time = current_task.Completion_time - current_task.Arrival_time;
                current_task.Wait_time = current_task.Turn_around_time - current_task.Burst_time;

                current_task = null;

                return true;
            }

            if (current_task.current_time_slice_counter >= current_task.CPU_time_slice * 1000)
            {
                TaskBox<Task> taskBox = new TaskBox<Task>((current_task.Task_name) + " Time Slice Finished" +
                    ("\n V Runtime:" + current_task.V_runtime) +
                    ("\n Current Time Slice:" + current_task.current_time_slice_counter / 1000.0) +
                    ("\n CPU Slice:" + current_task.CPU_time_slice) +
                    ("\n Real Runtime:" + current_task.Execution_time / 1000.0) +
                    ("\nT=" + total_elapsed_time_seconds)); flow.Controls.Add(taskBox);

                current_task.current_time_slice_counter = 0;

                return true;
            }

            return false;
        }

        /****************************************/

        void Reschedule()
        {
            if (ready_queue.Count == 0)
                return;

            //Get task with minimum v_run_time
            Scheduler_CFS_Task next_task = ready_queue.OrderBy(t => t.V_runtime).ElementAt(0);

            //Remove the task from ready queue
            ready_queue.Remove(next_task);

            //Initialize next task for new cpu time 
            next_task.current_time_slice_counter = 0;
            next_task.First_response_time = (next_task.First_response_time == -1) ? total_elapsed_time_seconds : next_task.First_response_time;

            //If current task is not finished, put it back to ready queue and sort it (behaves like red-black tree)
            if(current_task != null && !current_task.Finished)
            {
                ready_queue.Add(current_task);
                ready_queue = ready_queue.OrderBy(t => t.V_runtime).ToList();
            }

            TaskBox<Task> taskBox = new TaskBox<Task>(next_task.Task_name + " Scheduled" 
                + ("\nT=" + Math.Round((total_elapsed_time / 1000.0), 2).ToString())); flow.Controls.Add(taskBox);

            //Update current task
            current_task = next_task;
        }

        /****************************************/

        private void Update_Time_Chart()
        {

            ready_queue = ready_queue.OrderBy(t => t.V_runtime).ToList();

            datagrid_rb_tree.DataSource = null;
            datagrid_rb_tree.DataSource = ready_queue;
            datagrid_rb_tree.Update();
            
            datagrid_task_list.DataSource = null;
            datagrid_task_list.DataSource = input_task_list.ToList<Scheduler_CFS_Task>();
            datagrid_task_list.Update();

            if (current_task != null)
            {
                TaskBox<Task> taskBox = new TaskBox<Task>((current_task.Task_name) +
                    ("\n V Runtime:" + current_task.V_runtime) +
                    ("\n Current Time Slice:" + current_task.current_time_slice_counter / 1000.0) +
                    ("\n CPU Slice:" + current_task.CPU_time_slice) +
                    ("\n Real Runtime:" + current_task.Execution_time / 1000) +
                    ("\nT=" + total_elapsed_time_seconds)); flow.Controls.Add(taskBox);
            }
            else
            {
                TaskBox<Task> taskBox = new TaskBox<Task>("\nT=" + Math.Round((total_elapsed_time / 1000.0), 2).ToString());
                flow.Controls.Add(taskBox);
            }
        }

        /****************************************/
    }
}
