// c2cs.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"
#define _SCL_SECURE_NO_WARNINGS

#define OUTPUT_CS_FILE "F:\\Projects\\LLVM#\\LLVM.NET\\NativeMethods.cs"
#define LLVM_INCLUDE_DIR "f:\\llvm-install\\include"
#define MINGW_INCLUDE_DIR "F:\\MinGW\\include"
#define GCC_INCLUDE_DIR "F:\\MinGW\\lib\\gcc\\mingw32\\4.6.2\\include"

#include <iostream>
#include <sstream>
#include <vector>

#pragma warning( disable : 4146 )

#include "llvm/Support/raw_ostream.h"
#include "llvm/Support/Host.h"

#include "clang/Frontend/DiagnosticOptions.h"
#include "clang/Frontend/TextDiagnosticPrinter.h"

#include "clang/Basic/LangOptions.h"
#include "clang/Basic/FileSystemOptions.h"

#include "clang/Basic/SourceManager.h"
#include "clang/Lex/HeaderSearch.h"
#include "clang/Basic/FileManager.h"

#include "clang/Frontend/HeaderSearchOptions.h"
#include "clang/Frontend/Utils.h"

#include "clang/Basic/TargetOptions.h"
#include "clang/Basic/TargetInfo.h"

#include "clang/Lex/Preprocessor.h"
#include "clang/Frontend/PreprocessorOptions.h"
#include "clang/Frontend/FrontendOptions.h"

#include "clang/Basic/IdentifierTable.h"
#include "clang/Basic/Builtins.h"

#include "clang/AST/ASTContext.h"
#include "clang/AST/ASTConsumer.h"

#include "clang/Parse/ParseAST.h"
#include "clang/Parse/Parser.h"
#include "clang/Frontend/CompilerInstance.h"
#include "clang/AST/Comment.h"

class MyASTConsumer : public clang::ASTConsumer
{
public:
    MyASTConsumer(llvm::raw_ostream& output) : clang::ASTConsumer(), outFile(output)
    {
      outFile << "using System;\n";
      outFile << "using System.Runtime.InteropServices;\n";
      outFile << "using System.Text;\n\n";
      outFile << "namespace LLVM\n{";
    }

    virtual ~MyASTConsumer() { }
     
    virtual void Initialize(clang::ASTContext &Context) 
    {
      m_context = &Context;
    }

    void Set(const clang::SourceManager* sourceMgr, const clang::LangOptions* langOptions)
    {
      m_sourceMgr = sourceMgr;
      m_langOptions = langOptions;
    }

