using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Collections;
using crosspascal.ast;
using crosspascal.ast.nodes;
using crosspascal.core;

namespace crosspascal.semantics
{

	class TypeWrapper : LvalueExpression
	{
		public TypeNode castType { get; set; }

		public TypeWrapper(TypeNode vt)
		{
			castType = vt;
			this.Type = vt;
		}
	}


	/// <summary>
	/// Implements a Name/Declaration Resolver with Type Inference and Validation
	/// </summary>

	class NameResolver : Processor
	{
		public DeclarationsEnvironment declEnv { get; set; }
		SourceFile source;

		// =================================================
		// Public interface
		
		public NameResolver(Traverser t) : base(t) { }

		public NameResolver(TreeTraverse t = null) : base(t) { }
		
		public void Reset(SourceFile sf)
		{
			source = sf;
			declEnv = new DeclarationsEnvironment();
		}

		bool Error(string msg, Node n = null)
		{
			string outp = "[ERROR in Name Resolving] " + msg;
			if (n != null)
				outp += n.Loc.ErrorMsg();

			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine(outp);
			Console.ResetColor();
			return false;
		}


		private Node resolved = null;

		private bool TraverseResolve(Node parent, Node child)
		{
			traverse(child);

			if (resolved != null)
			{	resolved.Loc = child.Loc;
				resolved.Parent = child.Parent;
			}

			return (resolved != null);
		}

		T ResolvedNode<T>() where T : Node
		{
			Node t = resolved;
			resolved = null;
			return (T) t;
		}
		
		public override bool StartProcessing(Node n)
		{
			if (declEnv == null || source == null)
			{	Error("Must initialize NameResolver before using", n);
				return false;
			}

			TraverseResolve(null,n);
			return true;
		}

		
		#region Processor interface
		//
		// Processor interface
		//


		public override bool Visit(FixmeNode node)
		{
			Visit((Node) node);
			return true;
		}
		
		public override bool Visit(NotSupportedNode node)
		{
			Visit((Node) node);
			return true;
		}
		
		public override bool Visit(EmptyNode node)
		{
			Visit((Node) node);
			return true;
		}


		#region	Lists

		public override bool Visit(NodeList node)
		{
			foreach (Node n in node.nodes)
				traverse(n);
			return true;
		}
		
		public override bool Visit(StatementList node)
		{
			foreach (Node n in node.nodes)
				traverse(n);
			return true;
		}
		
		public override bool Visit(TypeList node)
		{
			foreach (Node n in node.nodes)
				traverse(n);
			return true;
		}
		
		public override bool Visit(IntegralTypeList node)
		{
			foreach (Node n in node.nodes)
				traverse(n);
			return true;
		}
		
		public override bool Visit(IdentifierList node)
		{
			foreach (Node n in node.nodes)
				traverse(n);
			return true;
		}
		
		public override bool Visit(DeclarationList node)
		{
			foreach (Node n in node.nodes)
				traverse(n);
			return true;
		}
		
		public override bool Visit(EnumValueList node)
		{
			foreach (Node n in node.nodes)
				traverse(n);
			return true;
		}

		#endregion // Lists


		#region Sections

		public override bool Visit(TranslationUnit node)
		{
			Visit((Declaration) node);
			return true;
		}
		
		public override bool Visit(ProgramNode node)
		{
			Visit((TranslationUnit) node);
			traverse(node.uses);
			traverse(node.body);
			return true;
		}
		
		public override bool Visit(LibraryNode node)
		{
			Visit((TranslationUnit) node);
			traverse(node.uses);
			traverse(node.body);
			return true;
		}
		
		public override bool Visit(UnitNode node)
		{
			// do not allow shadowing in interface section
			declEnv.CreateContext("unit " + node.name, false);
			Visit((TranslationUnit) node);
			traverse(node.@interface);
			traverse(node.implementation);
			traverse(node.initialization);
			traverse(node.finalization);
			return true;
		}
		
		public override bool Visit(PackageNode node)
		{
			Visit((TranslationUnit) node);
			traverse(node.requires);
			traverse(node.contains);
			return true;
		}
		
		public override bool Visit(UnitItem node)
		{
			Visit((Node) node);
			return true;
		}
		
		public override bool Visit(UsesItem node)
		{
			Visit((UnitItem) node);
			
			// Import dependency from a Unit SourceFile already parsed, by loading its interface context
			string id = node.name;
			var ctx = source.GetDependency(id).interfContext;
			ctx.id = id;
			declEnv.ImportContext(ctx);
			return true;
		}
		
		public override bool Visit(RequiresItem node)
		{
			Visit((UnitItem) node);
			return true;
		}
		
		public override bool Visit(ContainsItem node)
		{
			Visit((UnitItem) node);
			return true;
		}
		
		public override bool Visit(ExportItem node)
		{
			Visit((UnitItem) node);
			traverse(node.formalparams);
			return true;
		}
		
		public override bool Visit(Section node)
		{
			Visit((Node) node);
			traverse(node.decls);
			return true;
		}
		
