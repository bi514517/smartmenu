using smartmenu.Models;
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
        public foodType foodType;
        public foodSize foodSize;
        public float rate;

        public food(string foodId)
        {
            this.foodId = foodId;
        }

        public food(string foodId, string foodName, int price, string image, string description, foodType foodType)
        {
            this.foodId = foodId;
            this.foodName = foodName;
            this.price = price;
            this.image = image;
            this.description = description;
            this.foodType = foodType;
        }

        public food(string foodId, string foodName, int price, string image, string description, foodType foodType, foodSize foodSize)
        {
            this.foodId = foodId;
            this.foodName = foodName;
            this.image = image;
            this.description = description;
            this.price = price;
            this.foodType = foodType;
            this.foodSize = foodSize;
        }

        public food(string foodId, string foodName, int price, string image, string description, foodType foodType, foodSize foodSize, float rate)
        {
            this.foodId = foodId;
            this.foodName = foodName;
            this.image = image;
            this.description = description;
            this.price = price;
            this.foodType = foodType;
            this.foodSize = foodSize;
            this.rate = rate;
        }
    }
}
