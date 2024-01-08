using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Xml.Linq;
using System.IO;
using System.Xml.Serialization;

namespace LibrarySystem
{

    public abstract class Item: IBorrowable
    {
        public string Name { get; set; }
        public virtual bool Borrow() 
        {
            return false;
        }
    }
    public interface IBorrowable
    {
        bool Borrow();
    }
    public class Library
    {
        public List<Item> Inventory;
        public int loggedInUser = 0;
        public int userDataId = 0;
        Library_Management_SystemEntities1 dbManager;
        enum Genres { Fiction, Fantasy, Politics, History, News, Reference, Entertainment };
        public Library()
        {
            Inventory = new List<Item>();
            dbManager = new Library_Management_SystemEntities1();
            var DataBooks =  dbManager.Books;
            foreach(var row in DataBooks)
            {
                Inventory.Add(row);
            }
            var DataMags = dbManager.Magazines;
            foreach (var row in DataMags)
            {
                Inventory.Add(row);
            }

        }
        private void MyBook_Borrowed(object sender, EventArgs e) // event handler
        {
            Console.WriteLine("Book has been borrowed!");
        }
        public void AddBook(string Name, string Author) // add book to list
        {
            Book book = new Book() { Name = Name, Author = Author };
            book.BookBorrowed += MyBook_Borrowed; //subscribed
            Inventory.Add(book);
            AddBookToDB(book);
            Debug.WriteLine("Book added succefully");
            Console.WriteLine("Book added succefully");

            return;
        }
        public void AddBook(string Name, string Publisher, string Author)
        {
            Book book = new Book() { Name = Name, Publisher = Publisher, Author = Author};
            Inventory.Add(book);
            book.BookBorrowed += MyBook_Borrowed; //subscribed
            AddBookToDB(book);
            Debug.WriteLine("Book added succefully");
            Console.WriteLine("Book added succefully");
     
            return;
        }
        public void AddBook(string Name, string Publisher, string Author, string Genre)
        {
            Book book = new Book() { Name = Name, Publisher = Publisher, Author = Author, Genre = Genre };
            Inventory.Add(book);
            book.BookBorrowed += MyBook_Borrowed; //subscribed
            AddBookToDB(book);
            Debug.WriteLine("Book added succefully");
            Console.WriteLine("Book added succefully");
            return;
        }
        public void AddMagazine(string Name, string Brand)
        {
            Magazine mag = new Magazine() { Name = Name, Brand = Brand };
            Inventory.Add(mag);
            AddMagToDb(mag);
            Debug.WriteLine("Magazine added succefully");
            Console.WriteLine("Magazine added successfully!");
            return;
        }
        public void AddMagazine(string Name, string Publisher, string Brand)
        {
            Magazine mag = new Magazine() { Name = Name, Publisher = Publisher, Brand = Brand};
            Inventory.Add(mag);
            Debug.WriteLine("Magazine added succefully");
            AddMagToDb(mag);
            Debug.WriteLine("Magazine added succefully");
            Console.WriteLine("Magazine added successfully!");
            return;
        }
        public void AddBookToDB(Book book)
        {
            //add object to DB
            
            dbManager.Books.Add(book);
            dbManager.SaveChanges();
        }
        public void AddMagToDb(Magazine mag)
        {
            //add object to DB
            
            dbManager.Magazines.Add(mag);
            dbManager.SaveChanges();
        }
        public void AddNewItem()
        {
            bool p = true;
            while(p)
            {
                Console.Write("Is your item a book or a magazine?(Book:1 or Mag:2): ");
                int choice = Convert.ToInt32(Console.ReadLine());

                if (choice == 1) // add book
                {
                    string name = "", author = "", publisher= "", genre="";
                    Console.Write("Enter Title of Book: ");
                    name = Console.ReadLine();

                    Console.Write("Enter Author of Book: ");
                    author = Console.ReadLine();

                    Console.Write("Enter Optional Fields?(y or n): ");
                    char option = Convert.ToChar(Console.ReadLine());
                    if(option == 'n')
                    {
                        //Add to database
                        AddBook(name, author);
                        p = false;
                        LogDataToFile(name);
                        break;
                    }
                    Console.Write("Enter Publisher of Book: ");
                    publisher = Console.ReadLine();

                    Console.Write("Enter Genre of Book [copy paste{ Fiction, Fantasy, Politics, History, News, Reference, Entertainment }]: ");
                    genre = Console.ReadLine();

                    //Add to database
                    AddBook(name, publisher, author, genre);
                    p = false;
                    LogDataToFile(name);
                }
                else if(choice == 2)
                {
                    string name="", brand = "", publisher = "";
                    Console.Write("Enter Name of Magazine: ");
                    name = Console.ReadLine();

                    Console.Write("Enter Brand of Magazine: ");
                    brand = Console.ReadLine();

                    Console.Write("Enter Optional Fields?(y or n): ");
                    char option = Convert.ToChar(Console.ReadLine());
                    if (option == 'n')
                    {
                        //Add to database
                        AddMagazine(name, brand);
                        p = false;
                        LogDataToFile(name);
                        break;
                    }
                    Console.Write("Enter Publisher of Magazine: ");
                    publisher = Console.ReadLine();

                    //Add to database
                    AddMagazine(name, publisher, brand);
                    LogDataToFile(name);
                    p = false;
                }
                else
                {
                    Console.WriteLine("Enter a Valid option!");
                }
            }
            
        }
        private void LogDataToFile(string name) //serialisation
        {
            string path = @"C:\Training Resources\Week C#\Assignments\Library Management System\LibrarySystem\Logs.txt";
            FileStream fileMangager = new FileStream(path, FileMode.Append);
            StreamWriter writer = new StreamWriter(fileMangager);
            writer.WriteLine($"{name} was added to the Inventory by User-{userDataId}- at {DateTime.Now.ToLongTimeString()} {DateTime.Now.ToLongDateString()}");
            writer.Flush();
            fileMangager.Close();
            Console.WriteLine("Data appended!");
        }
        public void BorrowItem()
        {
            Dictionary<int, Item> mp = new Dictionary<int, Item>();
            int i = 1;
            int choice;
            Console.WriteLine("Items available for borrowing are: ");
            foreach (var items in Inventory)
            {
                if (items.Borrow())
                {
                    mp[i] = items;
                    Console.WriteLine($"{i} : "+ items.Name);
                    i++;
                }
            }
            Console.Write("Enter your choice: ");
            choice = Convert.ToInt32(Console.ReadLine()); // not adding exception handling in this as already implemented

            //Console.WriteLine(userDataId);
            if (mp[choice] is Book book) //checking which child
            {
                string names = book.Name;
                var row = (from rowe in dbManager.Books
                           where rowe.Name == names && rowe.IssuedTo == null
                           select rowe).FirstOrDefault();
                row.IssuedTo = userDataId;
                dbManager.SaveChanges();
                book.OnBookBorrowed(); // event
            }
            else if (mp[choice] is Magazine Mag) //checking which child
            {
                string names = Mag.Name;
                var row = (from rowe in dbManager.Magazines
                           where rowe.Name == names && rowe.IssuedTo == null
                           select rowe).FirstOrDefault();
                row.IssuedTo = userDataId;
                dbManager.SaveChanges();
            }

            Console.WriteLine("Item Successfully Borrowed! Do remember to return it!\n");
        }
        public void ViewInventory()
        {
            
            Console.WriteLine("Current Items in the Library Are:- ");
            foreach (var item in Inventory)
            {
                Console.WriteLine(item.Name);
            }
            Console.WriteLine();
        }
        public void ReturnItem()
        {
            var data = (from row in dbManager.Books
                        where row.IssuedTo == userDataId
                        select row).ToList();
            if (data != null)
            {
                Dictionary<int, Item> mp = new Dictionary<int, Item>();
                int i = 1;
                int choice;
                Console.WriteLine("Items Currently Issued to you are: ");
                foreach(var item in data)
                {
                    mp[i] = item;
                    Console.WriteLine($"{i}. {item.Name}");
                    i++;
                }
                Console.Write("Which one do you want to return: ");
                choice = Convert.ToInt32(Console.ReadLine());
                if (mp[choice] is Book book) //checking which child
                {
                    string names = book.Name;
                    var row = (from rowe in dbManager.Books
                               where rowe.Name == names
                               select rowe).FirstOrDefault();
                    row.IssuedTo = null;
                    dbManager.SaveChanges();
                    book.OnBookBorrowed(); // event
                }
                else if (mp[choice] is Magazine Mag) //checking which child
                {
                    string names = Mag.Name;
                    var row = (from rowe in dbManager.Magazines
                               where rowe.Name == names
                               select rowe).FirstOrDefault();
                    row.IssuedTo = null;
                    dbManager.SaveChanges();
                }
                Console.WriteLine("Book Returned!");
            }
            else
            {
                Console.WriteLine("No items Issued!");
            }
        }
        public static void ChoiceMenu(Library library)
        {
            int choice;
            bool p = true;
            while (p)
            {
                Console.WriteLine("-----------------------------------\nAvailable operations: ");
                Console.WriteLine("1. Add a book or magazine.");
                Console.WriteLine("2. View the library inventory.");
                Console.WriteLine("3. Issue a book from the inventory.");
                Console.WriteLine("4. Return a book to the library.");
                Console.WriteLine("5. Exit");
                Console.Write("Enter Your Choice: ");
                choice = Convert.ToInt32(Console.ReadLine());
                Console.WriteLine();
                switch (choice) 
                {
                    case 1:
                        //Add Either Book or Magazine
                        library.AddNewItem();
                        break;
                    case 2:
                        //View Inventory
                        library.ViewInventory();
                        break;
                    case 3:
                        //Issue a book/Magazine
                        library.BorrowItem();
                        break;
                    case 4:
                        //Return a book/Magazine
                        library.ReturnItem();
                        break;
                    case 5:
                        p = false;
                        break;
                    default:
                        Console.WriteLine("Enter a Valid Option!");
                        continue;
                }
            }
        }
        
    }
    //public class Item // Earlier implemented to show polymorphism and inheritance, changed due to database practice
    //{
    //    public string Name { get; set; }
    //    public string Publisher { get; set; }