		public override bool Visit(CodeSection node)
		{
			Visit((Section) node);
			traverse(node.block);
			return true;
		}
		
		public override bool Visit(ProgramBody node)
		{
			Visit((CodeSection) node);
			return true;
		}
		
		public override bool Visit(InitializationSection node)
		{
			Visit((CodeSection) node);
			return true;
		}
		
		public override bool Visit(FinalizationSection node)
		{
			Visit((CodeSection) node);
			return true;
		}
		
		public override bool Visit(DeclarationSection node)
		{
			traverse(node.uses);
			Visit((Section)node);
			return true;
		}
		
		public override bool Visit(InterfaceSection node)
		{
			// do not allow shadowing in the implementation section
			declEnv.CreateContext("interface", false);

			Visit((DeclarationSection) node);
			// Finalize processing of an Unit's interface section, by saving its symbol context
			source.interfContext = declEnv.ExportContext();
			return true;
		}
		
		public override bool Visit(ImplementationSection node)
		{
			// allow shadowing in main body
			declEnv.CreateContext("implementation", true);
			Visit((DeclarationSection) node);
			return true;
		}
		
		public override bool Visit(AssemblerRoutineBody node)
		{
			Visit((RoutineBody) node);
			return true;
		}

		#endregion Sections


		#region Declarations
		//
		// Declarations
		// 

		public override bool Visit(Declaration node)
		{
			Visit((Node) node);
			
			// Important!! Register declaration *BEFORE* processing the type.
			// the type may open a context for subtypes
			declEnv.RegisterDeclaration(node.name, node);

		/*	// OLD prolly won't be used
			// all declaration types are visited
			if ((node.type is CompositeType && !(node is CompositeDeclaration))
			||	(node.type is RecordType	&& !(node is RecordDeclaration)))
				// do not traverse, avoid circular deps
				return true;
		*/

			if (TraverseResolve(node, node.type))
				node.type = ResolvedNode<TypeNode>();

			return true;
		}
		
		public override bool Visit(LabelDeclaration node)
		{
			Visit((Declaration) node);
			return true;
		}
		
		public override bool Visit(ValueDeclaration node)
		{
			Visit((Declaration) node);
			return true;
		}
		
		public override bool Visit(VarDeclaration node)
		{
			Visit((ValueDeclaration) node);
			if (TraverseResolve(node, node.init))
				node.init = ResolvedNode<Expression>();
			return true;
		}
		
		public override bool Visit(ParamDeclaration node)
		{
			Visit((ValueDeclaration) node);
			if (TraverseResolve(node, node.init))
				node.init = ResolvedNode<Expression>();
			return true;
		}
		
		public override bool Visit(VarParamDeclaration node)
		{
			Visit((ParamDeclaration) node);
			return true;
		}
		
		public override bool Visit(ConstParamDeclaration node)
		{
			Visit((ParamDeclaration) node);
			return true;
		}
		
		public override bool Visit(OutParamDeclaration node)
		{
			Visit((ParamDeclaration) node);
			return true;
		}
		
		public override bool Visit(ConstDeclaration node)
		{
			Visit((ValueDeclaration) node);
			if (TraverseResolve(node, node.init))
				node.init = ResolvedNode<Expression>();
			return true;
		}
		
		public override bool Visit(EnumValue node)
		{
			Visit((ConstDeclaration) node);
			return true;
		}
		
		public override bool Visit(TypeDeclaration node)
		{
			Visit((Declaration) node);
			return true;
		}

		#endregion Declarations


		#region Routines
		//
		// Routines
		// 

		// TODO override ToString for ever non-singular funcparamtype 

		public override bool Visit(CallableDeclaration node)
		{
			// first resolve the params types, in order to determine the fully qualified proc name
			foreach (ParamDeclaration p in node.Type.@params)
				if (TraverseResolve(p, p.type))
					p.type = ResolvedNode<TypeNode>();

			String fullqualname = node.name;
			if (node.Directives.Contains((int) GeneralDirective.Overload))
				foreach (ParamDeclaration p in node.Type.@params)
				{
					String qualname = p.type.ToString();
					fullqualname += "$" + qualname.Substring(qualname.LastIndexOf('.') + 1);
				}

			node.QualifiedName = fullqualname;
			
			return true;
		}
		
		public override bool Visit(RoutineDeclaration node)
		{
			Visit((CallableDeclaration)node);
			node.declaringScope = node.Parent.Parent as Section;

			declEnv.RegisterDeclaration(node.QualifiedName, node);
			declEnv.CreateContext(node.QualifiedName + " Params");
			traverse(node.Type);
			traverse(node.Directives);
			declEnv.ExitContext();

			return true;
		}

