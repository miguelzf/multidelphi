using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LLVM
{
    public static class Targets
    {
        public const string Win32TargetTriple = "i686-pc-win32";
    }

    public unsafe class TargetData : IDisposable, IPointerWrapper
    {
        private LLVMTargetDataRef* m_handle;
        private bool m_bNeedsDisposing;

        public TargetData(LLVMTargetDataRef* handle, bool needsDisposing)
        {
            m_handle = handle;
            m_bNeedsDisposing = needsDisposing;
        }

        ~TargetData()
        {
            //Dispose(false);
        }

        public LLVMTargetDataRef* Handle
        {
            get { return m_handle; }
        }

        public static TargetData Create(string triple)
        {
            LLVMTargetDataRef* handle = Native.CreateTargetData(triple);
            if(handle == null)
                return null;

            return new TargetData(handle, true);
        }

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Dispose(bool disposing)
        {
            if(m_bNeedsDisposing && m_handle != null)
            {
                Native.DisposeTargetData(m_handle);
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
