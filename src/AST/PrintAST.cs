using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DGrok.DelphiNodes;
using DGrok.Framework;

namespace crosspascal.AST
{

	// For debug, prints the whole tree

	class PrintAST : Processor
	{
		private int identLevel = 0;
		private StringBuilder builder = new StringBuilder();


		// =================================================
		// Public interface

		public PrintAST(Type t) : base(t) { }

		public PrintAST(TreeTraverse t = null) : base(t) { }

		public override string ToString()
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

		// Printing helper
		private void EnterNode(AstNode n)
		{
			string name = n.GetType().Name;
			builder.Append(' ', identLevel);
			builder.AppendLine(name);
			identLevel++;
		}

		private void LeaveNode(AstNode n)
		{
			identLevel--;
		}

		public override void VisitDelimitedItemNode(AstNode node, AstNode item, Token delimiter)
		{
			EnterNode(node);
			traverse(item);
			LeaveNode(node);
		}

		public override void VisitListNode(AstNode node, IEnumerable<AstNode> items)
		{
			EnterNode(node);
			foreach (AstNode item in items)
				traverse(item);
			LeaveNode(node);
		}

		// Only called from Visit(CodeBase codeBase)
		public override void VisitSourceFile(string fileName, AstNode node)
		{
			EnterNode(node);
			traverse(node);
			LeaveNode(node);
		}

		public override void VisitArrayTypeNode(ArrayTypeNode node)
		{
			EnterNode(node);
			traverse(node.IndexListNode);
			traverse(node.TypeNode);
			LeaveNode(node);
		}

		public override void VisitAssemblerStatementNode(AssemblerStatementNode node)
		{
			EnterNode(node);
			LeaveNode(node);
		}

		public override void VisitAttributeNode(AttributeNode node)
		{
			EnterNode(node);
			traverse(node.ScopeNode);
			traverse(node.ValueNode);
			LeaveNode(node);
		}

		public override void VisitBinaryOperationNode(BinaryOperationNode node)
		{
			EnterNode(node);
			traverse(node.LeftNode);
			traverse(node.OperatorNode);
			traverse(node.RightNode);
			LeaveNode(node);
		}

		public override void VisitBlockNode(BlockNode node)
		{
			EnterNode(node);
			traverse(node.StatementListNode);
			LeaveNode(node);
		}

		public override void VisitCaseSelectorNode(CaseSelectorNode node)
		{
			EnterNode(node);
			traverse(node.ValueListNode);
			traverse(node.StatementNode);
			LeaveNode(node);
		}

		public override void VisitCaseStatementNode(CaseStatementNode node)
		{
			EnterNode(node);
			traverse(node.ExpressionNode);
			traverse(node.SelectorListNode);
			traverse(node.ElseStatementListNode);
			LeaveNode(node);
		}

		public override void VisitClassOfNode(ClassOfNode node)
		{
			EnterNode(node);
			traverse(node.TypeNode);
			LeaveNode(node);
		}

		public override void VisitClassTypeNode(ClassTypeNode node)
		{
			EnterNode(node);
			traverse(node.DispositionNode);
			traverse(node.InheritanceListNode);
			traverse(node.ContentListNode);
			LeaveNode(node);
		}

		public override void VisitConstantDeclNode(ConstantDeclNode node)
		{
			EnterNode(node);
			traverse(node.NameNode);
			traverse(node.TypeNode);
			traverse(node.ValueNode);
			traverse(node.PortabilityDirectiveListNode);
			LeaveNode(node);
		}

		public override void VisitConstantListNode(ConstantListNode node)
		{
			EnterNode(node);
			traverse(node.ItemListNode);
			LeaveNode(node);
		}

		public override void VisitConstSectionNode(ConstSectionNode node)
		{
			EnterNode(node);
			traverse(node.ConstListNode);
			LeaveNode(node);
		}

		public override void VisitDirectiveNode(DirectiveNode node)
		{
			EnterNode(node);
			traverse(node.ValueNode);
			traverse(node.DataNode);
			LeaveNode(node);
		}

		public override void VisitEnumeratedTypeElementNode(EnumeratedTypeElementNode node)
		{
			EnterNode(node);
			traverse(node.NameNode);
			traverse(node.ValueNode);
			LeaveNode(node);
		}

		public override void VisitEnumeratedTypeNode(EnumeratedTypeNode node)
		{
			EnterNode(node);
			traverse(node.ItemListNode);
			LeaveNode(node);
		}

