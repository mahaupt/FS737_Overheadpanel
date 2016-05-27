using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSToolbox
{
    public class TimerManager
    {

        /*
         * 
         * This TimerManager class provides a clock for the normal timer classes.
         * In the method Update, it updates all timer in the Timer class instances.
         * It has to be created once, to enable Timer usage.
         * 
         */

         //Sample time in ms
        public const double timer_interval_ms = 0.1;


        private List<Timer> timerRequestList;
        public static TimerManager instance;
        System.Timers.Timer timer;

        public TimerManager()
        {
            timerRequestList = new List<Timer>();

            //create timer that is called every 100 ms
            timer = new System.Timers.Timer();
            timer.Elapsed += new System.Timers.ElapsedEventHandler(Update);
            timer.Interval = (int)(timer_interval_ms*1000);
            timer.Enabled = true;

            //make instance public
            instance = this;
        }


        // Update is called once per frame
        private static void Update(object source, System.Timers.ElapsedEventArgs e)
        {
            if (instance != null)
            {
                foreach (Timer tr in instance.timerRequestList.ToArray())
                {
                    tr.PassTime(timer_interval_ms);
                }
            }
        }


        public static void addTimer(Timer timer)
        {
            if (instance != null)
            {
                instance.timerRequestList.Add(timer);
            }
        }
    }
}
