using Gumo.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Gumo.Infrastructure.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(GumoDbContext db, UserManager<IdentityUser> userManager, string? adminEmail = null, string? adminPassword = null)
    {
        await db.Database.MigrateAsync();

        if (await db.Locations.AnyAsync()) return;

        var belgisches = new Location
        {
            Name = "GuMo Belgisches Viertel",
            Address = "Belgisches Viertel, Cologne",
            Hours = "Mon–Fri 8:00–18:00, Sat–Sun 9:00–17:00",
            GoogleMapsUrl = "https://maps.google.com/?q=GuMo+Belgisches+Viertel+Cologne"
        };
        var deutz = new Location
        {
            Name = "GuMo Deutz",
            Address = "Deutz, Cologne",
            Hours = "Mon–Fri 8:00–18:00, Sat–Sun 9:00–17:00",
            GoogleMapsUrl = "https://maps.google.com/?q=GuMo+Deutz+Cologne"
        };
        db.Locations.AddRange(belgisches, deutz);

        var seasonal   = new Category { Name = "Seasonal Specials", DisplayOrder = 1 };
        var deals      = new Category { Name = "Deals",             DisplayOrder = 2 };
        var kaffee     = new Category { Name = "Kaffee",            DisplayOrder = 3 };
        var iced       = new Category { Name = "Iced Drinks",       DisplayOrder = 4 };
        var bagel      = new Category { Name = "GuMos Bagel",       DisplayOrder = 5 };
        var focaccia   = new Category { Name = "GuMo's Focaccia",   DisplayOrder = 6 };
        var oats       = new Category { Name = "Oats",              DisplayOrder = 7 };
        var acai       = new Category { Name = "Acai",              DisplayOrder = 8 };
        var juices     = new Category { Name = "Juices",            DisplayOrder = 9 };
        var sweets     = new Category { Name = "Sweet Treats",      DisplayOrder = 10 };
        db.Categories.AddRange(seasonal, deals, kaffee, iced, bagel, focaccia, oats, acai, juices, sweets);

        var items = new List<MenuItem>
        {
            // Seasonal Specials
            new() { Name = "Green Tuna Flat",       Description = "Tuna Creme, Pesto, Avocado, Tomato, Rucola",                                                              Price = 9.90m,  Category = seasonal, DisplayOrder = 1, ImageFileName = "5abc5441c664a3f816cf4a7dbf1d4e7690fa04fe.jpeg" },
            new() { Name = "Baked Oats Biscoff",    Description = "Baked Oats Base – Granola, Banana, Walnuts, Blueberries, Lotus Cream",                                    Price = 5.40m,  Category = seasonal, DisplayOrder = 2, ImageFileName = "44d57d4a0b38388fe03e8681ee10c63c79a0b041.jpeg" },
            new() { Name = "Cold Oats Biscoff",     Description = "Cold Oats Base – Granola, Banana, Walnuts, Blueberries, Lotus Cream",                                     Price = 6.20m,  Category = seasonal, DisplayOrder = 3, ImageFileName = "d68157ba146e9700666f3f5bd01c179a60428b5e.jpeg" },
            new() { Name = "Avocado Scramble",      Description = "Scrambled Eggs, Avocado, Tomato and Feta cheese",                                                          Price = 6.90m,  Category = seasonal, DisplayOrder = 4, ImageFileName = "071300bcc7a1d4fba801961e3a18d20df3a2936d.png" },
            new() { Name = "Biscoff Latte",         Description = "Espresso, Milk, Biscoff Cream",                                                                            Price = 5.15m,  Category = seasonal, DisplayOrder = 5, ImageFileName = "58f5671a64baa17a715cd43b5f110f295fca95f7.png" },
            new() { Name = "Salted Maple Latte",    Description = "Espresso, Milk, Maple Cream",                                                                              Price = 5.90m,  Category = seasonal, DisplayOrder = 6, ImageFileName = "b0c45b95f9c0809b702be15fe1e2a8ca9f4ea927.png" },
            new() { Name = "Iced Biscoff Latte",    Description = "Espresso, Milk, Biscoff Cream, Ice",                                                                       Price = 5.40m,  Category = seasonal, DisplayOrder = 7, ImageFileName = "4954364a84564b709c98d33dd6017015231f5df7.png" },
            new() { Name = "Iced Salted Maple Latte", Description = "Espresso, Milk, Maple Cream, Ice",                                                                       Price = 6.20m,  Category = seasonal, DisplayOrder = 8, ImageFileName = "120e0f654dc6dd282232e98463f2d4e1fcb4d98d.png" },

            // Deals
            new() { Name = "Morning Deal",          Description = null,                                                                                                        Price = 5.60m,  Category = deals, DisplayOrder = 1, ImageFileName = "ae3d2b46825769c3f0cd58a764065f87cd534cdd.png" },

            // Kaffee
            new() { Name = "Biscoff Latte",         Description = "Espresso, Milk, Biscoff Cream",                                                                            Price = 5.15m,  Category = kaffee, DisplayOrder = 1,  ImageFileName = "58f5671a64baa17a715cd43b5f110f295fca95f7.png" },
            new() { Name = "Salted Maple Latte",    Description = "Espresso, Milk, Maple Cream",                                                                              Price = 5.90m,  Category = kaffee, DisplayOrder = 2,  ImageFileName = "b0c45b95f9c0809b702be15fe1e2a8ca9f4ea927.png" },
            new() { Name = "Cappuccino Small",      Description = null,                                                                                                        Price = 3.60m,  Category = kaffee, DisplayOrder = 3,  ImageFileName = "dfe46b5ad71e0c636f4a0335aee4afc4fba1aef7.png" },
            new() { Name = "Cappuccino Large",      Description = null,                                                                                                        Price = 4.15m,  Category = kaffee, DisplayOrder = 4,  ImageFileName = "dfe46b5ad71e0c636f4a0335aee4afc4fba1aef7.png" },
            new() { Name = "Flat White",            Description = null,                                                                                                        Price = 4.00m,  Category = kaffee, DisplayOrder = 5,  ImageFileName = "dfe46b5ad71e0c636f4a0335aee4afc4fba1aef7.png" },
            new() { Name = "Latte Macchiato",       Description = null,                                                                                                        Price = 4.00m,  Category = kaffee, DisplayOrder = 6,  ImageFileName = "dfe46b5ad71e0c636f4a0335aee4afc4fba1aef7.png" },
            new() { Name = "Small Americano",       Description = null,                                                                                                        Price = 2.90m,  Category = kaffee, DisplayOrder = 7,  ImageFileName = "4e9f72c1058b876fa128060aff26345ab8f86628.jpeg" },
            new() { Name = "Large Americano",       Description = null,                                                                                                        Price = 3.40m,  Category = kaffee, DisplayOrder = 8,  ImageFileName = "4e9f72c1058b876fa128060aff26345ab8f86628.jpeg" },
            new() { Name = "Espresso",              Description = null,                                                                                                        Price = 2.50m,  Category = kaffee, DisplayOrder = 9,  ImageFileName = "0cacf2f2e1aa99e7ccb31af6254578ceedb857c2.png" },
            new() { Name = "Double Espresso",       Description = null,                                                                                                        Price = 3.30m,  Category = kaffee, DisplayOrder = 10, ImageFileName = "0cacf2f2e1aa99e7ccb31af6254578ceedb857c2.png" },
            new() { Name = "Espresso Macchiato",    Description = null,                                                                                                        Price = 2.90m,  Category = kaffee, DisplayOrder = 11, ImageFileName = "0cacf2f2e1aa99e7ccb31af6254578ceedb857c2.png" },
            new() { Name = "Pure Matcha",           Description = "Bio/Organic Matcha Kotobuki",                                                                              Price = 5.10m,  Category = kaffee, DisplayOrder = 12, ImageFileName = "dfbff1531c0140bbdf9f7b283c668543718f889b.jpeg" },
            new() { Name = "Sweet Matcha",          Description = "Matcha Syrup – Matchary",                                                                                  Price = 4.70m,  Category = kaffee, DisplayOrder = 13, ImageFileName = "dfbff1531c0140bbdf9f7b283c668543718f889b.jpeg" },
            new() { Name = "Chai Latte",            Description = null,                                                                                                        Price = 4.30m,  Category = kaffee, DisplayOrder = 14, ImageFileName = "f3e32f5132a30b0772d113ea24753c3d1c62e8d2.jpeg" },
            new() { Name = "Hot Chocolate Small",   Description = null,                                                                                                        Price = 3.50m,  Category = kaffee, DisplayOrder = 15, ImageFileName = "ddc3b1537a9a372f4a7a482e6da300840575a182.jpeg" },
            new() { Name = "Hot Chocolate Large",   Description = null,                                                                                                        Price = 3.90m,  Category = kaffee, DisplayOrder = 16, ImageFileName = "ddc3b1537a9a372f4a7a482e6da300840575a182.jpeg" },
            new() { Name = "Fresh Mint Tea",        Description = null,                                                                                                        Price = 3.70m,  Category = kaffee, DisplayOrder = 17, ImageFileName = "85b7d26f721a520e72e97fae182f487f01d5438d.png" },
            new() { Name = "Tea",                   Description = null,                                                                                                        Price = 3.70m,  Category = kaffee, DisplayOrder = 18, ImageFileName = "34bca8031fcdb2f842d754fe399cd109d357a5f5.png" },
            new() { Name = "Earl Grey Tea",         Description = null,                                                                                                        Price = 3.50m,  Category = kaffee, DisplayOrder = 19, ImageFileName = "c6efd53d44a0635f3f726e7dbe0f758fd79990b9.png" },
            new() { Name = "Blend GuMo X Ernst",    Description = null,                                                                                                        Price = 12.99m, Category = kaffee, DisplayOrder = 20, ImageFileName = "c46a687270a6962af0178957c09765c3b4330b81.png" },

            // Iced Drinks
            new() { Name = "Iced Biscoff Latte",    Description = "Espresso, Milk, Biscoff Cream, Ice",                                                                       Price = 5.40m,  Category = iced, DisplayOrder = 1,  ImageFileName = "4954364a84564b709c98d33dd6017015231f5df7.png" },
            new() { Name = "Iced Salted Maple Latte", Description = "Espresso, Milk, Maple Cream, Ice",                                                                       Price = 6.20m,  Category = iced, DisplayOrder = 2,  ImageFileName = "120e0f654dc6dd282232e98463f2d4e1fcb4d98d.png" },
            new() { Name = "Iced Cappuccino Small", Description = null,                                                                                                        Price = 3.90m,  Category = iced, DisplayOrder = 3,  ImageFileName = "89128c65064c5ed46adc9063029739146c1a19d9.png" },
            new() { Name = "Iced Cappuccino Large", Description = null,                                                                                                        Price = 4.50m,  Category = iced, DisplayOrder = 4,  ImageFileName = "a6ceb55dd1b84f6c43a082940652406362aa0b29.png" },
            new() { Name = "Iced Flat White",       Description = null,                                                                                                        Price = 4.30m,  Category = iced, DisplayOrder = 5,  ImageFileName = "bb835685cb9bc014773ef7cf207c55383ed6d101.png" },
            new() { Name = "Iced Latte Macchiato",  Description = null,                                                                                                        Price = 4.20m,  Category = iced, DisplayOrder = 6,  ImageFileName = "ba3fb3e9b0a8c935053a7d2d09f96bfd9a3dae3c.png" },
            new() { Name = "Iced Americano Small",  Description = null,                                                                                                        Price = 3.05m,  Category = iced, DisplayOrder = 7,  ImageFileName = "432de070f4032c17a3a4662a2992776a052688f7.png" },
            new() { Name = "Iced Americano Large",  Description = null,                                                                                                        Price = 3.65m,  Category = iced, DisplayOrder = 8,  ImageFileName = "44b0fd026057fc0321ca2eca93319219ab609087.png" },
            new() { Name = "Iced Pure Matcha",      Description = "Bio/Organic Matcha Kotobuki",                                                                              Price = 5.30m,  Category = iced, DisplayOrder = 9,  ImageFileName = "b3789e140a601ea262d3fa157c4ae3df4c8b5bd0.png" },
            new() { Name = "Iced Coffee by Maxi Nutriton", Description = "MaxPresso",                                                                                         Price = 4.90m,  Category = iced, DisplayOrder = 10, ImageFileName = "92e028ced6467421ffb00d84d6b09d1420c03343.jpeg" },
            new() { Name = "Iced Sweet Matcha",     Description = "Matcha Syrup – Matchary",                                                                                  Price = 4.90m,  Category = iced, DisplayOrder = 11, ImageFileName = "b3789e140a601ea262d3fa157c4ae3df4c8b5bd0.png" },
            new() { Name = "Iced Tea Peach by Maxi Nutriton", Description = "Peach Ice Tea",                                                                                  Price = 4.90m,  Category = iced, DisplayOrder = 12, ImageFileName = "2ecf65decf0ea2b0ec02b923e2904c1eadba1fb1.jpeg" },
            new() { Name = "Iced Chai Latte",       Description = null,                                                                                                        Price = 4.50m,  Category = iced, DisplayOrder = 13, ImageFileName = "48b99e853dbe975d856b3b0e97ccb8252457ec04.png" },
            new() { Name = "Iced Kakao Small",      Description = null,                                                                                                        Price = 3.65m,  Category = iced, DisplayOrder = 14, ImageFileName = "d69dcede5bf3ad76600b2e2b7c7cecacdc536bc1.png" },
            new() { Name = "Iced Kakao Large",      Description = null,                                                                                                        Price = 4.10m,  Category = iced, DisplayOrder = 15, ImageFileName = "d69dcede5bf3ad76600b2e2b7c7cecacdc536bc1.png" },

            // GuMos Bagel
            new() { Name = "OG Bagel",              Description = "Vegan Cream Cheese",                                                                IsVegan = true,         Price = 5.40m,  Category = bagel, DisplayOrder = 1,  ImageFileName = "91b99b65c54350121c7f8f77be226cf1bcd08ab5.jpeg" },
            new() { Name = "GOAT",                  Description = "Ricotta, Spinach, Goat Cheese, Walnuts, Honey",                                                            Price = 7.90m,  Category = bagel, DisplayOrder = 2,  ImageFileName = "bec07affe759d8cfe08a3525f5caf4cb87fe7718.jpeg" },
            new() { Name = "GuMmus Bagel",          Description = "Hummus, Spinach Leaves, Grilled Bell Peppers, Fried Onions",                        IsVegan = true,         Price = 7.50m,  Category = bagel, DisplayOrder = 3,  ImageFileName = "6aa452974f742cc21392636249b64045624ab94d.jpeg" },
            new() { Name = "Peanabanana Bagel",     Description = "Peanut Butter, Banana, Banana Chips, Cinnamon",                                     IsVegan = true,         Price = 6.30m,  Category = bagel, DisplayOrder = 4,  ImageFileName = "283950780f230a5efdb1bb0850260c15e470429b.jpeg" },
            new() { Name = "Bella Mortadella Bagel", Description = "Truffle Cream, Mortadella, Tomato, Pistachios, Pepper",                                                   Price = 7.90m,  Category = bagel, DisplayOrder = 5,  ImageFileName = "8045be86e6d99c2dc60579dcbbc94d5f5e2ccb9a.jpeg" },
            new() { Name = "Salmon Bagel",          Description = "Herbs Cream, Spinach, Salmon, Red Onions, Tomato",                                                         Price = 7.90m,  Category = bagel, DisplayOrder = 6,  ImageFileName = "6a87f29f3b7acf7bb95b2951b2104e5d89456168.jpeg" },
            new() { Name = "Ham Spread Bagel",      Description = "Truffle Cream, Air-Dried Ham, Tomato, Pistachios, Pepper",                                                 Price = 8.90m,  Category = bagel, DisplayOrder = 7,  ImageFileName = "49695f25b373a50c96b72a7a7e91927899317e89.jpeg" },
            new() { Name = "Truffle & Camembert Bagel", Description = "Truffle Cream, Camembert, Spinach Leaves, Cranberries",                                                Price = 7.50m,  Category = bagel, DisplayOrder = 8,  ImageFileName = "aff09c05d51e08e0249fd5d25225d04bd6533396.jpeg" },
            new() { Name = "BuonGiorno Bagel",      Description = "Green Pesto, Bruschetta, Mozzarella, Tomato, Rucola",                                                      Price = 6.90m,  Category = bagel, DisplayOrder = 9,  ImageFileName = "14ea802d3405471a056f2dd8fe2840eaf4061a76.jpeg" },
            new() { Name = "Hot Turkey Bagel",      Description = "Cherry Pepper Cream Cheese, Truffle Cream, Rucola, Pulled Turkey, Tomato, Red Onions",                    Price = 7.90m,  Category = bagel, DisplayOrder = 10, ImageFileName = "80db8d82e93f226b85ae75153961086111e6f796.jpeg" },
            new() { Name = "Pulled Turkey Bagel",   Description = "Cherry Pepper Cream Cheese, Rucola, Pulled Turkey, Tomato, Feta Cheese, Fried Onions",                    Price = 7.90m,  Category = bagel, DisplayOrder = 11, ImageFileName = "cb333c6ef670172d13e77bf990703025be26a595.jpeg" },

            // GuMo's Focaccia
            new() { Name = "GuMmus Focaccia",       Description = "Hummus, Spinach Leaves, Grilled Bell Peppers, Fried Onions",                        IsVegan = true,         Price = 8.00m,  Category = focaccia, DisplayOrder = 1, ImageFileName = "f73115bd8ed3120196d3fcc697adc317503a2b01.png" },
            new() { Name = "Peanabanana Focaccia",  Description = "Peanut Butter, Banana, Banana Chips, Cinnamon",                                     IsVegan = true,         Price = 6.90m,  Category = focaccia, DisplayOrder = 2, ImageFileName = "e8a3f97fbe261df532fa26618f3f3fcefcf3cb8f.png" },
            new() { Name = "Salmon Focaccia",       Description = "Herbs Cream, Spinach, Salmon, Red Onions, Tomato",                                                         Price = 8.40m,  Category = focaccia, DisplayOrder = 3, ImageFileName = "f0f80681972745b3a99972d47e83201217fd5fc7.png" },
            new() { Name = "Bella Mortadella Focaccia", Description = "Truffle Cream, Mortadella, Tomato, Pistachios, Pepper",                                                Price = 8.40m,  Category = focaccia, DisplayOrder = 4, ImageFileName = "ce1f8d4fd5ee468abb41f8abf6da815de2f9b9da.png" },
            new() { Name = "Truffle & Camembert Focaccia", Description = "Truffle Cream, Camembert, Spinach Leaves, Cranberries",                                             Price = 8.00m,  Category = focaccia, DisplayOrder = 5, ImageFileName = "810dc397f88173cbf35c00151c9c6207898efc6d.png" },
            new() { Name = "BuonGiorno Focaccia",   Description = "Green Pesto, Bruschetta, Mozzarella, Tomato, Rucola",                                                      Price = 7.40m,  Category = focaccia, DisplayOrder = 6, ImageFileName = "af572a7edfe1369fa9360ff88c44124c3237b03d.png" },
            new() { Name = "Hot Turkey Focaccia",   Description = "Cherry Pepper Cream Cheese, Truffle Cream, Rucola, Pulled Turkey, Tomato, Red Onions",                    Price = 8.40m,  Category = focaccia, DisplayOrder = 7, ImageFileName = "4769aaf09f8b28d0d9ae03faad7bda25be982f05.png" },
            new() { Name = "Pulled Turkey Focaccia", Description = "Cherry Pepper Cream Cheese, Rucola, Pulled Turkey, Tomato, Feta Cheese, Fried Onions",                   Price = 8.40m,  Category = focaccia, DisplayOrder = 8, ImageFileName = "5945f69ce4bdef0aece866eaaa65bf7c7096ecc9.png" },

            // Oats
            new() { Name = "Baked it your own",     Description = "Baked Oats Base",                                                                   IsVegan = true,         Price = 3.90m,  Category = oats, DisplayOrder = 1,  ImageFileName = "0b0bd8eccfd6f3f23211ffd32813cab94622f3e4.jpeg" },
            new() { Name = "Baked Oats Chocolate Heaven", Description = "Baked Oats Base – Banana, Blueberries, Chocolate toppings",                   IsVegan = true,         Price = 5.20m,  Category = oats, DisplayOrder = 2,  ImageFileName = "964400ffe5905a3b90f3dac84069602112584805.jpeg" },
            new() { Name = "Baked Oats Biscoff",    Description = "Baked Oats Base – Granola, Banana, Walnuts, Blueberries, Lotus Cream",              IsVegan = true,         Price = 5.40m,  Category = oats, DisplayOrder = 3,  ImageFileName = "44d57d4a0b38388fe03e8681ee10c63c79a0b041.jpeg" },
            new() { Name = "Baked Oats Pistachio Dream", Description = "Baked Oats Base – Granola, Pistachio Cream, Pistachio, Raspberry, Coconut",   IsVegan = true,         Price = 5.60m,  Category = oats, DisplayOrder = 4,  ImageFileName = "3630a59f9d064848fcaf646c1c6d8d6516f45bc7.jpeg" },
            new() { Name = "Baked Oats Power Day",  Description = "Baked Oats Base – Granola, Peanut Butter, Banana, Dates, Chia Seeds, Caramel",     IsVegan = true,         Price = 5.40m,  Category = oats, DisplayOrder = 5,  ImageFileName = "664c8946e105e0e9c54e0ea7f79bd22cb5cc1087.jpeg" },
            new() { Name = "Cold Oats Biscoff",     Description = "Cold Oats Base – Granola, Banana, Walnuts, Blueberries, Lotus Cream",               IsVegan = true,         Price = 6.20m,  Category = oats, DisplayOrder = 6,  ImageFileName = "d68157ba146e9700666f3f5bd01c179a60428b5e.jpeg" },
            new() { Name = "Cold Oats Pistachio Dream", Description = "Cold Oats Base – Granola, Pistachio Cream, Pistachios, Raspberries, Coconut",   IsVegan = true,         Price = 6.50m,  Category = oats, DisplayOrder = 7,  ImageFileName = "e384422484c31004cd05848024e111af9898fef4.jpeg" },
            new() { Name = "Cold Oats Power Day",   Description = "Cold Oats Base – Granola, Peanut Butter, Banana, Dates, Chia Seeds, Caramel",      IsVegan = true,         Price = 6.20m,  Category = oats, DisplayOrder = 8,  ImageFileName = "c6ae66f8bb03b6b3e5b31f7a4505a9201c45bd0f.jpeg" },
            new() { Name = "Cold Oats Chocolate Heaven", Description = "Cold Oats Base – Banana, Blueberry, Chocolate Chips, Agave Syrup",             IsVegan = true,         Price = 5.90m,  Category = oats, DisplayOrder = 9,  ImageFileName = "d3b560b278f374ce0cf6c65fb40ad54ddf305c7e.jpeg" },
            new() { Name = "Cold Oats",             Description = "Cold Oats, Apple, Oat Flakes, Vanilla Soy Milk",                                    IsVegan = true,         Price = 4.50m,  Category = oats, DisplayOrder = 10, ImageFileName = "5a204f38a243c687592a21176b6bddc40d405699.jpeg" },

            // Acai
            new() { Name = "Jana X GuMo Small",     Description = "Acai Base, Salted Caramel, Chia Pudding, Banana, Granola, Blueberry, Salt",                                Price = 12.00m, Category = acai, DisplayOrder = 1,  ImageFileName = "c4a53603fb6e8895f408831dc23aea2c91ced221.png" },
            new() { Name = "Jana X GuMo Large",     Description = "Acai Base, Salted Caramel, Chia Pudding, Banana, Granola, Blueberry, Salt",                                Price = 15.00m, Category = acai, DisplayOrder = 2,  ImageFileName = "c4a53603fb6e8895f408831dc23aea2c91ced221.png" },
            new() { Name = "Power Acai Small",      Description = "Acai Base, Oatmeal, Peanut Butter, Banana, Dates, Chia Seeds",                      IsVegan = true,         Price = 11.50m, Category = acai, DisplayOrder = 3,  ImageFileName = "020aad680ac956feab9aceb3fb36ee1d072a3435.png" },
            new() { Name = "Power Acai Large",      Description = "Acai Base, Oatmeal, Peanut Butter, Banana, Dates, Chia Seeds",                      IsVegan = true,         Price = 14.50m, Category = acai, DisplayOrder = 4,  ImageFileName = "020aad680ac956feab9aceb3fb36ee1d072a3435.png" },
            new() { Name = "Fruity Acai Small",     Description = "Acai Base, Banana, Blueberry, Raspberry, and Mango Puree",                          IsVegan = true,         Price = 11.20m, Category = acai, DisplayOrder = 5,  ImageFileName = "17a30bf11bcde714ea80fc2e292867676c0fa95f.png" },
            new() { Name = "Fruity Acai Large",     Description = "Acai Base, Banana, Blueberry, Raspberry, and Mango Puree",                          IsVegan = true,         Price = 14.20m, Category = acai, DisplayOrder = 6,  ImageFileName = "17a30bf11bcde714ea80fc2e292867676c0fa95f.png" },
            new() { Name = "Classy Acai Small",     Description = "Acai Base, Granola, Banana, Raspberry, Blueberry, Coconut Flakes",                                          Price = 10.95m, Category = acai, DisplayOrder = 7,  ImageFileName = "56f0fd0b2caf741922e00a9d947310b4e8fcae1f.png" },
            new() { Name = "Classy Acai Large",     Description = "Acai Base, Granola, Banana, Raspberry, Blueberry, Coconut Flakes",                                          Price = 13.95m, Category = acai, DisplayOrder = 8,  ImageFileName = "56f0fd0b2caf741922e00a9d947310b4e8fcae1f.png" },
            new() { Name = "Pistachio X Acai Small", Description = "Acai Base, Granola, Pistachio Butter, Chocolate Chips, Pistachios, Raspberries, Chia Seeds, Coconut",     Price = 11.95m, Category = acai, DisplayOrder = 9,  ImageFileName = "76ce9e77c812a49452d8fa953d4cf8485a79a84e.png" },
            new() { Name = "Pistachio X Acai Large", Description = "Acai Base, Granola, Pistachio Butter, Chocolate Chips, Pistachios, Raspberries, Chia Seeds, Coconut",     Price = 14.95m, Category = acai, DisplayOrder = 10, ImageFileName = "76ce9e77c812a49452d8fa953d4cf8485a79a84e.png" },

            // Juices
            new() { Name = "Mr Ginger Shot",        Description = "Ginger, Orange, Apple, Lemon",                                                      IsVegan = true,         Price = 3.00m,  Category = juices, DisplayOrder = 1,  ImageFileName = "817c71f379e8fbe56e660b596dec5ef8a0479cec.png" },
            new() { Name = "Mr Ginger 0.33l",       Description = "Orange, Apple, Lemon, Ginger",                                                      IsVegan = true,         Price = 5.70m,  Category = juices, DisplayOrder = 2,  ImageFileName = "4707391be5f5bbf99020565f33694c52653ea41b.png" },
            new() { Name = "Mr Ginger 0.5l",        Description = "Orange, Apple, Lemon, Ginger",                                                      IsVegan = true,         Price = 7.50m,  Category = juices, DisplayOrder = 3,  ImageFileName = "4707391be5f5bbf99020565f33694c52653ea41b.png" },
            new() { Name = "Sunrise Juice 0.33l",   Description = "Orange, Carrot, Lemon, Ginger, Mint",                                               IsVegan = true,         Price = 5.70m,  Category = juices, DisplayOrder = 4,  ImageFileName = "ea636c5d8f006b1fe9ecfa1dc2e5058f2dcadef9.png" },
            new() { Name = "Sunrise Juice 0.5l",    Description = "Orange, Carrot, Lemon, Ginger, Mint",                                               IsVegan = true,         Price = 7.70m,  Category = juices, DisplayOrder = 5,  ImageFileName = "ea636c5d8f006b1fe9ecfa1dc2e5058f2dcadef9.png" },
            new() { Name = "Red Juice 0.33l",       Description = "Beetroot, Carrot, Orange, Ginger, Lemon",                                           IsVegan = true,         Price = 5.50m,  Category = juices, DisplayOrder = 6,  ImageFileName = "14716c498dbd81122d4be9c28878ec14979d4f0a.png" },
            new() { Name = "Red Juice 0.5l",        Description = "Beetroot, Carrot, Orange, Ginger, Lemon",                                           IsVegan = true,         Price = 7.50m,  Category = juices, DisplayOrder = 7,  ImageFileName = "14716c498dbd81122d4be9c28878ec14979d4f0a.png" },
            new() { Name = "Green Juice 0.33l",     Description = "Apple, Cucumber, Lime",                                                             IsVegan = true,         Price = 5.30m,  Category = juices, DisplayOrder = 8,  ImageFileName = "c05fea37c683e3939617a60c63081796eae28f0e.png" },
            new() { Name = "Green Juice 0.5l",      Description = "Apple, Cucumber, Lime",                                                             IsVegan = true,         Price = 7.30m,  Category = juices, DisplayOrder = 9,  ImageFileName = "c05fea37c683e3939617a60c63081796eae28f0e.png" },
            new() { Name = "Orange Juice 0.33l",    Description = "Fresh Oranges",                                                                     IsVegan = true,         Price = 5.70m,  Category = juices, DisplayOrder = 10, ImageFileName = "133aca1ed5913498ffa5b6a83f39eacf63f4d3a4.png" },
            new() { Name = "Orange Juice 0.5l",     Description = "Fresh Oranges",                                                                     IsVegan = true,         Price = 7.70m,  Category = juices, DisplayOrder = 11, ImageFileName = "133aca1ed5913498ffa5b6a83f39eacf63f4d3a4.png" },
            new() { Name = "Apple Juice 0.33l",     Description = "Red Apple",                                                                         IsVegan = true,         Price = 5.30m,  Category = juices, DisplayOrder = 12, ImageFileName = "ee7a64bb4be4acd2d4551627b371777b3dee0bad.png" },
            new() { Name = "Apple Juice 0.5l",      Description = "Red Apple",                                                                         IsVegan = true,         Price = 7.30m,  Category = juices, DisplayOrder = 13, ImageFileName = "ee7a64bb4be4acd2d4551627b371777b3dee0bad.png" },

            // Sweet Treats
            new() { Name = "Banana Bread",          Description = null,                                                                                 IsVegan = true,         Price = 3.30m,  Category = sweets, DisplayOrder = 1, ImageFileName = "a0738401ec51a988159725b53959222a74009cd0.png" },
            new() { Name = "Cherry Matcha Banana Bread", Description = null,                                                                            IsVegan = true,         Price = 3.70m,  Category = sweets, DisplayOrder = 2, ImageFileName = "2677f1bdfda01896c4daa982157d27fcb30934f0.png" },
            new() { Name = "Zimtschnecken",         Description = null,                                                                                                        Price = 3.30m,  Category = sweets, DisplayOrder = 3 },
        };

        db.MenuItems.AddRange(items);

        await db.SaveChangesAsync();

        // The admin is only created when credentials are supplied via configuration
        // (Admin:Email / Admin:Password, e.g. user-secrets or environment variables).
        if (!userManager.Users.Any() && !string.IsNullOrWhiteSpace(adminEmail) && !string.IsNullOrWhiteSpace(adminPassword))
        {
            var admin = new IdentityUser { UserName = adminEmail, Email = adminEmail, EmailConfirmed = true };
            await userManager.CreateAsync(admin, adminPassword);
        }
    }
}
