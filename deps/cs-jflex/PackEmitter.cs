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
using System.Text;

namespace CSFlex
{


/**
 * Encodes <code>int</code> arrays as strings.
 * 
 * Also splits up strings when longer than 64K in UTF8 encoding.
 * Subclasses emit unpacking code.
 * 
 * Usage protocol:
 * <code>p.emitInit();</code><br>
 * <code>for each data: p.emitData(data);</code><br>
 * <code>p.emitUnpack();</code> 
 * 
 * @author Gerwin Klein
 * @version $Revision: 1.6 $, $Date: 2004/04/12 10:07:47 $
 */
public abstract class PackEmitter {

  /** name of the generated array (mixed case, no yy prefix) */
  protected String name;
    
  /** current UTF8 length of generated string in current chunk */
  private int _UTF8Length;

  /** position in the current line */
  private int linepos;
  
  /** max number of entries per line */
  private const int maxEntries = 16;
  
  /** output buffer */
  protected StringBuilder @out = new StringBuilder();

  /** number of existing string chunks */ 
  protected int chunks;
    
  /** maximum size of chunks */
  // String constants are stored as UTF8 with 2 bytes length
  // field in class files. One Unicode char can be up to 3 
  // UTF8 bytes. 64K max and two chars safety. 
  private const int maxSize = 0xFFFF-6;
  
  /** indent for string lines */
  private const String indent = "    ";
  private const String csharp_indent = "   ";
  
  /**
   * Create new emitter for an array.
   * 
   * @param name  the name of the generated array
   */
  public PackEmitter(String name) {
    this.name = name;
  }
  
  /**
   * Convert array name into all uppercase internal scanner 
   * constant name.
   * 
   * @return <code>name</code> as a internal constant name.
   * @see PackEmitter#name
   */
  protected String constName() {
    return "ZZ_" + name.ToUpper();
  }
  
  /**
   * Return current output buffer.
   */
  public override String ToString() {
    return @out.ToString();
  }

  /**
   * Emit declaration of decoded member and open first chunk.
   */  
  public void emitInit() {
    if (Options.emit_csharp)
    {
      @out.Append("  private static readonly int [] ");
      @out.Append(constName());
      @out.Append(";");
    }
    else
    {
      @out.Append("  private static final int [] ");
      @out.Append(constName());
      @out.Append(" = zzUnpack");
      @out.Append(name);
      @out.Append("();");
    }
    nl();
    nextChunk();
  }

  /**
   * Emit single unicode character. 
   * 
   * Updates length, position, etc.
   *
   * @param i  the character to emit.
   * @prec  0 <= i <= 0xFFFF 
   */   
  public void emitUC(int i) {     
    if (i < 0 || i > 0xFFFF) 
      throw new ArgumentException("character value expected", "i");
  
    // cast ok because of prec  
    char c = (char) i;    

    printUC(c);
    _UTF8Length += UTF8Length(c);
    linepos++;   
  }

  /**
   * Execute line/chunk break if necessary. 
   * Leave space for at least two chars.
   */  
  public void breaks() {
    if (_UTF8Length >= maxSize) {
      // close current chunk
      if (Options.emit_csharp)
        @out.Append("0 };");
      else
        @out.Append("\";");
      nl();
      
      nextChunk();
    }
    else {
      if (linepos >= maxEntries) {
        // line break
        if (Options.emit_csharp)
        {
          nl();
          @out.Append(csharp_indent);
        }
        else
        {
          @out.Append("\"+");
          nl();
          @out.Append(indent);
          @out.Append("\"");
        }
        linepos = 0;      
      }
    }
  }
  
  /**
   * Emit the unpacking code. 
   */
  public abstract void emitUnpack();

  /**
   *  emit next chunk 
   */
  private void nextChunk() {
    if (Options.emit_csharp)
    {
      nl();
      @out.Append("  private static readonly ushort[] ");
      @out.Append(constName());
      @out.Append("_PACKED_");
      @out.Append(chunks);
      @out.Append(" = new ushort[] {");
      nl();
      @out.Append(csharp_indent);
    }
    else
    {
      nl();
      @out.Append("  private static final String ");
      @out.Append(constName());
      @out.Append("_PACKED_");
      @out.Append(chunks);
      @out.Append(" =");
      nl();
      @out.Append(indent);
      @out.Append("\"");
    }

    _UTF8Length = 0;
    linepos = 0;
    chunks++;
  }
  
  /**
   *  emit newline 
   */
  protected void nl() {
    @out.Append(Out.NL);
  }
  
  /**
   * Append a unicode/octal escaped character 
   * to <code>out</code> buffer.
   * 
   * @param c the character to append
   */
  private void printUC(char c) {
    if (Options.emit_csharp)
      @out.Append(" ");

    if (c > 255) 
    {
      if (Options.emit_csharp)
      {
        @out.Append("0x");
        if (c < 0x1000) @out.Append("0");
        @out.Append(Integer.toHexString(c));
      }
      else
      {
        @out.Append("\\u");
        if (c < 0x1000) @out.Append("0");
        @out.Append(Integer.toHexString(c));
      }
    }
    else 
    {
      if (Options.emit_csharp)
        @out.Append(((int)c).ToString());
      else
      {
        @out.Append("\\");
        @out.Append(Integer.toOctalString(c));
      }
    }    

    if (Options.emit_csharp)
      @out.Append(",");
  } 

  /**
   * Calculates the number of bytes a Unicode character
   * would have in UTF8 representation in a class file.
   *
   * @param value  the char code of the Unicode character
   * @prec  0 <= value <= 0xFFFF
   *
   * @return length of UTF8 representation.
   */
  private int UTF8Length(char value) {
    // if (value < 0 || value > 0xFFFF) throw new Error("not a char value ("+value+")");

    // see JVM spec §4.4.7, p 111
    if (value == 0) return 2;
    if (value <= 0x7F) return 1;

    // workaround for javac bug (up to jdk 1.3):
    if (value <  0x0400) return 2;
    if (value <= 0x07FF) return 3;

    // correct would be:
    // if (value <= 0x7FF) return 2;
    return 3;
  }

  // convenience
  protected void println(String s) {
    @out.Append(s);
    nl();
  }
}
}