using System;

namespace Project1Afdemp
{
    class ReadAndParse
    {
        public static T InputType<T>()
        {
            var method = typeof(T).GetMethod("Parse", new[] { typeof(string) });

            while (true)
            {
                try
                {
                    var value = (T)method.Invoke(null, new[] { Console.ReadLine() });
                    return value;
                }
                catch (Exception)
                {
                    Console.WriteLine("You need to give the correct type!\nPlease try again..");
                }
            }
        }
    }
}
