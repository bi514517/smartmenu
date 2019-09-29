using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication5.Models
{
    public class foodType
    {
        public int id;
        public string name;

        public foodType(int id, string name)
        {
            this.id = id;
            this.name = name;
        }
    }
}
