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
    class Waffle : IceCream
    {
        public string WaffleFlavour { get; set; }
        public Waffle() : base() { }
        public Waffle(string option, int scoops, List<Flavour> flavours,
            List<Topping> toppings, string waffleflavour) : base(option, scoops, flavours, toppings)
        {
            WaffleFlavour = waffleflavour;
        }
        public override double CalculatePrice()
        {
            double price;
            if (base.Scoops == 1)
            {
                price = 7.00;
            }
            else if (base.Scoops == 2)
            {
                price = 8.50;
            }
            else
            {
                price = 9.50;
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

            if (WaffleFlavour == "Red Velvet" || WaffleFlavour == "Charcoal" ||
                WaffleFlavour == "Pandan")
            {
                price += 3;
            }
            return price;
        }
        public override string ToString()
        {
            return base.ToString() + "\tWaffle Flavour: " + WaffleFlavour;
        }
    }
}
