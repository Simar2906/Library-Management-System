using System.Diagnostics;
using System.Xml.Linq;
Library library = new Library();
Console.WriteLine("Welcome to my Library MS!!");
User user = User.Login();


//while (1)
//{
//    Console.WriteLine("-----------------------------------\nAvailable operations: ");
//    Console.WriteLine("1. Add a book.");
//    Console.WriteLine("2. View the library interview.");
//    Console.WriteLine("3. Issue a book from the inventory.");
//    Console.WriteLine("4. Return a book to the library.");
//    Console.ReadLine();
//}
//library.AddBook("Harry Potter", "JK Rowling");
//Console.WriteLine(library.Inventory.Count);
//foreach(var book in library.Inventory)
//{
//    Console.WriteLine(book.Name);
//}

public class Library
{
    public List<Book> Inventory;
    enum Genres {Fiction, Fantasy, Politics, History, News, Reference, Entertainment};
    public Library()
    {
        Inventory = new List<Book>();
    }
    public void AddBook(string Name, string Author)
    {
        Book book = new Book() { Name = Name, Author = Author };
        Inventory.Add(book);
        //Inventory.Append(book);
        Debug.WriteLine("Book added succefully");
        return;
    }
    public void AddBook(string Name, string Publisher, string Author, string Genre)
    {
        Book book = new Book() { Name = Name, Publisher = Publisher, Author=Author, Genre = Genre };
        Inventory.Append(book);
        Debug.WriteLine("Book added succefully");
        return;
    }
    
}
public class Item
{
    public string Name { get; set; }
    public string Publisher { get; set; }

}
public class Book:Item
{
    public string Author { get; set; } // extend this from another class    
    public string Genre { get; set; }
   
}
public class Magazine:Item
{
    public string Brand { get; set; }
}
public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
    public List<Book> issuedBooks;
    public User()
    {
        issuedBooks = new List<Book>();
    }
    public static User Login()
    {
        Console.Write("Please Enter Your Name: ");
        string name = Console.ReadLine();
        if (name == null)
        {
            name = "";
        }
        int libraryId = 0;
        bool validFlag = true;
        while(validFlag) // I know not a good practice, just for try catch
        {
            try
            {
                Console.Write("Please Enter Your Library Id No.: ");
                libraryId = Convert.ToInt32(Console.ReadLine());
                validFlag = false;
            }
            catch(Exception ex)
            {
                Console.WriteLine("Enter a Valid Id!!");
                Console.WriteLine(ex.Message);
            }
        }

        User user = new User() { Name = name, Id = libraryId };

        Console.WriteLine("Created new user successfully!");
        return user;
    }
}

