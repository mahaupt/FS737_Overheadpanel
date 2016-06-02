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
        private static FSIClient fsi;

        public HYDRAULICS()
        {
            //debug variable
            is_debug = true;

            //starting FSI Client for IRS
            fsi = new FSIClient("Overhead HYDRAULICS");
            fsi.OnVarReceiveEvent += fsiOnVarReceive;
            fsi.DeclareAsWanted(new FSIID[]
                {
                    FSIID.SLI_GEN_1_RTL,
                    FSIID.SLI_GEN_2_RTL,
                    
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

            fsi.MBI_HYDRAULICS_LAMPTEST = false;

            fsi.ProcessWrites();
            LightController.ProcessWrites();
        }


        static void fsiOnVarReceive(FSIID id)
        {
            //ELEC 1
            if (id == FSIID.MBI_HYDRAULICS_ELEC_1_SWITCH && fsi.MBI_HYDRAULICS_ELEC_1_SWITCH == true)
            {
                if (fsi.MBI_HYDRAULICS_ELEC_1_SWITCH) {
                    debug("HYDRAULICS ELEC 1 On");
                } else {
                    debug("HYDRAULICS ELEC 1 Off");
                }

                //ELT light
                LightController.set(FSIID.MBI_HYDRAULICS_ELEC_1_LOW_PRESSURE_LIGHT, !fsi.MBI_HYDRAULICS_ELEC_1_SWITCH);
                LightController.ProcessWrites();
            }

            //ELEC 2
            if (id == FSIID.MBI_HYDRAULICS_ELEC_2_SWITCH)
            {
                if (fsi.MBI_HYDRAULICS_ELEC_2_SWITCH) {
                    debug("HYDRAULICS ELEC 2 On");
                } else {
                    debug("HYDRAULICS ELEC 2 Off");
                }

                //ELT light
                LightController.set(FSIID.MBI_HYDRAULICS_ELEC_2_LOW_PRESSURE_LIGHT, !fsi.MBI_HYDRAULICS_ELEC_2_SWITCH);
                LightController.ProcessWrites();
            }


            //ENG 1
            if (id == FSIID.MBI_HYDRAULICS_ENG_1_SWITCH)
            {
                if (fsi.MBI_HYDRAULICS_ENG_1_SWITCH == true) {
                    debug("HYDRAULICS ENG 1 On");
                } else {
                    debug("HYDRAULICS ENG 1 Off");
                }

                //ELT light
                sim_hydraulics();
            }

            //ENG 2
            if (id == FSIID.MBI_HYDRAULICS_ENG_2_SWITCH && fsi.MBI_HYDRAULICS_ENG_2_SWITCH == true)
            {
                if (fsi.MBI_HYDRAULICS_ENG_2_SWITCH) {
                    debug("HYDRAULICS ENG 2 On");
                } else {
                    debug("HYDRAULICS ENG 2 Off");
                }

                //ELT light
                sim_hydraulics();
            }
            
            
            //engine ready to load
            if (id == FSIID.SLI_GEN_1_RTL || id == FSIID.SLI_GEN_2_RTL) {
                sim_hydraulics();
            }
        }
        
        
        private static void sim_hydraulics() {
            //gen 1 hyd pump
            if (fsi.MBI_HYDRAULICS_ENG_1_SWITCH && fsi.SLI_GEN_1_RTL) {
                LightController.set(FSIID.MBI_HYDRAULICS_ENG_1_LOW_PRESSURE_LIGHT, false);
            } else {
                LightController.set(FSIID.MBI_HYDRAULICS_ENG_1_LOW_PRESSURE_LIGHT, true);
            }
            
            //gen 2 hyd pump
            if (fsi.MBI_HYDRAULICS_ENG_2_SWITCH && fsi.SLI_GEN_2_RTL) {
                LightController.set(FSIID.MBI_HYDRAULICS_ENG_2_LOW_PRESSURE_LIGHT, false);
            } else {
                LightController.set(FSIID.MBI_HYDRAULICS_ENG_2_LOW_PRESSURE_LIGHT, true);
            }
            
            LightController.ProcessWrites();
        }

    }
}
