using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication5.Models
{
    public class food
    {
        public string foodId, foodName , image, description;
        public int price;

        public food(string foodId, string foodName, int price, string image, string description)
        {
            this.foodId = foodId;
            this.foodName = foodName;
            this.price = price;
            this.image = image;
            this.description = description;
        }

    }
}
