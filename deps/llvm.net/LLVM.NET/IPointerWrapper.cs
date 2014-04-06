using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LLVM
{
    public interface IPointerWrapper
    {
        IntPtr NativePointer { get; }
    }
}
