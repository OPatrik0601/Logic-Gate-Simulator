using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Gate
{
    class Switch : Mod
    {
        private bool active = false;
        public override string imagePath {
            get {
                return (active) ? "switch_1.png" : "switch_0.png";
            }
        }

        public override int InputNumber {
            get {
                return 0;
            }
        }

        public override bool IsThereOutput {
            get {
                return true;
            }
        }

        private void Swap()
        {
            active = (active) ? false : true;
        }

        public override bool isActive(Connection[] conns)
        {
            return active;
        }

        public void OnClick(object sender, object e)
        {
            Swap();
        }
    }
}
