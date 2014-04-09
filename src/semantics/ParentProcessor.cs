using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using crosspascal.ast;
using crosspascal.ast.nodes;

namespace crosspascal.semantics
{
	class ParentProcessor : Processor<bool>
	{
		
		private bool TraverseSetParent(Node parent, Node child)
		{
			if (child == null)
				return false;

			child.Parent = parent;
			return traverse(child);
		}

		public override bool DefaultReturnValue()
		{
			return true;
		}


		//
		// Do not set parent backreference in types! 
		// They are to be re-used throughout the tree (actually making it a DAG)
		//

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
				TraverseSetParent(node,n);
			return true;
		}
		
		public override bool Visit(StatementList node)
		{
			foreach (Node n in node.nodes)
				TraverseSetParent(node,n);
			return true;
		}
		
		public override bool Visit(TypeList node)
		{
			foreach (Node n in node.nodes)
				TraverseSetParent(node,n);
			return true;
		}
		
		public override bool Visit(DeclarationList node)
		{
			foreach (Node n in node.nodes)
				TraverseSetParent(node,n);
			return true;
		}
		
		public override bool Visit(EnumValueList node)
		{
			foreach (Node n in node.nodes)
				TraverseSetParent(node,n);
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
			TraverseSetParent(node,node.section);
			return true;
		}
		
		public override bool Visit(LibraryNode node)
		{
			Visit((TranslationUnit) node);
			TraverseSetParent(node, node.section);
			return true;
		}

		public override bool Visit(ProgramSection node)
		{
			Visit((TopLevelDeclarationSection)node);
			TraverseSetParent(node, node.block);
			return true;
		}

		public override bool Visit(ParametersSection node)
		{
			Visit((Section)node);
			TraverseSetParent(node, node.returnVar);
			return true;
		}
		
		public override bool Visit(UnitNode node)
		{
			Visit((TranslationUnit) node);
			TraverseSetParent(node,node.@interface);
			TraverseSetParent(node,node.implementation);
			TraverseSetParent(node,node.initialization);
			TraverseSetParent(node,node.finalization);
			return true;
		}
		
		public override bool Visit(PackageNode node)
		{
			Visit((TranslationUnit) node);
			TraverseSetParent(node,node.requires);
			TraverseSetParent(node,node.contains);
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
			TraverseSetParent(node,node.formalparams);
			return true;
		}
		
		public override bool Visit(Section node)
		{
			Visit((Node) node);
			TraverseSetParent(node,node.decls);
			return true;
		}
		
		public override bool Visit(RoutineSection node)
		{
			Visit((Section) node);
			TraverseSetParent(node, node.block);
			return true;
		}
				
		public override bool Visit(TopLevelDeclarationSection node)
		{
			TraverseSetParent(node, node.uses);
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
			TraverseSetParent(node,node.type);
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
			TraverseSetParent(node,node.init);
			return true;
		}
		
		public override bool Visit(ParamDeclaration node)
		{
			Visit((ValueDeclaration) node);
			TraverseSetParent(node,node.init);
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
			TraverseSetParent(node,node.init);
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
			TraverseSetParent(node, node.@params);
			TraverseSetParent(node, node.funcret);
			TraverseSetParent(node, node.Directives);
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
			TraverseSetParent(node,node.Type);
			TraverseSetParent(node,node.Directives);
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
			TraverseSetParent(node,node.body);
			return true;
		}

		public override bool Visit(MethodDefinition node)
		{
			Visit((MethodDeclaration)node);
			TraverseSetParent(node,node.body);
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
			TraverseSetParent(node,node.Type);
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
			TraverseSetParent(node,node.section);
			return true;
		}
		
		public override bool Visit(ClassType node)
		{
			Visit((CompositeType) node);
			TraverseSetParent(node,node.self);
			return true;
		}
		
		public override bool Visit(InterfaceType node)
		{
			Visit((CompositeType) node);
			TraverseSetParent(node,node.guid);
			return true;
		}

		public override bool Visit(ClassRefType node)
		{
			//	Do not traverse this node! circular dependency
			//	traverse(node.reftype);
			return true;
		}

		public override bool Visit(RecordRefType node)
		{
			//	Do not traverse this node! circular dependency
			//	traverse(node.reftype);
			return true;
		}

		public override bool Visit(ObjectSection node)
		{
			Visit((Section) node);
			TraverseSetParent(node,node.fields);
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
			TraverseSetParent(node,node.varfields);
			return true;
		}
		
		public override bool Visit(VarEntryDeclaration node)
		{
			Visit((RecordFieldDeclaration) node);
			TraverseSetParent(node,node.tagvalue);
			TraverseSetParent(node,node.fields);
			return true;
		}
		
		public override bool Visit(PropertyDeclaration node)
		{
			Visit((FieldDeclaration) node);
			TraverseSetParent(node,node.specifiers);
			return true;
		}
		
		public override bool Visit(ArrayProperty node)
		{
			Visit((PropertyDeclaration) node);
			TraverseSetParent(node,node.indexes);
			return true;
		}
		
