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
using System.Text;

namespace CSFlex
{

/**
 *
 * @author Gerwin Klein
 * @version JFlex 1.4, $Revision: 2.5 $, $Date: 2004/04/12 10:07:47 $
 * @author Jonathan Gilbert
 * @version CSFlex 1.4
 */
public class CharClasses {

  /** debug flag (for char classes only) */
  private static readonly bool DEBUG = false;

  /** the largest character that can be used in char classes */
  public const char maxChar = '\uFFFF';

  /** the char classes */
  private ArrayList /* of IntCharSet */ classes;

  /** the largest character actually used in a specification */
  private char maxCharUsed;

  /**
   * Constructs a new CharClass object that provides space for 
   * classes of characters from 0 to maxCharCode.
   *
   * Initially all characters are in class 0.
   *
   * @param maxCharCode the last character code to be
   *                    considered. (127 for 7bit Lexers, 
   *                    255 for 8bit Lexers and 0xFFFF
   *                    for Unicode Lexers).
   */
  public CharClasses(int maxCharCode) {
    if (maxCharCode < 0 || maxCharCode > 0xFFFF) 
      throw new ArgumentException();

    maxCharUsed = (char) maxCharCode;

    classes = new PrettyArrayList();
    classes.Add(new IntCharSet(new Interval((char) 0, maxChar)));
  }


  /**
   * Returns the greatest Unicode value of the current input character set.
   */
  public char getMaxCharCode() {
    return maxCharUsed;
  }
  

  /**
   * Sets the larges Unicode value of the current input character set.
   *
   * @param charCode   the largest character code, used for the scanner 
   *                   (i.e. %7bit, %8bit, %16bit etc.)
   */
  public void setMaxCharCode(int charCode) {
    if (charCode < 0 || charCode > 0xFFFF) 
      throw new ArgumentException();

    maxCharUsed = (char) charCode;
  }
  

  /**
   * Returns the current number of character classes.
   */
  public int getNumClasses() {
    return classes.Count;
  }



  /**
   * Updates the current partition, so that the specified set of characters
   * gets a new character class.
   *
   * Characters that are elements of <code>set</code> are not in the same
   * equivalence class with characters that are not elements of <code>set</code>.
   *
   * @param set       the set of characters to distinguish from the rest    
   * @param caseless  if true upper/lower/title case are considered equivalent  
   */
  public void makeClass(IntCharSet set, bool caseless) {
    if (caseless) set = set.getCaseless();
    
    if ( DEBUG ) {
      Out.dump("makeClass("+set+")");
      dump();
    }

    try
    {
      int oldSize = classes.Count;
      for (int i = 0; i < oldSize; i++) 
      {
        IntCharSet x  = (IntCharSet) classes[i];

        if (x.Equals(set)) return;

        IntCharSet and = x.and(set);

        if ( and.containsElements() ) 
        {
          if ( x.Equals(and) ) 
          {          
            set.sub(and);
            continue;
          }
          else if ( set.Equals(and) ) 
          {
            x.sub(and);
            classes.Add(and);
            return;
          }

          set.sub(and);
          x.sub(and);
          classes.Add(and);
        }
      }
    }
    finally
    {
      if (DEBUG) 
      {
        Out.dump("makeClass(..) finished");
        dump();
      }
    }
  }
  

  /**
   * Returns the code of the character class the specified character belongs to.
   */
  public int getClassCode(char letter) {
    int i = -1;
    while (true) {
      IntCharSet x = (IntCharSet) classes[++i];
      if ( x.contains(letter) ) return i;      
    }
  }

  /**
   * Dump charclasses to the dump output stream
   */
  public void dump() {
    Out.dump(ToString());
  }  

  
  /**
   * Return a string representation of one char class
   *
   * @param theClass  the index of the class to
   */
  public String ToString(int theClass) {
    return classes[theClass].ToString();
  }


  /**
   * Return a string representation of the char classes
   * stored in this class. 
   *
   * Enumerates the classes by index.
   */
  public override String ToString() {
    StringBuilder result = new StringBuilder("CharClasses:");

    result.Append(Out.NL);

    for (int i = 0; i < classes.Count; i++) 
      result.AppendFormat("class {1}:{0}{2}{0}", Out.NL, i, classes[i]);

    return result.ToString();
  }

  
  /**
   * Creates a new character class for the single character <code>singleChar</code>.
   *    
   * @param caseless  if true upper/lower/title case are considered equivalent  
   */
  public void makeClass(char singleChar, bool caseless) {
    makeClass(new IntCharSet(singleChar), caseless);
  }


  /**
   * Creates a new character class for each character of the specified String.
   *    
   * @param caseless  if true upper/lower/title case are considered equivalent  
   */
  public void makeClass(String str, bool caseless) {
    for (int i = 0; i < str.Length; i++) makeClass(str[i], caseless);
  }  


