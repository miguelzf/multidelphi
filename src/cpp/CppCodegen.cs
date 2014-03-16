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

		public override void Visit(CompositeDeclaration node)
		{
			Visit((TypeDeclaration)node);
		}

		public override void Visit(ClassBody node)
		{
			Visit((Section)node);
		}

		public override void Visit(ClassDefinition node)
		{
			Visit((CompositeDeclaration)node);
		}

		public override void Visit(InterfaceDefinition node)
		{
			Visit((CompositeDeclaration)node);
		}

		public override void Visit(ClassContent node)
		{
			Visit((Node)node);
		}

		public override void Visit(ClassMethod node)
		{
			Visit((ClassContent)node);
		}

		public override void Visit(ClassProperty node)
		{
			Visit((ClassContent)node);
		}

		public override void Visit(PropertyReadNode node)
		{
			Visit((Node)node);
		}

		public override void Visit(PropertyWriteNode node)
		{
			Visit((Node)node);
		}

		public override void Visit(PropertySpecifiers node)
		{
			Visit((Node)node);
		}

		public override void Visit(PropertySpecifier node)
		{
			Visit((Node)node);
		}

		public override void Visit(PropertyDefault node)
		{
			Visit((PropertySpecifier)node);
		}

		public override void Visit(PropertyImplements node)
		{
			Visit((PropertySpecifier)node);
		}

		public override void Visit(PropertyStored node)
		{
			Visit((PropertySpecifier)node);
		}

		public override void Visit(PropertyIndex node)
		{
			Visit((PropertySpecifier)node);
		}

		public override void Visit(Declaration node)
		{
			Visit((Node)node);
		}

		public override void Visit(LabelDeclaration node)
		{
			Visit((Declaration)node);
		}

		public override void Visit(VarDeclaration node)
		{
			Visit((Declaration)node);
		}

		public override void Visit(ParameterDeclaration node)
		{
			Visit((VarDeclaration)node);
		}

		public override void Visit(VarParameterDeclaration node)
		{
			Visit((ParameterDeclaration)node);
		}

		public override void Visit(ConstParameterDeclaration node)
		{
			Visit((ParameterDeclaration)node);
		}

		public override void Visit(OutParameterDeclaration node)
		{
			Visit((ParameterDeclaration)node);
		}

		public override void Visit(FieldDeclaration node)
		{
			Visit((Declaration)node);
		}

		public override void Visit(ConstDeclaration node)
		{
			Visit((Declaration)node);
		}

		public override void Visit(EnumValue node)
		{
			Visit((ConstDeclaration)node);
		}

		public override void Visit(TypeDeclaration node)
		{
			Visit((Declaration)node);
		}

		public override void Visit(CallableDeclaration node)
		{
			Visit((TypeDeclaration)node);
		}

		public override void Visit(Expression node)
		{
			Visit((Node)node);
		}

		public override void Visit(EmptyExpression node)
		{
			Visit((Expression)node);
		}

		public override void Visit(ExpressionList node)
		{
			Visit((Expression)node);
		}

		public override void Visit(ConstExpression node)
		{
			Visit((Expression)node);
		}

		public override void Visit(StructuredConstant node)
		{
			Visit((ConstExpression)node);
		}

		public override void Visit(ArrayConst node)
		{
			Visit((StructuredConstant)node);
		}

		public override void Visit(RecordConst node)
		{
			Visit((StructuredConstant)node);
		}

		public override void Visit(FieldInitList node)
		{
			Visit((ExpressionList)node);
		}

		public override void Visit(FieldInit node)
		{
			Visit((ConstExpression)node);
		}

		public override void Visit(BinaryExpression node)
		{
			Visit((Expression)node);
		}

		public override void Visit(SetIn node)
		{
			Visit((BinaryExpression)node);
		}

		public override void Visit(SetRange node)
		{
			Visit((BinaryExpression)node);
		}

		public override void Visit(ArithmethicBinaryExpression node)
		{
			Visit((BinaryExpression)node);
		}

		public override void Visit(Subtraction node)
		{
			Visit((ArithmethicBinaryExpression)node);
		}

		public override void Visit(Addition node)
		{
			Visit((ArithmethicBinaryExpression)node);
		}

		public override void Visit(Product node)
		{
			Visit((ArithmethicBinaryExpression)node);
		}

		public override void Visit(Division node)
		{
			Visit((ArithmethicBinaryExpression)node);
		}

		public override void Visit(Quotient node)
		{
			Visit((ArithmethicBinaryExpression)node);
		}

		public override void Visit(Modulus node)
		{
			Visit((ArithmethicBinaryExpression)node);
		}

		public override void Visit(ShiftRight node)
		{
			Visit((ArithmethicBinaryExpression)node);
		}

		public override void Visit(ShiftLeft node)
		{
			Visit((ArithmethicBinaryExpression)node);
		}

		public override void Visit(LogicalBinaryExpression node)
		{
			Visit((BinaryExpression)node);
		}

		public override void Visit(LogicalAnd node)
		{
			Visit((LogicalBinaryExpression)node);
		}

		public override void Visit(LogicalOr node)
		{
			Visit((LogicalBinaryExpression)node);
		}

		public override void Visit(LogicalXor node)
		{
			Visit((LogicalBinaryExpression)node);
		}

		public override void Visit(Equal node)
		{
			Visit((LogicalBinaryExpression)node);
		}

		public override void Visit(NotEqual node)
		{
			Visit((LogicalBinaryExpression)node);
		}

		public override void Visit(LessThan node)
		{
			Visit((LogicalBinaryExpression)node);
		}

		public override void Visit(LessOrEqual node)
		{
			Visit((LogicalBinaryExpression)node);
		}

		public override void Visit(GreaterThan node)
		{
			Visit((LogicalBinaryExpression)node);
		}

		public override void Visit(GreaterOrEqual node)
		{
			Visit((LogicalBinaryExpression)node);
		}

		public override void Visit(TypeBinaryExpression node)
		{
			Visit((BinaryExpression)node);
		}

		public override void Visit(TypeIs node)
		{
			Visit((TypeBinaryExpression)node);
		}

		public override void Visit(TypeCast node)
		{
			Visit((TypeBinaryExpression)node);
		}

		public override void Visit(UnaryExpression node)
		{
			Visit((Expression)node);
		}

		public override void Visit(SimpleUnaryExpression node)
		{
			Visit((Expression)node);
		}

		public override void Visit(UnaryPlus node)
		{
			Visit((SimpleUnaryExpression)node);
		}

		public override void Visit(UnaryMinus node)
		{
			Visit((SimpleUnaryExpression)node);
		}

		public override void Visit(LogicalNot node)
		{
			Visit((SimpleUnaryExpression)node);
		}

		public override void Visit(AddressLvalue node)
		{
			Visit((SimpleUnaryExpression)node);
		}

		public override void Visit(Set node)
		{
			Visit((UnaryExpression)node);
		}

		public override void Visit(ConstantValue node)
		{
			Visit((Node)node);
		}

		public override void Visit(IntegralValue node)
		{
			Visit((ConstantValue)node);
		}

		public override void Visit(StringValue node)
		{
			Visit((ConstantValue)node);
		}

		public override void Visit(RealValue node)
		{
			Visit((ConstantValue)node);
		}

		public override void Visit(Literal node)
		{
			Visit((UnaryExpression)node);
		}

		public override void Visit(OrdinalLiteral node)
		{
			Visit((Literal)node);
		}

		public override void Visit(IntLiteral node)
		{
			Visit((OrdinalLiteral)node);
		}

		public override void Visit(CharLiteral node)
		{
			Visit((OrdinalLiteral)node);
		}

		public override void Visit(BoolLiteral node)
		{
			Visit((OrdinalLiteral)node);
		}

		public override void Visit(StringLiteral node)
		{
			Visit((Literal)node);
		}

		public override void Visit(RealLiteral node)
		{
			Visit((Literal)node);
		}

		public override void Visit(PointerLiteral node)
		{
			Visit((Literal)node);
		}

		public override void Visit(LvalueExpression node)
		{
			Visit((UnaryExpression)node);
		}

		public override void Visit(ArrayAccess node)
		{
			Visit((LvalueExpression)node);
		}

		public override void Visit(PointerDereference node)
		{
			Visit((LvalueExpression)node);
		}

		public override void Visit(InheritedCall node)
		{
			Visit((LvalueExpression)node);
		}

		public override void Visit(RoutineCall node)
		{
			Visit((LvalueExpression)node);
		}

		public override void Visit(FieldAcess node)
		{
			Visit((LvalueExpression)node);
		}

		public override void Visit(Identifier node)
		{
			Visit((LvalueExpression)node);
		}

		public override void Visit(Node node)
		{
		}

		public override void Visit(FixmeNode node)
		{
			Visit((Node)node);
		}

		public override void Visit(NotSupportedNode node)
		{
			Visit((Node)node);
		}

		public override void Visit(EmptyNode node)
		{
			Visit((Node)node);
		}

		public override void Visit(NodeList node)
		{
		}

		public override void Visit(StatementList node)
		{
		}

		public override void Visit(TypeList node)
		{
		}

		public override void Visit(IntegralTypeList node)
		{
		}

		public override void Visit(IdentifierList node)
		{
		}

		public override void Visit(DeclarationList node)
		{
		}

		public override void Visit(EnumValueList node)
		{
		}

		public override void Visit(ParameterList node)
		{
		}

		public override void Visit(FieldList node)
		{
		}

		public override void Visit(ProceduralType node)
		{
			Visit((TypeNode)node);
		}

		public override void Visit(MethodType node)
		{
			Visit((ProceduralType)node);
		}

		public override void Visit(RoutineDeclaration node)
		{
			Visit((CallableDeclaration)node);
		}

		public override void Visit(MethodDeclaration node)
		{
			Visit((CallableDeclaration)node);
		}

		public override void Visit(SpecialMethodDeclaration node)
		{
			Visit((MethodDeclaration)node);
		}

		public override void Visit(ConstructorDeclaration node)
		{
			Visit((SpecialMethodDeclaration)node);
		}

		public override void Visit(DestructorDeclaration node)
		{
			Visit((SpecialMethodDeclaration)node);
		}

		public override void Visit(RoutineDefinition node)
		{
			Visit((Declaration)node);
		}

		public override void Visit(CallableDirectives node)
		{
			Visit((Node)node);
		}

		public override void Visit(RoutineDirectives node)
		{
			Visit((CallableDirectives)node);
		}

		public override void Visit(MethodDirectives node)
		{
			Visit((CallableDirectives)node);
		}

		public override void Visit(ExternalDirective node)
		{
		}

		public override void Visit(CompilationUnit node)
		{
			Visit((Node)node);
		}

		public override void Visit(ProgramNode node)
		{
			Visit((CompilationUnit)node);
		}

		public override void Visit(LibraryNode node)
		{
			Visit((CompilationUnit)node);
		}

		public override void Visit(UnitNode node)
		{
			Visit((CompilationUnit)node);
		}

		public override void Visit(PackageNode node)
		{
			Visit((CompilationUnit)node);
		}

		public override void Visit(UnitItem node)
		{
			Visit((Node)node);
		}

		public override void Visit(UsesItem node)
		{
			Visit((UnitItem)node);
		}

		public override void Visit(RequiresItem node)
		{
			Visit((UnitItem)node);
		}

		public override void Visit(ContainsItem node)
		{
			Visit((UnitItem)node);
		}

		public override void Visit(ExportItem node)
		{
			Visit((UnitItem)node);
		}

		public override void Visit(Section node)
		{
			Visit((Node)node);
		}

		public override void Visit(CodeSection node)
		{
			Visit((Section)node);
		}

		public override void Visit(ProgramBody node)
		{
			Visit((CodeSection)node);
		}

		public override void Visit(RoutineBody node)
		{
			Visit((CodeSection)node);
		}

		public override void Visit(InitializationSection node)
		{
			Visit((CodeSection)node);
		}

		public override void Visit(FinalizationSection node)
		{
			Visit((CodeSection)node);
		}

		public override void Visit(DeclarationSection node)
		{
			Visit((Section)node);
		}

		public override void Visit(InterfaceSection node)
		{
			Visit((DeclarationSection)node);
		}

		public override void Visit(ImplementationSection node)
		{
			Visit((DeclarationSection)node);
		}

		public override void Visit(AssemblerRoutineBody node)
		{
			Visit((RoutineBody)node);
		}

		public override void Visit(Statement node)
		{
			Visit((Node)node);
		}

		public override void Visit(LabelStatement node)
		{
			Visit((Statement)node);
		}

		public override void Visit(EmptyStatement node)
		{
			Visit((Statement)node);
		}

		public override void Visit(BreakStatement node)
		{
			Visit((Statement)node);
		}

		public override void Visit(ContinueStatement node)
		{
			Visit((Statement)node);
		}

		public override void Visit(Assignement node)
		{
			Visit((Statement)node);
		}

		public override void Visit(GotoStatement node)
		{
			Visit((Statement)node);
		}

		public override void Visit(IfStatement node)
		{
			Visit((Statement)node);
		}

		public override void Visit(ExpressionStatement node)
		{
			Visit((Statement)node);
		}

		public override void Visit(CaseSelector node)
		{
			Visit((Statement)node);
		}

		public override void Visit(CaseStatement node)
		{
			Visit((Statement)node);
		}

		public override void Visit(LoopStatement node)
		{
			Visit((Statement)node);
		}

		public override void Visit(RepeatLoop node)
		{
			Visit((LoopStatement)node);
		}

		public override void Visit(WhileLoop node)
		{
			Visit((LoopStatement)node);
		}

		public override void Visit(ForLoop node)
		{
			Visit((LoopStatement)node);
		}

		public override void Visit(BlockStatement node)
		{
			outputCode("{", true, true);
			ident++;
			Visit((Statement)node);
			ident--;
			outputCode("}", true, true);
		}

		public override void Visit(WithStatement node)
		{
		}

		public override void Visit(TryFinallyStatement node)
		{
			Visit((Statement)node);
		}

		public override void Visit(TryExceptStatement node)
		{
			Visit((Statement)node);
		}

		public override void Visit(ExceptionBlock node)
		{
			Visit((Statement)node);
		}

		public override void Visit(RaiseStatement node)
		{
			Visit((Statement)node);
		}

		public override void Visit(OnStatement node)
		{
			Visit((Statement)node);
		}

		public override void Visit(AssemblerBlock node)
		{
			Visit((BlockStatement)node);
		}

		public override void Visit(TypeNode node)
		{
			Visit((Node)node);
		}

		public override void Visit(UndefinedType node)
		{
			Visit((TypeNode)node);
		}

		public override void Visit(CompositeType node)
		{
			Visit((TypeNode)node);
		}

		public override void Visit(ClassType node)
		{
			Visit((CompositeType)node);
		}

		public override void Visit(InterfaceType node)
		{
			Visit((CompositeType)node);
		}

		public override void Visit(VariableType node)
		{
			Visit((TypeNode)node);
		}

		public override void Visit(MetaclassType node)
		{
			Visit((VariableType)node);
		}

		public override void Visit(TypeUnknown node)
		{
			Visit((TypeNode)node);
		}

		public override void Visit(EnumType node)
		{
			Visit((VariableType)node);
		}

		public override void Visit(RangeType node)
		{
			Visit((VariableType)node);
		}

		public override void Visit(ScalarType node)
		{
			Visit((VariableType)node);
		}

		public override void Visit(ScalarTypeForward node)
		{
			Visit((ScalarType)node);
		}

		public override void Visit(StringType node)
		{
			Visit((ScalarType)node);
		}

		public override void Visit(FixedStringType node)
		{
			Visit((ScalarType)node);
		}

		public override void Visit(VariantType node)
		{
			Visit((ScalarType)node);
		}

		public override void Visit(PointerType node)
		{
			Visit((ScalarType)node);
		}

		public override void Visit(IntegralType node)
		{
			Visit((ScalarType)node);
		}

		public override void Visit(IntegerType node)
		{
			Visit((IntegralType)node);
		}

		public override void Visit(SignedIntegerType node)
		{
			Visit((IntegerType)node);
		}

		public override void Visit(UnsignedIntegerType node)
		{
			Visit((IntegerType)node);
		}

		public override void Visit(UnsignedInt8Type node)
		{
			Visit((UnsignedIntegerType)node);
		}

		public override void Visit(UnsignedInt16Type node)
		{
			Visit((UnsignedIntegerType)node);
		}

		public override void Visit(UnsignedInt32Type node)
		{
			Visit((UnsignedIntegerType)node);
		}

		public override void Visit(UnsignedInt64Type node)
		{
			Visit((UnsignedIntegerType)node);
		}

		public override void Visit(SignedInt8Type node)
		{
			Visit((SignedIntegerType)node);
		}

		public override void Visit(SignedInt16Type node)
		{
			Visit((SignedIntegerType)node);
		}

		public override void Visit(SignedInt32Type node)
		{
			Visit((SignedIntegerType)node);
		}

		public override void Visit(SignedInt64Type node)
		{
			Visit((IntegerType)node);
		}

		public override void Visit(BoolType node)
		{
			Visit((IntegralType)node);
		}

		public override void Visit(CharType node)
		{
			Visit((IntegralType)node);
		}

		public override void Visit(RealType node)
		{
			Visit((ScalarType)node);
		}

		public override void Visit(FloatType node)
		{
			Visit((RealType)node);
		}

		public override void Visit(DoubleType node)
		{
			Visit((RealType)node);
		}

		public override void Visit(ExtendedType node)
		{
			Visit((RealType)node);
		}

		public override void Visit(CurrencyType node)
		{
			Visit((RealType)node);
		}

		public override void Visit(StructuredType node)
		{
			Visit((VariableType)node);
		}

		public override void Visit(ArrayType node)
		{
			Visit((StructuredType)node);
		}

		public override void Visit(SetType node)
		{
			Visit((StructuredType)node);
		}

		public override void Visit(FileType node)
		{
			Visit((StructuredType)node);
		}

		public override void Visit(RecordType node)
		{
			Visit((StructuredType)node);
		}
	}
}
