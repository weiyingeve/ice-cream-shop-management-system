//==========================================================
// Student Number : S10257093
// Student Name : Isabelle Tan
// Partner Name : Charlotte Lee
//==========================================================

//Toppings and Flavours
using ice_cream_shop_management_system;
using System.Collections;
using System.ComponentModel;
using System.Data;

string[] Toppings = { "Sprinkles", "Mochi", "Sago", "Oreos" };
string[] regFlavours = { "Vanilla", "Chocolate", "Strawberry" };
string[] premFlavours = { "Durian", "Ube", "Sea Salt" };
string[] waffleFlavours = {"Red Velvet", "Charcoal", "Pandan"};
string[] Options = { "Cup", "Cone", "Waffle" };

//Customers
Dictionary<int, Customer> customers = new Dictionary<int, Customer>();
string[] CustomerCsvLines = File.ReadAllLines("customers.csv");
for (int i = 1; i < CustomerCsvLines.Length; i++)
{
    string[] line = CustomerCsvLines[i].Split(",");
    Customer customer = new Customer(line[0], Convert.ToInt32(line[1]), DateTime.Parse(line[2]));
    PointCard pointCard = new PointCard(Convert.ToInt32(line[4]), Convert.ToInt32(line[5]));
    pointCard.Tier = line[3];
    customer.Rewards = pointCard;
    customers.Add(customer.MemberId, customer);
}

//Orders
Dictionary<int, Order> orders = new Dictionary<int, Order>();
Dictionary<int, int> orderIDCustomerID = new Dictionary<int, int>();
string[] ordersFile = File.ReadAllLines("orders.csv");
for (int i = 1; i < ordersFile.Length; i++)
{
    string[] line = ordersFile[i].Split(",");
    int orderID = Convert.ToInt32(line[0]);

    //add and retrieve orders
    Order order;
    if (orders.Keys.Contains(orderID))
    {
        order = orders[orderID];
    }
    else
    {
        order = new Order(orderID, DateTime.Parse(line[2]));
        order.TimeFulfilled = DateTime.Parse(line[3]);
        orders.Add(orderID, order);
        orderIDCustomerID.Add(orderID, Convert.ToInt32(line[1]));
    }

    // add ice cream to order
    string TypeofIceCream = line[4];
    // getting the ice cream flavours
    int ScoopsNum = Convert.ToInt32(line[5]);
    List<Flavour> flavours = new List<Flavour>();
    for (int x = 8; x < 8 + ScoopsNum; x++)
    {
        Flavour TypeofFlavour = flavours.Find(f => f.Type == line[x]);
        if (TypeofFlavour == null)
        {
            flavours.Add(new Flavour(line[x], premFlavours.Contains(line[x]), 1));
        }
        else
        {
            TypeofFlavour.Quantity += 1;
        }
    }

    //Get toppings
    List<Topping> toppings = new List<Topping>();
    for (int x = 11; x < 15; x++)
    {
        if (line[x] != "")
        {
            toppings.Add(new Topping(line[x]));
        }
    }

    //Create Ice Cream
    IceCream iceCream = null;
    switch (TypeofIceCream)
    {
        case "Waffle":
            iceCream = new Waffle("Waffle", ScoopsNum, flavours, toppings, line[7]);
            break;
        case "Cone":
            iceCream = new Cone("Cone", ScoopsNum, flavours, toppings, line[6] == "TRUE");
            break;
        case "Cup":
            iceCream = new Cup("Cup", ScoopsNum, flavours, toppings);
            break;
    }
    order.AddIceCream(iceCream);
}

//Add Order to Customer
foreach (int orderID in orderIDCustomerID.Keys)
{
    int customerID = orderIDCustomerID[orderID];
    Customer customer = customers[customerID];
    customer.OrderHistory.Add(orders[orderID]);
}

//Order Queue
Queue<Order> orderqueue = new Queue<Order>();

//Order Queue
Queue<Order> goldqueue = new Queue<Order>();

