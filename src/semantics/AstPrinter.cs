using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Collections;
using crosspascal.ast;
using crosspascal.ast.nodes;

namespace crosspascal.semantics
{

	// For debug, prints the whole tree

	class AstPrinter : Processor<bool>
	{
		private int identLevel = 0;
		private StringBuilder builder = new StringBuilder();

		const string identText = "  ";

		// =================================================
		// Public interface

		public AstPrinter(Traverser<bool> t) : base(t) { }
		
		public AstPrinter(TreeTraverse<bool> t = null) : base(t) { }


		public string Output()
		{
			return builder.ToString();
		}
		
		public string GetRepresenation()
		{
			return this.ToString();
		}
		
		public void Reset()
		{
			identLevel = 0;
			builder.Clear();
		}

		// =================================================

		private String GetNodeNames(Node n)
		{
			BindingFlags allbindings = BindingFlags.Instance| BindingFlags.Public
									|  BindingFlags.Static	| BindingFlags.FlattenHierarchy;
			StringComparison scase = StringComparison.OrdinalIgnoreCase;

			Type ntype = n.GetType();
			string names = "";

			foreach (var f in ntype.GetFields(allbindings))
			{
				string fname = f.Name.ToLower();
				if (f.FieldType.Name.Equals("string", scase))
				{
					if (fname.Contains("name")
					|| fname.Contains("id")
					|| fname.Contains("val")
					|| fname.Contains("qualif")
					|| fname.Contains("field"))
						names += " " + f.GetValue(n);
				}
			}

			if (names != "")
				names = ":" + names;
			return names;
		}

		// Printing helper
		private void EnterNode(Node n)
		{
			string name = n.GetType().Name + GetNodeNames(n);
			builder.Append(String.Concat(Enumerable.Repeat(identText, identLevel)));
			builder.AppendLine(name);
			identLevel++;
		}

		private void LeaveNode(Node n)
		{
			identLevel--;
		}

		
		private void TraversePrint(Node n)
		{
			if (n == null)
				return;

		//	Console.WriteLine("[DEBUG] Traverse node " + n);
			EnterNode(n);
			traverse(n);
			LeaveNode(n);
		}
		

		public override bool Process(Node n)
		{
			TraversePrint(n);
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
				TraversePrint(n);
			return true;
		}
		
		public override bool Visit(StatementList node)
		{
			foreach (Node n in node.nodes)
				TraversePrint(n);
			return true;
		}
		
		public override bool Visit(TypeList node)
		{
			foreach (Node n in node.nodes)
				TraversePrint(n);
			return true;
		}
				
		public override bool Visit(DeclarationList node)
		{
			foreach (Node n in node.nodes)
				TraversePrint(n);
			return true;
		}
		
		public override bool Visit(EnumValueList node)
		{
			foreach (Node n in node.nodes)
				TraversePrint(n);
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
			TraversePrint(node.section);
			return true;
		}
		
		public override bool Visit(LibraryNode node)
		{
			Visit((TranslationUnit) node);
			TraversePrint(node.section);
			return true;
		}

		public override bool Visit(ProgramSection node)
		{
			Visit((TopLevelDeclarationSection) node);
			TraversePrint(node.block);
			return true;
		}
		
		public override bool Visit(UnitNode node)
		{
			Visit((TranslationUnit) node);
			TraversePrint(node.@interface);
			TraversePrint(node.implementation);
			TraversePrint(node.initialization);
			TraversePrint(node.finalization);
			return true;
		}
		
		public override bool Visit(PackageNode node)
		{
			Visit((TranslationUnit) node);
			TraversePrint(node.requires);
			TraversePrint(node.contains);
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
			TraversePrint(node.formalparams);
			return true;
		}
		
		public override bool Visit(Section node)
		{
			Visit((Node) node);
			traverse(node.decls); // do not print
			return true;
		}
		
		public override bool Visit(RoutineSection node)
		{
			Visit((Section) node);
			TraversePrint(node.block);
			return true;
		}

		public override bool Visit(ParametersSection node)
		{
			Visit((Section)node);
			TraversePrint(node.returnVar);
			return true;
		}
						
		public override bool Visit(TopLevelDeclarationSection node)
		{
			TraversePrint(node.uses);
			Visit((Section)node);
			return true;
		}
		
		public override bool Visit(InterfaceSection node)
		{
			Visit((TopLevelDeclarationSection) node);
			return true;
		}
		
		public override bool Visit(ImplementationSection node)
		{
			Visit((TopLevelDeclarationSection) node);
			return true;
		}
		
		public override bool Visit(Declaration node)
		{
			Visit((Node) node);
			TraversePrint(node.type);
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
			TraversePrint(node.init);
			return true;
		}
		
