using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FSInterface;

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
                    FSIID.MBI_HYDRAULICS_ELEC_1_SWITCH,
                    FSIID.MBI_HYDRAULICS_ELEC_2_SWITCH,
                    FSIID.MBI_HYDRAULICS_ENG_1_SWITCH,
                    FSIID.MBI_HYDRAULICS_ENG_2_SWITCH
                }
            );

            //standard values
            fsi.MBI_HYDRAULICS_ELEC_1_LOW_PRESSURE_LIGHT = true;
            fsi.MBI_HYDRAULICS_ELEC_1_OVERHEAT_LIGHT = false;
            fsi.MBI_HYDRAULICS_ELEC_2_LOW_PRESSURE_LIGHT = true;
            fsi.MBI_HYDRAULICS_ELEC_2_OVERHEAT_LIGHT = false;
            fsi.MBI_HYDRAULICS_ENG_1_LOW_PRESSURE_LIGHT = true;
            fsi.MBI_HYDRAULICS_ENG_2_LOW_PRESSURE_LIGHT = true;

            fsi.ProcessWrites();
        }


        static void fsiOnVarReceive(FSIID id)
        {
            //ELEC 1
            if (id == FSIID.MBI_HYDRAULICS_ELEC_1_SWITCH && fsi.MBI_HYDRAULICS_ELEC_1_SWITCH == true)
            {
                debug("HYDRAULICS ELEC 1 On");

                //ELT light
                fsi.MBI_HYDRAULICS_ELEC_1_LOW_PRESSURE_LIGHT = false;
                fsi.ProcessWrites();
            }
            if (id == FSIID.MBI_HYDRAULICS_ELEC_1_SWITCH && fsi.MBI_HYDRAULICS_ELEC_1_SWITCH == false)
            {
                debug("HYDRAULICS ELEC 1 Off");

                //ELT light
                fsi.MBI_HYDRAULICS_ELEC_1_LOW_PRESSURE_LIGHT = true;
                fsi.ProcessWrites();
            }

            //ELEC 2
            if (id == FSIID.MBI_HYDRAULICS_ELEC_2_SWITCH && fsi.MBI_HYDRAULICS_ELEC_2_SWITCH == true)
            {
                debug("HYDRAULICS ELEC 2 On");

                //ELT light
                fsi.MBI_HYDRAULICS_ELEC_2_LOW_PRESSURE_LIGHT = false;
                fsi.ProcessWrites();
            }
            if (id == FSIID.MBI_HYDRAULICS_ELEC_2_SWITCH && fsi.MBI_HYDRAULICS_ELEC_2_SWITCH == false)
            {
                debug("HYDRAULICS ELEC 2 Off");

                //ELT light
                fsi.MBI_HYDRAULICS_ELEC_2_LOW_PRESSURE_LIGHT = true;
                fsi.ProcessWrites();
            }


            //ENG 1
            if (id == FSIID.MBI_HYDRAULICS_ENG_1_SWITCH && fsi.MBI_HYDRAULICS_ENG_1_SWITCH == true)
            {
                debug("HYDRAULICS ENG 1 On");

                //ELT light
                fsi.MBI_HYDRAULICS_ENG_1_LOW_PRESSURE_LIGHT = false;
                fsi.ProcessWrites();
            }
            if (id == FSIID.MBI_HYDRAULICS_ENG_1_SWITCH && fsi.MBI_HYDRAULICS_ENG_1_SWITCH == false)
            {
                debug("HYDRAULICS ENG 1 Off");

                //ELT light
                fsi.MBI_HYDRAULICS_ENG_1_LOW_PRESSURE_LIGHT = true;
                fsi.ProcessWrites();
            }

            //ENG 2
            if (id == FSIID.MBI_HYDRAULICS_ENG_2_SWITCH && fsi.MBI_HYDRAULICS_ENG_2_SWITCH == true)
            {
                debug("HYDRAULICS ENG 2 On");

                //ELT light
                fsi.MBI_HYDRAULICS_ENG_2_LOW_PRESSURE_LIGHT = false;
                fsi.ProcessWrites();
            }
            if (id == FSIID.MBI_HYDRAULICS_ENG_2_SWITCH && fsi.MBI_HYDRAULICS_ENG_2_SWITCH == false)
            {
                debug("HYDRAULICS ENG 2 Off");

                //ELT light
                fsi.MBI_HYDRAULICS_ENG_2_LOW_PRESSURE_LIGHT = true;
                fsi.ProcessWrites();
            }
        }
    }
}
