//==========================================================
// Student Number : S10257093
// Student Name : Isabelle Tan
// Partner Name : Charlotte Lee
//==========================================================

//Toppings and Flavours
using ice_cream_shop_management_system;

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
            customer.CurrentOrder != null ? customer.CurrentOrder.Id.ToString() : "No Current Order",
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
    }
}
