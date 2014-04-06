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
		const string llvmLibName = "llvm-3.2.dll";


        [DllImport(llvmLibName, EntryPoint="LLVMVerifyModule", CallingConvention=CallingConvention.Cdecl)]
        public static extern int VerifyModule(LLVMModuleRef* M, LLVMVerifierFailureAction Action, [MarshalAs(UnmanagedType.LPStr)] ref StringBuilder OutMessage);

        [DllImport(llvmLibName, EntryPoint="LLVMVerifyFunction", CallingConvention=CallingConvention.Cdecl)]
        public static extern int VerifyFunction(LLVMValueRef* Fn, LLVMVerifierFailureAction Action);

        [DllImport(llvmLibName, EntryPoint="LLVMViewFunctionCFG", CallingConvention=CallingConvention.Cdecl)]
        public static extern void ViewFunctionCFG(LLVMValueRef* Fn);

        [DllImport(llvmLibName, EntryPoint="LLVMViewFunctionCFGOnly", CallingConvention=CallingConvention.Cdecl)]
        public static extern void ViewFunctionCFGOnly(LLVMValueRef* Fn);

        [DllImport(llvmLibName, EntryPoint="LLVMParseBitcode", CallingConvention=CallingConvention.Cdecl)]
        public static extern int ParseBitcode(LLVMMemoryBufferRef* MemBuf, ref LLVMModuleRef * OutModule, [MarshalAs(UnmanagedType.LPStr)] ref StringBuilder OutMessage);

        [DllImport(llvmLibName, EntryPoint="LLVMParseBitcodeInContext", CallingConvention=CallingConvention.Cdecl)]
        public static extern int ParseBitcodeInContext(LLVMContextRef* ContextRef, LLVMMemoryBufferRef* MemBuf, ref LLVMModuleRef * OutModule, [MarshalAs(UnmanagedType.LPStr)] ref StringBuilder OutMessage);

        /* Reads a module from the specified path, returning via the OutMP parameter
            a module provider which performs lazy deserialization. Returns 0 on success.
            Optionally returns a human-readable error message via OutMessage.*/
        [DllImport(llvmLibName, EntryPoint="LLVMGetBitcodeModuleInContext", CallingConvention=CallingConvention.Cdecl)]
        public static extern int GetBitcodeModuleInContext(LLVMContextRef* ContextRef, LLVMMemoryBufferRef* MemBuf, ref LLVMModuleRef * OutM, [MarshalAs(UnmanagedType.LPStr)] ref StringBuilder OutMessage);

        [DllImport(llvmLibName, EntryPoint="LLVMGetBitcodeModule", CallingConvention=CallingConvention.Cdecl)]
        public static extern int GetBitcodeModule(LLVMMemoryBufferRef* MemBuf, ref LLVMModuleRef * OutM, [MarshalAs(UnmanagedType.LPStr)] ref StringBuilder OutMessage);

        /* Deprecated: Use LLVMGetBitcodeModuleInContext instead.*/
        [DllImport(llvmLibName, EntryPoint="LLVMGetBitcodeModuleProviderInContext", CallingConvention=CallingConvention.Cdecl)]
        public static extern int GetBitcodeModuleProviderInContext(LLVMContextRef* ContextRef, LLVMMemoryBufferRef* MemBuf, ref LLVMModuleProviderRef * OutMP, [MarshalAs(UnmanagedType.LPStr)] ref StringBuilder OutMessage);

        /* Deprecated: Use LLVMGetBitcodeModule instead.*/
        [DllImport(llvmLibName, EntryPoint="LLVMGetBitcodeModuleProvider", CallingConvention=CallingConvention.Cdecl)]
        public static extern int GetBitcodeModuleProvider(LLVMMemoryBufferRef* MemBuf, ref LLVMModuleProviderRef * OutMP, [MarshalAs(UnmanagedType.LPStr)] ref StringBuilder OutMessage);

        /* @defgroup LLVMCBitWriter Bit Writer
         * @ingroup LLVMC
         *
         * @{
         */
        
        /*===-- Operations on modules ---------------------------------------------===*/
        
        /** Writes a module to the specified path. Returns 0 on success.*/
        [DllImport(llvmLibName, EntryPoint="LLVMWriteBitcodeToFile", CallingConvention=CallingConvention.Cdecl)]
        public static extern int WriteBitcodeToFile(LLVMModuleRef* M, [In][MarshalAs(UnmanagedType.LPStr)] string Path);

        /* Writes a module to an open file descriptor. Returns 0 on success.*/
        [DllImport(llvmLibName, EntryPoint="LLVMWriteBitcodeToFD", CallingConvention=CallingConvention.Cdecl)]
        public static extern int WriteBitcodeToFD(LLVMModuleRef* M, int FD, int ShouldClose, int Unbuffered);

        /* Deprecated for LLVMWriteBitcodeToFD. Writes a module to an open file
            descriptor. Returns 0 on success. Closes the Handle.*/
        [DllImport(llvmLibName, EntryPoint="LLVMWriteBitcodeToFileHandle", CallingConvention=CallingConvention.Cdecl)]
        public static extern int WriteBitcodeToFileHandle(LLVMModuleRef* M, int Handle);

        /* @}*/
        [DllImport(llvmLibName, EntryPoint="LLVMInitializeCore", CallingConvention=CallingConvention.Cdecl)]
        public static extern void InitializeCore(LLVMPassRegistryRef* R);

        [DllImport(llvmLibName, EntryPoint="LLVMDisposeMessage", CallingConvention=CallingConvention.Cdecl)]
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
        [DllImport(llvmLibName, EntryPoint="LLVMContextCreate", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMContextRef* ContextCreate();

        /* Obtain the global context instance.*/
        [DllImport(llvmLibName, EntryPoint="LLVMGetGlobalContext", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMContextRef* GetGlobalContext();

        /* Destroy a context instance.
         *
         * This should be called for every call to LLVMContextCreate() or memory
         * will be leaked.*/
        [DllImport(llvmLibName, EntryPoint="LLVMContextDispose", CallingConvention=CallingConvention.Cdecl)]
        public static extern void ContextDispose(LLVMContextRef* C);

        [DllImport(llvmLibName, EntryPoint="LLVMGetMDKindIDInContext", CallingConvention=CallingConvention.Cdecl)]
        public static extern uint GetMDKindIDInContext(LLVMContextRef* C, [In][MarshalAs(UnmanagedType.LPStr)] string Name, uint SLen);

        [DllImport(llvmLibName, EntryPoint="LLVMGetMDKindID", CallingConvention=CallingConvention.Cdecl)]
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
        [DllImport(llvmLibName, EntryPoint="LLVMModuleCreateWithName", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMModuleRef* ModuleCreateWithName([In][MarshalAs(UnmanagedType.LPStr)] string ModuleID);

        /* Create a new, empty module in a specific context.
         *
         * Every invocation should be paired with LLVMDisposeModule() or memory
         * will be leaked.*/
        [DllImport(llvmLibName, EntryPoint="LLVMModuleCreateWithNameInContext", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMModuleRef* ModuleCreateWithNameInContext([In][MarshalAs(UnmanagedType.LPStr)] string ModuleID, LLVMContextRef* C);

        /* Destroy a module instance.
         *
         * This must be called for every created module or memory will be
         * leaked.*/
        [DllImport(llvmLibName, EntryPoint="LLVMDisposeModule", CallingConvention=CallingConvention.Cdecl)]
        public static extern void DisposeModule(LLVMModuleRef* M);

        /* Obtain the data layout for a module.
         *
         * @see Module::getDataLayout()*/
        [DllImport(llvmLibName, EntryPoint="LLVMGetDataLayout", CallingConvention=CallingConvention.Cdecl)]
        public static extern System.IntPtr GetDataLayout(LLVMModuleRef* M);

        /* Set the data layout for a module.
         *
         * @see Module::setDataLayout()*/
        [DllImport(llvmLibName, EntryPoint="LLVMSetDataLayout", CallingConvention=CallingConvention.Cdecl)]
        public static extern void SetDataLayout(LLVMModuleRef* M, [In][MarshalAs(UnmanagedType.LPStr)] string Triple);

        /* Obtain the target triple for a module.
         *
         * @see Module::getTargetTriple()*/
        [DllImport(llvmLibName, EntryPoint="LLVMGetTarget", CallingConvention=CallingConvention.Cdecl)]
        public static extern System.IntPtr GetTarget(LLVMModuleRef* M);

        /* Set the target triple for a module.
         *
         * @see Module::setTargetTriple()*/
        [DllImport(llvmLibName, EntryPoint="LLVMSetTarget", CallingConvention=CallingConvention.Cdecl)]
        public static extern void SetTarget(LLVMModuleRef* M, [In][MarshalAs(UnmanagedType.LPStr)] string Triple);

        /* Dump a representation of a module to stderr.
         *
         * @see Module::dump()*/
        [DllImport(llvmLibName, EntryPoint="LLVMDumpModule", CallingConvention=CallingConvention.Cdecl)]
        public static extern void DumpModule(LLVMModuleRef* M);

        /* Set inline assembly for a module.
         *
         * @see Module::setModuleInlineAsm()*/
        [DllImport(llvmLibName, EntryPoint="LLVMSetModuleInlineAsm", CallingConvention=CallingConvention.Cdecl)]
        public static extern void SetModuleInlineAsm(LLVMModuleRef* M, [In][MarshalAs(UnmanagedType.LPStr)] string Asm);

        /* Obtain the context to which this module is associated.
         *
         * @see Module::getContext()*/
        [DllImport(llvmLibName, EntryPoint="LLVMGetModuleContext", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMContextRef* GetModuleContext(LLVMModuleRef* M);

        /* Obtain a Type from a module by its registered name.*/
        [DllImport(llvmLibName, EntryPoint="LLVMGetTypeByName", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMTypeRef* GetTypeByName(LLVMModuleRef* M, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        /* Obtain the number of operands for named metadata in a module.
         *
         * @see llvm::Module::getNamedMetadata()*/
        [DllImport(llvmLibName, EntryPoint="LLVMGetNamedMetadataNumOperands", CallingConvention=CallingConvention.Cdecl)]
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
        [DllImport(llvmLibName, EntryPoint="LLVMGetNamedMetadataOperands", CallingConvention=CallingConvention.Cdecl)]
        public static extern void GetNamedMetadataOperands(LLVMModuleRef* M, [In][MarshalAs(UnmanagedType.LPStr)] string name, System.IntPtr[] Dest);

        /* Add an operand to named metadata.
         *
         * @see llvm::Module::getNamedMetadata()
         * @see llvm::MDNode::addOperand()*/
        [DllImport(llvmLibName, EntryPoint="LLVMAddNamedMetadataOperand", CallingConvention=CallingConvention.Cdecl)]
        public static extern void AddNamedMetadataOperand(LLVMModuleRef* M, [In][MarshalAs(UnmanagedType.LPStr)] string name, LLVMValueRef* Val);

        /* Add a function to a module under a specified name.
         *
         * @see llvm::Function::Create()*/
        [DllImport(llvmLibName, EntryPoint="LLVMAddFunction", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* AddFunction(LLVMModuleRef* M, [In][MarshalAs(UnmanagedType.LPStr)] string Name, LLVMTypeRef* FunctionTy);

        /* Obtain a Function value from a Module by its name.
         *
         * The returned value corresponds to a llvm::Function value.
         *
         * @see llvm::Module::getFunction()*/
        [DllImport(llvmLibName, EntryPoint="LLVMGetNamedFunction", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* GetNamedFunction(LLVMModuleRef* M, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        /* Obtain an iterator to the first Function in a Module.
         *
         * @see llvm::Module::begin()*/
        [DllImport(llvmLibName, EntryPoint="LLVMGetFirstFunction", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* GetFirstFunction(LLVMModuleRef* M);

        /* Obtain an iterator to the last Function in a Module.
         *
         * @see llvm::Module::end()*/
        [DllImport(llvmLibName, EntryPoint="LLVMGetLastFunction", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* GetLastFunction(LLVMModuleRef* M);

        /* Advance a Function iterator to the next Function.
         *
         * Returns NULL if the iterator was already at the end and there are no more
         * functions.*/
        [DllImport(llvmLibName, EntryPoint="LLVMGetNextFunction", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* GetNextFunction(LLVMValueRef* Fn);

        /* Decrement a Function iterator to the previous Function.
         *
         * Returns NULL if the iterator was already at the beginning and there are
         * no previous functions.*/
        [DllImport(llvmLibName, EntryPoint="LLVMGetPreviousFunction", CallingConvention=CallingConvention.Cdecl)]
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
        [DllImport(llvmLibName, EntryPoint="LLVMGetTypeKind", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMTypeKind GetTypeKind(LLVMTypeRef* Ty);

        /* Whether the type has a known size.
         *
         * Things that don't have a size are abstract types, labels, and void.a
         *
         * @see llvm::Type::isSized()*/
        [DllImport(llvmLibName, EntryPoint="LLVMTypeIsSized", CallingConvention=CallingConvention.Cdecl)]
        public static extern int TypeIsSized(LLVMTypeRef* Ty);

        /* Obtain the context to which this type instance is associated.
         *
         * @see llvm::Type::getContext()*/
        [DllImport(llvmLibName, EntryPoint="LLVMGetTypeContext", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMContextRef* GetTypeContext(LLVMTypeRef* Ty);

        /* @defgroup LLVMCCoreTypeInt Integer Types
         *
         * Functions in this section operate on integer types.
         *
         * @{
         */
        
        /**
         * Obtain an integer type from a context with specified bit width.*/
        [DllImport(llvmLibName, EntryPoint="LLVMInt1TypeInContext", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMTypeRef* Int1TypeInContext(LLVMContextRef* C);

        [DllImport(llvmLibName, EntryPoint="LLVMInt8TypeInContext", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMTypeRef* Int8TypeInContext(LLVMContextRef* C);

        [DllImport(llvmLibName, EntryPoint="LLVMInt16TypeInContext", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMTypeRef* Int16TypeInContext(LLVMContextRef* C);

        [DllImport(llvmLibName, EntryPoint="LLVMInt32TypeInContext", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMTypeRef* Int32TypeInContext(LLVMContextRef* C);

        [DllImport(llvmLibName, EntryPoint="LLVMInt64TypeInContext", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMTypeRef* Int64TypeInContext(LLVMContextRef* C);

        [DllImport(llvmLibName, EntryPoint="LLVMIntTypeInContext", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMTypeRef* IntTypeInContext(LLVMContextRef* C, uint NumBits);

        /* Obtain an integer type from the global context with a specified bit
         * width.*/
        [DllImport(llvmLibName, EntryPoint="LLVMInt1Type", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMTypeRef* Int1Type();

        [DllImport(llvmLibName, EntryPoint="LLVMInt8Type", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMTypeRef* Int8Type();

        [DllImport(llvmLibName, EntryPoint="LLVMInt16Type", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMTypeRef* Int16Type();

        [DllImport(llvmLibName, EntryPoint="LLVMInt32Type", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMTypeRef* Int32Type();

        [DllImport(llvmLibName, EntryPoint="LLVMInt64Type", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMTypeRef* Int64Type();

        [DllImport(llvmLibName, EntryPoint="LLVMIntType", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMTypeRef* IntType(uint NumBits);

        [DllImport(llvmLibName, EntryPoint="LLVMGetIntTypeWidth", CallingConvention=CallingConvention.Cdecl)]
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
        [DllImport(llvmLibName, EntryPoint="LLVMHalfTypeInContext", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMTypeRef* HalfTypeInContext(LLVMContextRef* C);

        /* Obtain a 32-bit floating point type from a context.*/
        [DllImport(llvmLibName, EntryPoint="LLVMFloatTypeInContext", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMTypeRef* FloatTypeInContext(LLVMContextRef* C);

        /* Obtain a 64-bit floating point type from a context.*/
        [DllImport(llvmLibName, EntryPoint="LLVMDoubleTypeInContext", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMTypeRef* DoubleTypeInContext(LLVMContextRef* C);

        /* Obtain a 80-bit floating point type (X87) from a context.*/
        [DllImport(llvmLibName, EntryPoint="LLVMX86FP80TypeInContext", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMTypeRef* X86FP80TypeInContext(LLVMContextRef* C);

        /* Obtain a 128-bit floating point type (112-bit mantissa) from a
         * context.*/
        [DllImport(llvmLibName, EntryPoint="LLVMFP128TypeInContext", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMTypeRef* FP128TypeInContext(LLVMContextRef* C);

        /* Obtain a 128-bit floating point type (two 64-bits) from a context.*/
        [DllImport(llvmLibName, EntryPoint="LLVMPPCFP128TypeInContext", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMTypeRef* PPCFP128TypeInContext(LLVMContextRef* C);

        /* Obtain a floating point type from the global context.
         *
         * These map to the functions in this group of the same name.*/
        [DllImport(llvmLibName, EntryPoint="LLVMHalfType", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMTypeRef* HalfType();

        [DllImport(llvmLibName, EntryPoint="LLVMFloatType", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMTypeRef* FloatType();

        [DllImport(llvmLibName, EntryPoint="LLVMDoubleType", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMTypeRef* DoubleType();

        [DllImport(llvmLibName, EntryPoint="LLVMX86FP80Type", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMTypeRef* X86FP80Type();

        [DllImport(llvmLibName, EntryPoint="LLVMFP128Type", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMTypeRef* FP128Type();

        [DllImport(llvmLibName, EntryPoint="LLVMPPCFP128Type", CallingConvention=CallingConvention.Cdecl)]
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
        [DllImport(llvmLibName, EntryPoint="LLVMFunctionType", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMTypeRef* FunctionType(LLVMTypeRef* ReturnType, System.IntPtr[] ParamTypes, uint ParamCount, int IsVarArg);

        /* Returns whether a function type is variadic.*/
        [DllImport(llvmLibName, EntryPoint="LLVMIsFunctionVarArg", CallingConvention=CallingConvention.Cdecl)]
        public static extern int IsFunctionVarArg(LLVMTypeRef* FunctionTy);

        /* Obtain the Type this function Type returns.*/
        [DllImport(llvmLibName, EntryPoint="LLVMGetReturnType", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMTypeRef* GetReturnType(LLVMTypeRef* FunctionTy);

        /* Obtain the number of parameters this function accepts.*/
        [DllImport(llvmLibName, EntryPoint="LLVMCountParamTypes", CallingConvention=CallingConvention.Cdecl)]
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
        [DllImport(llvmLibName, EntryPoint="LLVMGetParamTypes", CallingConvention=CallingConvention.Cdecl)]
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
        [DllImport(llvmLibName, EntryPoint="LLVMStructTypeInContext", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMTypeRef* StructTypeInContext(LLVMContextRef* C, System.IntPtr[] ElementTypes, uint ElementCount, int Packed);

        /* Create a new structure type in the global context.
         *
         * @see llvm::StructType::create()*/
        [DllImport(llvmLibName, EntryPoint="LLVMStructType", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMTypeRef* StructType(System.IntPtr[] ElementTypes, uint ElementCount, int Packed);

        /* Create an empty structure in a context having a specified name.
         *
         * @see llvm::StructType::create()*/
        [DllImport(llvmLibName, EntryPoint="LLVMStructCreateNamed", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMTypeRef* StructCreateNamed(LLVMContextRef* C, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        /* Obtain the name of a structure.
         *
         * @see llvm::StructType::getName()*/
        [DllImport(llvmLibName, EntryPoint="LLVMGetStructName", CallingConvention=CallingConvention.Cdecl)]
        public static extern System.IntPtr GetStructName(LLVMTypeRef* Ty);

        /* Set the contents of a structure type.
         *
         * @see llvm::StructType::setBody()*/
        [DllImport(llvmLibName, EntryPoint="LLVMStructSetBody", CallingConvention=CallingConvention.Cdecl)]
        public static extern void StructSetBody(LLVMTypeRef* StructTy, System.IntPtr[] ElementTypes, uint ElementCount, int Packed);

        /* Get the number of elements defined inside the structure.
         *
         * @see llvm::StructType::getNumElements()*/
        [DllImport(llvmLibName, EntryPoint="LLVMCountStructElementTypes", CallingConvention=CallingConvention.Cdecl)]
        public static extern uint CountStructElementTypes(LLVMTypeRef* StructTy);

        /* Get the elements within a structure.
         *
         * The function is passed the address of a pre-allocated array of
         * LLVMTypeRef at least LLVMCountStructElementTypes() long. After
         * invocation, this array will be populated with the structure's
         * elements. The objects in the destination array will have a lifetime
         * of the structure type itself, which is the lifetime of the context it
         * is contained in.*/
        [DllImport(llvmLibName, EntryPoint="LLVMGetStructElementTypes", CallingConvention=CallingConvention.Cdecl)]
        public static extern void GetStructElementTypes(LLVMTypeRef* StructTy, System.IntPtr[] Dest);

        /* Determine whether a structure is packed.
         *
         * @see llvm::StructType::isPacked()*/
        [DllImport(llvmLibName, EntryPoint="LLVMIsPackedStruct", CallingConvention=CallingConvention.Cdecl)]
        public static extern int IsPackedStruct(LLVMTypeRef* StructTy);

        /* Determine whether a structure is opaque.
         *
         * @see llvm::StructType::isOpaque()*/
        [DllImport(llvmLibName, EntryPoint="LLVMIsOpaqueStruct", CallingConvention=CallingConvention.Cdecl)]
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
        [DllImport(llvmLibName, EntryPoint="LLVMGetElementType", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMTypeRef* GetElementType(LLVMTypeRef* Ty);

        /* Create a fixed size array type that refers to a specific type.
         *
         * The created type will exist in the context that its element type
         * exists in.
         *
         * @see llvm::ArrayType::get()*/
        [DllImport(llvmLibName, EntryPoint="LLVMArrayType", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMTypeRef* ArrayType(LLVMTypeRef* ElementType, uint ElementCount);

        /* Obtain the length of an array type.
         *
         * This only works on types that represent arrays.
         *
         * @see llvm::ArrayType::getNumElements()*/
        [DllImport(llvmLibName, EntryPoint="LLVMGetArrayLength", CallingConvention=CallingConvention.Cdecl)]
        public static extern uint GetArrayLength(LLVMTypeRef* ArrayTy);

        /* Create a pointer type that points to a defined type.
         *
         * The created type will exist in the context that its pointee type
         * exists in.
         *
         * @see llvm::PointerType::get()*/
        [DllImport(llvmLibName, EntryPoint="LLVMPointerType", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMTypeRef* PointerType(LLVMTypeRef* ElementType, uint AddressSpace);

        /* Obtain the address space of a pointer type.
         *
         * This only works on types that represent pointers.
         *
         * @see llvm::PointerType::getAddressSpace()*/
        [DllImport(llvmLibName, EntryPoint="LLVMGetPointerAddressSpace", CallingConvention=CallingConvention.Cdecl)]
        public static extern uint GetPointerAddressSpace(LLVMTypeRef* PointerTy);

        /* Create a vector type that contains a defined type and has a specific
         * number of elements.
         *
         * The created type will exist in the context thats its element type
         * exists in.
         *
         * @see llvm::VectorType::get()*/
        [DllImport(llvmLibName, EntryPoint="LLVMVectorType", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMTypeRef* VectorType(LLVMTypeRef* ElementType, uint ElementCount);

        /* Obtain the number of elements in a vector type.
         *
         * This only works on types that represent vectors.
         *
         * @see llvm::VectorType::getNumElements()*/
        [DllImport(llvmLibName, EntryPoint="LLVMGetVectorSize", CallingConvention=CallingConvention.Cdecl)]
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
        [DllImport(llvmLibName, EntryPoint="LLVMVoidTypeInContext", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMTypeRef* VoidTypeInContext(LLVMContextRef* C);

        /* Create a label type in a context.*/
        [DllImport(llvmLibName, EntryPoint="LLVMLabelTypeInContext", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMTypeRef* LabelTypeInContext(LLVMContextRef* C);

        /* Create a X86 MMX type in a context.*/
        [DllImport(llvmLibName, EntryPoint="LLVMX86MMXTypeInContext", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMTypeRef* X86MMXTypeInContext(LLVMContextRef* C);

        /* These are similar to the above functions except they operate on the
         * global context.*/
        [DllImport(llvmLibName, EntryPoint="LLVMVoidType", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMTypeRef* VoidType();

        [DllImport(llvmLibName, EntryPoint="LLVMLabelType", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMTypeRef* LabelType();

        [DllImport(llvmLibName, EntryPoint="LLVMX86MMXType", CallingConvention=CallingConvention.Cdecl)]
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
        [DllImport(llvmLibName, EntryPoint="LLVMTypeOf", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMTypeRef* TypeOf(LLVMValueRef* Val);

        /* Obtain the string name of a value.
         *
         * @see llvm::Value::getName()*/
        [DllImport(llvmLibName, EntryPoint="LLVMGetValueName", CallingConvention=CallingConvention.Cdecl)]
        public static extern System.IntPtr GetValueName(LLVMValueRef* Val);

        /* Set the string name of a value.
         *
         * @see llvm::Value::setName()*/
        [DllImport(llvmLibName, EntryPoint="LLVMSetValueName", CallingConvention=CallingConvention.Cdecl)]
        public static extern void SetValueName(LLVMValueRef* Val, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        /* Dump a representation of a value to stderr.
         *
         * @see llvm::Value::dump()*/
        [DllImport(llvmLibName, EntryPoint="LLVMDumpValue", CallingConvention=CallingConvention.Cdecl)]
        public static extern void DumpValue(LLVMValueRef* Val);

        /* Replace all uses of a value with another one.
         *
         * @see llvm::Value::replaceAllUsesWith()*/
        [DllImport(llvmLibName, EntryPoint="LLVMReplaceAllUsesWith", CallingConvention=CallingConvention.Cdecl)]
        public static extern void ReplaceAllUsesWith(LLVMValueRef* OldVal, LLVMValueRef* NewVal);

        /* Determine whether the specified constant instance is constant.*/
        [DllImport(llvmLibName, EntryPoint="LLVMIsConstant", CallingConvention=CallingConvention.Cdecl)]
        public static extern int IsConstant(LLVMValueRef* Val);

        /* Determine whether a value instance is undefined.*/
        [DllImport(llvmLibName, EntryPoint="LLVMIsUndef", CallingConvention=CallingConvention.Cdecl)]
        public static extern int IsUndef(LLVMValueRef* Val);

        [DllImport(llvmLibName, EntryPoint="LLVMIsAArgument", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* IsAArgument(LLVMValueRef* Val);

        [DllImport(llvmLibName, EntryPoint="LLVMIsABasicBlock", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* IsABasicBlock(LLVMValueRef* Val);

        [DllImport(llvmLibName, EntryPoint="LLVMIsAInlineAsm", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* IsAInlineAsm(LLVMValueRef* Val);

        [DllImport(llvmLibName, EntryPoint="LLVMIsAMDNode", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* IsAMDNode(LLVMValueRef* Val);

        [DllImport(llvmLibName, EntryPoint="LLVMIsAMDString", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* IsAMDString(LLVMValueRef* Val);

        [DllImport(llvmLibName, EntryPoint="LLVMIsAUser", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* IsAUser(LLVMValueRef* Val);

        [DllImport(llvmLibName, EntryPoint="LLVMIsAConstant", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* IsAConstant(LLVMValueRef* Val);

        [DllImport(llvmLibName, EntryPoint="LLVMIsABlockAddress", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* IsABlockAddress(LLVMValueRef* Val);

        [DllImport(llvmLibName, EntryPoint="LLVMIsAConstantAggregateZero", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* IsAConstantAggregateZero(LLVMValueRef* Val);

        [DllImport(llvmLibName, EntryPoint="LLVMIsAConstantArray", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* IsAConstantArray(LLVMValueRef* Val);

        [DllImport(llvmLibName, EntryPoint="LLVMIsAConstantExpr", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* IsAConstantExpr(LLVMValueRef* Val);

        [DllImport(llvmLibName, EntryPoint="LLVMIsAConstantFP", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* IsAConstantFP(LLVMValueRef* Val);

        [DllImport(llvmLibName, EntryPoint="LLVMIsAConstantInt", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* IsAConstantInt(LLVMValueRef* Val);

        [DllImport(llvmLibName, EntryPoint="LLVMIsAConstantPointerNull", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* IsAConstantPointerNull(LLVMValueRef* Val);

        [DllImport(llvmLibName, EntryPoint="LLVMIsAConstantStruct", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* IsAConstantStruct(LLVMValueRef* Val);

        [DllImport(llvmLibName, EntryPoint="LLVMIsAConstantVector", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* IsAConstantVector(LLVMValueRef* Val);

        [DllImport(llvmLibName, EntryPoint="LLVMIsAGlobalValue", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* IsAGlobalValue(LLVMValueRef* Val);

        [DllImport(llvmLibName, EntryPoint="LLVMIsAFunction", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* IsAFunction(LLVMValueRef* Val);

        [DllImport(llvmLibName, EntryPoint="LLVMIsAGlobalAlias", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* IsAGlobalAlias(LLVMValueRef* Val);

        [DllImport(llvmLibName, EntryPoint="LLVMIsAGlobalVariable", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* IsAGlobalVariable(LLVMValueRef* Val);

        [DllImport(llvmLibName, EntryPoint="LLVMIsAUndefValue", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* IsAUndefValue(LLVMValueRef* Val);

        [DllImport(llvmLibName, EntryPoint="LLVMIsAInstruction", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* IsAInstruction(LLVMValueRef* Val);

        [DllImport(llvmLibName, EntryPoint="LLVMIsABinaryOperator", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* IsABinaryOperator(LLVMValueRef* Val);

        [DllImport(llvmLibName, EntryPoint="LLVMIsACallInst", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* IsACallInst(LLVMValueRef* Val);

        [DllImport(llvmLibName, EntryPoint="LLVMIsAIntrinsicInst", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* IsAIntrinsicInst(LLVMValueRef* Val);

        [DllImport(llvmLibName, EntryPoint="LLVMIsADbgInfoIntrinsic", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* IsADbgInfoIntrinsic(LLVMValueRef* Val);

        [DllImport(llvmLibName, EntryPoint="LLVMIsADbgDeclareInst", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* IsADbgDeclareInst(LLVMValueRef* Val);

        [DllImport(llvmLibName, EntryPoint="LLVMIsAMemIntrinsic", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* IsAMemIntrinsic(LLVMValueRef* Val);

        [DllImport(llvmLibName, EntryPoint="LLVMIsAMemCpyInst", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* IsAMemCpyInst(LLVMValueRef* Val);

        [DllImport(llvmLibName, EntryPoint="LLVMIsAMemMoveInst", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* IsAMemMoveInst(LLVMValueRef* Val);

        [DllImport(llvmLibName, EntryPoint="LLVMIsAMemSetInst", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* IsAMemSetInst(LLVMValueRef* Val);

        [DllImport(llvmLibName, EntryPoint="LLVMIsACmpInst", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* IsACmpInst(LLVMValueRef* Val);

        [DllImport(llvmLibName, EntryPoint="LLVMIsAFCmpInst", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* IsAFCmpInst(LLVMValueRef* Val);

        [DllImport(llvmLibName, EntryPoint="LLVMIsAICmpInst", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* IsAICmpInst(LLVMValueRef* Val);

        [DllImport(llvmLibName, EntryPoint="LLVMIsAExtractElementInst", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* IsAExtractElementInst(LLVMValueRef* Val);

        [DllImport(llvmLibName, EntryPoint="LLVMIsAGetElementPtrInst", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* IsAGetElementPtrInst(LLVMValueRef* Val);

        [DllImport(llvmLibName, EntryPoint="LLVMIsAInsertElementInst", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* IsAInsertElementInst(LLVMValueRef* Val);

        [DllImport(llvmLibName, EntryPoint="LLVMIsAInsertValueInst", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* IsAInsertValueInst(LLVMValueRef* Val);

        [DllImport(llvmLibName, EntryPoint="LLVMIsALandingPadInst", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* IsALandingPadInst(LLVMValueRef* Val);

        [DllImport(llvmLibName, EntryPoint="LLVMIsAPHINode", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* IsAPHINode(LLVMValueRef* Val);

        [DllImport(llvmLibName, EntryPoint="LLVMIsASelectInst", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* IsASelectInst(LLVMValueRef* Val);

        [DllImport(llvmLibName, EntryPoint="LLVMIsAShuffleVectorInst", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* IsAShuffleVectorInst(LLVMValueRef* Val);

        [DllImport(llvmLibName, EntryPoint="LLVMIsAStoreInst", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* IsAStoreInst(LLVMValueRef* Val);

        [DllImport(llvmLibName, EntryPoint="LLVMIsATerminatorInst", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* IsATerminatorInst(LLVMValueRef* Val);

        [DllImport(llvmLibName, EntryPoint="LLVMIsABranchInst", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* IsABranchInst(LLVMValueRef* Val);

        [DllImport(llvmLibName, EntryPoint="LLVMIsAIndirectBrInst", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* IsAIndirectBrInst(LLVMValueRef* Val);

        [DllImport(llvmLibName, EntryPoint="LLVMIsAInvokeInst", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* IsAInvokeInst(LLVMValueRef* Val);

        [DllImport(llvmLibName, EntryPoint="LLVMIsAReturnInst", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* IsAReturnInst(LLVMValueRef* Val);

        [DllImport(llvmLibName, EntryPoint="LLVMIsASwitchInst", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* IsASwitchInst(LLVMValueRef* Val);

        [DllImport(llvmLibName, EntryPoint="LLVMIsAUnreachableInst", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* IsAUnreachableInst(LLVMValueRef* Val);

        [DllImport(llvmLibName, EntryPoint="LLVMIsAResumeInst", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* IsAResumeInst(LLVMValueRef* Val);

        [DllImport(llvmLibName, EntryPoint="LLVMIsAUnaryInstruction", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* IsAUnaryInstruction(LLVMValueRef* Val);

        [DllImport(llvmLibName, EntryPoint="LLVMIsAAllocaInst", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* IsAAllocaInst(LLVMValueRef* Val);

        [DllImport(llvmLibName, EntryPoint="LLVMIsACastInst", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* IsACastInst(LLVMValueRef* Val);

        [DllImport(llvmLibName, EntryPoint="LLVMIsABitCastInst", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* IsABitCastInst(LLVMValueRef* Val);

        [DllImport(llvmLibName, EntryPoint="LLVMIsAFPExtInst", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* IsAFPExtInst(LLVMValueRef* Val);

        [DllImport(llvmLibName, EntryPoint="LLVMIsAFPToSIInst", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* IsAFPToSIInst(LLVMValueRef* Val);

        [DllImport(llvmLibName, EntryPoint="LLVMIsAFPToUIInst", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* IsAFPToUIInst(LLVMValueRef* Val);

        [DllImport(llvmLibName, EntryPoint="LLVMIsAFPTruncInst", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* IsAFPTruncInst(LLVMValueRef* Val);

        [DllImport(llvmLibName, EntryPoint="LLVMIsAIntToPtrInst", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* IsAIntToPtrInst(LLVMValueRef* Val);

        [DllImport(llvmLibName, EntryPoint="LLVMIsAPtrToIntInst", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* IsAPtrToIntInst(LLVMValueRef* Val);

        [DllImport(llvmLibName, EntryPoint="LLVMIsASExtInst", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* IsASExtInst(LLVMValueRef* Val);

        [DllImport(llvmLibName, EntryPoint="LLVMIsASIToFPInst", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* IsASIToFPInst(LLVMValueRef* Val);

        [DllImport(llvmLibName, EntryPoint="LLVMIsATruncInst", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* IsATruncInst(LLVMValueRef* Val);

        [DllImport(llvmLibName, EntryPoint="LLVMIsAUIToFPInst", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* IsAUIToFPInst(LLVMValueRef* Val);

        [DllImport(llvmLibName, EntryPoint="LLVMIsAZExtInst", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* IsAZExtInst(LLVMValueRef* Val);

        [DllImport(llvmLibName, EntryPoint="LLVMIsAExtractValueInst", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* IsAExtractValueInst(LLVMValueRef* Val);

        [DllImport(llvmLibName, EntryPoint="LLVMIsALoadInst", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* IsALoadInst(LLVMValueRef* Val);

        [DllImport(llvmLibName, EntryPoint="LLVMIsAVAArgInst", CallingConvention=CallingConvention.Cdecl)]
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
        [DllImport(llvmLibName, EntryPoint="LLVMGetFirstUse", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMUseRef* GetFirstUse(LLVMValueRef* Val);

        /* Obtain the next use of a value.
         *
         * This effectively advances the iterator. It returns NULL if you are on
         * the final use and no more are available.*/
        [DllImport(llvmLibName, EntryPoint="LLVMGetNextUse", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMUseRef* GetNextUse(LLVMUseRef* U);

        /* Obtain the user value for a user.
         *
         * The returned value corresponds to a llvm::User type.
         *
         * @see llvm::Use::getUser()*/
        [DllImport(llvmLibName, EntryPoint="LLVMGetUser", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* GetUser(LLVMUseRef* U);

        /* Obtain the value this use corresponds to.
         *
         * @see llvm::Use::get().*/
        [DllImport(llvmLibName, EntryPoint="LLVMGetUsedValue", CallingConvention=CallingConvention.Cdecl)]
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
        [DllImport(llvmLibName, EntryPoint="LLVMGetOperand", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* GetOperand(LLVMValueRef* Val, uint Index);

        /* Set an operand at a specific index in a llvm::User value.
         *
         * @see llvm::User::setOperand()*/
        [DllImport(llvmLibName, EntryPoint="LLVMSetOperand", CallingConvention=CallingConvention.Cdecl)]
        public static extern void SetOperand(LLVMValueRef* User, uint Index, LLVMValueRef* Val);

        /* Obtain the number of operands in a llvm::User value.
         *
         * @see llvm::User::getNumOperands()*/
        [DllImport(llvmLibName, EntryPoint="LLVMGetNumOperands", CallingConvention=CallingConvention.Cdecl)]
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
        [DllImport(llvmLibName, EntryPoint="LLVMConstNull", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* ConstNull(LLVMTypeRef* Ty);

        /* Obtain a constant value referring to the instance of a type
         * consisting of all ones.
         *
         * This is only valid for integer types.
         *
         * @see llvm::Constant::getAllOnesValue()*/
        [DllImport(llvmLibName, EntryPoint="LLVMConstAllOnes", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* ConstAllOnes(LLVMTypeRef* Ty);

        /* Obtain a constant value referring to an undefined value of a type.
         *
         * @see llvm::UndefValue::get()*/
        [DllImport(llvmLibName, EntryPoint="LLVMGetUndef", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* GetUndef(LLVMTypeRef* Ty);

        /* Determine whether a value instance is null.
         *
         * @see llvm::Constant::isNullValue()*/
        [DllImport(llvmLibName, EntryPoint="LLVMIsNull", CallingConvention=CallingConvention.Cdecl)]
        public static extern int IsNull(LLVMValueRef* Val);

        /* Obtain a constant that is a constant pointer pointing to NULL for a
         * specified type.*/
        [DllImport(llvmLibName, EntryPoint="LLVMConstPointerNull", CallingConvention=CallingConvention.Cdecl)]
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
        [DllImport(llvmLibName, EntryPoint="LLVMConstInt", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* ConstInt(LLVMTypeRef* IntTy, ulong N, int SignExtend);

        /* Obtain a constant value for an integer of arbitrary precision.
         *
         * @see llvm::ConstantInt::get()*/
        [DllImport(llvmLibName, EntryPoint="LLVMConstIntOfArbitraryPrecision", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* ConstIntOfArbitraryPrecision(LLVMTypeRef* IntTy, uint NumWords, System.IntPtr Words);

        /* Obtain a constant value for an integer parsed from a string.
         *
         * A similar API, LLVMConstIntOfStringAndSize is also available. If the
         * string's length is available, it is preferred to call that function
         * instead.
         *
         * @see llvm::ConstantInt::get()*/
        [DllImport(llvmLibName, EntryPoint="LLVMConstIntOfString", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* ConstIntOfString(LLVMTypeRef* IntTy, [In][MarshalAs(UnmanagedType.LPStr)] string Text, byte Radix);

        /* Obtain a constant value for an integer parsed from a string with
         * specified length.
         *
         * @see llvm::ConstantInt::get()*/
        [DllImport(llvmLibName, EntryPoint="LLVMConstIntOfStringAndSize", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* ConstIntOfStringAndSize(LLVMTypeRef* IntTy, [In][MarshalAs(UnmanagedType.LPStr)] string Text, uint SLen, byte Radix);

        /* Obtain a constant value referring to a double floating point value.*/
        [DllImport(llvmLibName, EntryPoint="LLVMConstReal", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* ConstReal(LLVMTypeRef* RealTy, double N);

        /* Obtain a constant for a floating point value parsed from a string.
         *
         * A similar API, LLVMConstRealOfStringAndSize is also available. It
         * should be used if the input string's length is known.*/
        [DllImport(llvmLibName, EntryPoint="LLVMConstRealOfString", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* ConstRealOfString(LLVMTypeRef* RealTy, [In][MarshalAs(UnmanagedType.LPStr)] string Text);

        /* Obtain a constant for a floating point value parsed from a string.*/
        [DllImport(llvmLibName, EntryPoint="LLVMConstRealOfStringAndSize", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* ConstRealOfStringAndSize(LLVMTypeRef* RealTy, [In][MarshalAs(UnmanagedType.LPStr)] string Text, uint SLen);

        /* Obtain the zero extended value for an integer constant value.
         *
         * @see llvm::ConstantInt::getZExtValue()*/
        [DllImport(llvmLibName, EntryPoint="LLVMConstIntGetZExtValue", CallingConvention=CallingConvention.Cdecl)]
        public static extern ulong ConstIntGetZExtValue(LLVMValueRef* ConstantVal);

        /* Obtain the sign extended value for an integer constant value.
         *
         * @see llvm::ConstantInt::getSExtValue()*/
        [DllImport(llvmLibName, EntryPoint="LLVMConstIntGetSExtValue", CallingConvention=CallingConvention.Cdecl)]
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
        [DllImport(llvmLibName, EntryPoint="LLVMConstStringInContext", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* ConstStringInContext(LLVMContextRef* C, [In][MarshalAs(UnmanagedType.LPStr)] string Str, uint Length, int DontNullTerminate);

        /* Create a ConstantDataSequential with string content in the global context.
         *
         * This is the same as LLVMConstStringInContext except it operates on the
         * global context.
         *
         * @see LLVMConstStringInContext()
         * @see llvm::ConstantDataArray::getString()*/
        [DllImport(llvmLibName, EntryPoint="LLVMConstString", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* ConstString([In][MarshalAs(UnmanagedType.LPStr)] string Str, uint Length, int DontNullTerminate);

        /* Create an anonymous ConstantStruct with the specified values.
         *
         * @see llvm::ConstantStruct::getAnon()*/
        [DllImport(llvmLibName, EntryPoint="LLVMConstStructInContext", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* ConstStructInContext(LLVMContextRef* C, System.IntPtr[] ConstantVals, uint Count, int Packed);

        /* Create a ConstantStruct in the global Context.
         *
         * This is the same as LLVMConstStructInContext except it operates on the
         * global Context.
         *
         * @see LLVMConstStructInContext()*/
        [DllImport(llvmLibName, EntryPoint="LLVMConstStruct", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* ConstStruct(System.IntPtr[] ConstantVals, uint Count, int Packed);

        /* Create a ConstantArray from values.
         *
         * @see llvm::ConstantArray::get()*/
        [DllImport(llvmLibName, EntryPoint="LLVMConstArray", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* ConstArray(LLVMTypeRef* ElementTy, System.IntPtr[] ConstantVals, uint Length);

        /* Create a non-anonymous ConstantStruct from values.
         *
         * @see llvm::ConstantStruct::get()*/
        [DllImport(llvmLibName, EntryPoint="LLVMConstNamedStruct", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* ConstNamedStruct(LLVMTypeRef* StructTy, System.IntPtr[] ConstantVals, uint Count);

        /* Create a ConstantVector from values.
         *
         * @see llvm::ConstantVector::get()*/
        [DllImport(llvmLibName, EntryPoint="LLVMConstVector", CallingConvention=CallingConvention.Cdecl)]
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
        [DllImport(llvmLibName, EntryPoint="LLVMGetConstOpcode", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMOpcode GetConstOpcode(LLVMValueRef* ConstantVal);

        [DllImport(llvmLibName, EntryPoint="LLVMAlignOf", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* AlignOf(LLVMTypeRef* Ty);

        [DllImport(llvmLibName, EntryPoint="LLVMSizeOf", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* SizeOf(LLVMTypeRef* Ty);

        [DllImport(llvmLibName, EntryPoint="LLVMConstNeg", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* ConstNeg(LLVMValueRef* ConstantVal);

        [DllImport(llvmLibName, EntryPoint="LLVMConstNSWNeg", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* ConstNSWNeg(LLVMValueRef* ConstantVal);

        [DllImport(llvmLibName, EntryPoint="LLVMConstNUWNeg", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* ConstNUWNeg(LLVMValueRef* ConstantVal);

        [DllImport(llvmLibName, EntryPoint="LLVMConstFNeg", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* ConstFNeg(LLVMValueRef* ConstantVal);

        [DllImport(llvmLibName, EntryPoint="LLVMConstNot", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* ConstNot(LLVMValueRef* ConstantVal);

        [DllImport(llvmLibName, EntryPoint="LLVMConstAdd", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* ConstAdd(LLVMValueRef* LHSConstant, LLVMValueRef* RHSConstant);

        [DllImport(llvmLibName, EntryPoint="LLVMConstNSWAdd", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* ConstNSWAdd(LLVMValueRef* LHSConstant, LLVMValueRef* RHSConstant);

        [DllImport(llvmLibName, EntryPoint="LLVMConstNUWAdd", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* ConstNUWAdd(LLVMValueRef* LHSConstant, LLVMValueRef* RHSConstant);

        [DllImport(llvmLibName, EntryPoint="LLVMConstFAdd", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* ConstFAdd(LLVMValueRef* LHSConstant, LLVMValueRef* RHSConstant);

        [DllImport(llvmLibName, EntryPoint="LLVMConstSub", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* ConstSub(LLVMValueRef* LHSConstant, LLVMValueRef* RHSConstant);

        [DllImport(llvmLibName, EntryPoint="LLVMConstNSWSub", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* ConstNSWSub(LLVMValueRef* LHSConstant, LLVMValueRef* RHSConstant);

        [DllImport(llvmLibName, EntryPoint="LLVMConstNUWSub", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* ConstNUWSub(LLVMValueRef* LHSConstant, LLVMValueRef* RHSConstant);

        [DllImport(llvmLibName, EntryPoint="LLVMConstFSub", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* ConstFSub(LLVMValueRef* LHSConstant, LLVMValueRef* RHSConstant);

        [DllImport(llvmLibName, EntryPoint="LLVMConstMul", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* ConstMul(LLVMValueRef* LHSConstant, LLVMValueRef* RHSConstant);

        [DllImport(llvmLibName, EntryPoint="LLVMConstNSWMul", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* ConstNSWMul(LLVMValueRef* LHSConstant, LLVMValueRef* RHSConstant);

        [DllImport(llvmLibName, EntryPoint="LLVMConstNUWMul", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* ConstNUWMul(LLVMValueRef* LHSConstant, LLVMValueRef* RHSConstant);

        [DllImport(llvmLibName, EntryPoint="LLVMConstFMul", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* ConstFMul(LLVMValueRef* LHSConstant, LLVMValueRef* RHSConstant);

        [DllImport(llvmLibName, EntryPoint="LLVMConstUDiv", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* ConstUDiv(LLVMValueRef* LHSConstant, LLVMValueRef* RHSConstant);

        [DllImport(llvmLibName, EntryPoint="LLVMConstSDiv", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* ConstSDiv(LLVMValueRef* LHSConstant, LLVMValueRef* RHSConstant);

        [DllImport(llvmLibName, EntryPoint="LLVMConstExactSDiv", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* ConstExactSDiv(LLVMValueRef* LHSConstant, LLVMValueRef* RHSConstant);

        [DllImport(llvmLibName, EntryPoint="LLVMConstFDiv", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* ConstFDiv(LLVMValueRef* LHSConstant, LLVMValueRef* RHSConstant);

        [DllImport(llvmLibName, EntryPoint="LLVMConstURem", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* ConstURem(LLVMValueRef* LHSConstant, LLVMValueRef* RHSConstant);

        [DllImport(llvmLibName, EntryPoint="LLVMConstSRem", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* ConstSRem(LLVMValueRef* LHSConstant, LLVMValueRef* RHSConstant);

        [DllImport(llvmLibName, EntryPoint="LLVMConstFRem", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* ConstFRem(LLVMValueRef* LHSConstant, LLVMValueRef* RHSConstant);

        [DllImport(llvmLibName, EntryPoint="LLVMConstAnd", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* ConstAnd(LLVMValueRef* LHSConstant, LLVMValueRef* RHSConstant);

        [DllImport(llvmLibName, EntryPoint="LLVMConstOr", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* ConstOr(LLVMValueRef* LHSConstant, LLVMValueRef* RHSConstant);

        [DllImport(llvmLibName, EntryPoint="LLVMConstXor", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* ConstXor(LLVMValueRef* LHSConstant, LLVMValueRef* RHSConstant);

        [DllImport(llvmLibName, EntryPoint="LLVMConstICmp", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* ConstICmp(LLVMIntPredicate Predicate, LLVMValueRef* LHSConstant, LLVMValueRef* RHSConstant);

        [DllImport(llvmLibName, EntryPoint="LLVMConstFCmp", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* ConstFCmp(LLVMRealPredicate Predicate, LLVMValueRef* LHSConstant, LLVMValueRef* RHSConstant);

        [DllImport(llvmLibName, EntryPoint="LLVMConstShl", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* ConstShl(LLVMValueRef* LHSConstant, LLVMValueRef* RHSConstant);

        [DllImport(llvmLibName, EntryPoint="LLVMConstLShr", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* ConstLShr(LLVMValueRef* LHSConstant, LLVMValueRef* RHSConstant);

        [DllImport(llvmLibName, EntryPoint="LLVMConstAShr", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* ConstAShr(LLVMValueRef* LHSConstant, LLVMValueRef* RHSConstant);

        [DllImport(llvmLibName, EntryPoint="LLVMConstGEP", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* ConstGEP(LLVMValueRef* ConstantVal, System.IntPtr[] ConstantIndices, uint NumIndices);

        [DllImport(llvmLibName, EntryPoint="LLVMConstInBoundsGEP", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* ConstInBoundsGEP(LLVMValueRef* ConstantVal, System.IntPtr[] ConstantIndices, uint NumIndices);

        [DllImport(llvmLibName, EntryPoint="LLVMConstTrunc", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* ConstTrunc(LLVMValueRef* ConstantVal, LLVMTypeRef* ToType);

        [DllImport(llvmLibName, EntryPoint="LLVMConstSExt", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* ConstSExt(LLVMValueRef* ConstantVal, LLVMTypeRef* ToType);

        [DllImport(llvmLibName, EntryPoint="LLVMConstZExt", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* ConstZExt(LLVMValueRef* ConstantVal, LLVMTypeRef* ToType);

        [DllImport(llvmLibName, EntryPoint="LLVMConstFPTrunc", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* ConstFPTrunc(LLVMValueRef* ConstantVal, LLVMTypeRef* ToType);

        [DllImport(llvmLibName, EntryPoint="LLVMConstFPExt", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* ConstFPExt(LLVMValueRef* ConstantVal, LLVMTypeRef* ToType);

        [DllImport(llvmLibName, EntryPoint="LLVMConstUIToFP", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* ConstUIToFP(LLVMValueRef* ConstantVal, LLVMTypeRef* ToType);

        [DllImport(llvmLibName, EntryPoint="LLVMConstSIToFP", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* ConstSIToFP(LLVMValueRef* ConstantVal, LLVMTypeRef* ToType);

        [DllImport(llvmLibName, EntryPoint="LLVMConstFPToUI", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* ConstFPToUI(LLVMValueRef* ConstantVal, LLVMTypeRef* ToType);

        [DllImport(llvmLibName, EntryPoint="LLVMConstFPToSI", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* ConstFPToSI(LLVMValueRef* ConstantVal, LLVMTypeRef* ToType);

        [DllImport(llvmLibName, EntryPoint="LLVMConstPtrToInt", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* ConstPtrToInt(LLVMValueRef* ConstantVal, LLVMTypeRef* ToType);

        [DllImport(llvmLibName, EntryPoint="LLVMConstIntToPtr", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* ConstIntToPtr(LLVMValueRef* ConstantVal, LLVMTypeRef* ToType);

        [DllImport(llvmLibName, EntryPoint="LLVMConstBitCast", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* ConstBitCast(LLVMValueRef* ConstantVal, LLVMTypeRef* ToType);

        [DllImport(llvmLibName, EntryPoint="LLVMConstZExtOrBitCast", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* ConstZExtOrBitCast(LLVMValueRef* ConstantVal, LLVMTypeRef* ToType);

        [DllImport(llvmLibName, EntryPoint="LLVMConstSExtOrBitCast", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* ConstSExtOrBitCast(LLVMValueRef* ConstantVal, LLVMTypeRef* ToType);

        [DllImport(llvmLibName, EntryPoint="LLVMConstTruncOrBitCast", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* ConstTruncOrBitCast(LLVMValueRef* ConstantVal, LLVMTypeRef* ToType);

        [DllImport(llvmLibName, EntryPoint="LLVMConstPointerCast", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* ConstPointerCast(LLVMValueRef* ConstantVal, LLVMTypeRef* ToType);

        [DllImport(llvmLibName, EntryPoint="LLVMConstIntCast", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* ConstIntCast(LLVMValueRef* ConstantVal, LLVMTypeRef* ToType, int isSigned);

        [DllImport(llvmLibName, EntryPoint="LLVMConstFPCast", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* ConstFPCast(LLVMValueRef* ConstantVal, LLVMTypeRef* ToType);

        [DllImport(llvmLibName, EntryPoint="LLVMConstSelect", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* ConstSelect(LLVMValueRef* ConstantCondition, LLVMValueRef* ConstantIfTrue, LLVMValueRef* ConstantIfFalse);

        [DllImport(llvmLibName, EntryPoint="LLVMConstExtractElement", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* ConstExtractElement(LLVMValueRef* VectorConstant, LLVMValueRef* IndexConstant);

        [DllImport(llvmLibName, EntryPoint="LLVMConstInsertElement", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* ConstInsertElement(LLVMValueRef* VectorConstant, LLVMValueRef* ElementValueConstant, LLVMValueRef* IndexConstant);

        [DllImport(llvmLibName, EntryPoint="LLVMConstShuffleVector", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* ConstShuffleVector(LLVMValueRef* VectorAConstant, LLVMValueRef* VectorBConstant, LLVMValueRef* MaskConstant);

        [DllImport(llvmLibName, EntryPoint="LLVMConstExtractValue", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* ConstExtractValue(LLVMValueRef* AggConstant, System.IntPtr IdxList, uint NumIdx);

        [DllImport(llvmLibName, EntryPoint="LLVMConstInsertValue", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* ConstInsertValue(LLVMValueRef* AggConstant, LLVMValueRef* ElementValueConstant, System.IntPtr IdxList, uint NumIdx);

        [DllImport(llvmLibName, EntryPoint="LLVMConstInlineAsm", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* ConstInlineAsm(LLVMTypeRef* Ty, [In][MarshalAs(UnmanagedType.LPStr)] string AsmString, [In][MarshalAs(UnmanagedType.LPStr)] string Constraints, int HasSideEffects, int IsAlignStack);

        [DllImport(llvmLibName, EntryPoint="LLVMBlockAddress", CallingConvention=CallingConvention.Cdecl)]
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
        [DllImport(llvmLibName, EntryPoint="LLVMGetGlobalParent", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMModuleRef* GetGlobalParent(LLVMValueRef* Global);

        [DllImport(llvmLibName, EntryPoint="LLVMIsDeclaration", CallingConvention=CallingConvention.Cdecl)]
        public static extern int IsDeclaration(LLVMValueRef* Global);

        [DllImport(llvmLibName, EntryPoint="LLVMGetLinkage", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMLinkage GetLinkage(LLVMValueRef* Global);

        [DllImport(llvmLibName, EntryPoint="LLVMSetLinkage", CallingConvention=CallingConvention.Cdecl)]
        public static extern void SetLinkage(LLVMValueRef* Global, LLVMLinkage Linkage);

        [DllImport(llvmLibName, EntryPoint="LLVMGetSection", CallingConvention=CallingConvention.Cdecl)]
        public static extern System.IntPtr GetSection(LLVMValueRef* Global);

        [DllImport(llvmLibName, EntryPoint="LLVMSetSection", CallingConvention=CallingConvention.Cdecl)]
        public static extern void SetSection(LLVMValueRef* Global, [In][MarshalAs(UnmanagedType.LPStr)] string Section);

        [DllImport(llvmLibName, EntryPoint="LLVMGetVisibility", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMVisibility GetVisibility(LLVMValueRef* Global);

        [DllImport(llvmLibName, EntryPoint="LLVMSetVisibility", CallingConvention=CallingConvention.Cdecl)]
        public static extern void SetVisibility(LLVMValueRef* Global, LLVMVisibility Viz);

        [DllImport(llvmLibName, EntryPoint="LLVMGetAlignment", CallingConvention=CallingConvention.Cdecl)]
        public static extern uint GetAlignment(LLVMValueRef* Global);

        [DllImport(llvmLibName, EntryPoint="LLVMSetAlignment", CallingConvention=CallingConvention.Cdecl)]
        public static extern void SetAlignment(LLVMValueRef* Global, uint Bytes);

        /* @defgroup LLVMCoreValueConstantGlobalVariable Global Variables
         *
         * This group contains functions that operate on global variable values.
         *
         * @see llvm::GlobalVariable
         *
         * @{*/
        [DllImport(llvmLibName, EntryPoint="LLVMAddGlobal", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* AddGlobal(LLVMModuleRef* M, LLVMTypeRef* Ty, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        [DllImport(llvmLibName, EntryPoint="LLVMAddGlobalInAddressSpace", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* AddGlobalInAddressSpace(LLVMModuleRef* M, LLVMTypeRef* Ty, [In][MarshalAs(UnmanagedType.LPStr)] string Name, uint AddressSpace);

        [DllImport(llvmLibName, EntryPoint="LLVMGetNamedGlobal", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* GetNamedGlobal(LLVMModuleRef* M, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        [DllImport(llvmLibName, EntryPoint="LLVMGetFirstGlobal", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* GetFirstGlobal(LLVMModuleRef* M);

        [DllImport(llvmLibName, EntryPoint="LLVMGetLastGlobal", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* GetLastGlobal(LLVMModuleRef* M);

        [DllImport(llvmLibName, EntryPoint="LLVMGetNextGlobal", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* GetNextGlobal(LLVMValueRef* GlobalVar);

        [DllImport(llvmLibName, EntryPoint="LLVMGetPreviousGlobal", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* GetPreviousGlobal(LLVMValueRef* GlobalVar);

        [DllImport(llvmLibName, EntryPoint="LLVMDeleteGlobal", CallingConvention=CallingConvention.Cdecl)]
        public static extern void DeleteGlobal(LLVMValueRef* GlobalVar);

        [DllImport(llvmLibName, EntryPoint="LLVMGetInitializer", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* GetInitializer(LLVMValueRef* GlobalVar);

        [DllImport(llvmLibName, EntryPoint="LLVMSetInitializer", CallingConvention=CallingConvention.Cdecl)]
        public static extern void SetInitializer(LLVMValueRef* GlobalVar, LLVMValueRef* ConstantVal);

        [DllImport(llvmLibName, EntryPoint="LLVMIsThreadLocal", CallingConvention=CallingConvention.Cdecl)]
        public static extern int IsThreadLocal(LLVMValueRef* GlobalVar);

        [DllImport(llvmLibName, EntryPoint="LLVMSetThreadLocal", CallingConvention=CallingConvention.Cdecl)]
        public static extern void SetThreadLocal(LLVMValueRef* GlobalVar, int IsThreadLocal);

        [DllImport(llvmLibName, EntryPoint="LLVMIsGlobalConstant", CallingConvention=CallingConvention.Cdecl)]
        public static extern int IsGlobalConstant(LLVMValueRef* GlobalVar);

        [DllImport(llvmLibName, EntryPoint="LLVMSetGlobalConstant", CallingConvention=CallingConvention.Cdecl)]
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
        [DllImport(llvmLibName, EntryPoint="LLVMAddAlias", CallingConvention=CallingConvention.Cdecl)]
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
        [DllImport(llvmLibName, EntryPoint="LLVMDeleteFunction", CallingConvention=CallingConvention.Cdecl)]
        public static extern void DeleteFunction(LLVMValueRef* Fn);

        /* Obtain the ID number from a function instance.
         *
         * @see llvm::Function::getIntrinsicID()*/
        [DllImport(llvmLibName, EntryPoint="LLVMGetIntrinsicID", CallingConvention=CallingConvention.Cdecl)]
        public static extern uint GetIntrinsicID(LLVMValueRef* Fn);

        /* Obtain the calling function of a function.
         *
         * The returned value corresponds to the LLVMCallConv enumeration.
         *
         * @see llvm::Function::getCallingConv()*/
        [DllImport(llvmLibName, EntryPoint="LLVMGetFunctionCallConv", CallingConvention=CallingConvention.Cdecl)]
        public static extern uint GetFunctionCallConv(LLVMValueRef* Fn);

        /* Set the calling convention of a function.
         *
         * @see llvm::Function::setCallingConv()
         *
         * @param Fn Function to operate on
         * @param CC LLVMCallConv to set calling convention to*/
        [DllImport(llvmLibName, EntryPoint="LLVMSetFunctionCallConv", CallingConvention=CallingConvention.Cdecl)]
        public static extern void SetFunctionCallConv(LLVMValueRef* Fn, uint CC);

        /* Obtain the name of the garbage collector to use during code
         * generation.
         *
         * @see llvm::Function::getGC()*/
        [DllImport(llvmLibName, EntryPoint="LLVMGetGC", CallingConvention=CallingConvention.Cdecl)]
        public static extern System.IntPtr GetGC(LLVMValueRef* Fn);

        /* Define the garbage collector to use during code generation.
         *
         * @see llvm::Function::setGC()*/
        [DllImport(llvmLibName, EntryPoint="LLVMSetGC", CallingConvention=CallingConvention.Cdecl)]
        public static extern void SetGC(LLVMValueRef* Fn, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        /* Add an attribute to a function.
         *
         * @see llvm::Function::addAttribute()*/
        [DllImport(llvmLibName, EntryPoint="LLVMAddFunctionAttr", CallingConvention=CallingConvention.Cdecl)]
        public static extern void AddFunctionAttr(LLVMValueRef* Fn, LLVMAttribute PA);

        /* Obtain an attribute from a function.
         *
         * @see llvm::Function::getAttributes()*/
        [DllImport(llvmLibName, EntryPoint="LLVMGetFunctionAttr", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMAttribute GetFunctionAttr(LLVMValueRef* Fn);

        /* Remove an attribute from a function.*/
        [DllImport(llvmLibName, EntryPoint="LLVMRemoveFunctionAttr", CallingConvention=CallingConvention.Cdecl)]
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
        [DllImport(llvmLibName, EntryPoint="LLVMCountParams", CallingConvention=CallingConvention.Cdecl)]
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
        [DllImport(llvmLibName, EntryPoint="LLVMGetParams", CallingConvention=CallingConvention.Cdecl)]
        public static extern void GetParams(LLVMValueRef* Fn, System.IntPtr[] Params);

        /* Obtain the parameter at the specified index.
         *
         * Parameters are indexed from 0.
         *
         * @see llvm::Function::arg_begin()*/
        [DllImport(llvmLibName, EntryPoint="LLVMGetParam", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* GetParam(LLVMValueRef* Fn, uint Index);

        /* Obtain the function to which this argument belongs.
         *
         * Unlike other functions in this group, this one takes a LLVMValueRef
         * that corresponds to a llvm::Attribute.
         *
         * The returned LLVMValueRef is the llvm::Function to which this
         * argument belongs.*/
        [DllImport(llvmLibName, EntryPoint="LLVMGetParamParent", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* GetParamParent(LLVMValueRef* Inst);

        /* Obtain the first parameter to a function.
         *
         * @see llvm::Function::arg_begin()*/
        [DllImport(llvmLibName, EntryPoint="LLVMGetFirstParam", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* GetFirstParam(LLVMValueRef* Fn);

        /* Obtain the last parameter to a function.
         *
         * @see llvm::Function::arg_end()*/
        [DllImport(llvmLibName, EntryPoint="LLVMGetLastParam", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* GetLastParam(LLVMValueRef* Fn);

        /* Obtain the next parameter to a function.
         *
         * This takes a LLVMValueRef obtained from LLVMGetFirstParam() (which is
         * actually a wrapped iterator) and obtains the next parameter from the
         * underlying iterator.*/
        [DllImport(llvmLibName, EntryPoint="LLVMGetNextParam", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* GetNextParam(LLVMValueRef* Arg);

        /* Obtain the previous parameter to a function.
         *
         * This is the opposite of LLVMGetNextParam().*/
        [DllImport(llvmLibName, EntryPoint="LLVMGetPreviousParam", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* GetPreviousParam(LLVMValueRef* Arg);

        /* Add an attribute to a function argument.
         *
         * @see llvm::Argument::addAttr()*/
        [DllImport(llvmLibName, EntryPoint="LLVMAddAttribute", CallingConvention=CallingConvention.Cdecl)]
        public static extern void AddAttribute(LLVMValueRef* Arg, LLVMAttribute PA);

        /* Remove an attribute from a function argument.
         *
         * @see llvm::Argument::removeAttr()*/
        [DllImport(llvmLibName, EntryPoint="LLVMRemoveAttribute", CallingConvention=CallingConvention.Cdecl)]
        public static extern void RemoveAttribute(LLVMValueRef* Arg, LLVMAttribute PA);

        /* Get an attribute from a function argument.*/
        [DllImport(llvmLibName, EntryPoint="LLVMGetAttribute", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMAttribute GetAttribute(LLVMValueRef* Arg);

        /* Set the alignment for a function parameter.
         *
         * @see llvm::Argument::addAttr()
         * @see llvm::Attribute::constructAlignmentFromInt()*/
        [DllImport(llvmLibName, EntryPoint="LLVMSetParamAlignment", CallingConvention=CallingConvention.Cdecl)]
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
        [DllImport(llvmLibName, EntryPoint="LLVMMDStringInContext", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* MDStringInContext(LLVMContextRef* C, [In][MarshalAs(UnmanagedType.LPStr)] string Str, uint SLen);

        /* Obtain a MDString value from the global context.*/
        [DllImport(llvmLibName, EntryPoint="LLVMMDString", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* MDString([In][MarshalAs(UnmanagedType.LPStr)] string Str, uint SLen);

        /* Obtain a MDNode value from a context.
         *
         * The returned value corresponds to the llvm::MDNode class.*/
        [DllImport(llvmLibName, EntryPoint="LLVMMDNodeInContext", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* MDNodeInContext(LLVMContextRef* C, System.IntPtr[] Vals, uint Count);

        /* Obtain a MDNode value from the global context.*/
        [DllImport(llvmLibName, EntryPoint="LLVMMDNode", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* MDNode(System.IntPtr[] Vals, uint Count);

        /* Obtain the underlying string from a MDString value.
         *
         * @param V Instance to obtain string from.
         * @param Len Memory address which will hold length of returned string.
         * @return String data in MDString.*/
        [DllImport(llvmLibName, EntryPoint="LLVMGetMDString", CallingConvention=CallingConvention.Cdecl)]
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
        [DllImport(llvmLibName, EntryPoint="LLVMBasicBlockAsValue", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BasicBlockAsValue(LLVMBasicBlockRef* BB);

        /* Determine whether a LLVMValueRef is itself a basic block.*/
        [DllImport(llvmLibName, EntryPoint="LLVMValueIsBasicBlock", CallingConvention=CallingConvention.Cdecl)]
        public static extern int ValueIsBasicBlock(LLVMValueRef* Val);

        /* Convert a LLVMValueRef to a LLVMBasicBlockRef instance.*/
        [DllImport(llvmLibName, EntryPoint="LLVMValueAsBasicBlock", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMBasicBlockRef* ValueAsBasicBlock(LLVMValueRef* Val);

        /* Obtain the function to which a basic block belongs.
         *
         * @see llvm::BasicBlock::getParent()*/
        [DllImport(llvmLibName, EntryPoint="LLVMGetBasicBlockParent", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* GetBasicBlockParent(LLVMBasicBlockRef* BB);

        /* Obtain the terminator instruction for a basic block.
         *
         * If the basic block does not have a terminator (it is not well-formed
         * if it doesn't), then NULL is returned.
         *
         * The returned LLVMValueRef corresponds to a llvm::TerminatorInst.
         *
         * @see llvm::BasicBlock::getTerminator()*/
        [DllImport(llvmLibName, EntryPoint="LLVMGetBasicBlockTerminator", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* GetBasicBlockTerminator(LLVMBasicBlockRef* BB);

        /* Obtain the number of basic blocks in a function.
         *
         * @param Fn Function value to operate on.*/
        [DllImport(llvmLibName, EntryPoint="LLVMCountBasicBlocks", CallingConvention=CallingConvention.Cdecl)]
        public static extern uint CountBasicBlocks(LLVMValueRef* Fn);

        /* Obtain all of the basic blocks in a function.
         *
         * This operates on a function value. The BasicBlocks parameter is a
         * pointer to a pre-allocated array of LLVMBasicBlockRef of at least
         * LLVMCountBasicBlocks() in length. This array is populated with
         * LLVMBasicBlockRef instances.*/
        [DllImport(llvmLibName, EntryPoint="LLVMGetBasicBlocks", CallingConvention=CallingConvention.Cdecl)]
        public static extern void GetBasicBlocks(LLVMValueRef* Fn, System.IntPtr[] BasicBlocks);

        /* Obtain the first basic block in a function.
         *
         * The returned basic block can be used as an iterator. You will likely
         * eventually call into LLVMGetNextBasicBlock() with it.
         *
         * @see llvm::Function::begin()*/
        [DllImport(llvmLibName, EntryPoint="LLVMGetFirstBasicBlock", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMBasicBlockRef* GetFirstBasicBlock(LLVMValueRef* Fn);

        /* Obtain the last basic block in a function.
         *
         * @see llvm::Function::end()*/
        [DllImport(llvmLibName, EntryPoint="LLVMGetLastBasicBlock", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMBasicBlockRef* GetLastBasicBlock(LLVMValueRef* Fn);

        /* Advance a basic block iterator.*/
        [DllImport(llvmLibName, EntryPoint="LLVMGetNextBasicBlock", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMBasicBlockRef* GetNextBasicBlock(LLVMBasicBlockRef* BB);

        /* Go backwards in a basic block iterator.*/
        [DllImport(llvmLibName, EntryPoint="LLVMGetPreviousBasicBlock", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMBasicBlockRef* GetPreviousBasicBlock(LLVMBasicBlockRef* BB);

        /* Obtain the basic block that corresponds to the entry point of a
         * function.
         *
         * @see llvm::Function::getEntryBlock()*/
        [DllImport(llvmLibName, EntryPoint="LLVMGetEntryBasicBlock", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMBasicBlockRef* GetEntryBasicBlock(LLVMValueRef* Fn);

        /* Append a basic block to the end of a function.
         *
         * @see llvm::BasicBlock::Create()*/
        [DllImport(llvmLibName, EntryPoint="LLVMAppendBasicBlockInContext", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMBasicBlockRef* AppendBasicBlockInContext(LLVMContextRef* C, LLVMValueRef* Fn, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        /* Append a basic block to the end of a function using the global
         * context.
         *
         * @see llvm::BasicBlock::Create()*/
        [DllImport(llvmLibName, EntryPoint="LLVMAppendBasicBlock", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMBasicBlockRef* AppendBasicBlock(LLVMValueRef* Fn, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        /* Insert a basic block in a function before another basic block.
         *
         * The function to add to is determined by the function of the
         * passed basic block.
         *
         * @see llvm::BasicBlock::Create()*/
        [DllImport(llvmLibName, EntryPoint="LLVMInsertBasicBlockInContext", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMBasicBlockRef* InsertBasicBlockInContext(LLVMContextRef* C, LLVMBasicBlockRef* BB, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        /* Insert a basic block in a function using the global context.
         *
         * @see llvm::BasicBlock::Create()*/
        [DllImport(llvmLibName, EntryPoint="LLVMInsertBasicBlock", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMBasicBlockRef* InsertBasicBlock(LLVMBasicBlockRef* InsertBeforeBB, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        /* Remove a basic block from a function and delete it.
         *
         * This deletes the basic block from its containing function and deletes
         * the basic block itself.
         *
         * @see llvm::BasicBlock::eraseFromParent()*/
        [DllImport(llvmLibName, EntryPoint="LLVMDeleteBasicBlock", CallingConvention=CallingConvention.Cdecl)]
        public static extern void DeleteBasicBlock(LLVMBasicBlockRef* BB);

        /* Remove a basic block from a function.
         *
         * This deletes the basic block from its containing function but keep
         * the basic block alive.
         *
         * @see llvm::BasicBlock::removeFromParent()*/
        [DllImport(llvmLibName, EntryPoint="LLVMRemoveBasicBlockFromParent", CallingConvention=CallingConvention.Cdecl)]
        public static extern void RemoveBasicBlockFromParent(LLVMBasicBlockRef* BB);

        /* Move a basic block to before another one.
         *
         * @see llvm::BasicBlock::moveBefore()*/
        [DllImport(llvmLibName, EntryPoint="LLVMMoveBasicBlockBefore", CallingConvention=CallingConvention.Cdecl)]
        public static extern void MoveBasicBlockBefore(LLVMBasicBlockRef* BB, LLVMBasicBlockRef* MovePos);

        /* Move a basic block to after another one.
         *
         * @see llvm::BasicBlock::moveAfter()*/
        [DllImport(llvmLibName, EntryPoint="LLVMMoveBasicBlockAfter", CallingConvention=CallingConvention.Cdecl)]
        public static extern void MoveBasicBlockAfter(LLVMBasicBlockRef* BB, LLVMBasicBlockRef* MovePos);

        /* Obtain the first instruction in a basic block.
         *
         * The returned LLVMValueRef corresponds to a llvm::Instruction
         * instance.*/
        [DllImport(llvmLibName, EntryPoint="LLVMGetFirstInstruction", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* GetFirstInstruction(LLVMBasicBlockRef* BB);

        /* Obtain the last instruction in a basic block.
         *
         * The returned LLVMValueRef corresponds to a LLVM:Instruction.*/
        [DllImport(llvmLibName, EntryPoint="LLVMGetLastInstruction", CallingConvention=CallingConvention.Cdecl)]
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
        [DllImport(llvmLibName, EntryPoint="LLVMHasMetadata", CallingConvention=CallingConvention.Cdecl)]
        public static extern int HasMetadata(LLVMValueRef* Val);

        /* Return metadata associated with an instruction value.*/
        [DllImport(llvmLibName, EntryPoint="LLVMGetMetadata", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* GetMetadata(LLVMValueRef* Val, uint KindID);

        /* Set metadata associated with an instruction value.*/
        [DllImport(llvmLibName, EntryPoint="LLVMSetMetadata", CallingConvention=CallingConvention.Cdecl)]
        public static extern void SetMetadata(LLVMValueRef* Val, uint KindID, LLVMValueRef* Node);

        /* Obtain the basic block to which an instruction belongs.
         *
         * @see llvm::Instruction::getParent()*/
        [DllImport(llvmLibName, EntryPoint="LLVMGetInstructionParent", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMBasicBlockRef* GetInstructionParent(LLVMValueRef* Inst);

        /* Obtain the instruction that occurs after the one specified.
         *
         * The next instruction will be from the same basic block.
         *
         * If this is the last instruction in a basic block, NULL will be
         * returned.*/
        [DllImport(llvmLibName, EntryPoint="LLVMGetNextInstruction", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* GetNextInstruction(LLVMValueRef* Inst);

        /* Obtain the instruction that occured before this one.
         *
         * If the instruction is the first instruction in a basic block, NULL
         * will be returned.*/
        [DllImport(llvmLibName, EntryPoint="LLVMGetPreviousInstruction", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* GetPreviousInstruction(LLVMValueRef* Inst);

        /* Remove and delete an instruction.
         *
         * The instruction specified is removed from its containing building
         * block and then deleted.
         *
         * @see llvm::Instruction::eraseFromParent()*/
        [DllImport(llvmLibName, EntryPoint="LLVMInstructionEraseFromParent", CallingConvention=CallingConvention.Cdecl)]
        public static extern void InstructionEraseFromParent(LLVMValueRef* Inst);

        /* Obtain the code opcode for an individual instruction.
         *
         * @see llvm::Instruction::getOpCode()*/
        [DllImport(llvmLibName, EntryPoint="LLVMGetInstructionOpcode", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMOpcode GetInstructionOpcode(LLVMValueRef* Inst);

        /* Obtain the predicate of an instruction.
         *
         * This is only valid for instructions that correspond to llvm::ICmpInst
         * or llvm::ConstantExpr whose opcode is llvm::Instruction::ICmp.
         *
         * @see llvm::ICmpInst::getPredicate()*/
        [DllImport(llvmLibName, EntryPoint="LLVMGetICmpPredicate", CallingConvention=CallingConvention.Cdecl)]
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
        [DllImport(llvmLibName, EntryPoint="LLVMSetInstructionCallConv", CallingConvention=CallingConvention.Cdecl)]
        public static extern void SetInstructionCallConv(LLVMValueRef* Instr, uint CC);

        /* Obtain the calling convention for a call instruction.
         *
         * This is the opposite of LLVMSetInstructionCallConv(). Reads its
         * usage.
         *
         * @see LLVMSetInstructionCallConv()*/
        [DllImport(llvmLibName, EntryPoint="LLVMGetInstructionCallConv", CallingConvention=CallingConvention.Cdecl)]
        public static extern uint GetInstructionCallConv(LLVMValueRef* Instr);

        [DllImport(llvmLibName, EntryPoint="LLVMAddInstrAttribute", CallingConvention=CallingConvention.Cdecl)]
        public static extern void AddInstrAttribute(LLVMValueRef* Instr, uint index, LLVMAttribute LLVMAttribute);

        [DllImport(llvmLibName, EntryPoint="LLVMRemoveInstrAttribute", CallingConvention=CallingConvention.Cdecl)]
        public static extern void RemoveInstrAttribute(LLVMValueRef* Instr, uint index, LLVMAttribute LLVMAttribute);

        [DllImport(llvmLibName, EntryPoint="LLVMSetInstrParamAlignment", CallingConvention=CallingConvention.Cdecl)]
        public static extern void SetInstrParamAlignment(LLVMValueRef* Instr, uint index, uint align);

        /* Obtain whether a call instruction is a tail call.
         *
         * This only works on llvm::CallInst instructions.
         *
         * @see llvm::CallInst::isTailCall()*/
        [DllImport(llvmLibName, EntryPoint="LLVMIsTailCall", CallingConvention=CallingConvention.Cdecl)]
        public static extern int IsTailCall(LLVMValueRef* CallInst);

        /* Set whether a call instruction is a tail call.
         *
         * This only works on llvm::CallInst instructions.
         *
         * @see llvm::CallInst::setTailCall()*/
        [DllImport(llvmLibName, EntryPoint="LLVMSetTailCall", CallingConvention=CallingConvention.Cdecl)]
        public static extern void SetTailCall(LLVMValueRef* CallInst, int IsTailCall);

        /* @}
         */
        
        /**
         * Obtain the default destination basic block of a switch instruction.
         *
         * This only works on llvm::SwitchInst instructions.
         *
         * @see llvm::SwitchInst::getDefaultDest()*/
        [DllImport(llvmLibName, EntryPoint="LLVMGetSwitchDefaultDest", CallingConvention=CallingConvention.Cdecl)]
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
        [DllImport(llvmLibName, EntryPoint="LLVMAddIncoming", CallingConvention=CallingConvention.Cdecl)]
        public static extern void AddIncoming(LLVMValueRef* PhiNode, System.IntPtr[] IncomingValues, System.IntPtr[] IncomingBlocks, uint Count);

        /* Obtain the number of incoming basic blocks to a PHI node.*/
        [DllImport(llvmLibName, EntryPoint="LLVMCountIncoming", CallingConvention=CallingConvention.Cdecl)]
        public static extern uint CountIncoming(LLVMValueRef* PhiNode);

        /* Obtain an incoming value to a PHI node as a LLVMValueRef.*/
        [DllImport(llvmLibName, EntryPoint="LLVMGetIncomingValue", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* GetIncomingValue(LLVMValueRef* PhiNode, uint Index);

        /* Obtain an incoming value to a PHI node as a LLVMBasicBlockRef.*/
        [DllImport(llvmLibName, EntryPoint="LLVMGetIncomingBlock", CallingConvention=CallingConvention.Cdecl)]
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
        [DllImport(llvmLibName, EntryPoint="LLVMCreateBuilderInContext", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMBuilderRef* CreateBuilderInContext(LLVMContextRef* C);

        [DllImport(llvmLibName, EntryPoint="LLVMCreateBuilder", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMBuilderRef* CreateBuilder();

        [DllImport(llvmLibName, EntryPoint="LLVMPositionBuilder", CallingConvention=CallingConvention.Cdecl)]
        public static extern void PositionBuilder(LLVMBuilderRef* Builder, LLVMBasicBlockRef* Block, LLVMValueRef* Instr);

        [DllImport(llvmLibName, EntryPoint="LLVMPositionBuilderBefore", CallingConvention=CallingConvention.Cdecl)]
        public static extern void PositionBuilderBefore(LLVMBuilderRef* Builder, LLVMValueRef* Instr);

        [DllImport(llvmLibName, EntryPoint="LLVMPositionBuilderAtEnd", CallingConvention=CallingConvention.Cdecl)]
        public static extern void PositionBuilderAtEnd(LLVMBuilderRef* Builder, LLVMBasicBlockRef* Block);

        [DllImport(llvmLibName, EntryPoint="LLVMGetInsertBlock", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMBasicBlockRef* GetInsertBlock(LLVMBuilderRef* Builder);

        [DllImport(llvmLibName, EntryPoint="LLVMClearInsertionPosition", CallingConvention=CallingConvention.Cdecl)]
        public static extern void ClearInsertionPosition(LLVMBuilderRef* Builder);

        [DllImport(llvmLibName, EntryPoint="LLVMInsertIntoBuilder", CallingConvention=CallingConvention.Cdecl)]
        public static extern void InsertIntoBuilder(LLVMBuilderRef* Builder, LLVMValueRef* Instr);

        [DllImport(llvmLibName, EntryPoint="LLVMInsertIntoBuilderWithName", CallingConvention=CallingConvention.Cdecl)]
        public static extern void InsertIntoBuilderWithName(LLVMBuilderRef* Builder, LLVMValueRef* Instr, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        [DllImport(llvmLibName, EntryPoint="LLVMDisposeBuilder", CallingConvention=CallingConvention.Cdecl)]
        public static extern void DisposeBuilder(LLVMBuilderRef* Builder);

        [DllImport(llvmLibName, EntryPoint="LLVMSetCurrentDebugLocation", CallingConvention=CallingConvention.Cdecl)]
        public static extern void SetCurrentDebugLocation(LLVMBuilderRef* Builder, LLVMValueRef* L);

        [DllImport(llvmLibName, EntryPoint="LLVMGetCurrentDebugLocation", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* GetCurrentDebugLocation(LLVMBuilderRef* Builder);

        [DllImport(llvmLibName, EntryPoint="LLVMSetInstDebugLocation", CallingConvention=CallingConvention.Cdecl)]
        public static extern void SetInstDebugLocation(LLVMBuilderRef* Builder, LLVMValueRef* Inst);

        [DllImport(llvmLibName, EntryPoint="LLVMBuildRetVoid", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildRetVoid(LLVMBuilderRef* LLVMBuilderRef);

        [DllImport(llvmLibName, EntryPoint="LLVMBuildRet", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildRet(LLVMBuilderRef* LLVMBuilderRef, LLVMValueRef* V);

        [DllImport(llvmLibName, EntryPoint="LLVMBuildAggregateRet", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildAggregateRet(LLVMBuilderRef* LLVMBuilderRef, System.IntPtr[] RetVals, uint N);

        [DllImport(llvmLibName, EntryPoint="LLVMBuildBr", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildBr(LLVMBuilderRef* LLVMBuilderRef, LLVMBasicBlockRef* Dest);

        [DllImport(llvmLibName, EntryPoint="LLVMBuildCondBr", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildCondBr(LLVMBuilderRef* LLVMBuilderRef, LLVMValueRef* If, LLVMBasicBlockRef* Then, LLVMBasicBlockRef* Else);

        [DllImport(llvmLibName, EntryPoint="LLVMBuildSwitch", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildSwitch(LLVMBuilderRef* LLVMBuilderRef, LLVMValueRef* V, LLVMBasicBlockRef* Else, uint NumCases);

        [DllImport(llvmLibName, EntryPoint="LLVMBuildIndirectBr", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildIndirectBr(LLVMBuilderRef* B, LLVMValueRef* Addr, uint NumDests);

        [DllImport(llvmLibName, EntryPoint="LLVMBuildInvoke", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildInvoke(LLVMBuilderRef* LLVMBuilderRef, LLVMValueRef* Fn, System.IntPtr[] Args, uint NumArgs, LLVMBasicBlockRef* Then, LLVMBasicBlockRef* Catch, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        [DllImport(llvmLibName, EntryPoint="LLVMBuildLandingPad", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildLandingPad(LLVMBuilderRef* B, LLVMTypeRef* Ty, LLVMValueRef* PersFn, uint NumClauses, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        [DllImport(llvmLibName, EntryPoint="LLVMBuildResume", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildResume(LLVMBuilderRef* B, LLVMValueRef* Exn);

        [DllImport(llvmLibName, EntryPoint="LLVMBuildUnreachable", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildUnreachable(LLVMBuilderRef* LLVMBuilderRef);

        [DllImport(llvmLibName, EntryPoint="LLVMAddCase", CallingConvention=CallingConvention.Cdecl)]
        public static extern void AddCase(LLVMValueRef* Switch, LLVMValueRef* OnVal, LLVMBasicBlockRef* Dest);

        [DllImport(llvmLibName, EntryPoint="LLVMAddDestination", CallingConvention=CallingConvention.Cdecl)]
        public static extern void AddDestination(LLVMValueRef* IndirectBr, LLVMBasicBlockRef* Dest);

        [DllImport(llvmLibName, EntryPoint="LLVMAddClause", CallingConvention=CallingConvention.Cdecl)]
        public static extern void AddClause(LLVMValueRef* LandingPad, LLVMValueRef* ClauseVal);

        [DllImport(llvmLibName, EntryPoint="LLVMSetCleanup", CallingConvention=CallingConvention.Cdecl)]
        public static extern void SetCleanup(LLVMValueRef* LandingPad, int Val);

        [DllImport(llvmLibName, EntryPoint="LLVMBuildAdd", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildAdd(LLVMBuilderRef* LLVMBuilderRef, LLVMValueRef* LHS, LLVMValueRef* RHS, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        [DllImport(llvmLibName, EntryPoint="LLVMBuildNSWAdd", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildNSWAdd(LLVMBuilderRef* LLVMBuilderRef, LLVMValueRef* LHS, LLVMValueRef* RHS, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        [DllImport(llvmLibName, EntryPoint="LLVMBuildNUWAdd", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildNUWAdd(LLVMBuilderRef* LLVMBuilderRef, LLVMValueRef* LHS, LLVMValueRef* RHS, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        [DllImport(llvmLibName, EntryPoint="LLVMBuildFAdd", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildFAdd(LLVMBuilderRef* LLVMBuilderRef, LLVMValueRef* LHS, LLVMValueRef* RHS, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        [DllImport(llvmLibName, EntryPoint="LLVMBuildSub", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildSub(LLVMBuilderRef* LLVMBuilderRef, LLVMValueRef* LHS, LLVMValueRef* RHS, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        [DllImport(llvmLibName, EntryPoint="LLVMBuildNSWSub", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildNSWSub(LLVMBuilderRef* LLVMBuilderRef, LLVMValueRef* LHS, LLVMValueRef* RHS, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        [DllImport(llvmLibName, EntryPoint="LLVMBuildNUWSub", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildNUWSub(LLVMBuilderRef* LLVMBuilderRef, LLVMValueRef* LHS, LLVMValueRef* RHS, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        [DllImport(llvmLibName, EntryPoint="LLVMBuildFSub", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildFSub(LLVMBuilderRef* LLVMBuilderRef, LLVMValueRef* LHS, LLVMValueRef* RHS, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        [DllImport(llvmLibName, EntryPoint="LLVMBuildMul", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildMul(LLVMBuilderRef* LLVMBuilderRef, LLVMValueRef* LHS, LLVMValueRef* RHS, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        [DllImport(llvmLibName, EntryPoint="LLVMBuildNSWMul", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildNSWMul(LLVMBuilderRef* LLVMBuilderRef, LLVMValueRef* LHS, LLVMValueRef* RHS, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        [DllImport(llvmLibName, EntryPoint="LLVMBuildNUWMul", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildNUWMul(LLVMBuilderRef* LLVMBuilderRef, LLVMValueRef* LHS, LLVMValueRef* RHS, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        [DllImport(llvmLibName, EntryPoint="LLVMBuildFMul", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildFMul(LLVMBuilderRef* LLVMBuilderRef, LLVMValueRef* LHS, LLVMValueRef* RHS, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        [DllImport(llvmLibName, EntryPoint="LLVMBuildUDiv", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildUDiv(LLVMBuilderRef* LLVMBuilderRef, LLVMValueRef* LHS, LLVMValueRef* RHS, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        [DllImport(llvmLibName, EntryPoint="LLVMBuildSDiv", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildSDiv(LLVMBuilderRef* LLVMBuilderRef, LLVMValueRef* LHS, LLVMValueRef* RHS, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        [DllImport(llvmLibName, EntryPoint="LLVMBuildExactSDiv", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildExactSDiv(LLVMBuilderRef* LLVMBuilderRef, LLVMValueRef* LHS, LLVMValueRef* RHS, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        [DllImport(llvmLibName, EntryPoint="LLVMBuildFDiv", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildFDiv(LLVMBuilderRef* LLVMBuilderRef, LLVMValueRef* LHS, LLVMValueRef* RHS, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        [DllImport(llvmLibName, EntryPoint="LLVMBuildURem", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildURem(LLVMBuilderRef* LLVMBuilderRef, LLVMValueRef* LHS, LLVMValueRef* RHS, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        [DllImport(llvmLibName, EntryPoint="LLVMBuildSRem", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildSRem(LLVMBuilderRef* LLVMBuilderRef, LLVMValueRef* LHS, LLVMValueRef* RHS, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        [DllImport(llvmLibName, EntryPoint="LLVMBuildFRem", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildFRem(LLVMBuilderRef* LLVMBuilderRef, LLVMValueRef* LHS, LLVMValueRef* RHS, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        [DllImport(llvmLibName, EntryPoint="LLVMBuildShl", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildShl(LLVMBuilderRef* LLVMBuilderRef, LLVMValueRef* LHS, LLVMValueRef* RHS, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        [DllImport(llvmLibName, EntryPoint="LLVMBuildLShr", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildLShr(LLVMBuilderRef* LLVMBuilderRef, LLVMValueRef* LHS, LLVMValueRef* RHS, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        [DllImport(llvmLibName, EntryPoint="LLVMBuildAShr", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildAShr(LLVMBuilderRef* LLVMBuilderRef, LLVMValueRef* LHS, LLVMValueRef* RHS, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        [DllImport(llvmLibName, EntryPoint="LLVMBuildAnd", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildAnd(LLVMBuilderRef* LLVMBuilderRef, LLVMValueRef* LHS, LLVMValueRef* RHS, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        [DllImport(llvmLibName, EntryPoint="LLVMBuildOr", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildOr(LLVMBuilderRef* LLVMBuilderRef, LLVMValueRef* LHS, LLVMValueRef* RHS, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        [DllImport(llvmLibName, EntryPoint="LLVMBuildXor", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildXor(LLVMBuilderRef* LLVMBuilderRef, LLVMValueRef* LHS, LLVMValueRef* RHS, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        [DllImport(llvmLibName, EntryPoint="LLVMBuildBinOp", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildBinOp(LLVMBuilderRef* B, LLVMOpcode Op, LLVMValueRef* LHS, LLVMValueRef* RHS, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        [DllImport(llvmLibName, EntryPoint="LLVMBuildNeg", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildNeg(LLVMBuilderRef* LLVMBuilderRef, LLVMValueRef* V, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        [DllImport(llvmLibName, EntryPoint="LLVMBuildNSWNeg", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildNSWNeg(LLVMBuilderRef* B, LLVMValueRef* V, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        [DllImport(llvmLibName, EntryPoint="LLVMBuildNUWNeg", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildNUWNeg(LLVMBuilderRef* B, LLVMValueRef* V, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        [DllImport(llvmLibName, EntryPoint="LLVMBuildFNeg", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildFNeg(LLVMBuilderRef* LLVMBuilderRef, LLVMValueRef* V, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        [DllImport(llvmLibName, EntryPoint="LLVMBuildNot", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildNot(LLVMBuilderRef* LLVMBuilderRef, LLVMValueRef* V, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        [DllImport(llvmLibName, EntryPoint="LLVMBuildMalloc", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildMalloc(LLVMBuilderRef* LLVMBuilderRef, LLVMTypeRef* Ty, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        [DllImport(llvmLibName, EntryPoint="LLVMBuildArrayMalloc", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildArrayMalloc(LLVMBuilderRef* LLVMBuilderRef, LLVMTypeRef* Ty, LLVMValueRef* Val, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        [DllImport(llvmLibName, EntryPoint="LLVMBuildAlloca", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildAlloca(LLVMBuilderRef* LLVMBuilderRef, LLVMTypeRef* Ty, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        [DllImport(llvmLibName, EntryPoint="LLVMBuildArrayAlloca", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildArrayAlloca(LLVMBuilderRef* LLVMBuilderRef, LLVMTypeRef* Ty, LLVMValueRef* Val, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        [DllImport(llvmLibName, EntryPoint="LLVMBuildFree", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildFree(LLVMBuilderRef* LLVMBuilderRef, LLVMValueRef* PointerVal);

        [DllImport(llvmLibName, EntryPoint="LLVMBuildLoad", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildLoad(LLVMBuilderRef* LLVMBuilderRef, LLVMValueRef* PointerVal, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        [DllImport(llvmLibName, EntryPoint="LLVMBuildStore", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildStore(LLVMBuilderRef* LLVMBuilderRef, LLVMValueRef* Val, LLVMValueRef* Ptr);

        [DllImport(llvmLibName, EntryPoint="LLVMBuildGEP", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildGEP(LLVMBuilderRef* B, LLVMValueRef* Pointer, System.IntPtr[] Indices, uint NumIndices, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        [DllImport(llvmLibName, EntryPoint="LLVMBuildInBoundsGEP", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildInBoundsGEP(LLVMBuilderRef* B, LLVMValueRef* Pointer, System.IntPtr[] Indices, uint NumIndices, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        [DllImport(llvmLibName, EntryPoint="LLVMBuildStructGEP", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildStructGEP(LLVMBuilderRef* B, LLVMValueRef* Pointer, uint Idx, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        [DllImport(llvmLibName, EntryPoint="LLVMBuildGlobalString", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildGlobalString(LLVMBuilderRef* B, [In][MarshalAs(UnmanagedType.LPStr)] string Str, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        [DllImport(llvmLibName, EntryPoint="LLVMBuildGlobalStringPtr", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildGlobalStringPtr(LLVMBuilderRef* B, [In][MarshalAs(UnmanagedType.LPStr)] string Str, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        [DllImport(llvmLibName, EntryPoint="LLVMGetVolatile", CallingConvention=CallingConvention.Cdecl)]
        public static extern int GetVolatile(LLVMValueRef* MemoryAccessInst);

        [DllImport(llvmLibName, EntryPoint="LLVMSetVolatile", CallingConvention=CallingConvention.Cdecl)]
        public static extern void SetVolatile(LLVMValueRef* MemoryAccessInst, int IsVolatile);

        [DllImport(llvmLibName, EntryPoint="LLVMBuildTrunc", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildTrunc(LLVMBuilderRef* LLVMBuilderRef, LLVMValueRef* Val, LLVMTypeRef* DestTy, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        [DllImport(llvmLibName, EntryPoint="LLVMBuildZExt", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildZExt(LLVMBuilderRef* LLVMBuilderRef, LLVMValueRef* Val, LLVMTypeRef* DestTy, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        [DllImport(llvmLibName, EntryPoint="LLVMBuildSExt", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildSExt(LLVMBuilderRef* LLVMBuilderRef, LLVMValueRef* Val, LLVMTypeRef* DestTy, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        [DllImport(llvmLibName, EntryPoint="LLVMBuildFPToUI", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildFPToUI(LLVMBuilderRef* LLVMBuilderRef, LLVMValueRef* Val, LLVMTypeRef* DestTy, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        [DllImport(llvmLibName, EntryPoint="LLVMBuildFPToSI", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildFPToSI(LLVMBuilderRef* LLVMBuilderRef, LLVMValueRef* Val, LLVMTypeRef* DestTy, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        [DllImport(llvmLibName, EntryPoint="LLVMBuildUIToFP", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildUIToFP(LLVMBuilderRef* LLVMBuilderRef, LLVMValueRef* Val, LLVMTypeRef* DestTy, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        [DllImport(llvmLibName, EntryPoint="LLVMBuildSIToFP", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildSIToFP(LLVMBuilderRef* LLVMBuilderRef, LLVMValueRef* Val, LLVMTypeRef* DestTy, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        [DllImport(llvmLibName, EntryPoint="LLVMBuildFPTrunc", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildFPTrunc(LLVMBuilderRef* LLVMBuilderRef, LLVMValueRef* Val, LLVMTypeRef* DestTy, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        [DllImport(llvmLibName, EntryPoint="LLVMBuildFPExt", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildFPExt(LLVMBuilderRef* LLVMBuilderRef, LLVMValueRef* Val, LLVMTypeRef* DestTy, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        [DllImport(llvmLibName, EntryPoint="LLVMBuildPtrToInt", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildPtrToInt(LLVMBuilderRef* LLVMBuilderRef, LLVMValueRef* Val, LLVMTypeRef* DestTy, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        [DllImport(llvmLibName, EntryPoint="LLVMBuildIntToPtr", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildIntToPtr(LLVMBuilderRef* LLVMBuilderRef, LLVMValueRef* Val, LLVMTypeRef* DestTy, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        [DllImport(llvmLibName, EntryPoint="LLVMBuildBitCast", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildBitCast(LLVMBuilderRef* LLVMBuilderRef, LLVMValueRef* Val, LLVMTypeRef* DestTy, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        [DllImport(llvmLibName, EntryPoint="LLVMBuildZExtOrBitCast", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildZExtOrBitCast(LLVMBuilderRef* LLVMBuilderRef, LLVMValueRef* Val, LLVMTypeRef* DestTy, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        [DllImport(llvmLibName, EntryPoint="LLVMBuildSExtOrBitCast", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildSExtOrBitCast(LLVMBuilderRef* LLVMBuilderRef, LLVMValueRef* Val, LLVMTypeRef* DestTy, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        [DllImport(llvmLibName, EntryPoint="LLVMBuildTruncOrBitCast", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildTruncOrBitCast(LLVMBuilderRef* LLVMBuilderRef, LLVMValueRef* Val, LLVMTypeRef* DestTy, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        [DllImport(llvmLibName, EntryPoint="LLVMBuildCast", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildCast(LLVMBuilderRef* B, LLVMOpcode Op, LLVMValueRef* Val, LLVMTypeRef* DestTy, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        [DllImport(llvmLibName, EntryPoint="LLVMBuildPointerCast", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildPointerCast(LLVMBuilderRef* LLVMBuilderRef, LLVMValueRef* Val, LLVMTypeRef* DestTy, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        [DllImport(llvmLibName, EntryPoint="LLVMBuildIntCast", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildIntCast(LLVMBuilderRef* LLVMBuilderRef, LLVMValueRef* Val, LLVMTypeRef* DestTy, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        [DllImport(llvmLibName, EntryPoint="LLVMBuildFPCast", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildFPCast(LLVMBuilderRef* LLVMBuilderRef, LLVMValueRef* Val, LLVMTypeRef* DestTy, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        [DllImport(llvmLibName, EntryPoint="LLVMBuildICmp", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildICmp(LLVMBuilderRef* LLVMBuilderRef, LLVMIntPredicate Op, LLVMValueRef* LHS, LLVMValueRef* RHS, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        [DllImport(llvmLibName, EntryPoint="LLVMBuildFCmp", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildFCmp(LLVMBuilderRef* LLVMBuilderRef, LLVMRealPredicate Op, LLVMValueRef* LHS, LLVMValueRef* RHS, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        [DllImport(llvmLibName, EntryPoint="LLVMBuildPhi", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildPhi(LLVMBuilderRef* LLVMBuilderRef, LLVMTypeRef* Ty, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        [DllImport(llvmLibName, EntryPoint="LLVMBuildCall", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildCall(LLVMBuilderRef* LLVMBuilderRef, LLVMValueRef* Fn, System.IntPtr[] Args, uint NumArgs, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        [DllImport(llvmLibName, EntryPoint="LLVMBuildSelect", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildSelect(LLVMBuilderRef* LLVMBuilderRef, LLVMValueRef* If, LLVMValueRef* Then, LLVMValueRef* Else, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        [DllImport(llvmLibName, EntryPoint="LLVMBuildVAArg", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildVAArg(LLVMBuilderRef* LLVMBuilderRef, LLVMValueRef* List, LLVMTypeRef* Ty, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        [DllImport(llvmLibName, EntryPoint="LLVMBuildExtractElement", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildExtractElement(LLVMBuilderRef* LLVMBuilderRef, LLVMValueRef* VecVal, LLVMValueRef* Index, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        [DllImport(llvmLibName, EntryPoint="LLVMBuildInsertElement", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildInsertElement(LLVMBuilderRef* LLVMBuilderRef, LLVMValueRef* VecVal, LLVMValueRef* EltVal, LLVMValueRef* Index, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        [DllImport(llvmLibName, EntryPoint="LLVMBuildShuffleVector", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildShuffleVector(LLVMBuilderRef* LLVMBuilderRef, LLVMValueRef* V1, LLVMValueRef* V2, LLVMValueRef* Mask, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        [DllImport(llvmLibName, EntryPoint="LLVMBuildExtractValue", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildExtractValue(LLVMBuilderRef* LLVMBuilderRef, LLVMValueRef* AggVal, uint Index, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        [DllImport(llvmLibName, EntryPoint="LLVMBuildInsertValue", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildInsertValue(LLVMBuilderRef* LLVMBuilderRef, LLVMValueRef* AggVal, LLVMValueRef* EltVal, uint Index, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        [DllImport(llvmLibName, EntryPoint="LLVMBuildIsNull", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildIsNull(LLVMBuilderRef* LLVMBuilderRef, LLVMValueRef* Val, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        [DllImport(llvmLibName, EntryPoint="LLVMBuildIsNotNull", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMValueRef* BuildIsNotNull(LLVMBuilderRef* LLVMBuilderRef, LLVMValueRef* Val, [In][MarshalAs(UnmanagedType.LPStr)] string Name);

        [DllImport(llvmLibName, EntryPoint="LLVMBuildPtrDiff", CallingConvention=CallingConvention.Cdecl)]
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
        [DllImport(llvmLibName, EntryPoint="LLVMCreateModuleProviderForExistingModule", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMModuleProviderRef* CreateModuleProviderForExistingModule(LLVMModuleRef* M);

        /* Destroys the module M.*/
        [DllImport(llvmLibName, EntryPoint="LLVMDisposeModuleProvider", CallingConvention=CallingConvention.Cdecl)]
        public static extern void DisposeModuleProvider(LLVMModuleProviderRef* M);

        /* @}
         */
        
        /**
         * @defgroup LLVMCCoreMemoryBuffers Memory Buffers
         *
         * @{*/
        [DllImport(llvmLibName, EntryPoint="LLVMCreateMemoryBufferWithContentsOfFile", CallingConvention=CallingConvention.Cdecl)]
        public static extern int CreateMemoryBufferWithContentsOfFile([In][MarshalAs(UnmanagedType.LPStr)] string Path, ref LLVMMemoryBufferRef * OutMemBuf, [MarshalAs(UnmanagedType.LPStr)] ref StringBuilder OutMessage);

        [DllImport(llvmLibName, EntryPoint="LLVMCreateMemoryBufferWithSTDIN", CallingConvention=CallingConvention.Cdecl)]
        public static extern int CreateMemoryBufferWithSTDIN(ref LLVMMemoryBufferRef * OutMemBuf, [MarshalAs(UnmanagedType.LPStr)] ref StringBuilder OutMessage);

        [DllImport(llvmLibName, EntryPoint="LLVMDisposeMemoryBuffer", CallingConvention=CallingConvention.Cdecl)]
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
        [DllImport(llvmLibName, EntryPoint="LLVMGetGlobalPassRegistry", CallingConvention=CallingConvention.Cdecl)]
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
        [DllImport(llvmLibName, EntryPoint="LLVMCreatePassManager", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMPassManagerRef* CreatePassManager();

        /* Constructs a new function-by-function pass pipeline over the module
            provider. It does not take ownership of the module provider. This type of
            pipeline is suitable for code generation and JIT compilation tasks.
            @see llvm::FunctionPassManager::FunctionPassManager*/
        [DllImport(llvmLibName, EntryPoint="LLVMCreateFunctionPassManagerForModule", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMPassManagerRef* CreateFunctionPassManagerForModule(LLVMModuleRef* M);

        /* Deprecated: Use LLVMCreateFunctionPassManagerForModule instead.*/
        [DllImport(llvmLibName, EntryPoint="LLVMCreateFunctionPassManager", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMPassManagerRef* CreateFunctionPassManager(LLVMModuleProviderRef* MP);

        /* Initializes, executes on the provided module, and finalizes all of the
            passes scheduled in the pass manager. Returns 1 if any of the passes
            modified the module, 0 otherwise.
            @see llvm::PassManager::run(Module&)*/
        [DllImport(llvmLibName, EntryPoint="LLVMRunPassManager", CallingConvention=CallingConvention.Cdecl)]
        public static extern int RunPassManager(LLVMPassManagerRef* PM, LLVMModuleRef* M);

        /* Initializes all of the function passes scheduled in the function pass
            manager. Returns 1 if any of the passes modified the module, 0 otherwise.
            @see llvm::FunctionPassManager::doInitialization*/
        [DllImport(llvmLibName, EntryPoint="LLVMInitializeFunctionPassManager", CallingConvention=CallingConvention.Cdecl)]
        public static extern int InitializeFunctionPassManager(LLVMPassManagerRef* FPM);

        /* Executes all of the function passes scheduled in the function pass manager
            on the provided function. Returns 1 if any of the passes modified the
            function, false otherwise.
            @see llvm::FunctionPassManager::run(Function&)*/
        [DllImport(llvmLibName, EntryPoint="LLVMRunFunctionPassManager", CallingConvention=CallingConvention.Cdecl)]
        public static extern int RunFunctionPassManager(LLVMPassManagerRef* FPM, LLVMValueRef* F);

        /* Finalizes all of the function passes scheduled in in the function pass
            manager. Returns 1 if any of the passes modified the module, 0 otherwise.
            @see llvm::FunctionPassManager::doFinalization*/
        [DllImport(llvmLibName, EntryPoint="LLVMFinalizeFunctionPassManager", CallingConvention=CallingConvention.Cdecl)]
        public static extern int FinalizeFunctionPassManager(LLVMPassManagerRef* FPM);

        /* Frees the memory of a pass pipeline. For function pipelines, does not free
            the module provider.
            @see llvm::PassManagerBase::~PassManagerBase.*/
        [DllImport(llvmLibName, EntryPoint="LLVMDisposePassManager", CallingConvention=CallingConvention.Cdecl)]
        public static extern void DisposePassManager(LLVMPassManagerRef* PM);

        [DllImport(llvmLibName, EntryPoint="LLVMInitializeHexagonTargetInfo", CallingConvention=CallingConvention.Cdecl)]
        public static extern void InitializeHexagonTargetInfo();

        [DllImport(llvmLibName, EntryPoint="LLVMInitializePTXTargetInfo", CallingConvention=CallingConvention.Cdecl)]
        public static extern void InitializePTXTargetInfo();

        [DllImport(llvmLibName, EntryPoint="LLVMInitializeMBlazeTargetInfo", CallingConvention=CallingConvention.Cdecl)]
        public static extern void InitializeMBlazeTargetInfo();

        [DllImport(llvmLibName, EntryPoint="LLVMInitializeCppBackendTargetInfo", CallingConvention=CallingConvention.Cdecl)]
        public static extern void InitializeCppBackendTargetInfo();

        [DllImport(llvmLibName, EntryPoint="LLVMInitializeMSP430TargetInfo", CallingConvention=CallingConvention.Cdecl)]
        public static extern void InitializeMSP430TargetInfo();

        [DllImport(llvmLibName, EntryPoint="LLVMInitializeXCoreTargetInfo", CallingConvention=CallingConvention.Cdecl)]
        public static extern void InitializeXCoreTargetInfo();

        [DllImport(llvmLibName, EntryPoint="LLVMInitializeCellSPUTargetInfo", CallingConvention=CallingConvention.Cdecl)]
        public static extern void InitializeCellSPUTargetInfo();

        [DllImport(llvmLibName, EntryPoint="LLVMInitializeMipsTargetInfo", CallingConvention=CallingConvention.Cdecl)]
        public static extern void InitializeMipsTargetInfo();

        [DllImport(llvmLibName, EntryPoint="LLVMInitializeARMTargetInfo", CallingConvention=CallingConvention.Cdecl)]
        public static extern void InitializeARMTargetInfo();

        [DllImport(llvmLibName, EntryPoint="LLVMInitializePowerPCTargetInfo", CallingConvention=CallingConvention.Cdecl)]
        public static extern void InitializePowerPCTargetInfo();

        [DllImport(llvmLibName, EntryPoint="LLVMInitializeSparcTargetInfo", CallingConvention=CallingConvention.Cdecl)]
        public static extern void InitializeSparcTargetInfo();

        [DllImport(llvmLibName, EntryPoint="LLVMInitializeX86TargetInfo", CallingConvention=CallingConvention.Cdecl)]
        public static extern void InitializeX86TargetInfo();

        [DllImport(llvmLibName, EntryPoint="LLVMInitializeHexagonTarget", CallingConvention=CallingConvention.Cdecl)]
        public static extern void InitializeHexagonTarget();

        [DllImport(llvmLibName, EntryPoint="LLVMInitializePTXTarget", CallingConvention=CallingConvention.Cdecl)]
        public static extern void InitializePTXTarget();

        [DllImport(llvmLibName, EntryPoint="LLVMInitializeMBlazeTarget", CallingConvention=CallingConvention.Cdecl)]
        public static extern void InitializeMBlazeTarget();

        [DllImport(llvmLibName, EntryPoint="LLVMInitializeCppBackendTarget", CallingConvention=CallingConvention.Cdecl)]
        public static extern void InitializeCppBackendTarget();

        [DllImport(llvmLibName, EntryPoint="LLVMInitializeMSP430Target", CallingConvention=CallingConvention.Cdecl)]
        public static extern void InitializeMSP430Target();

        [DllImport(llvmLibName, EntryPoint="LLVMInitializeXCoreTarget", CallingConvention=CallingConvention.Cdecl)]
        public static extern void InitializeXCoreTarget();

        [DllImport(llvmLibName, EntryPoint="LLVMInitializeCellSPUTarget", CallingConvention=CallingConvention.Cdecl)]
        public static extern void InitializeCellSPUTarget();

        [DllImport(llvmLibName, EntryPoint="LLVMInitializeMipsTarget", CallingConvention=CallingConvention.Cdecl)]
        public static extern void InitializeMipsTarget();

        [DllImport(llvmLibName, EntryPoint="LLVMInitializeARMTarget", CallingConvention=CallingConvention.Cdecl)]
        public static extern void InitializeARMTarget();

        [DllImport(llvmLibName, EntryPoint="LLVMInitializePowerPCTarget", CallingConvention=CallingConvention.Cdecl)]
        public static extern void InitializePowerPCTarget();

        [DllImport(llvmLibName, EntryPoint="LLVMInitializeSparcTarget", CallingConvention=CallingConvention.Cdecl)]
        public static extern void InitializeSparcTarget();

        [DllImport(llvmLibName, EntryPoint="LLVMInitializeX86Target", CallingConvention=CallingConvention.Cdecl)]
        public static extern void InitializeX86Target();

        [DllImport(llvmLibName, EntryPoint="LLVMInitializeHexagonTargetMC", CallingConvention=CallingConvention.Cdecl)]
        public static extern void InitializeHexagonTargetMC();

        [DllImport(llvmLibName, EntryPoint="LLVMInitializePTXTargetMC", CallingConvention=CallingConvention.Cdecl)]
        public static extern void InitializePTXTargetMC();

        [DllImport(llvmLibName, EntryPoint="LLVMInitializeMBlazeTargetMC", CallingConvention=CallingConvention.Cdecl)]
        public static extern void InitializeMBlazeTargetMC();

        [DllImport(llvmLibName, EntryPoint="LLVMInitializeCppBackendTargetMC", CallingConvention=CallingConvention.Cdecl)]
        public static extern void InitializeCppBackendTargetMC();

        [DllImport(llvmLibName, EntryPoint="LLVMInitializeMSP430TargetMC", CallingConvention=CallingConvention.Cdecl)]
        public static extern void InitializeMSP430TargetMC();

        [DllImport(llvmLibName, EntryPoint="LLVMInitializeXCoreTargetMC", CallingConvention=CallingConvention.Cdecl)]
        public static extern void InitializeXCoreTargetMC();

        [DllImport(llvmLibName, EntryPoint="LLVMInitializeCellSPUTargetMC", CallingConvention=CallingConvention.Cdecl)]
        public static extern void InitializeCellSPUTargetMC();

        [DllImport(llvmLibName, EntryPoint="LLVMInitializeMipsTargetMC", CallingConvention=CallingConvention.Cdecl)]
        public static extern void InitializeMipsTargetMC();

        [DllImport(llvmLibName, EntryPoint="LLVMInitializeARMTargetMC", CallingConvention=CallingConvention.Cdecl)]
        public static extern void InitializeARMTargetMC();

        [DllImport(llvmLibName, EntryPoint="LLVMInitializePowerPCTargetMC", CallingConvention=CallingConvention.Cdecl)]
        public static extern void InitializePowerPCTargetMC();

        [DllImport(llvmLibName, EntryPoint="LLVMInitializeSparcTargetMC", CallingConvention=CallingConvention.Cdecl)]
        public static extern void InitializeSparcTargetMC();

        [DllImport(llvmLibName, EntryPoint="LLVMInitializeX86TargetMC", CallingConvention=CallingConvention.Cdecl)]
        public static extern void InitializeX86TargetMC();

        [DllImport(llvmLibName, EntryPoint="LLVMInitializeHexagonAsmPrinter", CallingConvention=CallingConvention.Cdecl)]
        public static extern void InitializeHexagonAsmPrinter();

        [DllImport(llvmLibName, EntryPoint="LLVMInitializePTXAsmPrinter", CallingConvention=CallingConvention.Cdecl)]
        public static extern void InitializePTXAsmPrinter();

        [DllImport(llvmLibName, EntryPoint="LLVMInitializeMBlazeAsmPrinter", CallingConvention=CallingConvention.Cdecl)]
        public static extern void InitializeMBlazeAsmPrinter();

        [DllImport(llvmLibName, EntryPoint="LLVMInitializeMSP430AsmPrinter", CallingConvention=CallingConvention.Cdecl)]
        public static extern void InitializeMSP430AsmPrinter();

        [DllImport(llvmLibName, EntryPoint="LLVMInitializeXCoreAsmPrinter", CallingConvention=CallingConvention.Cdecl)]
        public static extern void InitializeXCoreAsmPrinter();

        [DllImport(llvmLibName, EntryPoint="LLVMInitializeCellSPUAsmPrinter", CallingConvention=CallingConvention.Cdecl)]
        public static extern void InitializeCellSPUAsmPrinter();

        [DllImport(llvmLibName, EntryPoint="LLVMInitializeMipsAsmPrinter", CallingConvention=CallingConvention.Cdecl)]
        public static extern void InitializeMipsAsmPrinter();

        [DllImport(llvmLibName, EntryPoint="LLVMInitializeARMAsmPrinter", CallingConvention=CallingConvention.Cdecl)]
        public static extern void InitializeARMAsmPrinter();

        [DllImport(llvmLibName, EntryPoint="LLVMInitializePowerPCAsmPrinter", CallingConvention=CallingConvention.Cdecl)]
        public static extern void InitializePowerPCAsmPrinter();

        [DllImport(llvmLibName, EntryPoint="LLVMInitializeSparcAsmPrinter", CallingConvention=CallingConvention.Cdecl)]
        public static extern void InitializeSparcAsmPrinter();

        [DllImport(llvmLibName, EntryPoint="LLVMInitializeX86AsmPrinter", CallingConvention=CallingConvention.Cdecl)]
        public static extern void InitializeX86AsmPrinter();

        [DllImport(llvmLibName, EntryPoint="LLVMInitializeMBlazeAsmParser", CallingConvention=CallingConvention.Cdecl)]
        public static extern void InitializeMBlazeAsmParser();

        [DllImport(llvmLibName, EntryPoint="LLVMInitializeMipsAsmParser", CallingConvention=CallingConvention.Cdecl)]
        public static extern void InitializeMipsAsmParser();

        [DllImport(llvmLibName, EntryPoint="LLVMInitializeARMAsmParser", CallingConvention=CallingConvention.Cdecl)]
        public static extern void InitializeARMAsmParser();

        [DllImport(llvmLibName, EntryPoint="LLVMInitializeX86AsmParser", CallingConvention=CallingConvention.Cdecl)]
        public static extern void InitializeX86AsmParser();

        [DllImport(llvmLibName, EntryPoint="LLVMInitializeMBlazeDisassembler", CallingConvention=CallingConvention.Cdecl)]
        public static extern void InitializeMBlazeDisassembler();

        [DllImport(llvmLibName, EntryPoint="LLVMInitializeMipsDisassembler", CallingConvention=CallingConvention.Cdecl)]
        public static extern void InitializeMipsDisassembler();

        [DllImport(llvmLibName, EntryPoint="LLVMInitializeARMDisassembler", CallingConvention=CallingConvention.Cdecl)]
        public static extern void InitializeARMDisassembler();

        [DllImport(llvmLibName, EntryPoint="LLVMInitializeX86Disassembler", CallingConvention=CallingConvention.Cdecl)]
        public static extern void InitializeX86Disassembler();

        /* @defgroup LLVMCExecutionEngine Execution Engine
         * @ingroup LLVMC
         *
         * @{*/
        [DllImport(llvmLibName, EntryPoint="LLVMLinkInJIT", CallingConvention=CallingConvention.Cdecl)]
        public static extern void LinkInJIT();

        [DllImport(llvmLibName, EntryPoint="LLVMLinkInInterpreter", CallingConvention=CallingConvention.Cdecl)]
        public static extern void LinkInInterpreter();

        [DllImport(llvmLibName, EntryPoint="LLVMCreateGenericValueOfInt", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMGenericValueRef* CreateGenericValueOfInt(LLVMTypeRef* Ty, ulong N, int IsSigned);

        [DllImport(llvmLibName, EntryPoint="LLVMCreateGenericValueOfPointer", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMGenericValueRef* CreateGenericValueOfPointer(System.IntPtr P);

        [DllImport(llvmLibName, EntryPoint="LLVMCreateGenericValueOfFloat", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMGenericValueRef* CreateGenericValueOfFloat(LLVMTypeRef* Ty, double N);

        [DllImport(llvmLibName, EntryPoint="LLVMGenericValueIntWidth", CallingConvention=CallingConvention.Cdecl)]
        public static extern uint GenericValueIntWidth(LLVMGenericValueRef* GenValRef);

        [DllImport(llvmLibName, EntryPoint="LLVMGenericValueToInt", CallingConvention=CallingConvention.Cdecl)]
        public static extern ulong GenericValueToInt(LLVMGenericValueRef* GenVal, int IsSigned);

        [DllImport(llvmLibName, EntryPoint="LLVMGenericValueToPointer", CallingConvention=CallingConvention.Cdecl)]
        public static extern System.IntPtr GenericValueToPointer(LLVMGenericValueRef* GenVal);

        [DllImport(llvmLibName, EntryPoint="LLVMGenericValueToFloat", CallingConvention=CallingConvention.Cdecl)]
        public static extern double GenericValueToFloat(LLVMTypeRef* TyRef, LLVMGenericValueRef* GenVal);

        [DllImport(llvmLibName, EntryPoint="LLVMDisposeGenericValue", CallingConvention=CallingConvention.Cdecl)]
        public static extern void DisposeGenericValue(LLVMGenericValueRef* GenVal);

        [DllImport(llvmLibName, EntryPoint="LLVMCreateExecutionEngineForModule", CallingConvention=CallingConvention.Cdecl)]
        public static extern int CreateExecutionEngineForModule(ref LLVMExecutionEngineRef * OutEE, LLVMModuleRef* M, [MarshalAs(UnmanagedType.LPStr)] ref StringBuilder OutError);

        [DllImport(llvmLibName, EntryPoint="LLVMCreateInterpreterForModule", CallingConvention=CallingConvention.Cdecl)]
        public static extern int CreateInterpreterForModule(ref LLVMExecutionEngineRef * OutInterp, LLVMModuleRef* M, [MarshalAs(UnmanagedType.LPStr)] ref StringBuilder OutError);

        [DllImport(llvmLibName, EntryPoint="LLVMCreateJITCompilerForModule", CallingConvention=CallingConvention.Cdecl)]
        public static extern int CreateJITCompilerForModule(ref LLVMExecutionEngineRef * OutJIT, LLVMModuleRef* M, uint OptLevel, [MarshalAs(UnmanagedType.LPStr)] ref StringBuilder OutError);

        /* Deprecated: Use LLVMCreateExecutionEngineForModule instead.*/
        [DllImport(llvmLibName, EntryPoint="LLVMCreateExecutionEngine", CallingConvention=CallingConvention.Cdecl)]
        public static extern int CreateExecutionEngine(ref LLVMExecutionEngineRef * OutEE, LLVMModuleProviderRef* MP, [MarshalAs(UnmanagedType.LPStr)] ref StringBuilder OutError);

        /* Deprecated: Use LLVMCreateInterpreterForModule instead.*/
        [DllImport(llvmLibName, EntryPoint="LLVMCreateInterpreter", CallingConvention=CallingConvention.Cdecl)]
        public static extern int CreateInterpreter(ref LLVMExecutionEngineRef * OutInterp, LLVMModuleProviderRef* MP, [MarshalAs(UnmanagedType.LPStr)] ref StringBuilder OutError);

        /* Deprecated: Use LLVMCreateJITCompilerForModule instead.*/
        [DllImport(llvmLibName, EntryPoint="LLVMCreateJITCompiler", CallingConvention=CallingConvention.Cdecl)]
        public static extern int CreateJITCompiler(ref LLVMExecutionEngineRef * OutJIT, LLVMModuleProviderRef* MP, uint OptLevel, [MarshalAs(UnmanagedType.LPStr)] ref StringBuilder OutError);

        [DllImport(llvmLibName, EntryPoint="LLVMDisposeExecutionEngine", CallingConvention=CallingConvention.Cdecl)]
        public static extern void DisposeExecutionEngine(LLVMExecutionEngineRef* EE);

        [DllImport(llvmLibName, EntryPoint="LLVMRunStaticConstructors", CallingConvention=CallingConvention.Cdecl)]
        public static extern void RunStaticConstructors(LLVMExecutionEngineRef* EE);

        [DllImport(llvmLibName, EntryPoint="LLVMRunStaticDestructors", CallingConvention=CallingConvention.Cdecl)]
        public static extern void RunStaticDestructors(LLVMExecutionEngineRef* EE);

        [DllImport(llvmLibName, EntryPoint="LLVMRunFunctionAsMain", CallingConvention=CallingConvention.Cdecl)]
        public static extern int RunFunctionAsMain(LLVMExecutionEngineRef* EE, LLVMValueRef* F, uint ArgC, [MarshalAs(UnmanagedType.LPStr)] ref StringBuilder ArgV, [MarshalAs(UnmanagedType.LPStr)] ref StringBuilder EnvP);

        [DllImport(llvmLibName, EntryPoint="LLVMRunFunction", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMGenericValueRef* RunFunction(LLVMExecutionEngineRef* EE, LLVMValueRef* F, uint NumArgs, System.IntPtr[] Args);

        [DllImport(llvmLibName, EntryPoint="LLVMFreeMachineCodeForFunction", CallingConvention=CallingConvention.Cdecl)]
        public static extern void FreeMachineCodeForFunction(LLVMExecutionEngineRef* EE, LLVMValueRef* F);

        [DllImport(llvmLibName, EntryPoint="LLVMAddModule", CallingConvention=CallingConvention.Cdecl)]
        public static extern void AddModule(LLVMExecutionEngineRef* EE, LLVMModuleRef* M);

        /* Deprecated: Use LLVMAddModule instead.*/
        [DllImport(llvmLibName, EntryPoint="LLVMAddModuleProvider", CallingConvention=CallingConvention.Cdecl)]
        public static extern void AddModuleProvider(LLVMExecutionEngineRef* EE, LLVMModuleProviderRef* MP);

        [DllImport(llvmLibName, EntryPoint="LLVMRemoveModule", CallingConvention=CallingConvention.Cdecl)]
        public static extern int RemoveModule(LLVMExecutionEngineRef* EE, LLVMModuleRef* M, ref LLVMModuleRef * OutMod, [MarshalAs(UnmanagedType.LPStr)] ref StringBuilder OutError);

        /* Deprecated: Use LLVMRemoveModule instead.*/
        [DllImport(llvmLibName, EntryPoint="LLVMRemoveModuleProvider", CallingConvention=CallingConvention.Cdecl)]
        public static extern int RemoveModuleProvider(LLVMExecutionEngineRef* EE, LLVMModuleProviderRef* MP, ref LLVMModuleRef * OutMod, [MarshalAs(UnmanagedType.LPStr)] ref StringBuilder OutError);

        [DllImport(llvmLibName, EntryPoint="LLVMFindFunction", CallingConvention=CallingConvention.Cdecl)]
        public static extern int FindFunction(LLVMExecutionEngineRef* EE, [In][MarshalAs(UnmanagedType.LPStr)] string Name, ref LLVMValueRef * OutFn);

        [DllImport(llvmLibName, EntryPoint="LLVMRecompileAndRelinkFunction", CallingConvention=CallingConvention.Cdecl)]
        public static extern System.IntPtr RecompileAndRelinkFunction(LLVMExecutionEngineRef* EE, LLVMValueRef* Fn);

        [DllImport(llvmLibName, EntryPoint="LLVMGetExecutionEngineTargetData", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMTargetDataRef* GetExecutionEngineTargetData(LLVMExecutionEngineRef* EE);

        [DllImport(llvmLibName, EntryPoint="LLVMAddGlobalMapping", CallingConvention=CallingConvention.Cdecl)]
        public static extern void AddGlobalMapping(LLVMExecutionEngineRef* EE, LLVMValueRef* Global, System.IntPtr Addr);

        [DllImport(llvmLibName, EntryPoint="LLVMGetPointerToGlobal", CallingConvention=CallingConvention.Cdecl)]
        public static extern System.IntPtr GetPointerToGlobal(LLVMExecutionEngineRef* EE, LLVMValueRef* Global);

        [DllImport(llvmLibName, EntryPoint="LLVMInitializeTransformUtils", CallingConvention=CallingConvention.Cdecl)]
        public static extern void InitializeTransformUtils(LLVMPassRegistryRef* R);

        [DllImport(llvmLibName, EntryPoint="LLVMInitializeScalarOpts", CallingConvention=CallingConvention.Cdecl)]
        public static extern void InitializeScalarOpts(LLVMPassRegistryRef* R);

        [DllImport(llvmLibName, EntryPoint="LLVMInitializeVectorization", CallingConvention=CallingConvention.Cdecl)]
        public static extern void InitializeVectorization(LLVMPassRegistryRef* R);

        [DllImport(llvmLibName, EntryPoint="LLVMInitializeInstCombine", CallingConvention=CallingConvention.Cdecl)]
        public static extern void InitializeInstCombine(LLVMPassRegistryRef* R);

        [DllImport(llvmLibName, EntryPoint="LLVMInitializeIPO", CallingConvention=CallingConvention.Cdecl)]
        public static extern void InitializeIPO(LLVMPassRegistryRef* R);

        [DllImport(llvmLibName, EntryPoint="LLVMInitializeInstrumentation", CallingConvention=CallingConvention.Cdecl)]
        public static extern void InitializeInstrumentation(LLVMPassRegistryRef* R);

        [DllImport(llvmLibName, EntryPoint="LLVMInitializeAnalysis", CallingConvention=CallingConvention.Cdecl)]
        public static extern void InitializeAnalysis(LLVMPassRegistryRef* R);

        [DllImport(llvmLibName, EntryPoint="LLVMInitializeIPA", CallingConvention=CallingConvention.Cdecl)]
        public static extern void InitializeIPA(LLVMPassRegistryRef* R);

        [DllImport(llvmLibName, EntryPoint="LLVMInitializeCodeGen", CallingConvention=CallingConvention.Cdecl)]
        public static extern void InitializeCodeGen(LLVMPassRegistryRef* R);

        [DllImport(llvmLibName, EntryPoint="LLVMInitializeTarget", CallingConvention=CallingConvention.Cdecl)]
        public static extern void InitializeTarget(LLVMPassRegistryRef* R);

        /* This provides C interface to initialize link time optimizer. This allows
          /// linker to use dlopen() interface to dynamically load LinkTimeOptimizer.
          /// extern "C" helps, because dlopen() interface uses name to find the symbol.*/
        [DllImport(llvmLibName, EntryPoint="llvm_create_optimizer", CallingConvention=CallingConvention.Cdecl)]
        public static extern System.IntPtr llvm_create_optimizer();

        [DllImport(llvmLibName, EntryPoint="llvm_destroy_optimizer", CallingConvention=CallingConvention.Cdecl)]
        public static extern void llvm_destroy_optimizer(System.IntPtr lto);

        [DllImport(llvmLibName, EntryPoint="llvm_read_object_file", CallingConvention=CallingConvention.Cdecl)]
        public static extern llvm_lto_status llvm_read_object_file(System.IntPtr lto, [In][MarshalAs(UnmanagedType.LPStr)] string input_filename);

        [DllImport(llvmLibName, EntryPoint="llvm_optimize_modules", CallingConvention=CallingConvention.Cdecl)]
        public static extern llvm_lto_status llvm_optimize_modules(System.IntPtr lto, [In][MarshalAs(UnmanagedType.LPStr)] string output_filename);

        [DllImport(llvmLibName, EntryPoint="LLVMCreateObjectFile", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMObjectFileRef* CreateObjectFile(LLVMMemoryBufferRef* MemBuf);

        [DllImport(llvmLibName, EntryPoint="LLVMDisposeObjectFile", CallingConvention=CallingConvention.Cdecl)]
        public static extern void DisposeObjectFile(LLVMObjectFileRef* ObjectFile);

        [DllImport(llvmLibName, EntryPoint="LLVMGetSections", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMSectionIteratorRef* GetSections(LLVMObjectFileRef* ObjectFile);

        [DllImport(llvmLibName, EntryPoint="LLVMDisposeSectionIterator", CallingConvention=CallingConvention.Cdecl)]
        public static extern void DisposeSectionIterator(LLVMSectionIteratorRef* SI);

        [DllImport(llvmLibName, EntryPoint="LLVMIsSectionIteratorAtEnd", CallingConvention=CallingConvention.Cdecl)]
        public static extern int IsSectionIteratorAtEnd(LLVMObjectFileRef* ObjectFile, LLVMSectionIteratorRef* SI);

        [DllImport(llvmLibName, EntryPoint="LLVMMoveToNextSection", CallingConvention=CallingConvention.Cdecl)]
        public static extern void MoveToNextSection(LLVMSectionIteratorRef* SI);

        [DllImport(llvmLibName, EntryPoint="LLVMMoveToContainingSection", CallingConvention=CallingConvention.Cdecl)]
        public static extern void MoveToContainingSection(LLVMSectionIteratorRef* Sect, LLVMSymbolIteratorRef* Sym);

        [DllImport(llvmLibName, EntryPoint="LLVMGetSymbols", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMSymbolIteratorRef* GetSymbols(LLVMObjectFileRef* ObjectFile);

        [DllImport(llvmLibName, EntryPoint="LLVMDisposeSymbolIterator", CallingConvention=CallingConvention.Cdecl)]
        public static extern void DisposeSymbolIterator(LLVMSymbolIteratorRef* SI);

        [DllImport(llvmLibName, EntryPoint="LLVMIsSymbolIteratorAtEnd", CallingConvention=CallingConvention.Cdecl)]
        public static extern int IsSymbolIteratorAtEnd(LLVMObjectFileRef* ObjectFile, LLVMSymbolIteratorRef* SI);

        [DllImport(llvmLibName, EntryPoint="LLVMMoveToNextSymbol", CallingConvention=CallingConvention.Cdecl)]
        public static extern void MoveToNextSymbol(LLVMSymbolIteratorRef* SI);

        [DllImport(llvmLibName, EntryPoint="LLVMGetSectionName", CallingConvention=CallingConvention.Cdecl)]
        public static extern System.IntPtr GetSectionName(LLVMSectionIteratorRef* SI);

        [DllImport(llvmLibName, EntryPoint="LLVMGetSectionSize", CallingConvention=CallingConvention.Cdecl)]
        public static extern ulong GetSectionSize(LLVMSectionIteratorRef* SI);

        [DllImport(llvmLibName, EntryPoint="LLVMGetSectionContents", CallingConvention=CallingConvention.Cdecl)]
        public static extern System.IntPtr GetSectionContents(LLVMSectionIteratorRef* SI);

        [DllImport(llvmLibName, EntryPoint="LLVMGetSectionAddress", CallingConvention=CallingConvention.Cdecl)]
        public static extern ulong GetSectionAddress(LLVMSectionIteratorRef* SI);

        [DllImport(llvmLibName, EntryPoint="LLVMGetSectionContainsSymbol", CallingConvention=CallingConvention.Cdecl)]
        public static extern int GetSectionContainsSymbol(LLVMSectionIteratorRef* SI, LLVMSymbolIteratorRef* Sym);

        [DllImport(llvmLibName, EntryPoint="LLVMGetRelocations", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMRelocationIteratorRef* GetRelocations(LLVMSectionIteratorRef* Section);

        [DllImport(llvmLibName, EntryPoint="LLVMDisposeRelocationIterator", CallingConvention=CallingConvention.Cdecl)]
        public static extern void DisposeRelocationIterator(LLVMRelocationIteratorRef* RI);

        [DllImport(llvmLibName, EntryPoint="LLVMIsRelocationIteratorAtEnd", CallingConvention=CallingConvention.Cdecl)]
        public static extern int IsRelocationIteratorAtEnd(LLVMSectionIteratorRef* Section, LLVMRelocationIteratorRef* RI);

        [DllImport(llvmLibName, EntryPoint="LLVMMoveToNextRelocation", CallingConvention=CallingConvention.Cdecl)]
        public static extern void MoveToNextRelocation(LLVMRelocationIteratorRef* RI);

        [DllImport(llvmLibName, EntryPoint="LLVMGetSymbolName", CallingConvention=CallingConvention.Cdecl)]
        public static extern System.IntPtr GetSymbolName(LLVMSymbolIteratorRef* SI);

        [DllImport(llvmLibName, EntryPoint="LLVMGetSymbolAddress", CallingConvention=CallingConvention.Cdecl)]
        public static extern ulong GetSymbolAddress(LLVMSymbolIteratorRef* SI);

        [DllImport(llvmLibName, EntryPoint="LLVMGetSymbolFileOffset", CallingConvention=CallingConvention.Cdecl)]
        public static extern ulong GetSymbolFileOffset(LLVMSymbolIteratorRef* SI);

        [DllImport(llvmLibName, EntryPoint="LLVMGetSymbolSize", CallingConvention=CallingConvention.Cdecl)]
        public static extern ulong GetSymbolSize(LLVMSymbolIteratorRef* SI);

        [DllImport(llvmLibName, EntryPoint="LLVMGetRelocationAddress", CallingConvention=CallingConvention.Cdecl)]
        public static extern ulong GetRelocationAddress(LLVMRelocationIteratorRef* RI);

        [DllImport(llvmLibName, EntryPoint="LLVMGetRelocationOffset", CallingConvention=CallingConvention.Cdecl)]
        public static extern ulong GetRelocationOffset(LLVMRelocationIteratorRef* RI);

        [DllImport(llvmLibName, EntryPoint="LLVMGetRelocationSymbol", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMSymbolIteratorRef* GetRelocationSymbol(LLVMRelocationIteratorRef* RI);

        [DllImport(llvmLibName, EntryPoint="LLVMGetRelocationType", CallingConvention=CallingConvention.Cdecl)]
        public static extern ulong GetRelocationType(LLVMRelocationIteratorRef* RI);

        [DllImport(llvmLibName, EntryPoint="LLVMGetRelocationTypeName", CallingConvention=CallingConvention.Cdecl)]
        public static extern System.IntPtr GetRelocationTypeName(LLVMRelocationIteratorRef* RI);

        [DllImport(llvmLibName, EntryPoint="LLVMGetRelocationValueString", CallingConvention=CallingConvention.Cdecl)]
        public static extern System.IntPtr GetRelocationValueString(LLVMRelocationIteratorRef* RI);

        /* LLVMInitializeAllTargetInfos - The main program should call this function if
            it wants access to all available targets that LLVM is configured to
            support.*/
        [DllImport(llvmLibName, EntryPoint="LLVMInitializeAllTargetInfos", CallingConvention=CallingConvention.Cdecl)]
        public static extern void InitializeAllTargetInfos();

        /* LLVMInitializeAllTargets - The main program should call this function if it
            wants to link in all available targets that LLVM is configured to
            support.*/
        [DllImport(llvmLibName, EntryPoint="LLVMInitializeAllTargets", CallingConvention=CallingConvention.Cdecl)]
        public static extern void InitializeAllTargets();

        /* LLVMInitializeAllTargetMCs - The main program should call this function if
            it wants access to all available target MC that LLVM is configured to
            support.*/
        [DllImport(llvmLibName, EntryPoint="LLVMInitializeAllTargetMCs", CallingConvention=CallingConvention.Cdecl)]
        public static extern void InitializeAllTargetMCs();

        /* LLVMInitializeAllAsmPrinters - The main program should call this function if
            it wants all asm printers that LLVM is configured to support, to make them
            available via the TargetRegistry.*/
        [DllImport(llvmLibName, EntryPoint="LLVMInitializeAllAsmPrinters", CallingConvention=CallingConvention.Cdecl)]
        public static extern void InitializeAllAsmPrinters();

        /* LLVMInitializeAllAsmParsers - The main program should call this function if
            it wants all asm parsers that LLVM is configured to support, to make them
            available via the TargetRegistry.*/
        [DllImport(llvmLibName, EntryPoint="LLVMInitializeAllAsmParsers", CallingConvention=CallingConvention.Cdecl)]
        public static extern void InitializeAllAsmParsers();

        /* LLVMInitializeAllDisassemblers - The main program should call this function
            if it wants all disassemblers that LLVM is configured to support, to make
            them available via the TargetRegistry.*/
        [DllImport(llvmLibName, EntryPoint="LLVMInitializeAllDisassemblers", CallingConvention=CallingConvention.Cdecl)]
        public static extern void InitializeAllDisassemblers();

        /* LLVMInitializeNativeTarget - The main program should call this function to
            initialize the native target corresponding to the host.  This is useful 
            for JIT applications to ensure that the target gets linked in correctly.*/
        [DllImport(llvmLibName, EntryPoint="LLVMInitializeNativeTarget", CallingConvention=CallingConvention.Cdecl)]
        public static extern int InitializeNativeTarget();

        /* Creates target data from a target layout string.
            See the constructor llvm::TargetData::TargetData.*/
        [DllImport(llvmLibName, EntryPoint="LLVMCreateTargetData", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMTargetDataRef* CreateTargetData([In][MarshalAs(UnmanagedType.LPStr)] string StringRep);

        /* Adds target data information to a pass manager. This does not take ownership
            of the target data.
            See the method llvm::PassManagerBase::add.*/
        [DllImport(llvmLibName, EntryPoint="LLVMAddTargetData", CallingConvention=CallingConvention.Cdecl)]
        public static extern void AddTargetData(LLVMTargetDataRef* LLVMTargetDataRef, LLVMPassManagerRef* LLVMPassManagerRef);

        /* Adds target library information to a pass manager. This does not take
            ownership of the target library info.
            See the method llvm::PassManagerBase::add.*/
        [DllImport(llvmLibName, EntryPoint="LLVMAddTargetLibraryInfo", CallingConvention=CallingConvention.Cdecl)]
        public static extern void AddTargetLibraryInfo(LLVMTargetLibraryInfoRef* LLVMTargetLibraryInfoRef, LLVMPassManagerRef* LLVMPassManagerRef);

        /* Converts target data to a target layout string. The string must be disposed
            with LLVMDisposeMessage.
            See the constructor llvm::TargetData::TargetData.*/
        [DllImport(llvmLibName, EntryPoint="LLVMCopyStringRepOfTargetData", CallingConvention=CallingConvention.Cdecl)]
        public static extern System.IntPtr CopyStringRepOfTargetData(LLVMTargetDataRef* LLVMTargetDataRef);

        /* Returns the byte order of a target, either LLVMBigEndian or
            LLVMLittleEndian.
            See the method llvm::TargetData::isLittleEndian.*/
        [DllImport(llvmLibName, EntryPoint="LLVMByteOrder", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMByteOrdering ByteOrder(LLVMTargetDataRef* LLVMTargetDataRef);

        /* Returns the pointer size in bytes for a target.
            See the method llvm::TargetData::getPointerSize.*/
        [DllImport(llvmLibName, EntryPoint="LLVMPointerSize", CallingConvention=CallingConvention.Cdecl)]
        public static extern uint PointerSize(LLVMTargetDataRef* LLVMTargetDataRef);

        /* Returns the integer type that is the same size as a pointer on a target.
            See the method llvm::TargetData::getIntPtrType.*/
        [DllImport(llvmLibName, EntryPoint="LLVMIntPtrType", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMTypeRef* IntPtrType(LLVMTargetDataRef* LLVMTargetDataRef);

        /* Computes the size of a type in bytes for a target.
            See the method llvm::TargetData::getTypeSizeInBits.*/
        [DllImport(llvmLibName, EntryPoint="LLVMSizeOfTypeInBits", CallingConvention=CallingConvention.Cdecl)]
        public static extern ulong SizeOfTypeInBits(LLVMTargetDataRef* LLVMTargetDataRef, LLVMTypeRef* LLVMTypeRef);

        /* Computes the storage size of a type in bytes for a target.
            See the method llvm::TargetData::getTypeStoreSize.*/
        [DllImport(llvmLibName, EntryPoint="LLVMStoreSizeOfType", CallingConvention=CallingConvention.Cdecl)]
        public static extern ulong StoreSizeOfType(LLVMTargetDataRef* LLVMTargetDataRef, LLVMTypeRef* LLVMTypeRef);

        /* Computes the ABI size of a type in bytes for a target.
            See the method llvm::TargetData::getTypeAllocSize.*/
        [DllImport(llvmLibName, EntryPoint="LLVMABISizeOfType", CallingConvention=CallingConvention.Cdecl)]
        public static extern ulong ABISizeOfType(LLVMTargetDataRef* LLVMTargetDataRef, LLVMTypeRef* LLVMTypeRef);

        /* Computes the ABI alignment of a type in bytes for a target.
            See the method llvm::TargetData::getTypeABISize.*/
        [DllImport(llvmLibName, EntryPoint="LLVMABIAlignmentOfType", CallingConvention=CallingConvention.Cdecl)]
        public static extern uint ABIAlignmentOfType(LLVMTargetDataRef* LLVMTargetDataRef, LLVMTypeRef* LLVMTypeRef);

        /* Computes the call frame alignment of a type in bytes for a target.
            See the method llvm::TargetData::getTypeABISize.*/
        [DllImport(llvmLibName, EntryPoint="LLVMCallFrameAlignmentOfType", CallingConvention=CallingConvention.Cdecl)]
        public static extern uint CallFrameAlignmentOfType(LLVMTargetDataRef* LLVMTargetDataRef, LLVMTypeRef* LLVMTypeRef);

        /* Computes the preferred alignment of a type in bytes for a target.
            See the method llvm::TargetData::getTypeABISize.*/
        [DllImport(llvmLibName, EntryPoint="LLVMPreferredAlignmentOfType", CallingConvention=CallingConvention.Cdecl)]
        public static extern uint PreferredAlignmentOfType(LLVMTargetDataRef* LLVMTargetDataRef, LLVMTypeRef* LLVMTypeRef);

        /* Computes the preferred alignment of a global variable in bytes for a target.
            See the method llvm::TargetData::getPreferredAlignment.*/
        [DllImport(llvmLibName, EntryPoint="LLVMPreferredAlignmentOfGlobal", CallingConvention=CallingConvention.Cdecl)]
        public static extern uint PreferredAlignmentOfGlobal(LLVMTargetDataRef* LLVMTargetDataRef, LLVMValueRef* GlobalVar);

        /* Computes the structure element that contains the byte offset for a target.
            See the method llvm::StructLayout::getElementContainingOffset.*/
        [DllImport(llvmLibName, EntryPoint="LLVMElementAtOffset", CallingConvention=CallingConvention.Cdecl)]
        public static extern uint ElementAtOffset(LLVMTargetDataRef* LLVMTargetDataRef, LLVMTypeRef* StructTy, ulong Offset);

        /* Computes the byte offset of the indexed struct element for a target.
            See the method llvm::StructLayout::getElementContainingOffset.*/
        [DllImport(llvmLibName, EntryPoint="LLVMOffsetOfElement", CallingConvention=CallingConvention.Cdecl)]
        public static extern ulong OffsetOfElement(LLVMTargetDataRef* LLVMTargetDataRef, LLVMTypeRef* StructTy, uint Element);

        /* Deallocates a TargetData.
            See the destructor llvm::TargetData::~TargetData.*/
        [DllImport(llvmLibName, EntryPoint="LLVMDisposeTargetData", CallingConvention=CallingConvention.Cdecl)]
        public static extern void DisposeTargetData(LLVMTargetDataRef* LLVMTargetDataRef);

        /* Returns the first llvm::Target in the registered targets list.*/
        [DllImport(llvmLibName, EntryPoint="LLVMGetFirstTarget", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMTargetRef* GetFirstTarget();

        /* Returns the next llvm::Target given a previous one (or null if there's none)*/
        [DllImport(llvmLibName, EntryPoint="LLVMGetNextTarget", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMTargetRef* GetNextTarget(LLVMTargetRef* T);

        /* Returns the name of a target. See llvm::Target::getName*/
        [DllImport(llvmLibName, EntryPoint="LLVMGetTargetName", CallingConvention=CallingConvention.Cdecl)]
        public static extern System.IntPtr GetTargetName(LLVMTargetRef* T);

        /* Returns the description  of a target. See llvm::Target::getDescription*/
        [DllImport(llvmLibName, EntryPoint="LLVMGetTargetDescription", CallingConvention=CallingConvention.Cdecl)]
        public static extern System.IntPtr GetTargetDescription(LLVMTargetRef* T);

        /* Returns if the target has a JIT*/
        [DllImport(llvmLibName, EntryPoint="LLVMTargetHasJIT", CallingConvention=CallingConvention.Cdecl)]
        public static extern int TargetHasJIT(LLVMTargetRef* T);

        /* Returns if the target has a TargetMachine associated*/
        [DllImport(llvmLibName, EntryPoint="LLVMTargetHasTargetMachine", CallingConvention=CallingConvention.Cdecl)]
        public static extern int TargetHasTargetMachine(LLVMTargetRef* T);

        /* Returns if the target as an ASM backend (required for emitting output)*/
        [DllImport(llvmLibName, EntryPoint="LLVMTargetHasAsmBackend", CallingConvention=CallingConvention.Cdecl)]
        public static extern int TargetHasAsmBackend(LLVMTargetRef* T);

        /* Creates a new llvm::TargetMachine. See llvm::Target::createTargetMachine*/
        [DllImport(llvmLibName, EntryPoint="LLVMCreateTargetMachine", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMTargetMachineRef* CreateTargetMachine(LLVMTargetRef* T, [In][MarshalAs(UnmanagedType.LPStr)] string Triple, [In][MarshalAs(UnmanagedType.LPStr)] string CPU, [In][MarshalAs(UnmanagedType.LPStr)] string Features, LLVMCodeGenOptLevel Level, LLVMRelocMode Reloc, LLVMCodeModel CodeModel);

        /* Dispose the LLVMTargetMachineRef instance generated by
          LLVMCreateTargetMachine.*/
        [DllImport(llvmLibName, EntryPoint="LLVMDisposeTargetMachine", CallingConvention=CallingConvention.Cdecl)]
        public static extern void DisposeTargetMachine(LLVMTargetMachineRef* T);

        /* Returns the Target used in a TargetMachine*/
        [DllImport(llvmLibName, EntryPoint="LLVMGetTargetMachineTarget", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMTargetRef* GetTargetMachineTarget(LLVMTargetMachineRef* T);

        /* Returns the triple used creating this target machine. See
          llvm::TargetMachine::getTriple. The result needs to be disposed with
          LLVMDisposeMessage.*/
        [DllImport(llvmLibName, EntryPoint="LLVMGetTargetMachineTriple", CallingConvention=CallingConvention.Cdecl)]
        public static extern System.IntPtr GetTargetMachineTriple(LLVMTargetMachineRef* T);

        /* Returns the cpu used creating this target machine. See
          llvm::TargetMachine::getCPU. The result needs to be disposed with
          LLVMDisposeMessage.*/
        [DllImport(llvmLibName, EntryPoint="LLVMGetTargetMachineCPU", CallingConvention=CallingConvention.Cdecl)]
        public static extern System.IntPtr GetTargetMachineCPU(LLVMTargetMachineRef* T);

        /* Returns the feature string used creating this target machine. See
          llvm::TargetMachine::getFeatureString. The result needs to be disposed with
          LLVMDisposeMessage.*/
        [DllImport(llvmLibName, EntryPoint="LLVMGetTargetMachineFeatureString", CallingConvention=CallingConvention.Cdecl)]
        public static extern System.IntPtr GetTargetMachineFeatureString(LLVMTargetMachineRef* T);

        /* Returns the llvm::TargetData used for this llvm:TargetMachine.*/
        [DllImport(llvmLibName, EntryPoint="LLVMGetTargetMachineData", CallingConvention=CallingConvention.Cdecl)]
        public static extern int GetTargetMachineData(LLVMTargetMachineRef* T);

        /* Emits an asm or object file for the given module to the filename. This
          wraps several c++ only classes (among them a file stream). Returns any
          error in ErrorMessage. Use LLVMDisposeMessage to dispose the message.*/
        [DllImport(llvmLibName, EntryPoint="LLVMTargetMachineEmitToFile", CallingConvention=CallingConvention.Cdecl)]
        public static extern int TargetMachineEmitToFile(LLVMTargetMachineRef* T, LLVMModuleRef* M, [In][MarshalAs(UnmanagedType.LPStr)] string Filename, LLVMCodeGenFileType codegen, [MarshalAs(UnmanagedType.LPStr)] ref StringBuilder ErrorMessage);

        /* @defgroup LLVMCTransformsIPO Interprocedural transformations
         * @ingroup LLVMCTransforms
         *
         * @{
         */
        
        /** See llvm::createArgumentPromotionPass function.*/
        [DllImport(llvmLibName, EntryPoint="LLVMAddArgumentPromotionPass", CallingConvention=CallingConvention.Cdecl)]
        public static extern void AddArgumentPromotionPass(LLVMPassManagerRef* PM);

        /* See llvm::createConstantMergePass function.*/
        [DllImport(llvmLibName, EntryPoint="LLVMAddConstantMergePass", CallingConvention=CallingConvention.Cdecl)]
        public static extern void AddConstantMergePass(LLVMPassManagerRef* PM);

        /* See llvm::createDeadArgEliminationPass function.*/
        [DllImport(llvmLibName, EntryPoint="LLVMAddDeadArgEliminationPass", CallingConvention=CallingConvention.Cdecl)]
        public static extern void AddDeadArgEliminationPass(LLVMPassManagerRef* PM);

        /* See llvm::createFunctionAttrsPass function.*/
        [DllImport(llvmLibName, EntryPoint="LLVMAddFunctionAttrsPass", CallingConvention=CallingConvention.Cdecl)]
        public static extern void AddFunctionAttrsPass(LLVMPassManagerRef* PM);

        /* See llvm::createFunctionInliningPass function.*/
        [DllImport(llvmLibName, EntryPoint="LLVMAddFunctionInliningPass", CallingConvention=CallingConvention.Cdecl)]
        public static extern void AddFunctionInliningPass(LLVMPassManagerRef* PM);

        /* See llvm::createAlwaysInlinerPass function.*/
        [DllImport(llvmLibName, EntryPoint="LLVMAddAlwaysInlinerPass", CallingConvention=CallingConvention.Cdecl)]
        public static extern void AddAlwaysInlinerPass(LLVMPassManagerRef* PM);

        /* See llvm::createGlobalDCEPass function.*/
        [DllImport(llvmLibName, EntryPoint="LLVMAddGlobalDCEPass", CallingConvention=CallingConvention.Cdecl)]
        public static extern void AddGlobalDCEPass(LLVMPassManagerRef* PM);

        /* See llvm::createGlobalOptimizerPass function.*/
        [DllImport(llvmLibName, EntryPoint="LLVMAddGlobalOptimizerPass", CallingConvention=CallingConvention.Cdecl)]
        public static extern void AddGlobalOptimizerPass(LLVMPassManagerRef* PM);

        /* See llvm::createIPConstantPropagationPass function.*/
        [DllImport(llvmLibName, EntryPoint="LLVMAddIPConstantPropagationPass", CallingConvention=CallingConvention.Cdecl)]
        public static extern void AddIPConstantPropagationPass(LLVMPassManagerRef* PM);

        /* See llvm::createPruneEHPass function.*/
        [DllImport(llvmLibName, EntryPoint="LLVMAddPruneEHPass", CallingConvention=CallingConvention.Cdecl)]
        public static extern void AddPruneEHPass(LLVMPassManagerRef* PM);

        /* See llvm::createIPSCCPPass function.*/
        [DllImport(llvmLibName, EntryPoint="LLVMAddIPSCCPPass", CallingConvention=CallingConvention.Cdecl)]
        public static extern void AddIPSCCPPass(LLVMPassManagerRef* PM);

        /* See llvm::createInternalizePass function.*/
        [DllImport(llvmLibName, EntryPoint="LLVMAddInternalizePass", CallingConvention=CallingConvention.Cdecl)]
        public static extern void AddInternalizePass(LLVMPassManagerRef* LLVMPassManagerRef, uint AllButMain);

        /* See llvm::createStripDeadPrototypesPass function.*/
        [DllImport(llvmLibName, EntryPoint="LLVMAddStripDeadPrototypesPass", CallingConvention=CallingConvention.Cdecl)]
        public static extern void AddStripDeadPrototypesPass(LLVMPassManagerRef* PM);

        /* See llvm::createStripSymbolsPass function.*/
        [DllImport(llvmLibName, EntryPoint="LLVMAddStripSymbolsPass", CallingConvention=CallingConvention.Cdecl)]
        public static extern void AddStripSymbolsPass(LLVMPassManagerRef* PM);

        /* @defgroup LLVMCTransformsPassManagerBuilder Pass manager builder
         * @ingroup LLVMCTransforms
         *
         * @{
         */
        
        /** See llvm::PassManagerBuilder.*/
        [DllImport(llvmLibName, EntryPoint="LLVMPassManagerBuilderCreate", CallingConvention=CallingConvention.Cdecl)]
        public static extern LLVMPassManagerBuilderRef* PassManagerBuilderCreate();

        [DllImport(llvmLibName, EntryPoint="LLVMPassManagerBuilderDispose", CallingConvention=CallingConvention.Cdecl)]
        public static extern void PassManagerBuilderDispose(LLVMPassManagerBuilderRef* PMB);

        /* See llvm::PassManagerBuilder::OptLevel.*/
        [DllImport(llvmLibName, EntryPoint="LLVMPassManagerBuilderSetOptLevel", CallingConvention=CallingConvention.Cdecl)]
        public static extern void PassManagerBuilderSetOptLevel(LLVMPassManagerBuilderRef* PMB, uint OptLevel);

        /* See llvm::PassManagerBuilder::SizeLevel.*/
        [DllImport(llvmLibName, EntryPoint="LLVMPassManagerBuilderSetSizeLevel", CallingConvention=CallingConvention.Cdecl)]
        public static extern void PassManagerBuilderSetSizeLevel(LLVMPassManagerBuilderRef* PMB, uint SizeLevel);

        /* See llvm::PassManagerBuilder::DisableUnitAtATime.*/
        [DllImport(llvmLibName, EntryPoint="LLVMPassManagerBuilderSetDisableUnitAtATime", CallingConvention=CallingConvention.Cdecl)]
        public static extern void PassManagerBuilderSetDisableUnitAtATime(LLVMPassManagerBuilderRef* PMB, int Value);

        /* See llvm::PassManagerBuilder::DisableUnrollLoops.*/
        [DllImport(llvmLibName, EntryPoint="LLVMPassManagerBuilderSetDisableUnrollLoops", CallingConvention=CallingConvention.Cdecl)]
        public static extern void PassManagerBuilderSetDisableUnrollLoops(LLVMPassManagerBuilderRef* PMB, int Value);

        /* See llvm::PassManagerBuilder::DisableSimplifyLibCalls*/
        [DllImport(llvmLibName, EntryPoint="LLVMPassManagerBuilderSetDisableSimplifyLibCalls", CallingConvention=CallingConvention.Cdecl)]
        public static extern void PassManagerBuilderSetDisableSimplifyLibCalls(LLVMPassManagerBuilderRef* PMB, int Value);

        /* See llvm::PassManagerBuilder::Inliner.*/
        [DllImport(llvmLibName, EntryPoint="LLVMPassManagerBuilderUseInlinerWithThreshold", CallingConvention=CallingConvention.Cdecl)]
        public static extern void PassManagerBuilderUseInlinerWithThreshold(LLVMPassManagerBuilderRef* PMB, uint Threshold);

        /* See llvm::PassManagerBuilder::populateFunctionPassManager.*/
        [DllImport(llvmLibName, EntryPoint="LLVMPassManagerBuilderPopulateFunctionPassManager", CallingConvention=CallingConvention.Cdecl)]
        public static extern void PassManagerBuilderPopulateFunctionPassManager(LLVMPassManagerBuilderRef* PMB, LLVMPassManagerRef* PM);

        /* See llvm::PassManagerBuilder::populateModulePassManager.*/
        [DllImport(llvmLibName, EntryPoint="LLVMPassManagerBuilderPopulateModulePassManager", CallingConvention=CallingConvention.Cdecl)]
        public static extern void PassManagerBuilderPopulateModulePassManager(LLVMPassManagerBuilderRef* PMB, LLVMPassManagerRef* PM);

        /* See llvm::PassManagerBuilder::populateLTOPassManager.*/
        [DllImport(llvmLibName, EntryPoint="LLVMPassManagerBuilderPopulateLTOPassManager", CallingConvention=CallingConvention.Cdecl)]
        public static extern void PassManagerBuilderPopulateLTOPassManager(LLVMPassManagerBuilderRef* PMB, LLVMPassManagerRef* PM, bool Internalize, bool RunInliner);

        /* @defgroup LLVMCTransformsScalar Scalar transformations
         * @ingroup LLVMCTransforms
         *
         * @{
         */
        
        /** See llvm::createAggressiveDCEPass function.*/
        [DllImport(llvmLibName, EntryPoint="LLVMAddAggressiveDCEPass", CallingConvention=CallingConvention.Cdecl)]
        public static extern void AddAggressiveDCEPass(LLVMPassManagerRef* PM);

        /* See llvm::createCFGSimplificationPass function.*/
        [DllImport(llvmLibName, EntryPoint="LLVMAddCFGSimplificationPass", CallingConvention=CallingConvention.Cdecl)]
        public static extern void AddCFGSimplificationPass(LLVMPassManagerRef* PM);

        /* See llvm::createDeadStoreEliminationPass function.*/
        [DllImport(llvmLibName, EntryPoint="LLVMAddDeadStoreEliminationPass", CallingConvention=CallingConvention.Cdecl)]
        public static extern void AddDeadStoreEliminationPass(LLVMPassManagerRef* PM);

        /* See llvm::createGVNPass function.*/
        [DllImport(llvmLibName, EntryPoint="LLVMAddGVNPass", CallingConvention=CallingConvention.Cdecl)]
        public static extern void AddGVNPass(LLVMPassManagerRef* PM);

        /* See llvm::createIndVarSimplifyPass function.*/
        [DllImport(llvmLibName, EntryPoint="LLVMAddIndVarSimplifyPass", CallingConvention=CallingConvention.Cdecl)]
        public static extern void AddIndVarSimplifyPass(LLVMPassManagerRef* PM);

        /* See llvm::createInstructionCombiningPass function.*/
        [DllImport(llvmLibName, EntryPoint="LLVMAddInstructionCombiningPass", CallingConvention=CallingConvention.Cdecl)]
        public static extern void AddInstructionCombiningPass(LLVMPassManagerRef* PM);

        /* See llvm::createJumpThreadingPass function.*/
        [DllImport(llvmLibName, EntryPoint="LLVMAddJumpThreadingPass", CallingConvention=CallingConvention.Cdecl)]
        public static extern void AddJumpThreadingPass(LLVMPassManagerRef* PM);

        /* See llvm::createLICMPass function.*/
        [DllImport(llvmLibName, EntryPoint="LLVMAddLICMPass", CallingConvention=CallingConvention.Cdecl)]
        public static extern void AddLICMPass(LLVMPassManagerRef* PM);

        /* See llvm::createLoopDeletionPass function.*/
        [DllImport(llvmLibName, EntryPoint="LLVMAddLoopDeletionPass", CallingConvention=CallingConvention.Cdecl)]
        public static extern void AddLoopDeletionPass(LLVMPassManagerRef* PM);

        /* See llvm::createLoopIdiomPass function*/
        [DllImport(llvmLibName, EntryPoint="LLVMAddLoopIdiomPass", CallingConvention=CallingConvention.Cdecl)]
        public static extern void AddLoopIdiomPass(LLVMPassManagerRef* PM);

        /* See llvm::createLoopRotatePass function.*/
        [DllImport(llvmLibName, EntryPoint="LLVMAddLoopRotatePass", CallingConvention=CallingConvention.Cdecl)]
        public static extern void AddLoopRotatePass(LLVMPassManagerRef* PM);

        /* See llvm::createLoopUnrollPass function.*/
        [DllImport(llvmLibName, EntryPoint="LLVMAddLoopUnrollPass", CallingConvention=CallingConvention.Cdecl)]
        public static extern void AddLoopUnrollPass(LLVMPassManagerRef* PM);

        /* See llvm::createLoopUnswitchPass function.*/
        [DllImport(llvmLibName, EntryPoint="LLVMAddLoopUnswitchPass", CallingConvention=CallingConvention.Cdecl)]
        public static extern void AddLoopUnswitchPass(LLVMPassManagerRef* PM);

        /* See llvm::createMemCpyOptPass function.*/
        [DllImport(llvmLibName, EntryPoint="LLVMAddMemCpyOptPass", CallingConvention=CallingConvention.Cdecl)]
        public static extern void AddMemCpyOptPass(LLVMPassManagerRef* PM);

        /* See llvm::createPromoteMemoryToRegisterPass function.*/
        [DllImport(llvmLibName, EntryPoint="LLVMAddPromoteMemoryToRegisterPass", CallingConvention=CallingConvention.Cdecl)]
        public static extern void AddPromoteMemoryToRegisterPass(LLVMPassManagerRef* PM);

        /* See llvm::createReassociatePass function.*/
        [DllImport(llvmLibName, EntryPoint="LLVMAddReassociatePass", CallingConvention=CallingConvention.Cdecl)]
        public static extern void AddReassociatePass(LLVMPassManagerRef* PM);

        /* See llvm::createSCCPPass function.*/
        [DllImport(llvmLibName, EntryPoint="LLVMAddSCCPPass", CallingConvention=CallingConvention.Cdecl)]
        public static extern void AddSCCPPass(LLVMPassManagerRef* PM);

        /* See llvm::createScalarReplAggregatesPass function.*/
        [DllImport(llvmLibName, EntryPoint="LLVMAddScalarReplAggregatesPass", CallingConvention=CallingConvention.Cdecl)]
        public static extern void AddScalarReplAggregatesPass(LLVMPassManagerRef* PM);

        /* See llvm::createScalarReplAggregatesPass function.*/
        [DllImport(llvmLibName, EntryPoint="LLVMAddScalarReplAggregatesPassSSA", CallingConvention=CallingConvention.Cdecl)]
        public static extern void AddScalarReplAggregatesPassSSA(LLVMPassManagerRef* PM);

        /* See llvm::createScalarReplAggregatesPass function.*/
        [DllImport(llvmLibName, EntryPoint="LLVMAddScalarReplAggregatesPassWithThreshold", CallingConvention=CallingConvention.Cdecl)]
        public static extern void AddScalarReplAggregatesPassWithThreshold(LLVMPassManagerRef* PM, int Threshold);

        /* See llvm::createSimplifyLibCallsPass function.*/
        [DllImport(llvmLibName, EntryPoint="LLVMAddSimplifyLibCallsPass", CallingConvention=CallingConvention.Cdecl)]
        public static extern void AddSimplifyLibCallsPass(LLVMPassManagerRef* PM);

        /* See llvm::createTailCallEliminationPass function.*/
        [DllImport(llvmLibName, EntryPoint="LLVMAddTailCallEliminationPass", CallingConvention=CallingConvention.Cdecl)]
        public static extern void AddTailCallEliminationPass(LLVMPassManagerRef* PM);

        /* See llvm::createConstantPropagationPass function.*/
        [DllImport(llvmLibName, EntryPoint="LLVMAddConstantPropagationPass", CallingConvention=CallingConvention.Cdecl)]
        public static extern void AddConstantPropagationPass(LLVMPassManagerRef* PM);

        /* See llvm::demotePromoteMemoryToRegisterPass function.*/
        [DllImport(llvmLibName, EntryPoint="LLVMAddDemoteMemoryToRegisterPass", CallingConvention=CallingConvention.Cdecl)]
        public static extern void AddDemoteMemoryToRegisterPass(LLVMPassManagerRef* PM);

        /* See llvm::createVerifierPass function.*/
        [DllImport(llvmLibName, EntryPoint="LLVMAddVerifierPass", CallingConvention=CallingConvention.Cdecl)]
        public static extern void AddVerifierPass(LLVMPassManagerRef* PM);

        /* See llvm::createCorrelatedValuePropagationPass function*/
        [DllImport(llvmLibName, EntryPoint="LLVMAddCorrelatedValuePropagationPass", CallingConvention=CallingConvention.Cdecl)]
        public static extern void AddCorrelatedValuePropagationPass(LLVMPassManagerRef* PM);

        /* See llvm::createEarlyCSEPass function*/
        [DllImport(llvmLibName, EntryPoint="LLVMAddEarlyCSEPass", CallingConvention=CallingConvention.Cdecl)]
        public static extern void AddEarlyCSEPass(LLVMPassManagerRef* PM);

        /* See llvm::createLowerExpectIntrinsicPass function*/
        [DllImport(llvmLibName, EntryPoint="LLVMAddLowerExpectIntrinsicPass", CallingConvention=CallingConvention.Cdecl)]
        public static extern void AddLowerExpectIntrinsicPass(LLVMPassManagerRef* PM);

        /* See llvm::createTypeBasedAliasAnalysisPass function*/
        [DllImport(llvmLibName, EntryPoint="LLVMAddTypeBasedAliasAnalysisPass", CallingConvention=CallingConvention.Cdecl)]
        public static extern void AddTypeBasedAliasAnalysisPass(LLVMPassManagerRef* PM);

        /* See llvm::createBasicAliasAnalysisPass function*/
        [DllImport(llvmLibName, EntryPoint="LLVMAddBasicAliasAnalysisPass", CallingConvention=CallingConvention.Cdecl)]
        public static extern void AddBasicAliasAnalysisPass(LLVMPassManagerRef* PM);

        /* @defgroup LLVMCTransformsVectorize Vectorization transformations
         * @ingroup LLVMCTransforms
         *
         * @{
         */
        
        /** See llvm::createBBVectorizePass function.*/
        [DllImport(llvmLibName, EntryPoint="LLVMAddBBVectorizePass", CallingConvention=CallingConvention.Cdecl)]
        public static extern void AddBBVectorizePass(LLVMPassManagerRef* PM);

    }
}