    //}
    //public class Book : Item
    //{
    //    public string Author { get; set; } // extend this from another class    
    //    public string Genre { get; set; }

    //}
    //public class Magazine : Item
    //{
    //    public string Brand { get; set; }
    //}
    
    internal class Program
    {
        
        public static void Login(Library library)
        {
            Library_Management_SystemEntities1 dbManager = new Library_Management_SystemEntities1();

            Console.Write("Please Enter Your Name: ");
            string name = Console.ReadLine();
            if (name == null)
            {
                name = "";
            }
            name = name.ToLower();
            int libraryId = 0;
            bool validFlag = true;
            while (validFlag) // I know not a good practice, just for try catch
            {
                try
                {
                    Console.Write("Please Enter Your Library Id No.: ");
                    libraryId = Convert.ToInt32(Console.ReadLine());
                    validFlag = false;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Enter a Valid Id!!");
                    Console.WriteLine(ex.Message);
                }
            }
            
            var Data = (from row in dbManager.Users
                        where row.Name == name && row.LibraryId == libraryId
                        select row);
            if (Data.Any())
            {
                Console.WriteLine($"Logged in Successfully!\nWelcome Back {Data.FirstOrDefault().Name}");
            }
            else
            {
                User user = new User() { Name = name, LibraryId = libraryId };
                dbManager.Users.Add(user);
                dbManager.SaveChanges();
                Console.WriteLine("Created new user successfully!");
            }
            library.loggedInUser = libraryId;
            int userDataId = (from rowe in dbManager.Users
                          where rowe.LibraryId == libraryId
                              select rowe.UserIdentifier).FirstOrDefault();
            library.userDataId = userDataId;

            return;
        }
        static void Main(string[] args)
        {

            Library library = new Library();
            Console.WriteLine("Welcome to my Library MS!!");
            Login(library);

            Library.ChoiceMenu(library);
        }
    }
}
