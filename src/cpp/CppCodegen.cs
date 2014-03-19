using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using crosspascal.ast;
using crosspascal.ast.nodes;

namespace crosspascal.cpp
{
	class CppCodegen : Processor
	{
		private int ident = 0;

		public void outputCode(string s, bool hasident, bool newline)
		{
			if (hasident)
				for(int i=0; i<ident; i++)
					Console.Write("	");
			Console.Write(s);
			if (newline)
				Console.WriteLine("");
		}


        public void PushIdent()
        {
            ident++;
        }

        public void PopIdent()
        {
            ident--;
        }

		public override bool Visit(Node node)
		{
			return true;
		}
		
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
            foreach (Node n in node)
                traverse(n); 

			return true;
		}
		
		public override bool Visit(StatementList node)
		{
            foreach (Node n in node)
                traverse(n); 
            return true;
		}
		
		public override bool Visit(TypeList node)
		{
            foreach (Node n in node)
                traverse(n);
            return true;
		}
		
		public override bool Visit(IntegralTypeList node)
		{
            foreach (Node n in node)
                traverse(n);
            return true;
		}
		
		public override bool Visit(IdentifierList node)
		{
            foreach (Node n in node)
                traverse(n);
            return true;
		}
		
		public override bool Visit(DeclarationList node)
		{
            foreach (Node n in node)
                traverse(n);
            return true;
		}
		
		public override bool Visit(EnumValueList node)
		{
            foreach (Node n in node)
                traverse(n);
            return true;
		}
				
		public override bool Visit(CompilationUnit node)
		{
			Visit((Node) node);
			return true;
		}
		
		public override bool Visit(ProgramNode node)
		{
			Visit((CompilationUnit) node);
			traverse(node.body);
			traverse(node.uses);
			return true;
		}
		
		public override bool Visit(LibraryNode node)
		{
			Visit((CompilationUnit) node);
			traverse(node.body);
			traverse(node.uses);
			return true;
		}
		
		public override bool Visit(UnitNode node)
		{
			Visit((CompilationUnit) node);
			traverse(node.interfce);
			traverse(node.implementation);
			traverse(node.init);
			return true;
		}
		
		public override bool Visit(PackageNode node)
		{
			Visit((CompilationUnit) node);
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
			Visit((Section) node);
			traverse(node.uses);
			return true;
		}
		
		public override bool Visit(InterfaceSection node)
		{
			Visit((DeclarationSection) node);
			return true;
		}
		
		public override bool Visit(ImplementationSection node)
		{
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
			traverse(node.type);
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
            traverse(node.type);
            int i = 0;
            string name = node.name;
            outputCode(name, false, false);
            i++;

			traverse(node.init);
            outputCode(";", false, true);
            return true;
        }

