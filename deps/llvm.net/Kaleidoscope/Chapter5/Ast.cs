using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LLVM;

namespace Kaleidoscope.Chapter5
{
    /// ExprAST - Base class for all expression nodes.
    abstract class ExprAST
    {
        public abstract Value CodeGen(IRBuilder builder);
    }

    /// NumberExprAST - Expression class for numeric literals like "1.0".
    class NumberExprAST : ExprAST
    {
        public double Val { get; set; }

        public NumberExprAST(double val) 
        {
            this.Val = val;
        }

        public override Value CodeGen(IRBuilder builder)
        {
            return Value.CreateConstReal(TypeRef.CreateDouble(), this.Val);
        }
    }

    /// VariableExprAST - Expression class for referencing a variable, like "a".
    class VariableExprAST : ExprAST 
    {
        public string Name { get; set; }

        public VariableExprAST(string name)
        {
            this.Name = name;
        }

        public override Value CodeGen(IRBuilder builder)
        {
            Value value = Value.Null;

            if(!CodeGenManager.NamedValues.TryGetValue(this.Name, out value))
                CodeGenManager.ErrorOutput.WriteLine("Unknown variable name.");

            return value;
        }
    }

    /// BinaryExprAST - Expression class for a binary operator.
    class BinaryExprAST : ExprAST 
    {
        public char Op { get; set; }
        public ExprAST LHS { get; set; }
        public ExprAST RHS { get; set; }

        public BinaryExprAST(char op, ExprAST lhs, ExprAST rhs) 
        {
            this.Op = op;
            this.LHS = lhs;
            this.RHS = rhs;
        }

        public override Value CodeGen(IRBuilder builder)
        {
            Value l = this.LHS.CodeGen(builder);
            Value r = this.RHS.CodeGen(builder);
            if(l.IsNull || r.IsNull) return Value.Null;

            switch(this.Op)
            {
                case '+':
                    return builder.BuildFAdd(l, r);
                case '-':
                    return builder.BuildFSub(l, r);
                case '*':
                    return builder.BuildFMul(l, r);
                case '<':
                    // Convert bool 0/1 to double 0.0 or 1.0
                    return builder.BuildFCmpAndPromote(l, LLVMRealPredicate.RealULT, 
                                                       r, TypeRef.CreateDouble());
            }

            CodeGenManager.ErrorOutput.WriteLine("Unknown binary operator.");
            return Value.Null;
        }
    }

    /// CallExprAST - Expression class for function calls.
    class CallExprAST : ExprAST 
    {
        public string Callee { get; set; }
        public List<ExprAST> Args { get; private set; }

        public CallExprAST(string callee, IEnumerable<ExprAST> args)
        {
            this.Callee = callee;
            this.Args = new List<ExprAST>(args);
        }

        public override Value CodeGen(IRBuilder builder)
        {
            // Look up the name in the global module table.
            Function func = CodeGenManager.Module.GetFunction(this.Callee);
            if(func == null)
            {
                CodeGenManager.ErrorOutput.WriteLine("Unknown function referenced.");
                return Value.Null;
            }

            // If argument mismatch error.
            if(func.ArgCount != Args.Count)
            {
                CodeGenManager.ErrorOutput.WriteLine("Incorrect # arguments passed.");
                return Value.Null;
            }

            List<Value> args = new List<Value>();
            foreach(var arg in this.Args)
            {
                Value val = arg.CodeGen(builder);
                if(val.IsNull)
                    return Value.Null;

                args.Add(val);
            }

            return builder.BuildCall(func, args.ToArray());
        }
    }

    /// IfExprAST - Expression class for if/then/else.
    class IfExprAST : ExprAST
    {
        public ExprAST Cond { get; set; }
        public ExprAST Then { get; set; }
        public ExprAST Else { get; set; }