		public override bool Visit(RoutineDefinition node)
		{
			Visit((CallableDeclaration)node);
			node.declaringScope = node.Parent.Parent as Section;

			bool checkRegister = true;
			// check if current callable is an implementation of a declared callable (in the interface)
			var decl = declEnv.GetDeclaration(node.QualifiedName);
			if (decl is RoutineDeclaration && node.declaringScope is ImplementationSection
			&& (decl as RoutineDeclaration).declaringScope is InterfaceSection)
			{	// implementation of a declared routine
				checkRegister = false;	// declare it, ignoring the interface shadowing
			}
			// 	if decl not null, will throw exception when trying to register

			declEnv.RegisterDeclaration(node.QualifiedName, node, checkRegister);

			declEnv.CreateContext(node.QualifiedName + " Params");

			traverse(node.Type);
			traverse(node.Directives);
			traverse(node.body);

			declEnv.ExitContext();
			return true;
		}
		
		public override bool Visit(MethodDeclaration node)
		{
			Visit((CallableDeclaration)node);
			declEnv.RegisterDeclaration(node.QualifiedName, node);
			declEnv.CreateContext(node.QualifiedName + " Params");
			traverse(node.Type);
			traverse(node.Directives);
			declEnv.ExitContext();
			return true;
		}

		public override bool Visit(MethodDefinition node)
		{
			Visit((CallableDeclaration)node);

			declEnv.RegisterDeclaration(node.objname+"."+node.QualifiedName, node);
			CompositeType type = declEnv.CreateCompositeContext(node.objname);
			node.declaringType = type;
			declEnv.CreateContext(node.QualifiedName + " Params");

			traverse(node.Type);
			traverse(node.Directives);
			traverse(node.body);	// opens context

			declEnv.ExitContext();
			declEnv.ExitCompositeContext(node.declaringType);
			return true;
		}

		public override bool Visit(RoutineBody node)
		{
			declEnv.CreateContext("routine def body");
			Visit((CodeSection)node);
			declEnv.ExitContext();
			return true;
		}

		public override bool Visit(ProceduralType node)
		{
			Visit((TypeNode)node);
			traverse(node.@params);
			traverse(node.funcret);
			traverse(node.returnVar);
			traverse(node.Directives);
			return true;
		}

		public override bool Visit(MethodType node)
		{
			Visit((ProceduralType)node);
			return true;
		}
		
		public override bool Visit(ImportDirectives node)
		{
			Visit((RoutineDirectives) node);
			if (node.External != null)
			{
				ExternalDirective dir = node.External;
				if (TraverseResolve(node, dir.File))
					dir.File = ResolvedNode<Expression>();
				if (TraverseResolve(node, dir.Name))
					dir.Name = ResolvedNode<Expression>();
			}
			return true;
		}
		
		#endregion	// routines


		#region Composites
		// 
		// Composites
		// 

		public override bool Visit(CompositeDeclaration node)
		{
			Visit((TypeDeclaration) node);
			return true;
		}
		
		public override bool Visit(ClassDeclaration node)
		{
			Visit((CompositeDeclaration) node);
			return true;
		}
		
		public override bool Visit(InterfaceDeclaration node)
		{
			Visit((CompositeDeclaration) node);
			return true;
		}
		
		/// <remarks>
		/// ATTENTION!! Should only be used in declarations of composites.
		/// For references/ids, use Class/InterfRefType
		/// </remarks>
		public override bool Visit(CompositeType node)
		{
			foreach (var s in node.GetAllMethods())
				s.declaringType = node;

			Visit((TypeNode)node);
			declEnv.CreateInheritedContext(node);
			traverse(node.sections);
			declEnv.ExitCompositeContext(node);
			return true;
		}
		
		public override bool Visit(ClassType node)
		{
			Visit((CompositeType) node);
			return true;
		}
		
		public override bool Visit(InterfaceType node)
		{
			Visit((CompositeType) node);
			traverse(node.guid);
			return true;
		}

		public override bool Visit(ScopedSection node)
		{
			Visit((Section) node);
			traverse(node.fields);
			return true;
		}
		
		public override bool Visit(ScopedSectionList node)
		{
			foreach (Node n in node.nodes)
				traverse(n);
			return true;
		}
		
		public override bool Visit(FieldDeclaration node)
		{
			Visit((ValueDeclaration) node);
			return true;
		}
		
		public override bool Visit(VariantDeclaration node)
		{
			Visit((FieldDeclaration) node);
			traverse(node.varfields);
			return true;
		}
		
		public override bool Visit(VarEntryDeclaration node)
		{
			Visit((FieldDeclaration) node);
			if (TraverseResolve(node, node.tagvalue))
				node.tagvalue = ResolvedNode<Expression>();
			traverse(node.fields);
			return true;
		}
		
		public override bool Visit(PropertyDeclaration node)
		{
			Visit((FieldDeclaration) node);
			traverse(node.specifiers);
			return true;
		}
		
		public override bool Visit(ArrayProperty node)
		{
			Visit((PropertyDeclaration) node);
			traverse(node.indexes);
			return true;
		}
		
		public override bool Visit(PropertySpecifiers node)
		{
			Visit((Node) node);
			if (TraverseResolve(node, node.index))
				node.index = ResolvedNode<IntLiteral>();
			if (TraverseResolve(node, node.stored))
				node.stored = ResolvedNode<ConstExpression>();
			TraverseResolve(node,node.@default);
			return true;
		}

		#endregion	// composites


