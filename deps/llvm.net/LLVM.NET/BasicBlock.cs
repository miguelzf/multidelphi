using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LLVM
{
    public unsafe class BasicBlock : Value
    {
        private LLVMBasicBlockRef* bb_handle;
        private readonly string m_name;

        public BasicBlock(string name, LLVMBasicBlockRef* handle)
        {
            if(handle == null)
                throw new ArgumentNullException("handle");

            if(name == null)
                name = Value.GetName((LLVMValueRef*)handle);

            bb_handle = handle;
            m_name = name;
        }

        public BasicBlock(string name, Function function)
        {
            Guard.ArgumentNull(name, "name");
            Guard.ArgumentNull(function, "function");

            bb_handle = Native.AppendBasicBlock(function.Handle, name);
            m_name = name;
        }

        public LLVMBasicBlockRef* BBHandle
        {
            get { return bb_handle; }
        }

		public override bool IsNull
		{
			get { return bb_handle == null; }
		}

		public override string Name
        {
            get { return m_name; }
        }

        public override bool IsUsed
        {
            get { return Value.Used((LLVMValueRef*)bb_handle); }
        }

        public Value GetTerminator()
        {
            LLVMValueRef* term = Native.GetBasicBlockTerminator(bb_handle);
            if(term == null)
                return Value.Null;

            return new Value(term);
        }

        public void MoveBefore(BasicBlock block)
        {
            Guard.ArgumentNull(block, "block");
            Native.MoveBasicBlockBefore(bb_handle, block.bb_handle);
        }

        public void MoveAfter(BasicBlock block)
        {
            Guard.ArgumentNull(block, "block");
            Native.MoveBasicBlockAfter(bb_handle, block.bb_handle);
        }

        public void Delete()
        {
            if(bb_handle == null)
                throw new InvalidOperationException("BasicBlock is null");

            Native.DeleteBasicBlock(bb_handle);
            bb_handle = null;
        }

        public void RemoveFromParent()
        {
            if(bb_handle == null)
                throw new InvalidOperationException("BasicBlock is null");

            Native.RemoveBasicBlockFromParent(bb_handle);
        }

        public Function GetParent()
        {
            if(bb_handle == null)
                throw new InvalidOperationException("BasicBlock is null");

            LLVMValueRef* func = Native.GetBasicBlockParent(bb_handle);
            if(func == null)
                return null;

            return new Function(func);
        }

    }
}
