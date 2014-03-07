using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace crosspascal.AST
{
	public enum FunctionKind
	{
		Procedure,
		Function,
		Constructor,
		Destructor
	}

	public enum Visibilty
	{
		Published,
		Public,
		Private,
		Protected 
	}

	public abstract class Node
	{ 
	}

	public abstract class GoalNode : Node
	{
	}

	public class Statement : Node
	{
		public string label;

		public void SetLabel(string label)
		{
			this.label = label;
		}
	}

	public class AssignementStatement : Statement
	{
		public LValueNode lvalue;
		public ExpressionNode expr;
		public bool inherited;

		public AssignementStatement(LValueNode lvalue, ExpressionNode expr, bool inherited)
		{
			this.lvalue = lvalue;
			this.expr = expr;
			this.inherited = inherited;
		}
	}

	public class GotoStatement : Statement
	{
		public LabelNode label;

		public GotoStatement(LabelNode label)
		{
			this.label = label;				 
		}
	}

	public class IfStatement : Statement
	{
		public ExpressionNode condition;
		public Statement ifTrue;
		public Statement ifFalse;

		public IfStatement(ExpressionNode condition, Statement ifTrue, Statement ifFalse)
		{
			this.condition = condition;
			this.ifTrue = ifTrue;
			this.ifFalse = ifFalse;
		}
	}

	public class InheritedStatement : Statement
	{
		public Statement body;

		public InheritedStatement(Statement body)
		{
			this.body = body;
		}
	}

	public class OnStatement : Statement
	{
		public IdentifierNode ident;
		public IdentifierNode type;
		public Statement body;

		public OnStatement(IdentifierNode ident, IdentifierNode type, Statement body)
		{
			this.ident = ident;
			this.type = type;
			this.body = body;
		}
	}

	public class OnListNode : Node
	{
		public OnStatement stmt;
		public OnListNode next;

		public OnListNode(OnStatement stmt, OnListNode next)
		{
			this.stmt = stmt;
			this.next = next;
		}
	}

	public class ExceptionBlockNode : Node
	{
		public OnListNode stmts;
		public Statement onElse;

		public ExceptionBlockNode(OnListNode stmts, Statement onElse)
		{
			this.stmts = stmts;
			this.onElse = onElse;
		}
	}

	public class RaiseStatement : Statement
	{
		public LValueNode lvalue;
		public ExpressionNode expr;

		public RaiseStatement(LValueNode lvalue, ExpressionNode expr)
		{
			this.lvalue = lvalue;
			this.expr = expr;
		}
	}

	public class CaseLabel : Node
	{
		public ExpressionNode minRange;
		public ExpressionNode maxRange;

		public CaseLabel(ExpressionNode minRange, ExpressionNode maxRange)
		{
			this.minRange = minRange;
			this.maxRange = maxRange;
		}
	}

	public class CaseLabelList : Node
	{ 
		public CaseLabel caselabel;
		public CaseLabelList next;

		public CaseLabelList(CaseLabel caselabel, CaseLabelList next)
		{
			this.caselabel = caselabel;
			this.next = next;
		}
	}

	public class CaseSelectorNode : Node
	{
		public CaseLabelList list;
		public Statement stmt;

		public CaseSelectorNode(CaseLabelList list, Statement stmt)
		{
			this.list = list;
			this.stmt = stmt;
		}
	}

	public class CaseSelectorList : Node
	{ 

	}

	public class CaseStatement : Statement
	{
		public ExpressionNode condition;
		public CaseSelectorList selectors;
		public Statement caseelse;

		public CaseStatement(ExpressionNode condition, CaseSelectorList selectors, Statement caseelse)
		{
			this.condition = condition;
			this.selectors = selectors;
			this.caseelse = caseelse;
		}
	}

	public class RepeatStatement : Statement
	{
		public ExpressionNode condition;
		public Statement block;

		public RepeatStatement(Statement block, ExpressionNode condition)
		{
			this.condition = condition;
			this.block = block;
		}
	}

	public class WhileStatement : Statement
	{
		public ExpressionNode condition;
		public Statement block;

		public WhileStatement(ExpressionNode condition, Statement block)
		{
			this.condition = condition;
			this.block = block;
		}
	}

	public enum CallConvention
	{
		Pascal,
		SafeCall,
		StdCall,
		CDecl,
		Register
	}

	public class CallConventionNode : Node
	{
		public CallConvention convention;

		public CallConventionNode(CallConvention convention)
		{
			this.convention = convention;
		}
	}

	public abstract class LiteralNode : Node
	{ 
		
	}

	public abstract class DeclarationNode : Node
	{

	}

	public class UnfinishedNode : Node
	{

		public UnfinishedNode()
		{}
	}

	public class IntegerLiteralNode : LiteralNode
	{
		public int value;

		public IntegerLiteralNode(int value)
		{
			this.value = value;
		}
	}

	public class CharLiteralNode : LiteralNode
	{
		public char value;

		public CharLiteralNode(char value)
		{
			this.value = value;
		}
	}

	public class StringLiteralNode : LiteralNode
	{
		public string value;

		public StringLiteralNode(string value)
		{
			this.value = value;
		}
	}

	public class BoolLiteralNode : LiteralNode
	{
		public bool value;

		public BoolLiteralNode(bool value)
		{
			this.value = value;
		}
	}

	public class RealLiteralNode : LiteralNode
	{
		public double value;

		public RealLiteralNode(double value)
		{
			this.value = value;
		}
	}

	public class NilLiteralNode : LiteralNode
	{
	}


	public class PropertyReadNode : Node
	{
		public IdentifierNode ident;

		public PropertyReadNode(IdentifierNode ident)
		{
			this.ident = ident;
		}
	}

	public class PropertyWriteNode : Node
	{
		public IdentifierNode ident;

		public PropertyWriteNode(IdentifierNode ident)
		{
			this.ident = ident;
		}
	}

	public class PropertySpecifierNode : Node
	{
		public PropertyReadNode read;
		public PropertyWriteNode write;
		// add more here as necessary

		public PropertySpecifierNode(PropertyReadNode read, PropertyWriteNode write)
		{
			this.read = read;
			this.write = write;
		}
	}

	public class FileType : TypeNode
	{
		public TypeNode type;

		public FileType(TypeNode type)
		{
			this.type = type;
		}
	}

	public abstract class LabelNode : Node
	{ 
	}

	public class StringLabel : LabelNode
	{
		IdentifierNode name;

		public StringLabel(IdentifierNode name)
		{
			this.name = name;
		}
	}

	public class NumberLabel : LabelNode
	{
		int number;

		public NumberLabel(int number)
		{
			this.number = number;
		}
	}

	public class LabelDeclarationNode : DeclarationNode
	{
		LabelNode label;
		LabelDeclarationNode next;

		public LabelDeclarationNode(LabelNode label, LabelDeclarationNode next)
		{
			this.label = label;
			this.next = next;
		}
	}

	public class ExportItem : DeclarationNode
	{
		IdentifierNode ident;
		string name;
		ExpressionNode index;

		public ExportItem(IdentifierNode ident, string name, ExpressionNode index)
		{
			this.ident = ident;
			this.name = name;
			this.index = index;
		}
	}

	public class ExportItemListNode : DeclarationNode
	{
		ExportItem export;
		ExportItemListNode next;


	}

	public class TypeDeclarationNode : DeclarationNode
	{
		public IdentifierNode ident;
		public TypeNode type;

		public TypeDeclarationNode(IdentifierNode ident, TypeNode type)
		{
			this.ident = ident;
			this.type = type;
		}
	}

	public class TypeDeclarationListNode : DeclarationNode
	{
		public TypeDeclarationNode decl;
		public TypeDeclarationListNode next;

		public TypeDeclarationListNode(TypeDeclarationNode decl, TypeDeclarationListNode next)
		{
			this.decl = decl;
			this.next = next;
		}
	}

	public class ProcedureTypeDeclarationNode : TypeDeclarationNode
	{
		public ProcedureDirectiveList dirs;

		public ProcedureTypeDeclarationNode(IdentifierNode ident, TypeNode type, ProcedureDirectiveList dirs)
			: base(ident, type)
		{
			this.dirs = dirs;
		}
	}

	public abstract class ParameterQualifierNode : Node	
	{
	}

	public class ConstParameterQualifier : ParameterQualifierNode
	{
	}

	public class VarParameterQualifier : ParameterQualifierNode
	{
	}

	public class OutParameterQualifier : ParameterQualifierNode
	{
	}
	
	public class ParameterNode : Node
	{
		public IdentifierListNode idlist;
		public IdentifierNode type;
		public ParameterQualifierNode qualifier;
		public ExpressionNode init;

		public ParameterNode(ParameterQualifierNode qualifier, IdentifierListNode idlist, IdentifierNode type, ExpressionNode init)
		{
			this.idlist = idlist;
			this.type = type;
			this.qualifier = qualifier;
			this.init = init;
		}
	}

	public class ParameterNodeList : Node
	{
		public ParameterNode param;
		public ParameterNodeList next;

		public ParameterNodeList(ParameterNode param, ParameterNodeList next)
		{
			this.param = param;
			this.next = next;
		}
	}

	public class PointerType : TypeNode
	{
	}

	public class IntegerType : TypeNode
	{
	}

	public class FloatingPointType : TypeNode
	{ 
	}

	public class FloatType : FloatingPointType
	{
	}

	public class DoubleType : FloatingPointType
	{
	}

	public class ExtendedType : FloatingPointType
	{
	}

	public class CurrencyType : FloatingPointType
	{
	}

	public class CharType : TypeNode
	{
	}

	public class BoolType : TypeNode
	{
	}

	public class UnsignedInt8Type : IntegerType // byte
	{
	}

	public class UnsignedInt16Type : IntegerType // word
	{
	}

	public class UnsignedInt32Type : IntegerType // cardinal
	{
	}

	public class UnsignedInt64Type : IntegerType // uint64
	{
	}

	public class SignedInt8Type : IntegerType // smallint
	{
	}

	public class SignedInt16Type : IntegerType // smallint
	{
	}

	public class SignedInt32Type : IntegerType // integer
	{
	}

	public class SignedInt64Type : IntegerType // int64
	{
	}

	public class StringType : TypeNode
	{
		public ExpressionNode size;

		public StringType(ExpressionNode size)
		{
			this.size = size;
		}
	}

	public class LValueNode : Node
	{
		public IdentifierNode ident;

		public LValueNode(IdentifierNode ident)
		{
			this.ident = ident;
		}
	}

	public class StatementBlock : Statement
	{
		public Statement stmt;
		public StatementBlock next;

		public StatementBlock(Statement stmt, StatementBlock next)
		{
			this.stmt = stmt;
			this.next = next;
		}
	}

	public class WithStatement : Statement
	{
		public Statement body;
		public ExpressionNode with;

		public WithStatement(ExpressionNode with, Statement body)
		{
			this.body = body;
			this.with = with;
		}
	}

	public class WhileStatement : Statement
	{
		public ExpressionNode expr;
		public Statement body;

		public WhileStatement(ExpressionNode expr, Statement body)
		{
			this.expr = expr;
			this.body = body;
		}
	}

	public class ForStatement : Statement
	{
		public Statement body;
		public IdentifierNode var;
		public ExpressionNode start;
		public ExpressionNode end;
		public int direction;

		public ForStatement(IdentifierNode var, ExpressionNode start, ExpressionNode end, Statement body)
		{
			this.body = body;
			this.var = var;
			this.start = start;
			this.end = end;
		}
	}

	public class TryFinallyStatement : Statement
	{
		public Statement body;
		public Statement final;

		public TryFinallyStatement(Statement body, Statement final)
		{
			this.body = body;
			this.final = final;
		}
	}

	public class TryExceptStatement : Statement
	{
		public Statement body;
		public Statement final;

		public TryExceptStatement(Statement body, Statement final)
		{
			this.body = body;
			this.final = final;
		}
	}

	public class OperatorNode : Node
	{
		public string op;

		public OperatorNode(string op)
		{
			this.op = op;
		}
	}

	public class ExpressionNode : Node
	{
	}

	public class ExpressionListNode : Node
	{
		ExpressionNode exp;
		ExpressionListNode next;

		public ExpressionListNode(ExpressionNode exp, ExpressionListNode next)
		{
			this.exp = exp;
			this.next = next;
		}
	}

	public class EnumList : DeclarationNode
	{
		public FieldInit element;
		public EnumList next;

		public EnumList(FieldInit element, EnumList next)
		{
			this.element = element;
			this.next = next;
		}
	}

	public class FieldInit : Node
	{
		IdentifierNode ident;
		ExpressionNode expr;

		public FieldInit(IdentifierNode ident, ExpressionNode expr)
		{
			this.ident = ident;
			this.expr = expr;
		}
	}

	public class FieldInitList : Node
	{
		FieldInit init;
		FieldInitList next;

		public FieldInitList(FieldInit init, FieldInitList next)
		{
			this.init = init;
			this.next = next;
		}
	}

	public enum ClassType
	{ 
		Record,
		Object,
		Class
	}

	public class ClassFieldList : Node
	{
		public VarDeclarationNode decl;
		public ClassFieldList next;

		public ClassFieldList(VarDeclarationNode decl, ClassFieldList next)
		{
			this.decl = decl;
			this.next = next;
		}
	}

	public abstract class ClassContent : Node
	{ 

	}

	public class ClassContentList : ClassContent
	{
		public ClassContent content;
		public ClassContentList next;

		public ClassContentList(ClassContent content, ClassContentList next)
		{
			this.content = content;
			this.next = next;
		}
	}

	public class ClassMethod : ClassContent
	{
		public ProcedureHeaderNode decl;

		public ClassMethod(ProcedureHeaderNode decl)
		{
			this.decl = decl;
		}
	}

	public class ClassProperty : ClassContent
	{
		
	}

	public enum Scope
	{
		Public,
		Protected,
		Private,
		Published
	}

	public class ClassStruct : ClassContent
	{
		public Scope scope;
		public ClassFieldList fields;
		public ClassContentList content;

		public ClassStruct(Scope scope, ClassFieldList fields, ClassContentList content)
		{
			this.scope = scope;
			this.fields = fields;
			this.content = content;
		}
	}

	public class ClassDefinition : TypeNode
	{
		public ClassType classType;
		public IdentifierListNode heritage;
		public ClassStruct classStruct;

		public ClassDefinition(ClassType classType, IdentifierListNode heritage, ClassStruct classStruct)
			: base()
		{
			this.classType = classType;
			this.heritage = heritage;
			this.classStruct = classStruct;
		}

	}


	public class InterfaceDefinition : TypeNode
	{
		public IdentifierListNode heritage;
		public ClassContentList methods;
		public ClassContentList properties;

		public InterfaceDefinition(IdentifierListNode heritage, ClassContentList methods, ClassContentList properties)
			: base()
		{			
			this.heritage = heritage;
			this.methods = methods;
			this.properties = properties;
		}

	}

	public class NegationNode : ExpressionNode
	{
		public ExpressionNode exp;

		public NegationNode(ExpressionNode exp)
		{
			this.exp = exp;
		}
	}

	public class AddressNode : ExpressionNode
	{
		public ExpressionNode exp;

		public AddressNode(ExpressionNode exp)
		{
			this.exp = exp;
		}
	}

	public class ArrayAccessNode : Node
	{
		public LValueNode lvalue;
		public ExpressionListNode acessors;

		public ArrayAccessNode(LValueNode lvalue, ExpressionListNode acessors)
		{
			this.lvalue = lvalue;
			this.acessors = acessors;
		}
	}

	public class PointerDereferenceNode : Node
	{
		public ExpressionNode expr;

		public PointerDereferenceNode(ExpressionNode expr)
		{
			this.expr = expr;
		}
	}

	public class TypeNode : Node
	{
		
	}

	public class TypeCastNode : Node
	{
		public ExpressionNode expr;
		public TypeNode type;

		public TypeCastNode(TypeNode type, ExpressionNode expr)
		{
			this.type = type;
			this.expr = expr;
		}
	}

	public class ProcedureCallNode : Statement
	{
		public LValueNode function;
		public ExpressionListNode arguments;

		public ProcedureCallNode(LValueNode function, ExpressionListNode arguments)
		{
			this.function = function;
			this.arguments = arguments;
		}
	}

	public class FieldAcessNode : LValueNode
	{
		public LValueNode obj;
		public IdentifierNode field;

		public FieldAcessNode(LValueNode obj, IdentifierNode field) : base(obj.ident)
		{
			this.obj = obj;
			this.field = field;
		}
	}

	public class IdentifierListNode : Node
	{
		public IdentifierNode ident;
		public IdentifierListNode next;

		public IdentifierListNode(IdentifierNode ident, IdentifierListNode next)
		{
			this.ident = ident;
			this.next = next;
		}
	}

	public class VarDeclarationOption : Node
	{
	}

	public class VariableInitNode : VarDeclarationOption
	{
		public ExpressionNode expr;

		public VariableInitNode(ExpressionNode expr)
		{
			this.expr = expr;
		}
	}

	public class VariableAbsoluteNode : VarDeclarationOption
	{
		public IdentifierNode ident;

		public VariableAbsoluteNode(IdentifierNode ident)
		{
			this.ident = ident;
		}
	}

	public class VarDeclarationNode : DeclarationNode
	{
		public IdentifierListNode ids;
		public TypeNode type;
		public VarDeclarationOption option;

		public VarDeclarationNode(IdentifierListNode ids, TypeNode type, VarDeclarationOption option)
		{
			this.ids = ids;
			this.type = type;
			this.option = option;
		}
	}

	public class VarDeclarationList : DeclarationNode
	{
		public VarDeclarationNode vardecl;
		public VarDeclarationList next;

		public VarDeclarationList(VarDeclarationNode vardecl, VarDeclarationList next)
		{
			this.vardecl = vardecl;
			this.next = next;
		}
	}

	public class ConstDeclarationNode : DeclarationNode
	{
		public IdentifierNode ident;
		public ExpressionNode expr;
		public TypeNode type;

		public ConstDeclarationNode(IdentifierNode ident, TypeNode type, ExpressionNode expr)
		{
			this.ident = ident;
			this.type = type;
			this.expr = expr;
		}
	}

	public class ConstDeclarationList : DeclarationNode
	{
		public ConstDeclarationNode constdecl;
		public ConstDeclarationList next;

		public ConstDeclarationList(ConstDeclarationNode constdecl, ConstDeclarationList next)
		{
			this.constdecl = constdecl;
			this.next = next;
		}
	}

	public class AssemblerListNode : Node
	{
		public string asmop;
		public AssemblerListNode next;

		public AssemblerListNode(string asmop, AssemblerListNode next)
		{
			this.asmop = asmop;
			this.next = next;
		}
	}


	public class ProcedureHeaderNode : Node
	{
		public bool isStatic;
		public FunctionKind kind;
		public IdentifierNode ident;
		public TypeNode returnType;
		public bool obj; 
		public ParameterNodeList parameters;
		public ProcedureDirectiveList directives;

		public ProcedureHeaderNode(FunctionKind kind, IdentifierNode ident, ParameterNodeList parameters, TypeNode returnType, bool obj, ProcedureDirectiveList directives)
		{
			this.isStatic = false;
			this.kind = kind;
			this.ident = ident;
			this.obj = obj;
			this.parameters = parameters;
			this.returnType = returnType;
			this.directives = directives;
		}
	}

	public class ProcedureBodyNode : Node
	{
		public DeclarationListNode decls;
		public StatementBlock body;

		public ProcedureBodyNode(DeclarationListNode decls, StatementBlock body)
		{
			this.decls = decls;
			this.body = body;
		}
	}

	public class AssemblerProcedureBodyNode : ProcedureBodyNode
	{
		AssemblerListNode asm;

		public AssemblerProcedureBodyNode(AssemblerListNode asm) : base(null, null)
		{
			this.asm = asm;
		}
	}

	public class ProcedureDefinitionNode : Node
	{
		public ProcedureHeaderNode header;
		public ProcedureBodyNode body;

		public ProcedureDefinitionNode(ProcedureHeaderNode header, ProcedureBodyNode body)		
		{
			this.header = header;
			this.body = body;
		}
	}

	public enum ProcedureDirectiveEnum
	{ 
		Absolute,
		Abstract,
		Assembler,
		Dynamic,
		Export,
		Inline,
		Override,
		Overload,
		Reintroduce,
		External,
		Forward,
		Virtual,
		VarArgs		
	}

	public class ProcedureDirective : Node
	{
		public ProcedureDirectiveEnum type;

		public ProcedureDirective(ProcedureDirectiveEnum type)
		{
			this.type = type;
		}
	}

	public class ProcedureDirectiveList : Node
	{
		public ProcedureDirective dir;
		public ProcedureDirectiveList next;

		public ProcedureDirectiveList(ProcedureDirective dir, ProcedureDirectiveList next)
		{
			this.dir = dir;
			this.next = next;
		}
	}

	public class ExternalProcedureDirective : ProcedureDirective
	{
		public IdentifierNode importLib;
		public string importName;
		
		public ExternalProcedureDirective(IdentifierNode importLib, string importName) : base (ProcedureDirectiveEnum.External)
		{
			this.importLib = importLib;
			this.importName = importName;
		}
	}

	public class ProcedurePointerDeclarationNode : DeclarationNode
	{
		public IdentifierListNode ids;
		public ProcedureHeaderNode proc;
		public ProcedureDirectiveList dirs;
		
		public ProcedurePointerDeclarationNode(IdentifierListNode ids, ProcedureHeaderNode proc, ProcedureDirectiveList dirs, Node init)
		{
			this.ids = ids;
			this.proc = proc;
			this.dirs = dirs;
			//this.init = init;
		}
	}


	public class UnaryOperationNode : ExpressionNode
	{
		public ExpressionNode a;
		public OperatorNode op;

		public UnaryOperationNode(ExpressionNode a, OperatorNode op)
		{
			this.a = a;
			this.op = op;
		}
	}

	public class BinaryOperationNode : ExpressionNode
	{
		public ExpressionNode a;
		public ExpressionNode b;
		public OperatorNode op;

		public BinaryOperationNode(ExpressionNode a, ExpressionNode b, OperatorNode op)
		{
			this.a = a;
			this.b = b;
			this.op = op;
		}
	}

	public class IdentifierNode : Node
	{
		public string value;

		public IdentifierNode(string val)
		{
			this.value = val;
		}
	}

	public class IdentifierNodeWithLocation : IdentifierNode
	{
		public string location;

		public IdentifierNodeWithLocation(string value, string location) : base(value)
		{
			this.location = location;
		}
	}

	public class UsesNode : Node
	{
		public IdentifierNode ident;
		public UsesNode next;

		public UsesNode(IdentifierNode ident, UsesNode next)
		{ 
			this.ident = ident;
			this.next = next;
		}
	}

	public class BlockWithDeclarationsNode : Node
	{
		public DeclarationNode decls;
		public StatementBlock block;

		public BlockWithDeclarationsNode(DeclarationNode decls, StatementBlock block)
		{
			this.decls = decls;
			this.block = block;
		}
	}

	public class ProgramNode : GoalNode
	{
		public IdentifierNode identifier;
		public BlockWithDeclarationsNode body;
		public UsesNode uses;

		public ProgramNode(IdentifierNode ident, UsesNode uses, BlockWithDeclarationsNode body)
		{
			this.identifier = ident;
			this.uses = uses;
			this.body = body;
		}
	}

	public class LibraryNode: GoalNode
	{
		public IdentifierNode identifier;
		public BlockWithDeclarationsNode body;
		public UsesNode uses;

		public LibraryNode(IdentifierNode ident, UsesNode uses, BlockWithDeclarationsNode body)
		{
			this.identifier = ident;
			this.uses = uses;
			this.body = body;
		}
	}

	public class DeclarationListNode : DeclarationNode
	{
		public DeclarationNode decl;
		public DeclarationListNode next;

		public DeclarationListNode(DeclarationNode decl, DeclarationListNode next)
		{
			this.decl = decl;
			this.next = next;
		}
	}

	public class UnitInterfaceNode: Node
	{
		public UsesNode uses;
		public DeclarationNode decls;

		public UnitInterfaceNode(UsesNode uses, DeclarationNode decls)
		{
			this.uses = uses;
			this.decls = decls;
		}
	}

	public class UnitImplementationNode : Node
	{
		public UsesNode uses;
		public DeclarationNode decls;

		public UnitImplementationNode(UsesNode uses, DeclarationNode decls)
		{
			this.uses = uses;
			this.decls = decls;
		}
	}

	public class UnitInitialization : Node
	{
		public Statement initialization;
		public Statement finalization;

		public UnitInitialization(Statement initialization, Statement finalization)
		{
			this.initialization = initialization;
			this.finalization = finalization;
		}
	}

	public class UnitNode : GoalNode
	{
		public IdentifierNode identifier;		
		public UnitInterfaceNode interfce;
		public UnitImplementationNode implementation;
		public Node init;

		public UnitNode(IdentifierNode ident, UnitInterfaceNode interfce, UnitImplementationNode impl, Node init)
		{
			this.identifier = ident;
			this.interfce = interfce;
			this.implementation = impl;
			this.init = init;
		}
	}

	public class PackageNode : GoalNode
	{
		public IdentifierNode identifier;
		public UsesNode requires;
		public UsesNode contains;

		public PackageNode(IdentifierNode ident, UsesNode requires, UsesNode contains)
		{
			this.identifier = ident;
			this.requires = requires;
			this.contains = contains;
		}
	}


	public class SetElement : Node
	{
		public ExpressionNode min;
		public ExpressionNode max;

		public SetElement(ExpressionNode min, ExpressionNode max)
		{
			this.min = min;
			this.max = max;
		}

	}

	public class SetList : Node
	{
		public SetElement element;
		public SetList next;

		public SetList(SetElement element, SetList next)
		{
			this.element = element;
			this.next = next;
		}

	}


}
