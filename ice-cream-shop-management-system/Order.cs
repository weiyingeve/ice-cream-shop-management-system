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
    class Order
    {
        public int Id { get; set; }
        public DateTime TimeReceived { get; set; }
        public DateTime? TimeFullfilled { get; set; }
        public List<IceCream> IceCreamList { get; set; } = new List<IceCream>();
        public Order() { }
        public Order(int id, DateTime timereceived)
        {
            Id = id;
            TimeReceived = timereceived;
        }
        public void ModifyIceCream(int orderid)
        {
            int orderno = orderid - 1;
            Console.WriteLine("Enter new option: ");
            IceCreamList[orderno].Option = Console.ReadLine();
            Console.WriteLine("Enter new number of scoops: ");
            IceCreamList[orderno].Scoops = Convert.ToInt32(Console.ReadLine());
            IceCreamList[orderno].Flavours.Clear();
            Console.WriteLine("Enter new flavour type (or nil to stop adding): ");
            string flavourtype = Console.ReadLine();
            Console.WriteLine("Is it premium? (True/False): ");
            bool flavourpremium = Convert.ToBoolean(Console.ReadLine());
            Console.WriteLine("Enter new flavour quantity: ");
            int flavourquantity = Convert.ToInt32(Console.ReadLine());
            Flavour flavour = new Flavour(flavourtype, flavourpremium, flavourquantity);
            IceCreamList[orderno].Flavours.Add(flavour);
            while (flavourtype != "nil")
            {
                Console.WriteLine("Enter new flavour (or nil to stop adding): ");
                flavourtype = Console.ReadLine();
                Console.WriteLine("Is it premium? (True/False): ");
                flavourpremium = Convert.ToBoolean(Console.ReadLine());
                Console.WriteLine("Enter new flavour quantity: ");
                flavourquantity = Convert.ToInt32(Console.ReadLine());
                flavour = new Flavour(flavourtype, flavourpremium, flavourquantity);
                IceCreamList[orderno].Flavours.Add(flavour);
            }
            IceCreamList[orderno].Toppings.Clear();
            Console.WriteLine("Enter new topping (or nil to stop adding): ");
            string toppingtype = Console.ReadLine();
            Topping topping = new Topping(toppingtype);
            IceCreamList[orderno].Toppings.Add(topping);
            while (toppingtype != "nil")
            {
                Console.WriteLine("Enter new topping (or nil to stop adding): ");
                toppingtype = Console.ReadLine();
                topping = new Topping(toppingtype);
                IceCreamList[orderno].Toppings.Add(topping);
            }
        }
        public void AddIceCream(IceCream iceCream)
        {
            IceCreamList.Add(iceCream);
            Console.WriteLine("Added successfully.");
        }
        public void DeleteIceCream(int num)
        {
            IceCreamList.RemoveAt(num);
            Console.WriteLine("Deleted successfully.");
        }
        public double CalculateTotal()
        {
            double total = 0;
            foreach (IceCream icecream in IceCreamList)
            {
                total += icecream.CalculatePrice();
            }
            return total;
        }
        public override string ToString()
        {
            return "ID: " + Id + "\tTime Received: " + TimeReceived + "\tTime Fullfilled: " + TimeFullfilled + "\tIceCreamList: " + IceCreamList;
        }
    }
}
