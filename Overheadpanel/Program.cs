using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FSToolbox;

namespace Overheadpanel
{
    class Program
    {
        private static TimerManager timerManager;
        private static LightController lightController;

        static void Main(string[] args)
        {
            //enabling timer manager
            timerManager = new TimerManager();
            lightController = new FSToolbox.LightController();
            
            //starting all the overhead progeams
            IRS irs = new IRS();
            ELT elt = new ELT();
            OXY oxy = new OXY();
            HEAT heat = new HEAT();
            ANTIICE antiice = new ANTIICE();
            HYDRAULICS hydraulics = new HYDRAULICS();
            ELEC elec = new ELEC();
            FUEL fuel = new FUEL();

            while (true)
            {
                System.Threading.Thread.Sleep(100);
            }
        }
    }
}
