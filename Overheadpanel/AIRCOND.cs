using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FSInterface;
using FSToolbox;

namespace Overheadpanel
{
    class AIRCOND : Panel
    {

        public AIRCOND()
        {
            //debug variable
            is_debug = true;

            //starting FSI Client for IRS
            FSIcm.inst.OnVarReceiveEvent += fsiOnVarReceive;
            /*FSIcm.inst.DeclareAsWanted(new FSIID[]
                {
                    FSIID.MBI_ELT_ARM_SWITCH
                }
            );*/

            //standard values
            LightController.set(FSIID.MBI_AIR_COND_CONT_CAB_ZONE_TEMP_LIGHT, false);
            LightController.set(FSIID.MBI_AIR_COND_AFT_CAB_ZONE_TEMP_LIGHT, false);
            LightController.set(FSIID.MBI_AIR_COND_FWD_CAB_ZONE_TEMP_LIGHT, false);

            FSIcm.inst.MBI_AIR_COND_LAMPTEST = false;

            FSIcm.inst.ProcessWrites();
        }


        static void fsiOnVarReceive(FSIID id)
        {
            
        }
    }
}
