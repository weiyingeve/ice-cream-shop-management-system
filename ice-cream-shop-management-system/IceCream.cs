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
    abstract class IceCream
    {
        public string Option { get; set; }
        public int Scoops { get; set; }
        public List<Flavour> Flavours { get; set; }
        public List<Topping> Toppings { get; set; }
        public IceCream() { }
        public IceCream(string option, int scoops, List<Flavour> flavours,
            List<Topping> toppings)
        {
            Option=option;
            Scoops=scoops;
            Flavours=flavours;
            Toppings=toppings;
        }
        public abstract double CalculatePrice();
        public override string ToString()
        {
            return "Option: " + Option + "\tScoops: " + Scoops;
        }
    }
}
