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
        public ELT()
        {
            //debug variable
            is_debug = true;

            //starting FSI Client for IRS
            FSIcm.inst.OnVarReceiveEvent += fsiOnVarReceive;
            FSIcm.inst.DeclareAsWanted(new FSIID[]
                {
                    FSIID.MBI_ELT_ARM_SWITCH
                }
            );

            //standard values
            LightController.set(FSIID.MBI_ELT_ACTIVE_LIGHT, false);

            FSIcm.inst.MBI_ELT_LAMPTEST = false;

            FSIcm.inst.ProcessWrites();
        }


        void fsiOnVarReceive(FSIID id)
        {
            if (id == FSIID.MBI_ELT_ARM_SWITCH)
            {
                if (!FSIcm.inst.MBI_ELT_ARM_SWITCH)
                {
                    debug("ELT On");
                } else
                {
                    debug("ELT Arm");
                }
                

                //ELT light
                LightController.set(FSIID.MBI_ELT_ACTIVE_LIGHT, !FSIcm.inst.MBI_ELT_ARM_SWITCH);
                LightController.ProcessWrites();
            }
        }
    }
}
