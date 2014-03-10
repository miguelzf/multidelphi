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

namespace CSFlex
{
 
/**
 * This is the main class of C# Flex controlling the scanner generation process. 
 * It is responsible for parsing the commandline, getting input files,
 * starting up the GUI if necessary, etc. 
 *
 * @author Gerwin Klein
 * @version JFlex 1.4, $Revision: 2.18 $, $Date: 2004/04/12 10:34:10 $
 * @author Jonathan Gilbert
 * @version CSFlex 1.4
 */
public class MainClass {
  
  /** C# Flex version */
  public const String version = "1.4"; //$NON-NLS-1$

  /**
   * Generates a scanner for the specified input file.
   *
   * @param inputFile  a file containing a lexical specification
   *                   to generate a scanner for.
   */
  public static void generate(File inputFile) {

    Out.resetCounters();

    Timer totalTime = new Timer();
    Timer time      = new Timer();
      
    LexScan scanner = null;
    LexParse parser = null;
    TextReader inputReader = null;
    
    totalTime.start();      

    try {  
      Out.println(ErrorMessages.READING, inputFile.ToString());
      inputReader = new StreamReader(inputFile);
      scanner = new LexScan(inputReader);
      scanner.setFile(inputFile);
      parser = new LexParse(scanner);
    }
    catch (FileNotFoundException) {
      Out.error(ErrorMessages.CANNOT_OPEN, inputFile.ToString());
      throw new GeneratorException();
    }
      
    try {  
      NFA nfa = (NFA) parser.parse().value;      

      Out.checkErrors();

      if (Options.dump) Out.dump(ErrorMessages.get(ErrorMessages.NFA_IS)+
                                 Out.NL+nfa+Out.NL); 
      
      if (Options.dot) 
        nfa.writeDot(Emitter.normalize("nfa.dot", null));       //$NON-NLS-1$

      Out.println(ErrorMessages.NFA_STATES, nfa.numStates);
      
      time.start();
      DFA dfa = nfa.getDFA();
      time.stop();
      Out.time(ErrorMessages.DFA_TOOK, time); 

      dfa.checkActions(scanner, parser);

      nfa = null;

      if (Options.dump) Out.dump(ErrorMessages.get(ErrorMessages.DFA_IS)+
                                 Out.NL+dfa+Out.NL);       

      if (Options.dot) 
        dfa.writeDot(Emitter.normalize("dfa-big.dot", null)); //$NON-NLS-1$

      time.start();
      dfa.minimize();
      time.stop();

      Out.time(ErrorMessages.MIN_TOOK, time); 
            
      if (Options.dump) 
        Out.dump(ErrorMessages.get(ErrorMessages.MIN_DFA_IS)+
                                   Out.NL+dfa); 

      if (Options.dot) 
        dfa.writeDot(Emitter.normalize("dfa-min.dot", null)); //$NON-NLS-1$

      time.start();
      
      Emitter e = new Emitter(inputFile, parser, dfa);
      e.emit();

      time.stop();

      Out.time(ErrorMessages.WRITE_TOOK, time); 
      
      totalTime.stop();
      
      Out.time(ErrorMessages.TOTAL_TIME, totalTime); 
    }
    catch (ScannerException e) {
      Out.error(e.file, e.message, e.line, e.column);
      throw new GeneratorException();
    }
    catch (MacroException e) {
      Out.error(e.Message);
      throw new GeneratorException();
    }
    catch (IOException e) {
      Out.error(ErrorMessages.IO_ERROR, e.ToString()); 
      throw new GeneratorException();
    }
    catch (OutOfMemoryException) {
      Out.error(ErrorMessages.OUT_OF_MEMORY);
      throw new GeneratorException();
    }
    catch (GeneratorException) {
      throw new GeneratorException();
    }
    catch (Exception e) {
      Out.error(e.ToString());
      throw new GeneratorException();
    }

  }

