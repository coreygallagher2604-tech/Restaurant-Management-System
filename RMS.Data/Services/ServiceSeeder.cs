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
        // SeedTables(rsvc);
       // SeedBookings(rsvc);
        // SeedOrders(rsvc);
    }

    // use this method FIRST to seed the database with dummy test data using an IUserService
    private static void SeedUsers(IUserService svc)
    {
        // Note: do not call initialise here

        // seed default users
        svc.Register("admin","admin@mail.com","password",Role.admin);
        svc.Register("autenticated","autenticated@mail.com","password",Role.autenticated);
        svc.Register("guest","guest@mail.com","password",Role.guest);
       
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

        // ===================== CHRISTMAS MENU =====================
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

        // ===================== VALENTINES MENU =====================
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

        // ===================== SUMMER BBQ MENU =====================
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


        // Adding menus to service

        svc.AddMenu("Lunch Menu", "Lunch", "Lunch selection with starters, mains and desserts.", true, lunchMenuItems);
        svc.AddMenu("Dinner Menu", "Dinner", "Dinner selection with starters, mains and desserts.", true, dinnerMenuItems);
        svc.AddMenu("Christmas Menu", "Seasonal", "Festive Christmas selection with seasonal favourites.", true, christmasMenuItems);
        svc.AddMenu("Valentines Menu", "Seasonal", "A romantic Valentines Day dining experience for two.", true, valentinesMenuItems);
        svc.AddMenu("Summer BBQ Menu", "Seasonal", "A sun-soaked outdoor barbecue experience with grilled favourites and refreshing desserts.", true, bbqMenuItems);
    }



}

