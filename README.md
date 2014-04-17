MultiPascal
===========

MultiPascal is a Delphi<sup>TM</sup> (ObjectPascal) multi-target compiler using the LLVM framework.  
This project is an attempt at a working multi-target Delphi7 compiler, capable of compiling to Flash (ActionScript 3).  

Its original goal was to target the [ActionScript Virtual Machine (AVM)][16], in order to port some large Delphi projects such as the [TERRA graphical engine][3] and the [Minimon 3D game][4].  Other future targets include x86, probably Javascript/[asm.js][17]+[WebGL][23] and probably [Haxe][18].

MultiPascal was started in February 2014 and it's still in its early stages. As of Abril 2014, the Delphi front-end is mostly done, with the C++ back-end supporting most of Delphi features and an initial prototype of the LLVM IR back-end.

Stages of the project:

* a Delphi front-end for LLVM

	Initially we thought of using the [Dgrok][19] recursive-descent (LL(k)) parser. However, looking at the work required to convert the simple lexical-oriented Parse Tree that Dgrok produces to a robust complete AST, we decided to create our own shift-reduce parser.     
	So MultiPascal ended up with a complete new shift-reduce (LALR) parser created with the C# ports of the Jay Yacc-based parser generator ([CSJay][10]) and the JFlex lexer generator ([CSFlex][12]).     
	Currently MultiPascal is built on top of [LLVM 3.2][6], the same version used in [FlaCC/Crossbridge][15].     


* a C++ back-end
	
* AS3 (Flash) back-end
	
* ABC (AS3 bytecode) back-end, based on [FlaCC/Crossbridge][15]  


      


**References:**


* Delphi   

	[Delphi Embarcadero wiki][1]    
	[Delphi Docs][2]    
	[TERRA graphical engine][3]      
	[Minimon 3D game][4]     

[1]:http://docwiki.embarcadero.com/RADStudio/XE6/en/Delphi_Reference    
[2]:http://www.delphibasics.co.uk/        
[3]:http://www.pascalgameengine.com    
[4]:http://minimon3d.com    

* LLVM

	[LLVM Documentation][5]   
	[LLVM 3.2][6]    
	[LLVM C API][20]    
	[LLVM Programmer's manual][21]    
	[LLVM IR Language reference][22]    
	[LLVM.NET wrapper][7]     

[5]:http://llvm.org/docs    
[6]:http://llvm.org/releases/3.2/docs/ReleaseNotes.html   
[7]:https://github.com/miguelzf/LLVM.NET    
[20]:http://llvm.org/docs/doxygen/html/group__LLVMC.html
[21]:http://llvm.org/docs/ProgrammersManual.html
[22]:http://llvm.org/docs/LangRef.html


* Parser/Lexer
	
	[Yacc/Flex docs][8]   
	[Jay][9]   and [CSJay][10]      
	[JFlex][11] and [CSFLEX][12]    
	[Dgrok][19]    

[8]:http://dinosaur.compilertools.net   
[9]:http://www.cs.rit.edu/~ats/projects/lp/doc/jay/package-summary.html    
[10]:https://code.google.com/p/jay    
[11]:http://jflex.de    
[12]:http://sourceforge.net/projects/csflex/      
[19]:http://dgrok.excastle.com/    

* Flash

	[AS3 documentation][13]   
	[Crossbridge][14]    
	[FlaCC/Crossbridge][15]      
	[AVM overview][16]
	[Tamarin JS/AS engine][24]

[13]:http://www.adobe.com/devnet/actionscript/documentation.html   
[14]:http://adobe-flash.github.io/crossbridge   
[15]:https://github.com/adobe-flash/crossbridge   
[16]:http://www.adobe.com/content/dam/Adobe/en/devnet/actionscript/articles/avm2overview.pdf
[24]:https://developer.mozilla.org/en-US/docs/Archive/Mozilla/Tamarin

* Misc
 
	[asm.js][17]    
	[WebGL][23]    
	[Haxe][18]    

[18]:http://haxe.org/
[17]:http://asmjs.org/
[23]:http://www.khronos.org/webgl/wiki/Main_Page