        public IfExprAST(ExprAST condExpr, ExprAST thenExpr, ExprAST elseExpr)
        {
            this.Cond = condExpr;
            this.Then = thenExpr;
            this.Else = elseExpr;
        }

        public override Value CodeGen(IRBuilder builder)
        {
            Value condV = this.Cond.CodeGen(builder);
            if(condV.IsNull) return condV;

            condV = builder.BuildFCmp(condV, LLVMRealPredicate.RealONE, 
                                      Value.CreateConstDouble(0));
            
            BasicBlock startBlock = builder.GetInsertPoint();
            Function func = startBlock.GetParent();

            BasicBlock thenBB = func.AppendBasicBlock("then");
            builder.SetInsertPoint(thenBB);

            Value thenV = this.Then.CodeGen(builder);
            if(thenV.IsNull) return thenV;
      
            /* Codegen of 'then' can change the current block, update then_bb for the
            * phi. We create a new name because one is used for the phi node, and the
            * other is used for the conditional branch. */
            BasicBlock newThenBB = builder.GetInsertPoint();

            // Emit else block
            BasicBlock elseBB = func.AppendBasicBlock("else");
            func.AppendBasicBlock(elseBB);
            builder.SetInsertPoint(elseBB);

            Value elseV = this.Else.CodeGen(builder);
            if(elseV.IsNull) return elseV;

            // Codegen of 'Else' can change the current block, update ElseBB for the PHI.
            BasicBlock newElseBB = builder.GetInsertPoint();

            // Emit merge block
            BasicBlock mergeBB = func.AppendBasicBlock("ifcont");
            func.AppendBasicBlock(mergeBB);
            builder.SetInsertPoint(mergeBB);

            PhiIncoming incoming = new PhiIncoming();
            incoming.Add(thenV, thenBB);
            incoming.Add(elseV, elseBB);
            Value phi = builder.BuildPhi(TypeRef.CreateDouble(), "iftmp", incoming);

            builder.SetInsertPoint(startBlock);
            builder.BuildCondBr(condV, thenBB, elseBB);

            builder.SetInsertPoint(thenBB);
            builder.BuildBr(mergeBB);

            builder.SetInsertPoint(elseBB);
            builder.BuildBr(mergeBB);

            builder.SetInsertPoint(mergeBB);

            return phi;
        }
    }

    /// forexpr ::= 'for' identifier '=' expr ',' expr (',' expr)? 'in' expression
    class ForExprAST : ExprAST
    {
        public string VarName { get; set; }
        public ExprAST Start { get; set; }
        public ExprAST End { get; set; }
        public ExprAST Step { get; set; }
        public ExprAST Body { get; set; }

        public ForExprAST(string varName, ExprAST startExpr, ExprAST endExpr, ExprAST stepExpr, ExprAST bodyExpr)
        {
            this.VarName = varName;
            this.Start = startExpr;
            this.End = endExpr;
            this.Step = stepExpr;
            this.Body = bodyExpr;
        }

