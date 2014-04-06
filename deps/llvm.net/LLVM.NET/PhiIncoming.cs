using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LLVM
{
    public class PhiIncoming
    {
        private List<Value> m_incomingVals = new List<Value>();
        private List<BasicBlock> m_incomingBlocks = new List<BasicBlock>();

        public PhiIncoming()
        {

        }

        public PhiIncoming(Value value, BasicBlock block)
        {
            Add(value, block);
        }

        public void Add(Value value, BasicBlock block)
        {
            Guard.ArgumentNull(value, "value");
            Guard.ArgumentNull(block, "block");

            m_incomingVals.Add(value);
            m_incomingBlocks.Add(block);
        }

        public IntPtr[] GetValuePointers()
        {
            return LLVMHelper.MarshallPointerArray(m_incomingVals);
        }

        public IntPtr[] GetBlockPointers()
        {
            return LLVMHelper.MarshallPointerArray(m_incomingBlocks);
        }
    }
}