		public override bool Visit(ParamDeclaration node)
		{
			Visit((ValueDeclaration) node);
			TraversePrint(node.init);
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
			TraversePrint(node.init);
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
			TraversePrint(node.@params);
			TraversePrint(node.funcret);
			TraversePrint(node.Directives);
			return true;
		}
		
		public override bool Visit(MethodType node)
		{
			Visit((ProceduralType) node);
			return true;
		}
		
		public override bool Visit(CallableDeclaration node)
		{
			Visit((Declaration) node);
			TraversePrint(node.Directives);
			return true;
		}
		
		public override bool Visit(RoutineDeclaration node)
		{
			Visit((CallableDeclaration) node);
			return true;
		}
		
		public override bool Visit(MethodDeclaration node)
		{
			Visit((CallableDeclaration) node);
			return true;
		}

		public override bool Visit(RoutineDefinition node)
		{
			Visit((RoutineDeclaration)node);
			TraversePrint(node.body);
			return true;
		}

		public override bool Visit(MethodDefinition node)
		{
			Visit((MethodDeclaration)node);
			TraversePrint(node.body);
			return true;
		}

		public override bool Visit(RoutineDirectives node)
		{
			Visit((Node) node);
			return true;
		}
		
		public override bool Visit(ImportDirectives node)
		{
			Visit((RoutineDirectives) node);
			return true;
		}
		
		public override bool Visit(MethodDirectives node)
		{
			Visit((RoutineDirectives) node);
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
			traverse(node.section);	// do not print
			return true;
		}
		
		public override bool Visit(ClassType node)
		{
			Visit((CompositeType) node);
			TraversePrint(node.self);
			return true;
		}
		
		public override bool Visit(InterfaceType node)
		{
			Visit((CompositeType) node);
			TraversePrint(node.guid);
			return true;
		}

		public override bool Visit(ClassRefType node)
		{
		//	Visit((ClassType)node);
			//	Do not traverse this node! circular dependency
			//	traverse(node.reftype);
			return true;
		}

		public override bool Visit(RecordRefType node)
		{
			Visit((RecordType)node);
			//	Do not traverse this node! circular dependency
			//	traverse(node.reftype);
			return true;
		}

		public override bool Visit(ObjectSection node)
		{
			traverse(node.fields);	// do not print
			Visit((Section)node);
			traverse(node.properties);	// do not print
			return true;
		}
		
		public override bool Visit(FieldDeclaration node)
		{
			Visit((ValueDeclaration) node);
			return true;
		}

		public override bool Visit(RecordFieldDeclaration node)
		{
			Visit((ValueDeclaration)node);
			return true;
		}

		public override bool Visit(VariantDeclaration node)
		{
			Visit((RecordFieldDeclaration) node);
			TraversePrint(node.varfields);
			return true;
		}
		
		public override bool Visit(VarEntryDeclaration node)
		{
			Visit((RecordFieldDeclaration) node);
			TraversePrint(node.tagvalue);
			TraversePrint(node.fields);
			return true;
		}
		
		public override bool Visit(PropertyDeclaration node)
		{
			Visit((FieldDeclaration) node);
			TraversePrint(node.specifiers);
			return true;
		}
		
		public override bool Visit(ArrayProperty node)
		{
			Visit((PropertyDeclaration) node);
			TraversePrint(node.indexes);
			return true;
		}
		
		public override bool Visit(PropertySpecifiers node)
		{
			Visit((Node) node);
			TraversePrint(node.index);
			TraversePrint(node.stored);
			TraversePrint(node.@default);
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
			TraversePrint(node.stmt);
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
			TraversePrint(node.lvalue);
			TraversePrint(node.expr);
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
			TraversePrint(node.condition);
			TraversePrint(node.thenblock);
			TraversePrint(node.elseblock);
			return true;
		}
		
		public override bool Visit(ExpressionStatement node)
		{
			Visit((Statement) node);
			TraversePrint(node.expr);
			return true;
		}
		
		public override bool Visit(CaseSelector node)
		{
			Visit((Statement) node);
			TraversePrint(node.list);
			TraversePrint(node.stmt);
			return true;
		}
		
		public override bool Visit(CaseStatement node)
		{
			Visit((Statement) node);
			TraversePrint(node.condition);
			TraversePrint(node.selectors);
			TraversePrint(node.caseelse);
			return true;
		}
		
		public override bool Visit(LoopStatement node)
		{
			Visit((Statement) node);
			TraversePrint(node.condition);
			TraversePrint(node.block);
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
			TraversePrint(node.var);
			TraversePrint(node.start);
			TraversePrint(node.end);
			return true;
		}
		
		public override bool Visit(BlockStatement node)
		{
			Visit((Statement) node);
			traverse(node.stmts);	// do not print
			return true;
		}
		
