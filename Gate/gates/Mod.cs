using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gate
{
    abstract class Mod
    {
        public abstract string imagePath { get; } //Image path in the img folder
        public abstract bool isActive(Connection[] conns);
        public abstract int InputNumber { get; } //Maximum number of inputs
        public abstract bool IsThereOutput { get; } //It's an input only mod?

        public void setCords(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public bool IsInputFull(Connection[] connections)
        {
            if (InputNumber == 0)
                return true;

            int counter = 0;
            for(int i=0;i<connections.Length;i++)
            {
                if(connections[i].input == this)
                {
                    counter++;
                }
            }
            if (counter == InputNumber)
                return true;
            else
                return false;
        }

        public int x { get; private set; }
        public int y { get; private set; }
    }
}
