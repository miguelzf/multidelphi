using System;
using System.Runtime.InteropServices;
using System.Text;

namespace LLVM
{
    public enum LLVMAttribute
    {
        ZExtAttribute = 1<<0,
        SExtAttribute = 1<<1,
        NoReturnAttribute = 1<<2,
        InRegAttribute = 1<<3,
        StructRetAttribute = 1<<4,
        NoUnwindAttribute = 1<<5,
        NoAliasAttribute = 1<<6,
        ByValAttribute = 1<<7,
        NestAttribute = 1<<8,
        ReadNoneAttribute = 1<<9,
        ReadOnlyAttribute = 1<<10,
        NoInlineAttribute = 1<<11,
        AlwaysInlineAttribute = 1<<12,
        OptimizeForSizeAttribute = 1<<13,
        StackProtectAttribute = 1<<14,
        StackProtectReqAttribute = 1<<15,
        Alignment = 31<<16,
        NoCaptureAttribute = 1<<21,
        NoRedZoneAttribute = 1<<22,
        NoImplicitFloatAttribute = 1<<23,
        NakedAttribute = 1<<24,
        InlineHintAttribute = 1<<25,
        StackAlignment = 7<<26,
        ReturnsTwice = 1 << 29,
        UWTable = 1 << 30,
        NonLazyBind = 1 << 31,
    }

    public enum LLVMOpcode
    {
        Ret = 1,
        Br = 2,
        Switch = 3,
        IndirectBr = 4,
        Invoke = 5,
        Unreachable = 7,
        Add = 8,
        FAdd = 9,
        Sub = 10,
        FSub = 11,
        Mul = 12,
        FMul = 13,
        UDiv = 14,
        SDiv = 15,
        FDiv = 16,
        URem = 17,
        SRem = 18,
        FRem = 19,
        Shl = 20,
        LShr = 21,
        AShr = 22,
        And = 23,
        Or = 24,
        Xor = 25,
        Alloca = 26,
        Load = 27,
        Store = 28,
        GetElementPtr = 29,
        Trunc = 30,
        ZExt = 31,
        SExt = 32,
        FPToUI = 33,
        FPToSI = 34,
        UIToFP = 35,
        SIToFP = 36,
        FPTrunc = 37,
        FPExt = 38,
        PtrToInt = 39,
        IntToPtr = 40,
        BitCast = 41,
        ICmp = 42,
        FCmp = 43,
        PHI = 44,
        Call = 45,
        Select = 46,
        UserOp1 = 47,
        UserOp2 = 48,
        VAArg = 49,
        ExtractElement = 50,
        InsertElement = 51,
        ShuffleVector = 52,
        ExtractValue = 53,
        InsertValue = 54,
        Fence = 55,
        AtomicCmpXchg = 56,
        AtomicRMW = 57,
        Resume = 58,
        LandingPad = 59,
    }

    public enum LLVMTypeKind
    {
        /* type with no size*/
        VoidTypeKind,
        /* 16 bit floating point type*/
        HalfTypeKind,
        /* 32 bit floating point type*/
        FloatTypeKind,
        /* 64 bit floating point type*/
        DoubleTypeKind,
        /* 80 bit floating point type (X87)*/
        X86_FP80TypeKind,
        /* 128 bit floating point type (112-bit mantissa)*/
        FP128TypeKind,
        /* 128 bit floating point type (two 64-bits)*/
        PPC_FP128TypeKind,
        /* Labels*/
        LabelTypeKind,
        /* Arbitrary bit width integers*/
        IntegerTypeKind,
        /* Functions*/
        FunctionTypeKind,
        /* Structures*/
        StructTypeKind,
        /* Arrays*/
        ArrayTypeKind,
        /* Pointers*/
        PointerTypeKind,
        /* SIMD 'packed' format, or other vector type*/
        VectorTypeKind,
        /* Metadata*/
        MetadataTypeKind,
        /* X86 MMX*/
        X86_MMXTypeKind,
    }

    public enum LLVMLinkage
    {
        /* Externally visible function*/
        ExternalLinkage,
        AvailableExternallyLinkage,
        /* Keep one copy of function when linking (inline)*/
        LinkOnceAnyLinkage,
        /* Same, but only replaced by something
                                    equivalent.*/
        LinkOnceODRLinkage,
        /* Keep one copy of function when linking (weak)*/
        WeakAnyLinkage,
        /* Same, but only replaced by something
                                    equivalent.*/
        WeakODRLinkage,
        /* Special purpose, only applies to global arrays*/
        AppendingLinkage,
        /* Rename collisions when linking (static
                                       functions)*/
        InternalLinkage,
        /* Like Internal, but omit from symbol table*/
        PrivateLinkage,
        /* Function to be imported from DLL*/
        DLLImportLinkage,
        /* Function to be accessible from DLL*/
        DLLExportLinkage,
        /* ExternalWeak linkage description*/
        ExternalWeakLinkage,
        /* Obsolete*/
        GhostLinkage,
        /* Tentative definitions*/
        CommonLinkage,
        /* Like Private, but linker removes.*/
        LinkerPrivateLinkage,
        /* Like LinkerPrivate, but is weak.*/
        LinkerPrivateWeakLinkage,
        /* Like LinkerPrivateWeak, but possibly
                                                   hidden.*/
        LinkerPrivateWeakDefAutoLinkage,
    }

    public enum LLVMVisibility
    {
        /* The GV is visible*/
        DefaultVisibility,
        /* The GV is hidden*/
        HiddenVisibility,
        /* The GV is protected*/
        ProtectedVisibility,
    }

    public enum LLVMCallConv
    {
        CCallConv = 0,
        FastCallConv = 8,
        ColdCallConv = 9,
        X86StdcallCallConv = 64,
        X86FastcallCallConv = 65,
    }

    public enum LLVMIntPredicate
    {
        /* equal*/
        IntEQ = 32,
        /* not equal*/
        IntNE,
        /* unsigned greater than*/
        IntUGT,
        /* unsigned greater or equal*/
        IntUGE,
        /* unsigned less than*/
        IntULT,
        /* unsigned less or equal*/
        IntULE,
        /* signed greater than*/
        IntSGT,
        /* signed greater or equal*/
        IntSGE,
        /* signed less than*/
        IntSLT,
        /* signed less or equal*/
        IntSLE,
    }

    public enum LLVMRealPredicate
    {
        /* Always false (always folded)*/
        RealPredicateFalse,
        /* True if ordered and equal*/
        RealOEQ,
        /* True if ordered and greater than*/
        RealOGT,
        /* True if ordered and greater than or equal*/
        RealOGE,
        /* True if ordered and less than*/
        RealOLT,
        /* True if ordered and less than or equal*/
        RealOLE,
        /* True if ordered and operands are unequal*/
        RealONE,
        /* True if ordered (no nans)*/
        RealORD,
        /* True if unordered: isnan(X) | isnan(Y)*/
        RealUNO,
        /* True if unordered or equal*/
        RealUEQ,
        /* True if unordered or greater than*/
        RealUGT,
        /* True if unordered, greater than, or equal*/
        RealUGE,
        /* True if unordered or less than*/
        RealULT,
        /* True if unordered, less than, or equal*/
        RealULE,
        /* True if unordered or not equal*/
        RealUNE,
        /* Always true (always folded)*/
        RealPredicateTrue,
    }

    public enum LLVMLandingPadClauseTy
    {
        /* A catch clause  */
        LandingPadCatch,
        /* A filter clause */
        LandingPadFilter,
    }

    /* @defgroup LLVMCAnalysis Analysis
     * @ingroup LLVMC
     *
     * @{*/
    public enum LLVMVerifierFailureAction
    {
        AbortProcessAction,
        PrintMessageAction,
        ReturnStatusAction,
    }

    /* @defgroup LLVMCTarget Target information
     * @ingroup LLVMC
     *
     * @{*/
    public enum LLVMByteOrdering
    {
        BigEndian,
        LittleEndian,
    }

    /* This provides a C-visible enumerator to manage status codes.
      /// This should map exactly onto the C++ enumerator LTOStatus.*/
    public enum llvm_lto_status
    {
        LTO_UNKNOWN,
        LTO_OPT_SUCCESS,
        LTO_READ_SUCCESS,
        LTO_READ_FAILURE,
        LTO_WRITE_FAILURE,
        LTO_NO_TARGET,
        LTO_NO_WORK,
        LTO_MODULE_MERGE_FAILURE,
        LTO_ASM_FAILURE,
        LTO_NULL_OBJECT,
    }

    public enum LLVMCodeGenOptLevel
    {
        CodeGenLevelNone,
        CodeGenLevelLess,
        CodeGenLevelDefault,
        CodeGenLevelAggressive,
    }

    public enum LLVMRelocMode
    {
        RelocDefault,
        RelocStatic,
        RelocPIC,
        RelocDynamicNoPic,
    }

    public enum LLVMCodeModel
    {
        CodeModelDefault,
        CodeModelJITDefault,
        CodeModelSmall,
        CodeModelKernel,
        CodeModelMedium,
        CodeModelLarge,
    }

    public enum LLVMCodeGenFileType
    {
        AssemblyFile,
        ObjectFile,
    }

    public unsafe struct LLVMTargetLibraryInfoRef {};
    public unsafe struct LLVMExecutionEngineRef {};
    public unsafe struct LLVMUseRef {};
    public unsafe struct LLVMBasicBlockRef {};
    public unsafe struct LLVMPassRegistryRef {};
    public unsafe struct LLVMModuleProviderRef {};
    public unsafe struct LLVMPassManagerRef {};
    public unsafe struct LLVMMemoryBufferRef {};
    public unsafe struct LLVMTargetRef {};
    public unsafe struct LLVMObjectFileRef {};
    public unsafe struct LLVMPassManagerBuilderRef {};
    public unsafe struct LLVMGenericValueRef {};
    public unsafe struct LLVMRelocationIteratorRef {};
    public unsafe struct LLVMTargetMachineRef {};
    public unsafe struct LLVMValueRef {};
    public unsafe struct LLVMSectionIteratorRef {};
    public unsafe struct LLVMSymbolIteratorRef {};
    public unsafe struct LLVMTargetDataRef {};
    public unsafe struct LLVMTypeRef {};
    public unsafe struct LLVMStructLayoutRef {};
    public unsafe struct LLVMContextRef {};
    public unsafe struct LLVMModuleRef {};
    public unsafe struct LLVMBuilderRef {};

    public static unsafe class Native
    {
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMVerifyModule", CallingConvention=CallingConvention.Cdecl)]
        public static extern int VerifyModule(LLVMModuleRef* M, LLVMVerifierFailureAction Action, [MarshalAs(UnmanagedType.LPStr)] ref StringBuilder OutMessage);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMVerifyFunction", CallingConvention=CallingConvention.Cdecl)]
        public static extern int VerifyFunction(LLVMValueRef* Fn, LLVMVerifierFailureAction Action);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMViewFunctionCFG", CallingConvention=CallingConvention.Cdecl)]
        public static extern void ViewFunctionCFG(LLVMValueRef* Fn);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMViewFunctionCFGOnly", CallingConvention=CallingConvention.Cdecl)]
        public static extern void ViewFunctionCFGOnly(LLVMValueRef* Fn);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMParseBitcode", CallingConvention=CallingConvention.Cdecl)]
        public static extern int ParseBitcode(LLVMMemoryBufferRef* MemBuf, ref LLVMModuleRef * OutModule, [MarshalAs(UnmanagedType.LPStr)] ref StringBuilder OutMessage);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMParseBitcodeInContext", CallingConvention=CallingConvention.Cdecl)]
        public static extern int ParseBitcodeInContext(LLVMContextRef* ContextRef, LLVMMemoryBufferRef* MemBuf, ref LLVMModuleRef * OutModule, [MarshalAs(UnmanagedType.LPStr)] ref StringBuilder OutMessage);

        /* Reads a module from the specified path, returning via the OutMP parameter
            a module provider which performs lazy deserialization. Returns 0 on success.
            Optionally returns a human-readable error message via OutMessage.*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMGetBitcodeModuleInContext", CallingConvention=CallingConvention.Cdecl)]
        public static extern int GetBitcodeModuleInContext(LLVMContextRef* ContextRef, LLVMMemoryBufferRef* MemBuf, ref LLVMModuleRef * OutM, [MarshalAs(UnmanagedType.LPStr)] ref StringBuilder OutMessage);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMGetBitcodeModule", CallingConvention=CallingConvention.Cdecl)]
        public static extern int GetBitcodeModule(LLVMMemoryBufferRef* MemBuf, ref LLVMModuleRef * OutM, [MarshalAs(UnmanagedType.LPStr)] ref StringBuilder OutMessage);

        /* Deprecated: Use LLVMGetBitcodeModuleInContext instead.*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMGetBitcodeModuleProviderInContext", CallingConvention=CallingConvention.Cdecl)]
        public static extern int GetBitcodeModuleProviderInContext(LLVMContextRef* ContextRef, LLVMMemoryBufferRef* MemBuf, ref LLVMModuleProviderRef * OutMP, [MarshalAs(UnmanagedType.LPStr)] ref StringBuilder OutMessage);

        /* Deprecated: Use LLVMGetBitcodeModule instead.*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMGetBitcodeModuleProvider", CallingConvention=CallingConvention.Cdecl)]
        public static extern int GetBitcodeModuleProvider(LLVMMemoryBufferRef* MemBuf, ref LLVMModuleProviderRef * OutMP, [MarshalAs(UnmanagedType.LPStr)] ref StringBuilder OutMessage);

        /* @defgroup LLVMCBitWriter Bit Writer
         * @ingroup LLVMC
         *
         * @{
         */
        
        /*===-- Operations on modules ---------------------------------------------===*/
        
        /** Writes a module to the specified path. Returns 0 on success.*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMWriteBitcodeToFile", CallingConvention=CallingConvention.Cdecl)]
        public static extern int WriteBitcodeToFile(LLVMModuleRef* M, [In][MarshalAs(UnmanagedType.LPStr)] string Path);

        /* Writes a module to an open file descriptor. Returns 0 on success.*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMWriteBitcodeToFD", CallingConvention=CallingConvention.Cdecl)]
        public static extern int WriteBitcodeToFD(LLVMModuleRef* M, int FD, int ShouldClose, int Unbuffered);

        /* Deprecated for LLVMWriteBitcodeToFD. Writes a module to an open file
            descriptor. Returns 0 on success. Closes the Handle.*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMWriteBitcodeToFileHandle", CallingConvention=CallingConvention.Cdecl)]
        public static extern int WriteBitcodeToFileHandle(LLVMModuleRef* M, int Handle);

        /* @}*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMInitializeCore", CallingConvention=CallingConvention.Cdecl)]
        public static extern void InitializeCore(LLVMPassRegistryRef* R);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMDisposeMessage", CallingConvention=CallingConvention.Cdecl)]
        public static extern void DisposeMessage([In][MarshalAs(UnmanagedType.LPStr)] string Message);

        /* @defgroup LLVMCCoreContext Contexts
         *
         * Contexts are execution states for the core LLVM IR system.
         *
         * Most types are tied to a context instance. Multiple contexts can
         * exist simultaneously. A single context is not thread safe. However,
         * different contexts can execute on different threads simultaneously.
         *
         * @{
         */
        
        /**
         * Create a new context.
         *
         * Every call to this function should be paired with a call to
         * LLVMContextDispose() or the context will leak memory.*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMContextCreate", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMContextRef* ContextCreate();

        /* Obtain the global context instance.*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMGetGlobalContext", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMContextRef* GetGlobalContext();

        /* Destroy a context instance.
         *
         * This should be called for every call to LLVMContextCreate() or memory
         * will be leaked.*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMContextDispose", CallingConvention=CallingConvention.Cdecl)]
        public static extern void ContextDispose(LLVMContextRef* C);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMGetMDKindIDInContext", CallingConvention=CallingConvention.Cdecl)]
        public static extern uint GetMDKindIDInContext(LLVMContextRef* C, [In][MarshalAs(UnmanagedType.LPStr)] string Name, uint SLen);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMGetMDKindID", CallingConvention=CallingConvention.Cdecl)]
        public static extern uint GetMDKindID([In][MarshalAs(UnmanagedType.LPStr)] string Name, uint SLen);

        /* @}
         */
        
        /**
         * @defgroup LLVMCCoreModule Modules
         *
         * Modules represent the top-level structure in a LLVM program. An LLVM
         * module is effectively a translation unit or a collection of
         * translation units merged together.
         *
         * @{
         */
        
        /**
         * Create a new, empty module in the global context.
         *
         * This is equivalent to calling LLVMModuleCreateWithNameInContext with
         * LLVMGetGlobalContext() as the context parameter.
         *
         * Every invocation should be paired with LLVMDisposeModule() or memory
         * will be leaked.*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMModuleCreateWithName", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMModuleRef* ModuleCreateWithName([In][MarshalAs(UnmanagedType.LPStr)] string ModuleID);

        /* Create a new, empty module in a specific context.
         *
         * Every invocation should be paired with LLVMDisposeModule() or memory
         * will be leaked.*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMModuleCreateWithNameInContext", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMModuleRef* ModuleCreateWithNameInContext([In][MarshalAs(UnmanagedType.LPStr)] string ModuleID, LLVMContextRef* C);

        /* Destroy a module instance.
         *
         * This must be called for every created module or memory will be
         * leaked.*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMDisposeModule", CallingConvention=CallingConvention.Cdecl)]
        public static extern void DisposeModule(LLVMModuleRef* M);

        /* Obtain the data layout for a module.
         *
         * @see Module::getDataLayout()*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMGetDataLayout", CallingConvention=CallingConvention.Cdecl)]
        public static extern System.IntPtr GetDataLayout(LLVMModuleRef* M);

        /* Set the data layout for a module.
         *
         * @see Module::setDataLayout()*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMSetDataLayout", CallingConvention=CallingConvention.Cdecl)]
        public static extern void SetDataLayout(LLVMModuleRef* M, [In][MarshalAs(UnmanagedType.LPStr)] string Triple);

        /* Obtain the target triple for a module.
         *
         * @see Module::getTargetTriple()*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMGetTarget", CallingConvention=CallingConvention.Cdecl)]
        public static extern System.IntPtr GetTarget(LLVMModuleRef* M);

        /* Set the target triple for a module.
         *
         * @see Module::setTargetTriple()*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMSetTarget", CallingConvention=CallingConvention.Cdecl)]
        public static extern void SetTarget(LLVMModuleRef* M, [In][MarshalAs(UnmanagedType.LPStr)] string Triple);

        /* Dump a representation of a module to stderr.
         *
         * @see Module::dump()*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMDumpModule", CallingConvention=CallingConvention.Cdecl)]
        public static extern void DumpModule(LLVMModuleRef* M);

        /* Set inline assembly for a module.
         *
         * @see Module::setModuleInlineAsm()*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMSetModuleInlineAsm", CallingConvention=CallingConvention.Cdecl)]
        public static extern void SetModuleInlineAsm(LLVMModuleRef* M, [In][MarshalAs(UnmanagedType.LPStr)] string Asm);

        /* Obtain the context to which this module is associated.
         *
         * @see Module::getContext()*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMGetModuleContext", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMContextRef* GetModuleContext(LLVMModuleRef* M);

        /* Obtain a Type from a module by its registered name.*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMGetTypeByName", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMTypeRef* GetTypeByName(LLVMModuleRef* M, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        /* Obtain the number of operands for named metadata in a module.
         *
         * @see llvm::Module::getNamedMetadata()*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMGetNamedMetadataNumOperands", CallingConvention=CallingConvention.Cdecl)]
        public static extern uint GetNamedMetadataNumOperands(LLVMModuleRef* M, [In][MarshalAs(UnmanagedType.LPStr)] string name);

        /* Obtain the named metadata operands for a module.
         *
         * The passed LLVMValueRef pointer should refer to an array of
         * LLVMValueRef at least LLVMGetNamedMetadataNumOperands long. This
         * array will be populated with the LLVMValueRef instances. Each
         * instance corresponds to a llvm::MDNode.
         *
         * @see llvm::Module::getNamedMetadata()
         * @see llvm::MDNode::getOperand()*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMGetNamedMetadataOperands", CallingConvention=CallingConvention.Cdecl)]
        public static extern void GetNamedMetadataOperands(LLVMModuleRef* M, [In][MarshalAs(UnmanagedType.LPStr)] string name, System.IntPtr[] Dest);

        /* Add an operand to named metadata.
         *
         * @see llvm::Module::getNamedMetadata()
         * @see llvm::MDNode::addOperand()*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMAddNamedMetadataOperand", CallingConvention=CallingConvention.Cdecl)]
        public static extern void AddNamedMetadataOperand(LLVMModuleRef* M, [In][MarshalAs(UnmanagedType.LPStr)] string name, LLVMValueRef* Val);

        /* Add a function to a module under a specified name.
         *
         * @see llvm::Function::Create()*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMAddFunction", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* AddFunction(LLVMModuleRef* M, [In][MarshalAs(UnmanagedType.LPStr)] string Name, LLVMTypeRef* FunctionTy);

        /* Obtain a Function value from a Module by its name.
         *
         * The returned value corresponds to a llvm::Function value.
         *
         * @see llvm::Module::getFunction()*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMGetNamedFunction", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* GetNamedFunction(LLVMModuleRef* M, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        /* Obtain an iterator to the first Function in a Module.
         *
         * @see llvm::Module::begin()*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMGetFirstFunction", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* GetFirstFunction(LLVMModuleRef* M);

        /* Obtain an iterator to the last Function in a Module.
         *
         * @see llvm::Module::end()*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMGetLastFunction", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* GetLastFunction(LLVMModuleRef* M);

        /* Advance a Function iterator to the next Function.
         *
         * Returns NULL if the iterator was already at the end and there are no more
         * functions.*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMGetNextFunction", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* GetNextFunction(LLVMValueRef* Fn);

        /* Decrement a Function iterator to the previous Function.
         *
         * Returns NULL if the iterator was already at the beginning and there are
         * no previous functions.*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMGetPreviousFunction", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* GetPreviousFunction(LLVMValueRef* Fn);

        /* @}
         */
        
