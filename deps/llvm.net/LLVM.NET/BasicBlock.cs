using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LLVM
{
    public unsafe class BasicBlock : IPointerWrapper
    {
        private LLVMBasicBlockRef* m_handle;
        private readonly string m_name;

        public BasicBlock(string name, LLVMBasicBlockRef* handle)
        {
            if(handle == null)
                throw new ArgumentNullException("handle");

            if(name == null)
                name = Value.GetName((LLVMValueRef*)handle);

            m_handle = handle;
            m_name = name;
        }

        public BasicBlock(string name, Function function)
        {
            Guard.ArgumentNull(name, "name");
            Guard.ArgumentNull(function, "function");

            m_handle = Native.AppendBasicBlock(function.Handle, name);
            m_name = name;
        }

        public LLVMBasicBlockRef* Handle
        {
            get { return m_handle; }
        }

        public string Name
        {
            get { return m_name; }
        }

        public bool IsUsed
        {
            get { return Value.Used((LLVMValueRef*)m_handle); }
        }

        public Value GetTerminator()
        {
            LLVMValueRef* term = Native.GetBasicBlockTerminator(m_handle);
            if(term == null)
                return Value.Null;

            return new Value(term);
        }

        public void MoveBefore(BasicBlock block)
        {
            Guard.ArgumentNull(block, "block");
            Native.MoveBasicBlockBefore(m_handle, block.m_handle);
        }

        public void MoveAfter(BasicBlock block)
        {
            Guard.ArgumentNull(block, "block");
            Native.MoveBasicBlockAfter(m_handle, block.m_handle);
        }

        public void Delete()
        {
            if(m_handle == null)
                throw new InvalidOperationException("BasicBlock is null");

            Native.DeleteBasicBlock(m_handle);
            m_handle = null;
        }

        public void RemoveFromParent()
        {
            if(m_handle == null)
                throw new InvalidOperationException("BasicBlock is null");

            Native.RemoveBasicBlockFromParent(m_handle);
        }

        public Function GetParent()
        {
            if(m_handle == null)
                throw new InvalidOperationException("BasicBlock is null");

            LLVMValueRef* func = Native.GetBasicBlockParent(m_handle);
            if(func == null)
                return null;

            return new Function(func);
        }

        #region IPointerWrapper Members

        IntPtr IPointerWrapper.NativePointer
        {
            get { return (IntPtr)m_handle; }
        }

        #endregion
    }
}
