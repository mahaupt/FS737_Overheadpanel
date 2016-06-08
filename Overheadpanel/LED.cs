using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FSInterface;
using FSToolbox;

namespace Overheadpanel
{
    class LED : Panel
    {
        public LED()
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
            LightController.set(FSIID.MBI_LED_FLAPS_1_FULL_EXT_LIGHT, false);
            LightController.set(FSIID.MBI_LED_FLAPS_1_TRANSIT_LIGHT, false);
            LightController.set(FSIID.MBI_LED_FLAPS_2_FULL_EXT_LIGHT, false);
            LightController.set(FSIID.MBI_LED_FLAPS_2_TRANSIT_LIGHT, false);
            LightController.set(FSIID.MBI_LED_FLAPS_3_FULL_EXT_LIGHT, false);
            LightController.set(FSIID.MBI_LED_FLAPS_3_TRANSIT_LIGHT, false);
            LightController.set(FSIID.MBI_LED_FLAPS_4_FULL_EXT_LIGHT, false);
            LightController.set(FSIID.MBI_LED_FLAPS_4_TRANSIT_LIGHT, false);
            LightController.set(FSIID.MBI_LED_SLATS_1_EXT_LIGHT, false);
            LightController.set(FSIID.MBI_LED_SLATS_1_FULL_EXT_LIGHT, false);
            LightController.set(FSIID.MBI_LED_SLATS_1_TRANSIT_LIGHT, false);
            LightController.set(FSIID.MBI_LED_SLATS_2_EXT_LIGHT, false);
            LightController.set(FSIID.MBI_LED_SLATS_2_FULL_EXT_LIGHT, false);
            LightController.set(FSIID.MBI_LED_SLATS_2_TRANSIT_LIGHT, false);
            LightController.set(FSIID.MBI_LED_SLATS_3_EXT_LIGHT, false);
            LightController.set(FSIID.MBI_LED_SLATS_3_FULL_EXT_LIGHT, false);
            LightController.set(FSIID.MBI_LED_SLATS_3_TRANSIT_LIGHT, false);
            LightController.set(FSIID.MBI_LED_SLATS_4_EXT_LIGHT, false);
            LightController.set(FSIID.MBI_LED_SLATS_4_FULL_EXT_LIGHT, false);
            LightController.set(FSIID.MBI_LED_SLATS_4_TRANSIT_LIGHT, false);
            LightController.set(FSIID.MBI_LED_SLATS_5_EXT_LIGHT, false);
            LightController.set(FSIID.MBI_LED_SLATS_5_FULL_EXT_LIGHT, false);
            LightController.set(FSIID.MBI_LED_SLATS_5_TRANSIT_LIGHT, false);
            LightController.set(FSIID.MBI_LED_SLATS_6_EXT_LIGHT, false);
            LightController.set(FSIID.MBI_LED_SLATS_6_FULL_EXT_LIGHT, false);
            LightController.set(FSIID.MBI_LED_SLATS_6_TRANSIT_LIGHT, false);
            LightController.set(FSIID.MBI_LED_SLATS_7_EXT_LIGHT, false);
            LightController.set(FSIID.MBI_LED_SLATS_7_FULL_EXT_LIGHT, false);
            LightController.set(FSIID.MBI_LED_SLATS_7_TRANSIT_LIGHT, false);
            LightController.set(FSIID.MBI_LED_SLATS_8_EXT_LIGHT, false);
            LightController.set(FSIID.MBI_LED_SLATS_8_FULL_EXT_LIGHT, false);
            LightController.set(FSIID.MBI_LED_SLATS_8_TRANSIT_LIGHT, false);

            FSIcm.inst.MBI_LED_LAMPTEST = false;

            FSIcm.inst.ProcessWrites();
        }