        /**
         * @defgroup LLVMCCoreType Types
         *
         * Types represent the type of a value.
         *
         * Types are associated with a context instance. The context internally
         * deduplicates types so there is only 1 instance of a specific type
         * alive at a time. In other words, a unique type is shared among all
         * consumers within a context.
         *
         * A Type in the C API corresponds to llvm::Type.
         *
         * Types have the following hierarchy:
         *
         *   types:
         *     integer type
         *     real type
         *     function type
         *     sequence types:
         *       array type
         *       pointer type
         *       vector type
         *     void type
         *     label type
         *     opaque type
         *
         * @{
         */
        
        /**
         * Obtain the enumerated type of a Type instance.
         *
         * @see llvm::Type:getTypeID()*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMGetTypeKind", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMTypeKind GetTypeKind(LLVMTypeRef* Ty);

        /* Whether the type has a known size.
         *
         * Things that don't have a size are abstract types, labels, and void.a
         *
         * @see llvm::Type::isSized()*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMTypeIsSized", CallingConvention=CallingConvention.Cdecl)]
        public static extern int TypeIsSized(LLVMTypeRef* Ty);

        /* Obtain the context to which this type instance is associated.
         *
         * @see llvm::Type::getContext()*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMGetTypeContext", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMContextRef* GetTypeContext(LLVMTypeRef* Ty);

        /* @defgroup LLVMCCoreTypeInt Integer Types
         *
         * Functions in this section operate on integer types.
         *
         * @{
         */
        
        /**
         * Obtain an integer type from a context with specified bit width.*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMInt1TypeInContext", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMTypeRef* Int1TypeInContext(LLVMContextRef* C);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMInt8TypeInContext", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMTypeRef* Int8TypeInContext(LLVMContextRef* C);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMInt16TypeInContext", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMTypeRef* Int16TypeInContext(LLVMContextRef* C);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMInt32TypeInContext", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMTypeRef* Int32TypeInContext(LLVMContextRef* C);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMInt64TypeInContext", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMTypeRef* Int64TypeInContext(LLVMContextRef* C);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMIntTypeInContext", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMTypeRef* IntTypeInContext(LLVMContextRef* C, uint NumBits);

        /* Obtain an integer type from the global context with a specified bit
         * width.*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMInt1Type", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMTypeRef* Int1Type();

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMInt8Type", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMTypeRef* Int8Type();

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMInt16Type", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMTypeRef* Int16Type();

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMInt32Type", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMTypeRef* Int32Type();

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMInt64Type", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMTypeRef* Int64Type();

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMIntType", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMTypeRef* IntType(uint NumBits);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMGetIntTypeWidth", CallingConvention=CallingConvention.Cdecl)]
        public static extern uint GetIntTypeWidth(LLVMTypeRef* IntegerTy);

        /* @}
         */
        
        /**
         * @defgroup LLVMCCoreTypeFloat Floating Point Types
         *
         * @{
         */
        
        /**
         * Obtain a 16-bit floating point type from a context.*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMHalfTypeInContext", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMTypeRef* HalfTypeInContext(LLVMContextRef* C);

        /* Obtain a 32-bit floating point type from a context.*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMFloatTypeInContext", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMTypeRef* FloatTypeInContext(LLVMContextRef* C);

        /* Obtain a 64-bit floating point type from a context.*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMDoubleTypeInContext", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMTypeRef* DoubleTypeInContext(LLVMContextRef* C);

        /* Obtain a 80-bit floating point type (X87) from a context.*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMX86FP80TypeInContext", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMTypeRef* X86FP80TypeInContext(LLVMContextRef* C);

        /* Obtain a 128-bit floating point type (112-bit mantissa) from a
         * context.*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMFP128TypeInContext", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMTypeRef* FP128TypeInContext(LLVMContextRef* C);

        /* Obtain a 128-bit floating point type (two 64-bits) from a context.*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMPPCFP128TypeInContext", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMTypeRef* PPCFP128TypeInContext(LLVMContextRef* C);

        /* Obtain a floating point type from the global context.
         *
         * These map to the functions in this group of the same name.*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMHalfType", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMTypeRef* HalfType();

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMFloatType", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMTypeRef* FloatType();

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMDoubleType", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMTypeRef* DoubleType();

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMX86FP80Type", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMTypeRef* X86FP80Type();

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMFP128Type", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMTypeRef* FP128Type();

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMPPCFP128Type", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMTypeRef* PPCFP128Type();

        /* @}
         */
        
        /**
         * @defgroup LLVMCCoreTypeFunction Function Types
         *
         * @{
         */
        
        /**
         * Obtain a function type consisting of a specified signature.
         *
         * The function is defined as a tuple of a return Type, a list of
         * parameter types, and whether the function is variadic.*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMFunctionType", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMTypeRef* FunctionType(LLVMTypeRef* ReturnType, System.IntPtr[] ParamTypes, uint ParamCount, int IsVarArg);

        /* Returns whether a function type is variadic.*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMIsFunctionVarArg", CallingConvention=CallingConvention.Cdecl)]
        public static extern int IsFunctionVarArg(LLVMTypeRef* FunctionTy);

        /* Obtain the Type this function Type returns.*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMGetReturnType", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMTypeRef* GetReturnType(LLVMTypeRef* FunctionTy);

        /* Obtain the number of parameters this function accepts.*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMCountParamTypes", CallingConvention=CallingConvention.Cdecl)]
        public static extern uint CountParamTypes(LLVMTypeRef* FunctionTy);

        /* Obtain the types of a function's parameters.
         *
         * The Dest parameter should point to a pre-allocated array of
         * LLVMTypeRef at least LLVMCountParamTypes() large. On return, the
         * first LLVMCountParamTypes() entries in the array will be populated
         * with LLVMTypeRef instances.
         *
         * @param FunctionTy The function type to operate on.
         * @param Dest Memory address of an array to be filled with result.*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMGetParamTypes", CallingConvention=CallingConvention.Cdecl)]
        public static extern void GetParamTypes(LLVMTypeRef* FunctionTy, System.IntPtr[] Dest);

        /* @}
         */
        
        /**
         * @defgroup LLVMCCoreTypeStruct Structure Types
         *
         * These functions relate to LLVMTypeRef instances.
         *
         * @see llvm::StructType
         *
         * @{
         */
        
        /**
         * Create a new structure type in a context.
         *
         * A structure is specified by a list of inner elements/types and
         * whether these can be packed together.
         *
         * @see llvm::StructType::create()*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMStructTypeInContext", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMTypeRef* StructTypeInContext(LLVMContextRef* C, System.IntPtr[] ElementTypes, uint ElementCount, int Packed);

        /* Create a new structure type in the global context.
         *
         * @see llvm::StructType::create()*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMStructType", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMTypeRef* StructType(System.IntPtr[] ElementTypes, uint ElementCount, int Packed);

        /* Create an empty structure in a context having a specified name.
         *
         * @see llvm::StructType::create()*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMStructCreateNamed", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMTypeRef* StructCreateNamed(LLVMContextRef* C, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        /* Obtain the name of a structure.
         *
         * @see llvm::StructType::getName()*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMGetStructName", CallingConvention=CallingConvention.Cdecl)]
        public static extern System.IntPtr GetStructName(LLVMTypeRef* Ty);

        /* Set the contents of a structure type.
         *
         * @see llvm::StructType::setBody()*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMStructSetBody", CallingConvention=CallingConvention.Cdecl)]
        public static extern void StructSetBody(LLVMTypeRef* StructTy, System.IntPtr[] ElementTypes, uint ElementCount, int Packed);

        /* Get the number of elements defined inside the structure.
         *
         * @see llvm::StructType::getNumElements()*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMCountStructElementTypes", CallingConvention=CallingConvention.Cdecl)]
        public static extern uint CountStructElementTypes(LLVMTypeRef* StructTy);

        /* Get the elements within a structure.
         *
         * The function is passed the address of a pre-allocated array of
         * LLVMTypeRef at least LLVMCountStructElementTypes() long. After
         * invocation, this array will be populated with the structure's
         * elements. The objects in the destination array will have a lifetime
         * of the structure type itself, which is the lifetime of the context it
         * is contained in.*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMGetStructElementTypes", CallingConvention=CallingConvention.Cdecl)]
        public static extern void GetStructElementTypes(LLVMTypeRef* StructTy, System.IntPtr[] Dest);

        /* Determine whether a structure is packed.
         *
         * @see llvm::StructType::isPacked()*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMIsPackedStruct", CallingConvention=CallingConvention.Cdecl)]
        public static extern int IsPackedStruct(LLVMTypeRef* StructTy);

        /* Determine whether a structure is opaque.
         *
         * @see llvm::StructType::isOpaque()*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMIsOpaqueStruct", CallingConvention=CallingConvention.Cdecl)]
        public static extern int IsOpaqueStruct(LLVMTypeRef* StructTy);

        /* @}
         */
        
        
        /**
         * @defgroup LLVMCCoreTypeSequential Sequential Types
         *
         * Sequential types represents "arrays" of types. This is a super class
         * for array, vector, and pointer types.
         *
         * @{
         */
        
        /**
         * Obtain the type of elements within a sequential type.
         *
         * This works on array, vector, and pointer types.
         *
         * @see llvm::SequentialType::getElementType()*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMGetElementType", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMTypeRef* GetElementType(LLVMTypeRef* Ty);

        /* Create a fixed size array type that refers to a specific type.
         *
         * The created type will exist in the context that its element type
         * exists in.
         *
         * @see llvm::ArrayType::get()*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMArrayType", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMTypeRef* ArrayType(LLVMTypeRef* ElementType, uint ElementCount);

        /* Obtain the length of an array type.
         *
         * This only works on types that represent arrays.
         *
         * @see llvm::ArrayType::getNumElements()*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMGetArrayLength", CallingConvention=CallingConvention.Cdecl)]
        public static extern uint GetArrayLength(LLVMTypeRef* ArrayTy);

        /* Create a pointer type that points to a defined type.
         *
         * The created type will exist in the context that its pointee type
         * exists in.
         *
         * @see llvm::PointerType::get()*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMPointerType", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMTypeRef* PointerType(LLVMTypeRef* ElementType, uint AddressSpace);

        /* Obtain the address space of a pointer type.
         *
         * This only works on types that represent pointers.
         *
         * @see llvm::PointerType::getAddressSpace()*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMGetPointerAddressSpace", CallingConvention=CallingConvention.Cdecl)]
        public static extern uint GetPointerAddressSpace(LLVMTypeRef* PointerTy);

        /* Create a vector type that contains a defined type and has a specific
         * number of elements.
         *
         * The created type will exist in the context thats its element type
         * exists in.
         *
         * @see llvm::VectorType::get()*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMVectorType", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMTypeRef* VectorType(LLVMTypeRef* ElementType, uint ElementCount);

        /* Obtain the number of elements in a vector type.
         *
         * This only works on types that represent vectors.
         *
         * @see llvm::VectorType::getNumElements()*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMGetVectorSize", CallingConvention=CallingConvention.Cdecl)]
        public static extern uint GetVectorSize(LLVMTypeRef* VectorTy);

        /* @}
         */
        
        /**
         * @defgroup LLVMCCoreTypeOther Other Types
         *
         * @{
         */
        
        /**
         * Create a void type in a context.*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMVoidTypeInContext", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMTypeRef* VoidTypeInContext(LLVMContextRef* C);

        /* Create a label type in a context.*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMLabelTypeInContext", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMTypeRef* LabelTypeInContext(LLVMContextRef* C);

        /* Create a X86 MMX type in a context.*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMX86MMXTypeInContext", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMTypeRef* X86MMXTypeInContext(LLVMContextRef* C);

        /* These are similar to the above functions except they operate on the
         * global context.*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMVoidType", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMTypeRef* VoidType();

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMLabelType", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMTypeRef* LabelType();

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMX86MMXType", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMTypeRef* X86MMXType();

        /* @defgroup LLVMCCoreValueGeneral General APIs
         *
         * Functions in this section work on all LLVMValueRef instances,
         * regardless of their sub-type. They correspond to functions available
         * on llvm::Value.
         *
         * @{
         */
        
        /**
         * Obtain the type of a value.
         *
         * @see llvm::Value::getType()*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMTypeOf", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMTypeRef* TypeOf(LLVMValueRef* Val);

        /* Obtain the string name of a value.
         *
         * @see llvm::Value::getName()*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMGetValueName", CallingConvention=CallingConvention.Cdecl)]
        public static extern System.IntPtr GetValueName(LLVMValueRef* Val);

        /* Set the string name of a value.
         *
         * @see llvm::Value::setName()*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMSetValueName", CallingConvention=CallingConvention.Cdecl)]
        public static extern void SetValueName(LLVMValueRef* Val, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        /* Dump a representation of a value to stderr.
         *
         * @see llvm::Value::dump()*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMDumpValue", CallingConvention=CallingConvention.Cdecl)]
        public static extern void DumpValue(LLVMValueRef* Val);

        /* Replace all uses of a value with another one.
         *
         * @see llvm::Value::replaceAllUsesWith()*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMReplaceAllUsesWith", CallingConvention=CallingConvention.Cdecl)]
        public static extern void ReplaceAllUsesWith(LLVMValueRef* OldVal, LLVMValueRef* NewVal);

        /* Determine whether the specified constant instance is constant.*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMIsConstant", CallingConvention=CallingConvention.Cdecl)]
        public static extern int IsConstant(LLVMValueRef* Val);

        /* Determine whether a value instance is undefined.*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMIsUndef", CallingConvention=CallingConvention.Cdecl)]
        public static extern int IsUndef(LLVMValueRef* Val);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMIsAArgument", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* IsAArgument(LLVMValueRef* Val);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMIsABasicBlock", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* IsABasicBlock(LLVMValueRef* Val);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMIsAInlineAsm", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* IsAInlineAsm(LLVMValueRef* Val);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMIsAMDNode", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* IsAMDNode(LLVMValueRef* Val);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMIsAMDString", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* IsAMDString(LLVMValueRef* Val);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMIsAUser", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* IsAUser(LLVMValueRef* Val);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMIsAConstant", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* IsAConstant(LLVMValueRef* Val);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMIsABlockAddress", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* IsABlockAddress(LLVMValueRef* Val);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMIsAConstantAggregateZero", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* IsAConstantAggregateZero(LLVMValueRef* Val);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMIsAConstantArray", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* IsAConstantArray(LLVMValueRef* Val);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMIsAConstantExpr", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* IsAConstantExpr(LLVMValueRef* Val);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMIsAConstantFP", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* IsAConstantFP(LLVMValueRef* Val);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMIsAConstantInt", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* IsAConstantInt(LLVMValueRef* Val);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMIsAConstantPointerNull", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* IsAConstantPointerNull(LLVMValueRef* Val);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMIsAConstantStruct", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* IsAConstantStruct(LLVMValueRef* Val);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMIsAConstantVector", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* IsAConstantVector(LLVMValueRef* Val);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMIsAGlobalValue", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* IsAGlobalValue(LLVMValueRef* Val);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMIsAFunction", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* IsAFunction(LLVMValueRef* Val);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMIsAGlobalAlias", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* IsAGlobalAlias(LLVMValueRef* Val);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMIsAGlobalVariable", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* IsAGlobalVariable(LLVMValueRef* Val);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMIsAUndefValue", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* IsAUndefValue(LLVMValueRef* Val);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMIsAInstruction", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* IsAInstruction(LLVMValueRef* Val);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMIsABinaryOperator", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* IsABinaryOperator(LLVMValueRef* Val);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMIsACallInst", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* IsACallInst(LLVMValueRef* Val);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMIsAIntrinsicInst", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* IsAIntrinsicInst(LLVMValueRef* Val);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMIsADbgInfoIntrinsic", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* IsADbgInfoIntrinsic(LLVMValueRef* Val);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMIsADbgDeclareInst", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* IsADbgDeclareInst(LLVMValueRef* Val);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMIsAMemIntrinsic", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* IsAMemIntrinsic(LLVMValueRef* Val);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMIsAMemCpyInst", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* IsAMemCpyInst(LLVMValueRef* Val);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMIsAMemMoveInst", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* IsAMemMoveInst(LLVMValueRef* Val);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMIsAMemSetInst", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* IsAMemSetInst(LLVMValueRef* Val);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMIsACmpInst", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* IsACmpInst(LLVMValueRef* Val);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMIsAFCmpInst", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* IsAFCmpInst(LLVMValueRef* Val);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMIsAICmpInst", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* IsAICmpInst(LLVMValueRef* Val);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMIsAExtractElementInst", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* IsAExtractElementInst(LLVMValueRef* Val);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMIsAGetElementPtrInst", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* IsAGetElementPtrInst(LLVMValueRef* Val);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMIsAInsertElementInst", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* IsAInsertElementInst(LLVMValueRef* Val);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMIsAInsertValueInst", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* IsAInsertValueInst(LLVMValueRef* Val);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMIsALandingPadInst", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* IsALandingPadInst(LLVMValueRef* Val);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMIsAPHINode", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* IsAPHINode(LLVMValueRef* Val);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMIsASelectInst", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* IsASelectInst(LLVMValueRef* Val);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMIsAShuffleVectorInst", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* IsAShuffleVectorInst(LLVMValueRef* Val);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMIsAStoreInst", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* IsAStoreInst(LLVMValueRef* Val);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMIsATerminatorInst", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* IsATerminatorInst(LLVMValueRef* Val);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMIsABranchInst", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* IsABranchInst(LLVMValueRef* Val);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMIsAIndirectBrInst", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* IsAIndirectBrInst(LLVMValueRef* Val);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMIsAInvokeInst", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* IsAInvokeInst(LLVMValueRef* Val);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMIsAReturnInst", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* IsAReturnInst(LLVMValueRef* Val);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMIsASwitchInst", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* IsASwitchInst(LLVMValueRef* Val);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMIsAUnreachableInst", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* IsAUnreachableInst(LLVMValueRef* Val);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMIsAResumeInst", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* IsAResumeInst(LLVMValueRef* Val);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMIsAUnaryInstruction", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* IsAUnaryInstruction(LLVMValueRef* Val);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMIsAAllocaInst", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* IsAAllocaInst(LLVMValueRef* Val);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMIsACastInst", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* IsACastInst(LLVMValueRef* Val);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMIsABitCastInst", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* IsABitCastInst(LLVMValueRef* Val);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMIsAFPExtInst", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* IsAFPExtInst(LLVMValueRef* Val);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMIsAFPToSIInst", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* IsAFPToSIInst(LLVMValueRef* Val);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMIsAFPToUIInst", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* IsAFPToUIInst(LLVMValueRef* Val);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMIsAFPTruncInst", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* IsAFPTruncInst(LLVMValueRef* Val);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMIsAIntToPtrInst", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* IsAIntToPtrInst(LLVMValueRef* Val);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMIsAPtrToIntInst", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* IsAPtrToIntInst(LLVMValueRef* Val);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMIsASExtInst", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* IsASExtInst(LLVMValueRef* Val);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMIsASIToFPInst", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* IsASIToFPInst(LLVMValueRef* Val);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMIsATruncInst", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* IsATruncInst(LLVMValueRef* Val);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMIsAUIToFPInst", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* IsAUIToFPInst(LLVMValueRef* Val);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMIsAZExtInst", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* IsAZExtInst(LLVMValueRef* Val);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMIsAExtractValueInst", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* IsAExtractValueInst(LLVMValueRef* Val);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMIsALoadInst", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* IsALoadInst(LLVMValueRef* Val);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMIsAVAArgInst", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* IsAVAArgInst(LLVMValueRef* Val);

        /* @}
         */
        
