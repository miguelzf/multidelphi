using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32.SafeHandles;

namespace LLVM
{
    public unsafe class Module : IDisposable, IPointerWrapper
    {
        private LLVMModuleRef* m_handle;

        public Module(string name)
        {
            Guard.ArgumentNull(name, "name");

            m_handle = Native.ModuleCreateWithName(name);
        }

        ~Module()
        {
            Dispose(false);
        }

        public LLVMModuleRef* Handle
        {
            get { return m_handle; }
        }

        public Function GetFunction(string name)
        {
            var func = Native.GetNamedFunction(m_handle, name);
            if(func == null)
                return null;

            return new Function(name, func);
        }

        public void Dump()
        {
            Native.DumpModule(m_handle);
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
                Native.DisposeModule(m_handle);
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