//Menu
int DisplayMenu()
{
    bool InvalidOPTION = false;
    int option;

    Console.WriteLine("-------------Menu-------------");
    Console.WriteLine("[1] List all customers");
    Console.WriteLine("[2] List all current orders");
    Console.WriteLine("[3] Register a new customer");
    Console.WriteLine("[4] Create a customer's order");
    Console.WriteLine("[5] Display order details of a customer");
    Console.WriteLine("[6] Modify order details");
    Console.WriteLine("[7] Process an order and checkout");
    Console.WriteLine("[8] Display monthly charged amounts breakdown & total charged amounts for the year");
    Console.WriteLine("[0] Exit");
    Console.WriteLine();
    do
    {
        Console.Write("Enter your choice: ");
        if (!int.TryParse(Console.ReadLine(), out option))
        {
            Console.WriteLine("Invalid input! Please try again.");
            InvalidOPTION = true;
        }
        else
        {
            InvalidOPTION = false;
        }
    } while (InvalidOPTION);
    return option;
}

//basic feature 1 - List all customers
void ListAllCustomers(Dictionary<int, Customer> customers)
{
    Console.WriteLine("{0, -10} {1, -10} {2, -15} {3, -20} {4, -15} {5, -10} {6, -10} {7,-10}",
        "Name", "MemberID", "DateOfBirth", "CurrentOrderID", "OrderHistoryID", "Points",
        "PunchCard", "Tier");

    foreach (Customer customer in customers.Values)
    {
        Console.WriteLine("{0, -10} {1, -10} {2, -15:dd/MM/yyyy} {3, -20} {4, -15} {5, -10} {6, -10} {7, -10}",
            customer.Name, customer.MemberId, customer.Dob,
            customer.CurrentOrder != null ? customer.CurrentOrder.Id.ToString() + 1 : "No Current Order",
            string.Join(", ", customer.OrderHistory.Select(order => order.Id)),
            customer.Rewards.Points,
            customer.Rewards.PunchCard,
            customer.Rewards.Tier);
    }
    Console.WriteLine();
}

//basic feature 2 - List all current orders
void ListAllOrders(Dictionary<int, Customer> customers)
{
    Console.WriteLine();
    if (goldqueue.Count > 0)
    {
        Console.WriteLine("Orders In Gold Queue: ");
        foreach (Order goldorder in goldqueue)
        {
            Console.WriteLine();
            Console.WriteLine($"Order ID: {goldorder.Id}");
            Console.WriteLine($"Time Received: {goldorder.TimeReceived}");
            foreach (IceCream iceCream in goldorder.IceCreamList)
            {
                Console.WriteLine($"Ice cream option: {iceCream.Option}");
                Console.WriteLine($"Ice cream scoops: {iceCream.Scoops}");
                Console.WriteLine("Ice cream flavours: ");
                foreach (Flavour flav in iceCream.Flavours)
                {
                    Console.WriteLine(flav.ToString());
                }
                Console.WriteLine("Ice cream toppings: ");
                foreach (Topping topping in iceCream.Toppings)
                {
                    Console.WriteLine(topping.ToString());
                }
            }
            Console.WriteLine();
        }
    }
    else
    {
        Console.WriteLine("No orders in gold queue.");
    }
    
    Console.WriteLine();

    if (orderqueue.Count > 0) 
    {
        Console.WriteLine("Orders In Regular Queue: ");
        foreach (Order order in orderqueue)
        {
            Console.WriteLine();
            Console.WriteLine($"Order ID: {order.Id}");
            Console.WriteLine($"Time Received: {order.TimeReceived}");
            foreach (IceCream iceCream in order.IceCreamList)
            {
                Console.WriteLine($"Ice cream option: {iceCream.Option}");
                Console.WriteLine($"Ice cream scoops: {iceCream.Scoops}");
                Console.WriteLine("Ice cream flavours: ");
                foreach (Flavour flav in iceCream.Flavours)
                {
                    Console.WriteLine(flav.ToString());
                }
                Console.WriteLine("Ice cream toppings: ");
                foreach (Topping topping in iceCream.Toppings)
                {
                    Console.WriteLine(topping.ToString());
                }
            }
            Console.WriteLine();
        }
    }
    else
    {
        Console.WriteLine("No orders in regular queue.");
    }
    Console.WriteLine();
}