		public override bool Visit(WithStatement node)
		{
			Visit((Statement) node);
			TraversePrint(node.with);
			TraversePrint(node.body);
			return true;
		}
		
		public override bool Visit(TryFinallyStatement node)
		{
			Visit((Statement) node);
			TraversePrint(node.body);
			TraversePrint(node.final);
			return true;
		}
		
		public override bool Visit(TryExceptStatement node)
		{
			Visit((Statement) node);
			TraversePrint(node.body);
			TraversePrint(node.final);
			return true;
		}
		
		public override bool Visit(ExceptionBlock node)
		{
			Visit((Statement) node);
			TraversePrint(node.onList);
			TraversePrint(node.@default);
			return true;
		}
		
		public override bool Visit(RaiseStatement node)
		{
			Visit((Statement) node);
			TraversePrint(node.lvalue);
			TraversePrint(node.expr);
			return true;
		}
		
		public override bool Visit(OnStatement node)
		{
			Visit((Statement) node);
			TraversePrint(node.body);
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
		//	TraversePrint(node.Type);
			builder.Remove(builder.Length - 2, 2);
			if (node.Type == null)
				builder.AppendLine("  (nulltype)");
			else
				builder.AppendLine("  (" + node.Type.NodeName() + ")");
			TraversePrint(node.Value);
		//	TraversePrint(node.ForcedType);
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
				TraversePrint(n);
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
			TraversePrint(node.exprlist);
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
				TraversePrint(n);
			return true;
		}
		
		public override bool Visit(FieldInit node)
		{
			Visit((ConstExpression) node);
			TraversePrint(node.expr);
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
			TraversePrint(node.expr);
			TraversePrint(node.set);
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
			TraversePrint(node.left);
			TraversePrint(node.right);
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
			TraversePrint(node.left);
			TraversePrint(node.right);
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
			TraversePrint(node.expr);
			TraversePrint(node.types);
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
			TraversePrint(node.expr);
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
			TraversePrint(node.setelems);
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
			TraversePrint(node.expr);
			return true;
		}
		
		public override bool Visit(StaticCast node)
		{
			Visit((LvalueExpression) node);
			TraversePrint(node.casttype);
			TraversePrint(node.expr);
			return true;
		}
		
		public override bool Visit(UnresolvedId node)
		{
			Visit((LvalueExpression) node);
			traverse(node.id);
			return true;
		}
		
		public override bool Visit(UnresolvedCall node)
		{
			Visit((LvalueExpression) node);
			TraversePrint(node.func);
			TraversePrint(node.args);
			return true;
		}
		
		public override bool Visit(ArrayAccess node)
		{
			Visit((LvalueExpression) node);
			TraversePrint(node.lvalue);
			TraversePrint(node.acessors);
			TraversePrint(node.array);
			return true;
		}
		
		public override bool Visit(PointerDereference node)
		{
			Visit((LvalueExpression) node);
			TraversePrint(node.expr);
			return true;
		}
		
		public override bool Visit(InheritedCall node)
		{
			Visit((RoutineCall)node);
			return true;
		}
		
		public override bool Visit(RoutineCall node)
		{
			Visit((LvalueExpression) node);
			TraversePrint(node.func);
			TraversePrint(node.args);
			return true;
		}
		
		public override bool Visit(ObjectAccess node)
		{
			Visit((LvalueExpression) node);
			TraversePrint(node.obj);
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
			Visit((TypeNode) node);
			return true;
		}
		
		public override bool Visit(UnresolvedVariableType node)
		{
			Visit((VariableType) node);
			return true;
		}
		
		public override bool Visit(UnresolvedIntegralType node)
		{
			Visit((IntegralType) node);
			return true;
		}
		
		public override bool Visit(UnresolvedOrdinalType node)
		{
			Visit((VariableType) node);
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
			TraversePrint(node.baseType);
			return true;
		}
		
		public override bool Visit(EnumType node)
		{
			Visit((VariableType) node);
			TraversePrint(node.enumVals);
			return true;
		}
		
		public override bool Visit(RangeType node)
		{
			Visit((VariableType) node);
			TraversePrint(node.min);
			TraversePrint(node.max);
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
			TraversePrint(node.expr);
			return true;
		}
		
		public override bool Visit(VariantType node)
		{
			Visit((VariableType) node);
			TraversePrint(node.actualtype);
			return true;
		}
		
		public override bool Visit(PointerType node)
		{
			Visit((ScalarType) node);
			TraversePrint(node.pointedType);
			return true;
		}
		
		public override bool Visit(StructuredType node)
		{
			Visit((VariableType) node);
			TraversePrint(node.basetype);
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
			traverse(node.compTypes);
			return true;
		}

		#endregion
	}
}