		public override bool Visit(ParamDeclaration node)
		{
			Visit((ValueDeclaration) node);
			traverse(node.init);
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
		
		public override bool Visit(FieldDeclaration node)
		{
			Visit((ValueDeclaration) node);
			return true;
		}
		
		public override bool Visit(ConstDeclaration node)
		{
            string name = node.name as string;
            outputCode("#define "+name+" ", false, false);			
			traverse(node.init);
            outputCode("", false, true);			
			return true;
		}
		
		public override bool Visit(TypeDeclaration node)
		{
			Visit((Declaration) node);
			return true;
		}
		
		public override bool Visit(CallableDeclaration node)
		{
			Visit((TypeDeclaration) node);
			traverse(node.Type);
            string name = node.name as string;
            outputCode(name, false, false);
            outputCode("(", false, false);
            ProceduralType pp = node.type as ProceduralType;
            DeclarationList p = pp.@params;
            foreach (ParamDeclaration pd in p)
            {
                traverse(pd);
            }
            outputCode(")", false, false);
			traverse(node.Directives);
			return true;
		}

		public override bool Visit(ProceduralType node)
		{
			Visit((TypeNode) node);
			return true;
		}

		public override bool Visit(MethodType node)
		{
			Visit((ProceduralType) node);
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

		public override bool Visit(RoutineDefinition node)
		{
			Visit((Declaration) node);
			traverse(node.header);
			traverse(node.body);
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
			traverse(node.Type);
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

		public override bool Visit(VariantDeclaration node)
		{
			Visit((FieldDeclaration) node);
			traverse(node.varfields);
			return true;
		}

		public override bool Visit(VarEntryDeclaration node)
		{
			Visit((FieldDeclaration) node);
			traverse(node.tagvalue);
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
			traverse(node.index);
			traverse(node.stored);
			traverse(node.@default);
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

		public override bool Visit(Assignement node)
		{
			Visit((Statement) node);
			traverse(node.lvalue);
			traverse(node.expr);
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
			traverse(node.condition);
			traverse(node.thenblock);
			traverse(node.elseblock);
			return true;
		}

		public override bool Visit(ExpressionStatement node)
		{
			Visit((Statement) node);
			traverse(node.expr);
			return true;
		}

		public override bool Visit(CaseSelector node)
		{
			Visit((Statement) node);
			traverse(node.list);
			traverse(node.stmt);
			return true;
		}

		public override bool Visit(CaseStatement node)
		{
			Visit((Statement) node);
			traverse(node.condition);
			traverse(node.selectors);
			traverse(node.caseelse);
			return true;
		}

		public override bool Visit(LoopStatement node)
		{
			Visit((Statement) node);
			traverse(node.condition);
			traverse(node.block);
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
			traverse(node.var);
			traverse(node.start);
			traverse(node.end);
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
			traverse(node.with);
			traverse(node.body);
			return true;
		}

		public override bool Visit(TryFinallyStatement node)
		{
			Visit((Statement) node);
			traverse(node.body);
			traverse(node.final);
			return true;
		}

		public override bool Visit(TryExceptStatement node)
		{
			Visit((Statement) node);
			traverse(node.body);
			traverse(node.final);
			return true;
		}

		public override bool Visit(ExceptionBlock node)
		{
			Visit((Statement) node);
			traverse(node.onList);
			traverse(node.@default);
			return true;
		}

		public override bool Visit(RaiseStatement node)
		{
			Visit((Statement) node);
			traverse(node.lvalue);
			traverse(node.expr);
			return true;
		}

		public override bool Visit(OnStatement node)
		{
			Visit((Statement) node);
			traverse(node.body);
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
			traverse(node.Type);
			traverse(node.Value);
			traverse(node.ForcedType);
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
			traverse(node.expr);
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
			traverse(node.expr);
			traverse(node.set);
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
			traverse(node.left);
			traverse(node.right);
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
			traverse(node.left);
			traverse(node.right);
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
			traverse(node.expr);
			traverse(node.types);
			return true;
		}

		public override bool Visit(TypeIs node)
		{
			Visit((TypeBinaryExpression) node);
			return true;
		}

		public override bool Visit(TypeCast node)
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
			traverse(node.expr);
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

		public override bool Visit(Literal node)
		{
			Visit((UnaryExpression) node);
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

		public override bool Visit(LvalueExpression node)
		{
			Visit((UnaryExpression) node);
			return true;
		}

		public override bool Visit(ArrayAccess node)
		{
			Visit((LvalueExpression) node);
			traverse(node.lvalue);
			traverse(node.acessors);
			traverse(node.array);
			return true;
		}

		public override bool Visit(PointerDereference node)
		{
			Visit((LvalueExpression) node);
			traverse(node.expr);
			return true;
		}

		public override bool Visit(InheritedCall node)
		{
			Visit((LvalueExpression) node);
			traverse(node.call);
			return true;
		}

		public override bool Visit(RoutineCall node)
		{
			Visit((LvalueExpression) node);
			traverse(node.func);
			traverse(node.args);
			traverse(node.basictype);
			return true;
		}

		public override bool Visit(FieldAcess node)
		{
			Visit((LvalueExpression) node);
			traverse(node.obj);
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

		public override bool Visit(UndefinedType node)
		{
			Visit((TypeNode) node);
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

		public override bool Visit(TypeUnknown node)
		{
			Visit((TypeNode) node);
			return true;
		}

		public override bool Visit(EnumType node)
		{
			Visit((VariableType) node);
			traverse(node.enumVals);
			return true;
		}

		public override bool Visit(EnumValue node)
		{
			Visit((ConstDeclaration) node);
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

		public override bool Visit(ScalarTypeForward node)
		{
			Visit((ScalarType) node);
			return true;
		}

		public override bool Visit(StringType node)
		{
			Visit((ScalarType) node);
			return true;
		}

		public override bool Visit(FixedStringType node)
		{
			Visit((ScalarType) node);
			traverse(node.expr);
			return true;
		}

		public override bool Visit(VariantType node)
		{
			Visit((ScalarType) node);
			traverse(node.actualtype);
			return true;
		}

		public override bool Visit(PointerType node)
		{
			Visit((ScalarType) node);
			traverse(node.pointedType);
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


	}
}
