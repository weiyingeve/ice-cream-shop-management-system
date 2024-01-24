﻿//==========================================================
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
    Console.WriteLine("-------------Menu-------------");
    Console.WriteLine("[1] List all customers");
    Console.WriteLine("[2] List all current orders");
    Console.WriteLine("[3] Register a new customer");
    Console.WriteLine("[4] Create a customer's order");
    Console.WriteLine("[5] Display order details of a customer");
    Console.WriteLine("[6] Modify order details");
    Console.WriteLine("[0] Exit");
    Console.WriteLine();
    Console.Write("Enter your choice: ");
    int option = Convert.ToInt32(Console.ReadLine());
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
    Console.Write("Enter Name: ");
    string name = Console.ReadLine();
    Console.Write("Enter ID Number: ");
    int id = Convert.ToInt32(Console.ReadLine());
    Console.Write("Enter Date of Birth: ");
    DateTime dob = DateTime.Parse(Console.ReadLine());

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
        Console.WriteLine("Customer resgistered successfully!");
        Console.WriteLine();
    }
    else
    {
        Console.WriteLine();
        Console.WriteLine("Customer resistration failed!");
        Console.WriteLine();
    }
}

//basic feature 4 - Create customer's order
void CreateCustomerOrder(Dictionary<int, Customer> customers, Dictionary<int, Order> orders)
{
    ListAllCustomers(customers);
    Console.WriteLine();
    Console.Write("Enter your customer ID: ");
    int id = Convert.ToInt32(Console.ReadLine());
    Customer customer = customers[id];

    Order order = new Order();

    Console.Write("Enter option: ");
    string option = Console.ReadLine();
    Console.Write("Enter number of scoops: ");
    int scoopnum = Convert.ToInt32(Console.ReadLine());
    List<Flavour> flavours = new List<Flavour>();
    Console.Write("Enter flavour type (or nil to stop adding): ");
    string flavourtype = Console.ReadLine();
    while (flavourtype != "nil")
    {
        Console.Write("Is it premium? (True/False): ");
        bool flavourpremium = Convert.ToBoolean(Console.ReadLine());
        Console.Write("Enter flavour quantity: ");
        int flavourquantity = Convert.ToInt32(Console.ReadLine());
        Flavour flavour = new Flavour(flavourtype, flavourpremium, flavourquantity);
        flavours.Add(flavour);
        Console.Write("Enter flavour (or nil to stop adding): ");
        flavourtype = Console.ReadLine();
    }
    List<Topping> toppings = new List<Topping>();
    Console.Write("Enter topping (or nil to stop adding): ");
    string toppingtype = Console.ReadLine();
    while (toppingtype != "nil")
    {
        Topping topping = new Topping(toppingtype);
        toppings.Add(topping);
        Console.Write("Enter topping (or nil to stop adding): ");
        toppingtype = Console.ReadLine();
    }

    IceCream iceCream = null;
    switch (option)
    {
        case "Waffle":
            Console.Write("Enter waffle flavour: ");
            string waffleflavour = Console.ReadLine();
            iceCream = new Waffle("Waffle", scoopnum, flavours, toppings, waffleflavour);
            break;
        case "Cone":
            Console.Write("Is cone dipped? (True/False): ");
            bool dipped = Convert.ToBoolean(Console.ReadLine());
            iceCream = new Cone("Cone", scoopnum, flavours, toppings, dipped);
            break;
        case "Cup":
            iceCream = new Cup("Cup", scoopnum, flavours, toppings);
            break;
    }
    order.AddIceCream(iceCream);

    Console.Write("Add Another Ice Cream? [Y/N]: ");
    string addicecream = Console.ReadLine();
    while (addicecream == "Y")
    {
        Console.Write("Enter option: ");
        option = Console.ReadLine();
        Console.Write("Enter number of scoops: ");
        scoopnum = Convert.ToInt32(Console.ReadLine());
        flavours = new List<Flavour>();
        Console.Write("Enter flavour type (or nil to stop adding): ");
        flavourtype = Console.ReadLine();
        while (flavourtype != "nil")
        {
            Console.Write("Is it premium? (True/False): ");
            bool flavourpremium = Convert.ToBoolean(Console.ReadLine());
            Console.Write("Enter flavour quantity: ");
            int flavourquantity = Convert.ToInt32(Console.ReadLine());
            Flavour flavour = new Flavour(flavourtype, flavourpremium, flavourquantity);
            flavours.Add(flavour);
            Console.Write("Enter flavour (or nil to stop adding): ");
            flavourtype = Console.ReadLine();
        }
        toppings = new List<Topping>();
        Console.Write("Enter topping (or nil to stop adding): ");
        toppingtype = Console.ReadLine();
        while (toppingtype != "nil")
        {
            Topping topping = new Topping(toppingtype);
            toppings.Add(topping);
            Console.Write("Enter topping (or nil to stop adding): ");
            toppingtype = Console.ReadLine();
        }

        iceCream = null;
        switch (option)
        {
            case "Waffle":
                Console.Write("Enter waffle flavour: ");
                string waffleflavour = Console.ReadLine();
                iceCream = new Waffle("Waffle", scoopnum, flavours, toppings, waffleflavour);
                break;
            case "Cone":
                Console.Write("Is cone dipped? (True/False): ");
                bool dipped = Convert.ToBoolean(Console.ReadLine());
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
    ListAllCustomers(customers);
    Console.WriteLine();
    Console.Write("Enter your customer ID: ");
    int id = Convert.ToInt32(Console.ReadLine());
    Customer customer = customers[id];

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
    ListAllCustomers(customers);
    Console.WriteLine();
    Console.Write("Enter your customer ID: ");
    int id = Convert.ToInt32(Console.ReadLine());
    Customer customer = customers[id];
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
        Console.WriteLine();
        Console.WriteLine("Choose one of the following options: ");
        Console.WriteLine("[1] Choose an existing ice cream object to modify.");
        Console.WriteLine("[2] Add an entirely new ice cream object to the order.");
        Console.WriteLine("[3] Choose an existing ice cream object to delete from the order.");
        Console.Write("Enter option: ");
        int option = Convert.ToInt32(Console.ReadLine());
        Console.WriteLine();
        if (option == 1)
        {
            Console.WriteLine("Enter which ice cream to modify: ");
            int IceCreamNo = Convert.ToInt32(Console.ReadLine());
            currentOrder.ModifyIceCream(IceCreamNo);
        }
        else if (option == 2)
        {
            Console.WriteLine("Enter ice cream option: ");
            string icecreamoption = Console.ReadLine();
            Console.WriteLine("Enter number of scoops: ");
            int scoopnum = Convert.ToInt32(Console.ReadLine());
            List<Flavour> flavours = new List<Flavour>();
            Console.WriteLine("Enter flavour type (or nil to stop adding): ");
            string flavourtype = Console.ReadLine();
            while (flavourtype != "nil")
            {
                Console.WriteLine("Is it premium? (True/False): ");
                bool flavourpremium = Convert.ToBoolean(Console.ReadLine());
                Console.WriteLine("Enter flavour quantity: ");
                int flavourquantity = Convert.ToInt32(Console.ReadLine());
                Flavour flavour = new Flavour(flavourtype, flavourpremium, flavourquantity);
                flavours.Add(flavour);
                Console.WriteLine("Enter flavour (or nil to stop adding): ");
                flavourtype = Console.ReadLine();
            }
            List<Topping> toppings = new List<Topping>();
            Console.WriteLine("Enter topping (or nil to stop adding): ");
            string toppingtype = Console.ReadLine();
            while (toppingtype != "nil")
            {
                Topping topping = new Topping(toppingtype);
                toppings.Add(topping);
                Console.WriteLine("Enter topping (or nil to stop adding): ");
                toppingtype = Console.ReadLine();
            }

            IceCream iceCream = null;
            switch (icecreamoption)
            {
                case "Waffle":
                    Console.Write("Enter waffle flavour: ");
                    string waffleflavour = Console.ReadLine();
                    iceCream = new Waffle("Waffle", scoopnum, flavours, toppings, waffleflavour);
                    break;
                case "Cone":
                    Console.Write("Is cone dipped? (True/False): ");
                    bool dipped = Convert.ToBoolean(Console.ReadLine());
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
            Console.WriteLine("Enter which ice cream to delete: ");
            int IceCreamNo = Convert.ToInt32(Console.ReadLine()) - 1;
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
    Order order = queue.Dequeue();
    
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
                    Console.Write("Enter number of points to redeem: ");
                    int redeempoints = Convert.ToInt32(Console.ReadLine());
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
    Console.Write("Enter the year: ");
    int year = Convert.ToInt32(Console.ReadLine());
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
    else
    {
        ModifyOrderDetails(customers);
    }
    Console.WriteLine();
    option = DisplayMenu();
}

ProcessOrderAndCheckout(goldqueue, customers);
DisplayChargedAmounts(orders);