  /**
   * Updates the current partition, so that the specified set of characters
   * gets a new character class.
   *
   * Characters that are elements of the set <code>v</code> are not in the same
   * equivalence class with characters that are not elements of the set <code>v</code>.
   *
   * @param v   a Vector of Interval objects. 
   *            This Vector represents a set of characters. The set of characters is
   *            the union of all intervalls in the Vector.
   *    
   * @param caseless  if true upper/lower/title case are considered equivalent  
   */
  public void makeClass(ArrayList /* Interval */ v, bool caseless) {
    makeClass(new IntCharSet(v), caseless);
  }
  

  /**
   * Updates the current partition, so that the set of all characters not contained in the specified 
   * set of characters gets a new character class.
   *
   * Characters that are elements of the set <code>v</code> are not in the same
   * equivalence class with characters that are not elements of the set <code>v</code>.
   *
   * This method is equivalent to <code>makeClass(v)</code>
   * 
   * @param v   a Vector of Interval objects. 
   *            This Vector represents a set of characters. The set of characters is
   *            the union of all intervalls in the Vector.
   * 
   * @param caseless  if true upper/lower/title case are considered equivalent  
   */
  public void makeClassNot(ArrayList v, bool caseless) {
    makeClass(new IntCharSet(v), caseless);
  }


  /**
   * Returns an array that contains the character class codes of all characters
   * in the specified set of input characters.
   */
  private int [] getClassCodes(IntCharSet set, bool negate) {

    if (DEBUG) {
      Out.dump("getting class codes for "+set);
      if (negate)
        Out.dump("[negated]");
    }

    int size = classes.Count;

    // [fixme: optimize]
    int[] temp = new int [size];
    int length  = 0;

    for (int i = 0; i < size; i++) {
      IntCharSet x = (IntCharSet) classes[i];
      if ( negate ) {
        if ( !set.and(x).containsElements() ) {
          temp[length++] = i;
          if (DEBUG) Out.dump("code "+i);
        }
      }
      else {
        if ( set.and(x).containsElements() ) {
          temp[length++] = i;
          if (DEBUG) Out.dump("code "+i);
        }
      }
    }

    int[] result = new int [length];
    Array.Copy(temp, 0, result, 0, length);
    
    return result;
  }


  /**
   * Returns an array that contains the character class codes of all characters
   * in the specified set of input characters.
   * 
   * @param intervallVec   a Vector of Intervalls, the set of characters to get
   *                       the class codes for
   *
   * @return an array with the class codes for intervallVec
   */
  public int [] getClassCodes(ArrayList /* Interval */ intervallVec) {
    return getClassCodes(new IntCharSet(intervallVec), false);
  }


  /**
   * Returns an array that contains the character class codes of all characters
   * that are <strong>not</strong> in the specified set of input characters.
   * 
   * @param intervallVec   a Vector of Intervalls, the complement of the
   *                       set of characters to get the class codes for
   *
   * @return an array with the class codes for the complement of intervallVec
   */
  public int [] getNotClassCodes(ArrayList /* Interval */ intervallVec) {
    return getClassCodes(new IntCharSet(intervallVec), true);
  }


  /**
   * Check consistency of the stored classes [debug].
   *
   * all classes must be disjoint, checks if all characters
   * have a class assigned.
   */
  public void check() {
    for (int i = 0; i < classes.Count; i++)
      for (int j = i+1; j < classes.Count; j++) {
        IntCharSet x = (IntCharSet) classes[i];
        IntCharSet y = (IntCharSet) classes[j];
        if ( x.and(y).containsElements() ) {
          Console.WriteLine("Error: non disjoint char classes {0} and {1}", i, j);
          Console.WriteLine("class {0}: {1}", i, x);
          Console.WriteLine("class {0}: {1}", j, y);
        }
      }

    // check if each character has a classcode 
    // (= if getClassCode terminates)
    for (char c = (char)0; c < maxChar; c++) {
      getClassCode(c);
      if (c % 100 == 0) Console.Write(".");
    }
    
    getClassCode(maxChar);   
  }


  /**
   * Returns an array of all CharClassIntervalls in this
   * char class collection. 
   *
   * The array is ordered by char code, i.e.
   * <code>result[i+1].start = result[i].end+1</code>
   *
   * Each CharClassInterval contains the number of the
   * char class it belongs to.
   */
  public CharClassInterval [] getIntervalls() {
    int i, c;
    int size = classes.Count;
    int numIntervalls = 0;   

    for (i = 0; i < size; i++) 
      numIntervalls+= ((IntCharSet) classes[i]).numIntervalls();    

    CharClassInterval [] result = new CharClassInterval[numIntervalls];
    
    i = 0; 
    c = 0;
    while (i < numIntervalls) {
      int       code = getClassCode((char) c);
      IntCharSet set = (IntCharSet) classes[code];
      Interval  iv  = set.getNext();
      
      result[i++]    = new CharClassInterval(iv.start, iv.end, code);
      c              = iv.end+1;
    }

    return result;
  }
}
}