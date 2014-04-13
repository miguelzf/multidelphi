using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LLVM
{
    public unsafe class ExecutionEngine : IDisposable, IPointerWrapper
    {
        private LLVMExecutionEngineRef* m_handle;

        public ExecutionEngine(Module module)
        {
            StringBuilder sb = new StringBuilder(1024);
		//	if (Native.CreateExecutionEngineForModule(ref m_handle, module.Handle, ref sb) == 0)
		//	if (Native.CreateInterpreterForModule(ref m_handle, module.Handle, ref sb) == 0)
			if (Native.CreateJITCompilerForModule(ref m_handle, module.Handle, 0, ref sb) == 0)
				Console.Error.WriteLine(sb.ToString());
        }

        ~ExecutionEngine()
        {
            //Dispose(false);
        }

        public LLVMExecutionEngineRef* Handle
        {
            get { return m_handle; }
        }

        public TargetData GetTargetData()
        {
            return new TargetData(Native.GetExecutionEngineTargetData(m_handle), false);
        }

        public GenericValue RunFunction(Function function, GenericValue[] args)
        {
            IntPtr[] argVals = new IntPtr[args.Length];
            for(int i = 0; i < argVals.Length; ++i)
                argVals[i] = (IntPtr)args[i].Handle;

            return new GenericValue(Native.RunFunction(m_handle, function.Handle, (uint)args.Length, argVals));
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
                Native.DisposeExecutionEngine(m_handle);
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
