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
        private static FSIcm fsicm;
        private static TimerManager timerManager;
        private static LightController lightController;

        static void Main(string[] args)
        {
            fsicm = new FSIcm("Overheadpanel Systems");
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
            AIRCOND aircond = new AIRCOND();
            PNEUMATICS pneumatics = new PNEUMATICS();
            TPANEL tpanel = new TPANEL();
            DOORS doors = new DOORS();
            ENGINEECC engineecc = new ENGINEECC();
            FRECSTALLTEST frecstalltest = new FRECSTALLTEST();
            LED led = new LED();

            //enable light control after all lights have been initialized
            LightController.enableUpdate();

            while (true)
            {
                System.Threading.Thread.Sleep(100);
            }
        }
    }
}
