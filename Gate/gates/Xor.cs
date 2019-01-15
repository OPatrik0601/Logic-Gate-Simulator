using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gate
{
    class Xor : Mod
    {
        public override string imagePath {
            get {
                return "xor.png";
            }
        }

        public override int InputNumber {
            get {
                return 2;
            }
        }

        public override bool IsThereOutput {
            get {
                return true;
            }
        }

        public override bool isActive(Connection[] conns)
        {
            int found = 0;
            int active = 0;
            for (int i = 0; i < conns.Length; i++)
            {
                if (conns[i].input == this)
                {
                    found++;
                    if (conns[i].output.isActive(conns))
                    {
                        active++;
                    }
                }
            }

            return (active == 1 && found == InputNumber) ? true : false;

            /* 
             * A    B   Out
             * 0    0   0   
             * 0    1   1
             * 1    0   1
             * 1    1   0
             */
        }
    }
}
