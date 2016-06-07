using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FSInterface;


//static fsi client for all panels to use

namespace FSToolbox
{
    public class FSIcm : FSIClient
    {
        public static FSIcm inst = null;

        public FSIcm(String desc) : base(desc)
        {
            inst = this;
        }
    }
}
