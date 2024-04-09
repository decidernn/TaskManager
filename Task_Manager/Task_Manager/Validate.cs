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
        public bool check_number(string number)
        {
            Regex phoneRegex = new Regex(@"^\+\d{1,3}\d{10}$");
            if (!phoneRegex.IsMatch(number))
                return false;
            return true;
        }

        public bool check_email(string email)
        {
            Regex emailRegex = new Regex(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$");
            if (!emailRegex.IsMatch(email))
                return false;
            return true;
        }

        public bool check_password(string password)
        {
            Regex passwordRegex = new Regex(@"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$");
            if (!passwordRegex.IsMatch(password))
                return false;
            return true;
        }
    }
}