  public static ArrayList parseOptions(String[] argv) {
    ArrayList files = new PrettyArrayList();

    for (int i = 0; i < argv.Length; i++) {

      if ( (argv[i] == "-d") || (argv[i] == "--outdir") ) { //$NON-NLS-1$ //$NON-NLS-2$
        if ( ++i >= argv.Length ) {
          Out.error(ErrorMessages.NO_DIRECTORY); 
          throw new GeneratorException();
        }
        Options.setDir(argv[i]);
        continue;
      }

      if ( (argv[i] == "--skel") || (argv[i] == "-skel") ) { //$NON-NLS-1$ //$NON-NLS-2$
        if ( ++i >= argv.Length ) {
          Out.error(ErrorMessages.NO_SKEL_FILE);
          throw new GeneratorException();
        }

        Options.setSkeleton(new File(argv[i]));
        continue;
      }

      if ( (argv[i] == "--nested-default-skeleton") || (argv[i] == "-nested") )
      {
        Options.setSkeleton(new File("<nested>"));
        continue;
      }

      if ( (argv[i] == "-jlex") || (argv[i] == "--jlex") ) { //$NON-NLS-1$ //$NON-NLS-2$
        Options.jlex = true;
        continue;
      }

      if ( (argv[i] == "-v") || (argv[i] == "--verbose") || (argv[i] == "-verbose") ) { //$NON-NLS-1$ //$NON-NLS-2$ //$NON-NLS-3$
        Options.verbose = true;
        // Options.progress = true;	// Miguel: never print dots
        continue;
      }

      if ( (argv[i] == "-q") || (argv[i] == "--quiet") || (argv[i] == "-quiet") ) { //$NON-NLS-1$ //$NON-NLS-2$ //$NON-NLS-3$
        Options.verbose = false;
        Options.progress = false;
        continue;
      }

	  if ((argv[i] == "--debug") || (argv[i] == "-debug"))
	  { //$NON-NLS-1$ //$NON-NLS-2$ //$NON-NLS-3$
		  Options.debug = true;
		  
		  continue;
	  }

      if ( (argv[i] == "--dump") || (argv[i] == "-dump") ) { //$NON-NLS-1$ //$NON-NLS-2$
        Options.dump = true;
        continue;
      }

      if ( (argv[i] == "--time") || (argv[i] == "-time") ) { //$NON-NLS-1$ //$NON-NLS-2$
        Options.time = true;
        continue;
      }

      if ( (argv[i] == "--version") || (argv[i] == "-version") ) { //$NON-NLS-1$ //$NON-NLS-2$
        Out.println(ErrorMessages.THIS_IS_CSFLEX, version); 
        throw new SilentExit();
      }

      if ( (argv[i] == "--dot") || (argv[i] == "-dot") ) { //$NON-NLS-1$ //$NON-NLS-2$
        Options.dot = true;
        continue;
      }

      if ( (argv[i] == "--help") || (argv[i] == "-h") || (argv[i] == "/h") ) { //$NON-NLS-1$ //$NON-NLS-2$ //$NON-NLS-3$
        printUsage();
        throw new SilentExit();
      }

      if ( (argv[i] == "--info") || (argv[i] == "-info") ) { //$NON-NLS-1$ //$NON-NLS-2$
        Out.printSystemInfo();
        throw new SilentExit();
      }
      
      if ( (argv[i] == "--nomin") || (argv[i] == "-nomin") ) { //$NON-NLS-1$ //$NON-NLS-2$
        Options.no_minimize = true;
        continue;
      }

      if ( (argv[i] == "--pack") || (argv[i] == "-pack") ) { //$NON-NLS-1$ //$NON-NLS-2$
        Options.gen_method = Options.PACK;
        continue;
      }

      if ( (argv[i] == "--table") || (argv[i] == "-table") ) { //$NON-NLS-1$ //$NON-NLS-2$
        Options.gen_method = Options.TABLE;
        continue;
      }

      if ( (argv[i] == "--switch") || (argv[i] == "-switch") ) { //$NON-NLS-1$ //$NON-NLS-2$
        Options.gen_method = Options.SWITCH;
        continue;
      }
      
      if ( (argv[i] == "--nobak") || (argv[i] == "-nobak") ) { //$NON-NLS-1$ //$NON-NLS-2$
        Options.no_backup = true;
        continue;
      }

	// Miguel: create partial class
	  if ((argv[i] == "--partial") || (argv[i] == "-partial"))
	  { //$NON-NLS-1$ //$NON-NLS-2$
		  Options.partialclass = true;
		  continue;
	  }
	  
		/*	// Miguel: only CSharp now
		if ( (argv[i] == "--csharp") || (argv[i] == "-cs") ) 
		{
		  Options.emit_csharp = true;
		  continue;
		}
	   */
      
      if ( argv[i].StartsWith("-") ) { //$NON-NLS-1$
        Out.error(ErrorMessages.UNKNOWN_COMMANDLINE, argv[i]);
        printUsage();
        throw new SilentExit();
      }

      // if argv[i] is not an option, try to read it as file 
      File f = new File(argv[i]);
      if ( f.isFile() && f.canRead() ) 
        files.Add(f);      
      else {
        Out.error("Sorry, couldn't open \""+f+"\""); //$NON-NLS-2$
        throw new GeneratorException();
      }
    }

    return files;
  }


