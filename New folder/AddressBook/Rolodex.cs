using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq.Expressions;

namespace AddressBook
{
    public class Rolodex
    {
        public Rolodex(string connectionString, string contactsFileName)
        {
            _contactsFileName = contactsFileName;
            _connectionString = connectionString;
            _contacts = new List<Contact>();
            _recipes = new Dictionary<RecipeType, List<Recipe>>();

            _recipes.Add(RecipeType.Appetizers, new List<Recipe>());
            _recipes[RecipeType.Entreés] = new List<Recipe>();
            _recipes.Add(RecipeType.Desserts, new List<Recipe>());
        }

    public void DoStuff()
{
    // Print a menu
    ShowMenu();
    
    // Get the user's choice
    MenuOption choice = GetMenuOption();
    List<Contact> contacts = ReadAllContacts();
    // while the user does not want to exit
    while (choice != MenuOption.Exit)
    {
        // figure out what they want to do
        // get information
        // do stuff
        if (choice == MenuOption.ListContacts || 
            choice == MenuOption.SearchContacts || 
            choice == MenuOption.RemoveContact)
        {
            if (contacts.Count != 0) // Only proceed if there are contacts
            {
                switch (choice)
                {
                    case MenuOption.ListContacts:
                        DoListContacts();
                        break;
                    case MenuOption.SearchContacts:
                        DoSearchContacts();
                        break;
                    case MenuOption.RemoveContact:
                        DoRemoveContact();
                        break;
                }
            }
            else
            {
                Console.WriteLine("No contacts available.");
            }
        }
        else // For other options
        {
            switch (choice)
            {
                case MenuOption.AddPerson:
                    DoAddPerson();
                    break;
                case MenuOption.AddCompany:
                    DoAddCompany();
                    break;
                case MenuOption.AddRecipe:
                    DoAddRecipe();
                    break;
                case MenuOption.SearchEverything:
                    DoSearchEverything();
                    break;
                case MenuOption.ListReceipes:
                    DoListRecipes();
                    break;
            }
        }
        
        ShowMenu();
        choice = GetMenuOption();
    }
}

private Dictionary<RecipeType, List<Recipe>> LoadRecipesFromFile()
{
    var loadedRecipes = new Dictionary<RecipeType, List<Recipe>>();
    foreach (RecipeType recipeType in Enum.GetValues(typeof(RecipeType))) {
        loadedRecipes[recipeType] = new List<Recipe>();
    }
    if (!File.Exists(_contactsFileName)) return loadedRecipes;

    using (StreamReader reader = new StreamReader(_contactsFileName)) {
        while (!reader.EndOfStream) {
            var line = reader.ReadLine();
            var parts = line.Split('|');

            if(parts.Length == 3 && parts[0] == "Recipe") {
                var title = parts[1];
                if(Enum.TryParse(parts[2], out RecipeType recipeType)) {
                    loadedRecipes[recipeType] = new List<Recipe>();
                }
                loadedRecipes[recipeType].Add(new Recipe(title));
            }
        }
    }
    return loadedRecipes;
}

private void DoListRecipes()
{
    Console.Clear();
    Console.WriteLine("RECIPES!");
    _recipes = LoadRecipesFromFile();
    foreach (RecipeType recipeType in Enum.GetValues(typeof(RecipeType)))
{
    if (_recipes.TryGetValue(recipeType, out List<Recipe> specificRecipes))
    {
        if (specificRecipes.Count > 0)
        {
            Console.WriteLine(recipeType.ToString().ToUpper());
            foreach (Recipe recipe in specificRecipes)
            {
                Console.WriteLine($"  {recipe}"); // Display the recipe as is
            }
        }
    }
}


    Console.ReadLine();
}


        //private void DoSearchEverything()
        //{
        //    Console.Clear();
        //    Console.WriteLine("SEARCH EVERYTHING!");
        //    Console.Write("Please enter a search term: ");
        //    string term = GetNonEmptyStringFromUser();

        //    List<Contact> contacts = ReadAllContacts();
        //    List<IMatchable> matchables = new List<IMatchable>();
        //    matchables.AddRange(contacts);

        //    using (SqlConnection connection = new SqlConnection(_connectionString))
        //    {
        //        connection.Open();
        //        SqlCommand command = connection.CreateCommand();
        //        command.CommandText = @"
        //            SELECT Name
        //              FROM Recipes
        //          ORDER BY Name
        //        ";

        //        SqlDataReader reader = command.ExecuteReader();

        //        while (reader.Read())
        //        {
        //            string recipeTitle = reader.GetString(0);
        //            Recipe recipe = new Recipe(recipeTitle);
        //            matchables.Add(recipe);
        //        }
        //    }

        //    foreach (IMatchable matcher in matchables)
        //    {
        //        if (matcher.Matches(term))
        //        {
        //            Console.WriteLine($"> {matcher}");
        //        }
        //    }
        //    Console.ReadLine();
        //}