		#region	Statements
		// 
		// Statements
		//
		
		public override bool Visit(LabelStatement node)
		{
			Visit((Statement) node);
			traverse(node.stmt);
			return true;
		}
		
		public override bool Visit(Assignment node)
		{
			Visit((Statement) node);
			if (TraverseResolve(node, node.lvalue))
				node.lvalue = ResolvedNode<LvalueExpression>();
			if (TraverseResolve(node, node.expr))
				node.expr = ResolvedNode<Expression>();
			return true;
		}
		
		// TODO check label
		public override bool Visit(GotoStatement node)
		{
			Visit((Statement) node);
			return true;
		}
		
		public override bool Visit(IfStatement node)
		{
			Visit((Statement) node);
			if (TraverseResolve(node, node.condition))
				node.condition = ResolvedNode<Expression>();
			traverse(node.thenblock);
			traverse(node.elseblock);
			return true;
		}
		
		public override bool Visit(ExpressionStatement node)
		{
			Visit((Statement) node);
			if (TraverseResolve(node, node.expr))
				node.expr = ResolvedNode<Expression>();
			return true;
		}
		
		public override bool Visit(CaseSelector node)
		{
			Visit((Statement) node);
			traverse(node.list);
			if (TraverseResolve(node, node.stmt))
				node.stmt = ResolvedNode<Statement>();
			return true;
		}
		
		public override bool Visit(CaseStatement node)
		{
			Visit((Statement) node);
			if (TraverseResolve(node, node.condition))
				node.condition = ResolvedNode<Expression>();
			traverse(node.selectors);
			if (TraverseResolve(node, node.caseelse))
				node.caseelse = ResolvedNode<Statement>();
			return true;
		}
		
		public override bool Visit(LoopStatement node)
		{
			Visit((Statement) node);
			if (TraverseResolve(node, node.condition))
				node.condition = ResolvedNode<Expression>();
			if (TraverseResolve(node, node.block))
				node.block = ResolvedNode<Statement>();
			return true;
		}
		
		public override bool Visit(ForLoop node)
		{
			Visit((LoopStatement) node);
			if (TraverseResolve(node, node.var))
				node.var = ResolvedNode<Identifier>();
			if (TraverseResolve(node, node.start))
				node.start = ResolvedNode<Expression>();
			if (TraverseResolve(node, node.end))
				node.end = ResolvedNode<Expression>();
			return true;
		}
		
		public override bool Visit(WithStatement node)
		{
			Visit((Statement) node);
			traverse(node.with);
			if (TraverseResolve(node, node.body))
				node.body = ResolvedNode<Statement>();
			return true;
		}
		
		public override bool Visit(TryFinallyStatement node)
		{
			Visit((Statement) node);
			if (TraverseResolve(node, node.body))
				node.body = ResolvedNode<BlockStatement>();
			if (TraverseResolve(node, node.final))
				node.final = ResolvedNode<BlockStatement>();
			return true;
		}
		
		public override bool Visit(TryExceptStatement node)
		{
			Visit((Statement) node);
			if (TraverseResolve(node, node.body))
				node.body = ResolvedNode<BlockStatement>();
			if (TraverseResolve(node, node.final))
				node.final = ResolvedNode<ExceptionBlock>();
			return true;
		}
		
		public override bool Visit(ExceptionBlock node)
		{
			Visit((Statement) node);
			traverse(node.onList);
			TraverseResolve(node,node.@default);
			return true;
		}
		
		public override bool Visit(RaiseStatement node)
		{
			Visit((Statement) node);
			if (TraverseResolve(node, node.lvalue))
				node.lvalue = ResolvedNode<LvalueExpression>();
			if (TraverseResolve(node, node.expr))
				node.expr = ResolvedNode<Expression>();
			return true;
		}
		
		public override bool Visit(OnStatement node)
		{
			Visit((Statement) node);
			if (TraverseResolve(node, node.body))
				node.body = ResolvedNode<Statement>();
			return true;
		}
		
		public override bool Visit(AssemblerBlock node)
		{
			Visit((BlockStatement) node);
			return true;
		}

		#endregion	// Statements


		#region	Expressions
		//
		// Expressions
		//

		public override bool Visit(Expression node)
		{
			Visit((Node) node);
			if (TraverseResolve(node, node.Type))
				node.Type = ResolvedNode<TypeNode>();
			if (TraverseResolve(node, node.Value))
				node.Value = ResolvedNode<ConstantValue>();
			if (TraverseResolve(node, node.ForcedType))
				node.ForcedType = ResolvedNode<TypeNode>();
			return true;
		}
		
		public override bool Visit(EmptyExpression node)
		{
			Visit((Expression) node);
			return true;
		}
		
		public override bool Visit(ExpressionList node)
		{
			foreach (Node n in node.nodes)
				traverse(n);
			return true;
		}
		
		public override bool Visit(ConstExpression node)
		{
			Visit((Expression) node);
			return true;
		}
		
		public override bool Visit(StructuredConstant node)
		{
			Visit((ConstExpression) node);
			traverse(node.exprlist);
			return true;
		}
		
