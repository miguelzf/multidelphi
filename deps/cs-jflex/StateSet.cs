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
 * A set of NFA states (= integers). 
 *
 * Very similar to java.util.BitSet, but is faster and doesn't crash
 *
 * @author Gerwin Klein
 * @version JFlex 1.4, $Revision: 2.2 $, $Date: 2004/04/12 10:07:48 $
 * @author Jonathan Gilbert
 * @version CSFlex 1.4
 */
public sealed class StateSet {

  private readonly bool DEBUG = false;

  public static readonly StateSet EMPTY = new StateSet();


  internal const int BITS = 6;
  internal const int MASK = (1<<BITS)-1;
  
  internal long[] bits;
  
  
  public StateSet() : this(256) {
  }  

  public StateSet(int size) {    
    bits = new long[size2nbits(size)];
  }

  public StateSet(int size, int state) : this(size) {
    addState(state);
  }

  public StateSet(StateSet set) {
    bits = new long[set.bits.Length];
    Array.Copy(set.bits, 0, bits, 0, set.bits.Length);
  }


  public void addState(int state) {
    if (DEBUG) {
      Out.dump("StateSet.addState("+state+") start"); //$NON-NLS-1$ //$NON-NLS-2$
      Out.dump("Set is : "+this); //$NON-NLS-1$
    }
   
    int index = state >> BITS;
    if (index >= bits.Length) resize(state);
    bits[index] |= (1L << (state & MASK));
    
    if (DEBUG) {
      Out.dump("StateSet.addState("+state+") end"); //$NON-NLS-1$ //$NON-NLS-2$
      Out.dump("Set is : "+this); //$NON-NLS-1$
    }
  }


  private int size2nbits (int size) {
    return ((size >> BITS) + 1);
  }


  private void resize(int size) {
    int needed = size2nbits(size);

    // if (needed < bits.length) return;
         
    long[] newbits = new long[Math.Max(bits.Length*4,needed)];
    Array.Copy(bits, 0, newbits, 0, bits.Length);
    
    bits = newbits;
  }


  public void clear() {    
    int l = bits.Length;
    for (int i = 0; i < l; i++) bits[i] = 0;
  }

  public bool isElement(int state) {
    int index = state >> BITS;
    if (index >= bits.Length)  return false;
    return (bits[index] & (1L << (state & MASK))) != 0;
  }

  /**
   * Returns one element of the set and removes it. 
   *
   * Precondition: the set is not empty.
   */
  public int getAndRemoveElement() {
    int i = 0;
    int o = 0;
    long m = 1;
    
    while (bits[i] == 0) i++;
    
    while ( (bits[i] & m) == 0 ) {
      m<<= 1;
      o++;
    }
    
    bits[i]&= ~m;
    
    return (i << BITS) + o;
  }

  public void remove(int state) {
    int index = state >> BITS;
    if (index >= bits.Length) return;
    bits[index] &= ~(1L << (state & MASK));
  }

  /**
   * Returns the set of elements that contained are in the specified set
   * but are not contained in this set.
   */
  public StateSet complement(StateSet set) {
    
    if (set == null) return null;
    
    StateSet result = new StateSet();
    
    result.bits = new long[set.bits.Length];
    
    int i;
    int m = Math.Min(bits.Length, set.bits.Length);
    
    for (i = 0; i < m; i++) {
      result.bits[i] = ~bits[i] & set.bits[i];
    }
    
    if (bits.Length < set.bits.Length) 
      Array.Copy(set.bits, m, result.bits, m, result.bits.Length-m);
    
    if (DEBUG) 
      Out.dump("Complement of "+this+Out.NL+"and "+set+Out.NL+" is :"+result); //$NON-NLS-1$ //$NON-NLS-2$ //$NON-NLS-3$
    
    return result;
  }