    virtual bool HandleTopLevelDecl( clang::DeclGroupRef d)
    {
        clang::DeclGroupRef::iterator it;
        for( it = d.begin(); it != d.end(); it++)
        {
          if((*it)->getKind() == clang::Decl::Kind::Enum)
          {
            clang::EnumDecl *ed = llvm::dyn_cast<clang::EnumDecl>(*it);
            clang::DeclarationName declName = ed->getDeclName();
            if(!declName)
              declName = ed->getTypedefNameForAnonDecl()->getDeclName();

            if(m_enumsSeen.find(llvm::StringRef(declName.getAsString())) != m_enumsSeen.end())
              continue;

            outFile << '\n';
            OutputComment(ed, outFile, 4);
            outFile.indent(4) << "public enum " << declName.getAsString() << "\n";
            outFile.indent(4) << "{\n";
        
            for(auto j = ed->enumerator_begin(); j != ed->enumerator_end(); ++j)
            {
              clang::EnumConstantDecl* ecd = *j;
              OutputComment(ecd, outFile, 8);

              llvm::StringRef name = ecd->getIdentifier()->getName();
              if(name.startswith("LLVM_"))
                name = name.substr(5);
              else if(name.startswith("LLVM"))
                name = name.substr(4);

              if(ecd->getInitExpr())
              {
                clang::SourceRange range = ecd->getInitExpr()-> getSourceRange();
                outFile.indent(8) << name.str() << " = " << GetTextForRange(range) << ",\n";
              }
              else
              {
                outFile.indent(8) << name.str() << ",\n";
              }
            }

            outFile.indent(4) << "}\n";

            m_enumsSeen.GetOrCreateValue(llvm::StringRef(declName.getAsString()), true);
          }
          else if((*it)->getKind() == clang::Decl::Kind::Function)
          {
            clang::SourceLocation loc = m_sourceMgr->getFileLoc((*it)->getLocation());

            if(!m_sourceMgr->isFromMainFile(loc) && !IsDefFile(m_sourceMgr->getFileLoc(loc)))
              continue;

            clang::FunctionDecl *fd = llvm::dyn_cast<clang::FunctionDecl>(*it);
            if(m_funcSeen.find(fd->getName()) != m_funcSeen.end())
              continue;

            std::string output;
            llvm::raw_string_ostream sstream(output);
            llvm::StringRef name = fd->getName();

            if(name.startswith("LLVM"))
              name = name.substr(4);

            OutputComment(fd, sstream, 8);
            sstream.indent(8) << "[DllImport(\"llvm-3.1.dll\", EntryPoint=\"" << fd->getName().str() << "\", CallingConvention=CallingConvention.Cdecl)]\n";
            sstream.indent(8) << "public static extern " << ConvertType(fd->getResultType(), true, false) << " " << name.str() << "(";
        
            std::string comma = "";
            for(auto j = fd->param_begin(); j != fd->param_end(); ++j)
            {
              std::string name = (*j)->getNameAsString();
              if(name.size() == 0)
                name = (*j)->getType().getAsString();

              sstream << comma << ConvertType((*j)->getType(), false, (*j)->getName().startswith("Out")) << " " << name;
              comma = ", ";
            }

            sstream << ");";
            sstream.flush();

            m_funcSeen.GetOrCreateValue(llvm::StringRef(fd->getName().str()), true);
            m_funcs.push_back(output);
          }
          else if((*it)->getKind() == clang::Decl::Kind::Typedef)
          {
            clang::SourceLocation loc = m_sourceMgr->getFileLoc((*it)->getLocation());
 
            if(!m_sourceMgr->isFromMainFile(loc) && !IsDefFile(m_sourceMgr->getFileLoc(loc)))
              continue;

            clang::TypedefDecl *decl = llvm::dyn_cast<clang::TypedefDecl>(*it);
            clang::QualType type = decl->getUnderlyingType();
            if(!type->isPointerType() || !type->getPointeeType()->isStructureOrClassType())
              continue;

            std::string output;
            llvm::raw_string_ostream sstream(output);
          
            sstream.indent(4) << "public unsafe struct " << decl->getName().str() << " {};";
            sstream.flush();

            m_types.GetOrCreateValue(llvm::StringRef(decl->getName().str()), output);
          }
        }
        return true;
    }

    void OutputComment(clang::Decl* decl, llvm::raw_ostream& stream, int indent)
    {
      clang::comments::FullComment* comment = m_context->getCommentForDecl(decl);
      if(!comment)
        return;

      std::stringstream ss(GetTextForRange(comment->getSourceRange()));
      std::string line;

      stream.indent(indent) << "/*";
      bool space = false;

      while(std::getline(ss, line))
      {
        if(space)
        {
          stream << "\n";
          stream.indent(indent);
        }

        stream << line;
        space = true;
      }

      stream << "*/\n";
    }

    bool IsDefFile(clang::SourceLocation loc)
    {
      clang::FileID fileId = m_sourceMgr->getFileID(loc);
      const clang::FileEntry* pEntry = m_sourceMgr->getFileEntryForID(fileId);
      return EndsWith(pEntry->getName(), ".def");
    }

    bool EndsWith(const char* str, const char* suffix)
    {
      if( str == NULL || suffix == NULL )
        return false;

      size_t str_len = strlen(str);
      size_t suffix_len = strlen(suffix);

      if(suffix_len > str_len)
        return 0;

      return 0 == strncmp( str + str_len - suffix_len, suffix, suffix_len );    
    }

    void Generate()
    {
      outFile << '\n';

      for(auto i = m_types.begin(); i != m_types.end(); ++i)
      {
        outFile << i->getValue() << "\n";
      }

      outFile << '\n';
      outFile.indent(4) << "public static unsafe class Native\n";
      outFile.indent(4) << "{\n";

      for(auto i = m_funcs.begin(); i != m_funcs.end(); ++i)
      {
        outFile << *i << "\n\n";
      }

      outFile.indent(4) << "}\n}";
    }

