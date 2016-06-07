using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSToolbox
{
    class Integrator<T>
    {
        private T standard;
        private T value;
        private T result;

        public Integrator(T startValue)
        {
            value = startValue;
            standard = startValue;
        }


        public void setStartValue(T start)
        {
            standard = start;
        }


        public void integrate(double time)
        {
            //result += value * time;
        }

        public void setValue(T value)
        {

        }

        public T getResult()
        {
            return result;
        }

        public void reset()
        {
            value = standard;
        }
    }
}
