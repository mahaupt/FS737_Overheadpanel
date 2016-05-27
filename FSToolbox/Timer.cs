using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSToolbox
{
    public class Timer
    {
        private Action callback;
        private double timeout;
        private double current_time;
        private bool enabled;


        public Timer(double _timeout, Action _func)
        {
            timeout = _timeout;
            callback = _func;
        }


        //called when TimerManager samples time
        public void PassTime(double _sec)
        {
            //if timer is not enabled -- abort
            if (!enabled) return;

            current_time += _sec;

            //Timer is expired, call callback
            if (current_time >= timeout)
            {
                Reset();
                callback();
            }
        }

        public void Reset()
        {
            enabled = false;
            current_time = 0;
        }

        public void Start()
        {
            enabled = true;
        }

        public void Stop()
        {
            enabled = false;
        }
    }
}
