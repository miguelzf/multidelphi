using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace crosspascal.AST
{
	public enum FunctionClass
	{
		Procedure,
		Function,
		Constructor,
		Destructor
	}

	public enum Visibilty
	{
		Published,
		Public,
		Private,
		Protected 
	}

	class VariableType
	{
		public string primitive;
		public int baseOffset;
		public int elementCount;
		public bool isPointer;
		public bool isDynamicArray;
		public FunctionSignature signature;
		public ModuleFunction function;
	}

  class Variable 
  {
    public string name;
    public Variable owner;
    public VariableType type;
    public Expression initialValue;
    public ModuleFunction parent;
    public List<Variable> childs;
	public Visibilty visibility;

	public string GetPath()
	{
		string result = this.name.ToLower();
		if (owner!=null) 
			result = owner.GetPath() + '.' + result;
		return result;
	}

	public Variable GetVariable(string s, VariableAcessor ac)
	{
		string A,B;

		s = s.ToLower();
		A = this.GetPath();
		if (ac==null)
			B = A;
		else
			B = ac.GetPathBack() + ac.GetPathFront();

		if (this.name.ToLower().Equals(s) && A.Equals(B))
			return this;

		foreach (Variable v in childs)
		{
			Variable result = v.GetVariable(s, ac);
			if (result!=null) 
				return result;
		}
	
		return null;
	}

  }    

  class Argument : Variable
  {
    public bool isVar;
    public bool isConst;
  }

  class FunctionSignature 
  {
	public string name;
    public ModuleFunction owner;
    public FunctionClass functionClass;
    public VariableType type;
    public string callConvention;
    public string externalLocation;
    public string externalName;
    public bool isStatic;
    public bool isVirtual;
    public bool isAbstract;
    public bool isOverride;
    public bool isOverloaded;
    public List<Argument> arguments;
	public Visibilty visibility;
  }

  class Constant 
  {
	public string name;
    public Expression value;
    public string init;
    public VariableType type;
    public ModuleFunction parent;
  }

  class TypeNode 
  {
    public string name;
	public string type;
    public ModuleFunction parent;
    public bool isPointer;
    public int baseOffset;
    public int elementCount;
  }

  class RecordType : TypeNode
  {
	public bool isPacked;
    public List<Variable> variables;
  }

  class EnumEntry 
  {
    public string name;
    public int value;
  }

  class EnumType : TypeNode
  {
    public List<EnumEntry> values;
  }

  class FunctionType : TypeNode
  {
    public FunctionSignature signature;
  }

  class ClassProperty 
  {
    public string name;
    public string read;
    public string write;
    public string owner;
    public VariableType type;
  }
  
  class ClassSignature : TypeNode
  {
    public ClassSignature ancestor;

    public List<Variable> variables;

    public List<FunctionSignature> privateMethods;
    public List<FunctionSignature> protectedMethods;
    public List<FunctionSignature> publicMethods;

    public ClassProperty[] properties;

	public Variable GetVariable(string s, VariableAcessor ac)
	{
		Variable result;
		string A,B;
		s = s.ToLower();
	
		foreach (Variable v in variables)
		{
			A = v.GetPath();
			if (ac==null) 
				B = A;
			else
				B = ac.GetPathBack() + ac.GetPathFront();

			if (v.name.ToLower().Equals(s) && A.Equals(B)) 
				return v;
			else
			{
				result = v.GetVariable(s, ac);
				if (result!=null)	
					return result;
			}        
		}
	
		if (this.ancestor!=null)
		{
			result = this.ancestor.GetVariable(s, ac);
			if (result!=null)	
				return result;
		}

		return null;
	}

	public ClassProperty GetProperty(string s)
	{
	  s = s.ToLower();
	  for (int i=0; i<properties.Length; i++)
	  if (properties[i].name.ToLower().Equals(s))
		return properties[i];

	  if (this.ancestor!=null)
	  {
		ClassProperty result = this.ancestor.GetProperty(s);
		if (result!=null) 
		  return result;
	  }

	  return null;
	}

  }

  class Statement 
  {
    public ModuleFunction parent;
	public ModuleSection section;

	public virtual Statement Reduce()
	{
		return this;
	}

	public static void ReduceAndReplace(ref Statement st)
	{
		if (st == null)
			return;
 
		st = st.Reduce();
	}
  }

	class StatementBlock : Statement
	{
	    public List<Statement> statements;
	
		public override Statement Reduce()
		{
			List<Statement> tempList = new List<Statement>();
			foreach (Statement st in statements)
			{
				Statement temp = st;
				ReduceAndReplace(ref temp);
				tempList.Add(temp);
			}
			statements = tempList;
			return this;
		}
	}

	class Expression		// : Statement
	{
		public ModuleFunction parent;
		public ModuleSection section;

		public virtual Expression Reduce()
		{
			return this;
		}

		public static void ReduceAndReplace(ref Expression exp)
		{
			if (exp == null)
				return;

			exp = exp.Reduce();
		}
  }

  class ConstantExpression : Expression
  {
    public string value;

	public override Expression Reduce()
	{ 
		Constant c = null;
		Module currentModule = parent.section.parent;

		if (currentModule.interfaceSection!=null)
			c = currentModule.interfaceSection.GetConstant(value);

		if (c==null && currentModule.implementationSection!=null)
			c = currentModule.implementationSection.GetConstant(value);

		if (c==null && this.parent!=null)
			c = this.parent.GetConstant(value);

		if (c!=null)
		{
			Console.WriteLine("Reducing ",c.name);
    
			c.value = c.value.Reduce();
			if (c.value is ConstantExpression)
				this.value = (c.value as ConstantExpression).value;
		}

		return this;
	}

  }

	class CastExpression : Expression
	{
		public string type;
		public Expression body;
		public Variable V;
		public VariableAcessor acessor;

		public override Expression Reduce()
		{
			return this;
		}
	}

  class VariableAcessor
  {
    public string element;
    public Expression index;
    public VariableAcessor next;
    public VariableAcessor parent;
    
	public bool isDereferenced;
    public bool IsFunctionCall;
    public List<Expression> arguments;

	public string GetPathFront(bool first = true)
	{
		string result;

		if (!first)
			result = this.element.ToLower();
		else
			result = "";

		if (this.next!=null)
			result = result + '.'+ this.next.GetPathFront(false);

		return result;
	}

	public string GetPathBack()
	{
		string result = this.element.ToLower();
		if (this.parent!=null) 
			result = parent.GetPathBack() + '.' + result;
		return result;
	}

	public void PostProcess()
	{
		Expression.ReduceAndReplace(ref index);

		if (next != null)
			next.PostProcess();
	}


  }

	class FunctionCall : Statement
	{
		public VariableAcessor acessor;

		public override Statement Reduce()
		{
			if (this.acessor!=null)
				this.acessor.PostProcess();

			List<Expression> tempList = new List<Expression>();
			foreach (Expression exp in acessor.arguments)
			{
				Expression temp = exp;
				Expression.ReduceAndReplace(ref temp);
				tempList.Add(exp);
			}
			acessor.arguments = tempList;

			if (acessor.element.ToLower().Equals("inc") || acessor.element.ToLower().Equals("dec"))
			{
				Statement result = null;
				if (acessor.arguments[0] is VariableExpression) 
				{
					Expression ammount;
					if (this.acessor.arguments.Count>1) 
						ammount = this.acessor.arguments[1];
					else
					{
						ConstantExpression c = new ConstantExpression();
						c.value = "1";
						ammount = c;						
					}
					
					IncrementStatement inc =new IncrementStatement(
						(acessor.arguments[0] as VariableExpression).v, 
						(acessor.arguments[0] as VariableExpression).acessor, 
						ammount, (acessor.element.ToLower().Equals("dec")));
					result = inc;
				}
				else
					Console.WriteLine("ERROR: Variable expected!");
				return result;
			}

			return this;
		}
	}


	class VariableExpression : Expression
	{
		public Variable v;
		public VariableAcessor acessor;
	
		public override Expression Reduce()
		{
			if (this.acessor!=null) 
				this.acessor.PostProcess();
	
			return this;
		}

	}

  class CallExpression : Expression
  {
    String name;
    string obj;
    VariableAcessor acessor;

	public override Expression Reduce()
	{
	  if (acessor!=null) 
		acessor.PostProcess();

	  if (name.ToLower().Equals("succ")) 
	  {
		BinaryExpression result = new BinaryExpression();
		result.op = "+";
		result.a = this.acessor.arguments[0];
		result.b = new ConstantExpression();
		(result.b as ConstantExpression).value = "1";
		return result.Reduce();		
	  }

	  if (name.ToLower().Equals("pred")) 
	  {
		BinaryExpression result = new BinaryExpression();
		result.op = "-";
		result.a = this.acessor.arguments[0];
		result.b = new ConstantExpression();
		(result.b as ConstantExpression).value = "1";
		return result.Reduce();		
	  }

	  if (this.section.IsType(name))
	  {
		CastExpression result = new CastExpression();
		result.type = this.name;
		result.body = acessor.arguments[0];
		return result;
	  }

		return this;
	}
  }

	class OperatorExpression : Expression
	{
		public string op;

		public bool IsCompareOperator()
		{
			return (op.Equals(">") || op.Equals("<") || op.Equals(">=") || op.Equals("<="));
		}

		public void Invert()
		{
		  if (op.Equals(">"))
			op = "<=";
		  else
		  if (op.Equals("<")) 
			op = ">=";
		  else
		  if (op.Equals(">="))
			op = "<";
		  else
		  if (op.Equals("<="))
			op = ">";
		}

  }

	class UnaryExpression : OperatorExpression
	{
	    public Expression a;

		public override Expression Reduce()
		{
			ReduceAndReplace(ref a);

			if (op.ToLower().Equals("not"))
			{
				if (a is BinaryExpression && (a as BinaryExpression).IsCompareOperator())
				{
					(a as BinaryExpression).Invert();
					return a;
				}
			}

			return this;
		}
	}

	class BinaryExpression : OperatorExpression
	{
		public Expression a, b;

		public override Expression Reduce()
		{
		  string s;
		  int x,y;
		  string aa,bb;
		
		  ReduceAndReplace(ref a);
		  ReduceAndReplace(ref b);

		  if (a is ConstantExpression && b is ConstantExpression && op!=null)
		  {			  
			aa = this.section.parent.EvaluateConstant((a as ConstantExpression).value);
			bb = this.section.parent.EvaluateConstant((b as ConstantExpression).value);
			if (aa!=null && bb!=null)
			{
				x = Convert.ToInt32(aa);
				y = Convert.ToInt32(bb);
				if (op.Equals("+"))
					s = (x+y).ToString();
				else
				if (op.Equals("-"))
					s = (x-y).ToString();
				else
				if (op.Equals("*"))
					s = (x*y).ToString();
				else
				if (op.Equals("/"))
					s = (x/y).ToString();
				else
				if (op.ToLower().Equals("div"))
					s = ((int)(x / y)).ToString();
				else
				if (op.ToLower().Equals("mod"))
					s = (x % y).ToString();
				else
					s = null;

				if (s!=null)
				{
					ConstantExpression result = new ConstantExpression();
					result.value = s;
					return result;
				}
			}
		  }

		  return this;
		}
	}

	class Assignment : Statement
	{
	    public Variable v;
	    public VariableAcessor acessor;
	    public Expression body;

		public override Statement Reduce()
		{
			if (this.acessor != null)
				acessor.PostProcess();

			Expression.ReduceAndReplace(ref body);
			return this;
		}

	}

	class IfStatement : Statement
	{
		public Expression condition;
		public Statement whenTrue;
		public Statement whenFalse;

		public override Statement Reduce()
		{
			Expression.ReduceAndReplace(ref condition);
			ReduceAndReplace(ref whenTrue);
			ReduceAndReplace(ref whenFalse);
			return this;
		}

	}

	class CaseEntry 
	{
		public List<Expression> constants;
		public Statement body;

		public CaseEntry(Expression constant, Statement body)
		{
			constants = new List<Expression>();
			constants.Add(constant);
			this.body = body;
		}

	}

	class CaseStatement : Statement
	{
		public Expression condition;
		public List<CaseEntry> entries;
		public Statement whenFalse;

		public CaseStatement(Expression condition)
		{
			this.condition = condition;
		}

		public void AddCase(Expression value, Statement body)
		{ 			
			foreach (CaseEntry c in entries)
			if (c.body == body)
			{
				c.constants.Add(value);
				return;
			}

			entries.Add(new CaseEntry(value, body));
		}

		public override Statement Reduce()
		{
			foreach (CaseEntry c in entries) 
			{
				ReduceAndReplace(ref c.body);
			}

			ReduceAndReplace(ref whenFalse);
			return this;
		}
	}

	class IncrementStatement : Statement
	{
		public Variable v;
		public VariableAcessor acessor;
		public Expression ammount;
		public bool isNegative; // true if Dec(), false if Inc()

		public IncrementStatement(Variable v, VariableAcessor acessor, Expression ammount, bool isNegative)
		{
			this.v = v;
			this.acessor = acessor;
			this.ammount = ammount;
			this.isNegative = isNegative;
		}

		public override Statement Reduce()
		{
			acessor.PostProcess(); 
			return this;
		}
	}

	class WhileStatement : Statement
	{
		public Expression condition;
		public Statement body;

		public WhileStatement(Expression condition, Statement body)
		{ 
		}

		public override Statement Reduce()
		{
			Expression.ReduceAndReplace(ref condition);
			Statement.ReduceAndReplace(ref body);
			return this;
		}
	}

	class RepeatStatement : Statement
	{
	    public Expression condition;
	    public Statement body;

		public RepeatStatement(Expression condition, Statement body)
		{
			this.condition = condition;
			this.body = body;
		}

		public override Statement Reduce()
		{
			Expression.ReduceAndReplace(ref condition);

			UnaryExpression p = new UnaryExpression();
			p.op = "not";
			p.a = condition;
			this.condition = p;
			Expression.ReduceAndReplace(ref condition);
			ReduceAndReplace(ref body);
			return this;
		}
	}

	class ForStatement : Statement
	{
		public Variable v;
		public VariableAcessor acessor;
		public Expression startValue;
		public Expression endValue;
		public Statement body;
		public bool isInverted; // true if DownTo, false if To

		public ForStatement(Variable v, VariableAcessor ac, Expression start, Expression end, Statement body)
		{
			this.v = v;
			this.acessor = ac;
			this.startValue = start;
			this.endValue = end;
			this.body = body;
		}

		public override Statement Reduce()
		{
			if (acessor!=null)
				acessor.PostProcess();

			Expression.ReduceAndReplace(ref startValue);
			Expression.ReduceAndReplace(ref endValue);
			ReduceAndReplace(ref body);

			return this;
		}
	}

	class WithStatement : Statement
	{
		public Variable v;
		public VariableAcessor acessor;
		public Statement body;
		public WithStatement parent;

		public WithStatement(Variable v, VariableAcessor ac, Statement body, WithStatement parent)
		{
			this.v = v;
			this.acessor = ac;
			this.body = body;
			this.parent = parent;
		}

		public override Statement Reduce()
		{
			acessor.PostProcess();
			ReduceAndReplace(ref body);
			return this;
		}

		public Variable GetVariable(string s, VariableAcessor ac)
		{
			Variable result;
			result = v.GetVariable(s, ac);
			if (result!=null) 
				return result;

			if (parent!=null) 
			{
				result = parent.GetVariable(s, ac);
				if (result!=null) 
					return result;
			}

			return null;
		}
	}

  class ModuleFunction 
  {
    public FunctionSignature signature;
    public Statement body;

    public List<Variable> variables;
    public List<Constant> constants;
    public List<TypeNode> types;
    public List<ModuleFunction> functions;
    
    public ModuleFunction parent;
    public ModuleSection section;

	public ModuleFunction(FunctionSignature signature, ModuleSection section, ModuleFunction parent)
	{
		this.body = null;
		this.signature = signature;
		this.section = section;
		this.parent = parent;
	}

	public string EvaluateConstant(string s)
	{
		s = s.ToLower();
		foreach (Constant c in constants)
		if (c.name.ToLower().Equals(s)) 
		{
			c.value.Reduce();
			if (c.value is ConstantExpression) 
			{
				return (c.value as ConstantExpression).value;
			}
		}
		return null;
	}
  
	public bool IsConstant(string s)
	{
		s = s.ToLower();
		foreach (Constant c in constants) 
		if (c.name.ToLower().Equals(s)) 
			return true;

		return false;
	}

	public bool IsType(string s)
	{
		s = s.ToLower();
		if (s.Equals("integer") || s.Equals("cardinal") || s.Equals("byte") || s.Equals("char") || s.Equals("boolean")
		|| s.Equals("shortint") || s.Equals("word") || s.Equals("smallint") || s.Equals("string") )
			return true;

		foreach (TypeNode t in types) 
		if (t.name.Equals(s))
			return true;

		return false;
	}

	public bool IsFunction(string s)
	{
		s = s.ToLower();
		return signature.name.ToLower().Equals(s);
	}

	public TypeNode GetType(string s)
	{
		s = s.ToLower();
		foreach (TypeNode t in types)
		if (t.name.ToLower().Equals(s))
			return t;
		
		return null;
	}

	public Constant GetConstant(string s)
	{
		s = s.ToLower();
		foreach (Constant c in constants)
		if (c.name.ToLower().Equals(s))
			return c;
		
		return null;
	}

	public ClassProperty GetProperty(string s)
	{
		if (signature == null || signature.owner == null)
			return null;

		ClassSignature c = this.section.parent.GetClass(signature.owner.signature.name);
		if (c != null)
			return c.GetProperty(s);
		else
			return null;
	}

	public Variable GetVariable(string s, VariableAcessor ac)
	{
		ClassSignature c;
		string A,B;
		Variable result;

		Module currentModule = this.section.parent;

		s = s.ToLower();

		if (this == null) 
			return this.section.GetVariable(s, ac);
		
		foreach (Variable v in variables)
		{
			A = v.GetPath();
			if (ac==null) 
			  B = A;
			else
			  B = ac.GetPathBack() + ac.GetPathFront();

			if (v.name.Equals(s) && A.Equals(B))
				return v;
			else
			{
			  result = v.GetVariable(s, ac);
			  if (result!=null) 
				return result;
			}
		}

		if (signature==null)
			return null;

		foreach (Argument arg in signature.arguments) 
		if (arg.name.Equals(s)) 
		{
			return arg;
		} 		
		else
		{
			result = arg.GetVariable(s, ac);
			if (result!=null) 
			  return result;
		}

		if (signature.owner!=null && signature.owner.signature.name.Length>0) 
		{
			c = currentModule.GetClass(signature.owner.signature.name);
			result = c.GetVariable(s, null);
			if (result!=null) 
			  return result;
		}

		if (parent!=null) 
			return parent.GetVariable(s, ac);

		return null;
	}
  }

	class ModuleSection
	{
		public Module parent;
		public string name;
		public List<string> units;
		
		public List<Variable> variables;
		public List<Constant> constants;
		public List<TypeNode> types;
		public List<ModuleFunction> functions;

		public Statement body;
		public Statement initialization;
		public Statement finalization;

		public ModuleSection(string name, Statement body)
		{
			this.name = name;
			this.body = body;
			this.initialization = null;
			this.finalization = null;
		}

		public ModuleSection(string name, Statement initialization, Statement finalization)
		{
			this.name = name;
			this.body = null;
			this.initialization = initialization;
			this.finalization = finalization;
		}

		public string EvaluateConstant(string s)
		{
			s = s.ToLower();
			foreach (Constant c in constants) 
			if (c.name.ToLower().Equals(s)) 
			{
				c.value.Reduce();
				if (c.value is ConstantExpression) 
					return (c.value as ConstantExpression).value;
			}

			foreach(ModuleFunction f in functions)
			{
				string result = f.EvaluateConstant(s);
				if (!String.IsNullOrEmpty(result))
					return result;
			}

			return null;
		}

		public bool IsConstant(string s)
		{		
			s = s.ToLower();
			foreach (ModuleFunction f in functions)
			{
				if (f.IsConstant(s))
					return true;
			}

			foreach (Constant c in constants)
			if (c.name.ToLower().Equals(s)) 
				return true;

		  return false;
		}

		public bool IsFunction(string s)
		{

			foreach (ModuleFunction f in functions)
			{
				if (f.IsFunction(s))
					return true;
			}
		  
			return false;
		}

		public bool IsType(string s)
		{
			foreach (ModuleFunction f in functions)
			{
				if (f.IsType(s))
					return true;
			}

			return false;
		}

		public Constant GetConstant(string s)
		{
			s = s.ToLower();
			foreach (Constant c in constants)
			if (c.name.ToLower().Equals(s))
				return c;
			return null;
		}

		public TypeNode GetType(string s)
		{
			s = s.ToLower();
			foreach (TypeNode t in types)
			if (t.name.ToLower().Equals(s)) 
				return t;
			
			foreach (ModuleFunction f in functions)
			{
				TypeNode result = f.GetType(s);
				if (result!=null) 
					return result;
			}

			return null;
		}

		public Variable GetVariable(string s, VariableAcessor ac)
		{
			string A,B;
			s = s.ToLower();
			foreach (Variable v in variables)
			{
				A = v.GetPath();
				if (ac==null) 
				  B = A;
				else
				  B = ac.GetPathBack() + ac.GetPathFront();

				if (v.name.ToLower().Equals(s) && A.Equals(B))
				{
				  return v;
				}
				else
				{
				  Variable result = v.GetVariable(s, ac);
				  if (result!=null) 
					return result;
				}
			}

			return null;
		}

	}

	class Module 
	{
		public string name;
		public Project parent;
		public ModuleSection interfaceSection;
		public ModuleSection implementationSection;
		public bool isUnit;

		public Module(string name, Project parent, ModuleSection interfaceSection, ModuleSection implementationSection)
		{
			this.name = name;
			this.parent = parent;
			this.interfaceSection = interfaceSection;
			this.implementationSection = implementationSection;
			this.isUnit = (interfaceSection != null);
		}

		private bool isNumber(string s)
		{ 
			if (String.IsNullOrEmpty(s))
				return false;
			return (s[0]>='0' && s[0]<='9');
		}

		public string EvaluateConstant(string s)
		{
			if (isNumber(s))
				return s;

			if (interfaceSection != null)
			{
				string result = interfaceSection.EvaluateConstant(s);
				if (!String.IsNullOrEmpty(result))
					return result;
			}

			if (implementationSection != null)
			{
				string result = implementationSection.EvaluateConstant(s);
				if (!String.IsNullOrEmpty(result))
					return result;
			}

			return null;
		}

		public ClassSignature GetClass(string name)
		{
			ClassSignature result = GetClass(this.interfaceSection, name);
			if (result == null)
				result = GetClass(this.implementationSection, name);
			if (result != null)
				return result;

			foreach (Module mod in parent.modules)
			if (mod!=null)
			{
				result = GetClass(mod.interfaceSection, name);
				if (result == null)
					result = GetClass(mod.implementationSection, name);
				if (result!=null) 
					return result;
			}
		
			return null;
		}


		public ClassSignature GetClass(ModuleSection section, string Name)
		{
			name = name.ToLower();
			foreach (TypeNode t in section.types)
			{
				if (t.type.Equals("class") && t.name.Equals(name))
					return t as ClassSignature;
			}
			return null;
		}

		public TypeNode GetType(string s)
		{
			TypeNode result;
			s = s.ToLower();
			if (interfaceSection!=null) 
			{
				result = interfaceSection.GetType(s);
				if (result!=null) 
					return result;
			}

			if (implementationSection!=null)
			{
				result = implementationSection.GetType(s);
				if (result!=null)
					return result;
			}

			return null;
		}


	}

	class Project
	{
		public string projectName;
		public List<Module> modules;
		public string[] paths;

		public Project(string name)
		{
			this.projectName = name;
			modules = new List<Module>();
			paths = null;
		}

		public TypeNode GetType(string s)
		{
			foreach (Module mod in modules)
			{
				TypeNode result = mod.GetType(s);
				if (result!=null)
					return result;
			}
			return null;
		}

		public Variable GetVariable(string s, VariableAcessor ac)
		{
			foreach (Module mod in modules)
			{
				if (mod!=null)
				{
					Variable result = mod.interfaceSection.GetVariable(s, ac);
					if (result!=null)
						return result;
				}
			}
			return null;
		}

	}

}
