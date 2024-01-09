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
    internal class Topping
    {
        public string Type { get; set; }
        public Topping() { }
        public Topping(string type)
        {
            Type=type;
        }
        public override string ToString()
        {
            return "Type: " + Type;
        }
    }
}
