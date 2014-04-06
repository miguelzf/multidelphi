LLVM.NET
========

This library includes all C bindings from LLVM 3.1 converted to pinvoke imports.  The library also includes wrappers around some of the core functionality to make it easier to use.  The wrappers are not necessary to use the library; all functionality is available in the LLVM.Native class.  However, the wrappers do help with reducing common code and reducing the need to use unsafe blocks.  

## Building LLVM for use with LLVM.NET

LLVM needs to be built as a shared library so that it can be dynamically loaded.  On Windows, this requires the use of Mingw as Visual Studio can't build LLVM as a shared library (yet).  With Mingw, you can do something like `./configure --enable-shared --enable-jit && make && make install`.

This has only been tested on Windows so far, but I've tried to make it easy to use with Mono.

c2cs
====

A C++ command line application for converting llvm-c header files to C#.  This tool uses clang's front end to parse the llvm-c headers and then uses the resulting AST to convert enums, typedefs, and function declarations to C#.  This uses the latest version of clang so comments can be copied to the output as well. I believe this could be easily modified to convert any header files, but right now it's specific to LLVM.  

This tool uses hardcoded paths for the output file and clang include directories.  It should be simple to change these to command line arguments.  The paths are all defined at the top of c2cs.cpp.

Kaleidoscope
============

This is a port of the LLVM Kaleidoscope tutorials to C#.