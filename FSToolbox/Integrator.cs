using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSToolbox
{
    class Integrator
    {
        private double standard;
        private double value;
        private double result;

        public Integrator(double startValue)
        {
            value = startValue;
            standard = startValue;
        }


        public void setStartValue(double start)
        {
            standard = start;
        }


        public void integrate(double time)
        {
            result += value * time;
        }

        public void setValue(double value)
        {

        }

        public double getResult()
        {
            return result;
        }

        public void reset()
        {
            value = standard;
        }
    }
}
