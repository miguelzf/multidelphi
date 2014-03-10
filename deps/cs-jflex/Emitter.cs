/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * C# Flex 1.4                                                             *
 * Copyright (C) 2004-2005  Jonathan Gilbert <logic@deltaq.org>            *
 * Derived from:                                                           *
 *                                                                         *
 *   JFlex 1.4                                                             *
 *   Copyright (C) 1998-2004  Gerwin Klein <lsf@jflex.de>                  *
 *   All rights reserved.                                                  *
 *                                                                         *
 * This program is free software; you can redistribute it and/or modify    *
 * it under the terms of the GNU General Public License. See the file      *
 * COPYRIGHT for more information.                                         *
 *                                                                         *
 * This program is distributed in the hope that it will be useful,         *
 * but WITHOUT ANY WARRANTY; without even the implied warranty of          *
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the           *
 * GNU General Public License for more details.                            *
 *                                                                         *
 * You should have received a copy of the GNU General Public License along *
 * with this program; if not, write to the Free Software Foundation, Inc., *
 * 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA                 *
 *                                                                         *
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

using System;
using System.Collections;
using System.IO;
using System.Text;

namespace CSFlex
{

/**
 * This class manages the actual code generation, putting
 * the scanner together, filling in skeleton sections etc.
 *
 * Table compression, String packing etc. is also done here.
 *
 * @author Gerwin Klein
 * @version JFlex 1.4, $Revision: 2.22 $, $Date: 2004/04/12 10:07:48 $
 * @author Jonathan Gilbert
 * @version CSFlex 1.4
 */
sealed public class Emitter {
    
  // bit masks for state attributes
  private const int FINAL = 1;
  private const int PUSHBACK = 2;
  private const int LOOKEND = 4;
  private const int NOLOOK = 8;

  private static readonly String date = DateTime.Now.ToShortDateString();

  private File inputFile;

  private TextWriter @out;
  private Skeleton skel;
  private LexScan scanner;
  private LexParse parser;
  private DFA dfa;

  // for switch statement:
  // table[i][j] is the set of input characters that leads from state i to state j
  private CharSet[][] table;

  private bool[] isTransition;
  
  // noTarget[i] is the set of input characters that have no target state in state i
  private CharSet[] noTarget;
      
  // for row killing:
  private int numRows;
  private int [] rowMap;
  private bool [] rowKilled;
  
  // for col killing:
  private int numCols;
  private int [] colMap;
  private bool [] colKilled;
  

  /** maps actions to their switch label */
  private Hashtable actionTable = new PrettyHashtable();

  private CharClassInterval [] intervalls;

  private String visibility = "public";

  public Emitter(File inputFile, LexParse parser, DFA dfa) {

    String name;

	if (Options.debug)
		parser.scanner.debugOption = true;

    if (Options.emit_csharp)
      name = parser.scanner.className + ".cs";
    else
      name = parser.scanner.className + ".java";

    File outputFile = normalize(name, inputFile);

    Out.println("Writing code to \""+outputFile+"\"");
    
    this.@out = new StreamWriter(outputFile);
    this.parser = parser;
    this.scanner = parser.scanner;
    this.visibility = scanner.visibility;
    this.inputFile = inputFile;
    this.dfa = dfa;
    this.skel = new Skeleton(@out);
  }


  /**
   * Constructs a file in Options.getDir() or in the same directory as
   * another file. Makes a backup if the file already exists.
   *
   * @param name  the name (without path) of the file
   * @param path  the path where to construct the file
   * @param input fallback location if path = <tt>null</tt>
   *              (expected to be a file in the directory to write to)   
   */
  public static File normalize(String name, File input) {
    File outputFile;

    if ( Options.getDir() == null ) 
      if ( input == null || input.getParent() == null )
        outputFile = new File(name);
      else
        outputFile = new File(input.getParent(), name);
    else 
      outputFile = new File(Options.getDir(), name);
        
    if ( outputFile.exists() && !Options.no_backup ) {      
      File backup = new File( outputFile.ToString()+"~" );
      
      if ( backup.exists() ) backup.delete();
      
      if ( outputFile.renameTo( backup ) )
        Out.println("Old file \""+outputFile+"\" saved as \""+backup+"\"");
      else
        Out.println("Couldn't save old file \""+outputFile+"\", overwriting!");
    }

    return outputFile;
  }
  
  private void println() {
    @out.WriteLine();
  }

  private void println(String line) {
    @out.WriteLine(line);
  }

  private void println(int i) {
    @out.WriteLine(i);
  }

  private void print(String line) {
    @out.Write(line);
  }

  private void print(int i) {
    @out.Write(i);
  }

  private void print(int i, int tab) {
    int exp;

    if (i < 0) 
      exp = 1;
    else
      exp = 10;

    while (tab-- > 1) {
      if (Math.Abs(i) < exp) print(" ");
      exp*= 10;
    }

    print(i);
  }

  private void emitScanError() {
    print("  private void zzScanError(int errorCode)");

    if (!Options.emit_csharp)
    {
      if (scanner.scanErrorException != null) 
        print(" throws "+scanner.scanErrorException);
    }

    println(" {");

    skel.emitNext();

    if (scanner.scanErrorException == null)
    {
      if (Options.emit_csharp)
        println("    throw new Exception(message);");
      else
        println("    throw new Error(message);");
    }
    else
      println("    throw new "+scanner.scanErrorException+"(message);");    

    skel.emitNext();

    print("  "+visibility+" void yypushback(int number) ");     
    
    if (scanner.scanErrorException == null)
      println(" {");
    else
    {
      if (Options.emit_csharp)
        println(" {");
      else
        println(" throws "+scanner.scanErrorException+" {");
    }
  }

