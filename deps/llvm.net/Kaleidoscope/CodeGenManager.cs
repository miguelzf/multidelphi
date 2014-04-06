using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LLVM;

namespace Kaleidoscope
{
    static unsafe class CodeGenManager
    {
        public static Module Module { get; set; }
        public static IDictionary<string, Value> NamedValues { get; private set; }
        public static TextWriter ErrorOutput { get; set; }
        /// BinopPrecedence - This holds the precedence for each binary operator that is
        /// defined.
        public static IDictionary<char, int> BinopPrecendence = new Dictionary<char, int>();

        static CodeGenManager()
        {
            NamedValues = new Dictionary<string, Value>();
            ErrorOutput = Console.Out;

            // Install standard binary operators.
            // 1 is lowest precedence.
            BinopPrecendence['<'] = 10;
            BinopPrecendence['+'] = 20;
            BinopPrecendence['-'] = 20;
            BinopPrecendence['*'] = 40;  // highest.
        }
    }
}