        /**
         * @defgroup LLVMCCoreValueUses Usage
         *
         * This module defines functions that allow you to inspect the uses of a
         * LLVMValueRef.
         *
         * It is possible to obtain a LLVMUseRef for any LLVMValueRef instance.
         * Each LLVMUseRef (which corresponds to a llvm::Use instance) holds a
         * llvm::User and llvm::Value.
         *
         * @{
         */
        
        /**
         * Obtain the first use of a value.
         *
         * Uses are obtained in an iterator fashion. First, call this function
         * to obtain a reference to the first use. Then, call LLVMGetNextUse()
         * on that instance and all subsequently obtained instances untl
         * LLVMGetNextUse() returns NULL.
         *
         * @see llvm::Value::use_begin()*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMGetFirstUse", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMUseRef* GetFirstUse(LLVMValueRef* Val);

        /* Obtain the next use of a value.
         *
         * This effectively advances the iterator. It returns NULL if you are on
         * the final use and no more are available.*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMGetNextUse", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMUseRef* GetNextUse(LLVMUseRef* U);

        /* Obtain the user value for a user.
         *
         * The returned value corresponds to a llvm::User type.
         *
         * @see llvm::Use::getUser()*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMGetUser", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* GetUser(LLVMUseRef* U);

        /* Obtain the value this use corresponds to.
         *
         * @see llvm::Use::get().*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMGetUsedValue", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* GetUsedValue(LLVMUseRef* U);

        /* @}
         */
        
        /**
         * @defgroup LLVMCCoreValueUser User value
         *
         * Function in this group pertain to LLVMValueRef instances that descent
         * from llvm::User. This includes constants, instructions, and
         * operators.
         *
         * @{
         */
        
        /**
         * Obtain an operand at a specific index in a llvm::User value.
         *
         * @see llvm::User::getOperand()*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMGetOperand", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* GetOperand(LLVMValueRef* Val, uint Index);

        /* Set an operand at a specific index in a llvm::User value.
         *
         * @see llvm::User::setOperand()*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMSetOperand", CallingConvention=CallingConvention.Cdecl)]
        public static extern void SetOperand(LLVMValueRef* User, uint Index, LLVMValueRef* Val);

        /* Obtain the number of operands in a llvm::User value.
         *
         * @see llvm::User::getNumOperands()*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMGetNumOperands", CallingConvention=CallingConvention.Cdecl)]
        public static extern int GetNumOperands(LLVMValueRef* Val);

        /* @}
         */
        
        /**
         * @defgroup LLVMCCoreValueConstant Constants
         *
         * This section contains APIs for interacting with LLVMValueRef that
         * correspond to llvm::Constant instances.
         *
         * These functions will work for any LLVMValueRef in the llvm::Constant
         * class hierarchy.
         *
         * @{
         */
        
        /**
         * Obtain a constant value referring to the null instance of a type.
         *
         * @see llvm::Constant::getNullValue()*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMConstNull", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* ConstNull(LLVMTypeRef* Ty);

        /* Obtain a constant value referring to the instance of a type
         * consisting of all ones.
         *
         * This is only valid for integer types.
         *
         * @see llvm::Constant::getAllOnesValue()*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMConstAllOnes", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* ConstAllOnes(LLVMTypeRef* Ty);

        /* Obtain a constant value referring to an undefined value of a type.
         *
         * @see llvm::UndefValue::get()*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMGetUndef", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* GetUndef(LLVMTypeRef* Ty);

        /* Determine whether a value instance is null.
         *
         * @see llvm::Constant::isNullValue()*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMIsNull", CallingConvention=CallingConvention.Cdecl)]
        public static extern int IsNull(LLVMValueRef* Val);

        /* Obtain a constant that is a constant pointer pointing to NULL for a
         * specified type.*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMConstPointerNull", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* ConstPointerNull(LLVMTypeRef* Ty);

        /* @defgroup LLVMCCoreValueConstantScalar Scalar constants
         *
         * Functions in this group model LLVMValueRef instances that correspond
         * to constants referring to scalar types.
         *
         * For integer types, the LLVMTypeRef parameter should correspond to a
         * llvm::IntegerType instance and the returned LLVMValueRef will
         * correspond to a llvm::ConstantInt.
         *
         * For floating point types, the LLVMTypeRef returned corresponds to a
         * llvm::ConstantFP.
         *
         * @{
         */
        
        /**
         * Obtain a constant value for an integer type.
         *
         * The returned value corresponds to a llvm::ConstantInt.
         *
         * @see llvm::ConstantInt::get()
         *
         * @param IntTy Integer type to obtain value of.
         * @param N The value the returned instance should refer to.
         * @param SignExtend Whether to sign extend the produced value.*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMConstInt", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* ConstInt(LLVMTypeRef* IntTy, ulong N, int SignExtend);

        /* Obtain a constant value for an integer of arbitrary precision.
         *
         * @see llvm::ConstantInt::get()*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMConstIntOfArbitraryPrecision", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* ConstIntOfArbitraryPrecision(LLVMTypeRef* IntTy, uint NumWords, System.IntPtr Words);

        /* Obtain a constant value for an integer parsed from a string.
         *
         * A similar API, LLVMConstIntOfStringAndSize is also available. If the
         * string's length is available, it is preferred to call that function
         * instead.
         *
         * @see llvm::ConstantInt::get()*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMConstIntOfString", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* ConstIntOfString(LLVMTypeRef* IntTy, [In][MarshalAs(UnmanagedType.LPStr)] string Text, byte Radix);

        /* Obtain a constant value for an integer parsed from a string with
         * specified length.
         *
         * @see llvm::ConstantInt::get()*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMConstIntOfStringAndSize", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* ConstIntOfStringAndSize(LLVMTypeRef* IntTy, [In][MarshalAs(UnmanagedType.LPStr)] string Text, uint SLen, byte Radix);

        /* Obtain a constant value referring to a double floating point value.*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMConstReal", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* ConstReal(LLVMTypeRef* RealTy, double N);

        /* Obtain a constant for a floating point value parsed from a string.
         *
         * A similar API, LLVMConstRealOfStringAndSize is also available. It
         * should be used if the input string's length is known.*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMConstRealOfString", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* ConstRealOfString(LLVMTypeRef* RealTy, [In][MarshalAs(UnmanagedType.LPStr)] string Text);

        /* Obtain a constant for a floating point value parsed from a string.*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMConstRealOfStringAndSize", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* ConstRealOfStringAndSize(LLVMTypeRef* RealTy, [In][MarshalAs(UnmanagedType.LPStr)] string Text, uint SLen);

        /* Obtain the zero extended value for an integer constant value.
         *
         * @see llvm::ConstantInt::getZExtValue()*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMConstIntGetZExtValue", CallingConvention=CallingConvention.Cdecl)]
        public static extern ulong ConstIntGetZExtValue(LLVMValueRef* ConstantVal);

        /* Obtain the sign extended value for an integer constant value.
         *
         * @see llvm::ConstantInt::getSExtValue()*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMConstIntGetSExtValue", CallingConvention=CallingConvention.Cdecl)]
        public static extern long ConstIntGetSExtValue(LLVMValueRef* ConstantVal);

        /* @}
         */
        
        /**
         * @defgroup LLVMCCoreValueConstantComposite Composite Constants
         *
         * Functions in this group operate on composite constants.
         *
         * @{
         */
        
        /**
         * Create a ConstantDataSequential and initialize it with a string.
         *
         * @see llvm::ConstantDataArray::getString()*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMConstStringInContext", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* ConstStringInContext(LLVMContextRef* C, [In][MarshalAs(UnmanagedType.LPStr)] string Str, uint Length, int DontNullTerminate);

        /* Create a ConstantDataSequential with string content in the global context.
         *
         * This is the same as LLVMConstStringInContext except it operates on the
         * global context.
         *
         * @see LLVMConstStringInContext()
         * @see llvm::ConstantDataArray::getString()*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMConstString", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* ConstString([In][MarshalAs(UnmanagedType.LPStr)] string Str, uint Length, int DontNullTerminate);

        /* Create an anonymous ConstantStruct with the specified values.
         *
         * @see llvm::ConstantStruct::getAnon()*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMConstStructInContext", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* ConstStructInContext(LLVMContextRef* C, System.IntPtr[] ConstantVals, uint Count, int Packed);

        /* Create a ConstantStruct in the global Context.
         *
         * This is the same as LLVMConstStructInContext except it operates on the
         * global Context.
         *
         * @see LLVMConstStructInContext()*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMConstStruct", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* ConstStruct(System.IntPtr[] ConstantVals, uint Count, int Packed);

        /* Create a ConstantArray from values.
         *
         * @see llvm::ConstantArray::get()*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMConstArray", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* ConstArray(LLVMTypeRef* ElementTy, System.IntPtr[] ConstantVals, uint Length);

        /* Create a non-anonymous ConstantStruct from values.
         *
         * @see llvm::ConstantStruct::get()*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMConstNamedStruct", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* ConstNamedStruct(LLVMTypeRef* StructTy, System.IntPtr[] ConstantVals, uint Count);

        /* Create a ConstantVector from values.
         *
         * @see llvm::ConstantVector::get()*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMConstVector", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* ConstVector(System.IntPtr[] ScalarConstantVals, uint Size);

        /* @}
         */
        
        /**
         * @defgroup LLVMCCoreValueConstantExpressions Constant Expressions
         *
         * Functions in this group correspond to APIs on llvm::ConstantExpr.
         *
         * @see llvm::ConstantExpr.
         *
         * @{*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMGetConstOpcode", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMOpcode GetConstOpcode(LLVMValueRef* ConstantVal);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMAlignOf", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* AlignOf(LLVMTypeRef* Ty);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMSizeOf", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* SizeOf(LLVMTypeRef* Ty);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMConstNeg", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* ConstNeg(LLVMValueRef* ConstantVal);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMConstNSWNeg", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* ConstNSWNeg(LLVMValueRef* ConstantVal);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMConstNUWNeg", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* ConstNUWNeg(LLVMValueRef* ConstantVal);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMConstFNeg", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* ConstFNeg(LLVMValueRef* ConstantVal);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMConstNot", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* ConstNot(LLVMValueRef* ConstantVal);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMConstAdd", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* ConstAdd(LLVMValueRef* LHSConstant, LLVMValueRef* RHSConstant);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMConstNSWAdd", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* ConstNSWAdd(LLVMValueRef* LHSConstant, LLVMValueRef* RHSConstant);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMConstNUWAdd", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* ConstNUWAdd(LLVMValueRef* LHSConstant, LLVMValueRef* RHSConstant);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMConstFAdd", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* ConstFAdd(LLVMValueRef* LHSConstant, LLVMValueRef* RHSConstant);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMConstSub", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* ConstSub(LLVMValueRef* LHSConstant, LLVMValueRef* RHSConstant);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMConstNSWSub", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* ConstNSWSub(LLVMValueRef* LHSConstant, LLVMValueRef* RHSConstant);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMConstNUWSub", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* ConstNUWSub(LLVMValueRef* LHSConstant, LLVMValueRef* RHSConstant);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMConstFSub", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* ConstFSub(LLVMValueRef* LHSConstant, LLVMValueRef* RHSConstant);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMConstMul", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* ConstMul(LLVMValueRef* LHSConstant, LLVMValueRef* RHSConstant);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMConstNSWMul", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* ConstNSWMul(LLVMValueRef* LHSConstant, LLVMValueRef* RHSConstant);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMConstNUWMul", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* ConstNUWMul(LLVMValueRef* LHSConstant, LLVMValueRef* RHSConstant);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMConstFMul", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* ConstFMul(LLVMValueRef* LHSConstant, LLVMValueRef* RHSConstant);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMConstUDiv", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* ConstUDiv(LLVMValueRef* LHSConstant, LLVMValueRef* RHSConstant);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMConstSDiv", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* ConstSDiv(LLVMValueRef* LHSConstant, LLVMValueRef* RHSConstant);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMConstExactSDiv", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* ConstExactSDiv(LLVMValueRef* LHSConstant, LLVMValueRef* RHSConstant);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMConstFDiv", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* ConstFDiv(LLVMValueRef* LHSConstant, LLVMValueRef* RHSConstant);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMConstURem", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* ConstURem(LLVMValueRef* LHSConstant, LLVMValueRef* RHSConstant);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMConstSRem", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* ConstSRem(LLVMValueRef* LHSConstant, LLVMValueRef* RHSConstant);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMConstFRem", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* ConstFRem(LLVMValueRef* LHSConstant, LLVMValueRef* RHSConstant);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMConstAnd", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* ConstAnd(LLVMValueRef* LHSConstant, LLVMValueRef* RHSConstant);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMConstOr", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* ConstOr(LLVMValueRef* LHSConstant, LLVMValueRef* RHSConstant);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMConstXor", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* ConstXor(LLVMValueRef* LHSConstant, LLVMValueRef* RHSConstant);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMConstICmp", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* ConstICmp(LLVMIntPredicate Predicate, LLVMValueRef* LHSConstant, LLVMValueRef* RHSConstant);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMConstFCmp", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* ConstFCmp(LLVMRealPredicate Predicate, LLVMValueRef* LHSConstant, LLVMValueRef* RHSConstant);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMConstShl", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* ConstShl(LLVMValueRef* LHSConstant, LLVMValueRef* RHSConstant);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMConstLShr", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* ConstLShr(LLVMValueRef* LHSConstant, LLVMValueRef* RHSConstant);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMConstAShr", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* ConstAShr(LLVMValueRef* LHSConstant, LLVMValueRef* RHSConstant);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMConstGEP", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* ConstGEP(LLVMValueRef* ConstantVal, System.IntPtr[] ConstantIndices, uint NumIndices);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMConstInBoundsGEP", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* ConstInBoundsGEP(LLVMValueRef* ConstantVal, System.IntPtr[] ConstantIndices, uint NumIndices);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMConstTrunc", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* ConstTrunc(LLVMValueRef* ConstantVal, LLVMTypeRef* ToType);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMConstSExt", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* ConstSExt(LLVMValueRef* ConstantVal, LLVMTypeRef* ToType);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMConstZExt", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* ConstZExt(LLVMValueRef* ConstantVal, LLVMTypeRef* ToType);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMConstFPTrunc", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* ConstFPTrunc(LLVMValueRef* ConstantVal, LLVMTypeRef* ToType);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMConstFPExt", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* ConstFPExt(LLVMValueRef* ConstantVal, LLVMTypeRef* ToType);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMConstUIToFP", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* ConstUIToFP(LLVMValueRef* ConstantVal, LLVMTypeRef* ToType);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMConstSIToFP", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* ConstSIToFP(LLVMValueRef* ConstantVal, LLVMTypeRef* ToType);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMConstFPToUI", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* ConstFPToUI(LLVMValueRef* ConstantVal, LLVMTypeRef* ToType);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMConstFPToSI", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* ConstFPToSI(LLVMValueRef* ConstantVal, LLVMTypeRef* ToType);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMConstPtrToInt", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* ConstPtrToInt(LLVMValueRef* ConstantVal, LLVMTypeRef* ToType);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMConstIntToPtr", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* ConstIntToPtr(LLVMValueRef* ConstantVal, LLVMTypeRef* ToType);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMConstBitCast", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* ConstBitCast(LLVMValueRef* ConstantVal, LLVMTypeRef* ToType);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMConstZExtOrBitCast", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* ConstZExtOrBitCast(LLVMValueRef* ConstantVal, LLVMTypeRef* ToType);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMConstSExtOrBitCast", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* ConstSExtOrBitCast(LLVMValueRef* ConstantVal, LLVMTypeRef* ToType);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMConstTruncOrBitCast", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* ConstTruncOrBitCast(LLVMValueRef* ConstantVal, LLVMTypeRef* ToType);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMConstPointerCast", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* ConstPointerCast(LLVMValueRef* ConstantVal, LLVMTypeRef* ToType);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMConstIntCast", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* ConstIntCast(LLVMValueRef* ConstantVal, LLVMTypeRef* ToType, int isSigned);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMConstFPCast", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* ConstFPCast(LLVMValueRef* ConstantVal, LLVMTypeRef* ToType);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMConstSelect", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* ConstSelect(LLVMValueRef* ConstantCondition, LLVMValueRef* ConstantIfTrue, LLVMValueRef* ConstantIfFalse);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMConstExtractElement", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* ConstExtractElement(LLVMValueRef* VectorConstant, LLVMValueRef* IndexConstant);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMConstInsertElement", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* ConstInsertElement(LLVMValueRef* VectorConstant, LLVMValueRef* ElementValueConstant, LLVMValueRef* IndexConstant);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMConstShuffleVector", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* ConstShuffleVector(LLVMValueRef* VectorAConstant, LLVMValueRef* VectorBConstant, LLVMValueRef* MaskConstant);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMConstExtractValue", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* ConstExtractValue(LLVMValueRef* AggConstant, System.IntPtr IdxList, uint NumIdx);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMConstInsertValue", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* ConstInsertValue(LLVMValueRef* AggConstant, LLVMValueRef* ElementValueConstant, System.IntPtr IdxList, uint NumIdx);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMConstInlineAsm", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* ConstInlineAsm(LLVMTypeRef* Ty, [In][MarshalAs(UnmanagedType.LPStr)] string AsmString, [In][MarshalAs(UnmanagedType.LPStr)] string Constraints, int HasSideEffects, int IsAlignStack);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMBlockAddress", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BlockAddress(LLVMValueRef* F, LLVMBasicBlockRef* BB);

        /* @}
         */
        
