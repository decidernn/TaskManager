using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Task_Manager
{
    public class PasswordHasher
    {
        public string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(password);
                byte[] hash = sha256.ComputeHash(bytes);

                StringBuilder errors = new StringBuilder();

                for (int i = 0; i < hash.Length; i++)
                {
                    errors.Append(hash[i].ToString("x2"));
                }

                return errors.ToString();
            }
        }
    }
}
