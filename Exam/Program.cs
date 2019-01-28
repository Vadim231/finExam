using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace Exam
{
    class Program
    {
        static void Main(string[] args)
        {
            Menu();





        }

        public static void PhoneVerification(string number, int code)
        {
            string stringCode = code.ToString();
            string stringNumber = "+7" + number;
            const string accountSid = "ACcf90a2876dc97df1bf427acf3f49ebb7";
            const string authToken = "a9e24afceb4df932b661a2945ad6b14f";

            TwilioClient.Init(accountSid, authToken);

            var message = MessageResource.Create(
                body: stringCode,
                from: new Twilio.Types.PhoneNumber("+13044403947"),
                to: new Twilio.Types.PhoneNumber(stringNumber)
            );
        }

        public static void Menu()
        {
            Console.WriteLine("1.Регистрация");
            Console.WriteLine("2.Восстановление");

            int menuNumber;

            bool success = Int32.TryParse(Console.ReadLine(), out menuNumber);
            if (!success)
            {
                Console.WriteLine("Неверно выбран вариант");
            }
            

            switch (menuNumber)
            {
                case 1:
                    using (UserContext context = new UserContext())
                    {
                        Console.Write("Name: ");
                        string name = Console.ReadLine();

                        
                        Console.WriteLine("Phone Number(7XX XXX XXXX): ");
                        string inputString = Console.ReadLine();


                        //проверка
                        var users = from Users in context.Users
                                    where Users.PhoneNumber == inputString
                                    select Users;
                        

                        User userSearch = users.FirstOrDefault();
                        try
                        {
                            if (userSearch.IsConfirmed == true)
                            {
                                Console.WriteLine("На этот номер уже был зарегистрирован аккаун, восстановите пароль.");
                                break;
                            }
                        }
                        catch(Exception) { Console.WriteLine("Данный номер еще не регистрировался, продоложайте (:"); }


                        var user1 = new User
                        {
                            Name = name,
                            PhoneNumber = inputString,
                            IsConfirmed = false,
                            TempCode = "0"
                        };

                        Random random = new Random(0);
                        int temp = random.Next(1000);
                        PhoneVerification(user1.PhoneNumber, temp);

                        user1.TempCode = temp.ToString();

                        Console.WriteLine("Verification Number: ");
                        inputString = Console.ReadLine();
                        int inputCode = int.Parse(inputString);



                        if (inputCode == temp)
                        {
                            user1.IsConfirmed = true;
                            context.Users.Add(user1);
                            context.SaveChanges();

                            Console.Clear();
                            Console.WriteLine("Регистрация пройдена успешно! \n");
                            Menu();
                        }
                        else
                        {
                            Console.WriteLine("Неверный код подтверждения: ");
                            Menu();
                        }
                        break;
                    }
                case 2:
                    using (UserContext context = new UserContext())
                    {
                        Console.Clear();
                        Console.Write("Phone number: +7");
                        string inputNumber = Console.ReadLine();

                        var users = from Users in context.Users
                                    where Users.PhoneNumber == inputNumber
                                    select Users;

                        

                        User userSearch = users.FirstOrDefault();
                        if(userSearch.IsConfirmed==true)
                        {
                            Console.WriteLine("На этот номер выслан временный пароль.");
                            break;
                        }
                        Random random = new Random(0);
                        int temp = random.Next(1000);
                        PhoneVerification(inputNumber, temp);

                        Console.WriteLine("Enter code in SMS: ");
                        string inputPassword = Console.ReadLine();

                        if (inputPassword == temp.ToString())
                        {
                            userSearch.TempCode = temp.ToString();
                            context.SaveChanges();

                            Console.Clear();
                            Console.WriteLine("Восстановление пройдено успешно! \n");
                            Menu();
                        }
                        else
                        {
                            Console.WriteLine("Неверный код: ");
                            Menu();
                        }
                        break;
                    }
            }
        }
    }
}
