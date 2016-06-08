using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FSInterface;
using FSToolbox;

namespace Overheadpanel
{
    class ENGINEECC : Panel
    {
        public ENGINEECC()
        {
            //debug variable
            is_debug = true;

            //starting FSI Client for IRS
            FSIcm.inst.OnVarReceiveEvent += fsiOnVarReceive;
            FSIcm.inst.DeclareAsWanted(new FSIID[]
                {
                    FSIID.FSI_ENG1_THROTTLE_LEVER,
                    FSIID.FSI_ENG2_THROTTLE_LEVER
                }
            );

            //standard values
            LightController.set(FSIID.MBI_EEC_ENG_1_ENGINE_CONTROL_LIGHT, false);
            LightController.set(FSIID.MBI_EEC_ENG_1_EEC_ALTN_LIGHT, false);
            LightController.set(FSIID.MBI_EEC_ENG_1_EEC_ON_LIGHT, true);
            LightController.set(FSIID.MBI_EEC_ENG_1_REVERSER_LIGHT, false);

            LightController.set(FSIID.MBI_EEC_ENG_2_ENGINE_CONTROL_LIGHT, false);
            LightController.set(FSIID.MBI_EEC_ENG_2_EEC_ALTN_LIGHT, false);
            LightController.set(FSIID.MBI_EEC_ENG_2_EEC_ON_LIGHT, true);
            LightController.set(FSIID.MBI_EEC_ENG_2_REVERSER_LIGHT, false);

            FSIcm.inst.MBI_EEC_LAMPTEST = false;

            FSIcm.inst.ProcessWrites();
        }


        void fsiOnVarReceive(FSIID id)
        {
            if (id == FSIID.FSI_ENG1_THROTTLE_LEVER)
            {
                if (FSIcm.inst.FSI_ENG1_THROTTLE_LEVER < 0)
                {
                    LightController.set(FSIID.MBI_EEC_ENG_1_REVERSER_LIGHT, true);
                } else
                {
                    LightController.set(FSIID.MBI_EEC_ENG_1_REVERSER_LIGHT, false);
                }
                LightController.ProcessWrites();
            }

            if (id == FSIID.FSI_ENG2_THROTTLE_LEVER)
            {
                if (FSIcm.inst.FSI_ENG2_THROTTLE_LEVER < 0)
                {
                    LightController.set(FSIID.MBI_EEC_ENG_2_REVERSER_LIGHT, true);
                }
                else
                {
                    LightController.set(FSIID.MBI_EEC_ENG_2_REVERSER_LIGHT, false);
                }
                LightController.ProcessWrites();
            }
        }
    }
}
