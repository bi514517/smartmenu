using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication5.Models;

namespace smartmenu.Models
{
    public class order
    {
        public int id;
        public table Table;
        public ArrayList ordered;
        public DateTime time;
        public order(int id, table TableIp,DateTime time, ArrayList ordered)
        {
            this.id = id;
            this.Table = TableIp;
            this.time = time;
            this.ordered = ordered;
        }
        public order(int id, table Table, DateTime time)
        {
            this.id = id;
            this.Table = Table;
            this.time = time;
        }
        public class orderItem
        {
            food food;
            int amount;

            public orderItem(food food, int amount)
            {
                this.food = food;
                this.amount = amount;
            }
        }
    }
}
