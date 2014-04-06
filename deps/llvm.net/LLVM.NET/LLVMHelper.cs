using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LLVM
{
    public static class LLVMHelper
    {
        public static IntPtr[] MarshallPointerArray<T>(IEnumerable<T> values) where T : IPointerWrapper
        {
            if(values == null)
                return new IntPtr[0];

            return values.Select(pw => pw.NativePointer).ToArray();
        }
    }
}
