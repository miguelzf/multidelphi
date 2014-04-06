using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LLVM
{
    public unsafe class GenericValue : IDisposable, IPointerWrapper
    {
        private LLVMGenericValueRef* m_handle;

        public GenericValue(LLVMGenericValueRef* value)
        {
            m_handle = value;
        }

        ~GenericValue()
        {
            Dispose(false);
        }

        public LLVMGenericValueRef* Handle
        {
            get { return m_handle; }
        }

        public long ToInt()
        {
            return (long)Native.GenericValueToInt(m_handle, 1);
        }

        public ulong ToUInt()
        {
            return Native.GenericValueToInt(m_handle, 0);
        }

        public double ToReal()
        {
            return Native.GenericValueToFloat(Native.DoubleType(), m_handle);
        }

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Dispose(bool disposing)
        {
            if(m_handle != null)
            {
                Native.DisposeGenericValue(m_handle);
                m_handle = null;
            }
        }

        #endregion

        #region IPointerWrapper Members

        IntPtr IPointerWrapper.NativePointer
        {
            get { return (IntPtr)m_handle; }
        }

        #endregion
    }
}
