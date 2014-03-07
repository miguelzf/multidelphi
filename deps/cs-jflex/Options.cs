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

namespace CSFlex
{

/**
 * Collects all global C# Flex options. Can be set from command line parser,
 * ant taks, gui, etc.
 * 
 * @author Gerwin Klein
 * @version JFlex 1.4, $Revision: 2.9 $, $Date: 2004/04/12 10:07:47 $
 * @author Jonathan Gilbert
 * @version CSFlex 1.4
 */
public class Options {

  /** If true, additional verbose debug information is produced
   *  This is a compile time option */
  public static readonly bool DEBUG = false;

	/** code generation method: maximum packing */
	public const int PACK   = 0;
	/** code generation method: traditional */
	public const int TABLE  = 1;
	/** code generation method: switch statement */
	public const int SWITCH = 2;

	// Miguel: disable generation of #line's by default
	public static bool emitlines = true;

	/** output directory */
	private static File directory;
  /** strict JLex compatibility */
  public static bool jlex;
  /** don't run minimization algorithm if this is true */
  public static bool no_minimize; 
  /** don't write backup files if this is true */
  public static bool no_backup; 
  /** default code generation method */
  public static int gen_method;
  /** If false, only error/warning output will be generated */
  public static bool verbose;
  /** If true, progress dots will be printed */
  public static bool progress;
  /** If true, C# Flex will print time statistics about the generation process */
  public static bool time;
  /** If true, C# Flex will write graphviz .dot files for generated automata */
  public static bool dot;
  /** If true, you will be flooded with information (e.g. dfa tables).  */
  public static bool dump;
  /** If true, the output will be C# code instead of Java.  */
  public static bool emit_csharp;

	static Options() { setDefaults();	}


  /**
   * @return the output directory
   */
  public static File getDir() {    
    return directory;
  }

  /**
   * Set output directory
   * 
   * @param dirName the name of the directory to write output files to
   */
  public static void setDir(String dirName) {
  	setDir(new File(dirName)); 
  }
  

	/**
	 * Set output directory
	 * 
	 * @param d  the directory to write output files to
	 */
  public static void setDir(File d) {
    if ( d.isFile() ) {
      Out.error("Error: \""+d+"\" is not a directory.");
      throw new GeneratorException();
    }
    
    if ( !d.isDirectory() && !d.mkdirs() ) {
      Out.error("Error: couldn't create directory \""+d+"\"");
      throw new GeneratorException();
    }
  
    directory = d;
  }

  /**
   * Sets all options back to default values. 
   */
  public static void setDefaults() {
  	directory = null;
    jlex = false;
		no_minimize = false;
		no_backup = false;
		gen_method = Options.PACK;    
    verbose = true;
    progress = false;	// Miguel: do not print dots
    time = false;
    dot = false;
    dump = false;
  }

  public static void setSkeleton(File skel) {
    if (((string)skel) == "<nested>")
      Skeleton.readNested();
    else
      Skeleton.readSkelFile(skel);
  }   
}
}