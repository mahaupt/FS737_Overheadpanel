using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FSInterface;
using FSToolbox;

namespace Overheadpanel
{
    class FRECSTALLTEST : Panel
    {
        public FRECSTALLTEST()
        {
            //debug variable
            is_debug = true;

            //starting FSI Client for IRS
            FSIcm.inst.OnVarReceiveEvent += fsiOnVarReceive;
            FSIcm.inst.DeclareAsWanted(new FSIID[]
                {
                    FSIID.MBI_FLIGHT_REC_SPEED_WARNING_TEST_1_SWITCH,
                    FSIID.MBI_FLIGHT_REC_SPEED_WARNING_TEST_2_SWITCH,
                    FSIID.MBI_FLIGHT_REC_STALL_WARNING_TEST_1_SWITCH,
                    FSIID.MBI_FLIGHT_REC_STALL_WARNING_TEST_2_SWITCH,
                    FSIID.MBI_FLIGHT_REC_TEST_SWITCH
                }
            );

            //standard values
            LightController.set(FSIID.MBI_FLIGHT_REC_OFF_LIGHT, true);

            FSIcm.inst.MBI_FLIGHT_REC_LAMPTEST = false;

            FSIcm.inst.ProcessWrites();
            LightController.ProcessWrites();
        }


        void fsiOnVarReceive(FSIID id)
        {
            //FLIGHT REC TEST
            if (id == FSIID.MBI_FLIGHT_REC_TEST_SWITCH)
            {
                if (FSIcm.inst.MBI_FLIGHT_REC_TEST_SWITCH)
                {
                    debug("FLIGHT REC TEST");
                    LightController.set(FSIID.MBI_FLIGHT_REC_OFF_LIGHT, false);
                }
                else
                {
                    debug("FLIGHT REC NORM");
                    LightController.set(FSIID.MBI_FLIGHT_REC_OFF_LIGHT, true);
                }
                LightController.ProcessWrites();
            }
        }
    }
}
