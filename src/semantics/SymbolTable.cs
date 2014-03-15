using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace crosspascal.semantics
{
	public class SymbolTable<T>
	{
		private class SymbolContext<T>
		{


		}

		Stack<SymbolContext<T>> contexts = new Stack<SymbolContext<T>>();

		public SymbolTable()
		{

		}

	}
}