		public override bool Visit(ArrayConst node)
		{
			Visit((StructuredConstant) node);
			return true;
		}
		
		public override bool Visit(RecordConst node)
		{
			Visit((StructuredConstant) node);
			return true;
		}
		
		public override bool Visit(FieldInitList node)
		{
			Visit((ExpressionList) node);
			foreach (Node n in node.nodes)
				traverse(n);
			return true;
		}
		
		public override bool Visit(FieldInit node)
		{
			Visit((ConstExpression) node);
			if (TraverseResolve(node, node.expr))
				node.expr = ResolvedNode<Expression>();
			return true;
		}
		
		public override bool Visit(ConstIdentifier node)
		{
			Visit((ConstExpression) node);
			return true;
		}
		
		public override bool Visit(Literal node)
		{
			Visit((ConstExpression) node);
			return true;
		}
		
		public override bool Visit(OrdinalLiteral node)
		{
			Visit((Literal) node);
			return true;
		}
		
		public override bool Visit(IntLiteral node)
		{
			Visit((OrdinalLiteral) node);
			return true;
		}
		
		public override bool Visit(CharLiteral node)
		{
			Visit((OrdinalLiteral) node);
			return true;
		}
		
		public override bool Visit(BoolLiteral node)
		{
			Visit((OrdinalLiteral) node);
			return true;
		}
		
		public override bool Visit(StringLiteral node)
		{
			Visit((Literal) node);
			return true;
		}
		
		public override bool Visit(RealLiteral node)
		{
			Visit((Literal) node);
			return true;
		}
		
		public override bool Visit(PointerLiteral node)
		{
			Visit((Literal) node);
			return true;
		}
		
		public override bool Visit(ConstantValue node)
		{
			Visit((Node) node);
			return true;
		}
		
		public override bool Visit(IntegralValue node)
		{
			Visit((ConstantValue) node);
			return true;
		}
		
		public override bool Visit(StringValue node)
		{
			Visit((ConstantValue) node);
			return true;
		}
		
		public override bool Visit(RealValue node)
		{
			Visit((ConstantValue) node);
			return true;
		}
		
		public override bool Visit(BinaryExpression node)
		{
			Visit((Expression) node);
			return true;
		}
		
		public override bool Visit(SetIn node)
		{
			Visit((BinaryExpression) node);
			if (TraverseResolve(node, node.expr))
				node.expr = ResolvedNode<Expression>();
			if (TraverseResolve(node, node.set))
				node.set = ResolvedNode<Expression>();
			return true;
		}
		
		public override bool Visit(SetRange node)
		{
			Visit((BinaryExpression) node);
			return true;
		}
		
		public override bool Visit(ArithmethicBinaryExpression node)
		{
			Visit((BinaryExpression) node);
			if (TraverseResolve(node, node.left))
				node.left = ResolvedNode<Expression>();
			if (TraverseResolve(node, node.right))
				node.right = ResolvedNode<Expression>();
			return true;
		}
		
		public override bool Visit(Subtraction node)
		{
			Visit((ArithmethicBinaryExpression) node);
			return true;
		}
		
		public override bool Visit(Addition node)
		{
			Visit((ArithmethicBinaryExpression) node);
			return true;
		}
		
		public override bool Visit(Product node)
		{
			Visit((ArithmethicBinaryExpression) node);
			return true;
		}
		
		public override bool Visit(Division node)
		{
			Visit((ArithmethicBinaryExpression) node);
			return true;
		}
		
		public override bool Visit(Quotient node)
		{
			Visit((ArithmethicBinaryExpression) node);
			return true;
		}
		
		public override bool Visit(Modulus node)
		{
			Visit((ArithmethicBinaryExpression) node);
			return true;
		}
		
		public override bool Visit(ShiftRight node)
		{
			Visit((ArithmethicBinaryExpression) node);
			return true;
		}
		
		public override bool Visit(ShiftLeft node)
		{
			Visit((ArithmethicBinaryExpression) node);
			return true;
		}
		
		public override bool Visit(LogicalBinaryExpression node)
		{
			Visit((BinaryExpression) node);
			if (TraverseResolve(node, node.left))
				node.left = ResolvedNode<Expression>();
			if (TraverseResolve(node, node.right))
				node.right = ResolvedNode<Expression>();
			return true;
		}
		
		public override bool Visit(LogicalAnd node)
		{
			Visit((LogicalBinaryExpression) node);
			return true;
		}
		
		public override bool Visit(LogicalOr node)
		{
			Visit((LogicalBinaryExpression) node);
			return true;
		}
		
		public override bool Visit(LogicalXor node)
		{
			Visit((LogicalBinaryExpression) node);
			return true;
		}
		
		public override bool Visit(Equal node)
		{
			Visit((LogicalBinaryExpression) node);
			return true;
		}
		
		public override bool Visit(NotEqual node)
		{
			Visit((LogicalBinaryExpression) node);
			return true;
		}
		
		public override bool Visit(LessThan node)
		{
			Visit((LogicalBinaryExpression) node);
			return true;
		}
		
