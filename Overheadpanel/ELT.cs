using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FSInterface;
using FSToolbox;

namespace Overheadpanel
{
    class ELT : Panel
    {
        private static FSIClient fsi;

        public ELT()
        {
            //debug variable
            is_debug = true;

            //starting FSI Client for IRS
            fsi = new FSIClient("Overhead ELT");
            fsi.OnVarReceiveEvent += fsiOnVarReceive;
            fsi.DeclareAsWanted(new FSIID[]
                {
                    FSIID.MBI_ELT_ARM_SWITCH
                }
            );

            //standard values
            LightController.set(FSIID.MBI_ELT_ACTIVE_LIGHT, false);

            fsi.MBI_ELT_LAMPTEST = false;

            fsi.ProcessWrites();
            LightController.ProcessWrites();
        }


        static void fsiOnVarReceive(FSIID id)
        {
            if (id == FSIID.MBI_ELT_ARM_SWITCH)
            {
                if (!fsi.MBI_ELT_ARM_SWITCH)
                {
                    debug("ELT On");
                } else
                {
                    debug("ELT Arm");
                }
                

                //ELT light
                LightController.set(FSIID.MBI_ELT_ACTIVE_LIGHT, !fsi.MBI_ELT_ARM_SWITCH);
                LightController.ProcessWrites();
            }
        }
    }
}
