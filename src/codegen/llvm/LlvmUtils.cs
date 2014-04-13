using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LLVM;

namespace crosspascal.codegen.llvm
{

	/// <summary>
	/// Extension methods to LLVM.NET
	/// Temporarily here
	/// </summary>
	public unsafe class Utils
	{

		public static void SetInitializer(Value vdecl, Value init)
		{
			Native.SetInitializer(vdecl.Handle, init.Handle);
		}

		public static String Tostring(TypeRef type)
		{
			string ret = type.TypeKind.ToString();
			if (type.TypeKind == LLVMTypeKind.PointerTypeKind)
				ret += " to " + Native.GetTypeKind(type.pointedType).ToString();
			return ret;
		}

		public static String GetType(Value val)
		{
			LLVMTypeRef* type = Native.TypeOf(val.Handle);
			return Native.GetTypeKind(type).ToString();
		}

	}
}
