using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    internal class Flavour
    {
        public string Type { get; set; }
        public bool Premium { get; set; }
        public int Quantity { get; set; }
        public Flavour() { }
        public Flavour(string type, bool premium, int quantity)
        {
            Type=type;
            Premium=premium;
            Quantity=quantity;
        }
        public override string ToString()
        {
            return "Type:" + Type + "\tPremium: " + Premium + "\tQuantity: " + Quantity;
        }
    }
}