﻿using System;
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
        public void ModifyIceCream(int orderid,string[] premFlavours,string[] regFlavours, string[] Toppings, string[] waffleFlavours)
        {
            bool InvalidOption = false;
            bool InvalidScoopNum = false;
            bool InvalidFlavourType = false;
            bool InvalidFlavourQuantity = false;
            bool InvalidToppingType = false;
            bool InvalidWaffleFlavour = false;
            bool InvalidDipped = false;
            string option;
            int scoopnum;
            string flavourtype;
            bool flavourpremium = false;
            int flavourquantity = 0;
            string toppingtype = null;
            string waffleflavour;
            bool dipped = false;

            int orderno = orderid - 1;
            do
            {
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
                }
            } while (InvalidOption);

            do
            {
                Console.Write("Enter number of scoops: ");
                if (!int.TryParse(Console.ReadLine(), out scoopnum))
                {
                    Console.WriteLine("Invalid option! Please enter a valid option.");
                    InvalidScoopNum = true;
                }
                else
                {
                    InvalidScoopNum= false;
                }
            } while (InvalidScoopNum);

            List<Flavour> flavours = new List<Flavour>();

            while (flavourquantity < scoopnum)
            {
                do
                {
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
                            InvalidToppingType = true;
                        }
                    }
                } while (InvalidToppingType);
                Topping topping = new Topping(toppingtype);
                toppings.Add(topping);
            }

            IceCream iceCream = null;
            switch (option)
            {
                case "Waffle":
                    do
                    {
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
