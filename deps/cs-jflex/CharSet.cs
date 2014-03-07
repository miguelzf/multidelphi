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
 * 
 * @author Gerwin Klein 
 * @version JFlex 1.4, $Revision: 2.1 $, $Date: 2004/04/12 10:07:47 $ 
 * @author Jonathan Gilbert
 * @version CSFlex 1.4
 */
public sealed class CharSet {

  internal const int BITS = 6;           // the number of bits to shift (2^6 = 64)
  internal const int MOD = (1<<BITS)-1;  // modulus
  
  internal long[] bits;

  private int numElements;
  
  
  public CharSet() {
    bits = new long[1];
  }


  public CharSet(int initialSize, int character) {
    bits = new long[(initialSize >> BITS)+1];
    add(character);
  }


  public void add(int character) {
    resize(character);

    if ( (bits[character >> BITS] & (1L << (character & MOD))) == 0) numElements++;

    bits[character >> BITS] |= (1L << (character & MOD));    
  }


  private int nbits2size (int nbits) {
    return ((nbits >> BITS) + 1);
  }


  private void resize(int nbits) {
    int needed = nbits2size(nbits);

    if (needed < bits.Length) return;
         
    long[] newbits = new long[Math.Max(bits.Length*2,needed)];
    Array.Copy(bits, 0, newbits, 0, bits.Length);
    
    bits = newbits;
  }


  public bool isElement(int character) {
    int index = character >> BITS;
    if (index >= bits.Length)  return false;
    return (bits[index] & (1L << (character & MOD))) != 0;
  }


  public CharSetEnumerator characters() {
    return new CharSetEnumerator(this);
  }


  public bool containsElements() {
    return numElements > 0;
  }

  public int size() {
    return numElements;
  }
  
  public override String ToString() {
    CharSetEnumerator @enum = characters();

    StringBuilder result = new StringBuilder("{");

    if ( @enum.hasMoreElements() ) result.Append(@enum.nextElement());

    while ( @enum.hasMoreElements() ) {
      int i = @enum.nextElement();
      result.Append(", ").Append(i);
    }

    result.Append("}");

    return result.ToString();
  }
}
}