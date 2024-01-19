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
        public DateTime? TimeFulfilled { get; set; }
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
            Console.Write("Enter new option: ");
            IceCreamList[orderno].Option = Console.ReadLine();
            Console.Write("Enter new number of scoops: ");
            IceCreamList[orderno].Scoops = Convert.ToInt32(Console.ReadLine());
            IceCreamList[orderno].Flavours.Clear();
            Console.Write("Enter new flavour (or nil to stop adding): ");
            string flavourtype = Console.ReadLine();
            while (flavourtype != "nil")
            {
                Console.Write("Is it premium? (True/False): ");
                bool flavourpremium = Convert.ToBoolean(Console.ReadLine());
                Console.Write("Enter new flavour quantity: ");
                int flavourquantity = Convert.ToInt32(Console.ReadLine());
                Flavour flavour = new Flavour(flavourtype, flavourpremium, flavourquantity);
                IceCreamList[orderno].Flavours.Add(flavour);
                Console.Write("Enter new flavour (or nil to stop adding): ");
                flavourtype = Console.ReadLine();
            }
            IceCreamList[orderno].Toppings.Clear();
            Console.Write("Enter new topping (or nil to stop adding): ");
            string toppingtype = Console.ReadLine();
            while (toppingtype != "nil")
            {
                Topping topping = new Topping(toppingtype);
                IceCreamList[orderno].Toppings.Add(topping); 
                Console.Write("Enter new topping (or nil to stop adding): ");
                toppingtype = Console.ReadLine();
            }
        }
        public void AddIceCream(IceCream iceCream)
        {
            IceCreamList.Add(iceCream);
        }
        public void DeleteIceCream(int num)
        {
            IceCreamList.RemoveAt(num);
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
            return "ID: " + Id + "\tTime Received: " + TimeReceived + "\tTime Fulfilled: " + 
                TimeFulfilled + "\tIceCreamList: " + IceCreamList;
        }
    }
}