  private void emitMain() {
    if ( !(scanner.standalone || scanner.debugOption || scanner.cupDebug) ) return;

    if ( scanner.cupDebug ) {
      println("  /**");
      println("   * Converts an int token code into the name of the");
      println("   * token by reflection on the cup symbol class/interface "+scanner.cupSymbol);
      println("   *");
      println("   * This code was contributed by Karl Meissner <meissnersd@yahoo.com>"); 
      println("   */");
      if (Options.emit_csharp)
      {
        println("  private String getTokenName(int token) {");
        println("    try {");
        println("      System.Reflection.FieldInfo[] classFields = typeof(" + scanner.cupSymbol + ").GetFields();");
        println("      for (int i = 0; i < classFields.Length; i++) {");
        println("        if (((int)classFields[i].GetValue(null)) == token) {");
        println("          return classFields[i].Name;");
        println("        }");
        println("      }");
        println("    } catch (Exception e) {");
        println("      Out.error(e.ToString());");
        println("    }");
        println("");
        println("    return \"UNKNOWN TOKEN\";");
        println("  }");
      }
      else
      {
        println("  private String getTokenName(int token) {");
        println("    try {");
        println("      java.lang.reflect.Field [] classFields = " + scanner.cupSymbol + ".class.getFields();");
        println("      for (int i = 0; i < classFields.length; i++) {");
        println("        if (classFields[i].getInt(null) == token) {");
        println("          return classFields[i].getName();");
        println("        }");
        println("      }");
        println("    } catch (Exception e) {");
        println("      e.printStackTrace(System.err);");
        println("    }");
        println("");
        println("    return \"UNKNOWN TOKEN\";");
        println("  }");
      }
      println("");
      println("  /**");
      println("   * Same as "+scanner.functionName+" but also prints the token to standard out");
      println("   * for debugging.");
      println("   *");
      println("   * This code was contributed by Karl Meissner <meissnersd@yahoo.com>"); 
      println("   */");

      print("  "+visibility+" ");
      if ( scanner.tokenType == null ) {
        if ( scanner.isInteger )
          print( "int" );
        else 
          if ( scanner.isIntWrap )
            print( "Integer" );
          else
            print( "Yytoken" );
      }
      else
        print( scanner.tokenType );
      
      print(" debug_");
      
      print(scanner.functionName);
      
      if (Options.emit_csharp)
        print("()");
      else
      {
        print("() throws java.io.IOException");
    
        if ( scanner.lexThrow != null ) 
        {
          print(", ");
          print(scanner.lexThrow);
        }

        if ( scanner.scanErrorException != null ) 
        {
          print(", ");
          print(scanner.scanErrorException);
        }
      }
      
      println(" {");

      println("    java_cup.runtime.Symbol s = "+scanner.functionName+"();");
      if (Options.emit_csharp)
      {
        print("    Console.WriteLine( \"");
        
        int @base = 0;

        if (scanner.lineCount) { print("line:{"+@base+"}"); @base++; }
        if (scanner.columnCount) { print(" col:{"+@base+"}"); @base++; }
        println(" --{" + (@base) + "}--{" + (@base + 1) + "}--\",");
        println("      ");
        if (scanner.lineCount) print("yyline+1, ");
        if (scanner.columnCount) print("yycolumn+1, ");
        println("yytext(), getTokenName(s.sym));");
      }
      else
      {
        print("    Console.Out.WriteLine( ");
        if (scanner.lineCount) print("\"line:\" + (yyline+1) + ");
        if (scanner.columnCount) print("\" col:\" + (yycolumn+1) + ");
        println("\" --\"+ yytext() + \"--\" + getTokenName(s.sym) + \"--\");");
      }
      println("    return s;");
      println("  }");
      println("");
    }

    if ( scanner.standalone ) {
      println("  /**");
      println("   * Runs the scanner on input files.");
      println("   *");
      println("   * This is a standalone scanner, it will print any unmatched");
      println("   * text to System.out unchanged.");      
      println("   *");
      println("   * @param argv   the command line, contains the filenames to run");
      println("   *               the scanner on.");
      println("   */");
    }
    else {
      println("  /**");
      println("   * Runs the scanner on input files.");
      println("   *");
      println("   * This main method is the debugging routine for the scanner.");
      println("   * It prints debugging information about each returned token to");
      println("   * System.out until the end of file is reached, or an error occured.");
      println("   *"); 
      println("   * @param argv   the command line, contains the filenames to run");
      println("   *               the scanner on."); 
      println("   */"); 
    }      
    
    if (Options.emit_csharp)
    {
      println("  public static void Main(String[] argv) {");
      println("    if (argv.Length == 0) {");
      println("      Console.WriteLine(\"Usage : "+scanner.className+" <inputfile>\");");
      println("    }");
      println("    else {");
      println("      for (int i = 0; i < argv.Length; i++) {");
      println("        "+scanner.className+" scanner = null;");
      println("        try {");
      println("          scanner = new "+scanner.className+"( new StreamReader(argv[i]) );");

      if ( scanner.standalone ) 
      {      
        println("          while ( !scanner.zzAtEOF ) scanner."+scanner.functionName+"();");
      }
      else if (scanner.cupDebug ) 
      {
        println("          while ( !scanner.zzAtEOF ) scanner.debug_"+scanner.functionName+"();");
      }
      else 
      {
        println("          do {");
		println("             Console.WriteLine(scanner." + scanner.functionName + "());");
        println("          } while (!scanner.zzAtEOF);");
        println("");
      }
 
      println("        }");
      println("        catch (FileNotFoundException) {");
      println("          Console.WriteLine(\"File not found : \\\"{0}\\\"\", argv[i]);");
      println("        }");
      println("        catch (IOException e) {");
      println("          Console.WriteLine(\"IO error scanning file \\\"{0}\\\"\", argv[i]);");
      println("          Console.WriteLine(e);");
      println("        }"); 
      println("        catch (Exception e) {");
      println("          Console.WriteLine(\"Unexpected exception:\");");
      println("          Console.WriteLine(e.ToString());");
      println("        }"); 
      println("      }");
      println("    }");
      println("  }");
    }
    else
    {
      println("  public static void main(String argv[]) {");
      println("    if (argv.length == 0) {");
      println("      System.out.println(\"Usage : java "+scanner.className+" <inputfile>\");");
      println("    }");
      println("    else {");
      println("      for (int i = 0; i < argv.length; i++) {");
      println("        "+scanner.className+" scanner = null;");
      println("        try {");
      println("          scanner = new "+scanner.className+"( new java.io.FileReader(argv[i]) );");

      if ( scanner.standalone ) 
      {      
        println("          while ( !scanner.zzAtEOF ) scanner."+scanner.functionName+"();");
      }
      else if (scanner.cupDebug ) 
      {
        println("          while ( !scanner.zzAtEOF ) scanner.debug_"+scanner.functionName+"();");
      }
      else 
      {
        println("          do {");
        println("            System.out.println(scanner."+scanner.functionName+"());");
        println("          } while (!scanner.zzAtEOF);");
        println("");
      }
 
      println("        }");
      println("        catch (java.io.FileNotFoundException e) {");
      println("          System.out.println(\"File not found : \\\"\"+argv[i]+\"\\\"\");");
      println("        }");
      println("        catch (java.io.IOException e) {");
      println("          System.out.println(\"IO error scanning file \\\"\"+argv[i]+\"\\\"\");");
      println("          System.out.println(e);");
      println("        }"); 
      println("        catch (Exception e) {");
      println("          System.out.println(\"Unexpected exception:\");");
      println("          e.printStackTrace();");
      println("        }"); 
      println("      }");
      println("    }");
      println("  }");
    }
    println("");    
  }
  
  private void emitNoMatch() {
    println("            zzScanError(ZZ_NO_MATCH);");
  }
  
