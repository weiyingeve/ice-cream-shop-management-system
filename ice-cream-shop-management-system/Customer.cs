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
    class Customer
    {
        public string Name { get; set; }
        public int MemberId { get; set; }
        public DateTime Dob { get; set; }
        public Order CurrentOrder { get; set; }
        public List<Order> OrderHistory { get; set; } = new List<Order>();
        public PointCard Rewards { get; set; }
        public Customer() { }
        public Customer(string name, int memberId, DateTime dob)
        {
            Name=name;
            MemberId=memberId;
            Dob=dob;
        }
        public Order MakeOrder()
        {
            Console.WriteLine("Enter order id: ");
            int id = Convert.ToInt32(Console.ReadLine());
            DateTime timeReceived = DateTime.Now;
            return new Order(id, timeReceived);
        }
        public bool IsBirthday()
        {
            int month = Dob.Month;
            int day = Dob.Year;
            if (month == DateTime.Now.Month && day == DateTime.Now.Day)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public override string ToString()
        {
            return "Name: " + Name + "\tMember ID: " + MemberId + "\tDate Of Birth: " + Dob;
        }
    }
}
