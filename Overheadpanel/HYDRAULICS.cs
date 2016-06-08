using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FSInterface;
using FSToolbox;

namespace Overheadpanel
{
    class HYDRAULICS : Panel
    { 
        public HYDRAULICS()
        {
            //debug variable
            is_debug = true;

            //starting FSI Client for IRS
            FSIcm.inst.OnVarReceiveEvent += fsiOnVarReceive;
            FSIcm.inst.DeclareAsWanted(new FSIID[]
                {
                    FSIID.SLI_GEN_1_RTL,
                    FSIID.SLI_GEN_2_RTL,
                    FSIID.SLI_AC_XFR_BUS_1_PHASE_1_VOLTAGE,
                    FSIID.SLI_AC_XFR_BUS_2_PHASE_1_VOLTAGE,

                    FSIID.MBI_HYDRAULICS_ELEC_1_SWITCH,
                    FSIID.MBI_HYDRAULICS_ELEC_2_SWITCH,
                    FSIID.MBI_HYDRAULICS_ENG_1_SWITCH,
                    FSIID.MBI_HYDRAULICS_ENG_2_SWITCH
                }
            );

            //standard values
            LightController.set(FSIID.MBI_HYDRAULICS_ELEC_1_LOW_PRESSURE_LIGHT, true);
            LightController.set(FSIID.MBI_HYDRAULICS_ELEC_1_OVERHEAT_LIGHT, false);
            LightController.set(FSIID.MBI_HYDRAULICS_ELEC_2_LOW_PRESSURE_LIGHT, true);
            LightController.set(FSIID.MBI_HYDRAULICS_ELEC_2_OVERHEAT_LIGHT, false);
            LightController.set(FSIID.MBI_HYDRAULICS_ENG_1_LOW_PRESSURE_LIGHT, true);
            LightController.set(FSIID.MBI_HYDRAULICS_ENG_2_LOW_PRESSURE_LIGHT, true);

            FSIcm.inst.MBI_HYDRAULICS_LAMPTEST = false;

            FSIcm.inst.ProcessWrites();
        }


        void fsiOnVarReceive(FSIID id)
        {
            //ELEC 1
            if (id == FSIID.MBI_HYDRAULICS_ELEC_1_SWITCH)
            {
                if (FSIcm.inst.MBI_HYDRAULICS_ELEC_1_SWITCH) {
                    debug("HYDRAULICS ELEC 1 On");
                } else {
                    debug("HYDRAULICS ELEC 1 Off");
                }
                sim_hydraulics();
            }

            //ELEC 2
            if (id == FSIID.MBI_HYDRAULICS_ELEC_2_SWITCH)
            {
                if (FSIcm.inst.MBI_HYDRAULICS_ELEC_2_SWITCH) {
                    debug("HYDRAULICS ELEC 2 On");
                } else {
                    debug("HYDRAULICS ELEC 2 Off");
                }
                sim_hydraulics();
            }


            //ENG 1
            if (id == FSIID.MBI_HYDRAULICS_ENG_1_SWITCH)
            {
                if (FSIcm.inst.MBI_HYDRAULICS_ENG_1_SWITCH == true) {
                    debug("HYDRAULICS ENG 1 On");
                } else {
                    debug("HYDRAULICS ENG 1 Off");
                }

                //ELT light
                sim_hydraulics();
            }

            //ENG 2
            if (id == FSIID.MBI_HYDRAULICS_ENG_2_SWITCH)
            {
                if (FSIcm.inst.MBI_HYDRAULICS_ENG_2_SWITCH) {
                    debug("HYDRAULICS ENG 2 On");
                } else {
                    debug("HYDRAULICS ENG 2 Off");
                }

                //ELT light
                sim_hydraulics();
            }
            
            
            //engine ready to load
            if (id == FSIID.SLI_GEN_1_RTL || id == FSIID.SLI_GEN_2_RTL || id == FSIID.SLI_AC_XFR_BUS_1_PHASE_1_VOLTAGE || id == FSIID.SLI_AC_XFR_BUS_2_PHASE_1_VOLTAGE) {
                sim_hydraulics();
            }
        }
        
        
        private static void sim_hydraulics() {
            bool hyd_A_eng = false;
            bool hyd_B_eng = false;
            bool hyd_A_elec = false;
            bool hyd_B_elec = false;

            //gen 1 hyd pump
            if (FSIcm.inst.MBI_HYDRAULICS_ENG_1_SWITCH && FSIcm.inst.SLI_GEN_1_RTL) {
                LightController.set(FSIID.MBI_HYDRAULICS_ENG_1_LOW_PRESSURE_LIGHT, false);
                hyd_A_eng = true;
            } else {
                LightController.set(FSIID.MBI_HYDRAULICS_ENG_1_LOW_PRESSURE_LIGHT, true);
            }
            
            //gen 2 hyd pump
            if (FSIcm.inst.MBI_HYDRAULICS_ENG_2_SWITCH && FSIcm.inst.SLI_GEN_2_RTL) {
                LightController.set(FSIID.MBI_HYDRAULICS_ENG_2_LOW_PRESSURE_LIGHT, false);
                hyd_B_eng = true;
            } else {
                LightController.set(FSIID.MBI_HYDRAULICS_ENG_2_LOW_PRESSURE_LIGHT, true);
            }

            //elec 1 hyd pump
            if (FSIcm.inst.MBI_HYDRAULICS_ELEC_1_SWITCH && FSIcm.inst.SLI_AC_XFR_BUS_1_PHASE_1_VOLTAGE > 50)
            {
                hyd_B_elec = true;
                LightController.set(FSIID.MBI_HYDRAULICS_ELEC_1_LOW_PRESSURE_LIGHT, false);
            } else
            {
                LightController.set(FSIID.MBI_HYDRAULICS_ELEC_1_LOW_PRESSURE_LIGHT, true);
            }

            //elec 2 hyd pump
            if (FSIcm.inst.MBI_HYDRAULICS_ELEC_2_SWITCH && FSIcm.inst.SLI_AC_XFR_BUS_2_PHASE_1_VOLTAGE > 50)
            {
                hyd_A_elec = true;
                LightController.set(FSIID.MBI_HYDRAULICS_ELEC_2_LOW_PRESSURE_LIGHT, false);
            }
            else
            {
                LightController.set(FSIID.MBI_HYDRAULICS_ELEC_2_LOW_PRESSURE_LIGHT, true);
            }


            //system A pressure
            if (hyd_A_elec || hyd_A_eng)
            {
                FSIcm.inst.SLI_HYD_A_PRESSURE = 3000.0f;
            } else
            {
                FSIcm.inst.SLI_HYD_A_PRESSURE = 0;
            }

            //system B pressure
            if (hyd_B_elec || hyd_B_eng)
            {
                FSIcm.inst.SLI_HYD_B_PRESSURE = 3000.0f;
            } else
            {
                FSIcm.inst.SLI_HYD_B_PRESSURE = 0;
            }

            //stby hyd pressure when one elec xfr bus powered or engines are alive
            if (FSIcm.inst.SLI_AC_XFR_BUS_1_PHASE_1_VOLTAGE > 50 || FSIcm.inst.SLI_AC_XFR_BUS_2_PHASE_1_VOLTAGE > 50 || hyd_A_eng || hyd_B_eng)
            {
                FSIcm.inst.SLI_HYD_STBY_PRESSURE = 3000;
            } else
            {
                FSIcm.inst.SLI_HYD_STBY_PRESSURE = 0;
            }

            LightController.ProcessWrites();
        }

    }
}