//basic feature 3 - Register a new customer
void NewCustomer(Dictionary<int, Customer> customers)
{
    bool InvalidName = false;
    bool InvalidID = false;
    bool InvalidDOB = false;
    string name;
    int id;
    DateTime dob;


    do
    {
        Console.Write("Enter Name: ");
        name = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(name))
        {
            Console.WriteLine("Invalid name. Please enter a valid name.");
            InvalidName = true;
        }
        else
        {
            InvalidName = false;

        }
    } while (InvalidName);

    do
    {
        Console.Write("Enter ID Number (XXXXXX): ");
        if (!int.TryParse(Console.ReadLine(), out id))
        {
            Console.WriteLine("Invalid ID format. Please enter a valid integer.");
            InvalidID = true;
        }
        else
        {
            InvalidID = false;
            if (id < 0 || id > 999999)
            {
                Console.WriteLine("ID entered out of range. Please try again.");
                InvalidID = true;
            }
            else if (customers.ContainsKey(id))
            {
                Console.WriteLine("Customer ID already exists. Try again.");
                InvalidID = true;
            }
            else
            {
                continue;
            }
        }
    } while (InvalidID);

    do
    {
        Console.Write("Enter Date of Birth: ");
        if (!DateTime.TryParse(Console.ReadLine(), out dob))
        {
            Console.WriteLine("Invalid Date of Birth format. Please enter a valid date (dd/MM/yyyy).");
            InvalidDOB = true;
        }
        else
        {
            InvalidDOB = false;
            if (DateTime.Compare(DateTime.Now, dob) < 0)
            {
                Console.WriteLine("Date cannot be in the future. Please try again.");
                InvalidDOB = true;
            }
            else continue;
        }
    } while (InvalidDOB);
    
    Customer customer = new Customer(name, id, dob);
    PointCard pointCard = new PointCard();
    pointCard.Tier = "Ordinary";
    customer.Rewards = pointCard;
    customers.Add(id, customer);
    string data = name + "," + id + "," + dob.ToString();

    using (StreamWriter sw = new StreamWriter("customers.csv", true))
    {
        sw.WriteLine(data);
    }

    if (customers.ContainsKey(id))
    {
        Console.WriteLine();
        Console.WriteLine("Customer registered successfully!");
        Console.WriteLine();
    }
    else
    {
        Console.WriteLine();
        Console.WriteLine("Customer registration failed!");
        Console.WriteLine();
    }
}

//basic feature 4 - Create customer's order
void CreateCustomerOrder(Dictionary<int, Customer> customers, Dictionary<int, Order> orders)
{
    bool InvalidID = false;
    bool InvalidOption = false;
    bool InvalidScoopNum = false;
    bool InvalidFlavourType = false;
    bool InvalidFlavourQuantity = false;
    bool InvalidToppingType = false;
    bool InvalidWaffleFlavour = false;
    bool InvalidDipped = false;
    bool InvalidAddIceCream = false;
    int id;
    string option;
    int scoopnum;
    string flavourtype;
    bool flavourpremium = false;
    int flavourquantity = 0;
    string toppingtype = null;
    string waffleflavour;
    bool dipped = false;
    string addicecream;
    Customer customer = null;

    ListAllCustomers(customers);
    Console.WriteLine();
    do
    {
        Console.Write("Enter your customer ID: ");
        if (!int.TryParse(Console.ReadLine(), out id))
        {
            Console.WriteLine("Invalid ID format. Please enter a valid integer.");
            InvalidID = true;
        }
        else
        {
            InvalidID = false;
            if (customers.ContainsKey(id))
            {
                customer = customers[id];
            }
            else
            {
                Console.WriteLine("ID entered is invalid! Please try again.");
                InvalidID = true;
            }
        }
    } while (InvalidID);
    
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
            if (scoopnum > 4 || scoopnum < 0)
            {
                Console.WriteLine("Invalid scoop number! Please enter a valid option.");
                InvalidScoopNum = true;
            }
            else continue;
        }
    } while (InvalidScoopNum);
    
    List<Flavour> flavours = new List<Flavour>();
    
    while (flavourquantity < scoopnum)
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
                    Console.WriteLine("Topping entered not in menu. Try again!");
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
            }while (InvalidDipped);
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
                if (option.Contains(option))
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

        flavours = new List<Flavour>();

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
        toppings = new List<Topping>();
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
                        Console.WriteLine("Topping entered not in menu. Try again!");
                        InvalidToppingType = true;
                    }
                }
            } while (InvalidToppingType);
            Topping topping = new Topping(toppingtype);
            toppings.Add(topping);
        }

        iceCream = null;
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
        order.AddIceCream(iceCream);
    }

    customer.CurrentOrder = order;
    orders.Add(orders.Count + 1, order);

    if (customer.Rewards.Tier == "Gold")
    {
        goldqueue.Enqueue(order);
    }
    else
    {
        orderqueue.Enqueue(order);
    }

    if (orders.ContainsValue(order))
    {
        Console.WriteLine("Order made successfully!");
    }
    else
    {
        Console.WriteLine("Order made failed!");
    }
}

