using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FSInterface;

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
            fsi.MBI_ELT_ACTIVE_LIGHT = false;

            fsi.ProcessWrites();
        }


        static void fsiOnVarReceive(FSIID id)
        {
            if (id == FSIID.MBI_ELT_ARM_SWITCH && fsi.MBI_ELT_ARM_SWITCH == false)
            {
                debug("ELT On");

                //ELT light
                fsi.MBI_ELT_ACTIVE_LIGHT = true;
                fsi.ProcessWrites();
            }
            if (id == FSIID.MBI_ELT_ARM_SWITCH && fsi.MBI_ELT_ARM_SWITCH == true)
            {
                debug("ELT Arm");

                fsi.MBI_ELT_ACTIVE_LIGHT = false;
                fsi.ProcessWrites();
            }
        }
    }
}
