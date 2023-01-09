using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scheduler
{
    class Task
    {

        private static int _task_count_ = 0;

        string task_name;

        int burst_time;

        int arrival_time;

        double wait_time;

        double turn_around_time;

        double first_response_time;

        double completion_time;

        int execution_time;

        bool finished;

        public Task(int burst_time, int arrival_time)
        {
            this.task_name = "P" + _task_count_.ToString();
            this.burst_time = burst_time;
            this.arrival_time = arrival_time;
            this.wait_time = -1;
            this.turn_around_time = -1;
            this.first_response_time = -1;
            this.Completion_time = -1;
            this.Finished = false;
            _task_count_++;
        }

        [DisplayName("Task Name")]
        public string Task_name { get => task_name; set => task_name = value; }

        [DisplayName("Burst Time")]
        public int Burst_time { get => burst_time; set => burst_time = value; }

        [DisplayName("Arrival Time")]
        public int Arrival_time { get => arrival_time; set => arrival_time = value; }

        [DisplayName("Wait Time")]
        public double Wait_time { get => wait_time; set => wait_time = value; }

        [DisplayName("Turn Around Time")]
        public double Turn_around_time { get => turn_around_time; set => turn_around_time = value; }

        [DisplayName("First Response Time")]
        public double First_response_time { get => first_response_time; set => first_response_time = value; }

        [DisplayName("Completion Time")]
        public double Completion_time { get => completion_time; set => completion_time = value; }

        [DisplayName("Execution Time MS")]
        public int Execution_time { get => execution_time; set => execution_time = value; }

        public bool Finished { get => finished; set => finished = value; }
    }
}