//basic feature 5 - Display order details of a customer
void DisplayOrderDetailsOfCustomer(Dictionary<int, Customer> customers)
{
    bool InvalidID;
    int id;
    Customer customer = null;

    ListAllCustomers(customers);
    Console.WriteLine();
    do
    {
        Console.Write("Enter your customer ID: ");
        if (!int.TryParse(Console.ReadLine(), out id))
        {
            Console.WriteLine("Invalid ID format. Please enter a valid integer.");
            InvalidID = true;
        }
        else
        {
            InvalidID = false;
            if (customers.ContainsKey(id))
            {
                customer = customers[id];
            }
            else
            {
                Console.WriteLine("ID entered is invalid! Please try again.");
                InvalidID = true;
            }
        }
    } while (InvalidID);

    Console.WriteLine();
    foreach (Order order in customer.OrderHistory)
    {
        if (order != null)
        {
            Console.WriteLine($"Time Received: {order.TimeReceived}");
            foreach (IceCream iceCream in order.IceCreamList)
            {
                Console.WriteLine($"Ice cream option: {iceCream.Option}");
                Console.WriteLine($"Ice cream scoops: {iceCream.Scoops}");
                Console.WriteLine("Ice cream flavours: ");
                foreach (Flavour flav in iceCream.Flavours)
                {
                    Console.WriteLine(flav.ToString());
                }
                Console.WriteLine("Ice cream toppings: ");
                foreach (Topping topping in iceCream.Toppings)
                {
                    Console.WriteLine(topping.ToString());
                }
            }
            Console.WriteLine();
        }
        else
        {
            Console.WriteLine("No previous orders.");
            break;
        }
    }
    if (customer.CurrentOrder != null)
    {
        Console.WriteLine($"{customer.Name}'s Order: ");
        Console.WriteLine($"Order ID: {customer.CurrentOrder.Id}");
        Console.WriteLine($"Time Received: {customer.CurrentOrder.TimeReceived}");
        foreach (IceCream iceCream in customer.CurrentOrder.IceCreamList)
        {
            Console.WriteLine($"Ice cream option: {iceCream.Option}");
            Console.WriteLine($"Ice cream scoops: {iceCream.Scoops}");
            Console.WriteLine("Ice cream flavours: ");
            foreach (Flavour flav in iceCream.Flavours)
            {
                Console.WriteLine(flav.ToString());
            }
            Console.WriteLine("Ice cream toppings: ");
            foreach (Topping topping in iceCream.Toppings)
            {
                Console.WriteLine(topping.ToString());
            }
        }
        Console.WriteLine();
    }
    else
    {
        Console.WriteLine("No current orders.");
    }
}