  private void emitNextInput() {
    println("          if (zzCurrentPosL < zzEndReadL)");
    println("            zzInput = zzBufferL[zzCurrentPosL++];");
    println("          else if (zzAtEOF) {");
    println("            zzInput = YYEOF;");
    if (Options.emit_csharp)
      println("            goto zzForAction;");
    else
      println("            break zzForAction;");
    println("          }");
    println("          else {");
    println("            // store back cached positions");
    println("            zzCurrentPos  = zzCurrentPosL;");
    println("            zzMarkedPos   = zzMarkedPosL;");
    if ( scanner.lookAheadUsed ) 
      println("            zzPushbackPos = zzPushbackPosL;");
    if (Options.emit_csharp)
      println("            bool eof = zzRefill();");
    else
      println("            boolean eof = zzRefill();");
    println("            // get translated positions and possibly new buffer");
    println("            zzCurrentPosL  = zzCurrentPos;");
    println("            zzMarkedPosL   = zzMarkedPos;");
    println("            zzBufferL      = zzBuffer;");
    println("            zzEndReadL     = zzEndRead;");
    if ( scanner.lookAheadUsed ) 
      println("            zzPushbackPosL = zzPushbackPos;");
    println("            if (eof) {");
    println("              zzInput = YYEOF;");
    if (Options.emit_csharp)
      println("            goto zzForAction;");
    else
      println("            break zzForAction;");
    println("            }");
    println("            else {");
    println("              zzInput = zzBufferL[zzCurrentPosL++];");
    println("            }");
    println("          }"); 
  }

  private void emitHeader() {
    println("/* The following code was generated by CSFlex "+MainClass.version+" on "+date+" */");   
    println(""); 
  } 

  private void emitUserCode() {
    if ( scanner.userCode.Length > 0 )
    {
      if (Options.emit_csharp)
      {
		  if (Options.emitlines) println("#line 1 \"" + scanner.file + "\"");
        println(scanner.userCode.ToString());
		if (Options.emitlines) println("#line default");
      }
      else
        println(scanner.userCode.ToString());
    }
  }

  private void emitEpilogue() 
  {
    if (scanner.epilogue.Length > 0)
    {
      if (Options.emit_csharp)
      {
		  if (Options.emitlines) println("#line " + scanner.epilogue_line + " \"" + scanner.file + "\"");
        println(scanner.epilogue.ToString());
		if (Options.emitlines) println("#line default");
      }
      else
        println(scanner.epilogue.ToString());
    }
  }

  private void emitClassName() {    
    if (!endsWithJavadoc(scanner.userCode)) {
      String path = inputFile.ToString();
      // slashify path (avoid backslash u sequence = unicode escape)
      if (File.separatorChar != '/') {
	    path = path.Replace(File.separatorChar, '/');
      }
    	
      println("/**");
      println(" * This class is a scanner generated by <a href=\"http://www.sourceforge.net/projects/csflex/\">C# Flex</a>, based on");
      println(" * <a href=\"http://www.jflex.de/\">JFlex</a>, version "+MainClass.version);
      println(" * on "+date+" from the specification file");
      println(" * <tt>"+path+"</tt>");      
      println(" */");
    }   

    if ( scanner.isPublic ) print("public ");

    if ( scanner.isAbstract) print("abstract ");
   
    if ( scanner.isFinal )
    {
      if (Options.emit_csharp)
        print("sealed ");
      else
        print("final ");
    }
    
    print("class ");
    print(scanner.className);
    
    if ( scanner.isExtending != null ) {
      if (Options.emit_csharp)
        print(": ");
      else
        print(" extends ");
      print(scanner.isExtending);
    }

    if ( scanner.isImplementing != null ) {
      if (Options.emit_csharp)
      {
        if (scanner.isExtending != null) // then we already output the ':'
          print(", ");
        else
          print(": ");
      }
      else
        print(" implements ");
      print(scanner.isImplementing);
    }   
    
    println(" {");
  }  

  /**
   * Try to find out if user code ends with a javadoc comment 
   * 
   * @param buffer   the user code
   * @return true    if it ends with a javadoc comment
   */
  public static bool endsWithJavadoc(StringBuilder usercode) {
    String s = usercode.ToString().Trim();
        
    if (!s.EndsWith("*/")) return false;
    
    // find beginning of javadoc comment   
    int i = s.LastIndexOf("/**");    
    if (i < 0) return false; 
       
    // javadoc comment shouldn't contain a comment end
    return s.Substring(i,s.Length-2-i).IndexOf("*/") < 0;
  }


  private void emitLexicalStates() {
    IEnumerator stateNames = scanner.states.names();

    string @const = (Options.emit_csharp ? "const" : "static final");
    
    while ( stateNames.MoveNext() ) {
      String name = (String) stateNames.Current;
      
      int num = scanner.states.getNumber(name).intValue();

      if (scanner.bolUsed)      
        println("  "+visibility+" "+@const+" int "+name+" = "+2*num+";");
      else
        println("  "+visibility+" "+@const+" int "+name+" = "+dfa.lexState[2*num]+";");
    }
    
    if (scanner.bolUsed) {
      println("");
      println("  /**");
      println("   * ZZ_LEXSTATE[l] is the state in the DFA for the lexical state l");
      println("   * ZZ_LEXSTATE[l+1] is the state in the DFA for the lexical state l");
      println("   *                  at the beginning of a line");
      println("   * l is of the form l = 2*k, k a non negative integer");
      println("   */");
      if (Options.emit_csharp)
        println("  private static readonly int[] ZZ_LEXSTATE = new int[]{ ");
      else
        println("  private static final int ZZ_LEXSTATE[] = { ");
  
      int i, j = 0;
      print("    ");

      for (i = 0; i < dfa.lexState.Length-1; i++) {
        print( dfa.lexState[i], 2 );

        print(", ");

        if (++j >= 16) {
          println();
          print("    ");
          j = 0;
        }
      }
            
      println( dfa.lexState[i] );
      println("  };");

    }
  }

  private void emitDynamicInit() {    
    int count = 0;
    int value = dfa.table[0][0];

    println("  /** ");
    println("   * The transition table of the DFA");
    println("   */");

    CountEmitter e = new CountEmitter("Trans");
    e.setValTranslation(+1); // allow vals in [-1, 0xFFFE]
    e.emitInit();
    
    for (int i = 0; i < dfa.numStates; i++) {
      if ( !rowKilled[i] ) {
        for (int c = 0; c < dfa.numInput; c++) {
          if ( !colKilled[c] ) {
            if (dfa.table[i][c] == value) {
              count++;
            } 
            else {
              e.emit(count, value);

              count = 1;
              value = dfa.table[i][c];              
            }
          }
        }
      }
    }

    e.emit(count, value);
    e.emitUnpack();
    
    println(e.ToString());
  }


  private void emitCharMapInitFunction() {

    CharClasses cl = parser.getCharClasses();
    
    if ( cl.getMaxCharCode() < 256 ) return;

    println("");
    println("  /** ");
    println("   * Unpacks the compressed character translation table.");
    println("   *");
    println("   * @param packed   the packed character translation table");
    println("   * @return         the unpacked character translation table");
    println("   */");
    if (Options.emit_csharp)
    {
      println("  private static char [] zzUnpackCMap(ushort[] packed) {");
      println("    char [] map = new char[0x10000];");
      println("    int i = 0;  /* index in packed string  */");
      println("    int j = 0;  /* index in unpacked array */");
      println("    while (i < "+2*intervalls.Length+") {");
      println("      int  count = packed[i++];");
      println("      char value = (char)packed[i++];");
      println("      do map[j++] = value; while (--count > 0);");
      println("    }");
      println("    return map;");
      println("  }");
    }
    else
    {
      println("  private static char [] zzUnpackCMap(String packed) {");
      println("    char [] map = new char[0x10000];");
      println("    int i = 0;  /* index in packed string  */");
      println("    int j = 0;  /* index in unpacked array */");
      println("    while (i < "+2*intervalls.Length+") {");
      println("      int  count = packed.charAt(i++);");
      println("      char value = packed.charAt(i++);");
      println("      do map[j++] = value; while (--count > 0);");
      println("    }");
      println("    return map;");
      println("  }");
    }
  }