        /**
         * @defgroup LLVMCCoreValueConstantGlobals Global Values
         *
         * This group contains functions that operate on global values. Functions in
         * this group relate to functions in the llvm::GlobalValue class tree.
         *
         * @see llvm::GlobalValue
         *
         * @{*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMGetGlobalParent", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMModuleRef* GetGlobalParent(LLVMValueRef* Global);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMIsDeclaration", CallingConvention=CallingConvention.Cdecl)]
        public static extern int IsDeclaration(LLVMValueRef* Global);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMGetLinkage", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMLinkage GetLinkage(LLVMValueRef* Global);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMSetLinkage", CallingConvention=CallingConvention.Cdecl)]
        public static extern void SetLinkage(LLVMValueRef* Global, LLVMLinkage Linkage);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMGetSection", CallingConvention=CallingConvention.Cdecl)]
        public static extern System.IntPtr GetSection(LLVMValueRef* Global);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMSetSection", CallingConvention=CallingConvention.Cdecl)]
        public static extern void SetSection(LLVMValueRef* Global, [In][MarshalAs(UnmanagedType.LPStr)] string Section);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMGetVisibility", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMVisibility GetVisibility(LLVMValueRef* Global);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMSetVisibility", CallingConvention=CallingConvention.Cdecl)]
        public static extern void SetVisibility(LLVMValueRef* Global, LLVMVisibility Viz);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMGetAlignment", CallingConvention=CallingConvention.Cdecl)]
        public static extern uint GetAlignment(LLVMValueRef* Global);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMSetAlignment", CallingConvention=CallingConvention.Cdecl)]
        public static extern void SetAlignment(LLVMValueRef* Global, uint Bytes);

        /* @defgroup LLVMCoreValueConstantGlobalVariable Global Variables
         *
         * This group contains functions that operate on global variable values.
         *
         * @see llvm::GlobalVariable
         *
         * @{*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMAddGlobal", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* AddGlobal(LLVMModuleRef* M, LLVMTypeRef* Ty, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMAddGlobalInAddressSpace", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* AddGlobalInAddressSpace(LLVMModuleRef* M, LLVMTypeRef* Ty, [In][MarshalAs(UnmanagedType.LPStr)] string Name, uint AddressSpace);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMGetNamedGlobal", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* GetNamedGlobal(LLVMModuleRef* M, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMGetFirstGlobal", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* GetFirstGlobal(LLVMModuleRef* M);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMGetLastGlobal", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* GetLastGlobal(LLVMModuleRef* M);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMGetNextGlobal", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* GetNextGlobal(LLVMValueRef* GlobalVar);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMGetPreviousGlobal", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* GetPreviousGlobal(LLVMValueRef* GlobalVar);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMDeleteGlobal", CallingConvention=CallingConvention.Cdecl)]
        public static extern void DeleteGlobal(LLVMValueRef* GlobalVar);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMGetInitializer", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* GetInitializer(LLVMValueRef* GlobalVar);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMSetInitializer", CallingConvention=CallingConvention.Cdecl)]
        public static extern void SetInitializer(LLVMValueRef* GlobalVar, LLVMValueRef* ConstantVal);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMIsThreadLocal", CallingConvention=CallingConvention.Cdecl)]
        public static extern int IsThreadLocal(LLVMValueRef* GlobalVar);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMSetThreadLocal", CallingConvention=CallingConvention.Cdecl)]
        public static extern void SetThreadLocal(LLVMValueRef* GlobalVar, int IsThreadLocal);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMIsGlobalConstant", CallingConvention=CallingConvention.Cdecl)]
        public static extern int IsGlobalConstant(LLVMValueRef* GlobalVar);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMSetGlobalConstant", CallingConvention=CallingConvention.Cdecl)]
        public static extern void SetGlobalConstant(LLVMValueRef* GlobalVar, int IsConstant);

        /* @}
         */
        
        /**
         * @defgroup LLVMCoreValueConstantGlobalAlias Global Aliases
         *
         * This group contains function that operate on global alias values.
         *
         * @see llvm::GlobalAlias
         *
         * @{*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMAddAlias", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* AddAlias(LLVMModuleRef* M, LLVMTypeRef* Ty, LLVMValueRef* Aliasee, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        /* @}
         */
        
        /**
         * @defgroup LLVMCCoreValueFunction Function values
         *
         * Functions in this group operate on LLVMValueRef instances that
         * correspond to llvm::Function instances.
         *
         * @see llvm::Function
         *
         * @{
         */
        
        /**
         * Remove a function from its containing module and deletes it.
         *
         * @see llvm::Function::eraseFromParent()*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMDeleteFunction", CallingConvention=CallingConvention.Cdecl)]
        public static extern void DeleteFunction(LLVMValueRef* Fn);

        /* Obtain the ID number from a function instance.
         *
         * @see llvm::Function::getIntrinsicID()*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMGetIntrinsicID", CallingConvention=CallingConvention.Cdecl)]
        public static extern uint GetIntrinsicID(LLVMValueRef* Fn);

        /* Obtain the calling function of a function.
         *
         * The returned value corresponds to the LLVMCallConv enumeration.
         *
         * @see llvm::Function::getCallingConv()*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMGetFunctionCallConv", CallingConvention=CallingConvention.Cdecl)]
        public static extern uint GetFunctionCallConv(LLVMValueRef* Fn);

        /* Set the calling convention of a function.
         *
         * @see llvm::Function::setCallingConv()
         *
         * @param Fn Function to operate on
         * @param CC LLVMCallConv to set calling convention to*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMSetFunctionCallConv", CallingConvention=CallingConvention.Cdecl)]
        public static extern void SetFunctionCallConv(LLVMValueRef* Fn, uint CC);

        /* Obtain the name of the garbage collector to use during code
         * generation.
         *
         * @see llvm::Function::getGC()*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMGetGC", CallingConvention=CallingConvention.Cdecl)]
        public static extern System.IntPtr GetGC(LLVMValueRef* Fn);

        /* Define the garbage collector to use during code generation.
         *
         * @see llvm::Function::setGC()*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMSetGC", CallingConvention=CallingConvention.Cdecl)]
        public static extern void SetGC(LLVMValueRef* Fn, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        /* Add an attribute to a function.
         *
         * @see llvm::Function::addAttribute()*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMAddFunctionAttr", CallingConvention=CallingConvention.Cdecl)]
        public static extern void AddFunctionAttr(LLVMValueRef* Fn, LLVMAttribute PA);

        /* Obtain an attribute from a function.
         *
         * @see llvm::Function::getAttributes()*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMGetFunctionAttr", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMAttribute GetFunctionAttr(LLVMValueRef* Fn);

        /* Remove an attribute from a function.*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMRemoveFunctionAttr", CallingConvention=CallingConvention.Cdecl)]
        public static extern void RemoveFunctionAttr(LLVMValueRef* Fn, LLVMAttribute PA);

        /* @defgroup LLVMCCoreValueFunctionParameters Function Parameters
         *
         * Functions in this group relate to arguments/parameters on functions.
         *
         * Functions in this group expect LLVMValueRef instances that correspond
         * to llvm::Function instances.
         *
         * @{
         */
        
        /**
         * Obtain the number of parameters in a function.
         *
         * @see llvm::Function::arg_size()*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMCountParams", CallingConvention=CallingConvention.Cdecl)]
        public static extern uint CountParams(LLVMValueRef* Fn);

        /* Obtain the parameters in a function.
         *
         * The takes a pointer to a pre-allocated array of LLVMValueRef that is
         * at least LLVMCountParams() long. This array will be filled with
         * LLVMValueRef instances which correspond to the parameters the
         * function receives. Each LLVMValueRef corresponds to a llvm::Argument
         * instance.
         *
         * @see llvm::Function::arg_begin()*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMGetParams", CallingConvention=CallingConvention.Cdecl)]
        public static extern void GetParams(LLVMValueRef* Fn, System.IntPtr[] Params);

        /* Obtain the parameter at the specified index.
         *
         * Parameters are indexed from 0.
         *
         * @see llvm::Function::arg_begin()*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMGetParam", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* GetParam(LLVMValueRef* Fn, uint Index);

        /* Obtain the function to which this argument belongs.
         *
         * Unlike other functions in this group, this one takes a LLVMValueRef
         * that corresponds to a llvm::Attribute.
         *
         * The returned LLVMValueRef is the llvm::Function to which this
         * argument belongs.*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMGetParamParent", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* GetParamParent(LLVMValueRef* Inst);

        /* Obtain the first parameter to a function.
         *
         * @see llvm::Function::arg_begin()*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMGetFirstParam", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* GetFirstParam(LLVMValueRef* Fn);

        /* Obtain the last parameter to a function.
         *
         * @see llvm::Function::arg_end()*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMGetLastParam", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* GetLastParam(LLVMValueRef* Fn);

        /* Obtain the next parameter to a function.
         *
         * This takes a LLVMValueRef obtained from LLVMGetFirstParam() (which is
         * actually a wrapped iterator) and obtains the next parameter from the
         * underlying iterator.*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMGetNextParam", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* GetNextParam(LLVMValueRef* Arg);

        /* Obtain the previous parameter to a function.
         *
         * This is the opposite of LLVMGetNextParam().*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMGetPreviousParam", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* GetPreviousParam(LLVMValueRef* Arg);

        /* Add an attribute to a function argument.
         *
         * @see llvm::Argument::addAttr()*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMAddAttribute", CallingConvention=CallingConvention.Cdecl)]
        public static extern void AddAttribute(LLVMValueRef* Arg, LLVMAttribute PA);

        /* Remove an attribute from a function argument.
         *
         * @see llvm::Argument::removeAttr()*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMRemoveAttribute", CallingConvention=CallingConvention.Cdecl)]
        public static extern void RemoveAttribute(LLVMValueRef* Arg, LLVMAttribute PA);

        /* Get an attribute from a function argument.*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMGetAttribute", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMAttribute GetAttribute(LLVMValueRef* Arg);

        /* Set the alignment for a function parameter.
         *
         * @see llvm::Argument::addAttr()
         * @see llvm::Attribute::constructAlignmentFromInt()*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMSetParamAlignment", CallingConvention=CallingConvention.Cdecl)]
        public static extern void SetParamAlignment(LLVMValueRef* Arg, uint align);

        /* @}
         */
        
        /**
         * @}
         */
        
        /**
         * @}
         */
        
        /**
         * @}
         */
        
        /**
         * @defgroup LLVMCCoreValueMetadata Metadata
         *
         * @{
         */
        
        /**
         * Obtain a MDString value from a context.
         *
         * The returned instance corresponds to the llvm::MDString class.
         *
         * The instance is specified by string data of a specified length. The
         * string content is copied, so the backing memory can be freed after
         * this function returns.*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMMDStringInContext", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* MDStringInContext(LLVMContextRef* C, [In][MarshalAs(UnmanagedType.LPStr)] string Str, uint SLen);

        /* Obtain a MDString value from the global context.*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMMDString", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* MDString([In][MarshalAs(UnmanagedType.LPStr)] string Str, uint SLen);

        /* Obtain a MDNode value from a context.
         *
         * The returned value corresponds to the llvm::MDNode class.*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMMDNodeInContext", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* MDNodeInContext(LLVMContextRef* C, System.IntPtr[] Vals, uint Count);

        /* Obtain a MDNode value from the global context.*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMMDNode", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* MDNode(System.IntPtr[] Vals, uint Count);

        /* Obtain the underlying string from a MDString value.
         *
         * @param V Instance to obtain string from.
         * @param Len Memory address which will hold length of returned string.
         * @return String data in MDString.*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMGetMDString", CallingConvention=CallingConvention.Cdecl)]
        public static extern System.IntPtr GetMDString(LLVMValueRef* V, System.IntPtr Len);

        /* @}
         */
        
        /**
         * @defgroup LLVMCCoreValueBasicBlock Basic Block
         *
         * A basic block represents a single entry single exit section of code.
         * Basic blocks contain a list of instructions which form the body of
         * the block.
         *
         * Basic blocks belong to functions. They have the type of label.
         *
         * Basic blocks are themselves values. However, the C API models them as
         * LLVMBasicBlockRef.
         *
         * @see llvm::BasicBlock
         *
         * @{
         */
        
        /**
         * Convert a basic block instance to a value type.*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMBasicBlockAsValue", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BasicBlockAsValue(LLVMBasicBlockRef* BB);

        /* Determine whether a LLVMValueRef is itself a basic block.*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMValueIsBasicBlock", CallingConvention=CallingConvention.Cdecl)]
        public static extern int ValueIsBasicBlock(LLVMValueRef* Val);

        /* Convert a LLVMValueRef to a LLVMBasicBlockRef instance.*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMValueAsBasicBlock", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMBasicBlockRef* ValueAsBasicBlock(LLVMValueRef* Val);

        /* Obtain the function to which a basic block belongs.
         *
         * @see llvm::BasicBlock::getParent()*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMGetBasicBlockParent", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* GetBasicBlockParent(LLVMBasicBlockRef* BB);

        /* Obtain the terminator instruction for a basic block.
         *
         * If the basic block does not have a terminator (it is not well-formed
         * if it doesn't), then NULL is returned.
         *
         * The returned LLVMValueRef corresponds to a llvm::TerminatorInst.
         *
         * @see llvm::BasicBlock::getTerminator()*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMGetBasicBlockTerminator", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* GetBasicBlockTerminator(LLVMBasicBlockRef* BB);

        /* Obtain the number of basic blocks in a function.
         *
         * @param Fn Function value to operate on.*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMCountBasicBlocks", CallingConvention=CallingConvention.Cdecl)]
        public static extern uint CountBasicBlocks(LLVMValueRef* Fn);

        /* Obtain all of the basic blocks in a function.
         *
         * This operates on a function value. The BasicBlocks parameter is a
         * pointer to a pre-allocated array of LLVMBasicBlockRef of at least
         * LLVMCountBasicBlocks() in length. This array is populated with
         * LLVMBasicBlockRef instances.*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMGetBasicBlocks", CallingConvention=CallingConvention.Cdecl)]
        public static extern void GetBasicBlocks(LLVMValueRef* Fn, System.IntPtr[] BasicBlocks);

        /* Obtain the first basic block in a function.
         *
         * The returned basic block can be used as an iterator. You will likely
         * eventually call into LLVMGetNextBasicBlock() with it.
         *
         * @see llvm::Function::begin()*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMGetFirstBasicBlock", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMBasicBlockRef* GetFirstBasicBlock(LLVMValueRef* Fn);

        /* Obtain the last basic block in a function.
         *
         * @see llvm::Function::end()*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMGetLastBasicBlock", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMBasicBlockRef* GetLastBasicBlock(LLVMValueRef* Fn);

        /* Advance a basic block iterator.*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMGetNextBasicBlock", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMBasicBlockRef* GetNextBasicBlock(LLVMBasicBlockRef* BB);

        /* Go backwards in a basic block iterator.*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMGetPreviousBasicBlock", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMBasicBlockRef* GetPreviousBasicBlock(LLVMBasicBlockRef* BB);

        /* Obtain the basic block that corresponds to the entry point of a
         * function.
         *
         * @see llvm::Function::getEntryBlock()*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMGetEntryBasicBlock", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMBasicBlockRef* GetEntryBasicBlock(LLVMValueRef* Fn);

        /* Append a basic block to the end of a function.
         *
         * @see llvm::BasicBlock::Create()*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMAppendBasicBlockInContext", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMBasicBlockRef* AppendBasicBlockInContext(LLVMContextRef* C, LLVMValueRef* Fn, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        /* Append a basic block to the end of a function using the global
         * context.
         *
         * @see llvm::BasicBlock::Create()*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMAppendBasicBlock", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMBasicBlockRef* AppendBasicBlock(LLVMValueRef* Fn, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        /* Insert a basic block in a function before another basic block.
         *
         * The function to add to is determined by the function of the
         * passed basic block.
         *
         * @see llvm::BasicBlock::Create()*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMInsertBasicBlockInContext", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMBasicBlockRef* InsertBasicBlockInContext(LLVMContextRef* C, LLVMBasicBlockRef* BB, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        /* Insert a basic block in a function using the global context.
         *
         * @see llvm::BasicBlock::Create()*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMInsertBasicBlock", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMBasicBlockRef* InsertBasicBlock(LLVMBasicBlockRef* InsertBeforeBB, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        /* Remove a basic block from a function and delete it.
         *
         * This deletes the basic block from its containing function and deletes
         * the basic block itself.
         *
         * @see llvm::BasicBlock::eraseFromParent()*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMDeleteBasicBlock", CallingConvention=CallingConvention.Cdecl)]
        public static extern void DeleteBasicBlock(LLVMBasicBlockRef* BB);

        /* Remove a basic block from a function.
         *
         * This deletes the basic block from its containing function but keep
         * the basic block alive.
         *
         * @see llvm::BasicBlock::removeFromParent()*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMRemoveBasicBlockFromParent", CallingConvention=CallingConvention.Cdecl)]
        public static extern void RemoveBasicBlockFromParent(LLVMBasicBlockRef* BB);

        /* Move a basic block to before another one.
         *
         * @see llvm::BasicBlock::moveBefore()*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMMoveBasicBlockBefore", CallingConvention=CallingConvention.Cdecl)]
        public static extern void MoveBasicBlockBefore(LLVMBasicBlockRef* BB, LLVMBasicBlockRef* MovePos);

        /* Move a basic block to after another one.
         *
         * @see llvm::BasicBlock::moveAfter()*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMMoveBasicBlockAfter", CallingConvention=CallingConvention.Cdecl)]
        public static extern void MoveBasicBlockAfter(LLVMBasicBlockRef* BB, LLVMBasicBlockRef* MovePos);

        /* Obtain the first instruction in a basic block.
         *
         * The returned LLVMValueRef corresponds to a llvm::Instruction
         * instance.*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMGetFirstInstruction", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* GetFirstInstruction(LLVMBasicBlockRef* BB);

        /* Obtain the last instruction in a basic block.
         *
         * The returned LLVMValueRef corresponds to a LLVM:Instruction.*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMGetLastInstruction", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* GetLastInstruction(LLVMBasicBlockRef* BB);

        /* @}
         */
        
