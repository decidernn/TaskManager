using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Task_Manager
{
    public class Validator
    {
        public bool check_phone(string phoneNumber)
        {
            return phoneNumber.Length >= 11 && phoneNumber.Length <= 12;
        }

        public bool check_login (string login)
        {
            return login.Length >= 4;
        }

        public bool check_email(string email)
        {
            return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        }

        public bool check_password(string password)
        {
            return password.Length >= 8 && Regex.IsMatch(password, @"(?=.*[A-Z])(?=.*\d.*\d)(?=.*[^\da-zA-Z]).*$");
        }
    }
}