  private void emitZZTrans() {    

    int i,c;
    int n = 0;
    
    println("  /** ");
    println("   * The transition table of the DFA");
    println("   */");
    if (Options.emit_csharp)
      println("  private static readonly int[] ZZ_TRANS = new int[] {");
    else
      println("  private static final int ZZ_TRANS [] = {"); //XXX

    print("    ");
    for (i = 0; i < dfa.numStates; i++) {
      
      if ( !rowKilled[i] ) {        
        for (c = 0; c < dfa.numInput; c++) {          
          if ( !colKilled[c] ) {            
            if (n >= 10) {
              println();
              print("    ");
              n = 0;
            }
            print( dfa.table[i][c] );
            if (i != dfa.numStates-1 || c != dfa.numInput-1)
              print( ", ");
            n++;
          }
        }
      }
    }

    println();
    println("  };");
  }
  
  private void emitCharMapArrayUnPacked() {
   
    CharClasses cl = parser.getCharClasses();
    intervalls = cl.getIntervalls();
    
    println("");
    println("  /** ");
    println("   * Translates characters to character classes");
    println("   */");
    if (Options.emit_csharp)
      println("  private static readonly char[] ZZ_CMAP = new char[] {");
    else
      println("  private static final char [] ZZ_CMAP = {");
  
    int n = 0;  // numbers of entries in current line    
    print("    ");
    
    int max =  cl.getMaxCharCode();
    int i = 0;     
    while ( i < intervalls.Length && intervalls[i].start <= max ) {

      int end = Math.Min(intervalls[i].end, max);
      for (int c = intervalls[i].start; c <= end; c++) {

        if (Options.emit_csharp)
          print("(char)");
        print(colMap[intervalls[i].charClass], 2);

        if (c < max) {
          print(", ");        
          if ( ++n >= 16 ) { 
            println();
            print("    ");
            n = 0; 
          }
        }
      }

      i++;
    }

    println();
    println("  };");
    println();
  }

  private void emitCSharpStaticConstructor(bool include_char_map_array)
  {
    if (!Options.emit_csharp)
      return;

    println("  static "+scanner.className+"()");
    println("  {");
    if (include_char_map_array)
      println("    ZZ_CMAP = zzUnpackCMap(ZZ_CMAP_PACKED);");
    println("    ZZ_ACTION = zzUnpackAction();");
    println("    ZZ_ROWMAP = zzUnpackRowMap();");
    println("    ZZ_TRANS = zzUnpackTrans();");
    println("    ZZ_ATTRIBUTE = zzUnpackAttribute();");
    println("  }");
    println("");
  }

  private void emitCharMapArray() {       
    CharClasses cl = parser.getCharClasses();

    if ( cl.getMaxCharCode() < 256 ) {
      emitCSharpStaticConstructor(false);
      emitCharMapArrayUnPacked();
      return;
    }
    else
      emitCSharpStaticConstructor(true);

    // ignores cl.getMaxCharCode(), emits all intervalls instead

    intervalls = cl.getIntervalls();
    
    println("");
    println("  /** ");
    println("   * Translates characters to character classes");
    println("   */");
    if (Options.emit_csharp)
      println("  private static readonly ushort[] ZZ_CMAP_PACKED = new ushort[] {");
    else
      println("  private static final String ZZ_CMAP_PACKED = ");
  
    int n = 0;  // numbers of entries in current line    
    if (Options.emit_csharp)
      print("   ");
    else
      print("    \"");
    
    int i = 0; 
    while ( i < intervalls.Length-1 ) {
      int count = intervalls[i].end-intervalls[i].start+1;
      int value = colMap[intervalls[i].charClass];
      
      printUC(count);
      printUC(value);

      if ( ++n >= 10 ) { 
        if (Options.emit_csharp)
        {
          println("");
          print("   ");
        }
        else
        {
          println("\"+");
          print("    \"");
        }
        n = 0; 
      }

      i++;
    }

    printUC(intervalls[i].end-intervalls[i].start+1);
    printUC(colMap[intervalls[i].charClass]);

    if (Options.emit_csharp)
      println(" 0 };"); // the extraneous 0 can't be avoided without restructuring printUC()
    else
      println("\";");
    println();

    println("  /** ");
    println("   * Translates characters to character classes");
    println("   */");
    if (Options.emit_csharp)
      println("  private static readonly char[] ZZ_CMAP;");
    else
      println("  private static final char [] ZZ_CMAP = zzUnpackCMap(ZZ_CMAP_PACKED);");
    println();
  }


  /**
   * Print number as octal/unicode escaped string character.
   * 
   * @param c   the value to print
   * @prec  0 <= c <= 0xFFFF 
   */
  private void printUC(int c) {
    if (Options.emit_csharp)
      @out.Write(" ");

    if (c > 255) 
    {
      if (Options.emit_csharp)
      {
        @out.Write("0x");
        if (c < 0x1000) @out.Write("0");
        @out.Write(Integer.toHexString(c));
      }
      else
      {
        @out.Write("\\u");
        if (c < 0x1000) @out.Write("0");
        @out.Write(Integer.toHexString(c));
      }
    }
    else {
      if (Options.emit_csharp)
        @out.Write(c.ToString());
      else
      {
        @out.Write("\\");
        @out.Write(Integer.toOctalString(c));
      }
    }    

    if (Options.emit_csharp)
      @out.Write(",");
  }


  private void emitRowMapArray() {
    println("");
    println("  /** ");
    println("   * Translates a state to a row index in the transition table");
    println("   */");
    
    HiLowEmitter e = new HiLowEmitter("RowMap");
    e.emitInit();
    for (int i = 0; i < dfa.numStates; i++) {
      e.emit(rowMap[i]*numCols);
    }    
    e.emitUnpack();
    println(e.ToString());
  }


  private void emitAttributes() {
    println("  /**");
    println("   * ZZ_ATTRIBUTE[aState] contains the attributes of state <code>aState</code>");
    println("   */");
    
    CountEmitter e = new CountEmitter("Attribute");    
    e.emitInit();
    
    int count = 1;
    int value = 0; 
    if ( dfa.isFinal[0]    ) value = FINAL;
    if ( dfa.isPushback[0] ) value|= PUSHBACK;
    if ( dfa.isLookEnd[0]  ) value|= LOOKEND;
    if ( !isTransition[0]  ) value|= NOLOOK;
       
    for (int i = 1;  i < dfa.numStates; i++) {      
      int attribute = 0;      
      if ( dfa.isFinal[i]    ) attribute = FINAL;
      if ( dfa.isPushback[i] ) attribute|= PUSHBACK;
      if ( dfa.isLookEnd[i]  ) attribute|= LOOKEND;
      if ( !isTransition[i]  ) attribute|= NOLOOK;

      if (value == attribute) {
        count++;
      }
      else {        
        e.emit(count, value);
        count = 1;
        value = attribute;
      }
    }
    
    e.emit(count, value);    
    e.emitUnpack();
    
    println(e.ToString());
  }


