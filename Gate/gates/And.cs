using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gate
{
    class And : Mod
    {
        public override string imagePath {
            get {
                return "and.png";
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
            int counter = 0;
            int found = 0;
            for (int i = 0; i < conns.Length; i++)
            {
                if (conns[i].input == this)
                {
                    counter++;
                    if (conns[i].output.isActive(conns))
                    {
                        found++;
                    }
                }
            }

            return (counter == InputNumber && found == InputNumber) ? true : false;

            /* 
             * A    B   Out
             * 0    0   0   
             * 0    1   0
             * 1    0   0
             * 1    1   1
             */
        }
    }
}
