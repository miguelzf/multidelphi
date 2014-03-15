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

		public override void Visit(CompositeDeclaration node)
		{
		}
				
		public override void Visit(ClassBody node)
		{
			traverse(node.fields);
			traverse(node.content);
		}
				
		public override void Visit(ClassDefinition node)
		{
			traverse(node.ClassBody);
		}
				
		public override void Visit(InterfaceDefinition node)
		{
			traverse(node.methods);
			traverse(node.properties);
		}
				
		public override void Visit(ClassContent node)
		{
		}
				
		public override void Visit(ClassMethod node)
		{
			traverse(node.decl);
		}
				
		public override void Visit(ClassProperty node)
		{
			traverse(node.type);
			traverse(node.index);
			traverse(node.specs);
			traverse(node.def);
		}
				
		public override void Visit(PropertyReadNode node)
		{
		}
				
		public override void Visit(PropertyWriteNode node)
		{
		}
				
		public override void Visit(PropertySpecifiers node)
		{
			traverse(node.index);
			traverse(node.read);
			traverse(node.write);
			traverse(node.stored);
			traverse(node.def);
			traverse(node.impl);
		}
				
		public override void Visit(PropertySpecifier node)
		{
		}
				
		public override void Visit(PropertyDefault node)
		{
			traverse(node.lit);
		}
				
		public override void Visit(PropertyImplements node)
		{
		}
				
		public override void Visit(PropertyStored node)
		{
		}
				
		public override void Visit(PropertyIndex node)
		{
		}
				
		public override void Visit(Declaration node)
		{
			traverse(node.type);
		}
				
		public override void Visit(LabelDeclaration node)
		{
		}
				
		public override void Visit(VarDeclaration node)
		{
			traverse(node.init);
		}
				
		public override void Visit(ParameterDeclaration node)
		{
		}
				
		public override void Visit(VarParameterDeclaration node)
		{
		}
				
		public override void Visit(ConstParameterDeclaration node)
		{
		}
				
		public override void Visit(OutParameterDeclaration node)
		{
		}
				
		public override void Visit(FieldDeclaration node)
		{
		}
				
		public override void Visit(ConstDeclaration node)
		{
			traverse(node.init);
		}
				
		public override void Visit(EnumValue node)
		{
		}
				
		public override void Visit(TypeDeclaration node)
		{
		}
				
		public override void Visit(CompositeDeclaration node)
		{
		}
				
		public override void Visit(CallableDeclaration node)
		{
		}
				
		public override void Visit(Expression node)
		{
			traverse(node.Type);
		}
				
		public override void Visit(EmptyExpression node)
		{
		}
				
		public override void Visit(ExpressionList node)
		{
		}
				
		public override void Visit(ConstExpression node)
		{
			traverse(node.expr);
		}
				
		public override void Visit(StructuredConstant node)
		{
		}
				
		public override void Visit(ArrayConst node)
		{
		}
				
		public override void Visit(RecordConst node)
		{
		}
				
		public override void Visit(FieldInitList node)
		{
		}
				
		public override void Visit(FieldInit node)
		{
		}
				
		public override void Visit(BinaryExpression node)
		{
		}
				
		public override void Visit(SetIn node)
		{
			traverse(node.expr);
			traverse(node.set);
		}
				
		public override void Visit(SetRange node)
		{
		}
				
		public override void Visit(ArithmethicBinaryExpression node)
		{
			traverse(node.left);
			traverse(node.right);
		}
				
		public override void Visit(Subtraction node)
		{
		}
				
		public override void Visit(Addition node)
		{
		}
				
		public override void Visit(Product node)
		{
		}
				
		public override void Visit(Division node)
		{
		}
				
		public override void Visit(Quotient node)
		{
		}
				
		public override void Visit(Modulus node)
		{
		}
				
		public override void Visit(ShiftRight node)
		{
		}
				
		public override void Visit(ShiftLeft node)
		{
		}
				
		public override void Visit(LogicalBinaryExpression node)
		{
			traverse(node.left);
			traverse(node.right);
		}
				
		public override void Visit(LogicalAnd node)
		{
		}
				
		public override void Visit(LogicalOr node)
		{
		}
				
		public override void Visit(LogicalXor node)
		{
		}
				
		public override void Visit(Equal node)
		{
		}
				
		public override void Visit(NotEqual node)
		{
		}
				
		public override void Visit(LessThan node)
		{
		}
				
		public override void Visit(LessOrEqual node)
		{
		}
				
		public override void Visit(GreaterThan node)
		{
		}
				
		public override void Visit(GreaterOrEqual node)
		{
		}
				
		public override void Visit(TypeBinaryExpression node)
		{
			traverse(node.expr);
			traverse(node.types);
		}
				
		public override void Visit(TypeIs node)
		{
		}
				
		public override void Visit(TypeCast node)
		{
		}
				
		public override void Visit(UnaryExpression node)
		{
		}
				
		public override void Visit(SimpleUnaryExpression node)
		{
			traverse(node.expr);
		}
				
		public override void Visit(UnaryPlus node)
		{
		}
				
		public override void Visit(UnaryMinus node)
		{
		}
				
		public override void Visit(LogicalNot node)
		{
		}
				
		public override void Visit(AddressLvalue node)
		{
		}
				
		public override void Visit(Set node)
		{
			traverse(node.setelems);
		}
				
		public override void Visit(ConstantValue node)
		{
		}
				
		public override void Visit(IntegralValue node)
		{
		}
				
		public override void Visit(StringValue node)
		{
		}
				
		public override void Visit(RealValue node)
		{
		}
				
		public override void Visit(Literal node)
		{
		}
				
		public override void Visit(OrdinalLiteral node)
		{
		}
				
		public override void Visit(IntLiteral node)
		{
		}
				
		public override void Visit(CharLiteral node)
		{
		}
				
		public override void Visit(BoolLiteral node)
		{
		}
				
		public override void Visit(StringLiteral node)
		{
		}
				
		public override void Visit(RealLiteral node)
		{
		}
				
		public override void Visit(PointerLiteral node)
		{
		}
				
		public override void Visit(LvalueExpression node)
		{
		}
				
		public override void Visit(ArrayAccess node)
		{
			traverse(node.lvalue);
			traverse(node.acessors);
			traverse(node.array);
		}
				
		public override void Visit(PointerDereference node)
		{
			traverse(node.expr);
		}
				
		public override void Visit(InheritedCall node)
		{
			traverse(node.call);
		}
				
		public override void Visit(RoutineCall node)
		{
			traverse(node.func);
			traverse(node.args);
			traverse(node.basictype);
		}
				
		public override void Visit(FieldAcess node)
		{
			traverse(node.obj);
		}
				
		public override void Visit(Identifier node)
		{
		}
				
		public override void Visit(Node node)
		{
		}
				
		public override void Visit(FixmeNode node)
		{
		}
				
		public override void Visit(NotSupportedNode node)
		{
		}
				
		public override void Visit(EmptyNode node)
		{
		}
				
		public override void Visit(ListNode node)
		{
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
				
		public override void Visit(Processor node)
		{
		}
				
		public override void Visit(ProceduralType node)
		{
			traverse(node.@params);
			traverse(node.funcret);
			traverse(node.Directives{get);
		}
				
		public override void Visit(MethodType node)
		{
		}
				
		public override void Visit(CallableDeclaration node)
		{
			traverse(node.Type{get);
		}
				
		public override void Visit(RoutineDeclaration node)
		{
		}
				
		public override void Visit(MethodDeclaration node)
		{
		}
				
		public override void Visit(SpecialMethodDeclaration node)
		{
		}
				
		public override void Visit(ConstructorDeclaration node)
		{
		}
				
		public override void Visit(DestructorDeclaration node)
		{
		}
				
		public override void Visit(RoutineDefinition node)
		{
			traverse(node.header);
			traverse(node.body);
		}
				
		public override void Visit(CallableDirectives node)
		{
		}
				
		public override void Visit(RoutineDirectives node)
		{
		}
				
		public override void Visit(MethodDirectives node)
		{
		}
				
		public override void Visit(ExternalDirective node)
		{
			traverse(node.File);
		}
				
		public override void Visit(CompilationUnit node)
		{
		}
				
		public override void Visit(ProgramNode node)
		{
			traverse(node.body);
			traverse(node.uses);
		}
				
		public override void Visit(LibraryNode node)
		{
			traverse(node.body);
			traverse(node.uses);
		}
				
		public override void Visit(UnitNode node)
		{
			traverse(node.interfce);
			traverse(node.implementation);
			traverse(node.init);
		}
				
		public override void Visit(PackageNode node)
		{
			traverse(node.requires);
			traverse(node.contains);
		}
				
		public override void Visit(UnitItem node)
		{
		}
				
		public override void Visit(UsesItem node)
		{
		}
				
		public override void Visit(RequiresItem node)
		{
		}
				
		public override void Visit(ContainsItem node)
		{
		}
				
		public override void Visit(ExportItem node)
		{
		}
				
		public override void Visit(Section node)
		{
			traverse(node.decls);
		}
				
		public override void Visit(CodeSection node)
		{
			traverse(node.block);
		}
				
		public override void Visit(ProgramBody node)
		{
		}
				
		public override void Visit(RoutineBody node)
		{
		}
				
		public override void Visit(InitializationSection node)
		{
		}
				
		public override void Visit(FinalizationSection node)
		{
		}
				
		public override void Visit(DeclarationSection node)
		{
			traverse(node.uses);
		}
				
		public override void Visit(InterfaceSection node)
		{
		}
				
		public override void Visit(ImplementationSection node)
		{
		}
				
		public override void Visit(AssemblerRoutineBody node)
		{
		}
				
		public override void Visit(Statement node)
		{
		}
				
		public override void Visit(LabelStatement node)
		{
			traverse(node.stmt);
		}
				
		public override void Visit(EmptyStatement node)
		{
		}
				
		public override void Visit(BreakStatement node)
		{
		}
				
		public override void Visit(ContinueStatement node)
		{
		}
				
		public override void Visit(Assignement node)
		{
			traverse(node.lvalue);
			traverse(node.expr);
		}
				
		public override void Visit(GotoStatement node)
		{
		}
				
		public override void Visit(IfStatement node)
		{
			traverse(node.condition);
			traverse(node.thenblock);
			traverse(node.elseblock);
		}
				
		public override void Visit(ExpressionStatement node)
		{
			traverse(node.expr);
		}
				
		public override void Visit(CaseSelector node)
		{
			traverse(node.list);
			traverse(node.stmt);
		}
				
		public override void Visit(CaseStatement node)
		{
			traverse(node.condition);
			traverse(node.selectors);
			traverse(node.caseelse);
		}
				
		public override void Visit(LoopStatement node)
		{
			traverse(node.condition);
			traverse(node.block);
		}
				
		public override void Visit(RepeatLoop node)
		{
		}
				
		public override void Visit(WhileLoop node)
		{
		}
				
		public override void Visit(ForLoop node)
		{
			traverse(node.var);
			traverse(node.start);
			traverse(node.end);
		}
				
		public override void Visit(BlockStatement node)
		{
			traverse(node.stmts);
		}
				
		public override void Visit(WithStatement node)
		{
			traverse(node.with);
			traverse(node.body);
		}
				
		public override void Visit(TryFinallyStatement node)
		{
			traverse(node.body);
			traverse(node.final);
		}
				
		public override void Visit(TryExceptStatement node)
		{
			traverse(node.body);
			traverse(node.final);
		}
				
		public override void Visit(ExceptionBlock node)
		{
			traverse(node.onList);
			traverse(node.@default);
		}
				
		public override void Visit(RaiseStatement node)
		{
			traverse(node.lvalue);
			traverse(node.expr);
		}
				
		public override void Visit(OnStatement node)
		{
			traverse(node.body);
		}
				
		public override void Visit(AssemblerBlock node)
		{
		}
				
		public override void Visit(TypeNode node)
		{
		}
				
		public override void Visit(UndefinedType node)
		{
		}
				
		public override void Visit(DeclaredType node)
		{
		}
				
		public override void Visit(RecordType node)
		{
			traverse(node.compTypes);
		}
				
		public override void Visit(ProceduralType node)
		{
		}
				
		public override void Visit(ClassType node)
		{
		}
				
		public override void Visit(VariableType node)
		{
		}
				
		public override void Visit(MetaclassType node)
		{
			traverse(node.baseType);
		}
				
		public override void Visit(TypeUnknown node)
		{
		}
				
		public override void Visit(EnumType node)
		{
			traverse(node.enumVals);
		}
				
		public override void Visit(RangeType node)
		{
			traverse(node.min);
			traverse(node.max);
		}
				
		public override void Visit(ScalarType node)
		{
		}
				
		public override void Visit(StringType node)
		{
		}
				
		public override void Visit(FixedStringType node)
		{
			traverse(node.expr);
		}
				
		public override void Visit(VariantType node)
		{
			traverse(node.type);
		}
				
		public override void Visit(PointerType node)
		{
			traverse(node.pointedType);
		}
				
		public override void Visit(IntegralType node)
		{
		}
				
		public override void Visit(IntegerType node)
		{
		}
				
		public override void Visit(SignedIntegerType node)
		{
		}
				
		public override void Visit(UnsignedIntegerType node)
		{
		}
				
		public override void Visit(UnsignedInt8Type node)
		{
		}
				
		public override void Visit(UnsignedInt16Type node)
		{
		}
				
		public override void Visit(UnsignedInt32Type node)
		{
		}
				
		public override void Visit(UnsignedInt64Type node)
		{
		}
				
		public override void Visit(SignedInt8Type node)
		{
		}
				
		public override void Visit(SignedInt16Type node)
		{
		}
				
		public override void Visit(SignedInt32Type node)
		{
		}
				
		public override void Visit(SignedInt64Type node)
		{
		}
				
		public override void Visit(BoolType node)
		{
		}
				
		public override void Visit(CharType node)
		{
		}
				
		public override void Visit(RealType node)
		{
		}
				
		public override void Visit(FloatType node)
		{
		}
				
		public override void Visit(DoubleType node)
		{
		}
				
		public override void Visit(ExtendedType node)
		{
		}
				
		public override void Visit(CurrencyType node)
		{
		}
				
		public override void Visit(StructuredType node)
		{
			traverse(node.basetype);
		}
				
		public override void Visit(ArrayType node)
		{
		}
				
		public override void Visit(SetType node)
		{
		}
				
		public override void Visit(FileType node)
		{
		}

	}
}