		public override bool Visit(PropertySpecifiers node)
		{
			Visit((Node) node);
			TraverseSetParent(node,node.index);
			TraverseSetParent(node,node.stored);
			TraverseSetParent(node,node.@default);
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
			TraverseSetParent(node,node.stmt);
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
			TraverseSetParent(node,node.lvalue);
			TraverseSetParent(node,node.expr);
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
			TraverseSetParent(node,node.condition);
			TraverseSetParent(node,node.thenblock);
			TraverseSetParent(node,node.elseblock);
			return true;
		}
		
		public override bool Visit(ExpressionStatement node)
		{
			Visit((Statement) node);
			TraverseSetParent(node,node.expr);
			return true;
		}
		
		public override bool Visit(CaseSelector node)
		{
			Visit((Statement) node);
			TraverseSetParent(node,node.list);
			TraverseSetParent(node,node.stmt);
			return true;
		}
		
		public override bool Visit(CaseStatement node)
		{
			Visit((Statement) node);
			TraverseSetParent(node,node.condition);
			TraverseSetParent(node,node.selectors);
			TraverseSetParent(node,node.caseelse);
			return true;
		}
		
		public override bool Visit(LoopStatement node)
		{
			Visit((Statement) node);
			TraverseSetParent(node,node.condition);
			TraverseSetParent(node,node.block);
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
			TraverseSetParent(node,node.var);
			TraverseSetParent(node,node.start);
			TraverseSetParent(node,node.end);
			return true;
		}
		
		public override bool Visit(BlockStatement node)
		{
			Visit((Statement) node);
			TraverseSetParent(node,node.stmts);
			return true;
		}
		
		public override bool Visit(WithStatement node)
		{
			Visit((Statement) node);
			TraverseSetParent(node,node.with);
			TraverseSetParent(node,node.body);
			return true;
		}
		
		public override bool Visit(TryFinallyStatement node)
		{
			Visit((Statement) node);
			TraverseSetParent(node,node.body);
			TraverseSetParent(node,node.final);
			return true;
		}
		
		public override bool Visit(TryExceptStatement node)
		{
			Visit((Statement) node);
			TraverseSetParent(node,node.body);
			TraverseSetParent(node,node.final);
			return true;
		}
		
		public override bool Visit(ExceptionBlock node)
		{
			Visit((Statement) node);
			TraverseSetParent(node,node.onList);
			TraverseSetParent(node,node.@default);
			return true;
		}
		
		public override bool Visit(RaiseStatement node)
		{
			Visit((Statement) node);
			TraverseSetParent(node,node.lvalue);
			TraverseSetParent(node,node.expr);
			return true;
		}
		
		public override bool Visit(OnStatement node)
		{
			Visit((Statement) node);
			TraverseSetParent(node,node.body);
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
			TraverseSetParent(node,node.Type);
			TraverseSetParent(node,node.Value);
			TraverseSetParent(node,node.ForcedType);
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
				TraverseSetParent(node,n);
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
			TraverseSetParent(node,node.exprlist);
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
				TraverseSetParent(node,n);
			return true;
		}
		
		public override bool Visit(FieldInit node)
		{
			Visit((ConstExpression) node);
			TraverseSetParent(node,node.expr);
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
			TraverseSetParent(node,node.expr);
			TraverseSetParent(node,node.set);
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
			TraverseSetParent(node,node.left);
			TraverseSetParent(node,node.right);
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
			TraverseSetParent(node,node.left);
			TraverseSetParent(node,node.right);
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
			TraverseSetParent(node,node.expr);
			TraverseSetParent(node,node.types);
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
			TraverseSetParent(node,node.expr);
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
			TraverseSetParent(node,node.setelems);
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
			TraverseSetParent(node,node.expr);
			return true;
		}
		
		public override bool Visit(StaticCast node)
		{
			Visit((LvalueExpression) node);
			TraverseSetParent(node,node.casttype);
			TraverseSetParent(node,node.expr);
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
			TraverseSetParent(node,node.func);
			TraverseSetParent(node,node.args);
			return true;
		}
		
		public override bool Visit(ArrayAccess node)
		{
			Visit((LvalueExpression) node);
			TraverseSetParent(node,node.lvalue);
			TraverseSetParent(node,node.acessors);
			TraverseSetParent(node,node.array);
			return true;
		}
		
		public override bool Visit(PointerDereference node)
		{
			Visit((LvalueExpression) node);
			TraverseSetParent(node,node.expr);
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
			TraverseSetParent(node,node.func);
			TraverseSetParent(node,node.args);
			return true;
		}
		
		public override bool Visit(ObjectAccess node)
		{
			Visit((LvalueExpression) node);
			TraverseSetParent(node,node.obj);
			return true;
		}
		
		public override bool Visit(Identifier node)
		{
			Visit((LvalueExpression) node);
			return true;
		}

		public override bool Visit(IdentifierStatic node)
		{
			Visit((LvalueExpression)node);
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
			traverse(node.baseType);
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
			traverse(node.min);
			traverse(node.max);
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
			traverse(node.expr);
			return true;
		}
		
		public override bool Visit(VariantType node)
		{
			Visit((VariableType) node);
			traverse(node.actualtype);
			return true;
		}
		
		public override bool Visit(PointerType node)
		{
			Visit((ScalarType) node);
			traverse(node.pointedType);
			return true;
		}
		
		public override bool Visit(StructuredType node)
		{
			Visit((VariableType) node);
			traverse(node.basetype);
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