        private void DoSearchEverything()
        {
            Console.Clear();
            Console.WriteLine("SEARCH EVERYTHING!");
            Console.Write("Please enter a search term: ");
            string term = GetNonEmptyStringFromUser();

            List<Contact> contacts = ReadAllContacts();
            List<IMatchable> matchables = new List<IMatchable>();
            matchables.AddRange(contacts);

            // using (SqlConnection connection = new SqlConnection(_connectionString))
            // {
            //     connection.Open();
            //     SqlCommand command = connection.CreateCommand();
            //     command.CommandText = @"
            //         SELECT Name
            //           FROM Recipes
            //       ORDER BY Name
            //     ";

            //     SqlDataReader reader = command.ExecuteReader();

            //     while (reader.Read())
            //     {
            //         string recipeTitle = reader.GetString(0);
            //         Recipe recipe = new Recipe(recipeTitle);
            //         matchables.Add(recipe);
            //     }
            // }
            var loadedRecipes = LoadRecipesFromFile();
            foreach (var recipeList in loadedRecipes.Values)
            {
                matchables.AddRange(recipeList);
            }
            foreach (IMatchable matcher in matchables)
            {
                if (matcher.Matches(term))
                {
                    Console.WriteLine($"> {matcher}");
                }
            }
            Console.ReadLine();
        }

        private void DoAddRecipe()
        {
            Console.Clear();
            Console.WriteLine("Please enter your recipe title:");
            string title = GetNonEmptyStringFromUser();
            Recipe recipe = new Recipe(title);

        
            Console.WriteLine("What kind of recipe is this?");

            for (int i = 0; i < (int)RecipeType.UPPER_LIMIT; i += 1)
            {
                Console.WriteLine($"{i}. {(RecipeType)i}");
            }

            int choice;
            if (int.TryParse(Console.ReadLine(), out choice) && choice >= 0 && choice < (int)RecipeType.UPPER_LIMIT)
            {
                RecipeType recipeType = (RecipeType)choice;
                List<Recipe> specificRecipes = _recipes[recipeType];
                specificRecipes.Add(recipe);
                PutInFile(recipe, recipeType);
                // using (SqlConnection connection = new SqlConnection(_connectionString))
                // {
                //     connection.Open();

                //     SqlCommand command = connection.CreateCommand();
                //     command.CommandText = @"
                //         insert into recipes(recipetypeid, name)
                //         values(@giraffe, @lemur)
                //     ";
                //     command.Parameters.AddWithValue("@giraffe", recipeType);
                //     command.Parameters.AddWithValue("@lemur", title);
                //     command.ExecuteNonQuery();
                // }
            }
            else
            {
                Console.WriteLine("Invalid choice. Please enter a valid recipe type.");
                Console.ReadLine();
            }

        }

        private void PutInFile(Recipe recipe, RecipeType recipeType) {
            using(StreamWriter writer = File.AppendText(_contactsFileName)) {
                string title = recipe.GetTitle();
                writer.WriteLine(string.Join("|", "Recipe", title, recipeType.ToString()));
            }
        }

        private void DoRemoveContact()
        {
            Console.Clear();
            Console.WriteLine("REMOVE A CONTACT!");
            Console.Write("Search for a contact: ");
            string term = GetNonEmptyStringFromUser();

            List<Contact> contacts = ReadAllContacts();
            File.Delete(_contactsFileName);
            if (contacts.Count!=0){
            foreach (Contact contact in contacts)
            {
                if (contact.Matches(term))
                {
                    Console.Write($"Remove {contact}? (y/N)");
                    if (Console.ReadLine().ToLower() == "y")
                    {
                        continue;
                    }
                    else
                    {
                        PutInFile(contact);
                    }
                }
                else
                {
                    PutInFile(contact);
                }
            }}else
            {
            Console.WriteLine("No more contacts found.");
            }
            Console.WriteLine("Press Enter to return to the menu...");
            Console.ReadLine();
        }

        private void DoSearchContacts()
        {
            Console.Clear();
            Console.WriteLine("SEARCH!");
            Console.Write("Please enter a search term: ");
            string term = GetNonEmptyStringFromUser();

            List<Contact> contacts = ReadAllContacts();
            if(contacts.Count!=0){
            foreach (Contact contact in contacts)
            {
                if (contact.Matches(term))
                {
                    Console.WriteLine($"> {contact}");
                }
            }}else
            {
                Console.WriteLine("No contacts found");
            }

            Console.WriteLine("Press Enter to continue...");
            Console.ReadLine();
        }

        private void DoListContacts()
        {
            Console.Clear();
            Console.WriteLine("YOUR CONTACTS");

            List<Contact> contacts = ReadAllContacts();

            foreach (Contact contact in contacts)
            {
                Console.WriteLine(contact);
            }

            Console.ReadLine();
        }

