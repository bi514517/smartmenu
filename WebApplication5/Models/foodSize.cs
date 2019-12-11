using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace smartmenu.Models
{
    public class foodSize
    {
        public int id;
        public string name;

        public foodSize(int id, string name)
        {
            this.id = id;
            this.name = name;
        }
    }
}