		public override bool Visit(LessOrEqual node)
		{
			Visit((LogicalBinaryExpression) node);
			return true;
		}
		
		public override bool Visit(GreaterThan node)
		{
			Visit((LogicalBinaryExpression) node);
			return true;
		}
		
		public override bool Visit(GreaterOrEqual node)
		{
			Visit((LogicalBinaryExpression) node);
			return true;
		}
		
		public override bool Visit(TypeBinaryExpression node)
		{
			Visit((BinaryExpression) node);
			if (TraverseResolve(node, node.expr))
				node.expr = ResolvedNode<Expression>();
			if (TraverseResolve(node, node.types))
				node.types = ResolvedNode<TypeNode>();
			return true;
		}
		
		public override bool Visit(TypeIs node)
		{
			Visit((TypeBinaryExpression) node);
			return true;
		}
		
		public override bool Visit(RuntimeCast node)
		{
			Visit((TypeBinaryExpression) node);
			return true;
		}
		
		public override bool Visit(UnaryExpression node)
		{
			Visit((Expression) node);
			return true;
		}
		
		public override bool Visit(SimpleUnaryExpression node)
		{
			Visit((Expression) node);
			if (TraverseResolve(node, node.expr))
				node.expr = ResolvedNode<Expression>();
			return true;
		}
		
		public override bool Visit(UnaryPlus node)
		{
			Visit((SimpleUnaryExpression) node);
			return true;
		}
		
		public override bool Visit(UnaryMinus node)
		{
			Visit((SimpleUnaryExpression) node);
			return true;
		}
		
		public override bool Visit(LogicalNot node)
		{
			Visit((SimpleUnaryExpression) node);
			return true;
		}
		
		public override bool Visit(Set node)
		{
			Visit((UnaryExpression) node);
			traverse(node.setelems);
			return true;
		}

		public override bool Visit(AddressLvalue node)
		{
			Visit((SimpleUnaryExpression)node);
			return true;
		}	

		#endregion  // Expressions



		#region Lvalues

		public override bool Visit(LvalueExpression node)
		{
		//	Visit((UnaryExpression) node);
			return true;
		}
		
		public override bool Visit(ExprAsLvalue node)
		{
			Visit((LvalueExpression) node);
			if (TraverseResolve(node, node.expr))
				node.expr = ResolvedNode<Expression>();

			node.Type = node.expr.Type;
			return true;
		}
		
		public override bool Visit(StaticCast node)
		{
			Visit((LvalueExpression) node);
			if (TraverseResolve(node, node.casttype))
				node.casttype = ResolvedNode<VariableType>();
			if (TraverseResolve(node, node.expr))
				node.expr = ResolvedNode<Expression>();

			// TODO check if cast is valid

			node.Type = node.casttype;
			return true;
		}


		/// <summary>
		/// Resolves an id, disambiguating between routines, variables and types
		/// </summary>
		public override bool Visit(UnresolvedId node)
		{
			String name = node.id.name;

			Declaration d = declEnv.GetDeclaration(name);
			if (d == null)
				return Error("DeclarationNotFound: " + name, node);
				//	throw new DeclarationNotFound(name);

			if (d is TypeDeclaration)
			{	resolved = new TypeWrapper(d.type);	// type for casts and instantiations
				return true;
			}

			else if (d.type is ProceduralType)
			{
				var call = new RoutineCall(node.id);
				call.Type = (d.type as ProceduralType).funcret;
				resolved = call;
			}

			else if (d is ValueDeclaration)
				resolved = node.id;

			else
				Error("unexpected declaration type " + d, node);

			// Process identifier
			node.id.Type = d.type;
			node.id.decl = d;
			return true;
		}
		
		/// <summary>
		/// Resolves a call-like syntactic structure, into
		/// 1) routine calls
		/// 2) static type casts
		/// 3) class instantiations
		/// </summary>
		public override bool Visit(UnresolvedCall node)
		{
			if (TraverseResolve(node, node.func))
				node.func = ResolvedNode<LvalueExpression>();
			traverse(node.args);

			if (node.func is TypeWrapper)
			{
				if (node.args.Count() > 1)
					return Error("Cast may take only 1 argument", node);
				resolved = new StaticCast((node.func as TypeWrapper).castType, node.args.Get(0));
			}

			if (node.func is RoutineCall)	// call with an id, default
			{
				RoutineCall call = node.func as RoutineCall;
				call.args = node.args;
				resolved = call;
			}

			else
			{	RoutineCall call = new RoutineCall(node.func, node.args);
				traverse(call);	// resolve if needed
				if (resolved == null)
					resolved = call;
			}

			return true;
		}
		
