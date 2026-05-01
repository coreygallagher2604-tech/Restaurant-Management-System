using RMS.Data.Entities;
using RMS.Data.Repository;
using RMS.Data.Security;

namespace RMS.Data.Services;

public static class ServiceSeeder
{

    // default seeder using Db versions of services
    public static void Seed()
    {
        IUserService usvc = new UserServiceDb();
        IRestaurantService rsvc = new RestaurantServiceDb();
       
        usvc.Initialise();
        rsvc.Initialise();

        SeedUsers(usvc);
        SeedRestaurant(rsvc);
        SeedTables(rsvc);
        SeedBookings(rsvc);
       // SeedOrders(rsvc);
    }

    // use this method FIRST to seed the database with dummy test data using an IUserService
    private static void SeedUsers(IUserService svc)
    {
        // System admin — full access to everything including system config
        svc.Register("admin", "admin@rms.com", "password", Role.admin);

        // Owner/boss — full restaurant access, can void orders, manage staff
        svc.Register("owner", "owner@rms.com", "password", Role.owner);

        // Manager — can manage menus, ingredients, bookings, view orders
        svc.Register("manager", "manager@rms.com", "password", Role.manager);

        // Staff — can take orders, manage bookings, mark order stages
        svc.Register("staff", "staff@rms.com", "password", Role.staff);

        // Registered guest — can make bookings, view menus
        svc.Register("guest", "guest@rms.com", "password", Role.guest);
    }
    
    // use this method SECOND to seed the database with dummy test data using an IRestaurantService
    private static void SeedRestaurant(IRestaurantService svc)
    