		public override void VisitExceptionItemNode(ExceptionItemNode node)
		{
			EnterNode(node);
			traverse(node.NameNode);
			traverse(node.TypeNode);
			traverse(node.StatementNode);
			LeaveNode(node);
		}

		public override void VisitExportsItemNode(ExportsItemNode node)
		{
			EnterNode(node);
			traverse(node.NameNode);
			traverse(node.SpecifierListNode);
			LeaveNode(node);
		}

		public override void VisitExportsSpecifierNode(ExportsSpecifierNode node)
		{
			EnterNode(node);
			traverse(node.ValueNode);
			LeaveNode(node);
		}

		public override void VisitExportsStatementNode(ExportsStatementNode node)
		{
			EnterNode(node);
			traverse(node.ItemListNode);
			LeaveNode(node);
		}

		public override void VisitFancyBlockNode(FancyBlockNode node)
		{
			EnterNode(node);
			traverse(node.DeclListNode);
			traverse(node.BlockNode);
			LeaveNode(node);
		}

		public override void VisitFieldDeclNode(FieldDeclNode node)
		{
			EnterNode(node);
			traverse(node.NameListNode);
			traverse(node.TypeNode);
			traverse(node.PortabilityDirectiveListNode);
			LeaveNode(node);
		}

		public override void VisitFieldSectionNode(FieldSectionNode node)
		{
			EnterNode(node);
			traverse(node.FieldListNode);
			LeaveNode(node);
		}

		public override void VisitFileTypeNode(FileTypeNode node)
		{
			EnterNode(node);
			traverse(node.TypeNode);
			LeaveNode(node);
		}

		public override void VisitForInStatementNode(ForInStatementNode node)
		{
			EnterNode(node);
			traverse(node.LoopVariableNode);
			traverse(node.ExpressionNode);
			traverse(node.StatementNode);
			LeaveNode(node);
		}

		public override void VisitForStatementNode(ForStatementNode node)
		{
			EnterNode(node);
			traverse(node.LoopVariableNode);

			traverse(node.StartingValueNode);
			traverse(node.DirectionNode);
			traverse(node.EndingValueNode);
			traverse(node.StatementNode);
			LeaveNode(node);
		}

		public override void VisitGotoStatementNode(GotoStatementNode node)
		{
			EnterNode(node);
			traverse(node.LabelIdNode);
			LeaveNode(node);
		}

		public override void VisitIfStatementNode(IfStatementNode node)
		{
			EnterNode(node);
			traverse(node.ConditionNode);
			traverse(node.ThenStatementNode);
			traverse(node.ElseStatementNode);
			LeaveNode(node);
		}

		public override void VisitInitSectionNode(InitSectionNode node)
		{
			EnterNode(node);
			traverse(node.InitializationStatementListNode);
			traverse(node.FinalizationStatementListNode);
			LeaveNode(node);
		}

		public override void VisitInterfaceTypeNode(InterfaceTypeNode node)
		{
			EnterNode(node);
			traverse(node.BaseInterfaceNode);
			traverse(node.GuidNode);
			traverse(node.MethodAndPropertyListNode);
			LeaveNode(node);
		}

		public override void VisitLabelDeclSectionNode(LabelDeclSectionNode node)
		{
			EnterNode(node);
			traverse(node.LabelListNode);
			LeaveNode(node);
		}

		public override void VisitLabeledStatementNode(LabeledStatementNode node)
		{
			EnterNode(node);
			traverse(node.LabelIdNode);
			traverse(node.StatementNode);
			LeaveNode(node);
		}

		public override void VisitMethodHeadingNode(MethodHeadingNode node)
		{
			EnterNode(node);
			traverse(node.MethodTypeNode);
			traverse(node.NameNode);
			traverse(node.ParameterListNode);
			traverse(node.ReturnTypeNode);
			traverse(node.DirectiveListNode);
			LeaveNode(node);
		}

		public override void VisitMethodImplementationNode(MethodImplementationNode node)
		{
			EnterNode(node);
			traverse(node.MethodHeadingNode);
			traverse(node.FancyBlockNode);
			LeaveNode(node);
		}

		public override void VisitMethodResolutionNode(MethodResolutionNode node)
		{
			EnterNode(node);
			traverse(node.MethodTypeNode);
			traverse(node.InterfaceMethodNode);
			traverse(node.EqualSignNode);
			traverse(node.ImplementationMethodNode);
			LeaveNode(node);
		}

