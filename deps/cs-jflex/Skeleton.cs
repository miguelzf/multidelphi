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
using System.Reflection;
using System.Text;

namespace CSFlex
{

/**
 * This class stores the skeleton of generated scanners.
 *
 * The skeleton consists of several parts that can be emitted to
 * a file. Usually there is a portion of generated code
 * (produced in class Emitter) between every two parts of skeleton code.
 *
 * There is a static part (the skeleton code) and state based iterator
 * part to this class. The iterator part is used to emit consecutive skeleton
 * sections to some <code>PrintWriter</code>. 
 *
 * @see CSFlex.Emitter
 *
 * @author Gerwin Klein
 * @version JFlex 1.4, $Revision: 2.12 $, $Date: 2004/04/12 10:07:47 $
 * @author Jonathan Gilbert
 * @version CSFlex 1.4
 */
public class Skeleton {
  /** expected number of sections in the skeleton file */
  private const int size = 21;

  /** platform specific newline */
  private static readonly String NL = Environment.NewLine;  //$NON-NLS-1$

  /** The skeleton */  
  public static String[] line;

  /** Whether the skeleton is C#-capable */
  private static bool isCSharpSkeleton;
  private static bool notCSharpSkeletonWarned;
  
  /** initialization */   
  static Skeleton() { readDefault(); }  
  
  // the state based, iterator part of Skeleton:

  /**
   * The current part of the skeleton (an index of nextStop[])
   */
  private int pos;  
  
  /**
   * The writer to write the skeleton-parts to
   */
  private TextWriter @out;


  /**
   * Creates a new skeleton (iterator) instance. 
   *
   * @param   out  the writer to write the skeleton-parts to
   */
  public Skeleton(TextWriter @out) {
    this.@out = @out;
  }


  /**
   * Emits the next part of the skeleton
   */
  public void emitNext() {
    if (isCSharpSkeleton)
    {
      if (Options.emit_csharp)
      {
        pos++;
        @out.Write( line[pos++] );
      }
      else
      {
        @out.Write( line[pos++] );
        pos++;
      }
    }
    else
    {
      if (Options.emit_csharp && !notCSharpSkeletonWarned)
      {
        Out.warning(ErrorMessages.get(ErrorMessages.NOT_CSHARP_SKELETON));
        notCSharpSkeletonWarned = true;
      }

      @out.Write( line[pos++] );
    }
  }


  /**
   * Make the skeleton private.
   *
   * Replaces all occurences of " public " in the skeleton with " private ". 
   */
  public static void makePrivate() {
    for (int i=0; i < line.Length; i++) {
      line[i] = replace(" public ", " private ", line[i]);   //$NON-NLS-1$ //$NON-NLS-2$
    }
  } 


  /**
   * Reads an external skeleton file for later use with this class.
   * 
   * @param skeletonFile  the file to read (must be != null and readable)
   */
  public static void readSkelFile(string skeletonFile) {
    if (skeletonFile == null)
      throw new ArgumentException("Skeleton file must not be null", "skeletonFile"); //$NON-NLS-1$

    try
    {
      FileStream stream = new FileStream(skeletonFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
      stream.Close();
    }
    catch
    {
      Out.error(ErrorMessages.CANNOT_READ_SKEL, skeletonFile.ToString()); 
      throw new GeneratorException();
    }

    Out.println(ErrorMessages.READING_SKEL, skeletonFile.ToString());

    StreamReader reader = null;
    try 
    {
      reader = new StreamReader(skeletonFile, Encoding.UTF8, true);
      readSkel(reader);
    }
    catch (IOException) 
    {
      Out.error(ErrorMessages.SKEL_IO_ERROR); 
      throw new GeneratorException();
    }
    finally
    {
      if (reader != null)
        reader.Close();
    }
  }


  /**
   * Reads an external skeleton file from a BufferedReader.
   * 
   * @param  reader             the reader to read from (must be != null)
   * @throws IOException        if an IO error occurs
   * @throws GeneratorException if the number of skeleton sections does not match 
   */
  public static void readSkel(TextReader reader) {
    isCSharpSkeleton = false;
    notCSharpSkeletonWarned = false;

    ArrayList lines = new PrettyArrayList();
    StringBuilder section = new StringBuilder();

    String ln;
    while ((ln = reader.ReadLine()) != null) {
      if (ln.StartsWith("---")) { //$NON-NLS-1$
        lines.Add(section.ToString());
        section.Length = 0;
      } else {
        section.Append(ln);
        section.Append(NL);
      }
    }

    if (section.Length > 0)
      lines.Add(section.ToString());

    if (lines.Count != size) {
      if (lines.Count == size * 2) 
        isCSharpSkeleton = true;
      else
      {
        Out.error(ErrorMessages.WRONG_SKELETON);
        throw new GeneratorException();
      }
    }

    line = new String[lines.Count];
    for (int i = 0; i < lines.Count; i++)
      line[i] = (String) lines[i];
  }
  
  /**
   * Replaces a with b in c.
   * 
   * @param a  the String to be replaced
   * @param b  the replacement
   * @param c  the String in which to replace a by b
   * @return a String object with a replaced by b in c 
   */
  public static String replace(String a, String b, String c) {
    return c.Replace(a, b);
  }

  
  /**
   * (Re)load the default skeleton. Looks in the current system class path.   
   */
  public static void readDefault() {
    try
    {
      Assembly assembly = typeof(Skeleton).Assembly;
      Stream stream = assembly.GetManifestResourceStream("csflex.skeleton.default");
      readSkel(new StreamReader(stream));
    } catch {
      Out.error(ErrorMessages.SKEL_IO_ERROR_DEFAULT); 
      throw new GeneratorException();
    }
  }

  public static void readNested() 
  {
    try
    {
      Assembly assembly = typeof(Skeleton).Assembly;
      Stream stream = assembly.GetManifestResourceStream("csflex.skeleton.nested");
      Out.println(ErrorMessages.READING_SKEL, "skeleton.nested");
      readSkel(new StreamReader(stream));
    } 
    catch 
    {
      Out.error(ErrorMessages.SKEL_IO_ERROR_DEFAULT); 
      throw new GeneratorException();
    }
  }
}
}