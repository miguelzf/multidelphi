using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DGrok.Framework;
using DGrok.DelphiNodes;

namespace crosspascal.AST
{
	// Defines a general AST Visitor, defining all DGrok.Visitor requeriments,
	// but without requiring the implementation of a Visitor pattern. 
	// Uses a Delegate to a generic Tree Traversal method instead

	delegate void TreeTraverse(AstNode n);

	abstract class Processor : DGrok.Framework.Visitor
	{
		public TreeTraverse traverse { get; set; }

		// dummy
		void emptyTraverse(AstNode n) {	}

		// Instantiate Traverser class
		public Processor(Type t)
		{
			if (t == null || !t.IsSubclassOf(typeof(GenericTraverser)))
				return;

			GenericTraverser instance = (GenericTraverser) Activator.CreateInstance(t, new object[] {this});
			traverse = instance.traverse;
		}

		// Create with given traverser function
		public Processor(TreeTraverse t = null)
		{
			if (t == null)
				traverse = emptyTraverse;
			else
				traverse = t;
		}


		// =========================================================================
		//	Complete interface from DGrok.Framework.Visitor, to be implemented
		// =========================================================================

		// Tokens are not processed... Dgrok shouldn't have included them in the AST

		public override void VisitDelimitedItemNode(AstNode node, AstNode item, Token delimiter)
		{
			traverse(item);
			traverse(delimiter);
		}

		public override void VisitListNode(AstNode node, IEnumerable<AstNode> items)
		{
			foreach (AstNode item in items)
				traverse(item);
		}

		// Only called from Visit(CodeBase codeBase)
		public override void VisitSourceFile(string fileName, AstNode node)
		{
			traverse(node);
		}


		// =========================================================================
		// Normal processor methods

