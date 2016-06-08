using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FSInterface;
using FSToolbox;

namespace Overheadpanel
{
    class OXY : Panel
    {
        public OXY()
        {
            //debug variable
            is_debug = true;

            //starting FSI Client for IRS
            FSIcm.inst.OnVarReceiveEvent += fsiOnVarReceive;
            FSIcm.inst.DeclareAsWanted(new FSIID[]
                {
                    FSIID.MBI_OXYGEN_PASS_OXY_SWITCH
                }
            );

            //standard values
            LightController.set(FSIID.MBI_OXYGEN_PASS_OXY_LIGHT, false);

            FSIcm.inst.MBI_OXYGEN_LAMPTEST = false;

            FSIcm.inst.ProcessWrites();
        }


        void fsiOnVarReceive(FSIID id)
        {
            if (id == FSIID.MBI_OXYGEN_PASS_OXY_SWITCH)
            {
                if (FSIcm.inst.MBI_OXYGEN_PASS_OXY_SWITCH == true)
                {
                    debug("OXY Pass ON");
                }
                else
                {
                    debug("OXY Pass NORM");
                }
                
                //ELT light
                LightController.set(FSIID.MBI_OXYGEN_PASS_OXY_LIGHT, FSIcm.inst.MBI_OXYGEN_PASS_OXY_SWITCH);
                LightController.ProcessWrites();
            }
        }
    }
}