        /**
         * @defgroup LLVMCCoreValueInstruction Instructions
         *
         * Functions in this group relate to the inspection and manipulation of
         * individual instructions.
         *
         * In the C++ API, an instruction is modeled by llvm::Instruction. This
         * class has a large number of descendents. llvm::Instruction is a
         * llvm::Value and in the C API, instructions are modeled by
         * LLVMValueRef.
         *
         * This group also contains sub-groups which operate on specific
         * llvm::Instruction types, e.g. llvm::CallInst.
         *
         * @{
         */
        
        /**
         * Determine whether an instruction has any metadata attached.*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMHasMetadata", CallingConvention=CallingConvention.Cdecl)]
        public static extern int HasMetadata(LLVMValueRef* Val);

        /* Return metadata associated with an instruction value.*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMGetMetadata", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* GetMetadata(LLVMValueRef* Val, uint KindID);

        /* Set metadata associated with an instruction value.*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMSetMetadata", CallingConvention=CallingConvention.Cdecl)]
        public static extern void SetMetadata(LLVMValueRef* Val, uint KindID, LLVMValueRef* Node);

        /* Obtain the basic block to which an instruction belongs.
         *
         * @see llvm::Instruction::getParent()*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMGetInstructionParent", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMBasicBlockRef* GetInstructionParent(LLVMValueRef* Inst);

        /* Obtain the instruction that occurs after the one specified.
         *
         * The next instruction will be from the same basic block.
         *
         * If this is the last instruction in a basic block, NULL will be
         * returned.*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMGetNextInstruction", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* GetNextInstruction(LLVMValueRef* Inst);

        /* Obtain the instruction that occured before this one.
         *
         * If the instruction is the first instruction in a basic block, NULL
         * will be returned.*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMGetPreviousInstruction", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* GetPreviousInstruction(LLVMValueRef* Inst);

        /* Remove and delete an instruction.
         *
         * The instruction specified is removed from its containing building
         * block and then deleted.
         *
         * @see llvm::Instruction::eraseFromParent()*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMInstructionEraseFromParent", CallingConvention=CallingConvention.Cdecl)]
        public static extern void InstructionEraseFromParent(LLVMValueRef* Inst);

        /* Obtain the code opcode for an individual instruction.
         *
         * @see llvm::Instruction::getOpCode()*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMGetInstructionOpcode", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMOpcode GetInstructionOpcode(LLVMValueRef* Inst);

        /* Obtain the predicate of an instruction.
         *
         * This is only valid for instructions that correspond to llvm::ICmpInst
         * or llvm::ConstantExpr whose opcode is llvm::Instruction::ICmp.
         *
         * @see llvm::ICmpInst::getPredicate()*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMGetICmpPredicate", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMIntPredicate GetICmpPredicate(LLVMValueRef* Inst);

        /* @defgroup LLVMCCoreValueInstructionCall Call Sites and Invocations
         *
         * Functions in this group apply to instructions that refer to call
         * sites and invocations. These correspond to C++ types in the
         * llvm::CallInst class tree.
         *
         * @{
         */
        
        /**
         * Set the calling convention for a call instruction.
         *
         * This expects an LLVMValueRef that corresponds to a llvm::CallInst or
         * llvm::InvokeInst.
         *
         * @see llvm::CallInst::setCallingConv()
         * @see llvm::InvokeInst::setCallingConv()*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMSetInstructionCallConv", CallingConvention=CallingConvention.Cdecl)]
        public static extern void SetInstructionCallConv(LLVMValueRef* Instr, uint CC);

        /* Obtain the calling convention for a call instruction.
         *
         * This is the opposite of LLVMSetInstructionCallConv(). Reads its
         * usage.
         *
         * @see LLVMSetInstructionCallConv()*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMGetInstructionCallConv", CallingConvention=CallingConvention.Cdecl)]
        public static extern uint GetInstructionCallConv(LLVMValueRef* Instr);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMAddInstrAttribute", CallingConvention=CallingConvention.Cdecl)]
        public static extern void AddInstrAttribute(LLVMValueRef* Instr, uint index, LLVMAttribute LLVMAttribute);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMRemoveInstrAttribute", CallingConvention=CallingConvention.Cdecl)]
        public static extern void RemoveInstrAttribute(LLVMValueRef* Instr, uint index, LLVMAttribute LLVMAttribute);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMSetInstrParamAlignment", CallingConvention=CallingConvention.Cdecl)]
        public static extern void SetInstrParamAlignment(LLVMValueRef* Instr, uint index, uint align);

        /* Obtain whether a call instruction is a tail call.
         *
         * This only works on llvm::CallInst instructions.
         *
         * @see llvm::CallInst::isTailCall()*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMIsTailCall", CallingConvention=CallingConvention.Cdecl)]
        public static extern int IsTailCall(LLVMValueRef* CallInst);

        /* Set whether a call instruction is a tail call.
         *
         * This only works on llvm::CallInst instructions.
         *
         * @see llvm::CallInst::setTailCall()*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMSetTailCall", CallingConvention=CallingConvention.Cdecl)]
        public static extern void SetTailCall(LLVMValueRef* CallInst, int IsTailCall);

        /* @}
         */
        
        /**
         * Obtain the default destination basic block of a switch instruction.
         *
         * This only works on llvm::SwitchInst instructions.
         *
         * @see llvm::SwitchInst::getDefaultDest()*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMGetSwitchDefaultDest", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMBasicBlockRef* GetSwitchDefaultDest(LLVMValueRef* SwitchInstr);

        /* @defgroup LLVMCCoreValueInstructionPHINode PHI Nodes
         *
         * Functions in this group only apply to instructions that map to
         * llvm::PHINode instances.
         *
         * @{
         */
        
        /**
         * Add an incoming value to the end of a PHI list.*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMAddIncoming", CallingConvention=CallingConvention.Cdecl)]
        public static extern void AddIncoming(LLVMValueRef* PhiNode, System.IntPtr[] IncomingValues, System.IntPtr[] IncomingBlocks, uint Count);

        /* Obtain the number of incoming basic blocks to a PHI node.*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMCountIncoming", CallingConvention=CallingConvention.Cdecl)]
        public static extern uint CountIncoming(LLVMValueRef* PhiNode);

        /* Obtain an incoming value to a PHI node as a LLVMValueRef.*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMGetIncomingValue", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* GetIncomingValue(LLVMValueRef* PhiNode, uint Index);

        /* Obtain an incoming value to a PHI node as a LLVMBasicBlockRef.*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMGetIncomingBlock", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMBasicBlockRef* GetIncomingBlock(LLVMValueRef* PhiNode, uint Index);

        /* @}
         */
        
        /**
         * @}
         */
        
        /**
         * @}
         */
        
        /**
         * @defgroup LLVMCCoreInstructionBuilder Instruction Builders
         *
         * An instruction builder represents a point within a basic block and is
         * the exclusive means of building instructions using the C interface.
         *
         * @{*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMCreateBuilderInContext", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMBuilderRef* CreateBuilderInContext(LLVMContextRef* C);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMCreateBuilder", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMBuilderRef* CreateBuilder();

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMPositionBuilder", CallingConvention=CallingConvention.Cdecl)]
        public static extern void PositionBuilder(LLVMBuilderRef* Builder, LLVMBasicBlockRef* Block, LLVMValueRef* Instr);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMPositionBuilderBefore", CallingConvention=CallingConvention.Cdecl)]
        public static extern void PositionBuilderBefore(LLVMBuilderRef* Builder, LLVMValueRef* Instr);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMPositionBuilderAtEnd", CallingConvention=CallingConvention.Cdecl)]
        public static extern void PositionBuilderAtEnd(LLVMBuilderRef* Builder, LLVMBasicBlockRef* Block);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMGetInsertBlock", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMBasicBlockRef* GetInsertBlock(LLVMBuilderRef* Builder);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMClearInsertionPosition", CallingConvention=CallingConvention.Cdecl)]
        public static extern void ClearInsertionPosition(LLVMBuilderRef* Builder);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMInsertIntoBuilder", CallingConvention=CallingConvention.Cdecl)]
        public static extern void InsertIntoBuilder(LLVMBuilderRef* Builder, LLVMValueRef* Instr);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMInsertIntoBuilderWithName", CallingConvention=CallingConvention.Cdecl)]
        public static extern void InsertIntoBuilderWithName(LLVMBuilderRef* Builder, LLVMValueRef* Instr, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMDisposeBuilder", CallingConvention=CallingConvention.Cdecl)]
        public static extern void DisposeBuilder(LLVMBuilderRef* Builder);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMSetCurrentDebugLocation", CallingConvention=CallingConvention.Cdecl)]
        public static extern void SetCurrentDebugLocation(LLVMBuilderRef* Builder, LLVMValueRef* L);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMGetCurrentDebugLocation", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* GetCurrentDebugLocation(LLVMBuilderRef* Builder);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMSetInstDebugLocation", CallingConvention=CallingConvention.Cdecl)]
        public static extern void SetInstDebugLocation(LLVMBuilderRef* Builder, LLVMValueRef* Inst);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMBuildRetVoid", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildRetVoid(LLVMBuilderRef* LLVMBuilderRef);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMBuildRet", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildRet(LLVMBuilderRef* LLVMBuilderRef, LLVMValueRef* V);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMBuildAggregateRet", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildAggregateRet(LLVMBuilderRef* LLVMBuilderRef, System.IntPtr[] RetVals, uint N);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMBuildBr", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildBr(LLVMBuilderRef* LLVMBuilderRef, LLVMBasicBlockRef* Dest);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMBuildCondBr", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildCondBr(LLVMBuilderRef* LLVMBuilderRef, LLVMValueRef* If, LLVMBasicBlockRef* Then, LLVMBasicBlockRef* Else);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMBuildSwitch", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildSwitch(LLVMBuilderRef* LLVMBuilderRef, LLVMValueRef* V, LLVMBasicBlockRef* Else, uint NumCases);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMBuildIndirectBr", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildIndirectBr(LLVMBuilderRef* B, LLVMValueRef* Addr, uint NumDests);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMBuildInvoke", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildInvoke(LLVMBuilderRef* LLVMBuilderRef, LLVMValueRef* Fn, System.IntPtr[] Args, uint NumArgs, LLVMBasicBlockRef* Then, LLVMBasicBlockRef* Catch, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMBuildLandingPad", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildLandingPad(LLVMBuilderRef* B, LLVMTypeRef* Ty, LLVMValueRef* PersFn, uint NumClauses, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMBuildResume", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildResume(LLVMBuilderRef* B, LLVMValueRef* Exn);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMBuildUnreachable", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildUnreachable(LLVMBuilderRef* LLVMBuilderRef);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMAddCase", CallingConvention=CallingConvention.Cdecl)]
        public static extern void AddCase(LLVMValueRef* Switch, LLVMValueRef* OnVal, LLVMBasicBlockRef* Dest);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMAddDestination", CallingConvention=CallingConvention.Cdecl)]
        public static extern void AddDestination(LLVMValueRef* IndirectBr, LLVMBasicBlockRef* Dest);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMAddClause", CallingConvention=CallingConvention.Cdecl)]
        public static extern void AddClause(LLVMValueRef* LandingPad, LLVMValueRef* ClauseVal);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMSetCleanup", CallingConvention=CallingConvention.Cdecl)]
        public static extern void SetCleanup(LLVMValueRef* LandingPad, int Val);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMBuildAdd", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildAdd(LLVMBuilderRef* LLVMBuilderRef, LLVMValueRef* LHS, LLVMValueRef* RHS, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMBuildNSWAdd", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildNSWAdd(LLVMBuilderRef* LLVMBuilderRef, LLVMValueRef* LHS, LLVMValueRef* RHS, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMBuildNUWAdd", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildNUWAdd(LLVMBuilderRef* LLVMBuilderRef, LLVMValueRef* LHS, LLVMValueRef* RHS, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMBuildFAdd", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildFAdd(LLVMBuilderRef* LLVMBuilderRef, LLVMValueRef* LHS, LLVMValueRef* RHS, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMBuildSub", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildSub(LLVMBuilderRef* LLVMBuilderRef, LLVMValueRef* LHS, LLVMValueRef* RHS, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMBuildNSWSub", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildNSWSub(LLVMBuilderRef* LLVMBuilderRef, LLVMValueRef* LHS, LLVMValueRef* RHS, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMBuildNUWSub", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildNUWSub(LLVMBuilderRef* LLVMBuilderRef, LLVMValueRef* LHS, LLVMValueRef* RHS, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMBuildFSub", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildFSub(LLVMBuilderRef* LLVMBuilderRef, LLVMValueRef* LHS, LLVMValueRef* RHS, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMBuildMul", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildMul(LLVMBuilderRef* LLVMBuilderRef, LLVMValueRef* LHS, LLVMValueRef* RHS, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMBuildNSWMul", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildNSWMul(LLVMBuilderRef* LLVMBuilderRef, LLVMValueRef* LHS, LLVMValueRef* RHS, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMBuildNUWMul", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildNUWMul(LLVMBuilderRef* LLVMBuilderRef, LLVMValueRef* LHS, LLVMValueRef* RHS, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMBuildFMul", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildFMul(LLVMBuilderRef* LLVMBuilderRef, LLVMValueRef* LHS, LLVMValueRef* RHS, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMBuildUDiv", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildUDiv(LLVMBuilderRef* LLVMBuilderRef, LLVMValueRef* LHS, LLVMValueRef* RHS, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMBuildSDiv", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildSDiv(LLVMBuilderRef* LLVMBuilderRef, LLVMValueRef* LHS, LLVMValueRef* RHS, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMBuildExactSDiv", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildExactSDiv(LLVMBuilderRef* LLVMBuilderRef, LLVMValueRef* LHS, LLVMValueRef* RHS, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMBuildFDiv", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildFDiv(LLVMBuilderRef* LLVMBuilderRef, LLVMValueRef* LHS, LLVMValueRef* RHS, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMBuildURem", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildURem(LLVMBuilderRef* LLVMBuilderRef, LLVMValueRef* LHS, LLVMValueRef* RHS, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMBuildSRem", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildSRem(LLVMBuilderRef* LLVMBuilderRef, LLVMValueRef* LHS, LLVMValueRef* RHS, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMBuildFRem", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildFRem(LLVMBuilderRef* LLVMBuilderRef, LLVMValueRef* LHS, LLVMValueRef* RHS, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMBuildShl", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildShl(LLVMBuilderRef* LLVMBuilderRef, LLVMValueRef* LHS, LLVMValueRef* RHS, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMBuildLShr", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildLShr(LLVMBuilderRef* LLVMBuilderRef, LLVMValueRef* LHS, LLVMValueRef* RHS, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMBuildAShr", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildAShr(LLVMBuilderRef* LLVMBuilderRef, LLVMValueRef* LHS, LLVMValueRef* RHS, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMBuildAnd", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildAnd(LLVMBuilderRef* LLVMBuilderRef, LLVMValueRef* LHS, LLVMValueRef* RHS, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMBuildOr", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildOr(LLVMBuilderRef* LLVMBuilderRef, LLVMValueRef* LHS, LLVMValueRef* RHS, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMBuildXor", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildXor(LLVMBuilderRef* LLVMBuilderRef, LLVMValueRef* LHS, LLVMValueRef* RHS, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMBuildBinOp", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildBinOp(LLVMBuilderRef* B, LLVMOpcode Op, LLVMValueRef* LHS, LLVMValueRef* RHS, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMBuildNeg", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildNeg(LLVMBuilderRef* LLVMBuilderRef, LLVMValueRef* V, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMBuildNSWNeg", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildNSWNeg(LLVMBuilderRef* B, LLVMValueRef* V, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMBuildNUWNeg", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildNUWNeg(LLVMBuilderRef* B, LLVMValueRef* V, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMBuildFNeg", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildFNeg(LLVMBuilderRef* LLVMBuilderRef, LLVMValueRef* V, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMBuildNot", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildNot(LLVMBuilderRef* LLVMBuilderRef, LLVMValueRef* V, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMBuildMalloc", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildMalloc(LLVMBuilderRef* LLVMBuilderRef, LLVMTypeRef* Ty, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMBuildArrayMalloc", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildArrayMalloc(LLVMBuilderRef* LLVMBuilderRef, LLVMTypeRef* Ty, LLVMValueRef* Val, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMBuildAlloca", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildAlloca(LLVMBuilderRef* LLVMBuilderRef, LLVMTypeRef* Ty, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMBuildArrayAlloca", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildArrayAlloca(LLVMBuilderRef* LLVMBuilderRef, LLVMTypeRef* Ty, LLVMValueRef* Val, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMBuildFree", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildFree(LLVMBuilderRef* LLVMBuilderRef, LLVMValueRef* PointerVal);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMBuildLoad", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildLoad(LLVMBuilderRef* LLVMBuilderRef, LLVMValueRef* PointerVal, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMBuildStore", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildStore(LLVMBuilderRef* LLVMBuilderRef, LLVMValueRef* Val, LLVMValueRef* Ptr);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMBuildGEP", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildGEP(LLVMBuilderRef* B, LLVMValueRef* Pointer, System.IntPtr[] Indices, uint NumIndices, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMBuildInBoundsGEP", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildInBoundsGEP(LLVMBuilderRef* B, LLVMValueRef* Pointer, System.IntPtr[] Indices, uint NumIndices, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMBuildStructGEP", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildStructGEP(LLVMBuilderRef* B, LLVMValueRef* Pointer, uint Idx, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMBuildGlobalString", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildGlobalString(LLVMBuilderRef* B, [In][MarshalAs(UnmanagedType.LPStr)] string Str, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMBuildGlobalStringPtr", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildGlobalStringPtr(LLVMBuilderRef* B, [In][MarshalAs(UnmanagedType.LPStr)] string Str, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMGetVolatile", CallingConvention=CallingConvention.Cdecl)]
        public static extern int GetVolatile(LLVMValueRef* MemoryAccessInst);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMSetVolatile", CallingConvention=CallingConvention.Cdecl)]
        public static extern void SetVolatile(LLVMValueRef* MemoryAccessInst, int IsVolatile);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMBuildTrunc", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildTrunc(LLVMBuilderRef* LLVMBuilderRef, LLVMValueRef* Val, LLVMTypeRef* DestTy, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMBuildZExt", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildZExt(LLVMBuilderRef* LLVMBuilderRef, LLVMValueRef* Val, LLVMTypeRef* DestTy, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMBuildSExt", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildSExt(LLVMBuilderRef* LLVMBuilderRef, LLVMValueRef* Val, LLVMTypeRef* DestTy, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMBuildFPToUI", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildFPToUI(LLVMBuilderRef* LLVMBuilderRef, LLVMValueRef* Val, LLVMTypeRef* DestTy, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMBuildFPToSI", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildFPToSI(LLVMBuilderRef* LLVMBuilderRef, LLVMValueRef* Val, LLVMTypeRef* DestTy, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMBuildUIToFP", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildUIToFP(LLVMBuilderRef* LLVMBuilderRef, LLVMValueRef* Val, LLVMTypeRef* DestTy, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMBuildSIToFP", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildSIToFP(LLVMBuilderRef* LLVMBuilderRef, LLVMValueRef* Val, LLVMTypeRef* DestTy, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMBuildFPTrunc", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildFPTrunc(LLVMBuilderRef* LLVMBuilderRef, LLVMValueRef* Val, LLVMTypeRef* DestTy, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMBuildFPExt", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildFPExt(LLVMBuilderRef* LLVMBuilderRef, LLVMValueRef* Val, LLVMTypeRef* DestTy, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMBuildPtrToInt", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildPtrToInt(LLVMBuilderRef* LLVMBuilderRef, LLVMValueRef* Val, LLVMTypeRef* DestTy, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMBuildIntToPtr", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildIntToPtr(LLVMBuilderRef* LLVMBuilderRef, LLVMValueRef* Val, LLVMTypeRef* DestTy, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMBuildBitCast", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildBitCast(LLVMBuilderRef* LLVMBuilderRef, LLVMValueRef* Val, LLVMTypeRef* DestTy, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMBuildZExtOrBitCast", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildZExtOrBitCast(LLVMBuilderRef* LLVMBuilderRef, LLVMValueRef* Val, LLVMTypeRef* DestTy, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMBuildSExtOrBitCast", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildSExtOrBitCast(LLVMBuilderRef* LLVMBuilderRef, LLVMValueRef* Val, LLVMTypeRef* DestTy, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMBuildTruncOrBitCast", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildTruncOrBitCast(LLVMBuilderRef* LLVMBuilderRef, LLVMValueRef* Val, LLVMTypeRef* DestTy, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMBuildCast", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildCast(LLVMBuilderRef* B, LLVMOpcode Op, LLVMValueRef* Val, LLVMTypeRef* DestTy, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMBuildPointerCast", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildPointerCast(LLVMBuilderRef* LLVMBuilderRef, LLVMValueRef* Val, LLVMTypeRef* DestTy, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMBuildIntCast", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildIntCast(LLVMBuilderRef* LLVMBuilderRef, LLVMValueRef* Val, LLVMTypeRef* DestTy, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMBuildFPCast", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildFPCast(LLVMBuilderRef* LLVMBuilderRef, LLVMValueRef* Val, LLVMTypeRef* DestTy, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMBuildICmp", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildICmp(LLVMBuilderRef* LLVMBuilderRef, LLVMIntPredicate Op, LLVMValueRef* LHS, LLVMValueRef* RHS, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMBuildFCmp", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildFCmp(LLVMBuilderRef* LLVMBuilderRef, LLVMRealPredicate Op, LLVMValueRef* LHS, LLVMValueRef* RHS, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMBuildPhi", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildPhi(LLVMBuilderRef* LLVMBuilderRef, LLVMTypeRef* Ty, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMBuildCall", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildCall(LLVMBuilderRef* LLVMBuilderRef, LLVMValueRef* Fn, System.IntPtr[] Args, uint NumArgs, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMBuildSelect", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildSelect(LLVMBuilderRef* LLVMBuilderRef, LLVMValueRef* If, LLVMValueRef* Then, LLVMValueRef* Else, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMBuildVAArg", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildVAArg(LLVMBuilderRef* LLVMBuilderRef, LLVMValueRef* List, LLVMTypeRef* Ty, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMBuildExtractElement", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildExtractElement(LLVMBuilderRef* LLVMBuilderRef, LLVMValueRef* VecVal, LLVMValueRef* Index, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMBuildInsertElement", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildInsertElement(LLVMBuilderRef* LLVMBuilderRef, LLVMValueRef* VecVal, LLVMValueRef* EltVal, LLVMValueRef* Index, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMBuildShuffleVector", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildShuffleVector(LLVMBuilderRef* LLVMBuilderRef, LLVMValueRef* V1, LLVMValueRef* V2, LLVMValueRef* Mask, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMBuildExtractValue", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildExtractValue(LLVMBuilderRef* LLVMBuilderRef, LLVMValueRef* AggVal, uint Index, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMBuildInsertValue", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildInsertValue(LLVMBuilderRef* LLVMBuilderRef, LLVMValueRef* AggVal, LLVMValueRef* EltVal, uint Index, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMBuildIsNull", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildIsNull(LLVMBuilderRef* LLVMBuilderRef, LLVMValueRef* Val, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMBuildIsNotNull", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildIsNotNull(LLVMBuilderRef* LLVMBuilderRef, LLVMValueRef* Val, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMBuildPtrDiff", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildPtrDiff(LLVMBuilderRef* LLVMBuilderRef, LLVMValueRef* LHS, LLVMValueRef* RHS, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        /* @}
         */
        
