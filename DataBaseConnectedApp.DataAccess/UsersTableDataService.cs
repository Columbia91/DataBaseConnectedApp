using DataBaseConnectedApp.Models;
using System.Collections.Generic;
using System.Data.Common;
using System;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Configuration;

namespace DataBaseConnectedApp.DataAccess
{
    public class UsersTableDataService
    {
        private readonly string _connectionString; //readonly постоянная переменная дает возможность задать значени через = или через конструктор
        private readonly string _providerName;
        private readonly DbProviderFactory _providerFactory;

        public UsersTableDataService()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["testConnectionString"].ConnectionString;

            _providerName = ConfigurationManager.ConnectionStrings["testConnectionString"].ProviderName;

            _providerFactory = DbProviderFactories.GetFactory(_providerName);
        }


        public List<User> GetAllUsers()
        {
            string isAdminValue = ConfigurationManager.AppSettings["isAdmin"];
            if(bool.TryParse(isAdminValue, out var isAdmin) && isAdmin)
            {
                Console.WriteLine("***МОД АДМИНИСТРАТОРА***");
            }

            var data = new List<User>(); //буферный список пользователей

            using (var connection = _providerFactory.CreateConnection())
            using (var command = connection.CreateCommand())
            {
                try
                {
                    connection.ConnectionString = _connectionString;
                    connection.Open();

                    command.CommandText = "SELECT * FROM Users";

                    var dataReader = command.ExecuteReader();

                    while(dataReader.Read())
                    {
                        int id = (int)dataReader["Id"];
                        string login = dataReader["Login"].ToString();
                        string password = dataReader["Password"].ToString();

                        data.Add(new User
                        {
                            Id = id,
                            Login = login,
                            Password = password
                        });
                    }
                    dataReader.Close();
                }
                catch (DbException exception)
                {
                    //TODO обработка ошибки
                    throw;
                }
                catch (Exception exception)
                {
                    //TODO обработка ошибки
                    throw;
                }
            }
            return data;
        }

        public void AddUser(User user)
        {
            using (var connection = _providerFactory.CreateConnection())
            using (var command = connection.CreateCommand())
            {
                DbTransaction transaction = null;
                try
                {
                    connection.Open();
                    transaction = connection.BeginTransaction();

                    command.CommandText = $"INSERT into Users values(@login, @password)";
                    command.Transaction = transaction;

                    DbParameter loginParameter = command.CreateParameter();

                    loginParameter.ParameterName = "@login";
                    loginParameter.Value = user.Login;
                    loginParameter.DbType = System.Data.DbType.String;
                    loginParameter.IsNullable = false;

                    DbParameter passwordParameter = command.CreateParameter();

                    passwordParameter.ParameterName = "@password";
                    passwordParameter.Value = user.Password;
                    passwordParameter.DbType = System.Data.DbType.String;
                    passwordParameter.IsNullable = false;

                    command.Parameters.AddRange(new DbParameter[] { loginParameter, passwordParameter });

                    var affectedRows = command.ExecuteNonQuery(); //число строк которые подвергнуты каким либо изменениям 

                    if(affectedRows < 1 )
                    {
                        throw new Exception("Вставка не была произведена");
                    }
                    transaction.Commit();
                    transaction.Dispose();
                }
                catch (DbException exception)
                {
                    transaction?.Rollback();
                    transaction?.Dispose();
                    //TODO обработка ошибки
                    throw;
                }
                catch (Exception exception)
                {
                    if(transaction != null)
                        transaction.Rollback();
                    transaction?.Dispose();
                    //TODO обработка ошибки
                    throw;
                }
            }
        }

        public void DeleteUserById(int id)
        {

        }

        public void UpdateUser(User user)
        {

        }
    }
}