//basic feature 6 - Modify order details
void ModifyOrderDetails(Dictionary<int, Customer> customers)
{
    bool InvalidID = false;
    bool InvalidOption = false;
    bool InvalidIceCreamNo = false;
    bool InvalidIceCreamOption = false;
    bool InvalidScoopNum = false;
    bool InvalidFlavourType = false;
    bool InvalidFlavourQuantity = false;
    bool InvalidToppingType = false;
    bool InvalidWaffleFlavour = false;
    bool InvalidDipped = false;
    int id;
    Customer customer = null;
    int option = 0;
    int IceCreamNo = 0;
    string icecreamoption;
    int scoopnum;
    string flavourtype;
    bool flavourpremium = false;
    int flavourquantity = 0;
    string toppingtype = null;
    string waffleflavour;
    bool dipped = false;

    ListAllCustomers(customers);
    Console.WriteLine();
    do
    {
        Console.Write("Enter your customer ID: ");
        if (!int.TryParse(Console.ReadLine(), out id))
        {
            Console.WriteLine("Invalid ID format. Please enter a valid integer.");
            InvalidID = true;
        }
        else
        {
            InvalidID = false;
            if (customers.ContainsKey(id))
            {
                customer = customers[id];
            }
            else
            {
                Console.WriteLine("ID entered is invalid! Please try again.");
                InvalidID = true;
            }
        }
    } while (InvalidID);

    if (customer.CurrentOrder != null)
    {
        Order currentOrder = customer.CurrentOrder;
        foreach (IceCream iceCream in customer.CurrentOrder.IceCreamList)
        {
            Console.WriteLine($"Ice cream {customer.CurrentOrder.IceCreamList.IndexOf(iceCream) + 1}");
            Console.WriteLine($"Ice cream option: {iceCream.Option}");
            Console.WriteLine($"Ice cream scoops: {iceCream.Scoops}");
            Console.WriteLine("Ice cream flavours: ");
            foreach (Flavour flav in iceCream.Flavours)
            {
                Console.WriteLine(flav.ToString());
            }
            Console.WriteLine("Ice cream toppings: ");
            foreach (Topping topping in iceCream.Toppings)
            {
                Console.WriteLine(topping.ToString());
            }
        }

        do
        {
            Console.WriteLine();
            Console.WriteLine("Choose one of the following options: ");
            Console.WriteLine("[1] Choose an existing ice cream object to modify.");
            Console.WriteLine("[2] Add an entirely new ice cream object to the order.");
            Console.WriteLine("[3] Choose an existing ice cream object to delete from the order.");
            Console.Write("Enter option: ");
            if (!int.TryParse(Console.ReadLine(), out option))
            {
                Console.WriteLine("Option entered is invalid. Try again!");
                InvalidOption = true;
            }
            else
            {
                InvalidOption = false;
                if (id != 1 || id != 2 || id != 3)
                {
                    Console.WriteLine("Option entered is invalid. Try again!");
                    InvalidOption = true;
                }
                else continue;
            }

        } while (InvalidOption);
        

        Console.WriteLine();
        if (option == 1)
        {
            do
            {
                Console.WriteLine("Enter which ice cream to modify: ");
                if (!int.TryParse(Console.ReadLine(), out IceCreamNo))
                {
                    Console.WriteLine("Value inputted is not an integer. Try again.");
                    InvalidIceCreamNo = true;
                }
                else
                {
                    InvalidIceCreamNo = false;
                }

            } while (InvalidIceCreamNo);
            currentOrder.ModifyIceCream(IceCreamNo, Options, premFlavours, regFlavours, Toppings, waffleFlavours);
        }
        else if (option == 2)
        {
            do
            {
                Console.Write("Enter option: ");
                icecreamoption = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(icecreamoption))
                {
                    Console.WriteLine("Invalid option! Please enter a valid option.");
                    InvalidOption = true;
                }
                else
                {
                    InvalidOption = false;
                    if (Options.Contains(icecreamoption))
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
                            Console.WriteLine("Topping entered not in menu. Try again!");
                            InvalidToppingType = true;
                        }
                    }
                } while (InvalidToppingType);
                Topping topping = new Topping(toppingtype);
                toppings.Add(topping);
            }

            IceCream iceCream = null;
            switch (icecreamoption)
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
            currentOrder.AddIceCream(iceCream);
        }
        else if (option == 3)
        {
            do
            {
                Console.WriteLine("Enter which ice cream to delete: ");
                if (!int.TryParse(Console.ReadLine(), out IceCreamNo))
                {
                    Console.WriteLine("Value inputted is not an integer. Try again.");
                    InvalidIceCreamNo = true;
                }
                else
                {
                    InvalidIceCreamNo = false;
                }

            } while (InvalidIceCreamNo);

            IceCreamNo -= 1;
            if (currentOrder.IceCreamList.Count > 1)
            {
                currentOrder.DeleteIceCream(IceCreamNo);
            }
            else
            {
                Console.WriteLine("Cannot have zero ice creams in an order.");
            }
        }
        else
        {
            Console.WriteLine("Option does not exist.");
        }
    }
    else
    {
        Console.WriteLine("No current orders.");
        return;
    }
    foreach (IceCream iceCream in customer.CurrentOrder.IceCreamList)
    {
        Console.WriteLine($"Ice cream {customer.CurrentOrder.IceCreamList.IndexOf(iceCream) + 1}");
        Console.WriteLine($"Ice cream option: {iceCream.Option}");
        Console.WriteLine($"Ice cream scoops: {iceCream.Scoops}");
        Console.WriteLine("Ice cream flavours: ");
        foreach (Flavour flav in iceCream.Flavours)
        {
            Console.WriteLine(flav.ToString());
        }
        Console.WriteLine("Ice cream toppings: ");
        foreach (Topping topping in iceCream.Toppings)
        {
            Console.WriteLine(topping.ToString());
        }
    }
}