        private List<Contact> ReadAllContacts()
        {
            List<Contact> contacts = new List<Contact>();
            try {
            using (StreamReader reader = File.OpenText(_contactsFileName))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    string[] parts = line.Split('|');

                    if (parts[0] == "Company")
                    {
                        Company company = new Company(parts[1], parts[2]);
                        contacts.Add(company);
                    }
                    else if (parts[0] == "Person")
                    {
                        Person person = new Person(parts[1], parts[2], parts[3]);
                        contacts.Add(person);
                    }
                    else
                    {
                        //Console.WriteLine("You have junk in your contacts file.");
                    }
                }
            }
            } catch (Exception ex) {
                // Console.WriteLine("No file found sorry try agai")
            }
            contacts.Sort();

            return contacts;
        }

        private void DoAddCompany()
        {
            Console.Clear();
            Console.WriteLine("Please enter information about the company.");
            Console.Write("Company name: ");
            string name = Console.ReadLine();

            Console.Write("Phone number: ");
            string phoneNumber = GetNonEmptyStringFromUser();

            PutInFile(new Company(name, phoneNumber));
        }

        private void DoAddPerson()
        {
            Console.Clear();
            Console.WriteLine("Please enter information about the person.");
            Console.Write("First name: ");
            string firstName = Console.ReadLine();

            Console.Write("Last name: ");
            string lastName = GetNonEmptyStringFromUser();

            Console.Write("Phone number: ");
            string phoneNumber = GetNonEmptyStringFromUser();

            PutInFile(new Person(firstName, lastName, phoneNumber));
        }

        private void PutInFile(Person person)
        {
            using (StreamWriter writer = File.AppendText(_contactsFileName))
            {
                string firstName = person.GetFirstName();
                string lastName = person.GetLastName();
                string phoneNumber = person.GetPhoneNumber();
                writer.WriteLine(string.Join("|", "Person", firstName, lastName, phoneNumber));
            }
        }

        private void PutInFile(Company company)
        {
            using (StreamWriter writer = File.AppendText(_contactsFileName))
            {
                string name = company.GetName();
                string phoneNumber = company.GetPhoneNumber();
                writer.WriteLine(string.Join("|", "Company", name, phoneNumber));
            }
        }

        private void PutInFile(Contact contact)
        {
            Person person = contact as Person;
            if (person != null)
            {
                PutInFile(person);
            }

            Company company = contact as Company;
            if (company != null)
            {
                PutInFile(company);
            }
        }

        private string GetNonEmptyStringFromUser()
        {
            string input = Console.ReadLine();
            while (input.Length == 0)
            {
                Console.WriteLine("That is not valid.");
                input = Console.ReadLine();
            }
            return input;
        }

        private int GetNumberFromUser()
        {
            while (true)
            {
                try
                {
                    string input = Console.ReadLine();
                    return int.Parse(input);
                }
                catch (FormatException)
                {
                    Console.WriteLine("You should type a number.");
                }
                catch (InvalidOperationException)
                {
                    Console.WriteLine("THAT WAS BAD! DO AGAIN!");
                }
                finally
                {
                    Console.WriteLine("THIS will ALWAYS be PRINTED.");
                }
                
            }
        }


        private MenuOption GetMenuOption()
        {
            while (true)
            {
                try
                {
                    int choice = GetNumberFromUser();

                    if (choice >= 0 && choice < (int)MenuOption.UPPER_LIMIT)
                    {
                        return (MenuOption)choice;
                    }
                    else
                    {
                        Console.WriteLine("Invalid menu option. Please choose a valid option.");
                    }
                }
                catch (FormatException)
                {
                    Console.WriteLine("You should type a number.");
                }
                catch (InvalidOperationException)
                {
                    Console.WriteLine("THAT WAS BAD! DO AGAIN!");
                }
                finally
                {
                    Console.WriteLine("THIS will ALWAYS be PRINTED.");
                }
            }
        }

        private void ShowMenu()
        {
            Console.Clear();
            Console.WriteLine($"ROLODEX! ({_contacts.Count}) ({_recipes.Count})");
            Console.WriteLine("1. Add a person");
            Console.WriteLine("2. Add a company");
            Console.WriteLine("3. List all contacts");
            Console.WriteLine("4. Search contacts");
            Console.WriteLine("5. Remove a contact");
            Console.WriteLine("-----------------------");
            Console.WriteLine("6. Add a recipe");
            Console.WriteLine("7. List recipes");
            Console.WriteLine("-----------------------");
            Console.WriteLine("8. Search everything!");
            Console.WriteLine();
            Console.WriteLine("0. Exit");
            Console.WriteLine();
            Console.Write("What would you like to do? ");
        }

        private readonly List<Contact> _contacts;
        private Dictionary<RecipeType, List<Recipe>> _recipes;
        private readonly string _connectionString;
        private readonly string _contactsFileName;

        
    }
}