    std::string ConvertType(clang::QualType type, bool isReturn, bool outParam)
    {
      if(type->isVoidType())
      {
        return "void";
      }
      
      if(type->isBooleanType())
      {
        return "bool";
      }

      if(type->isIntegralOrEnumerationType())
      {
        if(type->isEnumeralType())
        {
          const clang::EnumType* et = type->getAs<clang::EnumType>();
          clang::DeclarationName declName = et->getDecl()->getDeclName();
          if(!declName)
            declName = et->getDecl()->getTypedefNameForAnonDecl()->getDeclName();

          return declName.getAsString();
        }

        if(type->isUnsignedIntegerType())
        {
          switch(m_context->getTypeSize(type))
          {
          case 8: return "byte";
          case 16: return "ushort";
          case 32: return "uint";
          case 64: return "ulong";
          }
        }
        else
        {
          switch(m_context->getTypeSize(type))
          {
          case 8: return "sbyte";
          case 16: return "short";
          case 32: return "int";
          case 64: return "long";
          }
        }
      }

      if(type->isHalfType())
      {
        return "float";
      }

      if(type->isFloatingType())
      {
        return "double";
      }

      if(type->isPointerType())
      {
        if(!isReturn && type->getPointeeType()->isAnyCharacterType())
        {
          if(type->getPointeeType()->isWideCharType())
            return "[In][MarshalAs(UnmanagedType.LPWStr)] string";
          else
            return "[In][MarshalAs(UnmanagedType.LPStr)] string";
        }
        else if(type->getPointeeType()->isPointerType() && type->getPointeeType()->getPointeeType()->isAnyCharacterType())
        {
            return "[MarshalAs(UnmanagedType.LPStr)] ref StringBuilder";
        }
        else if(type->getPointeeType()->isStructureOrClassType())
        {
          return type.getAsString() + "*";
        }
        else if(type->getPointeeType()->isPointerType() && type->getPointeeType()->getPointeeType()->isStructureOrClassType())
        {
          return outParam ? "ref " + type.getAsString() : "System.IntPtr[]";
        }
      }

      return "System.IntPtr";
    }

    std::string GetTextForRange(clang::SourceRange range)
    {
      unsigned StartOff, EndOff;
      StartOff = GetLocationOffset(range.getBegin());
      EndOff   = GetLocationOffset(range.getEnd());

      const char *Ptr = m_sourceMgr->getCharacterData(range.getBegin());
      // Adjust the end offset to the end of the last token, instead of being the
      // start of the last token.
      EndOff += clang::Lexer::MeasureTokenLength(range.getEnd(), *m_sourceMgr, *m_langOptions);
      return std::string(Ptr, Ptr+EndOff-StartOff);    
    }

  unsigned GetLocationOffset(clang::SourceLocation Loc) const 
  {
    std::pair<clang::FileID,unsigned> V = m_sourceMgr->getDecomposedLoc(Loc);
    return V.second;
  }

private:
  llvm::raw_ostream& outFile;
  llvm::StringMap<bool> m_enumsSeen;
  llvm::StringMap<bool> m_funcSeen;
  std::vector<std::string> m_funcs;
  llvm::StringMap<std::string> m_types;
  const clang::SourceManager* m_sourceMgr;
  const clang::LangOptions* m_langOptions;
  clang::ASTContext* m_context;
};

clang::MacroInfo* RegisterMacro(clang::Preprocessor &PP, const char *Name)
{
  // Get the identifier.
  clang::IdentifierInfo *Id = PP.getIdentifierInfo(Name);

  // Mark it as being a macro that is builtin.
  clang::MacroInfo *MI = PP.AllocateMacroInfo(clang::SourceLocation());
  MI->setIsBuiltinMacro();
  PP.setMacroInfo(Id, MI);

  return MI;
}

