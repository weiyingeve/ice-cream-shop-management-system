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
            throw new Exception(); //edit later
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