        void fsiOnVarReceive(FSIID id)
        {
            if (id == FSIID.MBI_LED_TEST_SWITCH)
            {
                if (FSIcm.inst.MBI_LED_TEST_SWITCH)
                {
                    debug("LED Test On");
                }
                else
                {
                    debug("LED Test off");
                }

                //LED Test Lights
                LightController.set(FSIID.MBI_LED_FLAPS_1_FULL_EXT_LIGHT, FSIcm.inst.MBI_LED_TEST_SWITCH);
                LightController.set(FSIID.MBI_LED_FLAPS_1_TRANSIT_LIGHT, FSIcm.inst.MBI_LED_TEST_SWITCH);
                LightController.set(FSIID.MBI_LED_FLAPS_2_FULL_EXT_LIGHT, FSIcm.inst.MBI_LED_TEST_SWITCH);
                LightController.set(FSIID.MBI_LED_FLAPS_2_TRANSIT_LIGHT, FSIcm.inst.MBI_LED_TEST_SWITCH);
                LightController.set(FSIID.MBI_LED_FLAPS_3_FULL_EXT_LIGHT, FSIcm.inst.MBI_LED_TEST_SWITCH);
                LightController.set(FSIID.MBI_LED_FLAPS_3_TRANSIT_LIGHT, FSIcm.inst.MBI_LED_TEST_SWITCH);
                LightController.set(FSIID.MBI_LED_FLAPS_4_FULL_EXT_LIGHT, FSIcm.inst.MBI_LED_TEST_SWITCH);
                LightController.set(FSIID.MBI_LED_FLAPS_4_TRANSIT_LIGHT, FSIcm.inst.MBI_LED_TEST_SWITCH);
                LightController.set(FSIID.MBI_LED_SLATS_1_EXT_LIGHT, FSIcm.inst.MBI_LED_TEST_SWITCH);
                LightController.set(FSIID.MBI_LED_SLATS_1_FULL_EXT_LIGHT, FSIcm.inst.MBI_LED_TEST_SWITCH);
                LightController.set(FSIID.MBI_LED_SLATS_1_TRANSIT_LIGHT, FSIcm.inst.MBI_LED_TEST_SWITCH);
                LightController.set(FSIID.MBI_LED_SLATS_2_EXT_LIGHT, FSIcm.inst.MBI_LED_TEST_SWITCH);
                LightController.set(FSIID.MBI_LED_SLATS_2_FULL_EXT_LIGHT, FSIcm.inst.MBI_LED_TEST_SWITCH);
                LightController.set(FSIID.MBI_LED_SLATS_2_TRANSIT_LIGHT, FSIcm.inst.MBI_LED_TEST_SWITCH);
                LightController.set(FSIID.MBI_LED_SLATS_3_EXT_LIGHT, FSIcm.inst.MBI_LED_TEST_SWITCH);
                LightController.set(FSIID.MBI_LED_SLATS_3_FULL_EXT_LIGHT, FSIcm.inst.MBI_LED_TEST_SWITCH);
                LightController.set(FSIID.MBI_LED_SLATS_3_TRANSIT_LIGHT, FSIcm.inst.MBI_LED_TEST_SWITCH);
                LightController.set(FSIID.MBI_LED_SLATS_4_EXT_LIGHT, FSIcm.inst.MBI_LED_TEST_SWITCH);
                LightController.set(FSIID.MBI_LED_SLATS_4_FULL_EXT_LIGHT, FSIcm.inst.MBI_LED_TEST_SWITCH);
                LightController.set(FSIID.MBI_LED_SLATS_4_TRANSIT_LIGHT, FSIcm.inst.MBI_LED_TEST_SWITCH);
                LightController.set(FSIID.MBI_LED_SLATS_5_EXT_LIGHT, FSIcm.inst.MBI_LED_TEST_SWITCH);
                LightController.set(FSIID.MBI_LED_SLATS_5_FULL_EXT_LIGHT, FSIcm.inst.MBI_LED_TEST_SWITCH);
                LightController.set(FSIID.MBI_LED_SLATS_5_TRANSIT_LIGHT, FSIcm.inst.MBI_LED_TEST_SWITCH);
                LightController.set(FSIID.MBI_LED_SLATS_6_EXT_LIGHT, FSIcm.inst.MBI_LED_TEST_SWITCH);
                LightController.set(FSIID.MBI_LED_SLATS_6_FULL_EXT_LIGHT, FSIcm.inst.MBI_LED_TEST_SWITCH);
                LightController.set(FSIID.MBI_LED_SLATS_6_TRANSIT_LIGHT, FSIcm.inst.MBI_LED_TEST_SWITCH);
                LightController.set(FSIID.MBI_LED_SLATS_7_EXT_LIGHT, FSIcm.inst.MBI_LED_TEST_SWITCH);
                LightController.set(FSIID.MBI_LED_SLATS_7_FULL_EXT_LIGHT, FSIcm.inst.MBI_LED_TEST_SWITCH);
                LightController.set(FSIID.MBI_LED_SLATS_7_TRANSIT_LIGHT, FSIcm.inst.MBI_LED_TEST_SWITCH);
                LightController.set(FSIID.MBI_LED_SLATS_8_EXT_LIGHT, FSIcm.inst.MBI_LED_TEST_SWITCH);
                LightController.set(FSIID.MBI_LED_SLATS_8_FULL_EXT_LIGHT, FSIcm.inst.MBI_LED_TEST_SWITCH);
                LightController.set(FSIID.MBI_LED_SLATS_8_TRANSIT_LIGHT, FSIcm.inst.MBI_LED_TEST_SWITCH);
                LightController.ProcessWrites();
            }
        }
    }
}
