using System.Text;
using System.Security.Cryptography;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;


namespace H2TSHOP2024.Extension
{
    public static class HashMD5 
    {
        public static string ToMD5(this string str)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] hash = md5.ComputeHash(Encoding.UTF8.GetBytes(str));
            StringBuilder sbHash = new StringBuilder(); 
            foreach (byte b in hash)
            {
                sbHash.Append(String.Format("{0:x2}",b));
            }
            return sbHash.ToString();
        }
    }
}
