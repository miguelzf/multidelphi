using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LLVM
{
    public static class Guard
    {
        public static void ArgumentNull(object obj, string name)
        {
            if(obj == null)
                throw new ArgumentNullException(name);
        }

        public static void EmptyString(string s, string msg)
        {
            if(string.IsNullOrWhiteSpace(s))
                throw new ArgumentException(msg);
        }

        public static void IsTrue(bool condition, string msg)
        {
            if(!condition)
                throw new InvalidOperationException(msg);
        }
    }
}
