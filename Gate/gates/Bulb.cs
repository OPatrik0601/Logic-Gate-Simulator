using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Gate
{
    class Bulb : Mod
    {
        private bool isok = false;
        public override string imagePath {
            get {
                return (isok) ? "bulb_1.png" : "bulb_0.png";
            }
        }

        public override int InputNumber {
            get {
                return 1;
            }
        }

        public override bool IsThereOutput {
            get {
                return false;
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

            if (counter != 0 && found == InputNumber)
            {
                isok = true;
                return true;
            } else
            {
                isok = false;
                return false;
            }
        }
    }
}