//advanced features - a - Process an order and checkout
void ProcessOrderAndCheckout(Queue<Order> queue, Dictionary<int, Customer> customer)
{
    bool InvalidRedeemPoints = false;
    int redeempoints = 0;
    Order order = null;

    if (queue.Count > 0)
    {
        order = queue.Dequeue();
    }
    else
    {
        Console.WriteLine("No current orders in queue.");
        return;
    }
    
    
    foreach (IceCream icecream in order.IceCreamList)
    {
        Console.WriteLine($"Ice cream {order.IceCreamList.IndexOf(icecream) + 1}");
        Console.WriteLine($"Ice cream option: {icecream.Option}");
        Console.WriteLine($"Ice cream scoops: {icecream.Scoops}");
        Console.WriteLine("Ice cream flavours: ");
        foreach (Flavour flav in icecream.Flavours)
        {
            Console.WriteLine(flav.ToString());
        }
        Console.WriteLine("Ice cream toppings: ");
        foreach (Topping topping in icecream.Toppings)
        {
            Console.WriteLine(topping.ToString());
        }
    }

    double billamt = order.CalculateTotal();
    Console.WriteLine($"Total Bill Amount: ${billamt.ToString("0.00")}");

    foreach (Customer c in customers.Values)
    {
        if (c.CurrentOrder == order)
        {
            Console.WriteLine($"Membership status: {c.Rewards.Tier}");
            Console.WriteLine($"Points: {c.Rewards.Points}");
            if (c.IsBirthday())
            {
                if (order.IceCreamList.Count > 1)
                {
                    double mostExp = 0;
                    foreach (IceCream i in order.IceCreamList)
                    {
                        if (i.CalculatePrice() > mostExp)
                        {
                            mostExp = i.CalculatePrice();
                        }
                    }
                    billamt -= mostExp;
                }
                else
                {
                    billamt = 0;
                }
            }

            if (c.Rewards.PunchCard == 10)
            {
                billamt -= order.IceCreamList[0].CalculatePrice();
                c.Rewards.PunchCard = 0;
            }

            if (c.Rewards.Tier == "Gold" || c.Rewards.Tier == "Silver")
            {
                if (c.Rewards.Points > 0)
                {
                    do
                    {
                        Console.Write("Enter number of points to redeem: ");
                        if (!int.TryParse(Console.ReadLine(), out redeempoints))
                        {
                            Console.WriteLine("Enter a valid integer.");
                            InvalidRedeemPoints = true;
                        }
                        else
                        {
                            InvalidRedeemPoints = false;
                        }
                    } while (InvalidRedeemPoints);
                    
                    c.Rewards.RedeemPoints(redeempoints);
                    billamt -= redeempoints * 0.02;
                }
            }

            Console.WriteLine($"Final Bill Amount: ${billamt.ToString("0.00")}");
            Console.Write("Press any key to make payment: ");
            var payment = Console.ReadLine();

            foreach (IceCream ic in order.IceCreamList)
            {
                c.Rewards.Punch();
            }
            int earnedpoints = Convert.ToInt32(Math.Floor(billamt * 0.72));
            c.Rewards.AddPoints(earnedpoints);
            if (c.Rewards.Points >= 100)
            {
                c.Rewards.Tier = "Gold";
            }
            else if (c.Rewards.Points >= 50 && c.Rewards.Tier != "Gold")
            {
                c.Rewards.Tier = "Silver";
            }
            order.TimeFulfilled = DateTime.Now;
            c.CurrentOrder = null;
            c.OrderHistory.Add(order);
        }
        else continue;
    }
}