  private void emitClassCode() {
    if ( scanner.eofCode != null ) {
      println("  /** denotes if the user-EOF-code has already been executed */");
      if (Options.emit_csharp)
        println("  private bool zzEOFDone;");
      else
        println("  private boolean zzEOFDone;");
      println("");
    }
    
    if ( scanner.classCode != null ) {
      println("  /* user code: */");
      println(scanner.classCode);
    }
  }

  private void emitConstructorDecl() {
    
    print("  ");

    if (Options.emit_csharp)
    {
      if ( scanner.isPublic )
        print("public ");
      else
        print("internal ");
      print( scanner.className );      
      print("(TextReader @in)");
    }
    else
    {
      if ( scanner.isPublic )
        print("public ");
      print( scanner.className );      
      print("(java.io.Reader in)");

      if ( scanner.initThrow != null ) 
      {
        print(" throws ");
        print( scanner.initThrow );
      }
    }
   
    println(" {");

    if ( scanner.initCode != null ) {
      print("  ");
      print( scanner.initCode );
    }

    println("    this.zzReader = @in;");

    println("  }");
    println();


    if (Options.emit_csharp)
    {
      println("  /**");
      println("   * Creates a new scanner.");
      println("   * There is also TextReader version of this constructor.");
      println("   *");
      println("   * @param   in  the System.IO.Stream to read input from.");
      println("   */");

      print("  ");
      if ( scanner.isPublic )
        print("public ");
      else
        print("internal ");
      print( scanner.className );      
      print("(Stream @in)");
    
      println(" : this(new StreamReader(@in))");
      println("  {");    
      println("  }");
    }
    else
    {
      println("  /**");
      println("   * Creates a new scanner.");
      println("   * There is also java.io.Reader version of this constructor.");
      println("   *");
      println("   * @param   in  the java.io.Inputstream to read input from.");
      println("   */");

      print("  ");
      if ( scanner.isPublic ) print("public ");    
      print( scanner.className );      
      print("(java.io.InputStream in)");
    
      if ( scanner.initThrow != null ) 
      {
        print(" throws ");
        print( scanner.initThrow );
      }
    
      println("  {");    
      println("    this(new java.io.InputStreamReader(in));");
      println("  }");
    }
  }


  private void emitDoEOF() {
    if ( scanner.eofCode == null ) return;
    
    println("  /**");
    println("   * Contains user EOF-code, which will be executed exactly once,");
    println("   * when the end of file is reached");
    println("   */");
    
    print("  private void zzDoEOF()");
    
    if (!Options.emit_csharp)
      if ( scanner.eofThrow != null ) 
      {
        print(" throws ");
        print(scanner.eofThrow);
      }
    
    println(" {");
    
    println("    if (!zzEOFDone) {");
    println("      zzEOFDone = true;");
    println("      "+scanner.eofCode );
    println("    }");
    println("  }");
    println("");
    println("");
  }

  private void emitLexFunctHeader() {
    
    if (scanner.cupCompatible)  {
      // force public, because we have to implement java_cup.runtime.Symbol
      print("  public ");
    }
    else {
      print("  "+visibility+" ");
    }
    
    if ( scanner.tokenType == null ) {
      if ( scanner.isInteger )
        print( "int" );
      else 
      if ( scanner.isIntWrap )
        print( "Integer" );
      else
        print( "Yytoken" );
    }
    else
      print( scanner.tokenType );
      
    print(" ");
    
    print(scanner.functionName);
      
    if (Options.emit_csharp)
      print("()");
    else
    {
      print("() throws java.io.IOException");
    
      if ( scanner.lexThrow != null ) 
      {
        print(", ");
        print(scanner.lexThrow);
      }

      if ( scanner.scanErrorException != null ) 
      {
        print(", ");
        print(scanner.scanErrorException);
      }
    }
    
    println(" {");
    
    skel.emitNext();

    if ( scanner.useRowMap ) {
      println("    int [] zzTransL = ZZ_TRANS;");
      println("    int [] zzRowMapL = ZZ_ROWMAP;");
      println("    int [] zzAttrL = ZZ_ATTRIBUTE;");

    }

    if ( scanner.lookAheadUsed ) {
      println("    int zzPushbackPosL = zzPushbackPos = -1;");
      if (Options.emit_csharp)
        println("    bool zzWasPushback;");
      else
        println("    boolean zzWasPushback;");
    }

    skel.emitNext();    
        
    if ( scanner.charCount ) {
      println("      yychar+= zzMarkedPosL-zzStartRead;");
      println("");
    }
    
    if ( scanner.lineCount || scanner.columnCount ) {
      if (Options.emit_csharp)
        println("      bool zzR = false;");
      else
        println("      boolean zzR = false;");
      println("      for (zzCurrentPosL = zzStartRead; zzCurrentPosL < zzMarkedPosL;");
      println("                                                             zzCurrentPosL++) {");
      println("        switch (zzBufferL[zzCurrentPosL]) {");
      println("        case '\\u000B':"); 
      println("        case '\\u000C':"); 
      println("        case '\\u0085':");
      println("        case '\\u2028':"); 
      println("        case '\\u2029':"); 
      if ( scanner.lineCount )
        println("          yyline++;");
      if ( scanner.columnCount )
        println("          yycolumn = 0;");
      println("          zzR = false;");
      println("          break;");      
      println("        case '\\r':");
      if ( scanner.lineCount )
        println("          yyline++;");
      if ( scanner.columnCount )
        println("          yycolumn = 0;");
      println("          zzR = true;");
      println("          break;");
      println("        case '\\n':");
      println("          if (zzR)");
      println("            zzR = false;");
      println("          else {");
      if ( scanner.lineCount )
        println("            yyline++;");
      if ( scanner.columnCount )
        println("            yycolumn = 0;");
      println("          }");
      println("          break;");
      println("        default:");
      println("          zzR = false;");
      if ( scanner.columnCount ) 
        println("          yycolumn++;");
      if (Options.emit_csharp)
        println("          break;");
      println("        }");
      println("      }");
      println();

      if ( scanner.lineCount ) {
        println("      if (zzR) {");
        println("        // peek one character ahead if it is \\n (if we have counted one line too much)");
        if (Options.emit_csharp)
          println("        bool zzPeek;");
        else
          println("        boolean zzPeek;");
        println("        if (zzMarkedPosL < zzEndReadL)");
        println("          zzPeek = zzBufferL[zzMarkedPosL] == '\\n';");
        println("        else if (zzAtEOF)");
        println("          zzPeek = false;");
        println("        else {");
        if (Options.emit_csharp)
          println("          bool eof = zzRefill();");
        else
          println("          boolean eof = zzRefill();");
        println("          zzMarkedPosL = zzMarkedPos;");
        println("          zzBufferL = zzBuffer;");
        println("          if (eof) ");
        println("            zzPeek = false;");
        println("          else ");
        println("            zzPeek = zzBufferL[zzMarkedPosL] == '\\n';");
        println("        }");
        println("        if (zzPeek) yyline--;");
        println("      }");
      }
    }

    if ( scanner.bolUsed ) {
      // zzMarkedPos > zzStartRead <=> last match was not empty
      // if match was empty, last value of zzAtBOL can be used
      // zzStartRead is always >= 0
      println("      if (zzMarkedPosL > zzStartRead) {");
      println("        switch (zzBufferL[zzMarkedPosL-1]) {");
      println("        case '\\n':");
      println("        case '\\u000B':"); 
      println("        case '\\u000C':"); 
      println("        case '\\u0085':");
      println("        case '\\u2028':"); 
      println("        case '\\u2029':"); 
      println("          zzAtBOL = true;");
      println("          break;"); 
      println("        case '\\r': "); 
      println("          if (zzMarkedPosL < zzEndReadL)");
      println("            zzAtBOL = zzBufferL[zzMarkedPosL] != '\\n';");
      println("          else if (zzAtEOF)");
      println("            zzAtBOL = false;");
      println("          else {");
      if (Options.emit_csharp)
        println("            bool eof = zzRefill();");
      else
        println("            boolean eof = zzRefill();");
      println("            zzMarkedPosL = zzMarkedPos;");
      println("            zzBufferL = zzBuffer;");
      println("            if (eof) ");
      println("              zzAtBOL = false;");
      println("            else ");
      println("              zzAtBOL = zzBufferL[zzMarkedPosL] != '\\n';");
      println("          }");      
      println("          break;"); 
      println("        default:"); 
      println("          zzAtBOL = false;");
      if (Options.emit_csharp)
        println("          break;");
      println("        }"); 
      println("      }"); 
    }

    skel.emitNext();
    
    if (scanner.bolUsed) {
      println("      if (zzAtBOL)");
      println("        zzState = ZZ_LEXSTATE[zzLexicalState+1];");
      println("      else");    
      println("        zzState = ZZ_LEXSTATE[zzLexicalState];");
      println();
    }
    else {
      println("      zzState = zzLexicalState;");
      println();
    }

    if (scanner.lookAheadUsed)
      println("      zzWasPushback = false;");

    skel.emitNext();
  }

  
  private void emitGetRowMapNext() {
    println("          int zzNext = zzTransL[ zzRowMapL[zzState] + zzCMapL[zzInput] ];");
    if (Options.emit_csharp)
      println("          if (zzNext == "+DFA.NO_TARGET+") goto zzForAction;");
    else
      println("          if (zzNext == "+DFA.NO_TARGET+") break zzForAction;");
    println("          zzState = zzNext;");
    println();

    println("          int zzAttributes = zzAttrL[zzState];");

    if ( scanner.lookAheadUsed ) {
      println("          if ( (zzAttributes & "+PUSHBACK+") == "+PUSHBACK+" )");
      println("            zzPushbackPosL = zzCurrentPosL;");
      println();
    }

    println("          if ( (zzAttributes & "+FINAL+") == "+FINAL+" ) {");
    if ( scanner.lookAheadUsed ) 
      println("            zzWasPushback = (zzAttributes & "+LOOKEND+") == "+LOOKEND+";");

    skel.emitNext();

    if (Options.emit_csharp)
      println("            if ( (zzAttributes & "+NOLOOK+") == "+NOLOOK+" ) goto zzForAction;");
    else
      println("            if ( (zzAttributes & "+NOLOOK+") == "+NOLOOK+" ) break zzForAction;");

    skel.emitNext();    
  }  

