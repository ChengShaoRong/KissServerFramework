
using CSharpLike;
using KissFramework;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace KissServerFramework
{
    public class Utils
    {
        public static string HashPassword(string password)
        {
            return Framework.GetMD5(password.Trim()+Framework.config.passwordHashSalt);
        }
        static Regex emailRegex = new Regex(@"^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,5})+$");
        public static bool ValidMail(string address)
        {
            if (address.Length > 64)
                return false;
            return emailRegex.IsMatch(address);
        }
        static Regex guestRegex = new Regex(@"^([0-9]{2,9})+$");
        public static bool IsGuestName(string name)
        {
            return (name.StartsWith("guest#") && name.Length > 7 && guestRegex.IsMatch(name.Substring(6)));
        }
        static Regex nameRegex = new Regex(@"^([a-zA-Z0-9_\.\@\-]{6,64})+$");
        public static bool ValidName(string name)
        {
            if (IsGuestName(name))
                return true;
            return nameRegex.IsMatch(name);
        }
        public static bool ValidPassword(string password)
        {
            return (password.Length >= 6 && password.Length <= 64) || password.Length == 0;
        }

    }
}