		public override void VisitToken(Token token)
		{
			// CHECK that this is never called
			// CHECK that the virtual parent method (fallback) is also never called

			// Ignore tokens. Do nothing
		}
		public override void VisitArrayTypeNode(ArrayTypeNode node)
		{
			traverse(node.ArrayKeywordNode);
			traverse(node.OpenBracketNode);
			traverse(node.IndexListNode);
			traverse(node.CloseBracketNode);
			traverse(node.OfKeywordNode);
			traverse(node.TypeNode);
		}
		public override void VisitAssemblerStatementNode(AssemblerStatementNode node)
		{
			traverse(node.AsmKeywordNode);
			traverse(node.EndKeywordNode);
		}
		public override void VisitAttributeNode(AttributeNode node)
		{
			traverse(node.OpenBracketNode);
			traverse(node.ScopeNode);
			traverse(node.ColonNode);
			traverse(node.ValueNode);
			traverse(node.CloseBracketNode);
		}
		public override void VisitBinaryOperationNode(BinaryOperationNode node)
		{
			traverse(node.LeftNode);
			traverse(node.OperatorNode);
			traverse(node.RightNode);
		}
		public override void VisitBlockNode(BlockNode node)
		{
			traverse(node.BeginKeywordNode);
			traverse(node.StatementListNode);
			traverse(node.EndKeywordNode);
		}
		public override void VisitCaseSelectorNode(CaseSelectorNode node)
		{
			traverse(node.ValueListNode);
			traverse(node.ColonNode);
			traverse(node.StatementNode);
			traverse(node.SemicolonNode);
		}
		public override void VisitCaseStatementNode(CaseStatementNode node)
		{
			traverse(node.CaseKeywordNode);
			traverse(node.ExpressionNode);
			traverse(node.OfKeywordNode);
			traverse(node.SelectorListNode);
			traverse(node.ElseKeywordNode);
			traverse(node.ElseStatementListNode);
			traverse(node.EndKeywordNode);
		}
		public override void VisitClassOfNode(ClassOfNode node)
		{
			traverse(node.ClassKeywordNode);
			traverse(node.OfKeywordNode);
			traverse(node.TypeNode);
		}
		public override void VisitClassTypeNode(ClassTypeNode node)
		{
			traverse(node.ClassKeywordNode);
			traverse(node.DispositionNode);
			traverse(node.OpenParenthesisNode);
			traverse(node.InheritanceListNode);
			traverse(node.CloseParenthesisNode);
			traverse(node.ContentListNode);
			traverse(node.EndKeywordNode);
		}
		public override void VisitConstantDeclNode(ConstantDeclNode node)
		{
			traverse(node.NameNode);
			traverse(node.ColonNode);
			traverse(node.TypeNode);
			traverse(node.EqualSignNode);
			traverse(node.ValueNode);
			traverse(node.PortabilityDirectiveListNode);
			traverse(node.SemicolonNode);
		}
		public override void VisitConstantListNode(ConstantListNode node)
		{
			traverse(node.OpenParenthesisNode);
			traverse(node.ItemListNode);
			traverse(node.CloseParenthesisNode);
		}
		public override void VisitConstSectionNode(ConstSectionNode node)
		{
			traverse(node.ConstKeywordNode);
			traverse(node.ConstListNode);
		}
		public override void VisitDirectiveNode(DirectiveNode node)
		{
			traverse(node.SemicolonNode);
			traverse(node.KeywordNode);
			traverse(node.ValueNode);
			traverse(node.DataNode);
		}
		public override void VisitEnumeratedTypeElementNode(EnumeratedTypeElementNode node)
		{
			traverse(node.NameNode);
			traverse(node.EqualSignNode);
			traverse(node.ValueNode);
		}
		public override void VisitEnumeratedTypeNode(EnumeratedTypeNode node)
		{
			traverse(node.OpenParenthesisNode);
			traverse(node.ItemListNode);
			traverse(node.CloseParenthesisNode);
		}
		public override void VisitExceptionItemNode(ExceptionItemNode node)
		{
			traverse(node.OnSemikeywordNode);
			traverse(node.NameNode);
			traverse(node.ColonNode);
			traverse(node.TypeNode);
			traverse(node.DoKeywordNode);
			traverse(node.StatementNode);
			traverse(node.SemicolonNode);
		}
		public override void VisitExportsItemNode(ExportsItemNode node)
		{
			traverse(node.NameNode);
			traverse(node.SpecifierListNode);
		}
		public override void VisitExportsSpecifierNode(ExportsSpecifierNode node)
		{
			traverse(node.KeywordNode);
			traverse(node.ValueNode);
		}
		public override void VisitExportsStatementNode(ExportsStatementNode node)
		{
			traverse(node.ExportsKeywordNode);
			traverse(node.ItemListNode);
			traverse(node.SemicolonNode);
		}
		public override void VisitFancyBlockNode(FancyBlockNode node)
		{
			traverse(node.DeclListNode);
			traverse(node.BlockNode);
		}
		public override void VisitFieldDeclNode(FieldDeclNode node)
		{
			traverse(node.NameListNode);
			traverse(node.ColonNode);
			traverse(node.TypeNode);
			traverse(node.PortabilityDirectiveListNode);
			traverse(node.SemicolonNode);
		}
		public override void VisitFieldSectionNode(FieldSectionNode node)
		{
			traverse(node.ClassKeywordNode);
			traverse(node.VarKeywordNode);
			traverse(node.FieldListNode);
		}
		public override void VisitFileTypeNode(FileTypeNode node)
		{
			traverse(node.FileKeywordNode);
			traverse(node.OfKeywordNode);
			traverse(node.TypeNode);
		}
		public override void VisitForInStatementNode(ForInStatementNode node)
		{
			traverse(node.ForKeywordNode);
			traverse(node.LoopVariableNode);
			traverse(node.InKeywordNode);
			traverse(node.ExpressionNode);
			traverse(node.DoKeywordNode);
			traverse(node.StatementNode);
		}
		public override void VisitForStatementNode(ForStatementNode node)
		{
			traverse(node.ForKeywordNode);
			traverse(node.LoopVariableNode);
			traverse(node.ColonEqualsNode);
			traverse(node.StartingValueNode);
			traverse(node.DirectionNode);
			traverse(node.EndingValueNode);
			traverse(node.DoKeywordNode);
			traverse(node.StatementNode);
		}
		public override void VisitGotoStatementNode(GotoStatementNode node)
		{
			traverse(node.GotoKeywordNode);
			traverse(node.LabelIdNode);
		}
		public override void VisitIfStatementNode(IfStatementNode node)
		{
			traverse(node.IfKeywordNode);
			traverse(node.ConditionNode);
			traverse(node.ThenKeywordNode);
			traverse(node.ThenStatementNode);
			traverse(node.ElseKeywordNode);
			traverse(node.ElseStatementNode);
		}
		public override void VisitInitSectionNode(InitSectionNode node)
		{
			traverse(node.InitializationKeywordNode);
			traverse(node.InitializationStatementListNode);
			traverse(node.FinalizationKeywordNode);
			traverse(node.FinalizationStatementListNode);
			traverse(node.EndKeywordNode);
		}
		public override void VisitInterfaceTypeNode(InterfaceTypeNode node)
		{
			traverse(node.InterfaceKeywordNode);
			traverse(node.OpenParenthesisNode);
			traverse(node.BaseInterfaceNode);
			traverse(node.CloseParenthesisNode);
			traverse(node.OpenBracketNode);
			traverse(node.GuidNode);
			traverse(node.CloseBracketNode);
			traverse(node.MethodAndPropertyListNode);
			traverse(node.EndKeywordNode);
		}
		public override void VisitLabelDeclSectionNode(LabelDeclSectionNode node)
		{
			traverse(node.LabelKeywordNode);
			traverse(node.LabelListNode);
			traverse(node.SemicolonNode);
		}
		public override void VisitLabeledStatementNode(LabeledStatementNode node)
		{
			traverse(node.LabelIdNode);
			traverse(node.ColonNode);
			traverse(node.StatementNode);
		}
		public override void VisitMethodHeadingNode(MethodHeadingNode node)
		{
			traverse(node.ClassKeywordNode);
			traverse(node.MethodTypeNode);
			traverse(node.NameNode);
			traverse(node.OpenParenthesisNode);
			traverse(node.ParameterListNode);
			traverse(node.CloseParenthesisNode);
			traverse(node.ColonNode);
			traverse(node.ReturnTypeNode);
			traverse(node.DirectiveListNode);
			traverse(node.SemicolonNode);
		}
		public override void VisitMethodImplementationNode(MethodImplementationNode node)
		{
			traverse(node.MethodHeadingNode);
			traverse(node.FancyBlockNode);
			traverse(node.SemicolonNode);
		}
		public override void VisitMethodResolutionNode(MethodResolutionNode node)
		{
			traverse(node.MethodTypeNode);
			traverse(node.InterfaceMethodNode);
			traverse(node.EqualSignNode);
			traverse(node.ImplementationMethodNode);
			traverse(node.SemicolonNode);
		}
		public override void VisitNumberFormatNode(NumberFormatNode node)
		{
			traverse(node.ValueNode);
			traverse(node.SizeColonNode);
			traverse(node.SizeNode);
			traverse(node.PrecisionColonNode);
			traverse(node.PrecisionNode);
		}
		public override void VisitOpenArrayNode(OpenArrayNode node)
		{
			traverse(node.ArrayKeywordNode);
			traverse(node.OfKeywordNode);
			traverse(node.TypeNode);
		}
		public override void VisitPackageNode(PackageNode node)
		{
			traverse(node.PackageKeywordNode);
			traverse(node.NameNode);
			traverse(node.SemicolonNode);
			traverse(node.RequiresClauseNode);
			traverse(node.ContainsClauseNode);
			traverse(node.AttributeListNode);
			traverse(node.EndKeywordNode);
			traverse(node.DotNode);
		}
		public override void VisitPackedTypeNode(PackedTypeNode node)
		{
			traverse(node.PackedKeywordNode);
			traverse(node.TypeNode);
		}
		public override void VisitParameterizedNode(ParameterizedNode node)
		{
			traverse(node.LeftNode);
			traverse(node.OpenDelimiterNode);
			traverse(node.ParameterListNode);
			traverse(node.CloseDelimiterNode);
		}
		public override void VisitParameterNode(ParameterNode node)
		{
			traverse(node.ModifierNode);
			traverse(node.NameListNode);
			traverse(node.ColonNode);
			traverse(node.TypeNode);
			traverse(node.EqualSignNode);
			traverse(node.DefaultValueNode);
		}
		public override void VisitParenthesizedExpressionNode(ParenthesizedExpressionNode node)
		{
			traverse(node.OpenParenthesisNode);
			traverse(node.ExpressionNode);
			traverse(node.CloseParenthesisNode);
		}
		public override void VisitPointerDereferenceNode(PointerDereferenceNode node)
		{
			traverse(node.OperandNode);
			traverse(node.CaretNode);
		}
		public override void VisitPointerTypeNode(PointerTypeNode node)
		{
			traverse(node.CaretNode);
			traverse(node.TypeNode);
		}
		public override void VisitProcedureTypeNode(ProcedureTypeNode node)
		{
			traverse(node.MethodTypeNode);
			traverse(node.OpenParenthesisNode);
			traverse(node.ParameterListNode);
			traverse(node.CloseParenthesisNode);
			traverse(node.ColonNode);
			traverse(node.ReturnTypeNode);
			traverse(node.FirstDirectiveListNode);
			traverse(node.OfKeywordNode);
			traverse(node.ObjectKeywordNode);
			traverse(node.SecondDirectiveListNode);
		}
		public override void VisitProgramNode(ProgramNode node)
		{
			traverse(node.ProgramKeywordNode);
			traverse(node.NameNode);
			traverse(node.NoiseOpenParenthesisNode);
			traverse(node.NoiseContentListNode);
			traverse(node.NoiseCloseParenthesisNode);
			traverse(node.SemicolonNode);
			traverse(node.UsesClauseNode);
			traverse(node.DeclarationListNode);
			traverse(node.InitSectionNode);
			traverse(node.DotNode);
		}
		public override void VisitPropertyNode(PropertyNode node)
		{
			traverse(node.ClassKeywordNode);
			traverse(node.PropertyKeywordNode);
			traverse(node.NameNode);
			traverse(node.OpenBracketNode);
			traverse(node.ParameterListNode);
			traverse(node.CloseBracketNode);
			traverse(node.ColonNode);
			traverse(node.TypeNode);
			traverse(node.DirectiveListNode);
			traverse(node.SemicolonNode);
		}
		public override void VisitRaiseStatementNode(RaiseStatementNode node)
		{
			traverse(node.RaiseKeywordNode);
			traverse(node.ExceptionNode);
			traverse(node.AtSemikeywordNode);
			traverse(node.AddressNode);
		}
		public override void VisitRecordFieldConstantNode(RecordFieldConstantNode node)
		{
			traverse(node.NameNode);
			traverse(node.ColonNode);
			traverse(node.ValueNode);
		}
		public override void VisitRecordTypeNode(RecordTypeNode node)
		{
			traverse(node.RecordKeywordNode);
			traverse(node.ContentListNode);
			traverse(node.VariantSectionNode);
			traverse(node.EndKeywordNode);
		}
		public override void VisitRepeatStatementNode(RepeatStatementNode node)
		{
			traverse(node.RepeatKeywordNode);
			traverse(node.StatementListNode);
			traverse(node.UntilKeywordNode);
			traverse(node.ConditionNode);
		}
		public override void VisitRequiresClauseNode(RequiresClauseNode node)
		{
			traverse(node.RequiresSemikeywordNode);
			traverse(node.PackageListNode);
			traverse(node.SemicolonNode);
		}
		public override void VisitSetLiteralNode(SetLiteralNode node)
		{
			traverse(node.OpenBracketNode);
			traverse(node.ItemListNode);
			traverse(node.CloseBracketNode);
		}
		public override void VisitSetOfNode(SetOfNode node)
		{
			traverse(node.SetKeywordNode);
			traverse(node.OfKeywordNode);
			traverse(node.TypeNode);
		}
		public override void VisitStringOfLengthNode(StringOfLengthNode node)
		{
			traverse(node.StringKeywordNode);
			traverse(node.OpenBracketNode);
			traverse(node.LengthNode);
			traverse(node.CloseBracketNode);
		}
		public override void VisitTryExceptNode(TryExceptNode node)
		{
			traverse(node.TryKeywordNode);
			traverse(node.TryStatementListNode);
			traverse(node.ExceptKeywordNode);
			traverse(node.ExceptionItemListNode);
			traverse(node.ElseKeywordNode);
			traverse(node.ElseStatementListNode);
			traverse(node.EndKeywordNode);
		}
		public override void VisitTryFinallyNode(TryFinallyNode node)
		{
			traverse(node.TryKeywordNode);
			traverse(node.TryStatementListNode);
			traverse(node.FinallyKeywordNode);
			traverse(node.FinallyStatementListNode);
			traverse(node.EndKeywordNode);
		}
		public override void VisitTypeDeclNode(TypeDeclNode node)
		{
			traverse(node.NameNode);
			traverse(node.EqualSignNode);
			traverse(node.TypeKeywordNode);
			traverse(node.TypeNode);
			traverse(node.PortabilityDirectiveListNode);
			traverse(node.SemicolonNode);
		}
		public override void VisitTypeForwardDeclarationNode(TypeForwardDeclarationNode node)
		{
			traverse(node.NameNode);
			traverse(node.EqualSignNode);
			traverse(node.TypeNode);
			traverse(node.SemicolonNode);
		}
		public override void VisitTypeHelperNode(TypeHelperNode node)
		{
			traverse(node.TypeKeywordNode);
			traverse(node.HelperSemikeywordNode);
			traverse(node.OpenParenthesisNode);
			traverse(node.BaseHelperTypeNode);
			traverse(node.CloseParenthesisNode);
			traverse(node.ForKeywordNode);
			traverse(node.TypeNode);
			traverse(node.ContentListNode);
			traverse(node.EndKeywordNode);
		}
		public override void VisitTypeSectionNode(TypeSectionNode node)
		{
			traverse(node.TypeKeywordNode);
			traverse(node.TypeListNode);
		}
		public override void VisitUnaryOperationNode(UnaryOperationNode node)
		{
			traverse(node.OperatorNode);
			traverse(node.OperandNode);
		}
		public override void VisitUnitNode(UnitNode node)
		{
			traverse(node.UnitKeywordNode);
			traverse(node.UnitNameNode);
			traverse(node.PortabilityDirectiveListNode);
			traverse(node.SemicolonNode);
			traverse(node.InterfaceSectionNode);
			traverse(node.ImplementationSectionNode);
			traverse(node.InitSectionNode);
			traverse(node.DotNode);
		}
		public override void VisitUnitSectionNode(UnitSectionNode node)
		{
			traverse(node.HeaderKeywordNode);
			traverse(node.UsesClauseNode);
			traverse(node.ContentListNode);
		}
		public override void VisitUsedUnitNode(UsedUnitNode node)
		{
			traverse(node.NameNode);
			traverse(node.InKeywordNode);
			traverse(node.FileNameNode);
		}
		public override void VisitUsesClauseNode(UsesClauseNode node)
		{
			traverse(node.UsesKeywordNode);
			traverse(node.UnitListNode);
			traverse(node.SemicolonNode);
		}
		public override void VisitVarDeclNode(VarDeclNode node)
		{
			traverse(node.NameListNode);
			traverse(node.ColonNode);
			traverse(node.TypeNode);
			traverse(node.FirstPortabilityDirectiveListNode);
			traverse(node.AbsoluteSemikeywordNode);
			traverse(node.AbsoluteAddressNode);
			traverse(node.EqualSignNode);
			traverse(node.ValueNode);
			traverse(node.SecondPortabilityDirectiveListNode);
			traverse(node.SemicolonNode);
		}
		public override void VisitVariantGroupNode(VariantGroupNode node)
		{
			traverse(node.ValueListNode);
			traverse(node.ColonNode);
			traverse(node.OpenParenthesisNode);
			traverse(node.FieldDeclListNode);
			traverse(node.VariantSectionNode);
			traverse(node.CloseParenthesisNode);
			traverse(node.SemicolonNode);
		}
		public override void VisitVariantSectionNode(VariantSectionNode node)
		{
			traverse(node.CaseKeywordNode);
			traverse(node.NameNode);
			traverse(node.ColonNode);
			traverse(node.TypeNode);
			traverse(node.OfKeywordNode);
			traverse(node.VariantGroupListNode);
		}
		public override void VisitVarSectionNode(VarSectionNode node)
		{
			traverse(node.VarKeywordNode);
			traverse(node.VarListNode);
		}
		public override void VisitVisibilityNode(VisibilityNode node)
		{
			traverse(node.StrictSemikeywordNode);
			traverse(node.VisibilityKeywordNode);
		}
		public override void VisitVisibilitySectionNode(VisibilitySectionNode node)
		{
			traverse(node.VisibilityNode);
			traverse(node.ContentListNode);
		}
		public override void VisitWhileStatementNode(WhileStatementNode node)
		{
			traverse(node.WhileKeywordNode);
			traverse(node.ConditionNode);
			traverse(node.DoKeywordNode);
			traverse(node.StatementNode);
		}
		public override void VisitWithStatementNode(WithStatementNode node)
		{
			traverse(node.WithKeywordNode);
			traverse(node.ExpressionListNode);
			traverse(node.DoKeywordNode);
			traverse(node.StatementNode);
		}
	}
}