        /**
         * @defgroup LLVMCCoreModuleProvider Module Providers
         *
         * @{
         */
        
        /**
         * Changes the type of M so it can be passed to FunctionPassManagers and the
         * JIT.  They take ModuleProviders for historical reasons.*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMCreateModuleProviderForExistingModule", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMModuleProviderRef* CreateModuleProviderForExistingModule(LLVMModuleRef* M);

        /* Destroys the module M.*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMDisposeModuleProvider", CallingConvention=CallingConvention.Cdecl)]
        public static extern void DisposeModuleProvider(LLVMModuleProviderRef* M);

        /* @}
         */
        
        /**
         * @defgroup LLVMCCoreMemoryBuffers Memory Buffers
         *
         * @{*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMCreateMemoryBufferWithContentsOfFile", CallingConvention=CallingConvention.Cdecl)]
        public static extern int CreateMemoryBufferWithContentsOfFile([In][MarshalAs(UnmanagedType.LPStr)] string Path, ref LLVMMemoryBufferRef * OutMemBuf, [MarshalAs(UnmanagedType.LPStr)] ref StringBuilder OutMessage);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMCreateMemoryBufferWithSTDIN", CallingConvention=CallingConvention.Cdecl)]
        public static extern int CreateMemoryBufferWithSTDIN(ref LLVMMemoryBufferRef * OutMemBuf, [MarshalAs(UnmanagedType.LPStr)] ref StringBuilder OutMessage);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMDisposeMemoryBuffer", CallingConvention=CallingConvention.Cdecl)]
        public static extern void DisposeMemoryBuffer(LLVMMemoryBufferRef* MemBuf);

        /* @}
         */
        
        /**
         * @defgroup LLVMCCorePassRegistry Pass Registry
         *
         * @{
         */
        
        /** Return the global pass registry, for use with initialization functions.
            @see llvm::PassRegistry::getPassRegistry*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMGetGlobalPassRegistry", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMPassRegistryRef* GetGlobalPassRegistry();

        /* @}
         */
        
        /**
         * @defgroup LLVMCCorePassManagers Pass Managers
         *
         * @{
         */
        
        /** Constructs a new whole-module pass pipeline. This type of pipeline is
            suitable for link-time optimization and whole-module transformations.
            @see llvm::PassManager::PassManager*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMCreatePassManager", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMPassManagerRef* CreatePassManager();

        /* Constructs a new function-by-function pass pipeline over the module
            provider. It does not take ownership of the module provider. This type of
            pipeline is suitable for code generation and JIT compilation tasks.
            @see llvm::FunctionPassManager::FunctionPassManager*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMCreateFunctionPassManagerForModule", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMPassManagerRef* CreateFunctionPassManagerForModule(LLVMModuleRef* M);

        /* Deprecated: Use LLVMCreateFunctionPassManagerForModule instead.*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMCreateFunctionPassManager", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMPassManagerRef* CreateFunctionPassManager(LLVMModuleProviderRef* MP);

        /* Initializes, executes on the provided module, and finalizes all of the
            passes scheduled in the pass manager. Returns 1 if any of the passes
            modified the module, 0 otherwise.
            @see llvm::PassManager::run(Module&)*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMRunPassManager", CallingConvention=CallingConvention.Cdecl)]
        public static extern int RunPassManager(LLVMPassManagerRef* PM, LLVMModuleRef* M);

        /* Initializes all of the function passes scheduled in the function pass
            manager. Returns 1 if any of the passes modified the module, 0 otherwise.
            @see llvm::FunctionPassManager::doInitialization*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMInitializeFunctionPassManager", CallingConvention=CallingConvention.Cdecl)]
        public static extern int InitializeFunctionPassManager(LLVMPassManagerRef* FPM);

        /* Executes all of the function passes scheduled in the function pass manager
            on the provided function. Returns 1 if any of the passes modified the
            function, false otherwise.
            @see llvm::FunctionPassManager::run(Function&)*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMRunFunctionPassManager", CallingConvention=CallingConvention.Cdecl)]
        public static extern int RunFunctionPassManager(LLVMPassManagerRef* FPM, LLVMValueRef* F);

        /* Finalizes all of the function passes scheduled in in the function pass
            manager. Returns 1 if any of the passes modified the module, 0 otherwise.
            @see llvm::FunctionPassManager::doFinalization*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMFinalizeFunctionPassManager", CallingConvention=CallingConvention.Cdecl)]
        public static extern int FinalizeFunctionPassManager(LLVMPassManagerRef* FPM);

        /* Frees the memory of a pass pipeline. For function pipelines, does not free
            the module provider.
            @see llvm::PassManagerBase::~PassManagerBase.*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMDisposePassManager", CallingConvention=CallingConvention.Cdecl)]
        public static extern void DisposePassManager(LLVMPassManagerRef* PM);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMInitializeHexagonTargetInfo", CallingConvention=CallingConvention.Cdecl)]
        public static extern void InitializeHexagonTargetInfo();

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMInitializePTXTargetInfo", CallingConvention=CallingConvention.Cdecl)]
        public static extern void InitializePTXTargetInfo();

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMInitializeMBlazeTargetInfo", CallingConvention=CallingConvention.Cdecl)]
        public static extern void InitializeMBlazeTargetInfo();

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMInitializeCppBackendTargetInfo", CallingConvention=CallingConvention.Cdecl)]
        public static extern void InitializeCppBackendTargetInfo();

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMInitializeMSP430TargetInfo", CallingConvention=CallingConvention.Cdecl)]
        public static extern void InitializeMSP430TargetInfo();

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMInitializeXCoreTargetInfo", CallingConvention=CallingConvention.Cdecl)]
        public static extern void InitializeXCoreTargetInfo();

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMInitializeCellSPUTargetInfo", CallingConvention=CallingConvention.Cdecl)]
        public static extern void InitializeCellSPUTargetInfo();

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMInitializeMipsTargetInfo", CallingConvention=CallingConvention.Cdecl)]
        public static extern void InitializeMipsTargetInfo();

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMInitializeARMTargetInfo", CallingConvention=CallingConvention.Cdecl)]
        public static extern void InitializeARMTargetInfo();

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMInitializePowerPCTargetInfo", CallingConvention=CallingConvention.Cdecl)]
        public static extern void InitializePowerPCTargetInfo();

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMInitializeSparcTargetInfo", CallingConvention=CallingConvention.Cdecl)]
        public static extern void InitializeSparcTargetInfo();

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMInitializeX86TargetInfo", CallingConvention=CallingConvention.Cdecl)]
        public static extern void InitializeX86TargetInfo();

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMInitializeHexagonTarget", CallingConvention=CallingConvention.Cdecl)]
        public static extern void InitializeHexagonTarget();

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMInitializePTXTarget", CallingConvention=CallingConvention.Cdecl)]
        public static extern void InitializePTXTarget();

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMInitializeMBlazeTarget", CallingConvention=CallingConvention.Cdecl)]
        public static extern void InitializeMBlazeTarget();

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMInitializeCppBackendTarget", CallingConvention=CallingConvention.Cdecl)]
        public static extern void InitializeCppBackendTarget();

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMInitializeMSP430Target", CallingConvention=CallingConvention.Cdecl)]
        public static extern void InitializeMSP430Target();

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMInitializeXCoreTarget", CallingConvention=CallingConvention.Cdecl)]
        public static extern void InitializeXCoreTarget();

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMInitializeCellSPUTarget", CallingConvention=CallingConvention.Cdecl)]
        public static extern void InitializeCellSPUTarget();

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMInitializeMipsTarget", CallingConvention=CallingConvention.Cdecl)]
        public static extern void InitializeMipsTarget();

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMInitializeARMTarget", CallingConvention=CallingConvention.Cdecl)]
        public static extern void InitializeARMTarget();

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMInitializePowerPCTarget", CallingConvention=CallingConvention.Cdecl)]
        public static extern void InitializePowerPCTarget();

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMInitializeSparcTarget", CallingConvention=CallingConvention.Cdecl)]
        public static extern void InitializeSparcTarget();

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMInitializeX86Target", CallingConvention=CallingConvention.Cdecl)]
        public static extern void InitializeX86Target();

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMInitializeHexagonTargetMC", CallingConvention=CallingConvention.Cdecl)]
        public static extern void InitializeHexagonTargetMC();

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMInitializePTXTargetMC", CallingConvention=CallingConvention.Cdecl)]
        public static extern void InitializePTXTargetMC();

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMInitializeMBlazeTargetMC", CallingConvention=CallingConvention.Cdecl)]
        public static extern void InitializeMBlazeTargetMC();

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMInitializeCppBackendTargetMC", CallingConvention=CallingConvention.Cdecl)]
        public static extern void InitializeCppBackendTargetMC();

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMInitializeMSP430TargetMC", CallingConvention=CallingConvention.Cdecl)]
        public static extern void InitializeMSP430TargetMC();

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMInitializeXCoreTargetMC", CallingConvention=CallingConvention.Cdecl)]
        public static extern void InitializeXCoreTargetMC();

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMInitializeCellSPUTargetMC", CallingConvention=CallingConvention.Cdecl)]
        public static extern void InitializeCellSPUTargetMC();

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMInitializeMipsTargetMC", CallingConvention=CallingConvention.Cdecl)]
        public static extern void InitializeMipsTargetMC();

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMInitializeARMTargetMC", CallingConvention=CallingConvention.Cdecl)]
        public static extern void InitializeARMTargetMC();

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMInitializePowerPCTargetMC", CallingConvention=CallingConvention.Cdecl)]
        public static extern void InitializePowerPCTargetMC();

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMInitializeSparcTargetMC", CallingConvention=CallingConvention.Cdecl)]
        public static extern void InitializeSparcTargetMC();

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMInitializeX86TargetMC", CallingConvention=CallingConvention.Cdecl)]
        public static extern void InitializeX86TargetMC();

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMInitializeHexagonAsmPrinter", CallingConvention=CallingConvention.Cdecl)]
        public static extern void InitializeHexagonAsmPrinter();

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMInitializePTXAsmPrinter", CallingConvention=CallingConvention.Cdecl)]
        public static extern void InitializePTXAsmPrinter();

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMInitializeMBlazeAsmPrinter", CallingConvention=CallingConvention.Cdecl)]
        public static extern void InitializeMBlazeAsmPrinter();

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMInitializeMSP430AsmPrinter", CallingConvention=CallingConvention.Cdecl)]
        public static extern void InitializeMSP430AsmPrinter();

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMInitializeXCoreAsmPrinter", CallingConvention=CallingConvention.Cdecl)]
        public static extern void InitializeXCoreAsmPrinter();

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMInitializeCellSPUAsmPrinter", CallingConvention=CallingConvention.Cdecl)]
        public static extern void InitializeCellSPUAsmPrinter();

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMInitializeMipsAsmPrinter", CallingConvention=CallingConvention.Cdecl)]
        public static extern void InitializeMipsAsmPrinter();

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMInitializeARMAsmPrinter", CallingConvention=CallingConvention.Cdecl)]
        public static extern void InitializeARMAsmPrinter();

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMInitializePowerPCAsmPrinter", CallingConvention=CallingConvention.Cdecl)]
        public static extern void InitializePowerPCAsmPrinter();

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMInitializeSparcAsmPrinter", CallingConvention=CallingConvention.Cdecl)]
        public static extern void InitializeSparcAsmPrinter();

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMInitializeX86AsmPrinter", CallingConvention=CallingConvention.Cdecl)]
        public static extern void InitializeX86AsmPrinter();

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMInitializeMBlazeAsmParser", CallingConvention=CallingConvention.Cdecl)]
        public static extern void InitializeMBlazeAsmParser();

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMInitializeMipsAsmParser", CallingConvention=CallingConvention.Cdecl)]
        public static extern void InitializeMipsAsmParser();

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMInitializeARMAsmParser", CallingConvention=CallingConvention.Cdecl)]
        public static extern void InitializeARMAsmParser();

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMInitializeX86AsmParser", CallingConvention=CallingConvention.Cdecl)]
        public static extern void InitializeX86AsmParser();

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMInitializeMBlazeDisassembler", CallingConvention=CallingConvention.Cdecl)]
        public static extern void InitializeMBlazeDisassembler();

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMInitializeMipsDisassembler", CallingConvention=CallingConvention.Cdecl)]
        public static extern void InitializeMipsDisassembler();

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMInitializeARMDisassembler", CallingConvention=CallingConvention.Cdecl)]
        public static extern void InitializeARMDisassembler();

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMInitializeX86Disassembler", CallingConvention=CallingConvention.Cdecl)]
        public static extern void InitializeX86Disassembler();

        /* @defgroup LLVMCExecutionEngine Execution Engine
         * @ingroup LLVMC
         *
         * @{*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMLinkInJIT", CallingConvention=CallingConvention.Cdecl)]
        public static extern void LinkInJIT();

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMLinkInInterpreter", CallingConvention=CallingConvention.Cdecl)]
        public static extern void LinkInInterpreter();

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMCreateGenericValueOfInt", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMGenericValueRef* CreateGenericValueOfInt(LLVMTypeRef* Ty, ulong N, int IsSigned);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMCreateGenericValueOfPointer", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMGenericValueRef* CreateGenericValueOfPointer(System.IntPtr P);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMCreateGenericValueOfFloat", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMGenericValueRef* CreateGenericValueOfFloat(LLVMTypeRef* Ty, double N);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMGenericValueIntWidth", CallingConvention=CallingConvention.Cdecl)]
        public static extern uint GenericValueIntWidth(LLVMGenericValueRef* GenValRef);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMGenericValueToInt", CallingConvention=CallingConvention.Cdecl)]
        public static extern ulong GenericValueToInt(LLVMGenericValueRef* GenVal, int IsSigned);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMGenericValueToPointer", CallingConvention=CallingConvention.Cdecl)]
        public static extern System.IntPtr GenericValueToPointer(LLVMGenericValueRef* GenVal);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMGenericValueToFloat", CallingConvention=CallingConvention.Cdecl)]
        public static extern double GenericValueToFloat(LLVMTypeRef* TyRef, LLVMGenericValueRef* GenVal);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMDisposeGenericValue", CallingConvention=CallingConvention.Cdecl)]
        public static extern void DisposeGenericValue(LLVMGenericValueRef* GenVal);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMCreateExecutionEngineForModule", CallingConvention=CallingConvention.Cdecl)]
        public static extern int CreateExecutionEngineForModule(ref LLVMExecutionEngineRef * OutEE, LLVMModuleRef* M, [MarshalAs(UnmanagedType.LPStr)] ref StringBuilder OutError);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMCreateInterpreterForModule", CallingConvention=CallingConvention.Cdecl)]
        public static extern int CreateInterpreterForModule(ref LLVMExecutionEngineRef * OutInterp, LLVMModuleRef* M, [MarshalAs(UnmanagedType.LPStr)] ref StringBuilder OutError);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMCreateJITCompilerForModule", CallingConvention=CallingConvention.Cdecl)]
        public static extern int CreateJITCompilerForModule(ref LLVMExecutionEngineRef * OutJIT, LLVMModuleRef* M, uint OptLevel, [MarshalAs(UnmanagedType.LPStr)] ref StringBuilder OutError);

        /* Deprecated: Use LLVMCreateExecutionEngineForModule instead.*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMCreateExecutionEngine", CallingConvention=CallingConvention.Cdecl)]
        public static extern int CreateExecutionEngine(ref LLVMExecutionEngineRef * OutEE, LLVMModuleProviderRef* MP, [MarshalAs(UnmanagedType.LPStr)] ref StringBuilder OutError);

        /* Deprecated: Use LLVMCreateInterpreterForModule instead.*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMCreateInterpreter", CallingConvention=CallingConvention.Cdecl)]
        public static extern int CreateInterpreter(ref LLVMExecutionEngineRef * OutInterp, LLVMModuleProviderRef* MP, [MarshalAs(UnmanagedType.LPStr)] ref StringBuilder OutError);

        /* Deprecated: Use LLVMCreateJITCompilerForModule instead.*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMCreateJITCompiler", CallingConvention=CallingConvention.Cdecl)]
        public static extern int CreateJITCompiler(ref LLVMExecutionEngineRef * OutJIT, LLVMModuleProviderRef* MP, uint OptLevel, [MarshalAs(UnmanagedType.LPStr)] ref StringBuilder OutError);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMDisposeExecutionEngine", CallingConvention=CallingConvention.Cdecl)]
        public static extern void DisposeExecutionEngine(LLVMExecutionEngineRef* EE);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMRunStaticConstructors", CallingConvention=CallingConvention.Cdecl)]
        public static extern void RunStaticConstructors(LLVMExecutionEngineRef* EE);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMRunStaticDestructors", CallingConvention=CallingConvention.Cdecl)]
        public static extern void RunStaticDestructors(LLVMExecutionEngineRef* EE);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMRunFunctionAsMain", CallingConvention=CallingConvention.Cdecl)]
        public static extern int RunFunctionAsMain(LLVMExecutionEngineRef* EE, LLVMValueRef* F, uint ArgC, [MarshalAs(UnmanagedType.LPStr)] ref StringBuilder ArgV, [MarshalAs(UnmanagedType.LPStr)] ref StringBuilder EnvP);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMRunFunction", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMGenericValueRef* RunFunction(LLVMExecutionEngineRef* EE, LLVMValueRef* F, uint NumArgs, System.IntPtr[] Args);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMFreeMachineCodeForFunction", CallingConvention=CallingConvention.Cdecl)]
        public static extern void FreeMachineCodeForFunction(LLVMExecutionEngineRef* EE, LLVMValueRef* F);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMAddModule", CallingConvention=CallingConvention.Cdecl)]
        public static extern void AddModule(LLVMExecutionEngineRef* EE, LLVMModuleRef* M);

        /* Deprecated: Use LLVMAddModule instead.*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMAddModuleProvider", CallingConvention=CallingConvention.Cdecl)]
        public static extern void AddModuleProvider(LLVMExecutionEngineRef* EE, LLVMModuleProviderRef* MP);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMRemoveModule", CallingConvention=CallingConvention.Cdecl)]
        public static extern int RemoveModule(LLVMExecutionEngineRef* EE, LLVMModuleRef* M, ref LLVMModuleRef * OutMod, [MarshalAs(UnmanagedType.LPStr)] ref StringBuilder OutError);

        /* Deprecated: Use LLVMRemoveModule instead.*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMRemoveModuleProvider", CallingConvention=CallingConvention.Cdecl)]
        public static extern int RemoveModuleProvider(LLVMExecutionEngineRef* EE, LLVMModuleProviderRef* MP, ref LLVMModuleRef * OutMod, [MarshalAs(UnmanagedType.LPStr)] ref StringBuilder OutError);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMFindFunction", CallingConvention=CallingConvention.Cdecl)]
        public static extern int FindFunction(LLVMExecutionEngineRef* EE, [In][MarshalAs(UnmanagedType.LPStr)] string Name, ref LLVMValueRef * OutFn);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMRecompileAndRelinkFunction", CallingConvention=CallingConvention.Cdecl)]
        public static extern System.IntPtr RecompileAndRelinkFunction(LLVMExecutionEngineRef* EE, LLVMValueRef* Fn);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMGetExecutionEngineTargetData", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMTargetDataRef* GetExecutionEngineTargetData(LLVMExecutionEngineRef* EE);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMAddGlobalMapping", CallingConvention=CallingConvention.Cdecl)]
        public static extern void AddGlobalMapping(LLVMExecutionEngineRef* EE, LLVMValueRef* Global, System.IntPtr Addr);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMGetPointerToGlobal", CallingConvention=CallingConvention.Cdecl)]
        public static extern System.IntPtr GetPointerToGlobal(LLVMExecutionEngineRef* EE, LLVMValueRef* Global);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMInitializeTransformUtils", CallingConvention=CallingConvention.Cdecl)]
        public static extern void InitializeTransformUtils(LLVMPassRegistryRef* R);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMInitializeScalarOpts", CallingConvention=CallingConvention.Cdecl)]
        public static extern void InitializeScalarOpts(LLVMPassRegistryRef* R);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMInitializeVectorization", CallingConvention=CallingConvention.Cdecl)]
        public static extern void InitializeVectorization(LLVMPassRegistryRef* R);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMInitializeInstCombine", CallingConvention=CallingConvention.Cdecl)]
        public static extern void InitializeInstCombine(LLVMPassRegistryRef* R);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMInitializeIPO", CallingConvention=CallingConvention.Cdecl)]
        public static extern void InitializeIPO(LLVMPassRegistryRef* R);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMInitializeInstrumentation", CallingConvention=CallingConvention.Cdecl)]
        public static extern void InitializeInstrumentation(LLVMPassRegistryRef* R);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMInitializeAnalysis", CallingConvention=CallingConvention.Cdecl)]
        public static extern void InitializeAnalysis(LLVMPassRegistryRef* R);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMInitializeIPA", CallingConvention=CallingConvention.Cdecl)]
        public static extern void InitializeIPA(LLVMPassRegistryRef* R);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMInitializeCodeGen", CallingConvention=CallingConvention.Cdecl)]
        public static extern void InitializeCodeGen(LLVMPassRegistryRef* R);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMInitializeTarget", CallingConvention=CallingConvention.Cdecl)]
        public static extern void InitializeTarget(LLVMPassRegistryRef* R);

        /* This provides C interface to initialize link time optimizer. This allows
          /// linker to use dlopen() interface to dynamically load LinkTimeOptimizer.
          /// extern "C" helps, because dlopen() interface uses name to find the symbol.*/
        [DllImport("llvm-3.1.dll", EntryPoint="llvm_create_optimizer", CallingConvention=CallingConvention.Cdecl)]
        public static extern System.IntPtr llvm_create_optimizer();

