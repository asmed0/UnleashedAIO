using Jint;
using System;

namespace UnleashedAIO.Unleased.Utilities
{
    class AdyenEncrypt
    {
        public static string EncryptData()
        {

             var add = new Engine()
            .Execute("function add(a, b) { return a + b; }")
            .GetValue("add")
            ;

            add.Invoke(1, 2); // -> 3
            Console.WriteLine(add);
            return add.ToString();
        }
    }
}
