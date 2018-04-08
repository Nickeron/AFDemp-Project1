using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using System.Text;

namespace Project1Afdemp
{
    static class PasswordHandling
    {
        public static String PasswordToHash(SecureString password, string username)
        {
            StringBuilder StringBuild = new StringBuilder();
            IntPtr valuePtr = IntPtr.Zero;
            try
            {
                valuePtr = Marshal.SecureStringToGlobalAllocUnicode(password);
                string pass = Marshal.PtrToStringUni(valuePtr);
                using (SHA256 hash = SHA256.Create())
                {
                    Encoding enc = Encoding.UTF8;

                    // The username is the salt. 
                    // So 2 users with same password have different hashes. 
                    // For example if someone knows his own hash he can't see who has same password
                    string input = pass + username;

                    Byte[] result = hash.ComputeHash(enc.GetBytes(input));
                    
                    foreach (Byte b in result)
                        StringBuild.Append(b.ToString("x2")); // Can also use other encodings like BASE64 
                }
                return StringBuild.ToString();
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(valuePtr);
            }   
        }

        public static bool SecureCompare(this SecureString ss1, SecureString ss2)
        {
            IntPtr bstr1 = IntPtr.Zero;
            IntPtr bstr2 = IntPtr.Zero;
            try
            {
                bstr1 = Marshal.SecureStringToBSTR(ss1);
                bstr2 = Marshal.SecureStringToBSTR(ss2);
                int length1 = Marshal.ReadInt32(bstr1, -4);
                int length2 = Marshal.ReadInt32(bstr2, -4);
                if (length1 == length2)
                {
                    for (int x = 0; x < length1; ++x)
                    {
                        byte b1 = Marshal.ReadByte(bstr1, x);
                        byte b2 = Marshal.ReadByte(bstr2, x);
                        if (b1 != b2) return false;
                    }
                }
                else return false;
                return true;
            }
            finally
            {
                if (bstr2 != IntPtr.Zero) Marshal.ZeroFreeBSTR(bstr2);
                if (bstr1 != IntPtr.Zero) Marshal.ZeroFreeBSTR(bstr1);
            }
        }

        public static String GetPassword()
        {
            string password = "";
            while (true)
            {
                ConsoleKeyInfo keyPressed = Console.ReadKey(true);
                if (keyPressed.Key == ConsoleKey.Enter)
                {
                    break;
                }
                else if (keyPressed.Key == ConsoleKey.Backspace)
                {
                    if (password.Length > 0)
                    {
                        password.Remove(password.Length - 1);
                        Console.Write("\b \b");
                    }
                }
                else
                {
                    password += keyPressed.KeyChar;
                    Console.Write("*");
                }
            }
            return password;
        }

        public static SecureString ConvertToSecureString(string password)
        {
            var secureString = new SecureString();
            if (password.Length > 0)
            {
                foreach (var character in password.ToCharArray()) secureString.AppendChar(character);
            }
            return secureString;
        }
    }
}