        [DllImport("llvm-3.1.dll", EntryPoint="llvm_destroy_optimizer", CallingConvention=CallingConvention.Cdecl)]
        public static extern void llvm_destroy_optimizer(System.IntPtr lto);

        [DllImport("llvm-3.1.dll", EntryPoint="llvm_read_object_file", CallingConvention=CallingConvention.Cdecl)]
        public static extern llvm_lto_status llvm_read_object_file(System.IntPtr lto, [In][MarshalAs(UnmanagedType.LPStr)] string input_filename);

        [DllImport("llvm-3.1.dll", EntryPoint="llvm_optimize_modules", CallingConvention=CallingConvention.Cdecl)]
        public static extern llvm_lto_status llvm_optimize_modules(System.IntPtr lto, [In][MarshalAs(UnmanagedType.LPStr)] string output_filename);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMCreateObjectFile", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMObjectFileRef* CreateObjectFile(LLVMMemoryBufferRef* MemBuf);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMDisposeObjectFile", CallingConvention=CallingConvention.Cdecl)]
        public static extern void DisposeObjectFile(LLVMObjectFileRef* ObjectFile);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMGetSections", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMSectionIteratorRef* GetSections(LLVMObjectFileRef* ObjectFile);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMDisposeSectionIterator", CallingConvention=CallingConvention.Cdecl)]
        public static extern void DisposeSectionIterator(LLVMSectionIteratorRef* SI);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMIsSectionIteratorAtEnd", CallingConvention=CallingConvention.Cdecl)]
        public static extern int IsSectionIteratorAtEnd(LLVMObjectFileRef* ObjectFile, LLVMSectionIteratorRef* SI);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMMoveToNextSection", CallingConvention=CallingConvention.Cdecl)]
        public static extern void MoveToNextSection(LLVMSectionIteratorRef* SI);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMMoveToContainingSection", CallingConvention=CallingConvention.Cdecl)]
        public static extern void MoveToContainingSection(LLVMSectionIteratorRef* Sect, LLVMSymbolIteratorRef* Sym);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMGetSymbols", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMSymbolIteratorRef* GetSymbols(LLVMObjectFileRef* ObjectFile);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMDisposeSymbolIterator", CallingConvention=CallingConvention.Cdecl)]
        public static extern void DisposeSymbolIterator(LLVMSymbolIteratorRef* SI);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMIsSymbolIteratorAtEnd", CallingConvention=CallingConvention.Cdecl)]
        public static extern int IsSymbolIteratorAtEnd(LLVMObjectFileRef* ObjectFile, LLVMSymbolIteratorRef* SI);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMMoveToNextSymbol", CallingConvention=CallingConvention.Cdecl)]
        public static extern void MoveToNextSymbol(LLVMSymbolIteratorRef* SI);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMGetSectionName", CallingConvention=CallingConvention.Cdecl)]
        public static extern System.IntPtr GetSectionName(LLVMSectionIteratorRef* SI);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMGetSectionSize", CallingConvention=CallingConvention.Cdecl)]
        public static extern ulong GetSectionSize(LLVMSectionIteratorRef* SI);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMGetSectionContents", CallingConvention=CallingConvention.Cdecl)]
        public static extern System.IntPtr GetSectionContents(LLVMSectionIteratorRef* SI);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMGetSectionAddress", CallingConvention=CallingConvention.Cdecl)]
        public static extern ulong GetSectionAddress(LLVMSectionIteratorRef* SI);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMGetSectionContainsSymbol", CallingConvention=CallingConvention.Cdecl)]
        public static extern int GetSectionContainsSymbol(LLVMSectionIteratorRef* SI, LLVMSymbolIteratorRef* Sym);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMGetRelocations", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMRelocationIteratorRef* GetRelocations(LLVMSectionIteratorRef* Section);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMDisposeRelocationIterator", CallingConvention=CallingConvention.Cdecl)]
        public static extern void DisposeRelocationIterator(LLVMRelocationIteratorRef* RI);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMIsRelocationIteratorAtEnd", CallingConvention=CallingConvention.Cdecl)]
        public static extern int IsRelocationIteratorAtEnd(LLVMSectionIteratorRef* Section, LLVMRelocationIteratorRef* RI);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMMoveToNextRelocation", CallingConvention=CallingConvention.Cdecl)]
        public static extern void MoveToNextRelocation(LLVMRelocationIteratorRef* RI);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMGetSymbolName", CallingConvention=CallingConvention.Cdecl)]
        public static extern System.IntPtr GetSymbolName(LLVMSymbolIteratorRef* SI);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMGetSymbolAddress", CallingConvention=CallingConvention.Cdecl)]
        public static extern ulong GetSymbolAddress(LLVMSymbolIteratorRef* SI);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMGetSymbolFileOffset", CallingConvention=CallingConvention.Cdecl)]
        public static extern ulong GetSymbolFileOffset(LLVMSymbolIteratorRef* SI);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMGetSymbolSize", CallingConvention=CallingConvention.Cdecl)]
        public static extern ulong GetSymbolSize(LLVMSymbolIteratorRef* SI);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMGetRelocationAddress", CallingConvention=CallingConvention.Cdecl)]
        public static extern ulong GetRelocationAddress(LLVMRelocationIteratorRef* RI);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMGetRelocationOffset", CallingConvention=CallingConvention.Cdecl)]
        public static extern ulong GetRelocationOffset(LLVMRelocationIteratorRef* RI);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMGetRelocationSymbol", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMSymbolIteratorRef* GetRelocationSymbol(LLVMRelocationIteratorRef* RI);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMGetRelocationType", CallingConvention=CallingConvention.Cdecl)]
        public static extern ulong GetRelocationType(LLVMRelocationIteratorRef* RI);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMGetRelocationTypeName", CallingConvention=CallingConvention.Cdecl)]
        public static extern System.IntPtr GetRelocationTypeName(LLVMRelocationIteratorRef* RI);

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMGetRelocationValueString", CallingConvention=CallingConvention.Cdecl)]
        public static extern System.IntPtr GetRelocationValueString(LLVMRelocationIteratorRef* RI);

        /* LLVMInitializeAllTargetInfos - The main program should call this function if
            it wants access to all available targets that LLVM is configured to
            support.*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMInitializeAllTargetInfos", CallingConvention=CallingConvention.Cdecl)]
        public static extern void InitializeAllTargetInfos();

        /* LLVMInitializeAllTargets - The main program should call this function if it
            wants to link in all available targets that LLVM is configured to
            support.*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMInitializeAllTargets", CallingConvention=CallingConvention.Cdecl)]
        public static extern void InitializeAllTargets();

        /* LLVMInitializeAllTargetMCs - The main program should call this function if
            it wants access to all available target MC that LLVM is configured to
            support.*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMInitializeAllTargetMCs", CallingConvention=CallingConvention.Cdecl)]
        public static extern void InitializeAllTargetMCs();

        /* LLVMInitializeAllAsmPrinters - The main program should call this function if
            it wants all asm printers that LLVM is configured to support, to make them
            available via the TargetRegistry.*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMInitializeAllAsmPrinters", CallingConvention=CallingConvention.Cdecl)]
        public static extern void InitializeAllAsmPrinters();

        /* LLVMInitializeAllAsmParsers - The main program should call this function if
            it wants all asm parsers that LLVM is configured to support, to make them
            available via the TargetRegistry.*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMInitializeAllAsmParsers", CallingConvention=CallingConvention.Cdecl)]
        public static extern void InitializeAllAsmParsers();

        /* LLVMInitializeAllDisassemblers - The main program should call this function
            if it wants all disassemblers that LLVM is configured to support, to make
            them available via the TargetRegistry.*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMInitializeAllDisassemblers", CallingConvention=CallingConvention.Cdecl)]
        public static extern void InitializeAllDisassemblers();

        /* LLVMInitializeNativeTarget - The main program should call this function to
            initialize the native target corresponding to the host.  This is useful 
            for JIT applications to ensure that the target gets linked in correctly.*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMInitializeNativeTarget", CallingConvention=CallingConvention.Cdecl)]
        public static extern int InitializeNativeTarget();

        /* Creates target data from a target layout string.
            See the constructor llvm::TargetData::TargetData.*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMCreateTargetData", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMTargetDataRef* CreateTargetData([In][MarshalAs(UnmanagedType.LPStr)] string StringRep);

        /* Adds target data information to a pass manager. This does not take ownership
            of the target data.
            See the method llvm::PassManagerBase::add.*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMAddTargetData", CallingConvention=CallingConvention.Cdecl)]
        public static extern void AddTargetData(LLVMTargetDataRef* LLVMTargetDataRef, LLVMPassManagerRef* LLVMPassManagerRef);

        /* Adds target library information to a pass manager. This does not take
            ownership of the target library info.
            See the method llvm::PassManagerBase::add.*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMAddTargetLibraryInfo", CallingConvention=CallingConvention.Cdecl)]
        public static extern void AddTargetLibraryInfo(LLVMTargetLibraryInfoRef* LLVMTargetLibraryInfoRef, LLVMPassManagerRef* LLVMPassManagerRef);

        /* Converts target data to a target layout string. The string must be disposed
            with LLVMDisposeMessage.
            See the constructor llvm::TargetData::TargetData.*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMCopyStringRepOfTargetData", CallingConvention=CallingConvention.Cdecl)]
        public static extern System.IntPtr CopyStringRepOfTargetData(LLVMTargetDataRef* LLVMTargetDataRef);

        /* Returns the byte order of a target, either LLVMBigEndian or
            LLVMLittleEndian.
            See the method llvm::TargetData::isLittleEndian.*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMByteOrder", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMByteOrdering ByteOrder(LLVMTargetDataRef* LLVMTargetDataRef);

        /* Returns the pointer size in bytes for a target.
            See the method llvm::TargetData::getPointerSize.*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMPointerSize", CallingConvention=CallingConvention.Cdecl)]
        public static extern uint PointerSize(LLVMTargetDataRef* LLVMTargetDataRef);

        /* Returns the integer type that is the same size as a pointer on a target.
            See the method llvm::TargetData::getIntPtrType.*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMIntPtrType", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMTypeRef* IntPtrType(LLVMTargetDataRef* LLVMTargetDataRef);

        /* Computes the size of a type in bytes for a target.
            See the method llvm::TargetData::getTypeSizeInBits.*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMSizeOfTypeInBits", CallingConvention=CallingConvention.Cdecl)]
        public static extern ulong SizeOfTypeInBits(LLVMTargetDataRef* LLVMTargetDataRef, LLVMTypeRef* LLVMTypeRef);

        /* Computes the storage size of a type in bytes for a target.
            See the method llvm::TargetData::getTypeStoreSize.*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMStoreSizeOfType", CallingConvention=CallingConvention.Cdecl)]
        public static extern ulong StoreSizeOfType(LLVMTargetDataRef* LLVMTargetDataRef, LLVMTypeRef* LLVMTypeRef);

        /* Computes the ABI size of a type in bytes for a target.
            See the method llvm::TargetData::getTypeAllocSize.*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMABISizeOfType", CallingConvention=CallingConvention.Cdecl)]
        public static extern ulong ABISizeOfType(LLVMTargetDataRef* LLVMTargetDataRef, LLVMTypeRef* LLVMTypeRef);

        /* Computes the ABI alignment of a type in bytes for a target.
            See the method llvm::TargetData::getTypeABISize.*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMABIAlignmentOfType", CallingConvention=CallingConvention.Cdecl)]
        public static extern uint ABIAlignmentOfType(LLVMTargetDataRef* LLVMTargetDataRef, LLVMTypeRef* LLVMTypeRef);

        /* Computes the call frame alignment of a type in bytes for a target.
            See the method llvm::TargetData::getTypeABISize.*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMCallFrameAlignmentOfType", CallingConvention=CallingConvention.Cdecl)]
        public static extern uint CallFrameAlignmentOfType(LLVMTargetDataRef* LLVMTargetDataRef, LLVMTypeRef* LLVMTypeRef);

        /* Computes the preferred alignment of a type in bytes for a target.
            See the method llvm::TargetData::getTypeABISize.*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMPreferredAlignmentOfType", CallingConvention=CallingConvention.Cdecl)]
        public static extern uint PreferredAlignmentOfType(LLVMTargetDataRef* LLVMTargetDataRef, LLVMTypeRef* LLVMTypeRef);

        /* Computes the preferred alignment of a global variable in bytes for a target.
            See the method llvm::TargetData::getPreferredAlignment.*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMPreferredAlignmentOfGlobal", CallingConvention=CallingConvention.Cdecl)]
        public static extern uint PreferredAlignmentOfGlobal(LLVMTargetDataRef* LLVMTargetDataRef, LLVMValueRef* GlobalVar);

        /* Computes the structure element that contains the byte offset for a target.
            See the method llvm::StructLayout::getElementContainingOffset.*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMElementAtOffset", CallingConvention=CallingConvention.Cdecl)]
        public static extern uint ElementAtOffset(LLVMTargetDataRef* LLVMTargetDataRef, LLVMTypeRef* StructTy, ulong Offset);

        /* Computes the byte offset of the indexed struct element for a target.
            See the method llvm::StructLayout::getElementContainingOffset.*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMOffsetOfElement", CallingConvention=CallingConvention.Cdecl)]
        public static extern ulong OffsetOfElement(LLVMTargetDataRef* LLVMTargetDataRef, LLVMTypeRef* StructTy, uint Element);

        /* Deallocates a TargetData.
            See the destructor llvm::TargetData::~TargetData.*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMDisposeTargetData", CallingConvention=CallingConvention.Cdecl)]
        public static extern void DisposeTargetData(LLVMTargetDataRef* LLVMTargetDataRef);

        /* Returns the first llvm::Target in the registered targets list.*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMGetFirstTarget", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMTargetRef* GetFirstTarget();

        /* Returns the next llvm::Target given a previous one (or null if there's none)*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMGetNextTarget", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMTargetRef* GetNextTarget(LLVMTargetRef* T);

        /* Returns the name of a target. See llvm::Target::getName*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMGetTargetName", CallingConvention=CallingConvention.Cdecl)]
        public static extern System.IntPtr GetTargetName(LLVMTargetRef* T);

        /* Returns the description  of a target. See llvm::Target::getDescription*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMGetTargetDescription", CallingConvention=CallingConvention.Cdecl)]
        public static extern System.IntPtr GetTargetDescription(LLVMTargetRef* T);

        /* Returns if the target has a JIT*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMTargetHasJIT", CallingConvention=CallingConvention.Cdecl)]
        public static extern int TargetHasJIT(LLVMTargetRef* T);

        /* Returns if the target has a TargetMachine associated*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMTargetHasTargetMachine", CallingConvention=CallingConvention.Cdecl)]
        public static extern int TargetHasTargetMachine(LLVMTargetRef* T);

        /* Returns if the target as an ASM backend (required for emitting output)*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMTargetHasAsmBackend", CallingConvention=CallingConvention.Cdecl)]
        public static extern int TargetHasAsmBackend(LLVMTargetRef* T);

        /* Creates a new llvm::TargetMachine. See llvm::Target::createTargetMachine*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMCreateTargetMachine", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMTargetMachineRef* CreateTargetMachine(LLVMTargetRef* T, [In][MarshalAs(UnmanagedType.LPStr)] string Triple, [In][MarshalAs(UnmanagedType.LPStr)] string CPU, [In][MarshalAs(UnmanagedType.LPStr)] string Features, LLVMCodeGenOptLevel Level, LLVMRelocMode Reloc, LLVMCodeModel CodeModel);

        /* Dispose the LLVMTargetMachineRef instance generated by
          LLVMCreateTargetMachine.*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMDisposeTargetMachine", CallingConvention=CallingConvention.Cdecl)]
        public static extern void DisposeTargetMachine(LLVMTargetMachineRef* T);

        /* Returns the Target used in a TargetMachine*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMGetTargetMachineTarget", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMTargetRef* GetTargetMachineTarget(LLVMTargetMachineRef* T);

        /* Returns the triple used creating this target machine. See
          llvm::TargetMachine::getTriple. The result needs to be disposed with
          LLVMDisposeMessage.*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMGetTargetMachineTriple", CallingConvention=CallingConvention.Cdecl)]
        public static extern System.IntPtr GetTargetMachineTriple(LLVMTargetMachineRef* T);

        /* Returns the cpu used creating this target machine. See
          llvm::TargetMachine::getCPU. The result needs to be disposed with
          LLVMDisposeMessage.*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMGetTargetMachineCPU", CallingConvention=CallingConvention.Cdecl)]
        public static extern System.IntPtr GetTargetMachineCPU(LLVMTargetMachineRef* T);

        /* Returns the feature string used creating this target machine. See
          llvm::TargetMachine::getFeatureString. The result needs to be disposed with
          LLVMDisposeMessage.*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMGetTargetMachineFeatureString", CallingConvention=CallingConvention.Cdecl)]
        public static extern System.IntPtr GetTargetMachineFeatureString(LLVMTargetMachineRef* T);

        /* Returns the llvm::TargetData used for this llvm:TargetMachine.*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMGetTargetMachineData", CallingConvention=CallingConvention.Cdecl)]
        public static extern int GetTargetMachineData(LLVMTargetMachineRef* T);

        /* Emits an asm or object file for the given module to the filename. This
          wraps several c++ only classes (among them a file stream). Returns any
          error in ErrorMessage. Use LLVMDisposeMessage to dispose the message.*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMTargetMachineEmitToFile", CallingConvention=CallingConvention.Cdecl)]
        public static extern int TargetMachineEmitToFile(LLVMTargetMachineRef* T, LLVMModuleRef* M, [In][MarshalAs(UnmanagedType.LPStr)] string Filename, LLVMCodeGenFileType codegen, [MarshalAs(UnmanagedType.LPStr)] ref StringBuilder ErrorMessage);

        /* @defgroup LLVMCTransformsIPO Interprocedural transformations
         * @ingroup LLVMCTransforms
         *
         * @{
         */
        
        /** See llvm::createArgumentPromotionPass function.*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMAddArgumentPromotionPass", CallingConvention=CallingConvention.Cdecl)]
        public static extern void AddArgumentPromotionPass(LLVMPassManagerRef* PM);

        /* See llvm::createConstantMergePass function.*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMAddConstantMergePass", CallingConvention=CallingConvention.Cdecl)]
        public static extern void AddConstantMergePass(LLVMPassManagerRef* PM);

        /* See llvm::createDeadArgEliminationPass function.*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMAddDeadArgEliminationPass", CallingConvention=CallingConvention.Cdecl)]
        public static extern void AddDeadArgEliminationPass(LLVMPassManagerRef* PM);

        /* See llvm::createFunctionAttrsPass function.*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMAddFunctionAttrsPass", CallingConvention=CallingConvention.Cdecl)]
        public static extern void AddFunctionAttrsPass(LLVMPassManagerRef* PM);

        /* See llvm::createFunctionInliningPass function.*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMAddFunctionInliningPass", CallingConvention=CallingConvention.Cdecl)]
        public static extern void AddFunctionInliningPass(LLVMPassManagerRef* PM);

        /* See llvm::createAlwaysInlinerPass function.*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMAddAlwaysInlinerPass", CallingConvention=CallingConvention.Cdecl)]
        public static extern void AddAlwaysInlinerPass(LLVMPassManagerRef* PM);

        /* See llvm::createGlobalDCEPass function.*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMAddGlobalDCEPass", CallingConvention=CallingConvention.Cdecl)]
        public static extern void AddGlobalDCEPass(LLVMPassManagerRef* PM);

        /* See llvm::createGlobalOptimizerPass function.*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMAddGlobalOptimizerPass", CallingConvention=CallingConvention.Cdecl)]
        public static extern void AddGlobalOptimizerPass(LLVMPassManagerRef* PM);

        /* See llvm::createIPConstantPropagationPass function.*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMAddIPConstantPropagationPass", CallingConvention=CallingConvention.Cdecl)]
        public static extern void AddIPConstantPropagationPass(LLVMPassManagerRef* PM);

        /* See llvm::createPruneEHPass function.*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMAddPruneEHPass", CallingConvention=CallingConvention.Cdecl)]
        public static extern void AddPruneEHPass(LLVMPassManagerRef* PM);

        /* See llvm::createIPSCCPPass function.*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMAddIPSCCPPass", CallingConvention=CallingConvention.Cdecl)]
        public static extern void AddIPSCCPPass(LLVMPassManagerRef* PM);

        /* See llvm::createInternalizePass function.*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMAddInternalizePass", CallingConvention=CallingConvention.Cdecl)]
        public static extern void AddInternalizePass(LLVMPassManagerRef* LLVMPassManagerRef, uint AllButMain);

        /* See llvm::createStripDeadPrototypesPass function.*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMAddStripDeadPrototypesPass", CallingConvention=CallingConvention.Cdecl)]
        public static extern void AddStripDeadPrototypesPass(LLVMPassManagerRef* PM);

        /* See llvm::createStripSymbolsPass function.*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMAddStripSymbolsPass", CallingConvention=CallingConvention.Cdecl)]
        public static extern void AddStripSymbolsPass(LLVMPassManagerRef* PM);

        /* @defgroup LLVMCTransformsPassManagerBuilder Pass manager builder
         * @ingroup LLVMCTransforms
         *
         * @{
         */
        
        /** See llvm::PassManagerBuilder.*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMPassManagerBuilderCreate", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMPassManagerBuilderRef* PassManagerBuilderCreate();

        [DllImport("llvm-3.1.dll", EntryPoint="LLVMPassManagerBuilderDispose", CallingConvention=CallingConvention.Cdecl)]
        public static extern void PassManagerBuilderDispose(LLVMPassManagerBuilderRef* PMB);

        /* See llvm::PassManagerBuilder::OptLevel.*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMPassManagerBuilderSetOptLevel", CallingConvention=CallingConvention.Cdecl)]
        public static extern void PassManagerBuilderSetOptLevel(LLVMPassManagerBuilderRef* PMB, uint OptLevel);

        /* See llvm::PassManagerBuilder::SizeLevel.*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMPassManagerBuilderSetSizeLevel", CallingConvention=CallingConvention.Cdecl)]
        public static extern void PassManagerBuilderSetSizeLevel(LLVMPassManagerBuilderRef* PMB, uint SizeLevel);

        /* See llvm::PassManagerBuilder::DisableUnitAtATime.*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMPassManagerBuilderSetDisableUnitAtATime", CallingConvention=CallingConvention.Cdecl)]
        public static extern void PassManagerBuilderSetDisableUnitAtATime(LLVMPassManagerBuilderRef* PMB, int Value);

        /* See llvm::PassManagerBuilder::DisableUnrollLoops.*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMPassManagerBuilderSetDisableUnrollLoops", CallingConvention=CallingConvention.Cdecl)]
        public static extern void PassManagerBuilderSetDisableUnrollLoops(LLVMPassManagerBuilderRef* PMB, int Value);

        /* See llvm::PassManagerBuilder::DisableSimplifyLibCalls*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMPassManagerBuilderSetDisableSimplifyLibCalls", CallingConvention=CallingConvention.Cdecl)]
        public static extern void PassManagerBuilderSetDisableSimplifyLibCalls(LLVMPassManagerBuilderRef* PMB, int Value);

        /* See llvm::PassManagerBuilder::Inliner.*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMPassManagerBuilderUseInlinerWithThreshold", CallingConvention=CallingConvention.Cdecl)]
        public static extern void PassManagerBuilderUseInlinerWithThreshold(LLVMPassManagerBuilderRef* PMB, uint Threshold);

        /* See llvm::PassManagerBuilder::populateFunctionPassManager.*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMPassManagerBuilderPopulateFunctionPassManager", CallingConvention=CallingConvention.Cdecl)]
        public static extern void PassManagerBuilderPopulateFunctionPassManager(LLVMPassManagerBuilderRef* PMB, LLVMPassManagerRef* PM);

        /* See llvm::PassManagerBuilder::populateModulePassManager.*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMPassManagerBuilderPopulateModulePassManager", CallingConvention=CallingConvention.Cdecl)]
        public static extern void PassManagerBuilderPopulateModulePassManager(LLVMPassManagerBuilderRef* PMB, LLVMPassManagerRef* PM);

        /* See llvm::PassManagerBuilder::populateLTOPassManager.*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMPassManagerBuilderPopulateLTOPassManager", CallingConvention=CallingConvention.Cdecl)]
        public static extern void PassManagerBuilderPopulateLTOPassManager(LLVMPassManagerBuilderRef* PMB, LLVMPassManagerRef* PM, bool Internalize, bool RunInliner);

        /* @defgroup LLVMCTransformsScalar Scalar transformations
         * @ingroup LLVMCTransforms
         *
         * @{
         */
        
        /** See llvm::createAggressiveDCEPass function.*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMAddAggressiveDCEPass", CallingConvention=CallingConvention.Cdecl)]
        public static extern void AddAggressiveDCEPass(LLVMPassManagerRef* PM);

        /* See llvm::createCFGSimplificationPass function.*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMAddCFGSimplificationPass", CallingConvention=CallingConvention.Cdecl)]
        public static extern void AddCFGSimplificationPass(LLVMPassManagerRef* PM);

        /* See llvm::createDeadStoreEliminationPass function.*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMAddDeadStoreEliminationPass", CallingConvention=CallingConvention.Cdecl)]
        public static extern void AddDeadStoreEliminationPass(LLVMPassManagerRef* PM);

        /* See llvm::createGVNPass function.*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMAddGVNPass", CallingConvention=CallingConvention.Cdecl)]
        public static extern void AddGVNPass(LLVMPassManagerRef* PM);

        /* See llvm::createIndVarSimplifyPass function.*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMAddIndVarSimplifyPass", CallingConvention=CallingConvention.Cdecl)]
        public static extern void AddIndVarSimplifyPass(LLVMPassManagerRef* PM);

        /* See llvm::createInstructionCombiningPass function.*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMAddInstructionCombiningPass", CallingConvention=CallingConvention.Cdecl)]
        public static extern void AddInstructionCombiningPass(LLVMPassManagerRef* PM);

        /* See llvm::createJumpThreadingPass function.*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMAddJumpThreadingPass", CallingConvention=CallingConvention.Cdecl)]
        public static extern void AddJumpThreadingPass(LLVMPassManagerRef* PM);

        /* See llvm::createLICMPass function.*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMAddLICMPass", CallingConvention=CallingConvention.Cdecl)]
        public static extern void AddLICMPass(LLVMPassManagerRef* PM);

        /* See llvm::createLoopDeletionPass function.*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMAddLoopDeletionPass", CallingConvention=CallingConvention.Cdecl)]
        public static extern void AddLoopDeletionPass(LLVMPassManagerRef* PM);

        /* See llvm::createLoopIdiomPass function*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMAddLoopIdiomPass", CallingConvention=CallingConvention.Cdecl)]
        public static extern void AddLoopIdiomPass(LLVMPassManagerRef* PM);

        /* See llvm::createLoopRotatePass function.*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMAddLoopRotatePass", CallingConvention=CallingConvention.Cdecl)]
        public static extern void AddLoopRotatePass(LLVMPassManagerRef* PM);

        /* See llvm::createLoopUnrollPass function.*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMAddLoopUnrollPass", CallingConvention=CallingConvention.Cdecl)]
        public static extern void AddLoopUnrollPass(LLVMPassManagerRef* PM);

        /* See llvm::createLoopUnswitchPass function.*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMAddLoopUnswitchPass", CallingConvention=CallingConvention.Cdecl)]
        public static extern void AddLoopUnswitchPass(LLVMPassManagerRef* PM);

        /* See llvm::createMemCpyOptPass function.*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMAddMemCpyOptPass", CallingConvention=CallingConvention.Cdecl)]
        public static extern void AddMemCpyOptPass(LLVMPassManagerRef* PM);

        /* See llvm::createPromoteMemoryToRegisterPass function.*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMAddPromoteMemoryToRegisterPass", CallingConvention=CallingConvention.Cdecl)]
        public static extern void AddPromoteMemoryToRegisterPass(LLVMPassManagerRef* PM);

        /* See llvm::createReassociatePass function.*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMAddReassociatePass", CallingConvention=CallingConvention.Cdecl)]
        public static extern void AddReassociatePass(LLVMPassManagerRef* PM);

        /* See llvm::createSCCPPass function.*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMAddSCCPPass", CallingConvention=CallingConvention.Cdecl)]
        public static extern void AddSCCPPass(LLVMPassManagerRef* PM);

        /* See llvm::createScalarReplAggregatesPass function.*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMAddScalarReplAggregatesPass", CallingConvention=CallingConvention.Cdecl)]
        public static extern void AddScalarReplAggregatesPass(LLVMPassManagerRef* PM);

        /* See llvm::createScalarReplAggregatesPass function.*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMAddScalarReplAggregatesPassSSA", CallingConvention=CallingConvention.Cdecl)]
        public static extern void AddScalarReplAggregatesPassSSA(LLVMPassManagerRef* PM);

        /* See llvm::createScalarReplAggregatesPass function.*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMAddScalarReplAggregatesPassWithThreshold", CallingConvention=CallingConvention.Cdecl)]
        public static extern void AddScalarReplAggregatesPassWithThreshold(LLVMPassManagerRef* PM, int Threshold);

        /* See llvm::createSimplifyLibCallsPass function.*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMAddSimplifyLibCallsPass", CallingConvention=CallingConvention.Cdecl)]
        public static extern void AddSimplifyLibCallsPass(LLVMPassManagerRef* PM);

        /* See llvm::createTailCallEliminationPass function.*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMAddTailCallEliminationPass", CallingConvention=CallingConvention.Cdecl)]
        public static extern void AddTailCallEliminationPass(LLVMPassManagerRef* PM);

        /* See llvm::createConstantPropagationPass function.*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMAddConstantPropagationPass", CallingConvention=CallingConvention.Cdecl)]
        public static extern void AddConstantPropagationPass(LLVMPassManagerRef* PM);

        /* See llvm::demotePromoteMemoryToRegisterPass function.*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMAddDemoteMemoryToRegisterPass", CallingConvention=CallingConvention.Cdecl)]
        public static extern void AddDemoteMemoryToRegisterPass(LLVMPassManagerRef* PM);

        /* See llvm::createVerifierPass function.*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMAddVerifierPass", CallingConvention=CallingConvention.Cdecl)]
        public static extern void AddVerifierPass(LLVMPassManagerRef* PM);

        /* See llvm::createCorrelatedValuePropagationPass function*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMAddCorrelatedValuePropagationPass", CallingConvention=CallingConvention.Cdecl)]
        public static extern void AddCorrelatedValuePropagationPass(LLVMPassManagerRef* PM);

        /* See llvm::createEarlyCSEPass function*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMAddEarlyCSEPass", CallingConvention=CallingConvention.Cdecl)]
        public static extern void AddEarlyCSEPass(LLVMPassManagerRef* PM);

        /* See llvm::createLowerExpectIntrinsicPass function*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMAddLowerExpectIntrinsicPass", CallingConvention=CallingConvention.Cdecl)]
        public static extern void AddLowerExpectIntrinsicPass(LLVMPassManagerRef* PM);

        /* See llvm::createTypeBasedAliasAnalysisPass function*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMAddTypeBasedAliasAnalysisPass", CallingConvention=CallingConvention.Cdecl)]
        public static extern void AddTypeBasedAliasAnalysisPass(LLVMPassManagerRef* PM);

        /* See llvm::createBasicAliasAnalysisPass function*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMAddBasicAliasAnalysisPass", CallingConvention=CallingConvention.Cdecl)]
        public static extern void AddBasicAliasAnalysisPass(LLVMPassManagerRef* PM);

        /* @defgroup LLVMCTransformsVectorize Vectorization transformations
         * @ingroup LLVMCTransforms
         *
         * @{
         */
        
        /** See llvm::createBBVectorizePass function.*/
        [DllImport("llvm-3.1.dll", EntryPoint="LLVMAddBBVectorizePass", CallingConvention=CallingConvention.Cdecl)]
        public static extern void AddBBVectorizePass(LLVMPassManagerRef* PM);

    }
}