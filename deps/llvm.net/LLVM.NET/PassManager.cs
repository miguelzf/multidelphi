using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LLVM
{
    public unsafe class PassManager : IDisposable, IPointerWrapper
    {
        private LLVMPassManagerRef* m_handle;
        private readonly LLVMModuleRef* m_module;

        public PassManager(Module module)
        {
            Guard.ArgumentNull(module, "module");
            m_handle = Native.CreateFunctionPassManagerForModule(module.Handle);
            m_module = module.Handle;
        }

        ~PassManager()
        {
            //Dispose(false);
        }


        public LLVMPassManagerRef* Handle
        {
            get { return m_handle; }
        }

        public void Initialize()
        {
            Native.InitializeFunctionPassManager(m_handle);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="function"></param>
        /// <returns>True if the function was modified</returns>
        public bool Run(Function function)
        {
            return Native.RunFunctionPassManager(m_handle, function.Handle) != 0;
        }

        public void AddTargetData(TargetData targetData)
        {
            Native.AddTargetData(targetData.Handle, m_handle);
        }

        public void AddArgumentPromotionPass()
        {
            Native.AddArgumentPromotionPass(m_handle);
        }

        public void AddConstantMergePass()
        {
            Native.AddConstantMergePass(m_handle);
        }

        public void AddDeadArgEliminationPass()
        {
            Native.AddDeadArgEliminationPass(m_handle);
        }

        public void AddFunctionAttrsPass()
        {
            Native.AddFunctionAttrsPass(m_handle);
        }

        public void AddFunctionInliningPass()
        {
            Native.AddFunctionInliningPass(m_handle);
        }

        public void AddAlwaysInlinerPass()
        {
            Native.AddAlwaysInlinerPass(m_handle);
        }

        public void AddGlobalDCEPass()
        {
            Native.AddGlobalDCEPass(m_handle);
        }

        public void AddGlobalOptimizerPass()
        {
            Native.AddGlobalOptimizerPass(m_handle);
        }

        public void AddIPConstantPropagationPass()
        {
            Native.AddIPConstantPropagationPass(m_handle);
        }

        public void AddPruneEHPass()
        {
            Native.AddPruneEHPass(m_handle);
        }

        public void AddIPSCCPPass()
        {
            Native.AddIPSCCPPass(m_handle);
        }

        public void AddInternalizePass(uint AllButMain)
        {
            Native.AddInternalizePass(m_handle, AllButMain);
        }

        public void AddStripDeadPrototypesPass()
        {
            Native.AddStripDeadPrototypesPass(m_handle);
        }

        public void AddStripSymbolsPass()
        {
            Native.AddStripSymbolsPass(m_handle);
        }

        public void AddAggressiveDCEPass()
        {
            Native.AddAggressiveDCEPass(m_handle);
        }

        public void AddCFGSimplificationPass()
        {
            Native.AddCFGSimplificationPass(m_handle);
        }

        public void AddDeadStoreEliminationPass()
        {
            Native.AddDeadStoreEliminationPass(m_handle);
        }

        public void AddGVNPass()
        {
            Native.AddGVNPass(m_handle);
        }

        public void AddIndVarSimplifyPass()
        {
            Native.AddIndVarSimplifyPass(m_handle);
        }

        public void AddInstructionCombiningPass()
        {
            Native.AddInstructionCombiningPass(m_handle);
        }

        public void AddJumpThreadingPass()
        {
            Native.AddJumpThreadingPass(m_handle);
        }

        public void AddLICMPass()
        {
            Native.AddLICMPass(m_handle);
        }

        public void AddLoopDeletionPass()
        {
            Native.AddLoopDeletionPass(m_handle);
        }

        public void AddLoopIdiomPass()
        {
            Native.AddLoopIdiomPass(m_handle);
        }

        public void AddLoopRotatePass()
        {
            Native.AddLoopRotatePass(m_handle);
        }

        public void AddLoopUnrollPass()
        {
            Native.AddLoopUnrollPass(m_handle);
        }

        public void AddLoopUnswitchPass()
        {
            Native.AddLoopUnswitchPass(m_handle);
        }

        public void AddMemCpyOptPass()
        {
            Native.AddMemCpyOptPass(m_handle);
        }

        public void AddPromoteMemoryToRegisterPass()
        {
            Native.AddPromoteMemoryToRegisterPass(m_handle);
        }

        public void AddReassociatePass()
        {
            Native.AddReassociatePass(m_handle);
        }

        public void AddSCCPPass()
        {
            Native.AddSCCPPass(m_handle);
        }

        public void AddScalarReplAggregatesPass()
        {
            Native.AddScalarReplAggregatesPass(m_handle);
        }

        public void AddScalarReplAggregatesPassSSA()
        {
            Native.AddScalarReplAggregatesPassSSA(m_handle);
        }

        public void AddScalarReplAggregatesPassWithThreshold(int Threshold)
        {
            Native.AddScalarReplAggregatesPassWithThreshold(m_handle, Threshold);
        }

        public void AddSimplifyLibCallsPass()
        {
            Native.AddSimplifyLibCallsPass(m_handle);
        }

        public void AddTailCallEliminationPass()
        {
            Native.AddTailCallEliminationPass(m_handle);
        }

        public void AddConstantPropagationPass()
        {
            Native.AddConstantPropagationPass(m_handle);
        }

        public void AddDemoteMemoryToRegisterPass()
        {
            Native.AddDemoteMemoryToRegisterPass(m_handle);
        }

        public void AddVerifierPass()
        {
            Native.AddVerifierPass(m_handle);
        }

        public void AddCorrelatedValuePropagationPass()
        {
            Native.AddCorrelatedValuePropagationPass(m_handle);
        }

        public void AddEarlyCSEPass()
        {
            Native.AddEarlyCSEPass(m_handle);
        }

        public void AddLowerExpectIntrinsicPass()
        {
            Native.AddLowerExpectIntrinsicPass(m_handle);
        }

        public void AddTypeBasedAliasAnalysisPass()
        {
            Native.AddTypeBasedAliasAnalysisPass(m_handle);
        }

        public void AddBasicAliasAnalysisPass()
        {
            Native.AddBasicAliasAnalysisPass(m_handle);
        }

        public void AddBBVectorizePass()
        {
            Native.AddBBVectorizePass(m_handle);
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
                Native.DisposePassManager(m_handle);
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
