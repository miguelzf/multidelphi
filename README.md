crosspascal
===========

Delphi (ObjectPascal) to Actionscript3 (Flash) compiler, using the LLVM framework

Will include:

	- a Delphi front-end for LLVM
		Initially built on top of the Dgrok recursive-descent parser, currently it
		uses a complete new shift-reduce parser created with C#'s port of the Jay Yacc-based parser generator

	- Cpp back-end
	
	- AS3 (Flash) back-end
	
	- ABC (AS3 bytecode) back-end, based on FlaCC/Crossbridge

	