  private void emitTransitionTable() {
    transformTransitionTable();
    
    println("          zzInput = zzCMapL[zzInput];");
    println();

    if (Options.emit_csharp)
    {
      if ( scanner.lookAheadUsed ) 
        println("          bool zzPushback = false;");
      
      println("          bool zzIsFinal = false;");
      println("          bool zzNoLookAhead = false;");
      println();
    }
    else
    {
      if ( scanner.lookAheadUsed ) 
        println("          boolean zzPushback = false;");
      
      println("          boolean zzIsFinal = false;");
      println("          boolean zzNoLookAhead = false;");
      println();
    }
    
    if (Options.emit_csharp)
    {
      println("          switch (zzState) {");
      println("            case 2147483647:");
      println("              zzForNext: break;");
      println("            case 2147483646:");
      println("              goto zzForNext;");
    }
    else
      println("          zzForNext: { switch (zzState) {");

    for (int state = 0; state < dfa.numStates; state++)
      if (isTransition[state]) emitState(state);

    println("            default:");
    println("              // if this is ever reached, there is a serious bug in JFlex/C# Flex");
    println("              zzScanError(ZZ_UNKNOWN_ERROR);");
    println("              break;");
    if (Options.emit_csharp)
      println("          }");
    else
      println("          } }");
    println();
    
    println("          if ( zzIsFinal ) {");
    
    if ( scanner.lookAheadUsed ) 
      println("            zzWasPushback = zzPushback;");
    
    skel.emitNext();
    
    if (Options.emit_csharp)
      println("            if ( zzNoLookAhead ) goto zzForAction;");
    else
      println("            if ( zzNoLookAhead ) break zzForAction;");

    skel.emitNext();    
  }


  /**
   * Escapes all " ' \ tabs and newlines
   */
  private String escapify(String s) {
    StringBuilder result = new StringBuilder(s.Length*2);
    
    for (int i = 0; i < s.Length; i++) {
      char c = s[i];
      switch (c) {
      case '\'': result.Append("\\\'"); break;
      case '\"': result.Append("\\\""); break;
      case '\\': result.Append("\\\\"); break;
      case '\t': result.Append("\\t"); break;
      case '\r': if (i+1 == s.Length || s[i+1] != '\n') result.Append("\"+ZZ_NL+\""); 
                 break;
      case '\n': result.Append("\"+ZZ_NL+\""); break;
      default: result.Append(c); break;
      }
    }

    return result.ToString();
  }

  public void emitActionTable() {
    int lastAction = 1;    
    int count = 0;
    int value = 0;

    println("  /** ");
    println("   * Translates DFA states to action switch labels.");
    println("   */");
    CountEmitter e = new CountEmitter("Action");    
    e.emitInit();

    for (int i = 0; i < dfa.numStates; i++) {
      int newVal; 
      if ( dfa.isFinal[i] ) {
        Action action = dfa.action[i];
        Integer stored = (Integer) actionTable[action];
        if ( stored == null ) { 
          stored = new Integer(lastAction++);
          actionTable[action] = stored;
        }
        newVal = stored.intValue();
      }
      else {
        newVal = 0;
      }
      
      if (value == newVal) {
        count++;
      }
      else {
        if (count > 0) e.emit(count,value);
        count = 1;
        value = newVal;        
      }
    }
    
    if (count > 0) e.emit(count,value);

    e.emitUnpack();    
    println(e.ToString());
  }