  public void add(StateSet set) {
    
    if (DEBUG) Out.dump("StateSet.add("+set+") start"); //$NON-NLS-1$ //$NON-NLS-2$

    if (set == null) return;

    long[] tbits;
    long[] sbits = set.bits;
    int sbitsl = sbits.Length;

    if (bits.Length < sbitsl) {
      tbits = new long[sbitsl];
      Array.Copy(bits, 0, tbits, 0, bits.Length);
    }
    else {
      tbits = this.bits;
    }
    
    for (int i = 0; i < sbitsl; i++) {
      tbits[i] |= sbits[i];      
    }

    this.bits = tbits;
    
    if (DEBUG) {
      Out.dump("StateSet.add("+set+") end"); //$NON-NLS-1$ //$NON-NLS-2$
      Out.dump("Set is : "+this); //$NON-NLS-1$
    }
  }
  


  public bool containsSet(StateSet set) {

    if (DEBUG)
      Out.dump("StateSet.containsSet("+set+"), this="+this); //$NON-NLS-1$ //$NON-NLS-2$

    int i;
    int min = Math.Min(bits.Length, set.bits.Length);
    
    for (i = 0; i < min; i++) 
      if ( (bits[i] & set.bits[i]) != set.bits[i] ) return false;
    
    for (i = min; i < set.bits.Length; i++)
      if ( set.bits[i] != 0 ) return false;
    
    return true;
  }



  /**
   * @throws ClassCastException if b is not a StateSet
   * @throws NullPointerException if b is null
   */
  public override bool Equals(Object b) {

    int i = 0;
    int l1,l2;
    StateSet set = (StateSet) b;

    if (DEBUG) Out.dump("StateSet.equals("+set+"), this="+this); //$NON-NLS-1$ //$NON-NLS-2$

    l1 = bits.Length;
    l2 = set.bits.Length;

    if (l1 <= l2) {      
      while (i < l1) {
        if (bits[i] != set.bits[i]) return false;
        i++;
      }
      
      while (i < l2) 
        if (set.bits[i++] != 0) return false;
    }
    else {
      while (i < l2) {
        if (bits[i] != set.bits[i]) return false;
        i++;
      }
      
      while (i < l1) 
        if (bits[i++] != 0) return false;
    }

    return true;
  }

  public override int GetHashCode() {
    long h = 1234;
    long [] _bits = bits;
    int  i = bits.Length-1;

    // ignore zero high bits
    while (i >= 0 && _bits[i] == 0) i--;

    while (i >= 0)
      h ^= _bits[i--] * i;

    return (int)((h >> 32) ^ h);
  } 


  public StateSetEnumerator states() {
    return new StateSetEnumerator(this);
  }


  public bool containsElements() {
    for (int i = 0; i < bits.Length; i++)
      if (bits[i] != 0) return true;
      
    return false;
  }

  
  public StateSet copy() {
    StateSet set = new StateSet();
    set.bits = new long[bits.Length];
    Array.Copy(bits, 0, set.bits, 0, bits.Length);
    return set;
  }


  public void copy(StateSet set) {
    
    if (DEBUG) 
      Out.dump("StateSet.copy("+set+") start"); //$NON-NLS-1$ //$NON-NLS-2$

    if (set == null) {
      for (int i = 0; i < bits.Length; i++) bits[i] = 0;
      return;
    }

    if (bits.Length < set.bits.Length) {
      bits = new long[set.bits.Length];
    }
    else {
      for (int i = set.bits.Length; i < bits.Length; i++) bits[i] = 0;
    }

    Array.Copy(set.bits, 0, bits, 0, bits.Length);        

    if (DEBUG) {
      Out.dump("StateSet.copy("+set+") end"); //$NON-NLS-1$ //$NON-NLS-2$
      Out.dump("Set is : "+this); //$NON-NLS-1$
    }
  }

  
  public override String ToString() {
    StateSetEnumerator @enum = states();

    StringBuilder result = new StringBuilder("{"); //$NON-NLS-1$

    if ( @enum.hasMoreElements() ) result.Append(@enum.nextElement()); //$NON-NLS-1$

    while ( @enum.hasMoreElements() ) {
      int i = @enum.nextElement();
      result.Append(", ").Append(i); //$NON-NLS-1$
    }

    result.Append("}"); //$NON-NLS-1$

    return result.ToString();
  }  
}
}