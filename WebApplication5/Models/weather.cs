using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication5.Models
{
    public class weather
    {
        public int id;
        public string describe, icon;
        public float temperature, humidity;

        public weather()
        {
        }

        public weather(int id, string describe, string icon, float temperature, float humidity)
        {
            this.id = id;
            this.describe = describe;
            this.icon = icon;
            this.temperature = temperature -273;
            this.humidity = humidity;
        }
    }
}