    {        
        // Note: do not call initialise here
        var milk = svc.AddIngredient("Milk", true, "Contains lactose");
        var eggs = svc.AddIngredient("Eggs", true, "Contains egg proteins");
        var gluten = svc.AddIngredient("Gluten", true, "Contains gluten");
        var crustaceans = svc.AddIngredient("Crustaceans", true, "Contains crustacean proteins");
        var fish = svc.AddIngredient("Fish", true, "Contains fish proteins");
        var treeNuts = svc.AddIngredient("Tree Nuts", true, "Contains tree nut proteins");
        var sulfites = svc.AddIngredient("Sulfites", true, "Contains sulfites");
        var mustard = svc.AddIngredient("Mustard", true, "Contains mustard proteins");
        var celery = svc.AddIngredient("Celery", true, "Contains celery proteins");
        var soya = svc.AddIngredient("Soya", true, "Contains soya proteins");
        var peanuts = svc.AddIngredient("Peanuts", true, "Contains peanut proteins");
        var lupin = svc.AddIngredient("Lupin", true, "Contains lupin proteins");
        var molluscs = svc.AddIngredient("Molluscs", true, "Contains mollusc proteins");
        var sesame = svc.AddIngredient("Sesame", true, "Contains sesame");

        var chicken = svc.AddIngredient("Chicken Breast");
        var lettuce = svc.AddIngredient("Lettuce");
        var tomato = svc.AddIngredient("Tomato");
        var basil = svc.AddIngredient("Basil");
        var garlic = svc.AddIngredient("Garlic");
        var onion = svc.AddIngredient("Onion");
        var pasta = svc.AddIngredient("Pasta");
        var parmesan = svc.AddIngredient("Parmesan");
        var prawns = svc.AddIngredient("Prawns");
        var steak = svc.AddIngredient("Sirloin Beef");
        var potato = svc.AddIngredient("Potato");
        var broccoli = svc.AddIngredient("Broccoli");
        var salmon = svc.AddIngredient("Salmon");
        var lemon = svc.AddIngredient("Lemon");
        var apple = svc.AddIngredient("Apple");
        var chocolate = svc.AddIngredient("Chocolate");
        var strawberry = svc.AddIngredient("Strawberry");
        var cream = svc.AddIngredient("Cream");
        var sugar = svc.AddIngredient("Sugar");

        var lunchStarter1 = new MenuItem
        {
            Name = "Tomato Basil Soup",
            Type = "Starter",
            Description = "Homemade tomato and basil soup served warm.",
            Price = 5.95,
            Ingredients = new List<Ingredient> { tomato, basil, garlic, celery }
        };

        var lunchStarter2 = new MenuItem
        {
            Name = "Garlic Bread",
            Type = "Starter",
            Description = "Toasted bread with garlic butter and herbs.",
            Price = 4.95,
            Ingredients = new List<Ingredient> { garlic, gluten, milk }
        };

        var lunchMain1 = new MenuItem
        {
            Name = "Grilled Chicken Sandwich",
            Type = "Main",
            Description = "Grilled chicken with lettuce and tomato in a soft bun.",
            Price = 10.95,
            Ingredients = new List<Ingredient> { chicken, lettuce, tomato, gluten, mustard }
        };

        var lunchMain2 = new MenuItem
        {
            Name = "Pasta Primavera",
            Type = "Main",
            Description = "Pasta with seasonal vegetables and parmesan.",
            Price = 11.50,
            Ingredients = new List<Ingredient> { pasta, onion, tomato, garlic, milk, gluten, eggs }
        };

        var lunchDessert1 = new MenuItem
        {
            Name = "Chocolate Brownie",
            Type = "Dessert",
            Description = "Warm chocolate brownie served with cream.",
            Price = 5.50,
            Ingredients = new List<Ingredient> { chocolate, eggs, milk, gluten, sugar }
        };

        var lunchDessert2 = new MenuItem
        {
            Name = "Fresh Fruit Salad",
            Type = "Dessert",
            Description = "Seasonal fruit salad with strawberry and apple.",
            Price = 4.95,
            Ingredients = new List<Ingredient> { strawberry, apple, lemon }
        };

        var dinnerStarter1 = new MenuItem
        {
            Name = "Prawn Cocktail",
            Type = "Starter",
            Description = "Classic prawn cocktail with crisp lettuce.",
            Price = 7.95,
            Ingredients = new List<Ingredient> { prawns, crustaceans, lettuce, eggs, mustard }
        };

        var dinnerStarter2 = new MenuItem
        {
            Name = "Bruschetta",
            Type = "Starter",
            Description = "Toasted bread topped with tomato, basil and garlic.",
            Price = 6.50,
            Ingredients = new List<Ingredient> { tomato, basil, garlic, gluten }
        };

        var dinnerMain1 = new MenuItem
        {
            Name = "Sirloin Steak",
            Type = "Main",
            Description = "Grilled sirloin steak with pepper sauce and potatoes.",
            Price = 21.95,
            Ingredients = new List<Ingredient> { steak, potato, milk, sulfites }
        };

        var dinnerMain2 = new MenuItem
        {
            Name = "Baked Salmon",
            Type = "Main",
            Description = "Oven-baked salmon with broccoli and lemon butter.",
            Price = 19.95,
            Ingredients = new List<Ingredient> { salmon, fish, broccoli, lemon, milk, soya }
        };

        var dinnerDessert1 = new MenuItem
        {
            Name = "Vanilla Cheesecake",
            Type = "Dessert",
            Description = "Creamy vanilla cheesecake with berry coulis.",
            Price = 6.95,
            Ingredients = new List<Ingredient> { milk, eggs, gluten, treeNuts, strawberry, cream, sugar }
        };

        var dinnerDessert2 = new MenuItem
        {
            Name = "Apple Crumble",
            Type = "Dessert",
            Description = "Warm apple crumble topped with cream.",
            Price = 6.50,
            Ingredients = new List<Ingredient> { apple, gluten, milk, sugar }
        };

        var lunchMenuItems = new List<MenuItem>
        {
            lunchStarter1, lunchStarter2,
            lunchMain1, lunchMain2,
            lunchDessert1, lunchDessert2
        };

        var dinnerMenuItems = new List<MenuItem>
        {
            dinnerStarter1, dinnerStarter2,
            dinnerMain1, dinnerMain2,
            dinnerDessert1, dinnerDessert2
        };

        var allIngredientsUsed = new List<Ingredient>
        {
            milk, eggs, gluten, crustaceans, fish, treeNuts, sulfites, mustard, celery, soya,
            chicken, lettuce, tomato, basil, garlic, onion, pasta, parmesan, prawns, steak,
            potato, broccoli, salmon, lemon, apple, chocolate, strawberry, cream, sugar
        };

        // Additional ingredients for seasonal menus
        var bacon = svc.AddIngredient("Bacon");
        var turkey = svc.AddIngredient("Turkey");
        var cranberry = svc.AddIngredient("Cranberry");
        var stuffing = svc.AddIngredient("Stuffing");
        var parsnip = svc.AddIngredient("Parsnip");
        var rosemary = svc.AddIngredient("Rosemary");
        var lobster = svc.AddIngredient("Lobster");
        var champagne = svc.AddIngredient("Champagne");
        var raspberry = svc.AddIngredient("Raspberry");
        var vanilla = svc.AddIngredient("Vanilla");
        var rose = svc.AddIngredient("Rose Water");

        var corn = svc.AddIngredient("Corn");
        var pulledPork = svc.AddIngredient("Pulled Pork");
        var porkRibs = svc.AddIngredient("Pork Ribs");
        var beefPatty = svc.AddIngredient("Beef Patty");
        var cheddar = svc.AddIngredient("Cheddar");
        var honey = svc.AddIngredient("Honey");
        var paprika = svc.AddIngredient("Paprika");
        var peach = svc.AddIngredient("Peach");
        var watermelon = svc.AddIngredient("Watermelon");
        var mint = svc.AddIngredient("Mint");
        var lime = svc.AddIngredient("Lime");
        var halloumi = svc.AddIngredient("Halloumi");
        var peppers = svc.AddIngredient("Mixed Peppers");
        var courgette = svc.AddIngredient("Courgette");

        // -------------- CHRISTMAS MENU -------------------
        var christmasStarter1 = new MenuItem
        {
            Name = "Smoked Salmon Blini",
            Type = "Starter",
            Description = "Smoked salmon on warm blinis with cream cheese and capers.",
            Price = 9.95,
            Ingredients = new List<Ingredient> { salmon, fish, cream, milk, eggs, gluten }
        };

        var christmasStarter2 = new MenuItem
        {
            Name = "Cream of Parsnip Soup",
            Type = "Starter",
            Description = "Velvety parsnip soup with crispy bacon and rosemary cream.",
            Price = 7.50,
            Ingredients = new List<Ingredient> { parsnip, bacon, cream, milk, celery, rosemary }
        };

        var christmasMain1 = new MenuItem
        {
            Name = "Roast Turkey",
            Type = "Main",
            Description = "Traditional roast turkey with stuffing, cranberry sauce and roast potatoes.",
            Price = 24.95,
            Ingredients = new List<Ingredient> { turkey, stuffing, cranberry, potato, gluten, milk }
        };

        var christmasMain2 = new MenuItem
        {
            Name = "Baked Salmon with Champagne Sauce",
            Type = "Main",
            Description = "Oven-baked salmon fillet with a champagne and cream sauce.",
            Price = 22.95,
            Ingredients = new List<Ingredient> { salmon, fish, champagne, cream, milk, lemon, sulfites }
        };

        var christmasDessert1 = new MenuItem
        {
            Name = "Christmas Pudding",
            Type = "Dessert",
            Description = "Traditional Christmas pudding served with brandy cream.",
            Price = 7.95,
            Ingredients = new List<Ingredient> { eggs, milk, gluten, treeNuts, sugar, cream, sulfites }
        };

        var christmasDessert2 = new MenuItem
        {
            Name = "Yule Log",
            Type = "Dessert",
            Description = "Chocolate sponge roll filled with cream and dusted with icing sugar.",
            Price = 6.95,
            Ingredients = new List<Ingredient> { chocolate, eggs, milk, gluten, cream, sugar }
        };

        var christmasMenuItems = new List<MenuItem>
        {
            christmasStarter1, christmasStarter2,
            christmasMain1, christmasMain2,
            christmasDessert1, christmasDessert2
        };

        // -------------- VALENTINES MENU -------------------
        var valentinesStarter1 = new MenuItem
        {
            Name = "Lobster Bisque",
            Type = "Starter",
            Description = "Rich and creamy lobster bisque with a swirl of cream.",
            Price = 11.95,
            Ingredients = new List<Ingredient> { lobster, crustaceans, cream, milk, garlic, celery }
        };

        var valentinesStarter2 = new MenuItem
        {
            Name = "Strawberry & Prawn Salad",
            Type = "Starter",
            Description = "King prawns with strawberries, mixed leaves and a champagne vinaigrette.",
            Price = 9.95,
            Ingredients = new List<Ingredient> { prawns, crustaceans, strawberry, lettuce, champagne, mustard, sulfites }
        };

        var valentinesMain1 = new MenuItem
        {
            Name = "Fillet Steak for Two",
            Type = "Main",
            Description = "Two 6oz fillet steaks with truffle butter, asparagus and dauphinoise potatoes.",
            Price = 54.95,
            Ingredients = new List<Ingredient> { steak, potato, milk, eggs, garlic, rosemary, sulfites }
        };

        var valentinesMain2 = new MenuItem
        {
            Name = "Pan Seared Salmon",
            Type = "Main",
            Description = "Salmon fillet with a lemon butter sauce, capers and seasonal vegetables.",
            Price = 26.95,
            Ingredients = new List<Ingredient> { salmon, fish, lemon, milk, broccoli, soya }
        };

        var valentinesDessert1 = new MenuItem
        {
            Name = "Chocolate Fondant",
            Type = "Dessert",
            Description = "Warm dark chocolate fondant with a molten centre, served with vanilla ice cream.",
            Price = 8.95,
            Ingredients = new List<Ingredient> { chocolate, eggs, milk, gluten, sugar, vanilla }
        };

        var valentinesDessert2 = new MenuItem
        {
            Name = "Rose & Raspberry Panna Cotta",
            Type = "Dessert",
            Description = "Silky panna cotta infused with rose water and topped with fresh raspberries.",
            Price = 7.95,
            Ingredients = new List<Ingredient> { cream, milk, sugar, raspberry, rose, vanilla }
        };

        var valentinesMenuItems = new List<MenuItem>
        {
            valentinesStarter1, valentinesStarter2,
            valentinesMain1, valentinesMain2,
            valentinesDessert1, valentinesDessert2
        };

        // -------------- SUMMER BBQ MENU -------------------
        var bbqStarter1 = new MenuItem
        {
            Name = "BBQ Pulled Pork Sliders",
            Type = "Starter",
            Description = "Smoky pulled pork in brioche buns with coleslaw and BBQ sauce.",
            Price = 8.95,
            Ingredients = new List<Ingredient> { pulledPork, gluten, eggs, mustard }
        };

        var bbqStarter2 = new MenuItem
        {
            Name = "Halloumi Skewers",
            Type = "Starter",
            Description = "Grilled halloumi with mixed peppers, courgette and a lemon herb dressing.",
            Price = 7.50,
            Ingredients = new List<Ingredient> { halloumi, milk, peppers, courgette, lemon }
        };

        var bbqMain1 = new MenuItem
        {
            Name = "Classic Beef Burger",
            Type = "Main",
            Description = "Flame-grilled beef patty with cheddar, lettuce, tomato and mustard mayo in a toasted bun.",
            Price = 14.95,
            Ingredients = new List<Ingredient> { beefPatty, gluten, lettuce, tomato, cheddar, milk, mustard, eggs }
        };

        var bbqMain2 = new MenuItem
        {
            Name = "BBQ Pork Ribs with Corn",
            Type = "Main",
            Description = "Slow-cooked pork ribs glazed with smoky honey BBQ sauce, served with chargrilled corn.",
            Price = 18.95,
            Ingredients = new List<Ingredient> { porkRibs, corn, honey, paprika, gluten, sulfites }
        };

        var bbqDessert1 = new MenuItem
        {
            Name = "Grilled Peach Sundae",
            Type = "Dessert",
            Description = "Caramelised grilled peach served over vanilla ice cream with honeycomb and cream.",
            Price = 6.50,
            Ingredients = new List<Ingredient> { peach, cream, vanilla, honey, sugar, milk }
        };

        var bbqDessert2 = new MenuItem
        {
            Name = "Watermelon & Mint Granita",
            Type = "Dessert",
            Description = "Refreshing frozen watermelon granita with fresh mint and a squeeze of lime.",
            Price = 5.50,
            Ingredients = new List<Ingredient> { watermelon, mint, lime, sugar }
        };

        var bbqMenuItems = new List<MenuItem>
        {
            bbqStarter1, bbqStarter2,
            bbqMain1, bbqMain2,
            bbqDessert1, bbqDessert2
        };


        // -------------- SIDES -------------------
        var side1 = new MenuItem
        {
            Name = "Chunky Chips",
            Type = "Side",
            Description = "Thick-cut fried potatoes seasoned with sea salt.",
            Price = 3.50,
            Ingredients = new List<Ingredient> { potato, gluten }
        };

        var side2 = new MenuItem
        {
            Name = "Seasonal Vegetables",
            Type = "Side",
            Description = "Steamed seasonal vegetables with herb butter.",
            Price = 3.00,
            Ingredients = new List<Ingredient> { broccoli, milk }
        };

        var side3 = new MenuItem
        {
            Name = "House Salad",
            Type = "Side",
            Description = "Mixed leaves, tomato and cucumber with a light vinaigrette.",
            Price = 3.00,
            Ingredients = new List<Ingredient> { lettuce, tomato, mustard, sulfites }
        };

        var side4 = new MenuItem
        {
            Name = "Garlic Mashed Potato",
            Type = "Side",
            Description = "Creamy mashed potato with roasted garlic and butter.",
            Price = 3.50,
            Ingredients = new List<Ingredient> { potato, milk, garlic }
        };

        var side5 = new MenuItem
        {
            Name = "Onion Rings",
            Type = "Side",
            Description = "Crispy battered onion rings.",
            Price = 3.50,
            Ingredients = new List<Ingredient> { onion, gluten, eggs, milk }
        };

        var sideItems = new List<MenuItem> { side1, side2, side3, side4, side5 };

        // Adding menus to service

        svc.AddMenu("Lunch Menu", "Lunch", "Lunch selection with starters, mains and desserts.", true, lunchMenuItems.Concat(sideItems).ToList());
        svc.AddMenu("Dinner Menu", "Dinner", "Dinner selection with starters, mains and desserts.", true, dinnerMenuItems.Concat(sideItems).ToList());
        svc.AddMenu("Christmas Menu", "Seasonal", "Festive Christmas selection with seasonal favourites.", false, christmasMenuItems.Concat(sideItems).ToList());
        svc.AddMenu("Valentines Menu", "Seasonal", "A romantic Valentines Day dining experience for two.", false, valentinesMenuItems.Concat(sideItems).ToList());
        svc.AddMenu("Summer BBQ Menu", "Seasonal", "A sun-soaked outdoor barbecue experience with grilled favourites and refreshing desserts.", false, bbqMenuItems.Concat(sideItems).ToList());

        // -------------- DRINKS MENU -------------------
        // Allergen ingredients already seeded above — reuse: milk, eggs, gluten, sulfites, lupin

        // Soft Drinks
        var cola = new MenuItem { Name = "Coca-Cola", Type = "Soft Drink", Description = "Classic chilled Coca-Cola.", Price = 2.95, Ingredients = new List<Ingredient>() };
        var dietCola = new MenuItem { Name = "Diet Coke", Type = "Soft Drink", Description = "Diet Coke, ice cold.", Price = 2.95, Ingredients = new List<Ingredient>() };
        var lemonade = new MenuItem { Name = "Lemonade", Type = "Soft Drink", Description = "Sparkling lemonade.", Price = 2.95, Ingredients = new List<Ingredient>() };
        var orangeJuice = new MenuItem { Name = "Fresh Orange Juice", Type = "Soft Drink", Description = "Freshly squeezed orange juice.", Price = 3.50, Ingredients = new List<Ingredient>() };
        var appleJuice = new MenuItem { Name = "Apple Juice", Type = "Soft Drink", Description = "Chilled cloudy apple juice.", Price = 3.00, Ingredients = new List<Ingredient>() };
        var sparklingWater = new MenuItem { Name = "Sparkling Water", Type = "Soft Drink", Description = "Chilled sparkling mineral water.", Price = 2.50, Ingredients = new List<Ingredient>() };
        var stillWater = new MenuItem { Name = "Still Water", Type = "Soft Drink", Description = "Chilled still mineral water.", Price = 2.50, Ingredients = new List<Ingredient>() };

        // Mixers — these are soft drinks used as mixers, listed under Soft Drinks
        var tonicWater = new MenuItem { Name = "Tonic Water", Type = "Soft Drink", Description = "Classic tonic water.", Price = 2.00, Ingredients = new List<Ingredient>() };
        var slimlineTonic = new MenuItem { Name = "Slimline Tonic", Type = "Soft Drink", Description = "Light tonic water.", Price = 2.00, Ingredients = new List<Ingredient>() };
        var gingerBeer = new MenuItem { Name = "Ginger Beer", Type = "Soft Drink", Description = "Feisty ginger beer.", Price = 2.50, Ingredients = new List<Ingredient>() };
        var sodaWater = new MenuItem { Name = "Soda Water", Type = "Soft Drink", Description = "Plain soda water.", Price = 1.50, Ingredients = new List<Ingredient>() };

        // Hot Drinks
        var espresso = new MenuItem { Name = "Espresso", Type = "Hot Drink", Description = "Single shot espresso.", Price = 2.50, Ingredients = new List<Ingredient>() };
        var americano = new MenuItem { Name = "Americano", Type = "Hot Drink", Description = "Espresso with hot water.", Price = 3.00, Ingredients = new List<Ingredient>() };
        var flatWhite = new MenuItem { Name = "Flat White", Type = "Hot Drink", Description = "Double espresso with steamed milk.", Price = 3.50, Ingredients = new List<Ingredient> { milk } };
        var cappuccino = new MenuItem { Name = "Cappuccino", Type = "Hot Drink", Description = "Espresso with steamed milk and foam.", Price = 3.50, Ingredients = new List<Ingredient> { milk } };
        var latte = new MenuItem { Name = "Latte", Type = "Hot Drink", Description = "Espresso with lots of steamed milk.", Price = 3.50, Ingredients = new List<Ingredient> { milk } };
        var hotChocolate = new MenuItem { Name = "Hot Chocolate", Type = "Hot Drink", Description = "Rich hot chocolate with cream.", Price = 3.75, Ingredients = new List<Ingredient> { milk } };
        var irishBreakfastTea = new MenuItem { Name = "Irish Breakfast Tea", Type = "Hot Drink", Description = "Barry's Irish Breakfast Tea served with milk.", Price = 2.75, Ingredients = new List<Ingredient> { milk } };
        var herbalTea = new MenuItem { Name = "Herbal Tea", Type = "Hot Drink", Description = "Selection of herbal teas — peppermint, chamomile or green tea.", Price = 2.75, Ingredients = new List<Ingredient>() };

        // Beer & Cider
        var guinness = new MenuItem { Name = "Guinness (Pint)", Type = "Beer", Description = "Classic Guinness draught stout.", Price = 6.20, Ingredients = new List<Ingredient> { gluten } };
        var heineken = new MenuItem { Name = "Heineken (Pint)", Type = "Beer", Description = "Chilled Heineken lager.", Price = 5.90, Ingredients = new List<Ingredient> { gluten } };
        var coronaBottle = new MenuItem { Name = "Corona (Bottle)", Type = "Beer", Description = "Corona Extra 330ml bottle served with lime.", Price = 5.50, Ingredients = new List<Ingredient> { gluten } };
        var bulmersCider = new MenuItem { Name = "Bulmers Cider (Pint)", Type = "Cider", Description = "Bulmers Original Irish cider.", Price = 6.00, Ingredients = new List<Ingredient>() };

        // Wine
        var houseRedWine = new MenuItem { Name = "House Red Wine (Glass)", Type = "Wine", Description = "Smooth house red wine, 175ml.", Price = 6.50, Ingredients = new List<Ingredient> { sulfites } };
        var houseWhiteWine = new MenuItem { Name = "House White Wine (Glass)", Type = "Wine", Description = "Crisp house white wine, 175ml.", Price = 6.50, Ingredients = new List<Ingredient> { sulfites } };
        var prosecco = new MenuItem { Name = "Prosecco (Glass)", Type = "Wine", Description = "Chilled Italian prosecco, 125ml.", Price = 7.50, Ingredients = new List<Ingredient> { sulfites } };

        // Spirits
        var vodka = new MenuItem { Name = "Vodka", Type = "Spirit", Description = "Single measure of vodka served with your choice of mixer.", Price = 7.00, Ingredients = new List<Ingredient>() };
        var gin = new MenuItem { Name = "Gin", Type = "Spirit", Description = "Single measure of gin served with your choice of mixer.", Price = 7.50, Ingredients = new List<Ingredient>() };
        var whiskey = new MenuItem { Name = "Whiskey", Type = "Spirit", Description = "Single measure of Irish whiskey served with your choice of mixer.", Price = 7.50, Ingredients = new List<Ingredient> { gluten } };

        // Cocktails — using egg whites flags allergen
        var mojito = new MenuItem { Name = "Mojito", Type = "Cocktail", Description = "White rum, fresh mint, lime juice, sugar and soda.", Price = 10.00, Ingredients = new List<Ingredient> { mint, lime, sugar } };
        var cosmo = new MenuItem { Name = "Cosmopolitan", Type = "Cocktail", Description = "Vodka, triple sec, cranberry juice and lime.", Price = 10.00, Ingredients = new List<Ingredient> { cranberry, lime } };
        var whiskeySour = new MenuItem { Name = "Whiskey Sour", Type = "Cocktail", Description = "Bourbon, lemon juice, sugar syrup and egg white foam.", Price = 11.00, Ingredients = new List<Ingredient> { lemon, sugar, eggs } };
        var aperolSpritz = new MenuItem { Name = "Aperol Spritz", Type = "Cocktail", Description = "Aperol, prosecco and a splash of soda.", Price = 10.50, Ingredients = new List<Ingredient> { sulfites } };
        var strawberryDaiquiri = new MenuItem { Name = "Strawberry Daiquiri", Type = "Cocktail", Description = "White rum, strawberry puree, lime juice and sugar.", Price = 10.00, Ingredients = new List<Ingredient> { strawberry, lime, sugar } };
        var pinaColada = new MenuItem { Name = "Pina Colada", Type = "Cocktail", Description = "White rum, coconut cream and pineapple juice.", Price = 10.50, Ingredients = new List<Ingredient> { treeNuts } };

        var drinksMenuItems = new List<MenuItem>
        {
            cola, dietCola, lemonade, orangeJuice, appleJuice, sparklingWater, stillWater,
            tonicWater, slimlineTonic, gingerBeer, sodaWater,
            espresso, americano, flatWhite, cappuccino, latte, hotChocolate, irishBreakfastTea, herbalTea,
            guinness, heineken, coronaBottle, bulmersCider,
            houseRedWine, houseWhiteWine, prosecco,
            vodka, gin, whiskey,
            mojito, cosmo, whiskeySour, aperolSpritz, strawberryDaiquiri, pinaColada
        };

        svc.AddMenu("Drinks Menu", "Drinks", "Full drinks menu including soft drinks, hot drinks, beer, wine, spirits and cocktails.", true, drinksMenuItems);
    }