  private void emitActions() {
    println("      switch (zzAction < 0 ? zzAction : ZZ_ACTION[zzAction]) {");

    int i = actionTable.Count+1;  
    IEnumerator actions = actionTable.Keys.GetEnumerator();
    while ( actions.MoveNext() ) {
      Action action = (Action) actions.Current;
      int label = ((Integer) actionTable[action]).intValue();

      println("        case "+label+": "); 

      if (Options.emit_csharp)
      {
        println("          if (ZZ_SPURIOUS_WARNINGS_SUCK)");
        println("          {");

        if ( scanner.debugOption ) 
        {
          int @base = 0;

          print("            Console.WriteLine(\"");
          if ( scanner.lineCount ) { print("line: {"+@base+"} "); @base++; }
          if ( scanner.columnCount ) { print("col: {"+@base+"} "); @base++; }
          println("match: --{"+@base+"}--\",");
          print("              ");
          if ( scanner.lineCount ) print("yyline+1, ");
          if ( scanner.columnCount ) print("yycolumn+1, ");
          println("yytext());");

          print("            Console.WriteLine(\"action ["+action.priority+"] { ");
          print(escapify(action.content));
          println(" }\");");
        }

		if (Options.emitlines) println("#line " + action.priority + " \"" + escapify(scanner.file) + "\"");
        println(action.content);
		if (Options.emitlines) println("#line default");
        println("          }");
        println("          break;");
      }
      else
      {
        if ( scanner.debugOption ) 
        {
          print("          System.out.println(");
          if ( scanner.lineCount )
            print("\"line: \"+(yyline+1)+\" \"+");
          if ( scanner.columnCount )
            print("\"col: \"+(yycolumn+1)+\" \"+");
          println("\"match: --\"+yytext()+\"--\");");        
          print("          System.out.println(\"action ["+action.priority+"] { ");
          print(escapify(action.content));
          println(" }\");");
        }
      
        println("          { "+action.content);
        println("          }");
        println("        case "+(i++)+": break;"); 
      }
    }
  }

  private void emitEOFVal() {
    EOFActions eofActions = parser.getEOFActions();

    if ( scanner.eofCode != null ) 
      println("            zzDoEOF();");
      
    if ( eofActions.numActions() > 0 ) {
      println("            switch (zzLexicalState) {");
      
      IEnumerator stateNames = scanner.states.names();

      // record lex states already emitted:
      Hashtable used = new PrettyHashtable();

      // pick a start value for break case labels. 
      // must be larger than any value of a lex state:
      int last = dfa.numStates;
      
      while ( stateNames.MoveNext() ) {
        String name = (String) stateNames.Current;
        int num = scanner.states.getNumber(name).intValue();
        Action action = eofActions.getAction(num);

        // only emit code if the lex state is not redundant, so
        // that case labels don't overlap
        // (redundant = points to the same dfa state as another one).
        // applies only to scanners that don't use BOL, because
        // in BOL scanners lex states get mapped at runtime, so
        // case labels will always be unique.
        bool unused = true;                
        if (!scanner.bolUsed) {
          Integer key = new Integer(dfa.lexState[2*num]);
          unused = used[key] == null;
          
          if (!unused) 
            Out.warning("Lexical states <"+name+"> and <"+used[key]+"> are equivalent.");
          else
            used[key] = name;
        }

        if (action != null && unused) 
        {
          if (Options.emit_csharp)
          {
            println("            case "+name+":");
            println("              if (ZZ_SPURIOUS_WARNINGS_SUCK)");
            println("              {");
			if (Options.emitlines) println("#line " + action.priority + " \"" + scanner.file + "\"");
            println(action.content);
			if (Options.emitlines) println("#line default");
            println("              }");
            println("              break;");
          }
          else
          {
            println("            case "+name+":");
            println("              { "+action.content+" }");
            println("            case "+(++last)+": break;");
          }
        }
      }
      
      println("            default:");
    }

    if (eofActions.getDefault() != null) 
    {
      if (Options.emit_csharp)
      {
        Action dfl = eofActions.getDefault();

        println("              if (ZZ_SPURIOUS_WARNINGS_SUCK)");
        println("              {");
		if (Options.emitlines) println("#line " + dfl.priority + " \"" + scanner.file + "\"");
        println(dfl.content);
		if (Options.emitlines) println("#line default");
        println("              }");
      }
      else
      {
        println("              if (ZZ_SPURIOUS_WARNINGS_SUCK)");
        println("              { " + eofActions.getDefault().content + " }");
      }
      println("              break;");
    }
    else if ( scanner.eofVal != null ) 
    {
      println("              if (ZZ_SPURIOUS_WARNINGS_SUCK)");
      println("              { " + scanner.eofVal + " }");
      println("              break;");
    }
    else if ( scanner.isInteger ) 
      println("            return YYEOF;");
    else
      println("            return null;");

    if (eofActions.numActions() > 0)
      println("            }");
  }
  
  private void emitState(int state) {
    
    println("            case "+state+":");
    println("              switch (zzInput) {");
   
    int defaultTransition = getDefaultTransition(state);
    
    for (int next = 0; next < dfa.numStates; next++) {
            
      if ( next != defaultTransition && table[state][next] != null ) {
        emitTransition(state, next);
      }
    }
    
    if ( defaultTransition != DFA.NO_TARGET && noTarget[state] != null ) {
      emitTransition(state, DFA.NO_TARGET);
    }
    
    emitDefaultTransition(state, defaultTransition);
    
    println("              }");
    println("");
  }
  
  private void emitTransition(int state, int nextState) 
  {

    CharSetEnumerator chars;
    
    if (nextState != DFA.NO_TARGET) 
      chars = table[state][nextState].characters();
    else 
      chars = noTarget[state].characters();
  
    print("                case ");
    print((int)chars.nextElement());
    print(": ");
    
    while ( chars.hasMoreElements() ) 
    {
      println();
      print("                case ");
      print((int)chars.nextElement());
      print(": ");
    } 
    
    if ( nextState != DFA.NO_TARGET ) 
    {
      if ( dfa.isFinal[nextState] )
        print("zzIsFinal = true; ");
        
      if ( dfa.isPushback[nextState] ) 
        print("zzPushbackPosL = zzCurrentPosL; ");
      
      if ( dfa.isLookEnd[nextState] )
        print("zzPushback = true; ");

      if ( !isTransition[nextState] )
        print("zzNoLookAhead = true; ");
        
      if ( Options.emit_csharp ) 
        println("zzState = "+nextState+"; goto zzForNext;");
      else
        println("zzState = "+nextState+"; break zzForNext;");
    }
    else
    {
      if (Options.emit_csharp)
        println("goto zzForAction;");
      else
        println("break zzForAction;");
    }
  }
  
  private void emitDefaultTransition(int state, int nextState) {
    print("                default: ");
    
    if ( nextState != DFA.NO_TARGET ) 
    {
      if ( dfa.isFinal[nextState] )
        print("zzIsFinal = true; ");
        
      if ( dfa.isPushback[nextState] ) 
        print("zzPushbackPosL = zzCurrentPosL; ");

      if ( dfa.isLookEnd[nextState] )
        print("zzPushback = true; ");
          
      if ( !isTransition[nextState] )
        print("zzNoLookAhead = true; ");
        
      if ( Options.emit_csharp ) 
        println("zzState = "+nextState+"; goto zzForNext;");
      else
        println("zzState = "+nextState+"; break zzForNext;");
    }
    else
    {
      if (Options.emit_csharp)
        println( "goto zzForAction;" );
      else
        println( "break zzForAction;" );
    }
  }
  
