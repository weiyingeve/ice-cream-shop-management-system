using System;
using System.Collections.Generic;
using System.Data;
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
        public Order MakeOrder(Dictionary<int, Order> orders, string[] Toppings, 
            string[] regFlavours, string[] premFlavours, string[] waffleFlavours, string[] Options)
        {

            bool InvalidOption = false;
            bool InvalidScoopNum = false;
            bool InvalidFlavourType = false;
            bool InvalidFlavourQuantity = false;
            bool InvalidToppingType = false;
            bool InvalidWaffleFlavour = false;
            bool InvalidDipped = false;
            bool InvalidAddIceCream = false;
            string option;
            int scoopnum;
            string flavourtype;
            bool flavourpremium = false;
            int flavourquantity = 0;
            int totalflavourquantity = 0;
            string toppingtype = null;
            string waffleflavour;
            bool dipped = false;
            string addicecream;

            if (CurrentOrder != null)
            {
                Console.WriteLine("There is already a current order for this customer.");
                return null;
            }
            else
            {
                Order order = new Order();
                do
                {
                    Console.WriteLine("Options available: ");
                    Console.WriteLine("Cup");
                    Console.WriteLine("Cone");
                    Console.WriteLine("Waffle");
                    Console.Write("Enter option: ");
                    option = Console.ReadLine();
                    if (string.IsNullOrWhiteSpace(option))
                    {
                        Console.WriteLine("Invalid option! Please enter a valid option.");
                        InvalidOption = true;
                    }
                    else
                    {
                        InvalidOption = false;
                        if (Options.Contains(option))
                        {
                            continue;
                        }
                        else
                        {
                            Console.WriteLine("Option does not exist in menu! Please enter a valid option.");
                            InvalidOption = true;
                        }
                    }
                } while (InvalidOption);

                do
                {
                    Console.Write("Enter number of scoops(1/2/3): ");
                    if (!int.TryParse(Console.ReadLine(), out scoopnum))
                    {
                        Console.WriteLine("Invalid option! Please enter a valid option.");
                        InvalidScoopNum = true;
                    }
                    else
                    {
                        InvalidScoopNum= false;
                        if (scoopnum > 3 || scoopnum < 1)
                        {
                            Console.WriteLine("Invalid scoop number! Please enter a valid option.");
                            InvalidScoopNum = true;
                        }
                        else continue;
                    }
                } while (InvalidScoopNum);

                List<Flavour> flavours = new List<Flavour>();
                totalflavourquantity = 0;

                while (totalflavourquantity < scoopnum)
                {
                    do
                    {
                        Console.WriteLine("Flavours available: ");
                        Console.WriteLine("Vanilla");
                        Console.WriteLine("Chocolate");
                        Console.WriteLine("Strawberry");
                        Console.WriteLine("Durian");
                        Console.WriteLine("Ube");
                        Console.WriteLine("Sea Salt");
                        Console.Write("Enter flavour type: ");
                        flavourtype = Console.ReadLine();
                        if (string.IsNullOrWhiteSpace(flavourtype))
                        {
                            Console.WriteLine("Invalid Flavour Type! Please enter a valid Flavour Type.");
                            InvalidFlavourType = true;
                        }
                        else
                        {
                            InvalidFlavourType= false;
                            if (premFlavours.Contains(flavourtype))
                            {
                                flavourpremium = true;
                            }
                            else if (regFlavours.Contains(flavourtype))
                            {
                                flavourpremium = false;
                            }
                            else
                            {
                                Console.WriteLine("Flavour entered not in menu! Please enter a different flavour.");
                                InvalidFlavourType = true;
                            }
                        }
                    } while (InvalidFlavourType);

                    do
                    {
                        Console.Write("Enter flavour quantity: ");
                        if (!int.TryParse(Console.ReadLine(), out flavourquantity))
                        {
                            Console.WriteLine("Invalid input! Please enter valid Flavour Quantity.");
                            InvalidFlavourQuantity = true;
                        }
                        else
                        {
                            InvalidFlavourQuantity = false;
                            if (flavourquantity > scoopnum)
                            {
                                Console.WriteLine("Entered quantity more than scoop number. Please try again.");
                                InvalidFlavourQuantity = true;
                            }
                            totalflavourquantity += flavourquantity;

                            if (totalflavourquantity > scoopnum)
                            {
                                Console.WriteLine("You've exceeded the scoop number. Please try again.");
                                InvalidFlavourQuantity = true;
                                totalflavourquantity -= flavourquantity;
                            }
                        }
                    } while (InvalidFlavourQuantity);

                    Flavour flavour = new Flavour(flavourtype, flavourpremium, flavourquantity);
                    flavours.Add(flavour);
                }
                List<Topping> toppings = new List<Topping>();
                while (toppingtype != "nil")
                {
                    do
                    {
                        Console.WriteLine("Toppings available: ");
                        Console.WriteLine("Sprinkles");
                        Console.WriteLine("Mochi");
                        Console.WriteLine("Sago");
                        Console.WriteLine("Oreos");
                        Console.Write("Enter topping (or nil to stop adding): ");
                        toppingtype = Console.ReadLine();
                        if (string.IsNullOrWhiteSpace(toppingtype))
                        {
                            Console.WriteLine("Invalid Topping Type! Please enter a valid Topping Type.");
                            InvalidToppingType = true;
                        }
                        else
                        {
                            InvalidToppingType= false;
                            if (Toppings.Contains(toppingtype))
                            {
                                continue;
                            }
                            else if (toppingtype == "nil")
                            {
                                continue;
                            }
                            else
                            {
                                Console.WriteLine("Topping entered not in menu. Try again!");
                                InvalidToppingType = true;
                            }
                        }
                    } while (InvalidToppingType);
                    if (toppingtype != "nil" && toppings.Count < 4)
                    {
                        Topping topping = new Topping(toppingtype);
                        toppings.Add(topping);
                    }
                    else if (toppings.Count == 4)
                    {
                        Console.WriteLine("Max topping amount reached.");
                        toppingtype = "nil";
                    }
                }

                IceCream iceCream = null;
                switch (option)
                {
                    case "Waffle":
                        do
                        {
                            Console.WriteLine("Waffle Flavours available: ");
                            Console.WriteLine("Red Velvet");
                            Console.WriteLine("Charcoal");
                            Console.WriteLine("Pandan");
                            Console.Write("Enter waffle flavour: ");
                            waffleflavour = Console.ReadLine();
                            if (string.IsNullOrWhiteSpace(waffleflavour))
                            {
                                Console.WriteLine("Waffle Flavour entered invalid. Try again!");
                                InvalidWaffleFlavour = true;
                            }
                            else
                            {
                                InvalidWaffleFlavour = false;
                                if (waffleFlavours.Contains(waffleflavour))
                                {
                                    continue;
                                }
                                else
                                {
                                    Console.WriteLine("Waffle Flavour entered is not on the menu. Try again!");
                                    InvalidWaffleFlavour = true;
                                }
                            }
                        } while (InvalidWaffleFlavour);

                        iceCream = new Waffle("Waffle", scoopnum, flavours, toppings, waffleflavour);
                        break;
                    case "Cone":
                        do
                        {
                            Console.Write("Is cone dipped? (True/False): ");
                            if (!bool.TryParse(Console.ReadLine(), out dipped))
                            {
                                Console.WriteLine("Entered invalid input. Try again.");
                                InvalidDipped = true;
                            }
                            else
                            {
                                InvalidDipped = false;
                            }
                        } while (InvalidDipped);
                        iceCream = new Cone("Cone", scoopnum, flavours, toppings, dipped);
                        break;
                    case "Cup":
                        iceCream = new Cup("Cup", scoopnum, flavours, toppings);
                        break;
                }
                order.AddIceCream(iceCream);

                Console.Write("Add Another Ice Cream? [Y/N]: ");
                addicecream = Console.ReadLine();
                while (addicecream == "Y")
                {
                    do
                    {
                        Console.WriteLine("Options available: ");
                        Console.WriteLine("Cup");
                        Console.WriteLine("Cone");
                        Console.WriteLine("Waffle");
                        Console.Write("Enter option: ");
                        option = Console.ReadLine();
                        if (string.IsNullOrWhiteSpace(option))
                        {
                            Console.WriteLine("Invalid option! Please enter a valid option.");
                            InvalidOption = true;
                        }
                        else
                        {
                            InvalidOption = false;
                            if (Options.Contains(option))
                            {
                                continue;
                            }
                            else
                            {
                                Console.WriteLine("Option does not exist in menu! Please enter a valid option.");
                                InvalidOption = true;
                            }
                        }
                    } while (InvalidOption);

                    do
                    {
                        Console.Write("Enter number of scoops(1/2/3): ");
                        if (!int.TryParse(Console.ReadLine(), out scoopnum))
                        {
                            Console.WriteLine("Invalid option! Please enter a valid option.");
                            InvalidScoopNum = true;
                        }
                        else
                        {
                            InvalidScoopNum= false;
                            if (scoopnum > 3 || scoopnum < 1)
                            {
                                Console.WriteLine("Invalid scoop number! Please enter a valid option.");
                                InvalidScoopNum = true;
                            }
                            else continue;
                        }
                    } while (InvalidScoopNum);

                    flavours = new List<Flavour>();
                    totalflavourquantity = 0;

                    while (totalflavourquantity < scoopnum)
                    {
                        do
                        {
                            Console.WriteLine("Flavours available: ");
                            Console.WriteLine("Vanilla");
                            Console.WriteLine("Chocolate");
                            Console.WriteLine("Strawberry");
                            Console.WriteLine("Durian");
                            Console.WriteLine("Ube");
                            Console.WriteLine("Sea Salt");
                            Console.Write("Enter flavour type: ");
                            flavourtype = Console.ReadLine();
                            if (string.IsNullOrWhiteSpace(flavourtype))
                            {
                                Console.WriteLine("Invalid Flavour Type! Please enter a valid Flavour Type.");
                                InvalidFlavourType = true;
                            }
                            else
                            {
                                InvalidFlavourType= false;
                                if (premFlavours.Contains(flavourtype))
                                {
                                    flavourpremium = true;
                                }
                                else if (regFlavours.Contains(flavourtype))
                                {
                                    flavourpremium = false;
                                }
                                else
                                {
                                    Console.WriteLine("Flavour entered not in menu! Please enter a different flavour.");
                                    InvalidFlavourType = true;
                                }
                            }
                        } while (InvalidFlavourType);

                        do
                        {
                            Console.Write("Enter flavour quantity: ");
                            if (!int.TryParse(Console.ReadLine(), out flavourquantity))
                            {
                                Console.WriteLine("Invalid input! Please enter valid Flavour Quantity.");
                                InvalidFlavourQuantity = true;
                            }
                            else
                            {
                                InvalidFlavourQuantity = false;
                                if (flavourquantity > scoopnum)
                                {
                                    Console.WriteLine("Entered quantity more than scoop number. Please try again.");
                                    InvalidFlavourQuantity = true;
                                }
                                totalflavourquantity += flavourquantity;

                                if (totalflavourquantity > scoopnum)
                                {
                                    Console.WriteLine("You've exceeded the scoop number. Please try again.");
                                    InvalidFlavourQuantity = true;
                                    totalflavourquantity -= flavourquantity;
                                }
                            }
                        } while (InvalidFlavourQuantity);

                        Flavour flavour = new Flavour(flavourtype, flavourpremium, flavourquantity);
                        flavours.Add(flavour);
                    }
                    toppings = new List<Topping>();
                    toppingtype = null;
                    while (toppingtype != "nil")
                    {
                        do
                        {
                            Console.WriteLine("Toppings available: ");
                            Console.WriteLine("Sprinkles");
                            Console.WriteLine("Mochi");
                            Console.WriteLine("Sago");
                            Console.WriteLine("Oreos");
                            Console.Write("Enter topping (or nil to stop adding): ");
                            toppingtype = Console.ReadLine();
                            if (string.IsNullOrWhiteSpace(toppingtype))
                            {
                                Console.WriteLine("Invalid Topping Type! Please enter a valid Topping Type.");
                                InvalidToppingType = true;
                            }
                            else
                            {
                                InvalidToppingType= false;
                                if (Toppings.Contains(toppingtype))
                                {
                                    continue;
                                }
                                else if (toppingtype == "nil")
                                {
                                    continue;
                                }
                                else
                                {
                                    Console.WriteLine("Topping entered not in menu. Try again!");
                                    InvalidToppingType = true;
                                }
                            }
                        } while (InvalidToppingType);
                        if (toppingtype != "nil" && toppings.Count < 4)
                        {
                            Topping topping = new Topping(toppingtype);
                            toppings.Add(topping);
                        }
                        else if (toppings.Count == 4)
                        {
                            Console.WriteLine("Max topping amount reached.");
                            toppingtype = "nil";
                        }
                    }

                    iceCream = null;
                    switch (option)
                    {
                        case "Waffle":
                            do
                            {
                                Console.WriteLine("Waffle Flavours available: ");
                                Console.WriteLine("Red Velvet");
                                Console.WriteLine("Charcoal");
                                Console.WriteLine("Pandan");
                                Console.Write("Enter waffle flavour: ");
                                waffleflavour = Console.ReadLine();
                                if (string.IsNullOrWhiteSpace(waffleflavour))
                                {
                                    Console.WriteLine("Waffle Flavour entered invalid. Try again!");
                                    InvalidWaffleFlavour = true;
                                }
                                else
                                {
                                    InvalidWaffleFlavour = false;
                                    if (waffleFlavours.Contains(waffleflavour))
                                    {
                                        continue;
                                    }
                                    else
                                    {
                                        Console.WriteLine("Waffle Flavour entered is not on the menu. Try again!");
                                        InvalidWaffleFlavour = true;
                                    }
                                }
                            } while (InvalidWaffleFlavour);

                            iceCream = new Waffle("Waffle", scoopnum, flavours, toppings, waffleflavour);
                            break;
                        case "Cone":
                            do
                            {
                                Console.Write("Is cone dipped? (True/False): ");
                                if (!bool.TryParse(Console.ReadLine(), out dipped))
                                {
                                    Console.WriteLine("Entered invalid input. Try again.");
                                    InvalidDipped = true;
                                }
                                else
                                {
                                    InvalidDipped = false;
                                }
                            } while (InvalidDipped);
                            iceCream = new Cone("Cone", scoopnum, flavours, toppings, dipped);
                            break;
                        case "Cup":
                            iceCream = new Cup("Cup", scoopnum, flavours, toppings);
                            break;
                    }
                    order.AddIceCream(iceCream);

                    Console.Write("Add Another Ice Cream? [Y/N]: ");
                    addicecream = Console.ReadLine();
                }
                CurrentOrder = order;
                order.TimeReceived = DateTime.Now;
                return order;
            }
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