    private static void SeedTables(IRestaurantService svc)
    {
        // T1-3: 6 seats
        svc.AddTable(1, 6);
        svc.AddTable(2, 6);
        svc.AddTable(3, 6);
        // T4: 2 seats
        svc.AddTable(4, 2);
        // T5: 8 seats
        svc.AddTable(5, 8);
        // T6-7: 4 seats
        svc.AddTable(6, 4);
        svc.AddTable(7, 4);
        // T8: 2 seats
        svc.AddTable(8, 2);
        // T9-10: 4 seats
        svc.AddTable(9, 4);
        svc.AddTable(10, 4);
        // T11: 2 seats
        svc.AddTable(11, 2);
        // T12-15: 4 seats
        svc.AddTable(12, 4);
        svc.AddTable(13, 4);
        svc.AddTable(14, 4);
        svc.AddTable(15, 4);
        // T16: 2 seats
        svc.AddTable(16, 2);
        // T17: 4 seats
        svc.AddTable(17, 4);
        // T18: 2 seats
        svc.AddTable(18, 2);
        // T19-24: 4 seats
        svc.AddTable(19, 4);
        svc.AddTable(20, 4);
        svc.AddTable(21, 4);
        svc.AddTable(22, 4);
        svc.AddTable(23, 4);
        svc.AddTable(24, 4);
        // T25: 8 seats
        svc.AddTable(25, 8);
        // T26: 4 seats
        svc.AddTable(26, 4);
        // T27-28: 2 seats
        svc.AddTable(27, 2);
        svc.AddTable(28, 2);
    }