void ParseFile(llvm::StringRef filename, MyASTConsumer& astConsumer)
{
    clang::DiagnosticOptions diagnosticOptions;
    clang::TextDiagnosticPrinter *pTextDiagnosticPrinter =
        new clang::TextDiagnosticPrinter(
            llvm::outs(),
            diagnosticOptions);
    llvm::IntrusiveRefCntPtr<clang::DiagnosticIDs> pDiagIDs;
    clang::DiagnosticsEngine *pDiagnosticsEngine =
        new clang::DiagnosticsEngine(pDiagIDs, pTextDiagnosticPrinter);

    clang::LangOptions languageOptions;
    //languageOptions.CPlusPlus = true;
    languageOptions.Bool = true;

    clang::FileSystemOptions fileSystemOptions;
    clang::FileManager fileManager(fileSystemOptions);

    clang::SourceManager sourceManager(
        *pDiagnosticsEngine,
        fileManager);

    clang::HeaderSearchOptions headerSearchOptions;

    clang::TargetOptions targetOptions;
    targetOptions.Triple = llvm::sys::getDefaultTargetTriple();
    clang::TargetInfo *pTargetInfo = 
        clang::TargetInfo::CreateTargetInfo(
            *pDiagnosticsEngine,
            targetOptions);

    clang::HeaderSearch headerSearch(fileManager, 
                                     *pDiagnosticsEngine,
                                     languageOptions,
                                     pTargetInfo);
    headerSearchOptions.AddPath(LLVM_INCLUDE_DIR,
            clang::frontend::Angled,
            false,
            false,
            false);
    headerSearchOptions.AddPath(MINGW_INCLUDE_DIR,
            clang::frontend::Angled,
            false,
            false,
            false);
    headerSearchOptions.AddPath(GCC_INCLUDE_DIR,
            clang::frontend::Angled,
            false,
            false,
            false);

    clang::CompilerInstance compInst;

    clang::Preprocessor preprocessor(
        *pDiagnosticsEngine,
        languageOptions,
        pTargetInfo,
        sourceManager,
        headerSearch,
        compInst);


    clang::PreprocessorOptions preprocessorOptions;
    // disable predefined Macros so that you only see the tokens from your 
    // source file. Note, this has some nasty side-effects like also undefning
    // your archictecture and things like that.
    //preprocessorOptions.UsePredefines = false;
    RegisterMacro(preprocessor, "__STDC_CONSTANT_MACROS");
    RegisterMacro(preprocessor, "__STDC_LIMIT_MACROS");

    clang::FrontendOptions frontendOptions;
    clang::InitializePreprocessor(
        preprocessor,
        preprocessorOptions,
        headerSearchOptions,
        frontendOptions);

    const clang::FileEntry *pFile = fileManager.getFile(filename);
    sourceManager.createMainFileID(pFile);
    const clang::TargetInfo &targetInfo = *pTargetInfo;

    clang::IdentifierTable identifierTable(languageOptions);
    clang::SelectorTable selectorTable;

    clang::Builtin::Context builtinContext;
    builtinContext.InitializeTarget(targetInfo);
    clang::ASTContext astContext(
        languageOptions,
        sourceManager,
        pTargetInfo,
        identifierTable,
        selectorTable,
        builtinContext,
        0 /* size_reserve*/);

    //clang::Sema sema(
    //    preprocessor,
    //    astContext,
    //    astConsumer);

    astConsumer.Set(&sourceManager, &languageOptions);

    pTextDiagnosticPrinter->BeginSourceFile(languageOptions, &preprocessor);
    clang::ParseAST(preprocessor, &astConsumer, astContext); 
    pTextDiagnosticPrinter->EndSourceFile();

}

int _tmain(int argc, _TCHAR* argv[])
{  
  llvm::raw_fd_ostream output(OUTPUT_CS_FILE, std::string());
  MyASTConsumer astConsumer(output);

  ParseFile("Analysis.h", astConsumer);
  ParseFile("BitReader.h", astConsumer);
  ParseFile("BitWriter.h", astConsumer);
  ParseFile("Core.h", astConsumer);
  ParseFile("ExecutionEngine.h",  astConsumer);
  ParseFile("Initialization.h",  astConsumer);
  ParseFile("LinkTimeOptimizer.h",  astConsumer);
  ParseFile("Object.h", astConsumer);
  ParseFile("target.h", astConsumer);
  ParseFile("TargetMachine.h", astConsumer);
  ParseFile("Transforms/IPO.h", astConsumer);
  ParseFile("Transforms/PassManagerBuilder.h", astConsumer);
  ParseFile("Transforms/Scalar.h", astConsumer);
  ParseFile("Transforms/Vectorize.h",  astConsumer);

  astConsumer.Generate();
  output.flush();

  return 0;
}

