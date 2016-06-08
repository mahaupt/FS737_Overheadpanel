using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FSInterface;
using FSToolbox;

namespace Overheadpanel
{
    class ANTIICE : Panel
    {
        public ANTIICE()
        {
            //debug variable
            is_debug = true;

            //starting FSI Client for IRS
            FSIcm.inst.OnVarReceiveEvent += fsiOnVarReceive;
            FSIcm.inst.DeclareAsWanted(new FSIID[]
                {
                    FSIID.MBI_ANTI_ICE_ENG_1_ANTI_ICE_SWITCH,
                    FSIID.MBI_ANTI_ICE_ENG_2_ANTI_ICE_SWITCH,
                    FSIID.MBI_ANTI_ICE_WING_ANTI_ICE_SWITCH
                }
            );

            //standard values
            LightController.set(FSIID.MBI_ANTI_ICE_WING_L_VALVE_OPEN_LIGHT, false);
            LightController.set(FSIID.MBI_ANTI_ICE_WING_R_VALVE_OPEN_LIGHT, false);
            LightController.set(FSIID.MBI_ANTI_ICE_ENG_2_COWL_VALVE_OPEN_LIGHT, false);
            LightController.set(FSIID.MBI_ANTI_ICE_ENG_2_COWL_ANTI_ICE_LIGHT, false);
            LightController.set(FSIID.MBI_ANTI_ICE_ENG_1_COWL_VALVE_OPEN_LIGHT, false);
            LightController.set(FSIID.MBI_ANTI_ICE_ENG_1_COWL_ANTI_ICE_LIGHT, false);

            FSIcm.inst.MBI_ANTI_ICE_LAMPTEST = false;

            FSIcm.inst.ProcessWrites();
        }


        void fsiOnVarReceive(FSIID id)
        {

            //WING
            if (id == FSIID.MBI_ANTI_ICE_WING_ANTI_ICE_SWITCH && FSIcm.inst.MBI_ANTI_ICE_WING_ANTI_ICE_SWITCH == false)
            {
                debug("ANTI_ICE Wing On");

                LightController.set(FSIID.MBI_ANTI_ICE_WING_L_VALVE_OPEN_LIGHT, true);
                LightController.set(FSIID.MBI_ANTI_ICE_WING_R_VALVE_OPEN_LIGHT, true);
                LightController.ProcessWrites();
            }
            if (id == FSIID.MBI_ANTI_ICE_WING_ANTI_ICE_SWITCH && FSIcm.inst.MBI_ANTI_ICE_WING_ANTI_ICE_SWITCH == true)
            {
                debug("ANTI_ICE Wing Off");

                LightController.set(FSIID.MBI_ANTI_ICE_WING_L_VALVE_OPEN_LIGHT, false);
                LightController.set(FSIID.MBI_ANTI_ICE_WING_R_VALVE_OPEN_LIGHT, false);
                LightController.ProcessWrites();
            }


            //ENG 1
            if (id == FSIID.MBI_ANTI_ICE_ENG_1_ANTI_ICE_SWITCH && FSIcm.inst.MBI_ANTI_ICE_ENG_1_ANTI_ICE_SWITCH == false)
            {
                debug("ANTI_ICE ENG 1 On");

                LightController.set(FSIID.MBI_ANTI_ICE_ENG_1_COWL_VALVE_OPEN_LIGHT, true);
                LightController.ProcessWrites();
            }
            if (id == FSIID.MBI_ANTI_ICE_ENG_1_ANTI_ICE_SWITCH && FSIcm.inst.MBI_ANTI_ICE_ENG_1_ANTI_ICE_SWITCH == true)
            {
                debug("ANTI_ICE ENG 1 Off");

                LightController.set(FSIID.MBI_ANTI_ICE_ENG_1_COWL_VALVE_OPEN_LIGHT, false);
                LightController.ProcessWrites();
            }


            //ENG 2
            if (id == FSIID.MBI_ANTI_ICE_ENG_2_ANTI_ICE_SWITCH && FSIcm.inst.MBI_ANTI_ICE_ENG_2_ANTI_ICE_SWITCH == false)
            {
                debug("ANTI_ICE ENG 2 On");

                LightController.set(FSIID.MBI_ANTI_ICE_ENG_2_COWL_VALVE_OPEN_LIGHT, true);
                LightController.ProcessWrites();
            }
            if (id == FSIID.MBI_ANTI_ICE_ENG_2_ANTI_ICE_SWITCH && FSIcm.inst.MBI_ANTI_ICE_ENG_2_ANTI_ICE_SWITCH == true)
            {
                debug("ANTI_ICE ENG 2 Off");

                LightController.set(FSIID.MBI_ANTI_ICE_ENG_2_COWL_VALVE_OPEN_LIGHT, false);
                LightController.ProcessWrites();
            }
        }
        
    }
}
