﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Project1Afdemp
{
    class UserManager
    {
        public User TheUser { get; private set; }
        public string UserName { get; private set; }
        public string Password { get; private set; }
        public Accessibility UserAccess { get; private set; }
        private int LoginTries { get; set; }

        #region Constructors

        public UserManager(string userName, string password, bool isNewUser = false)
        {
            LoginTries = 1;
            if (IsWrongUserName(userName, isNewUser))
            {
                AskUserName(isNewUser);
            }
            else
            {
                UserName = userName;
            }
            LoginTries = 1;
            if (IsWrongPassword(password, isNewUser))
            {
                AskPassword(isNewUser);
            }
            else
            {
                Password = PasswordHandling.PasswordToHash(PasswordHandling.ConvertToSecureString(password), UserName);
            }
            SetAccessibility(isNewUser);
            // If is new user create a user to database
            using (var database = new DatabaseStuff())
            {
                if (isNewUser)
                {
                    try
                    {
                        database.Users.Add(new User(UserName, Password, UserAccess));
                        database.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message + " User could not get created");
                    }
                }
                TheUser = database.Users.Single(u => u.UserName == UserName);
            }
        }

        public UserManager(bool isNewUser = false) : this("", "", isNewUser) { }
        #endregion

        #region UserName Methods
        private void AskUserName(bool isNewUser)
        {
            Console.Clear();
            Console.Write("\n\n\tUser Name:\t");
            string userName = Console.ReadLine();
            while (IsWrongUserName(userName, isNewUser) && LoginTries < 3)
            {
                Console.Write("\n\n\tUser Name:\t");
                userName = Console.ReadLine();
                LoginTries++;   
            }
            if (LoginTries == 3 && IsWrongUserName(userName, isNewUser))
            {
                throw new ArgumentException("\n\n\tToo many false attempts");
            }
            UserName = userName;
        }

        private bool IsWrongUserName(string userName, bool isNewUser)
        {
            Console.Clear();
            Console.BackgroundColor = ConsoleColor.Red;
            if (userName.Length < 5 || userName.Length > 14)
            {
                Console.Write("\n\n\tUser Name has to be between 5 and 14 characters long!");
                Console.ResetColor();
                return true;
            }
            else if (userName.Contains(' '))
            {
                Console.Write("\n\n\tUser Name cannot contain spaces! Try again");
                Console.ResetColor();
                return true;
            }
            else if (UserNameAlreadyExists(userName) && isNewUser)
            {
                Console.Write("\n\n\tUser Name already exists! Try another");
                Console.ResetColor();
                return true;
            }
            else if (!UserNameAlreadyExists(userName) && !isNewUser)
            {
                Console.Write("\n\n\tThat username doesn't exist! Check again.");
                Console.ResetColor();
                return true;
            }
            Console.ResetColor();
            return false;
        }

        private bool UserNameAlreadyExists(string userName)
        {
            using (var database = new DatabaseStuff())
            {
                return database.Users.Any(i => i.UserName == userName);
            }
        }
        #endregion

        #region Password Methods
        private void AskPassword(bool isNewUser)
        {
            Console.Clear();
            Console.Write("\n\n\tPassword:\t");
            string password = PasswordHandling.GetPassword();
            while (IsWrongPassword(password, isNewUser) && LoginTries < 3)
            {
                Console.Write("\n\n\tPassword:\t");
                password = PasswordHandling.GetPassword();
                LoginTries++;
            }
            if (LoginTries == 3 && IsWrongPassword(password, isNewUser))
            {
                throw new ArgumentException("\n\n\tToo many false attempts");
            }
            Password = PasswordHandling.PasswordToHash(PasswordHandling.ConvertToSecureString(password), UserName);
        }

        private bool IsWrongPassword(string password, bool isNewUser)
        {
            Console.Clear();
            Console.BackgroundColor = ConsoleColor.Red;
            if (password.Length < 5 || password.Length > 20)
            {
                Console.Write("\n\n\tPassword has to be between 5 and 20 characters long!");
                Console.ResetColor();
                return true;
            }
            else if (password.Contains(' '))
            {
                Console.Write("\n\n\tPassword cannot contain spaces! Try again");
                Console.ResetColor();
                return true;
            }
            else if (!isNewUser && !IDmatched(password))
            {
                Console.Write("\n\n\tWrong Password! Try again.");
                Console.ResetColor();
                return true;
            }
            Console.ResetColor();
            return false;
        }

        private bool IDmatched(string password)
        {
            using (var database = new DatabaseStuff())
            {
                string passHash = database.Users.Single(i => i.UserName == UserName).Password;
                string givenPass = PasswordHandling.PasswordToHash(PasswordHandling.ConvertToSecureString(password), UserName);
                return (givenPass == passHash);
            }
        }
        #endregion

        private void SetAccessibility(bool isNewUser)
        {
            if (isNewUser)
            {
                if (UserName == "admin")
                {
                    UserAccess = Accessibility.administrator;
                }
                else if (UserName == "guest")
                {
                    UserAccess = Accessibility.guest;
                }
                else
                {
                    UserAccess = Accessibility.user;
                }
            }
            else
            {
                using (var database = new DatabaseStuff())
                {
                    UserAccess = database.Users.Single(u => u.UserName == UserName).UserAccess;
                }
            }  
        }

        public void ClearUnreadChat()
        {
            using(var database = new DatabaseStuff())
            {
                TheUser = database.Users.Single(u => u.UserName == UserName);
                List<ChatMessage> UnreadChatMessages = database.Chat.Include("UnreadUsers").Where(c=> c.SenderId!=TheUser.Id).ToList();

                foreach(ChatMessage message in UnreadChatMessages)
                {
                    message.UnreadUsers.Remove(TheUser);
                }
                database.SaveChanges();
            }
        }
    }
}
