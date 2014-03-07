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

namespace CSFlex
{

/**
 * Simple symbol table, mapping lexical state names to integers. 
 *
 * @author Gerwin Klein
 * @version JFlex 1.4, $Revision: 2.1 $, $Date: 2004/04/12 10:07:48 $
 * @author Jonathan Gilbert
 * @version CSFlex 1.4
 */
public class LexicalStates {
  
  /** maps state name to state number */
  Hashtable states; 

  /** codes of inclusive states (subset of states) */
  ArrayList inclusive;

  /** number of declared states */
  int numStates;


  /**
   * constructs a new lexical state symbol table
   */
  public LexicalStates() {
    states = new PrettyHashtable();
    inclusive = new PrettyArrayList();
  }

  
  /**
   * insert a new state declaration
   */
  public void insert(String name, bool is_inclusive) {
    if ( states.ContainsKey(name) ) return;

    Integer code = new Integer(numStates++);
    states[name] = code;

    if (is_inclusive) 
      inclusive.Add(code.intValue());
  }


  /**
   * returns the number (code) of a declared state, 
   * <code>null</code> if no such state has been declared.
   */
  public Integer getNumber(String name) {
    return (Integer)states[name];
  }

  
  /**
   * returns the number of declared states
   */
  public int number() {
    return numStates;
  }

  
  /**
   * returns the names of all states
   */
  public IEnumerator names() {
    return states.Keys.GetEnumerator();
  }

  /**
   * returns the code of all inclusive states
   */
  public IEnumerator getInclusiveStates() {
    return inclusive.GetEnumerator();
  }
}
}