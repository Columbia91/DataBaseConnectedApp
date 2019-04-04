using DataBaseConnectedApp.DataAccess;
using DataBaseConnectedApp.Models;

namespace DataBaseConnectedApp.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var dataService = new UsersTableDataService();
            dataService.AddUser(new User
            {
                Login = "admin",
                Password = "root"
            });
            foreach( var user in dataService.GetAllUsers())
            {
                System.Console.WriteLine($"{user.Id},{user.Login},{user.Password}");
            }
            System.Console.ReadLine();
        }
    }
}

