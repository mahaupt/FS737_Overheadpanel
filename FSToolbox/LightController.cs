using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FSInterface;
using System.Reflection;

/*
 * 
 * This class controls the control lights.
 * If there is no power, this class turns all lights to off, or to on for testing
 * You can also set light failures here
 * 
 */

namespace FSToolbox
{
    public class LightController
    {
        private static bool lights_brightness = true; //1: bright, 0: dimmed
        private static bool lights_power = true; //1: power on, 0: power off
        private static bool lights_test = false; //0: normal, : all lights on
        private static bool is_debug = true;
        private static bool updateLocked = true;


        private static Dictionary<FSIID, LightControllerLight> lightsList;

        public LightController()
        {
            lightsList = new Dictionary<FSIID, LightControllerLight>();

            FSIcm.inst.OnVarReceiveEvent += fsiOnVarReceive;
            FSIcm.inst.DeclareAsWanted(new FSIID[]
                {
                    FSIID.MBI_MIP_CM1_LIGHTS_TEST_SWITCH_DIM_POS,
                    FSIID.MBI_MIP_CM1_LIGHTS_TEST_SWITCH_TEST_POS
                }
            );

            FSIcm.inst.ProcessWrites();
        }


        public static void fsiOnVarReceive(FSIID id)
        {

            if (id ==FSIID.MBI_MIP_CM1_LIGHTS_TEST_SWITCH_DIM_POS || id == FSIID.MBI_MIP_CM1_LIGHTS_TEST_SWITCH_TEST_POS)
            {
                if (FSIcm.inst.MBI_MIP_CM1_LIGHTS_TEST_SWITCH_DIM_POS)
                {
                    debug("MIP Lights Dim");
                    setLightBrightness(false);
                    setLightTest(false);
                } else if (FSIcm.inst.MBI_MIP_CM1_LIGHTS_TEST_SWITCH_TEST_POS)
                {
                    debug("MIP Lights Test");
                    setLightBrightness(true);
                    setLightTest(true);
                } else
                {
                    
                    debug("MIP Lights Norm");
                    setLightTest(false);
                    setLightBrightness(true);
                }
                ProcessWrites();
            }
        }

        //set a light value
        public static void set(FSIID id, bool value)
        {
            if (!lightsList.ContainsKey(id))
            {
                String name = id.ToString("g") + "*";

                Type type = FSIcm.inst.GetType();
                MemberInfo[] memberInfos = type.GetMember(name);

                if (memberInfos.Length == 1)
                {
                    lightsList.Add(id, new LightControllerLight(memberInfos[0]));
                } else if (memberInfos.Length == 2)
                {
                    lightsList.Add(id, new LightControllerLight(memberInfos[0], memberInfos[1]));
                } else
                {
                    Console.WriteLine("ERROR: LightController: Too many Lights found for name: " + name);
                    return;
                }
            }

            //set the light value
            lightsList[id].set(value);
            lightsList[id].writeStatus(lights_power, lights_test, lights_brightness, ref FSIcm.inst);
        }


        public static void ProcessWrites()
        {
            FSIcm.inst.ProcessWrites();
        }

        private static void updateAll()
        {
            if (!updateLocked)
            //enure that this is not called until all lights have been added to the lightsList
            {
                for (int i = 0; i < lightsList.Count; i++)
                {
                    //Variable nicht Thread-Safe! Bei Errors erstmal auf weiter drücken
                    lightsList.ElementAt(i).Value.writeStatus(lights_power, lights_test, lights_brightness, ref FSIcm.inst);
                }
            }
        }

        public static void setLightPower(bool _light_power)
        {
            lights_power = _light_power;
            updateAll();
        }

        public static void setLightTest(bool _light_test)
        {
            lights_test = _light_test;
            updateAll();
        }

        public static void setLightBrightness(bool _lights_brightness)
        { 
            lights_brightness = _lights_brightness;
            updateAll();
        }

        protected static void debug(String str)
        {
            if (is_debug)
            {
                Console.WriteLine(str);
            }
        }

        public static void enableUpdate()
        {
            updateLocked = false;
            updateAll();
        }
    }


    class LightControllerLight
    {
        private int light_status = 1; //0: always_off, 1: normal, 2: always_on
        private bool light_on = false;
        private bool is_dimmable = false;

        MemberInfo lightMember, dimmedLightMember;


        public LightControllerLight(MemberInfo _light, MemberInfo _dimmable_light)
        {
            lightMember = _light;
            dimmedLightMember = _dimmable_light;
            is_dimmable = true;
        }

        public LightControllerLight(MemberInfo _light)
        {
            lightMember = _light;
            is_dimmable = false;
        }


        //set the light status
        public void set(bool value)
        {
            light_on = value;
        }

        //get if the light is on or off
        public bool get(bool main_light_power, bool lights_test)
        {
            if (main_light_power)
            {
                switch (light_status)
                {
                    case (0):
                    default:
                        return false;
                    case (1):
                        if (lights_test)
                            return true;
                        return light_on;
                    case (2):
                        return true;
                }
            } else
            {
                return false;
            }

        }


        public void writeStatus(bool main_lights_power, bool lights_test, bool light_brightness, ref FSIcm fsi_object)
        {
            bool true_light_status = get(main_lights_power, lights_test);

            //write light status
            if (!is_dimmable)
            {
                //only bright lamp
                if (true_light_status != (bool)((PropertyInfo)lightMember).GetValue(fsi_object))
                {
                    ((PropertyInfo)lightMember).SetValue(fsi_object, true_light_status);
                }
            } else
            {
                //bright
                if (true_light_status != (bool)((PropertyInfo)lightMember).GetValue(fsi_object))
                {
                    ((PropertyInfo)lightMember).SetValue(fsi_object, true_light_status);
                }

                //dimmed
                if (!light_brightness != (bool)((PropertyInfo)dimmedLightMember).GetValue(fsi_object))
                {
                    ((PropertyInfo)dimmedLightMember).SetValue(fsi_object, !light_brightness);
                }
            }

            
        }
    }
}
