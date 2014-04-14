using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LLVM;

namespace MultiPascal.Codegen.LlvmIL
{

	/// <summary>
	/// Class to extend the LLVM.net IRBuilder wrapper with some new instruction wrappers.
	/// Avoid changing the original LLVM.NET
	/// </summary>
	
	public unsafe class LlvmIRBuilder : LLVM.IRBuilder
	{
		public Value BuildIntToPtr(Value arg, TypeRef ptrType, string name = tmpvarname)
		{
			return new Value(Native.BuildIntToPtr(m_builder, arg.Handle, ptrType.Handle, name));
		}

		public Value AddGlobal(Module module, TypeRef type, String name)
		{
			return new Value(Native.AddGlobal(module.Handle, type.Handle, name));
		}

		public void ResetInsertPoint(Function func, BasicBlock bb)
		{
			func.AppendBasicBlock(bb);
			this.SetInsertPoint(bb);
		}
	}
}
