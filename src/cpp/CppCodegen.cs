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
		private StringBuilder builder;

		public CppCodegen()
		{
			builder = new StringBuilder();
		}

		public override string ToString()
		{
			return builder.ToString();
		}

		public void outputCode(string s, bool hasident, bool newline)
		{
			if (hasident)
				for(int i=0; i<ident; i++)
					builder.Append("	");
			builder.Append(s);
			if (newline)
				builder.AppendLine();
		}


        void PushIdent()
        {
            ident++;
        }

        void PopIdent()
        {
            ident--;
        }


		//
		// Processor interface
		//

		public override bool StartProcessing(Node n)
		{
			return traverse(n);
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
			Visit((Node) node);
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
			Visit((CodeSection) node);
			return true;
		}
		
		public override bool Visit(InitializationSection node)
		{
			traverse(node.decls);
			outputCode((node.Parent as UnitNode).name+"_init()", false, true);
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
			Visit((Declaration) node);
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
            outputCode(")", false, true);
			traverse(node.Directives);
			return true;
		}

		public override bool Visit(ProceduralType node)
		{
			traverse(node.funcret);
			outputCode("(*", false, false);
			traverse(node.@params);
			outputCode(")", false, false);
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
			outputCode(node.objname + "::" + node.metname+"(", false, false);
			traverse((node.type as ProceduralType).@params);
			outputCode(")", false, true);

			return true;
		}

		public override bool Visit(SpecialMethodDeclaration node)
		{
			Visit((MethodDeclaration) node);
			return true;
		}

		public override bool Visit(ConstructorDeclaration node)
		{
			outputCode(node.objname + "::" + node.objname+"(", false, false);
			traverse((node.type as ProceduralType).@params);
			outputCode(")", false, true);
			return true;
		}

		public override bool Visit(DestructorDeclaration node)
		{
			outputCode(node.objname + "::~" + node.objname + "(", false, false);
			traverse((node.type as ProceduralType).@params);
			outputCode(")", false, true);
			return true;
		}

		public override bool Visit(CallableDefinition node)
		{
			Visit((Declaration) node);
			traverse(node.header);
			traverse(node.body);

			if (node.header.IsFunction)
			{
				builder.Remove(builder.Length - 3, 3);
				outputCode("	return result;", true, true);
				outputCode("}", true, true);
			}

			outputCode("", false, true);
			return true;
		}

		public override bool Visit(RoutineDefinition node)
		{
			Visit((CallableDefinition)node);
			return true;
		}

		public override bool Visit(MethodDefinition node)
		{
			Visit((CallableDefinition)node);
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
			traverse(node.self);
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
			Visit((ClassType)node);
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
			Visit((ValueDeclaration)node);
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

		public override bool Visit(Assignment node)
		{
			//Visit((Statement) node);
            outputCode("", true, false);
			traverse(node.lvalue);
            outputCode(" = ", false, false);
			traverse(node.expr);
            outputCode(";", false, true);
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
            outputCode("", true, false);
			//Visit((Statement) node);
			traverse(node.expr);
            outputCode(";", false, true);
			return true;
		}

        public override bool Visit(ExprAsLvalue node)
        {
            outputCode("(", false, false);
            //Visit((Statement) node);
            traverse(node.expr);
            outputCode(")", false, false);
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
			//Visit((LoopStatement) node);
            outputCode("do ", true, true);
            traverse(node.block);
            outputCode("while (!", true, false);
            traverse(node.condition);
            outputCode(");", false, true);
			return true;
		}

		public override bool Visit(WhileLoop node)
		{
            outputCode("while ", true, false);
            traverse(node.condition);
            outputCode(" do", false, true);
			//Visit((LoopStatement) node);
            traverse(node.block);
			return true;
		}

		public override bool Visit(ForLoop node)
		{
			//Visit((LoopStatement) node);
            outputCode("for (", true, false);
			traverse(node.var);
            outputCode(" = ", false, false);
			traverse(node.start);
            outputCode("; ", false, false);
            traverse(node.var);
            if (node.direction<0)
                outputCode(" >= ", false, false);
            else
                outputCode(" <= ", false, false);
			traverse(node.end);
            outputCode("; ", false, false);
            traverse(node.var);

            if (node.direction < 0)
                outputCode("--)", false, true);
            else
                outputCode("++)", false, true);

            
            traverse(node.block);
            
			return true;
		}

		public override bool Visit(BlockStatement node)
        {
            outputCode("{", true, true);
            PushIdent();
            Visit((Statement)node);
            traverse(node.stmts);
            PopIdent();
            outputCode("}", true, true);
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
			//traverse(node.left);
			//traverse(node.right);
			return true;
		}

		public override bool Visit(Subtraction node)
		{
            traverse(node.left);
            outputCode(" - ", false, false);
            traverse(node.right);
			//Visit((ArithmethicBinaryExpression) node);
			return true;
		}

		public override bool Visit(Addition node)
		{
            traverse(node.left);
            outputCode(" + ", false, false);
            traverse(node.right);
			//Visit((ArithmethicBinaryExpression) node);
			return true;
		}

		public override bool Visit(Product node)
		{
            traverse(node.left);
            outputCode(" * ", false, false);
            traverse(node.right);
			//Visit((ArithmethicBinaryExpression) node);
			return true;
		}

		public override bool Visit(Division node)
		{
            traverse(node.left);
            outputCode(" / ", false, false);
            traverse(node.right);
			//Visit((ArithmethicBinaryExpression) node);
			return true;
		}

		public override bool Visit(Quotient node)
		{
            outputCode("(int)(", false, false);
            traverse(node.left);
            outputCode(" / ", false, false);
            traverse(node.right);
            outputCode(" )", false, false);
			//Visit((ArithmethicBinaryExpression) node);
			return true;
		}

		public override bool Visit(Modulus node)
		{
            traverse(node.left);
            outputCode(" % ", false, false);
            traverse(node.right);
			//Visit((ArithmethicBinaryExpression) node);
			return true;
		}

		public override bool Visit(ShiftRight node)
		{
            traverse(node.left);
            outputCode(" >> ", false, false);
            traverse(node.right);
			//Visit((ArithmethicBinaryExpression) node);
			return true;
		}

		public override bool Visit(ShiftLeft node)
		{
            traverse(node.left);
            outputCode(" << ", false, false);
            traverse(node.right);
			//Visit((ArithmethicBinaryExpression) node);
			return true;
		}

		public override bool Visit(LogicalBinaryExpression node)
		{
			Visit((BinaryExpression) node);
			return true;
		}

		public override bool Visit(LogicalAnd node)
		{
            traverse(node.left);
            outputCode(" & ", false, false);
            traverse(node.right);
			//Visit((LogicalBinaryExpression) node);
			return true;
		}

		public override bool Visit(LogicalOr node)
		{
            traverse(node.left);
            outputCode(" | ", false, false);
            traverse(node.right);
			//Visit((LogicalBinaryExpression) node);
			return true;
		}

		public override bool Visit(LogicalXor node)
		{
            traverse(node.left);
            outputCode(" ^ ", false, false);
            traverse(node.right);
			//Visit((LogicalBinaryExpression) node);
			return true;
		}

		public override bool Visit(Equal node)
		{
            traverse(node.left);
            outputCode(" = ", false, false);
            traverse(node.right);
			//Visit((LogicalBinaryExpression) node);
			return true;
		}

		public override bool Visit(NotEqual node)
		{
            traverse(node.left);
            outputCode(" != ", false, false);
            traverse(node.right);
			//Visit((LogicalBinaryExpression) node);
			return true;
		}

		public override bool Visit(LessThan node)
		{
            traverse(node.left);
            outputCode(" < ", false, false);
            traverse(node.right);
			//Visit((LogicalBinaryExpression) node);
			return true;
		}

		public override bool Visit(LessOrEqual node)
		{
            traverse(node.left);
            outputCode(" <= ", false, false);
            traverse(node.right);
			//Visit((LogicalBinaryExpression) node);
			return true;
		}

		public override bool Visit(GreaterThan node)
		{
            traverse(node.left);
            outputCode(" > ", false, false);
            traverse(node.right);
			//Visit((LogicalBinaryExpression) node);
			return true;
		}

		public override bool Visit(GreaterOrEqual node)
		{
            traverse(node.left);
            outputCode(" >= ", false, false);
            traverse(node.right);
			//Visit((LogicalBinaryExpression) node);
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
			traverse(node.expr);
			return true;
		}

		public override bool Visit(UnaryPlus node)
		{
			outputCode("+", false, false);
			Visit((SimpleUnaryExpression) node);
			return true;
		}

		public override bool Visit(UnaryMinus node)
		{
			outputCode("-", false, false);
			Visit((SimpleUnaryExpression) node);
			return true;
		}

		public override bool Visit(LogicalNot node)
		{
			outputCode("!", false, false);
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
			//Visit((OrdinalLiteral) node);
            outputCode((node.Value as IntegralValue).val.ToString()+" ", false, false);
			return true;
		}

		public override bool Visit(CharLiteral node)
		{
			Visit((OrdinalLiteral) node);
			return true;
		}

		public override bool Visit(BoolLiteral node)
		{
            if ((node.Value as IntegralValue).val != 0)
                outputCode("true ", false, false);
            else
                outputCode("false ", false, false);
			//Visit((OrdinalLiteral) node);
			return true;
		}

		public override bool Visit(StringLiteral node)
		{
            outputCode('"'+(node.Value as StringValue).val + '"', false, false);
			return true;
		}

		public override bool Visit(RealLiteral node)
		{
            outputCode((node.Value as RealValue).val.ToString()+ " ", false, false);
			//Visit((Literal) node);
			return true;
		}

		public override bool Visit(PointerLiteral node)
		{
            outputCode("null ", false, false);
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
			//Visit((LvalueExpression) node);
			traverse(node.func);
            outputCode("(", false, false); 
			traverse(node.args);
            outputCode(")", false, false);
			//traverse(node.basictype);
			return true;
		}

		public override bool Visit(ClassInstantiation node)
		{
			Visit((LvalueExpression)node);
			traverse(node.castType);
			traverse(node.args);
			return true;
		}

		public override bool Visit(FieldAccess node)
		{
			traverse(node.obj); 
			outputCode("." + node.field, false, false);			
			return true;
		}

		public override bool Visit(Identifier node)
		{
			//Visit((LvalueExpression) node);
			if (node.name.Equals("self"))
				outputCode("this", false, false);
			else
				outputCode(node.name, false, false);
			return true;
		}

		public override bool Visit(TypeNode node)
		{
			Visit((Node) node);
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

		public override bool Visit(StringType node)
		{
            outputCode("std::string ", true, false);
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
			//Visit((UnsignedIntegerType) node);
            outputCode("unsigned char ", false, false);
			return true;
		}

		public override bool Visit(UnsignedInt16Type node)
		{
			//Visit((UnsignedIntegerType) node);
            outputCode("unsigned short ", false, false);
			return true;
		}

		public override bool Visit(UnsignedInt32Type node)
		{
            outputCode("unsigned int ", false, false);
			//Visit((UnsignedIntegerType) node);
			return true;
		}

		public override bool Visit(UnsignedInt64Type node)
		{
            outputCode("unsigned long long ", false, false);
			//Visit((UnsignedIntegerType) node);
			return true;
		}

		public override bool Visit(SignedInt8Type node)
		{
            outputCode("char ", false, false);
			//Visit((SignedIntegerType) node);
			return true;
		}

		public override bool Visit(SignedInt16Type node)
		{
            outputCode("short ", false, false);
			//Visit((SignedIntegerType) node);
			return true;
		}

		public override bool Visit(SignedInt32Type node)
		{
            outputCode("int ", false, false);
			//Visit((SignedIntegerType) node);
			return true;
		}

		public override bool Visit(SignedInt64Type node)
		{
            outputCode("long long ", false, false);
			//Visit((IntegerType) node);
			return true;
		}

		public override bool Visit(BoolType node)
		{
            outputCode("bool ", false, false);
			//Visit((IntegralType) node);
			return true;
		}

		public override bool Visit(CharType node)
		{
            outputCode("char ", false, false);
			//Visit((IntegralType) node);
			return true;
		}

		public override bool Visit(RealType node)
		{
			Visit((ScalarType) node);
			return true;
		}

		public override bool Visit(FloatType node)
		{
            outputCode("float ", false, false);
			//Visit((RealType) node);
			return true;
		}

		public override bool Visit(DoubleType node)
		{
            outputCode("double ", false, false);
			//Visit((RealType) node);
			return true;
		}

		public override bool Visit(ExtendedType node)
		{
            outputCode("double ", false, false);
			//Visit((RealType) node);
			return true;
		}

		public override bool Visit(CurrencyType node)
		{
            outputCode("double ", false, false);
			//Visit((RealType) node);
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