		public override void VisitNumberFormatNode(NumberFormatNode node)
		{
			EnterNode(node);
			traverse(node.ValueNode);
			traverse(node.SizeNode);
			traverse(node.PrecisionNode);
			LeaveNode(node);
		}

		public override void VisitOpenArrayNode(OpenArrayNode node)
		{
			EnterNode(node);
			traverse(node.TypeNode);
			LeaveNode(node);
		}

		public override void VisitPackageNode(PackageNode node)
		{
			EnterNode(node);
			traverse(node.NameNode);
			traverse(node.RequiresClauseNode);
			traverse(node.ContainsClauseNode);
			traverse(node.AttributeListNode);
			LeaveNode(node);
		}

		public override void VisitPackedTypeNode(PackedTypeNode node)
		{
			EnterNode(node);
			traverse(node.TypeNode);
			LeaveNode(node);
		}

		public override void VisitParameterizedNode(ParameterizedNode node)
		{
			EnterNode(node);
			traverse(node.ParameterListNode);
			LeaveNode(node);
		}

		public override void VisitParameterNode(ParameterNode node)
		{
			EnterNode(node);
			traverse(node.ModifierNode);
			traverse(node.NameListNode);
			traverse(node.TypeNode);
			traverse(node.DefaultValueNode);
			LeaveNode(node);
		}

		public override void VisitParenthesizedExpressionNode(ParenthesizedExpressionNode node)
		{
			EnterNode(node);
			traverse(node.ExpressionNode);
			LeaveNode(node);
		}

		public override void VisitPointerDereferenceNode(PointerDereferenceNode node)
		{
			EnterNode(node);
			traverse(node.OperandNode);
			LeaveNode(node);
		}

		public override void VisitPointerTypeNode(PointerTypeNode node)
		{
			EnterNode(node);
			traverse(node.TypeNode);
			LeaveNode(node);
		}

		public override void VisitProcedureTypeNode(ProcedureTypeNode node)
		{
			EnterNode(node);
			traverse(node.MethodTypeNode);
			traverse(node.ParameterListNode);
			traverse(node.ReturnTypeNode);
			traverse(node.FirstDirectiveListNode);
			traverse(node.SecondDirectiveListNode);
			LeaveNode(node);
		}

		public override void VisitProgramNode(ProgramNode node)
		{
			EnterNode(node);
			traverse(node.NameNode);
			traverse(node.UsesClauseNode);
			traverse(node.DeclarationListNode);
			traverse(node.InitSectionNode);
			traverse(node.DotNode);
			LeaveNode(node);
		}

		public override void VisitPropertyNode(PropertyNode node)
		{
			EnterNode(node);
			traverse(node.NameNode);
			traverse(node.ParameterListNode);
			traverse(node.TypeNode);
			traverse(node.DirectiveListNode);
			LeaveNode(node);
		}

		public override void VisitRaiseStatementNode(RaiseStatementNode node)
		{
			EnterNode(node);
			traverse(node.ExceptionNode);
			traverse(node.AddressNode);
			LeaveNode(node);
		}

		public override void VisitRecordFieldConstantNode(RecordFieldConstantNode node)
		{
			EnterNode(node);
			traverse(node.NameNode);
			traverse(node.ValueNode);
			LeaveNode(node);
		}

		public override void VisitRecordTypeNode(RecordTypeNode node)
		{
			EnterNode(node);
			traverse(node.ContentListNode);
			traverse(node.VariantSectionNode);
			LeaveNode(node);
		}

		public override void VisitRepeatStatementNode(RepeatStatementNode node)
		{
			EnterNode(node);
			traverse(node.StatementListNode);
			traverse(node.ConditionNode);
			LeaveNode(node);
		}

		public override void VisitRequiresClauseNode(RequiresClauseNode node)
		{
			EnterNode(node);
			traverse(node.PackageListNode);
			LeaveNode(node);
		}

		public override void VisitSetLiteralNode(SetLiteralNode node)
		{
			EnterNode(node);
			traverse(node.OpenBracketNode);
			traverse(node.ItemListNode);
			traverse(node.CloseBracketNode);
			LeaveNode(node);
		}

		public override void VisitSetOfNode(SetOfNode node)
		{
			EnterNode(node);
			traverse(node.TypeNode);
			LeaveNode(node);
		}

		public override void VisitStringOfLengthNode(StringOfLengthNode node)
		{
			EnterNode(node);
			traverse(node.OpenBracketNode);
			traverse(node.LengthNode);
			traverse(node.CloseBracketNode);
			LeaveNode(node);
		}

