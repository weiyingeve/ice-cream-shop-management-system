using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//==========================================================
// Student Number : S10257093
// Student Name : Isabelle Tan
// Partner Name : Charlotte Lee
//==========================================================
namespace ice_cream_shop_management_system
{
    class Cup : IceCream
    {
        public Cup() : base() { }
        public Cup(string option, int scoops, List<Flavour> flavours,
            List<Topping> toppings) : base(option, scoops, flavours, toppings) { }
        public override double CalculatePrice()
        {
            double price;
            if (base.Scoops == 1)
            {
                price = 4.00;
            }
            else if (base.Scoops == 2)
            {
                price = 5.50;
            }
            else
            {
                price = 6.50;
            }

            foreach (Flavour flavour in base.Flavours)
            {
                if (flavour.Premium)
                {
                    price += 2 * flavour.Quantity;
                }
            }

            if (base.Toppings.Count > 0)
            {
                price += base.Toppings.Count * 1;
            }
            return price;
        }
        public override string ToString()
        {
            return base.ToString();
        }
    }
}