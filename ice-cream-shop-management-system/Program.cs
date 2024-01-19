//==========================================================
// Student Number : S10257093
// Student Name : Isabelle Tan
// Partner Name : Charlotte Lee
//==========================================================

//Toppings and Flavours
using ice_cream_shop_management_system;
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
    Console.WriteLine("Current Orders: ");
    foreach (Customer customer in customers.Values)
    {
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
        Console.WriteLine("Customer resgistered successfully!");
        Console.WriteLine();
    }
    else
    {
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

//advanced features - a


//advanced features - b - display monthly charged amounts breakdown & total charged amounts for the year
void DisplayChargedAmounts(Dictionary<int, Order> orders)
{
    Console.Write("Enter the year: ");
    int year = Convert.ToInt32(Console.ReadLine());
    SortedList<DateTime, Order> OrdersInYear = new SortedList<DateTime, Order>();
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
    float janTotal;
    float febTotal;
    float marTotal;
    float aprTotal;
    float mayTotal;
    float junTotal;
    float julTotal;
    float augTotal;
    float sepTotal;
    float octTotal;
    float novTotal;
    float decTotal;
    float overallTotal;
    
}
