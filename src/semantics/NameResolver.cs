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
		}
	}

	class NameResolver : Processor
	{
		DeclarationsRegistry nameReg;
		SourceFile source;

		// =================================================
		// Public interface
		
		public NameResolver(Traverser t) : base(t) { }

		public NameResolver(TreeTraverse t = null) : base(t) { }
		
		public void Reset(SourceFile sf)
		{
			source = sf;
			nameReg = new DeclarationsRegistry();
			nameReg.LoadRuntimeNames();
		}

		bool Error(string msg)
		{
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine("[ERROR in Name Resolving] " + msg);
			Console.ResetColor();
			return false;
		}


		private Node resolved = null;

		private bool TraverseResolve(Node parent, Node child)
		{
			traverse(child);

			if (resolved != null)
				resolved.Parent = child.Parent;

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
			if (nameReg == null || source == null)
			{	Error("Must initialize NameResolver before using");
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
			traverse(node.body);
			traverse(node.uses);
			return true;
		}
		
		public override bool Visit(UnitNode node)
		{
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
			nameReg.ImportContext(ctx);

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
		
		public override bool Visit(RoutineBody node)
		{
			nameReg.EnterContext("Routine Def body");
			Visit((CodeSection) node);
			nameReg.ExitContext();
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
			Visit((Section) node);
			traverse(node.uses);
			return true;
		}
		
		public override bool Visit(InterfaceSection node)
		{
			Visit((DeclarationSection) node);
			/// Finalize processing of an Unit's interface section, by saving its symbol context
			source.interfContext = nameReg.ExportContext();
			return true;
		}
		
		public override bool Visit(ImplementationSection node)
		{
			// Bothr the Interface and the Implementation do NOT open a new declaring context
			nameReg.EnterContext("implementation");	// TODO CHANGE THIS
			Console.WriteLine("IMPLEMENTATION");

			Visit((DeclarationSection) node);
			return true;
		}
		
		public override bool Visit(AssemblerRoutineBody node)
		{
			Visit((RoutineBody) node);
			return true;
		}
		
		public override bool Visit(Declaration node)
		{
			Visit((Node) node);
			
			// Important!! Register declaration *BEFORE* processing the type.
			// the type may open a context for subtypes
			nameReg.RegisterDeclaration(node.name, node);

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
		
		public override bool Visit(ProceduralType node)
		{
			Visit((TypeNode) node);
			traverse(node.@params);
			traverse(node.funcret);
			traverse(node.returnVar);
			traverse(node.Directives);
			return true;
		}
		
		public override bool Visit(MethodType node)
		{
			Visit((ProceduralType) node);
			return true;
		}
		
		// TODO allow for definitions in implementation of funcs declared in interface
		public override bool Visit(CallableDeclaration node)
		{
			Visit((Declaration) node);	// register decl
			return true;
		}
		
		public override bool Visit(RoutineDeclaration node)
		{
			nameReg.RegisterDeclaration(node.name, node);

			nameReg.EnterContext("Routine " + node.name + " Params");
			if (TraverseResolve(node, node.Type))
				node.type = ResolvedNode<ProceduralType>();
			TraverseResolve(node, node.Directives);

			if (!(node.Parent is CallableDefinition))
				nameReg.ExitContext();
			// else, keep context open for func body

			return true;
		}
		
		public override bool Visit(MethodDeclaration node)
		{
			nameReg.RegisterDeclaration(node.name, node);

			string declTag = "Method " + node.FullName() + " Params";
			if (node.Parent is CallableDefinition)
				nameReg.EnterMethodContext(node.objname);
			else
				nameReg.EnterContext(declTag);

		//	Visit((CallableDeclaration)node);

			if (TraverseResolve(node, node.Type))
				node.type = ResolvedNode<ProceduralType>();
			TraverseResolve(node, node.Directives);

			if (!(node.Parent is CallableDefinition))
				nameReg.ExitContext();
			// else, keep context open for func body
			return true;
		}
		
		public override bool Visit(SpecialMethodDeclaration node)
		{
			Visit((MethodDeclaration) node);
			return true;
		}
		
		public override bool Visit(ConstructorDeclaration node)
		{
			Visit((SpecialMethodDeclaration) node);
			return true;
		}
		
		public override bool Visit(DestructorDeclaration node)
		{
			Visit((SpecialMethodDeclaration) node);
			return true;
		}
		
		public override bool Visit(CallableDefinition node)
		{
			//Visit((Declaration) node);
			traverse(node.header);	// opens context
			traverse(node.body);	// opens context
			return true;
		}
		
		public override bool Visit(RoutineDefinition node)
		{
			Visit((CallableDefinition) node);
			nameReg.ExitContext();			// leave declaring/header context
			return true;
		}
		
		public override bool Visit(MethodDefinition node)
		{
			Visit((CallableDefinition) node);
			nameReg.LeaveMethodContext();	// leave declaring/header context
			return true;
		}
		
		public override bool Visit(CallableDirectives node)
		{
			Visit((Node) node);
			return true;
		}
		
		public override bool Visit(RoutineDirectives node)
		{
			Visit((CallableDirectives) node);
			return true;
		}
		
		public override bool Visit(MethodDirectives node)
		{
			Visit((CallableDirectives) node);
			return true;
		}
		
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
		
		public override bool Visit(CompositeType node)
		{
			Visit((TypeNode) node);
			nameReg.EnterContext("Composite");
			traverse(node.sections);
			nameReg.ExitContext();
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

		public override bool Visit(ClassRefType node)
		{
			// TODO
			return true;
		}

		public override bool Visit(RecordRefType node)
		{
			// TODO
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
		
		public override bool Visit(Statement node)
		{
			Visit((Node) node);
			return true;
		}
		
		public override bool Visit(LabelStatement node)
		{
			Visit((Statement) node);
			traverse(node.stmt);
			return true;
		}
		
		public override bool Visit(EmptyStatement node)
		{
			Visit((Statement) node);
			return true;
		}
		
		public override bool Visit(BreakStatement node)
		{
			Visit((Statement) node);
			return true;
		}
		
		public override bool Visit(ContinueStatement node)
		{
			Visit((Statement) node);
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
		
		public override bool Visit(RepeatLoop node)
		{
			Visit((LoopStatement) node);
			return true;
		}
		
		public override bool Visit(WhileLoop node)
		{
			Visit((LoopStatement) node);
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
		
		public override bool Visit(BlockStatement node)
		{
			Visit((Statement) node);
			traverse(node.stmts);
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
		
		public override bool Visit(AddressLvalue node)
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
		
		public override bool Visit(LvalueExpression node)
		{
			Visit((UnaryExpression) node);
			return true;
		}
		
		public override bool Visit(ExprAsLvalue node)
		{
			Visit((LvalueExpression) node);
			if (TraverseResolve(node, node.expr))
				node.expr = ResolvedNode<Expression>();
			return true;
		}
		
		public override bool Visit(StaticCast node)
		{
			Visit((LvalueExpression) node);
			if (TraverseResolve(node, node.casttype))
				node.casttype = ResolvedNode<VariableType>();
			if (TraverseResolve(node, node.expr))
				node.expr = ResolvedNode<Expression>();
			return true;
		}
		
		/// <summary>
		/// Resolves an id, disambiguating between routines, variables and types
		/// TODO classes, units
		/// </summary>
		public override bool Visit(UnresolvedId node)
		{
			String name = node.id.name;

			Declaration d = nameReg.GetDeclaration(name);
			if (d == null)
				return Error("DeclarationNotFound: " + name);
				//	throw new DeclarationNotFound(name);

			if (d is CallableDeclaration)
				resolved = new RoutineCall(node.id);

			else if (d is ValueDeclaration)
				resolved = node.id;

			else if (d is TypeDeclaration)
				resolved = new TypeWrapper(d.type);	// type for casts and instantiations

			else
				Error("unexpected declaration type " + d);

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
					return Error("Cast may take only 1 argument");
				resolved = new StaticCast((node.func as TypeWrapper).castType, node.args.Get(0));
				return true;
			}
			
			if (node.func is FieldAccess)
			{
				FieldAccess fa = node.func as FieldAccess;
				if (fa.obj is TypeWrapper)
				{
					TypeNode fat = (fa.obj as TypeWrapper).castType;
					if (fat is ClassType)
					{	resolved = new ClassInstantiation(fat as ClassType, fa.field, node.args);
						return true;
					}
					else
						return Error("Cannot instantiate non-class type");
				}
			}

			// default
			resolved = new RoutineCall(node.func, node.args);
			return true;
		}
		
		public override bool Visit(ArrayAccess node)
		{
			Visit((LvalueExpression) node);
			if (TraverseResolve(node, node.lvalue))
				node.lvalue = ResolvedNode<LvalueExpression>();
			traverse(node.acessors);
			if (TraverseResolve(node, node.array))
				node.array = ResolvedNode<ArrayConst>();
			return true;
		}
		
		public override bool Visit(PointerDereference node)
		{
			Visit((LvalueExpression) node);
			if (TraverseResolve(node, node.expr))
				node.expr = ResolvedNode<Expression>();
			return true;
		}
		
		public override bool Visit(InheritedCall node)
		{
			Visit((LvalueExpression) node);
			if (TraverseResolve(node, node.call))
				node.call = ResolvedNode<RoutineCall>();
			return true;
		}
		
		public override bool Visit(RoutineCall node)
		{
			Visit((LvalueExpression) node);
			if (TraverseResolve(node, node.func))
				node.func = ResolvedNode<LvalueExpression>();
			traverse(node.args);
			if (TraverseResolve(node, node.basictype))
				node.basictype = ResolvedNode<ScalarType>();
			return true;
		}
		
		public override bool Visit(FieldAccess node)
		{
			Visit((LvalueExpression) node);
			if (TraverseResolve(node, node.obj))
				node.obj = ResolvedNode<LvalueExpression>();
			return true;
		}
		
		public override bool Visit(Identifier node)
		{
			Visit((LvalueExpression) node);
			return true;
		}
		
		public override bool Visit(TypeNode node)
		{
			Visit((Node) node);
			return true;
		}
		
		public override bool Visit(UnresolvedType node)
		{
			resolved = nameReg.FetchType(node.id);
			return true;
		}
		
		public override bool Visit(UnresolvedClassType node)
		{
			resolved = nameReg.FetchType<ClassType>(node.id);
			return true;
		}
		
		public override bool Visit(UnresolvedVariableType node)
		{
			resolved = nameReg.FetchType<VariableType>(node.id);
			return true;
		}
		
		public override bool Visit(UnresolvedIntegralType node)
		{
			resolved = nameReg.FetchType<IntegralType>(node.id);
			return true;
		}
		
		public override bool Visit(UnresolvedOrdinalType node)
		{
			// TODO drill down to a specific IOrdinalType
			resolved = nameReg.FetchType<VariableType>(node.id);
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

			nameReg.EnterContext("record");
			traverse(node.compTypes);
			nameReg.ExitContext();
			return true;
		}

		#endregion
	}
}
