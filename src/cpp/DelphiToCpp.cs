using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace crosspascal.cpp
{
	class DelphiToCpp
	{
		public static string Operator(string op)
		{
			op = op.ToLower();
			if (op.Equals(":="))
				return "=";
			if (op.Equals("@"))
				return "&";
			if (op.Equals("shl"))
				return "<<";
			if (op.Equals("shr"))
				return ">>";
			if (op.Equals("^"))
				return "*";
			if (op.Equals("="))
				return "==";
			if (op.Equals("or"))
				return "||";
			if (op.Equals("and"))
				return "&&";
			if (op.Equals("not"))
				return "!";
			if (op.Equals("div"))
				return "/";
			if (op.Equals("mod"))
				return "%";
			return op;
		}

		public static string Variable(string var)
		{
			var = var.ToLower();
			if (var.Equals("self"))
				return "this";
			return var;
		}

		public static string Type(string s)
		{
			s = s.ToLower();
			if (s.Equals(""))
				return "void";
			if (s.Equals("string"))
				return "std::string";
			if (s.Equals("integer"))
				return "int";
			if (s.Equals("cardinal"))
				return "unsigned int";
			if (s.Equals("boolean"))
				return "bool";
			if (s.Equals("byte"))
				return "unsigned char";
			if (s.Equals("shorting"))
				return "char";
			return s;
		}

		public static string Constant(string s)
		{
			s = s.ToLower();
			if (s.Equals("nil"))
				return "NULL";
			return s;
		}

	}
}