		public override void VisitTryExceptNode(TryExceptNode node)
		{
			EnterNode(node);
			traverse(node.TryStatementListNode);
			traverse(node.ExceptionItemListNode);
			traverse(node.ElseStatementListNode);
			LeaveNode(node);
		}

		public override void VisitTryFinallyNode(TryFinallyNode node)
		{
			EnterNode(node);
			traverse(node.TryStatementListNode);
			traverse(node.FinallyStatementListNode);
			LeaveNode(node);
		}

		public override void VisitTypeDeclNode(TypeDeclNode node)
		{
			EnterNode(node);
			traverse(node.NameNode);
			traverse(node.EqualSignNode);
			traverse(node.TypeNode);
			traverse(node.PortabilityDirectiveListNode);

		}

		public override void VisitTypeForwardDeclarationNode(TypeForwardDeclarationNode node)
		{
			EnterNode(node);
			traverse(node.NameNode);
			traverse(node.EqualSignNode);
			traverse(node.TypeNode);

		}

		public override void VisitTypeHelperNode(TypeHelperNode node)
		{
			EnterNode(node);
			traverse(node.BaseHelperTypeNode);
			traverse(node.TypeNode);
			traverse(node.ContentListNode);
			LeaveNode(node);
		}

		public override void VisitTypeSectionNode(TypeSectionNode node)
		{
			EnterNode(node);
			traverse(node.TypeListNode);
			LeaveNode(node);
		}

		public override void VisitUnaryOperationNode(UnaryOperationNode node)
		{
			EnterNode(node);
			traverse(node.OperatorNode);
			traverse(node.OperandNode);
			LeaveNode(node);
		}

		public override void VisitUnitNode(UnitNode node)
		{
			EnterNode(node);
			traverse(node.UnitNameNode);
			traverse(node.PortabilityDirectiveListNode);
			traverse(node.InterfaceSectionNode);
			traverse(node.ImplementationSectionNode);
			traverse(node.InitSectionNode);
			traverse(node.DotNode);
			LeaveNode(node);
		}

		public override void VisitUnitSectionNode(UnitSectionNode node)
		{
			EnterNode(node);
			traverse(node.UsesClauseNode);
			traverse(node.ContentListNode);
			LeaveNode(node);
		}

		public override void VisitUsedUnitNode(UsedUnitNode node)
		{
			EnterNode(node);
			traverse(node.NameNode);
			traverse(node.FileNameNode);
			LeaveNode(node);
		}

		public override void VisitUsesClauseNode(UsesClauseNode node)
		{
			EnterNode(node);
			traverse(node.UnitListNode);
			LeaveNode(node);
		}

		public override void VisitVarDeclNode(VarDeclNode node)
		{
			EnterNode(node);
			traverse(node.NameListNode);
			traverse(node.TypeNode);
			traverse(node.FirstPortabilityDirectiveListNode);
			traverse(node.AbsoluteAddressNode);
			traverse(node.ValueNode);
			traverse(node.SecondPortabilityDirectiveListNode);
			LeaveNode(node);
		}

		public override void VisitVariantGroupNode(VariantGroupNode node)
		{
			EnterNode(node);
			traverse(node.ValueListNode);
			traverse(node.FieldDeclListNode);
			traverse(node.VariantSectionNode);
			LeaveNode(node);
		}

		public override void VisitVariantSectionNode(VariantSectionNode node)
		{
			EnterNode(node);
			traverse(node.NameNode);

			traverse(node.TypeNode);
			traverse(node.VariantGroupListNode);
			LeaveNode(node);
		}

		public override void VisitVarSectionNode(VarSectionNode node)
		{
			EnterNode(node);
			traverse(node.VarListNode);
			LeaveNode(node);
		}

		public override void VisitVisibilityNode(VisibilityNode node)
		{
			EnterNode(node);

		}

		public override void VisitVisibilitySectionNode(VisibilitySectionNode node)
		{
			EnterNode(node);
			traverse(node.VisibilityNode);
			traverse(node.ContentListNode);
			LeaveNode(node);
		}

		public override void VisitWhileStatementNode(WhileStatementNode node)
		{
			EnterNode(node);
			traverse(node.ConditionNode);
			traverse(node.StatementNode);
			LeaveNode(node);
		}

		public override void VisitWithStatementNode(WithStatementNode node)
		{
			EnterNode(node);
			traverse(node.ExpressionListNode);
			traverse(node.StatementNode);
		}
	}
}