    private static void SeedBookings(IRestaurantService svc)
    {
        // Grab menu items we need for orders
        var dinnerMenu = svc.SearchMenus().FirstOrDefault(m => m.Name == "Dinner Menu");
        var lunchMenu  = svc.SearchMenus().FirstOrDefault(m => m.Name == "Lunch Menu");
        if (dinnerMenu == null || lunchMenu == null) return;

        var prawnCocktail = dinnerMenu.MenuItems.FirstOrDefault(i => i.Name == "Prawn Cocktail");
        var bruschetta    = dinnerMenu.MenuItems.FirstOrDefault(i => i.Name == "Bruschetta");
        var steak         = dinnerMenu.MenuItems.FirstOrDefault(i => i.Name == "Sirloin Steak");
        var salmon        = dinnerMenu.MenuItems.FirstOrDefault(i => i.Name == "Baked Salmon");
        var cheesecake    = dinnerMenu.MenuItems.FirstOrDefault(i => i.Name == "Vanilla Cheesecake");
        var appleCrumble  = dinnerMenu.MenuItems.FirstOrDefault(i => i.Name == "Apple Crumble");
        var chickenSandwich = lunchMenu.MenuItems.FirstOrDefault(i => i.Name == "Grilled Chicken Sandwich");
        var pasta           = lunchMenu.MenuItems.FirstOrDefault(i => i.Name == "Pasta Primavera");
        var chocolateBrownie = lunchMenu.MenuItems.FirstOrDefault(i => i.Name == "Chocolate Brownie");
        var fruitSalad       = lunchMenu.MenuItems.FirstOrDefault(i => i.Name == "Fresh Fruit Salad");

        
        List<MenuItem> Items(params MenuItem[] menuItems) =>
            menuItems.Where(i => i != null).ToList();

        Order CompletedOrder(IRestaurantService s, int tableId, List<MenuItem> orderItems,
                             string reviewer = null, int stars = 5, string comment = null)
        {
            var quantities = orderItems
                .GroupBy(mi => mi.Id)
                .ToDictionary(g => g.Key, g => g.Count());
            var o = s.AddOrder(quantities, tableId);
            var completed = s.MarkOrderCompleted(o?.Id ?? 0);
            if (completed != null && reviewer != null)
                s.AddReview(completed.Id, reviewer, stars, comment ?? "");
            return completed;
        }

       
        void PastBooking(IRestaurantService s, string name, string phone, string email,
                         DateTime dt, int guests, int tableNumber,
                         List<MenuItem> orderItems, bool allergen = false,
                         string reviewer = null, int stars = 5, string reviewText = null,
                         string comment = "")
        {
            var order = CompletedOrder(s, tableNumber, orderItems, reviewer, stars, reviewText);
            var b = s.AddBooking(name, phone, email, dt, guests, allergen, allergen,
                                 comment, tableNumber, false);
            if (b != null) { b.Status = "Completed"; s.UpdateBooking(b); }
        }

      
        var dMinus2 = DateTime.Today.AddDays(-2);

        PastBooking(svc, "Connor Burke",    "0871112233", "connor@example.com",    dMinus2.AddHours(12),        2, 4,  Items(chickenSandwich, fruitSalad),           reviewer: "Connor Burke",    stars: 4, reviewText: "Lovely lunch, chicken sandwich was fresh.");
        PastBooking(svc, "Niamh Gallagher", "0852223344", "niamh@example.com",     dMinus2.AddHours(12).AddMinutes(30), 4, 5, Items(pasta, chocolateBrownie),        reviewer: "Niamh Gallagher", stars: 5, reviewText: "Pasta was incredible, will definitely be back.");
        PastBooking(svc, "Padraig Duffy",   "0863334455", "padraig@example.com",   dMinus2.AddHours(13),        3, 6,  Items(chickenSandwich, pasta, fruitSalad));
        PastBooking(svc, "Roisin Doherty",  "0874445566", "roisin@example.com",    dMinus2.AddHours(19),        2, 7,  Items(bruschetta, salmon, appleCrumble),       reviewer: "Roisin Doherty",  stars: 5, reviewText: "Salmon was perfectly cooked. Lovely atmosphere.");
        PastBooking(svc, "Seamus Kelly",    "0885556677", "seamus@example.com",    dMinus2.AddHours(19).AddMinutes(30), 4, 8, Items(prawnCocktail, steak, cheesecake), reviewer: "Seamus Kelly",    stars: 3, reviewText: "Steak was a little overcooked but service was friendly.");
        PastBooking(svc, "Aisling Brennan", "0896667788", "aisling@example.com",   dMinus2.AddHours(20),        2, 9,  Items(bruschetta, salmon, cheesecake),         allergen: true);
        PastBooking(svc, "Ciaran Walsh",    "0817778899", "ciaran@example.com",    dMinus2.AddHours(20).AddMinutes(30), 6, 1, Items(prawnCocktail, steak, cheesecake, appleCrumble));
        PastBooking(svc, "Maeve McGuigan",  "0828889900", "maeve@example.com",     dMinus2.AddHours(21),        4, 2,  Items(bruschetta, salmon, appleCrumble),       reviewer: "Maeve McGuigan",  stars: 4, reviewText: "Great food, slightly slow service but worth the wait.");

        
        var dMinus1 = DateTime.Today.AddDays(-1);

        PastBooking(svc, "Fionnuala Daly",  "0871234500", "fionnuala@example.com", dMinus1.AddHours(12),        2, 4,  Items(chickenSandwich, fruitSalad));
        PastBooking(svc, "Declan O'Brien",  "0852345600", "declan@example.com",    dMinus1.AddHours(12).AddMinutes(30), 4, 5, Items(pasta, chocolateBrownie),         reviewer: "Declan O'Brien", stars: 5, reviewText: "Best pasta I've had outside of Italy. Genuinely impressed.");
        PastBooking(svc, "Sinead Maguire",  "0863456700", "sinead@example.com",    dMinus1.AddHours(13),        2, 6,  Items(chickenSandwich, chocolateBrownie),     allergen: true);
        PastBooking(svc, "Brendan Carr",    "0874567800", "brendan@example.com",   dMinus1.AddHours(19),        4, 7,  Items(prawnCocktail, steak, cheesecake),       reviewer: "Brendan Carr",   stars: 4, reviewText: "Excellent steak night. Could do with more sauce options.");
        PastBooking(svc, "Orla Fitzsimmons","0885678900", "orla@example.com",      dMinus1.AddHours(19).AddMinutes(30), 2, 8, Items(bruschetta, salmon, cheesecake));
        PastBooking(svc, "Tadhg Boyle",     "0896789000", "tadhg@example.com",     dMinus1.AddHours(20),        6, 1,  Items(prawnCocktail, steak, appleCrumble, cheesecake), reviewer: "Tadhg Boyle", stars: 5, reviewText: "Celebrated my mum's birthday here. Perfect evening.");
        PastBooking(svc, "Aoife Murray",    "0817890100", "aoife@example.com",     dMinus1.AddHours(20).AddMinutes(30), 4, 2, Items(bruschetta, salmon, appleCrumble));
        PastBooking(svc, "Cillian Power",   "0828901200", "cillian@example.com",   dMinus1.AddHours(21),        3, 3,  Items(prawnCocktail, steak, cheesecake),       allergen: true);

       
        var today = DateTime.Today;

        // Lunch sittings — all completed and gone home
        var lunchO1 = CompletedOrder(svc, 4,  Items(chickenSandwich, fruitSalad),    "Siobhan Quinn",   4, "Quick and tasty lunch. Good value.");
        var lb1 = svc.AddBooking("Siobhan Quinn",   "0871111001", "siobhan@example.com",   today.AddHours(12),               2, false, false, "",                                    4,  false);
        if (lb1 != null) { lb1.Status = "Completed"; svc.UpdateBooking(lb1); }

        var lunchO2 = CompletedOrder(svc, 5,  Items(pasta, chocolateBrownie));
        var lb2 = svc.AddBooking("Donal Hegarty",   "0852222002", "donal@example.com",     today.AddHours(12).AddMinutes(30), 4, false, false, "",                                   5,  false);
        if (lb2 != null) { lb2.Status = "Completed"; svc.UpdateBooking(lb2); }

        var lunchO3 = CompletedOrder(svc, 6,  Items(chickenSandwich, pasta),         "Patricia Flynn",  5, "The pasta primavera is a must. Absolutely delicious.");
        var lb3 = svc.AddBooking("Patricia Flynn",  "0863333003", "patricia@example.com",  today.AddHours(13),               3, false, false, "",                                    6,  false);
        if (lb3 != null) { lb3.Status = "Completed"; svc.UpdateBooking(lb3); }

        var lunchO4 = CompletedOrder(svc, 7,  Items(chickenSandwich, fruitSalad));
        var lb4 = svc.AddBooking("Gerard Higgins",  "0874444004", "gerard@example.com",    today.AddHours(13).AddMinutes(30), 2, false, false, "",                                   7,  false);
        if (lb4 != null) { lb4.Status = "Completed"; svc.UpdateBooking(lb4); }

        // Early dinner sittings (17:30 start) — all completed by 20:30
        var earlyO1 = CompletedOrder(svc, 8,  Items(bruschetta, salmon, cheesecake), "Nuala Sheridan",  5, "Salmon was the best I've ever had. Highly recommend.");
        var eb1 = svc.AddBooking("Nuala Sheridan",  "0885555005", "nuala@example.com",     today.AddHours(17).AddMinutes(30), 2, false, false, "",                                   8,  false);
        if (eb1 != null) { eb1.Status = "Completed"; svc.UpdateBooking(eb1); }

        var earlyO2 = CompletedOrder(svc, 9,  Items(prawnCocktail, steak, appleCrumble), "Ronan Gallagher", 4, "Great food, came for a birthday and were looked after really well.");
        var eb2 = svc.AddBooking("Ronan Gallagher", "0896666006", "ronan@example.com",     today.AddHours(18),               4, false, false, "Birthday dinner",                     9,  false);
        if (eb2 != null) { eb2.Status = "Completed"; svc.UpdateBooking(eb2); }

        var earlyO3 = CompletedOrder(svc, 10, Items(bruschetta, salmon, cheesecake));
        var eb3 = svc.AddBooking("Kathleen Doherty","0817777007", "kathleen@example.com",  today.AddHours(18).AddMinutes(30), 6, true,  true,  "Nut allergy — please advise kitchen", 10, false);
        if (eb3 != null) { eb3.Status = "Completed"; svc.UpdateBooking(eb3); }
        if (earlyO3 != null)
            svc.AddAllergenConsent(earlyO3.Id, "Kathleen Doherty", "kathleen@example.com", "0817777007", true, earlyO3.OrderItems.Select(oi => oi.MenuItem).ToList());

        // ---- Current sitting — active at 20:30 ----

        // T1 — Alice Murphy, 4 guests, active order, seated
        var order1 = svc.AddOrder(Items(prawnCocktail, steak, cheesecake).GroupBy(mi => mi.Id).ToDictionary(g => g.Key, g => g.Count()), 1);
        svc.SetTableOccupancy(1, true, 4);
        var b1 = svc.AddBooking("Alice Murphy",   "0871234567", "alice@example.com",  today.AddHours(19),               4, false, false, "",                      1, true);
        if (b1 != null) { b1.Status = "Seated"; svc.UpdateBooking(b1); }

        // T2 — James Brady, 2 guests, active order, seated
        var order2 = svc.AddOrder(Items(bruschetta, salmon, appleCrumble).GroupBy(mi => mi.Id).ToDictionary(g => g.Key, g => g.Count()), 2);
        svc.SetTableOccupancy(2, true, 2);
        var b2 = svc.AddBooking("James Brady",    "0852345678", "james@example.com",  today.AddHours(19).AddMinutes(30), 2, false, false, "",                     2, true);
        if (b2 != null) { b2.Status = "Seated"; svc.UpdateBooking(b2); }

        // T3 — Sarah O'Neill, 3 guests, active order, allergen consent captured
        var order3 = svc.AddOrder(Items(prawnCocktail, salmon, cheesecake).GroupBy(mi => mi.Id).ToDictionary(g => g.Key, g => g.Count()), 3);
        svc.SetTableOccupancy(3, true, 3);
        var b3 = svc.AddBooking("Sarah O'Neill",  "0863456789", "sarah@example.com",  today.AddHours(20),               3, true,  true,  "Window seat preferred", 3, true);
        if (b3 != null) { b3.Status = "Seated"; svc.UpdateBooking(b3); }
        if (order3 != null)
            svc.AddAllergenConsent(order3.Id, "Sarah O'Neill", "sarah@example.com", "0863456789", true, order3.OrderItems.Select(oi => oi.MenuItem).ToList());

        // T4 — Martin Fox, 2 guests, booked for 21:00 — not arrived yet, table is free
        svc.AddBooking("Martin Fox",      "0874567891", "martin@example.com",    today.AddHours(21),               2, false, false, "",                              4, true);

        // T5 — Emma Byrne, 4 guests, booked for 21:00 birthday dinner — not arrived yet
        svc.AddBooking("Emma Byrne",      "0865678901", "emma@example.com",      today.AddHours(21),               4, false, false, "Celebrating a birthday",        5, true);

        
        var tom = DateTime.Today.AddDays(1);

        svc.AddBooking("Liam Walsh",      "0874567890", "liam@example.com",      tom.AddHours(12).AddMinutes(30), 2, false, false, "",                            4, true);
        svc.AddBooking("Grainne Mooney",  "0851234567", "grainne@example.com",   tom.AddHours(13),               4, false, false, "",                             5, true);
        svc.AddBooking("Eoin Cassidy",    "0862345678", "eoin@example.com",      tom.AddHours(19),               6, false, false, "Anniversary dinner",           1, true);
        svc.AddBooking("Mary Concannon",  "0873456789", "mary@example.com",      tom.AddHours(19),               2, false, false, "",                             6, true);
        svc.AddBooking("Peadar Nolan",    "0884567890", "peadar@example.com",    tom.AddHours(19).AddMinutes(30), 4, true,  false, "Shellfish allergy",           7, true);
        svc.AddBooking("Brid Connolly",   "0895678901", "brid@example.com",      tom.AddHours(20),               2, false, false, "",                             8, true);
        svc.AddBooking("Fergus Timmins",  "0816789012", "fergus@example.com",    tom.AddHours(20).AddMinutes(30), 6, false, false, "Large group — need highchair", 2, true);
        svc.AddBooking("Sorcha Keenan",   "0827890123", "sorcha@example.com",    tom.AddHours(21),               4, false, false, "",                             3, true);

        var d2 = DateTime.Today.AddDays(2);
        svc.AddBooking("Siobhan Ryan",      "0871234521", "siobhan.ryan@example.com",      d2.AddHours(12),               2, false, false, "",                        4, true);
        svc.AddBooking("Colm Brennan",      "0852345632", "colm.brennan@example.com",      d2.AddHours(12).AddMinutes(30), 4, false, false, "",                       5, true);
        svc.AddBooking("Fiona O'Sullivan",  "0863456743", "fiona.osullivan@example.com",   d2.AddHours(13),               3, false, false, "",                        6, true);
        svc.AddBooking("Cormac Lynch",      "0874567854", "cormac.lynch@example.com",      d2.AddHours(19),               4, false, false, "",                        1, true);
        svc.AddBooking("Deirdre Nolan",     "0885678965", "deirdre.nolan@example.com",     d2.AddHours(19).AddMinutes(30), 2, false, false, "",                       7, true);
        svc.AddBooking("Patrick Mulligan",  "0896789076", "patrick.mulligan@example.com",  d2.AddHours(20),               6, false, false, "",                        2, true);
        svc.AddBooking("Una McCarthy",      "0817890187", "una.mccarthy@example.com",      d2.AddHours(20).AddMinutes(30), 4, false, false, "",                       3, true);
        svc.AddBooking("Barry Kavanagh",    "0828901298", "barry.kavanagh@example.com",    d2.AddHours(21),               2, false, false, "",                        8, true);

        var d3 = DateTime.Today.AddDays(3);
        svc.AddBooking("Lorraine Fahy",     "0871112301", "lorraine.fahy@example.com",     d3.AddHours(12),               2, false, false, "",                        4, true);
        svc.AddBooking("Shane Doyle",       "0852223402", "shane.doyle@example.com",       d3.AddHours(12).AddMinutes(30), 4, false, false, "",                       5, true);
        svc.AddBooking("Eileen Costello",   "0863334503", "eileen.costello@example.com",   d3.AddHours(13),               3, true,  false, "Nut allergy",             6, true);
        svc.AddBooking("Niall Connolly",    "0874445604", "niall.connolly@example.com",    d3.AddHours(19),               4, false, false, "",                        1, true);
        svc.AddBooking("Yvonne Burke",      "0885556705", "yvonne.burke@example.com",      d3.AddHours(19).AddMinutes(30), 2, false, false, "",                       7, true);
        svc.AddBooking("Tomas Forde",       "0896667806", "tomas.forde@example.com",       d3.AddHours(20),               6, false, false, "Large work group",        2, true);
        svc.AddBooking("Caoimhe Sheridan",  "0817778907", "caoimhe.sheridan@example.com",  d3.AddHours(20).AddMinutes(30), 4, false, false, "",                       3, true);
        svc.AddBooking("Donal Fitzpatrick", "0828889008", "donal.fitzpatrick@example.com", d3.AddHours(21),               2, false, false, "",                        8, true);

        var d4 = DateTime.Today.AddDays(4);
        svc.AddBooking("Mairead Walsh",     "0871223401", "mairead.walsh@example.com",     d4.AddHours(12),               2, false, false, "",                        4, true);
        svc.AddBooking("Ronan Healy",       "0852334502", "ronan.healy@example.com",       d4.AddHours(12).AddMinutes(30), 4, false, false, "",                       5, true);
        svc.AddBooking("Cliodhna Murray",   "0863445603", "cliodhna.murray@example.com",   d4.AddHours(13),               3, false, false, "",                        6, true);
        svc.AddBooking("Seamus O'Brien",    "0874556704", "seamus.obrien@example.com",     d4.AddHours(19),               4, true,  false, "Gluten intolerance",      1, true);
        svc.AddBooking("Aoibheann Power",   "0885667805", "aoibheann.power@example.com",   d4.AddHours(19).AddMinutes(30), 2, false, false, "",                       7, true);
        svc.AddBooking("Ruairi Gallagher",  "0896778906", "ruairi.gallagher@example.com",  d4.AddHours(20),               6, false, false, "",                        2, true);
        svc.AddBooking("Maire Boyle",       "0817889007", "maire.boyle@example.com",       d4.AddHours(20).AddMinutes(30), 4, false, false, "",                       3, true);
        svc.AddBooking("Fintan Moran",      "0828990108", "fintan.moran@example.com",      d4.AddHours(21),               2, false, false, "",                        8, true);

        var d5 = DateTime.Today.AddDays(5);
        svc.AddBooking("Bridget Quinn",     "0871334501", "bridget.quinn@example.com",     d5.AddHours(12),               2, false, false, "",                        4, true);
        svc.AddBooking("Kevin Doherty",     "0852445602", "kevin.doherty@example.com",     d5.AddHours(12).AddMinutes(30), 4, false, false, "",                       5, true);
        svc.AddBooking("Nuala Higgins",     "0863556703", "nuala.higgins@example.com",     d5.AddHours(13),               3, false, false, "",                        6, true);
        svc.AddBooking("Eamonn Casey",      "0874667804", "eamonn.casey@example.com",      d5.AddHours(19),               4, false, false, "Business dinner",         1, true);
        svc.AddBooking("Sorcha Maguire",    "0885778905", "sorcha.maguire@example.com",    d5.AddHours(19).AddMinutes(30), 2, false, false, "",                       7, true);
        svc.AddBooking("Cathal O'Neill",    "0896889006", "cathal.oneill@example.com",     d5.AddHours(20),               6, false, false, "",                        2, true);
        svc.AddBooking("Ronan Carthy",      "0817990107", "ronan.carthy@example.com",      d5.AddHours(20).AddMinutes(30), 4, false, false, "",                       3, true);
        svc.AddBooking("Meadhbh Flynn",     "0828001208", "meadhbh.flynn@example.com",     d5.AddHours(21),               2, false, false, "",                        8, true);

        var d6 = DateTime.Today.AddDays(6);
        svc.AddBooking("Aidan Lawlor",      "0871445601", "aidan.lawlor@example.com",      d6.AddHours(12),               2, false, false, "",                        4, true);
        svc.AddBooking("Sinead Farrell",    "0852556702", "sinead.farrell@example.com",    d6.AddHours(12).AddMinutes(30), 4, false, false, "",                       5, true);
        svc.AddBooking("Ciaran McNamara",   "0863667803", "ciaran.mcnamara@example.com",   d6.AddHours(13),               3, false, false, "",                        6, true);
        svc.AddBooking("Grainne Timmons",   "0874778904", "grainne.timmons@example.com",   d6.AddHours(19),               4, false, false, "",                        1, true);
        svc.AddBooking("Padraig Sheridan",  "0885889005", "padraig.sheridan@example.com",  d6.AddHours(19).AddMinutes(30), 2, true,  false, "Dairy allergy",          7, true);
        svc.AddBooking("Niamh Keenan",      "0896990106", "niamh.keenan@example.com",      d6.AddHours(20),               6, false, false, "Graduation celebration",  2, true);
        svc.AddBooking("Conor Dowd",        "0817001207", "conor.dowd@example.com",        d6.AddHours(20).AddMinutes(30), 4, false, false, "",                       3, true);
        svc.AddBooking("Aine Naughton",     "0828112308", "aine.naughton@example.com",     d6.AddHours(21),               2, false, false, "",                        8, true);
    }

}