  private void emitPushback() {
    println("      if (zzWasPushback)");
    println("        zzMarkedPos = zzPushbackPosL;");
  }
  
  private int getDefaultTransition(int state) {
    int max = 0;
    
    for (int i = 0; i < dfa.numStates; i++) {
      if ( table[state][max] == null )
        max = i;
      else
      if ( table[state][i] != null && table[state][max].size() < table[state][i].size() )
        max = i;
    }
    
    if ( table[state][max] == null ) return DFA.NO_TARGET;
    if ( noTarget[state] == null ) return max;
    
    if ( table[state][max].size() < noTarget[state].size() ) 
      max = DFA.NO_TARGET;
    
    return max;
  }

  // for switch statement:
  private void transformTransitionTable() {
    
    int numInput = parser.getCharClasses().getNumClasses()+1;

    int i;    
    char j;
    
    table = new CharSet[dfa.numStates][];
    for (i=0; i < table.Length; i++)
      table[i] = new CharSet[dfa.numStates];
    noTarget = new CharSet[dfa.numStates];
    
    for (i = 0; i < dfa.numStates;  i++) 
      for (j = (char)0; j < dfa.numInput; j++) {

        int nextState = dfa.table[i][j];
        
        if ( nextState == DFA.NO_TARGET ) {
          if ( noTarget[i] == null ) 
            noTarget[i] = new CharSet(numInput, colMap[j]);
          else
            noTarget[i].add(colMap[j]);
        }
        else {
          if ( table[i][nextState] == null ) 
            table[i][nextState] = new CharSet(numInput, colMap[j]);
          else
            table[i][nextState].add(colMap[j]);
        }
      }
  }

  private void findActionStates() {
    isTransition = new bool [dfa.numStates];
    
    for (int i = 0; i < dfa.numStates;  i++) {
      char j = (char)0;
      while ( !isTransition[i] && j < dfa.numInput )
        isTransition[i] = dfa.table[i][j++] != DFA.NO_TARGET;
    }
  }

  
  private void reduceColumns() {
    colMap = new int [dfa.numInput];
    colKilled = new bool [dfa.numInput];

    int i,j,k;
    int translate = 0;
    bool equal;

    numCols = dfa.numInput;

    for (i = 0; i < dfa.numInput; i++) {
      
      colMap[i] = i-translate;
      
      for (j = 0; j < i; j++) {
        
        // test for equality:
        k = -1;
        equal = true;        
        while (equal && ++k < dfa.numStates) 
          equal = dfa.table[k][i] == dfa.table[k][j];
        
        if (equal) {
          translate++;
          colMap[i] = colMap[j];
          colKilled[i] = true;
          numCols--;
          break;
        } // if
      } // for j
    } // for i
  }
  
  private void reduceRows() {
    rowMap = new int [dfa.numStates];
    rowKilled = new bool [dfa.numStates];
    
    int i,j,k;
    int translate = 0;
    bool equal;

    numRows = dfa.numStates;

    // i is the state to add to the new table
    for (i = 0; i < dfa.numStates; i++) {
      
      rowMap[i] = i-translate;
      
      // check if state i can be removed (i.e. already
      // exists in entries 0..i-1)
      for (j = 0; j < i; j++) {
        
        // test for equality:
        k = -1;
        equal = true;
        while (equal && ++k < dfa.numInput) 
          equal = dfa.table[i][k] == dfa.table[j][k];
        
        if (equal) {
          translate++;
          rowMap[i] = rowMap[j];
          rowKilled[i] = true;
          numRows--;
          break;
        } // if
      } // for j
    } // for i
    
  } 


  /**
   * Set up EOF code sectioin according to scanner.eofcode 
   */
  private void setupEOFCode() {
    if (scanner.eofclose) {
      scanner.eofCode = LexScan.conc(scanner.eofCode, "  yyclose();");
      scanner.eofThrow = LexScan.concExc(scanner.eofThrow, "java.io.IOException");
    }    
  } 


  /**
   * Main Emitter method.  
   */
  public void emit() {    

    setupEOFCode();

    if (scanner.functionName == null) 
      scanner.functionName = "yylex";

    reduceColumns();
    findActionStates();

    emitHeader();
    emitUserCode();
    emitClassName();
    
    skel.emitNext();
    
    if (Options.emit_csharp)
    {
      println("  private const int ZZ_BUFFERSIZE = "+scanner.bufferSize+";");

      if (scanner.debugOption) 
      {
        println("  private static readonly String ZZ_NL = Environment.NewLine;");
      }

      println("  /**");
      println("   * This is used in 'if' statements to eliminate dead code");
      println("   * warnings for 'break;' after the end of a user action");
      println("   * block of code. The Java version does this by emitting");
      println("   * a second 'case' which is impossible to reach. Since this");
      println("   * is impossible for the compiler to deduce during semantic");
      println("   * analysis, the warning is stifled. However, C# does not");
      println("   * permit 'case' blocks to flow into each other, so the C#");
      println("   * output mode needs a different approach. In this case,");
      println("   * the entire user code is wrapped up in an 'if' statement");
      println("   * whose condition is always true. No warning is emitted");
      println("   * because the compiler doesn't strictly propagate the value");
      println("   * of 'static readonly' fields, and thus does not semantically");
      println("   * detect the fact that the 'if' will always be true.");
      println("   */");
      println("   public static readonly bool ZZ_SPURIOUS_WARNINGS_SUCK = true;");
    }
    else
    {
      println("  private static final int ZZ_BUFFERSIZE = "+scanner.bufferSize+";");

      if (scanner.debugOption) 
      {
        println("  private static final String ZZ_NL = System.getProperty(\"line.separator\");");
      }
    }

    skel.emitNext();

    emitLexicalStates();
   
    emitCharMapArray();
    
    emitActionTable();
    
    if (scanner.useRowMap) {
     reduceRows();
    
      emitRowMapArray();

      if (scanner.packed)
        emitDynamicInit();
      else
        emitZZTrans();
    }
    
    skel.emitNext();
    
    if (scanner.useRowMap) 
      emitAttributes();    

    skel.emitNext();
    
    emitClassCode();
    
    skel.emitNext();
    
    emitConstructorDecl();
        
    emitCharMapInitFunction();

    skel.emitNext();
    
    emitScanError();

    skel.emitNext();        

    emitDoEOF();
    
    skel.emitNext();
    
    emitLexFunctHeader();
    
    emitNextInput();

    if (scanner.useRowMap)
      emitGetRowMapNext();
    else
      emitTransitionTable();
        
    if (scanner.lookAheadUsed) 
      emitPushback();
        
    skel.emitNext();

    emitActions();
        
    skel.emitNext();

    emitEOFVal();
    
    skel.emitNext();
    
    emitNoMatch();

    skel.emitNext();
    
    emitMain();
    
    skel.emitNext();

    emitEpilogue();

    @out.Close();
  }

}
}