//advanced features - b - display monthly charged amounts breakdown & total charged amounts for the year
void DisplayChargedAmounts(Dictionary<int, Order> orders)
{
    bool InvalidYear = false;
    int year = 0;

    do
    {
        Console.Write("Enter the year: ");
        if (!int.TryParse(Console.ReadLine(), out year))
        {
            Console.WriteLine("Enter a valid integer.");
            InvalidYear = true;
        }
        else
        {
            InvalidYear = false;
        }
    } while (InvalidYear);

    Dictionary<DateTime, Order> OrdersInYear = new Dictionary<DateTime, Order>();
    foreach (Order order in orders.Values)
    {
        DateTime orderDateTime = (DateTime)order.TimeFulfilled;
        int orderyear = orderDateTime.Year;
        if (orderyear == year)
        {
            OrdersInYear.Add(orderDateTime, order);
        }
        else continue;
    }
    double janTotal = 0;
    double febTotal = 0;
    double marTotal = 0;
    double aprTotal = 0;
    double mayTotal = 0;
    double junTotal = 0;
    double julTotal = 0;
    double augTotal = 0;
    double sepTotal = 0;
    double octTotal = 0;
    double novTotal = 0;
    double decTotal = 0;
    double overallTotal = 0;

    foreach (KeyValuePair<DateTime, Order> pair in OrdersInYear)
    {
        if (pair.Key.Month == 1)
        {
            janTotal += pair.Value.CalculateTotal();
        }
        else if (pair.Key.Month == 2)
        {
            febTotal += pair.Value.CalculateTotal();
        }
        else if (pair.Key.Month == 3)
        {
            marTotal += pair.Value.CalculateTotal();
        }
        else if (pair.Key.Month == 4)
        {
            aprTotal += pair.Value.CalculateTotal();
        }
        else if (pair.Key.Month == 5)
        {
            mayTotal += pair.Value.CalculateTotal();
        }
        else if (pair.Key.Month == 6)
        {
            junTotal += pair.Value.CalculateTotal();
        }
        else if (pair.Key.Month == 7)
        {
            julTotal += pair.Value.CalculateTotal();
        }
        else if (pair.Key.Month == 8)
        {
            augTotal += pair.Value.CalculateTotal();
        }
        else if (pair.Key.Month == 9)
        {
            sepTotal += pair.Value.CalculateTotal();
        }
        else if (pair.Key.Month == 10)
        {
            octTotal += pair.Value.CalculateTotal();
        }
        else if (pair.Key.Month == 11)
        {
            novTotal += pair.Value.CalculateTotal();
        }
        else
        {
            decTotal += pair.Value.CalculateTotal();
        }
    }
    overallTotal = janTotal + febTotal + marTotal + aprTotal + mayTotal + junTotal +
        julTotal + augTotal + sepTotal + octTotal + novTotal + decTotal;

    Console.WriteLine();
    Console.WriteLine("Jan {0}: ${1}", year, janTotal.ToString("0.00"));
    Console.WriteLine("Feb {0}: ${1}", year, febTotal.ToString("0.00"));
    Console.WriteLine("Mar {0}: ${1}", year, marTotal.ToString("0.00"));
    Console.WriteLine("Apr {0}: ${1}", year, aprTotal.ToString("0.00"));
    Console.WriteLine("May {0}: ${1}", year, mayTotal.ToString("0.00"));
    Console.WriteLine("Jun {0}: ${1}", year, junTotal.ToString("0.00"));
    Console.WriteLine("Jul {0}: ${1}", year, julTotal.ToString("0.00"));
    Console.WriteLine("Aug {0}: ${1}", year, augTotal.ToString("0.00"));
    Console.WriteLine("Sep {0}: ${1}", year, sepTotal.ToString("0.00"));
    Console.WriteLine("Oct {0}: ${1}", year, octTotal.ToString("0.00"));
    Console.WriteLine("Nov {0}: ${1}", year, novTotal.ToString("0.00"));
    Console.WriteLine("Dec {0}: ${1}", year, decTotal.ToString("0.00"));
    Console.WriteLine();
    Console.WriteLine("Total: ${0}", overallTotal.ToString("0.00"));
}


int option = DisplayMenu();
while (option != 0)
{
    if (option == 1)
    {
        ListAllCustomers(customers);
    }
    else if (option == 2)
    {
        ListAllOrders(customers);
    }
    else if (option == 3)
    {
        NewCustomer(customers);
    }
    else if (option == 4)
    {
        CreateCustomerOrder(customers, orders);
    }
    else if (option == 5)
    {
        DisplayOrderDetailsOfCustomer(customers);
    }
    else if (option == 6)
    {
        ModifyOrderDetails(customers);
    }
    else if (option == 7)
    {
        if (goldqueue.Count > 0)
        {
            ProcessOrderAndCheckout(goldqueue, customers);
        }
        else
        {
            ProcessOrderAndCheckout(orderqueue, customers);
        }
    }
    else if (option == 8)
    {
        DisplayChargedAmounts(orders);
    }
    else
    {
        Console.WriteLine("Invalid input! Please enter a valid input.");
    }
    Console.WriteLine();
    option = DisplayMenu();
}