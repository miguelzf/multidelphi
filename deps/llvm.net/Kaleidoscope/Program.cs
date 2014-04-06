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

			var drivers = new List<IDriver>();
			drivers.Add(new Kaleidoscope.Chapter3.Driver());
			drivers.Add(new Kaleidoscope.Chapter4.Driver());
			drivers.Add(new Kaleidoscope.Chapter5.Driver());
			drivers.Add(new Kaleidoscope.Chapter6.Driver());
			drivers.Add(new Kaleidoscope.Chapter7.Driver());

			foreach (var driver in drivers)
				driver.Run();

            Console.ReadLine();
        }
    }
}
