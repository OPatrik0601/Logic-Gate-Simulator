using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gate
{
    class Inverter : Mod
    {
        public override string imagePath {
            get {
                return "inverter.png";
            }
        }

        public override int InputNumber {
            get {
                return 1;
            }
        }

        public override bool IsThereOutput {
            get {
                return true;
            }
        }

        public override bool isActive(Connection[] conns)
        {
            for (int i = 0; i < conns.Length; i++)
            {
                if (conns[i].input == this)
                {
                    if (conns[i].output.isActive(conns))
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
            }

            return false;

            /* 
             * A    Out
             * 0    1   
             * 1    0
             */
        }
    }
}