  public static void printUsage() {
    Out.println(""); //$NON-NLS-1$
    Out.println("Usage: csflex <options> <input-files>");
    Out.println("");
    Out.println("Where <options> can be one or more of");
    Out.println("-d <directory>   write generated file to <directory>");
    Out.println("--skel <file>    use external skeleton <file>");
    Out.println("--switch");
    Out.println("--table");
    Out.println("--pack           set default code generation method");
    Out.println("--jlex           strict JLex compatibility");
    Out.println("--nomin          skip minimization step");
    Out.println("--nobak          don't create backup files");
    Out.println("--dump           display transition tables"); 
    Out.println("--dot            write graphviz .dot files for the generated automata (alpha)");
    Out.println("--nested-default-skeleton");
    Out.println("-nested          use the skeleton with support for nesting (included files)");
    Out.println("--csharp         ***");
    Out.println("-csharp          * Important: Enable C# code generation");
    Out.println("--verbose        ***");
    Out.println("-v               display generation progress messages (default)");
    Out.println("--quiet");
    Out.println("-q               display errors only");
    Out.println("--time           display generation time statistics");
    Out.println("--version        print the version number of this copy of C# Flex");
    Out.println("--info           print system + JDK information");
    Out.println("--help");
    Out.println("-h               print this message");
    Out.println("");
    Out.println(ErrorMessages.THIS_IS_CSFLEX, version); 
    Out.println("Have a nice day!");
  }

  [System.Runtime.InteropServices.DllImport("kernel32")]
  private static extern bool FreeConsole();

  public static void generate(String[] argv) {
    ArrayList files = parseOptions(argv);

    if (files.Count > 0) {
      for (int i = 0; i < files.Count; i++)
        generate((File) files[i]);
    }
    else
        printUsage();
   /*
    else {
      FreeConsole();
      try
      {
        Application.Run(new MainFrame());
      }
      catch {}
    }    
    */
  }


  /**
   * Starts the generation process with the files in <code>argv</code> or
   * pops up a window to choose a file, when <code>argv</code> doesn't have
   * any file entries.
   *
   * @param argv the commandline.
   */
  public static void Main(String[] argv) {
    try {
      generate(argv);
    }
    catch (GeneratorException) {
      Out.statistics();
      Environment.Exit(1);
    }
    catch (SilentExit) {
      Environment.Exit(1);
    }
  }
}
}