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
        private static double timer_interval_s = 0.1;

        private List<Timer> timerRequestList;
        public static TimerManager instance;
        System.Timers.Timer timer;

        //events
        public delegate void TimedCallbackFunction(double time);
        public static event TimedCallbackFunction TimerCallbackEvent;

        //save last time to get time difference
        private static double last_time = 0;

        public TimerManager(double time = 0.1)
        {
            timer_interval_s = time;
            timerRequestList = new List<Timer>();

            //create timer that is called every 100 ms
            timer = new System.Timers.Timer();
            timer.Elapsed += new System.Timers.ElapsedEventHandler(Update);
            timer.Interval = (int)(timer_interval_s*1000);
            timer.Enabled = true;

            //make instance public
            instance = this;
        }


        // Update is called once per frame
        private static void Update(object source, System.Timers.ElapsedEventArgs e)
        {
            double time = (e.SignalTime.Millisecond + e.SignalTime.Second * 1000);
            time /= 1000;

            double diff = time - last_time;
            if (diff < 0) diff += 60;
            last_time = time;
            

            //call timer classes
            if (instance != null)
            {
                foreach (Timer tr in instance.timerRequestList.ToArray())
                {
                    tr.PassTime(diff);
                }
            }

            //call timed fucntions
            TimedCallbackFunction tcf = TimerCallbackEvent;
            if (tcf != null)
            {
                tcf(time);
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