        public override Value CodeGen(IRBuilder builder)
        {
            Value startV = this.Start.CodeGen(builder);
            if(startV.IsNull) return startV;

            BasicBlock startBlock = builder.GetInsertPoint();
            Function func = startBlock.GetParent();

            BasicBlock loopBB = func.AppendBasicBlock("loop");
            builder.BuildBr(loopBB);
            builder.SetInsertPoint(loopBB);

            Value variable = builder.BuildPhi(TypeRef.CreateDouble(), this.VarName, new PhiIncoming(startV, startBlock));

            /* Within the loop, the variable is defined equal to the PHI node. If it
            * shadows an existing variable, we have to restore it, so save it
            * now. */
            Value oldVal = Value.Null;
            CodeGenManager.NamedValues.TryGetValue(this.VarName, out oldVal);
            CodeGenManager.NamedValues[this.VarName] = variable;

            /* Emit the body of the loop.  This, like any other expr, can change the
            * current BB.  Note that we ignore the value computed by the body, but
            * don't allow an error */
            Body.CodeGen(builder);

            // Emit the step value;
            Value stepV = Value.Null;

            if(this.Step != null)
                stepV = this.Step.CodeGen(builder);
            else
                stepV = Value.CreateConstDouble(1);

            Value nextVar = builder.BuildFAdd(variable, stepV, "nextvar");

            // Compute the end condition
            Value endCond = this.End.CodeGen(builder);
            endCond = builder.BuildFCmp(endCond, LLVMRealPredicate.RealONE, Value.CreateConstDouble(0), "loopcond");

            BasicBlock loopEndBB = builder.GetInsertPoint();
            BasicBlock afterBB = func.AppendBasicBlock("afterloop");
            builder.BuildCondBr(endCond, loopBB, afterBB);
            builder.SetInsertPoint(afterBB);

            builder.AddPhiIncoming(variable, nextVar, loopEndBB);

            if(!oldVal.IsNull)
                CodeGenManager.NamedValues[this.VarName] = oldVal;

            return Value.CreateConstDouble(0);
        }
    }

    /// PrototypeAST - This class represents the "prototype" for a function,
    /// which captures its name, and its argument names (thus implicitly the number
    /// of arguments the function takes).
    class PrototypeAST 
    {
        public string Name { get; set; }
        public List<string> Args { get; private set; }

        public PrototypeAST(string name, IEnumerable<string> args)
        {
            this.Name = name;
            this.Args = new List<string>(args);
        }

        public Function CodeGen(IRBuilder builder)
        {
            List<TypeRef> args = new List<TypeRef>();
            this.Args.ForEach(a => args.Add(TypeRef.CreateDouble()));

            Function func = new Function(CodeGenManager.Module, this.Name,
                                                 TypeRef.CreateDouble(), args.ToArray());
            func.SetLinkage(LLVMLinkage.ExternalLinkage);

            // If F conflicted, there was already something named 'Name'.  If it has a
            // body, don't allow redefinition or reextern.
            if(func.IsDuplicate())
            {
                // Delete the one we just made and get the existing one.
                func.Delete();
                func = CodeGenManager.Module.GetFunction(this.Name);

                // If F already has a body, reject this.
                if(func.HasBody)
                {
                    CodeGenManager.ErrorOutput.WriteLine("redefinition of function.");
                    return null;
                }

                // If F took a different number of args, reject.
                if(func.ArgCount != this.Args.Count)
                {
                    CodeGenManager.ErrorOutput.WriteLine("redefinition of function with different # args.");
                    return null;
                }
            }

            // Set names for all arguments.
            for(int i = 0; i < func.ArgCount; ++i)
            {
                Value val = func.GetParameter((uint)i);
                val.Name = this.Args[i];
                CodeGenManager.NamedValues[this.Args[i]] = val;
            }

            return func;
        }
    }

    /// FunctionAST - This class represents a function definition itself.
    class FunctionAST 
    {
        public PrototypeAST Proto { get; set; }
        public ExprAST Body { get; set; }

        public FunctionAST(PrototypeAST proto, ExprAST body)
        {
            this.Proto = proto;
            this.Body = body;
        }

        public Function CodeGen(IRBuilder builder, PassManager passManager)
        {
            CodeGenManager.NamedValues.Clear();
            Function func = this.Proto.CodeGen(builder);
            if(func == null)
                return null;

            // Create a new basic block to start insertion into.
            BasicBlock bb = func.AppendBasicBlock("entry");
            builder.SetInsertPoint(bb);
            Value retVal = Body.CodeGen(builder);

            if(!retVal.IsNull)
            {
                builder.BuildReturn(retVal);

                // Validate the generated code, checking for consistency.
                func.Validate(LLVMVerifierFailureAction.PrintMessageAction);

                // Optimize the function.
                passManager.Run(func);

                return func;
            }

            // Error reading body, remove function.
            func.Delete();
            return null;
        }
    };
}
