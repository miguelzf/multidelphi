using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using crosspascal.AST;

namespace crosspascal.cpp
{
	class CppCodegen : Processor
	{

		//public CppCodegen(Type t) : base(t) { }

		public CppCodegen(TreeTraverse t = null) : base(t) { }

		#region OutputUtils
		private int identLevel = 0;

		// Printing helper
		private void EnterNode(Node n)
		{
			string name = n.GetType().Name;			
			identLevel++;
		}

		private void LeaveNode(Node n)
		{
			identLevel--;
		}


		private void BeginLine(string s)
		{
			for (int i = 0; i < identLevel; i++)
				Console.Write(" ");
			Console.Write(s);
		}

		private void EndLine(string s)
		{
			Console.WriteLine(s);
		}

		private void Output(string s)
		{
			Console.Write(s);
		}
		#endregion

/*
 * public override void VisitToken(Token token)
		{
			//this.Output(token.Text);
		}

		public void EmitTokenOrExpression(Node node)
		{
			//this.Output(node.ToString());
			if (node is Token)
				this.Output((node as Token).Text);
			else
				traverse(node);
		}

		public override void VisitDelimitedItemNode(Node node, Node item, Token delimiter)
		{
			EmitTokenOrExpression(item);
			
			if (delimiter == null)
				this.EndLine(";");
			else
				this.Output(delimiter.Text);

		}

		public override void VisitArrayTypeNode(ArrayTypeNode node)
		{
			traverse(node.IndexListNode);
			traverse(node.TypeNode);
		}

		public override void VisitAssemblerStatementNode(AssemblerStatementNode node)
		{

		}

		public override void VisitAttributeNode(AttributeNode node)
		{
			traverse(node.ScopeNode);
			traverse(node.ValueNode);
		}

		public override void VisitBinaryOperationNode(BinaryOperationNode node)
		{			
			EmitTokenOrExpression(node.LeftNode);
			traverse(node.OperatorNode);
			this.Output(DelphiToCpp.Operator(node.OperatorNode.Text));
			EmitTokenOrExpression(node.RightNode);			
		}

		public override void VisitBlockNode(BlockNode node)
		{
			this.BeginLine("");
			this.EndLine("{");
			traverse(node.StatementListNode);
			this.EndLine("}");
		}

		public override void VisitCaseSelectorNode(CaseSelectorNode node)
		{
			traverse(node.ValueListNode);
			traverse(node.StatementNode);
		}

		public override void VisitCaseStatementNode(CaseStatementNode node)
		{
			traverse(node.ExpressionNode);
			traverse(node.SelectorListNode);
			traverse(node.ElseStatementListNode);
		}

		public override void VisitClassOfNode(ClassOfNode node)
		{
			traverse(node.TypeNode);
		}

		public override void VisitClassTypeNode(ClassTypeNode node)
		{
			traverse(node.DispositionNode);
			traverse(node.InheritanceListNode);
			traverse(node.ContentListNode);
		}

		public override void VisitConstantDeclNode(ConstantDeclNode node)
		{
			traverse(node.NameNode);
			traverse(node.TypeNode);
			traverse(node.ValueNode);
			traverse(node.PortabilityDirectiveListNode);

		}

		public override void VisitConstantListNode(ConstantListNode node)
		{
			traverse(node.ItemListNode);
		}

		public override void VisitConstSectionNode(ConstSectionNode node)
		{
			traverse(node.ConstListNode);
		}

		public override void VisitDirectiveNode(DirectiveNode node)
		{
			traverse(node.ValueNode);
			traverse(node.DataNode);
		}

		public override void VisitEnumeratedTypeElementNode(EnumeratedTypeElementNode node)
		{
			traverse(node.NameNode);
			traverse(node.ValueNode);
		}

		public override void VisitEnumeratedTypeNode(EnumeratedTypeNode node)
		{
			traverse(node.ItemListNode);
		}

		public override void VisitExceptionItemNode(ExceptionItemNode node)
		{
			traverse(node.NameNode);
			traverse(node.TypeNode);
			traverse(node.StatementNode);
		}

		public override void VisitExportsItemNode(ExportsItemNode node)
		{
			traverse(node.NameNode);
			traverse(node.SpecifierListNode);
		}

		public override void VisitExportsSpecifierNode(ExportsSpecifierNode node)
		{
			traverse(node.ValueNode);
		}

		public override void VisitExportsStatementNode(ExportsStatementNode node)
		{
			traverse(node.ItemListNode);
		}

		public override void VisitFancyBlockNode(FancyBlockNode node)
		{
			traverse(node.DeclListNode);
			traverse(node.BlockNode);
		}

		public override void VisitFieldDeclNode(FieldDeclNode node)
		{
			traverse(node.NameListNode);
			traverse(node.TypeNode);
			traverse(node.PortabilityDirectiveListNode);
		}

		public override void VisitFieldSectionNode(FieldSectionNode node)
		{
			traverse(node.FieldListNode);
		}

		public override void VisitFileTypeNode(FileTypeNode node)
		{
			traverse(node.TypeNode);
		}

		public override void VisitForInStatementNode(ForInStatementNode node)
		{
			traverse(node.LoopVariableNode);
			traverse(node.ExpressionNode);
			traverse(node.StatementNode);
		}

		public override void VisitForStatementNode(ForStatementNode node)
		{
			traverse(node.LoopVariableNode);

			traverse(node.StartingValueNode);
			traverse(node.DirectionNode);
			traverse(node.EndingValueNode);
			traverse(node.StatementNode);
		}

		public override void VisitGotoStatementNode(GotoStatementNode node)
		{
			traverse(node.LabelIdNode);
		}

		public override void VisitIfStatementNode(IfStatementNode node)
		{
			traverse(node.ConditionNode);
			traverse(node.ThenStatementNode);
			traverse(node.ElseStatementNode);
		}

		public override void VisitInitSectionNode(InitSectionNode node)
		{
			traverse(node.InitializationStatementListNode);
			traverse(node.FinalizationStatementListNode);
		}

		public override void VisitInterfaceTypeNode(InterfaceTypeNode node)
		{
			traverse(node.BaseInterfaceNode);
			traverse(node.GuidNode);
			traverse(node.MethodAndPropertyListNode);
		}

		public override void VisitLabelDeclSectionNode(LabelDeclSectionNode node)
		{
			traverse(node.LabelListNode);
		}

		public override void VisitLabeledStatementNode(LabeledStatementNode node)
		{
			traverse(node.LabelIdNode);
			traverse(node.StatementNode);
		}

		public override void VisitMethodHeadingNode(MethodHeadingNode node)
		{
			traverse(node.MethodTypeNode);
			traverse(node.NameNode);
			traverse(node.ParameterListNode);
			traverse(node.ReturnTypeNode);
			traverse(node.DirectiveListNode);
		}

		public override void VisitMethodImplementationNode(MethodImplementationNode node)
		{
			traverse(node.MethodHeadingNode);
			traverse(node.FancyBlockNode);
		}

		public override void VisitMethodResolutionNode(MethodResolutionNode node)
		{
			traverse(node.MethodTypeNode);
			traverse(node.InterfaceMethodNode);
			traverse(node.EqualSignNode);
			traverse(node.ImplementationMethodNode);

		}

		public override void VisitNumberFormatNode(NumberFormatNode node)
		{
			traverse(node.ValueNode);

			traverse(node.SizeNode);

			traverse(node.PrecisionNode);
		}

		public override void VisitOpenArrayNode(OpenArrayNode node)
		{
			traverse(node.TypeNode);
		}

		public override void VisitPackageNode(PackageNode node)
		{
			traverse(node.NameNode);
			traverse(node.RequiresClauseNode);
			traverse(node.ContainsClauseNode);
			traverse(node.AttributeListNode);
		}

		public override void VisitPackedTypeNode(PackedTypeNode node)
		{
			traverse(node.TypeNode);
		}

		public override void VisitParameterizedNode(ParameterizedNode node)
		{
			traverse(node.ParameterListNode);
		}

		public override void VisitParameterNode(ParameterNode node)
		{
			traverse(node.ModifierNode);
			traverse(node.NameListNode);
			traverse(node.TypeNode);
			traverse(node.DefaultValueNode);
		}

		public override void VisitParenthesizedExpressionNode(ParenthesizedExpressionNode node)
		{
			traverse(node.ExpressionNode);
		}

		public override void VisitPointerDereferenceNode(PointerDereferenceNode node)
		{
			traverse(node.OperandNode);
		}

		public override void VisitPointerTypeNode(PointerTypeNode node)
		{
			traverse(node.TypeNode);
		}

		public override void VisitProcedureTypeNode(ProcedureTypeNode node)
		{
			traverse(node.MethodTypeNode);
			traverse(node.ParameterListNode);
			traverse(node.ReturnTypeNode);
			traverse(node.FirstDirectiveListNode);
			traverse(node.SecondDirectiveListNode);
		}

		public override void VisitProgramNode(ProgramNode node)
		{
			traverse(node.NameNode);
			traverse(node.UsesClauseNode);
			traverse(node.DeclarationListNode);
			traverse(node.InitSectionNode);
			traverse(node.DotNode);
		}

		public override void VisitPropertyNode(PropertyNode node)
		{
			traverse(node.NameNode);
			traverse(node.ParameterListNode);
			traverse(node.TypeNode);
			traverse(node.DirectiveListNode);
		}

		public override void VisitRaiseStatementNode(RaiseStatementNode node)
		{
			traverse(node.ExceptionNode);
			traverse(node.AddressNode);
		}

		public override void VisitRecordFieldConstantNode(RecordFieldConstantNode node)
		{
			traverse(node.NameNode);
			traverse(node.ValueNode);
		}

		public override void VisitRecordTypeNode(RecordTypeNode node)
		{
			traverse(node.ContentListNode);
			traverse(node.VariantSectionNode);
		}

		public override void VisitRepeatStatementNode(RepeatStatementNode node)
		{
			traverse(node.StatementListNode);
			traverse(node.ConditionNode);
		}

		public override void VisitRequiresClauseNode(RequiresClauseNode node)
		{
			traverse(node.PackageListNode);
		}

		public override void VisitSetLiteralNode(SetLiteralNode node)
		{
			traverse(node.OpenBracketNode);
			traverse(node.ItemListNode);
			traverse(node.CloseBracketNode);
		}

		public override void VisitSetOfNode(SetOfNode node)
		{
			traverse(node.TypeNode);
		}

		public override void VisitStringOfLengthNode(StringOfLengthNode node)
		{
			traverse(node.OpenBracketNode);
			traverse(node.LengthNode);
			traverse(node.CloseBracketNode);
		}

		public override void VisitTryExceptNode(TryExceptNode node)
		{
			traverse(node.TryStatementListNode);
			traverse(node.ExceptionItemListNode);
			traverse(node.ElseStatementListNode);
		}

		public override void VisitTryFinallyNode(TryFinallyNode node)
		{
			traverse(node.TryStatementListNode);
			traverse(node.FinallyStatementListNode);
		}

		public override void VisitTypeDeclNode(TypeDeclNode node)
		{
			traverse(node.NameNode);
			traverse(node.EqualSignNode);
			traverse(node.TypeNode);
			traverse(node.PortabilityDirectiveListNode);

		}

		public override void VisitTypeForwardDeclarationNode(TypeForwardDeclarationNode node)
		{
			traverse(node.NameNode);
			traverse(node.EqualSignNode);
			traverse(node.TypeNode);

		}

		public override void VisitTypeHelperNode(TypeHelperNode node)
		{
			traverse(node.BaseHelperTypeNode);
			traverse(node.TypeNode);
			traverse(node.ContentListNode);
		}

		public override void VisitTypeSectionNode(TypeSectionNode node)
		{
			traverse(node.TypeListNode);
		}

		public override void VisitUnaryOperationNode(UnaryOperationNode node)
		{
			traverse(node.OperatorNode);
			traverse(node.OperandNode);
		}

		public override void VisitUnitNode(UnitNode node)
		{
			traverse(node.UnitNameNode);
			traverse(node.PortabilityDirectiveListNode);

			traverse(node.InterfaceSectionNode);
			traverse(node.ImplementationSectionNode);
			traverse(node.InitSectionNode);
			traverse(node.DotNode);
		}

		public override void VisitUnitSectionNode(UnitSectionNode node)
		{
			traverse(node.UsesClauseNode);
			traverse(node.ContentListNode);
		}

		public override void VisitUsedUnitNode(UsedUnitNode node)
		{
			traverse(node.NameNode);
			traverse(node.FileNameNode);
		}

		public override void VisitUsesClauseNode(UsesClauseNode node)
		{
			traverse(node.UnitListNode);
		}

		public override void VisitVarDeclNode(VarDeclNode node)
		{
			//this.BeginLine(node.TypeNode);
			EmitTokenOrExpression(node.TypeNode);

			traverse(node.NameListNode);
			
			traverse(node.FirstPortabilityDirectiveListNode);
			traverse(node.AbsoluteAddressNode);
			traverse(node.ValueNode);
			traverse(node.SecondPortabilityDirectiveListNode);
			this.EndLine(";");
		}

		public override void VisitVariantGroupNode(VariantGroupNode node)
		{
			traverse(node.ValueListNode);
			traverse(node.FieldDeclListNode);
			traverse(node.VariantSectionNode);
		}

		public override void VisitVariantSectionNode(VariantSectionNode node)
		{
			traverse(node.NameNode);

			traverse(node.TypeNode);
			traverse(node.VariantGroupListNode);
		}

		public override void VisitVarSectionNode(VarSectionNode node)
		{
			traverse(node.VarListNode);
		}

		public override void VisitVisibilityNode(VisibilityNode node)
		{
		
		}

		public override void VisitVisibilitySectionNode(VisibilitySectionNode node)
		{
			traverse(node.VisibilityNode);
			traverse(node.ContentListNode);
		}

		public override void VisitWhileStatementNode(WhileStatementNode node)
		{
			traverse(node.ConditionNode);
			traverse(node.StatementNode);
		}

		public override void VisitWithStatementNode(WithStatementNode node)
		{
			traverse(node.ExpressionListNode);
			traverse(node.StatementNode);
		}
*/
	}
}
