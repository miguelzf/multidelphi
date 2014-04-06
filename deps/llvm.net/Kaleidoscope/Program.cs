using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kaleidoscope
{
    unsafe class Program
    {
        static void Main(string[] args)
        {
            LLVM.Native.LinkInJIT();
            //LLVM.Native.InitializeNativeTarget(); // Declared in bindings but not exported from the shared library.
            LLVM.Native.InitializeX86TargetInfo();
            LLVM.Native.InitializeX86Target();
            LLVM.Native.InitializeX86TargetMC();

            IDriver driver = new Kaleidoscope.Chapter7.Driver();
            driver.Run();

            Console.ReadLine();
        }
    }
}