		/// <summary>
		/// Process a routine call, or a class instantiation
		/// </summary>
		public override bool Visit(RoutineCall node)
		{
			Visit((LvalueExpression) node);
			if (TraverseResolve(node, node.func))
				node.func = ResolvedNode<LvalueExpression>();
			traverse(node.args);

			if (!(node.func.Type is ProceduralType))
				return Error("Attempt to Call a non-procedural type: " + node.func.Type, node);

			MethodType mt = node.func.Type as MethodType;
			if (mt != null && mt.IsConstructor)
			{
				if (!(node.func is ObjectAccess)
				|| !((node.func as ObjectAccess).obj is TypeWrapper))
					return Error("Attempt to call constructor using an instance reference", node);

				ObjectAccess ac = (node.func as ObjectAccess);
				resolved = new ClassInstantiation((ac.obj as TypeWrapper).Type as ClassType, ac.field, node.args);
			}

			return true;
		}
		
		/// <summary>
		/// Process an Object Access. May be an access to a field or a method,
		/// in which case it may be a class instantiation
		/// </summary>
		public override bool Visit(ObjectAccess node)
		{
			Visit((LvalueExpression) node);
			if (TraverseResolve(node, node.obj))
				node.obj = ResolvedNode<LvalueExpression>();

			TypeNode objtype = node.obj.Type;
			if (!objtype.IsFieldedType())
				return Error("Attempt to access non-object type", node);

			if (node.obj is TypeWrapper)
			{
				// nothing, catch as a method call
			}

			Declaration d;
			if (objtype is RecordType)
			{
				if ((d = (objtype as RecordType).GetField(node.field)) == null)
					return Error("Field " + node.field + " not found in Record", node);
			}

			else if (objtype is ClassType)
			{
				if ((d = (objtype as ClassType).GetMember(node.field)) == null)
					return Error("Member " + node.field + " not found in Class " + (objtype as ClassType).Name, node);
			}

			else if (objtype is InterfaceType)
			{
				if ((d = (objtype as InterfaceType).GetMethod(node.field)) == null)
					return Error("Method " + node.field + " not found in Interface " + (objtype as InterfaceType).Name, node);
			}
			else
				return Error("unknown object type", node);	// should never happen

			node.Type = d.type;
			// d can be a method or field
			if (d is MethodDeclaration)
			{
				MethodType mt = node.Type as MethodType;
				if (mt != null && mt.IsConstructor)
				{
					if (!(node.obj is TypeWrapper))
						return Error("Attempt to call constructor using an instance reference", node);

					resolved = new ClassInstantiation((node.obj as TypeWrapper).Type as ClassType, node.field);
				}
				else
					resolved = new RoutineCall(node);
			}

			return true;
		}
		
		public override bool Visit(Identifier node)
		{
			Visit((LvalueExpression) node);
			
			Declaration d = declEnv.GetDeclaration(node.name);
			node.Type = d.type;
			node.decl = d;

			return true;
		}

		public override bool Visit(ArrayAccess node)
		{
			Visit((LvalueExpression)node);
			if (TraverseResolve(node, node.lvalue))
				node.lvalue = ResolvedNode<LvalueExpression>();
			traverse(node.acessors);
			if (TraverseResolve(node, node.array))
				node.array = ResolvedNode<ArrayConst>();

			if (node.array != null && node.lvalue != null)
				Error("Internal: Array access both const and var", node);

			// const array access
			if (node.array != null)
			{
				// TODO
			}

			// var array access
			if (node.lvalue != null)
			{
				if (!(node.lvalue.Type is ArrayType))
					return Error("Expected array type in Array Access", node);

				node.Type = (node.lvalue.Type as ArrayType).basetype;
			}

			return true;
		}

		public override bool Visit(PointerDereference node)
		{
			Visit((LvalueExpression)node);
			if (TraverseResolve(node, node.expr))
				node.expr = ResolvedNode<Expression>();

			if (!(node.expr.Type is PointerType))
				return Error("Attempt to dereference non-pointer type", node);

			node.Type = (node.expr.Type as PointerType).pointedType;
			return true;
		}

		public override bool Visit(InheritedCall node)
		{
			Visit((LvalueExpression)node);
			if (TraverseResolve(node, node.call))
				node.call = ResolvedNode<RoutineCall>();
			return true;
		}

		#endregion	// lvalues


		#region	Types
		//
		// Types
		//

		public override bool Visit(TypeNode node)
		{
			Visit((Node) node);
			return true;
		}
		
		public override bool Visit(UnresolvedType node)
		{
			resolved = declEnv.FetchType(node.id);
			if (resolved is RecordType)
				resolved = new RecordRefType(node.id, (resolved as RecordType));
			if (resolved is ClassType)
				resolved = new ClassRefType(node.id, (resolved as ClassType));
			return true;
		}

		public override bool Visit(ClassRefType node)
		{
			if (node.reftype == null)
				node.reftype = declEnv.FetchType<ClassType>(node.qualifid);
			return true;
		}

		public override bool Visit(RecordRefType node)
		{
			if (node.reftype == null)
				node.reftype = declEnv.FetchType<RecordType>(node.qualifid);
			return true;
		}
		
		public override bool Visit(UnresolvedVariableType node)
		{
			resolved = declEnv.FetchType<VariableType>(node.id);
			if (resolved is RecordType)
				resolved = new RecordRefType(node.id, (resolved as RecordType));
			if (resolved is ClassType)
				resolved = new ClassRefType(node.id, (resolved as ClassType));
			return true;
		}
		
