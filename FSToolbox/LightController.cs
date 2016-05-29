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
        private static int lights_status = 1; //0: all_out, 1: normal, 2: all_on


        private static Dictionary<FSIID, LightControllerLight> lightsList;
        private static FSIClient fsi;

        public LightController()
        {
            fsi = new FSIClient("Light Controller");
            lightsList = new Dictionary<FSIID, LightControllerLight>();
        }

        //set a light value
        public static void set(FSIID id, bool value)
        {
            if (!lightsList.ContainsKey(id))
            {
                String name = id.ToString("g") + "*";

                Type type = fsi.GetType();
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
            lightsList[id].writeStatus(lights_status, lights_brightness, ref fsi);
        }


        public static void ProcessWrites()
        {
            fsi.ProcessWrites();
        }

        private static void updateAll()
        {
            foreach(KeyValuePair<FSIID, LightControllerLight> light in lightsList)
            {
                light.Value.writeStatus(lights_status, lights_brightness, ref fsi);
            }
        }

        public static void setLightStatus(int _light_status)
        {
            lights_status = _light_status;
            updateAll();
        }

        public static void setLightBrightness(bool _lights_brightness)
        { 
            lights_brightness = _lights_brightness;
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
        public bool get(int main_light_status)
        {
            if (main_light_status == 1)
            {
                switch (light_status)
                {
                    case (0):
                    default:
                        return false;
                    case (1):
                        return light_on;
                    case (2):
                        return true;
                }
            } else if (main_light_status == 2)
            {
                return true;
            } else
            {
                return false;
            }

        }


        public void writeStatus(int main_light_status, bool light_brightness, ref FSIClient fsi_object)
        {
            bool true_light_status = get(main_light_status);

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
                if ((true_light_status && light_brightness) != (bool)((PropertyInfo)lightMember).GetValue(fsi_object))
                {
                    ((PropertyInfo)lightMember).SetValue(fsi_object, (true_light_status && light_brightness));
                }

                //dimmed
                if ((true_light_status && !light_brightness) != (bool)((PropertyInfo)dimmedLightMember).GetValue(fsi_object))
                {
                    ((PropertyInfo)dimmedLightMember).SetValue(fsi_object, (true_light_status && !light_brightness));
                }
            }

            
        }
    }
}
