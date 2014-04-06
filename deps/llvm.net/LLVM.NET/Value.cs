using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LLVM
{
    public unsafe struct Value : IPointerWrapper
    {
        public static readonly Value Null = new Value();

        private readonly LLVMValueRef* m_handle;

        public Value(LLVMValueRef* handle)
        {
            if(handle == null)
                throw new ArgumentNullException("handle");

            m_handle = handle;
        }

        public LLVMValueRef* Handle
        {
            get { return m_handle; }
        }

        public bool IsNull
        {
            get { return m_handle == null; }
        }

        public bool IsUsed
        {
            get { return Used(m_handle); }
        }

        public string Name
        {
            get
            {
                return GetName(m_handle);
            }
            set
            {
                Native.SetValueName(m_handle, value);
            }
        }

        public static bool Used(LLVMValueRef* value)
        {
            if(value == null)
                throw new ArgumentNullException("value");

            return Native.GetFirstUse(value) != null;
        }

        public static int GetUseCount(LLVMValueRef* value)
        {
            if(value == null)
                throw new ArgumentNullException("value");

            LLVMUseRef* use = Native.GetFirstUse(value);
            int count = 0;

            while(use != null)
            {
                count++;
                use = Native.GetNextUse(use);
            }

            return count;
        }

        public static string GetName(LLVMValueRef* value)
        {
            if(value == null)
                throw new ArgumentNullException("value");

            IntPtr name = Native.GetValueName(value);
            if(name == null)
                return null;

            return System.Runtime.InteropServices.Marshal.PtrToStringAnsi(name);
        }

        public static Value CreateConstBool(bool value)
        {
            return new Value(Native.ConstInt(TypeRef.CreateBool().Handle, (ulong)(value ? 1 : 0), 0));
        }

        public static Value CreateConstInt(TypeRef type, long value)
        {
            return new Value(Native.ConstInt(type.Handle, (ulong)value, 1));
        }

        public static Value CreateConstInt8(sbyte value)
        {
            return new Value(Native.ConstInt(TypeRef.CreateInt8().Handle, (ulong)value, 1));
        }

        public static Value CreateConstInt16(short value)
        {
            return new Value(Native.ConstInt(TypeRef.CreateInt16().Handle, (ulong)value, 1));
        }

        public static Value CreateConstInt32(int value)
        {
            return new Value(Native.ConstInt(TypeRef.CreateInt32().Handle, (ulong)value, 1));
        }

        public static Value CreateConstInt64(long value)
        {
            return new Value(Native.ConstInt(TypeRef.CreateInt64().Handle, (ulong)value, 1));
        }

        public static Value CreateConstUInt(TypeRef type, ulong value)
        {
            return new Value(Native.ConstInt(type.Handle, value, 0));
        }

        public static Value CreateConstUInt8(byte value)
        {
            return new Value(Native.ConstInt(TypeRef.CreateInt8().Handle, value, 0));
        }

        public static Value CreateConstUInt16(ushort value)
        {
            return new Value(Native.ConstInt(TypeRef.CreateInt16().Handle, value, 0));
        }

        public static Value CreateConstUInt32(uint value)
        {
            return new Value(Native.ConstInt(TypeRef.CreateInt32().Handle, value, 0));
        }

        public static Value CreateConstUInt64(ulong value)
        {
            return new Value(Native.ConstInt(TypeRef.CreateInt64().Handle, value, 0));
        }

        public static Value CreateConstReal(TypeRef type, double value)
        {
            return new Value(Native.ConstReal(type.Handle, value));
        }

        public static Value CreateConstDouble(double value)
        {
            return new Value(Native.ConstReal(TypeRef.CreateDouble().Handle, value));
        }

        public static Value CreateConstFloat(float value)
        {
            return new Value(Native.ConstReal(TypeRef.CreateFloat().Handle, value));
        }

        #region IPointerWrapper Members

        IntPtr IPointerWrapper.NativePointer
        {
            get { return (IntPtr)m_handle; }
        }

        #endregion
    }
}
