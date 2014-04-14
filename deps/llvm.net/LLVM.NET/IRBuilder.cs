using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LLVM
{
    public unsafe class IRBuilder : IDisposable, IPointerWrapper
    {
        protected LLVMBuilderRef* m_builder;

        public IRBuilder()
        {
            m_builder = Native.CreateBuilder();
        }

        ~IRBuilder()
        {
            Dispose(false);
        }
	
		#region IDisposable Members
		
		public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Dispose(bool disposing)
        {
            if(m_builder != null)
            {
                Native.DisposeBuilder(m_builder);
                m_builder = null;
            }
        }

        #endregion

        #region IPointerWrapper Members

        IntPtr IPointerWrapper.NativePointer
        {
            get { return (IntPtr)m_builder; }
        }

        #endregion

		
		#region Instruction builders
		// LLVMCCoreInstructionBuilder Instruction Builders
		// An instruction builder represents a point within a basic block and is
		// the exclusive means of building instructions using the C interface.


		protected const string tmpvarname = "";

		#region Arithmetic and Logical Instructions
		
        public Value BuildAdd(Value lhs, Value rhs, string varName = tmpvarname)
        {
            return new Value(Native.BuildAdd(m_builder, lhs.Handle, rhs.Handle, varName));
        }

        public Value BuildFAdd(Value lhs, Value rhs, string varName = tmpvarname)
        {
            return new Value(Native.BuildFAdd(m_builder, lhs.Handle, rhs.Handle, varName));
        }

        public Value BuildSub(Value lhs, Value rhs, string varName = tmpvarname)
        {
            return new Value(Native.BuildSub(m_builder, lhs.Handle, rhs.Handle, varName));
        }

        public Value BuildFSub(Value lhs, Value rhs, string varName = tmpvarname)
        {
            return new Value(Native.BuildFSub(m_builder, lhs.Handle, rhs.Handle, varName));
        }

        public Value BuildMul(Value lhs, Value rhs, string varName = tmpvarname)
        {
            return new Value(Native.BuildMul(m_builder, lhs.Handle, rhs.Handle, varName));
        }

        public Value BuildFMul(Value lhs, Value rhs, string varName = tmpvarname)
        {
            return new Value(Native.BuildFMul(m_builder, lhs.Handle, rhs.Handle, varName));
        }

        public Value BuildUDiv(Value lhs, Value rhs, string varName = tmpvarname)
        {
            return new Value(Native.BuildUDiv(m_builder, lhs.Handle, rhs.Handle, varName));
        }

        public Value BuildSDiv(Value lhs, Value rhs, string varName = tmpvarname)
        {
            return new Value(Native.BuildSDiv(m_builder, lhs.Handle, rhs.Handle, varName));
        }

        public Value BuildExactSDiv(Value lhs, Value rhs, string varName = tmpvarname)
        {
            return new Value(Native.BuildExactSDiv(m_builder, lhs.Handle, rhs.Handle, varName));
        }

        public Value BuildFDiv(Value lhs, Value rhs, string varName = tmpvarname)
        {
            return new Value(Native.BuildFDiv(m_builder, lhs.Handle, rhs.Handle, varName));
        }

        public Value BuildURem(Value lhs, Value rhs, string varName = tmpvarname)
        {
            return new Value(Native.BuildURem(m_builder, lhs.Handle, rhs.Handle, varName));
        }
		
        public Value BuildSRem(Value lhs, Value rhs, string varName = tmpvarname)
        {
            return new Value(Native.BuildSRem(m_builder, lhs.Handle, rhs.Handle, varName));
        }

        public Value BuildFRem(Value lhs, Value rhs, string varName = tmpvarname)
        {
            return new Value(Native.BuildFRem(m_builder, lhs.Handle, rhs.Handle, varName));
        }

        public Value BuildShl(Value lhs, Value rhs, string varName = tmpvarname)
        {
            return new Value(Native.BuildShl(m_builder, lhs.Handle, rhs.Handle, varName));
        }

        public Value BuildLShr(Value lhs, Value rhs, string varName = tmpvarname)
        {
            return new Value(Native.BuildLShr(m_builder, lhs.Handle, rhs.Handle, varName));
        }

        public Value BuildAShr(Value lhs, Value rhs, string varName = tmpvarname)
        {
            return new Value(Native.BuildAShr(m_builder, lhs.Handle, rhs.Handle, varName));
        }

        public Value BuildAnd(Value lhs, Value rhs, string varName = tmpvarname)
        {
            return new Value(Native.BuildAnd(m_builder, lhs.Handle, rhs.Handle, varName));
        }

        public Value BuildOr(Value lhs, Value rhs, string varName = tmpvarname)
        {
            return new Value(Native.BuildOr(m_builder, lhs.Handle, rhs.Handle, varName));
        }

        public Value BuildXor(Value lhs, Value rhs, string varName = tmpvarname)
        {
            return new Value(Native.BuildXor(m_builder, lhs.Handle, rhs.Handle, varName));
        }
	
        public Value BuildNeg(Value val, string varName = tmpvarname)
        {
            return new Value(Native.BuildNeg(m_builder, val.Handle, varName));
        }
		
        public Value BuildFNeg(Value val, string varName = tmpvarname)
        {
            return new Value(Native.BuildFNeg(m_builder, val.Handle, varName));
        }
		
        public Value BuildNot(Value val, string varName = tmpvarname)
        {
            return new Value(Native.BuildNot(m_builder, val.Handle, varName));
        }
	
		#endregion
		
		
		#region Comparing and Branching instructions

		public Value BuildICmp(Value lhs, LLVMIntPredicate predicate, Value rhs, string varName = tmpvarname)
        {
            return new Value(Native.BuildICmp(m_builder, predicate, lhs.Handle, rhs.Handle, varName));
        }

        public Value BuildFCmp(Value lhs, LLVMRealPredicate predicate, Value rhs, string varName = tmpvarname)
        {
            return new Value(Native.BuildFCmp(m_builder, predicate, lhs.Handle, rhs.Handle, varName));
        }

        public Value BuildFCmpAndPromote(Value lhs, LLVMRealPredicate predicate, Value rhs, TypeRef promoteType)
        {
            lhs = BuildFCmp(lhs, predicate, rhs);
            return new Value(Native.BuildUIToFP(m_builder, lhs.Handle, promoteType.Handle, "promotetmp")); 
        }

        public Value BuildCondBr(Value ifVal, BasicBlock thenBlock, BasicBlock elseBlock)
        {
            return new Value(Native.BuildCondBr(m_builder, ifVal.Handle, thenBlock.BBHandle, elseBlock.BBHandle));
        }

        public Value BuildBr(BasicBlock branchBlock)
        {
            return new Value(Native.BuildBr(m_builder, branchBlock.BBHandle));
        }
		
		#endregion

		public Value BuildAlloca(TypeRef arg, string name = tmpvarname)
		{
			return new Value(Native.BuildAlloca(m_builder, arg.Handle, name));
		}

		public Value BuildCall(Function func, IEnumerable<Value> args, string varName = tmpvarname)
        {
            IntPtr[] argVals = LLVMHelper.MarshallPointerArray(args);

            return new Value(Native.BuildCall(m_builder, func.Handle, argVals, (uint)argVals.Length, varName));
        }

        public Value BuildLoad(Value value, string varName = tmpvarname)
        {
            return new Value(Native.BuildLoad(m_builder, value.Handle, varName));
        }

        public Value BuildStore(Value src, Value dest)
        {
            return new Value(Native.BuildStore(m_builder, src.Handle, dest.Handle));
        }

		public Value BuildEntryBlockAlloca(Function function, TypeRef type, string varName = tmpvarname)
        {
            LLVMBasicBlockRef* block = Native.GetInsertBlock(m_builder);
            LLVMBasicBlockRef* entry = Native.GetEntryBasicBlock(function.Handle);
            Native.PositionBuilderAtEnd(m_builder, entry);
            LLVMValueRef* alloca = Native.BuildAlloca(m_builder, type.Handle, varName);
            Native.PositionBuilderAtEnd(m_builder, block);

            return new Value(alloca);
        }

        public Value BuildReturn()
        {
            return new Value(Native.BuildRetVoid(m_builder));
        }

        public Value BuildReturn(Value returnValue)
        {
            return new Value(Native.BuildRet(m_builder, returnValue.Handle));
        }

        public Value BuildReturn(IEnumerable<Value> returnValues)
        {
            IntPtr[] retVals = LLVMHelper.MarshallPointerArray(returnValues);
            return new Value(Native.BuildAggregateRet(m_builder, retVals, (uint)retVals.Length));
        }

        public Value BuildPhi(TypeRef type, string name, PhiIncoming incoming)
        {
            IntPtr[] valPointers = incoming.GetValuePointers();
            IntPtr[] blockPointers = incoming.GetBlockPointers();

            LLVMValueRef* phi = Native.BuildPhi(m_builder, type.Handle, name);
            Native.AddIncoming(phi, valPointers, blockPointers, (uint)valPointers.Length);

            return new Value(phi);
        }

        public void AddPhiIncoming(Value phiNode, Value value, BasicBlock block)
        {
            IntPtr[] valPointers = new IntPtr[] { (IntPtr)value.Handle };
            IntPtr[] blockPointers = new IntPtr[] { (IntPtr)block.BBHandle };

            Native.AddIncoming(phiNode.Handle, valPointers, blockPointers, 1);
        }

        public void SetInsertPoint(BasicBlock bb)
        {
            Native.PositionBuilderAtEnd(m_builder, bb.BBHandle);
        }

        public BasicBlock GetInsertPoint()
        {
            LLVMBasicBlockRef* bb = Native.GetInsertBlock(m_builder);
            if(bb == null)
                return null;

            return new BasicBlock(null, bb);
        }

        public void ClearInsertPoint()
        {
            Native.ClearInsertionPosition(m_builder);
		}

		#endregion	// instructions
	}
}