		public override bool Visit(UnresolvedIntegralType node)
		{
			resolved = declEnv.FetchType<IntegralType>(node.id);
			return true;
		}
		
		public override bool Visit(UnresolvedOrdinalType node)
		{
			// TODO drill down to a specific IOrdinalType
			resolved = declEnv.FetchType<VariableType>(node.id);
			return true;
		}
		
		public override bool Visit(VariableType node)
		{
			Visit((TypeNode) node);
			return true;
		}
		
		public override bool Visit(MetaclassType node)
		{
			Visit((VariableType) node);
			if (TraverseResolve(node, node.baseType))
				node.baseType = ResolvedNode<TypeNode>();
			return true;
		}
		
		public override bool Visit(EnumType node)
		{
			Visit((VariableType) node);
			traverse(node.enumVals);
			return true;
		}
		
		public override bool Visit(RangeType node)
		{
			Visit((VariableType) node);
			if (TraverseResolve(node, node.min))
				node.min = ResolvedNode<Expression>();
			if (TraverseResolve(node, node.max))
				node.max = ResolvedNode<Expression>();
			return true;
		}
		
		public override bool Visit(ScalarType node)
		{
			Visit((VariableType) node);
			return true;
		}
		
		public override bool Visit(IntegralType node)
		{
			Visit((ScalarType) node);
			return true;
		}
		
		public override bool Visit(IntegerType node)
		{
			Visit((IntegralType) node);
			return true;
		}
		
		public override bool Visit(SignedIntegerType node)
		{
			Visit((IntegerType) node);
			return true;
		}
		
		public override bool Visit(UnsignedIntegerType node)
		{
			Visit((IntegerType) node);
			return true;
		}
		
		public override bool Visit(UnsignedInt8Type node)
		{
			Visit((UnsignedIntegerType) node);
			return true;
		}
		
		public override bool Visit(UnsignedInt16Type node)
		{
			Visit((UnsignedIntegerType) node);
			return true;
		}
		
		public override bool Visit(UnsignedInt32Type node)
		{
			Visit((UnsignedIntegerType) node);
			return true;
		}
		
		public override bool Visit(UnsignedInt64Type node)
		{
			Visit((UnsignedIntegerType) node);
			return true;
		}
		
		public override bool Visit(SignedInt8Type node)
		{
			Visit((SignedIntegerType) node);
			return true;
		}
		
		public override bool Visit(SignedInt16Type node)
		{
			Visit((SignedIntegerType) node);
			return true;
		}
		
		public override bool Visit(SignedInt32Type node)
		{
			Visit((SignedIntegerType) node);
			return true;
		}
		
		public override bool Visit(SignedInt64Type node)
		{
			Visit((IntegerType) node);
			return true;
		}
		
		public override bool Visit(BoolType node)
		{
			Visit((IntegralType) node);
			return true;
		}
		
		public override bool Visit(CharType node)
		{
			Visit((IntegralType) node);
			return true;
		}
		
		public override bool Visit(RealType node)
		{
			Visit((ScalarType) node);
			return true;
		}
		
		public override bool Visit(FloatType node)
		{
			Visit((RealType) node);
			return true;
		}
		
		public override bool Visit(DoubleType node)
		{
			Visit((RealType) node);
			return true;
		}
		
		public override bool Visit(ExtendedType node)
		{
			Visit((RealType) node);
			return true;
		}
		
		public override bool Visit(CurrencyType node)
		{
			Visit((RealType) node);
			return true;
		}
		
		public override bool Visit(StringType node)
		{
			Visit((ScalarType) node);
			return true;
		}
		
		public override bool Visit(FixedStringType node)
		{
			Visit((StringType) node);
			if (TraverseResolve(node, node.expr))
				node.expr = ResolvedNode<Expression>();
			return true;
		}
		
		public override bool Visit(VariantType node)
		{
			Visit((VariableType) node);
			if (TraverseResolve(node, node.actualtype))
				node.actualtype = ResolvedNode<VariableType>();
			return true;
		}
		
		public override bool Visit(PointerType node)
		{
			Visit((ScalarType) node);
			if (TraverseResolve(node, node.pointedType))
				node.pointedType = ResolvedNode<TypeNode>();
			return true;
		}
		
		public override bool Visit(StructuredType node)
		{
			Visit((VariableType) node);
			if (TraverseResolve(node, node.basetype))
				node.basetype = ResolvedNode<TypeNode>();
			return true;
		}
		
		public override bool Visit(ArrayType node)
		{
			Visit((StructuredType) node);
			return true;
		}
		
		public override bool Visit(SetType node)
		{
			Visit((StructuredType) node);
			return true;
		}
		
		public override bool Visit(FileType node)
		{
			Visit((StructuredType) node);
			return true;
		}
		
		public override bool Visit(RecordType node)
		{
			Visit((StructuredType) node);

			declEnv.CreateContext("record");
			traverse(node.compTypes);
			declEnv.ExitContext();
			return true;
		}

		#endregion // types

		#endregion
	}
}
