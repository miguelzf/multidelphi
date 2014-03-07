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
 * Stores all rules of the specification for later access in RegExp -> NFA
 *
 * @author Gerwin Klein
 * @version JFlex 1.4, $Revision: 2.3 $, $Date: 2004/04/12 10:07:48 $
 * @author Jonathan Gilbert
 * @version CSFlex 1.4
 */
public class RegExps {
  
  /** the spec line in which a regexp is used */
  ArrayList /* of Integer */ lines;

  /** the lexical states in wich the regexp is used */
  ArrayList /* of ArrayList of Integer */ states;

  /** the regexp */
  ArrayList /* of RegExp */ regExps;

  /** the action of a regexp */
  ArrayList /* of Action */ actions;
  
  /** flag if it is a BOL regexp */
  ArrayList /* of Boolean */ BOL;

  /** the lookahead expression */
  ArrayList /* of RegExp */ look;

  public RegExps() {
    states = new PrettyArrayList();
    regExps = new PrettyArrayList();
    actions = new PrettyArrayList();
    BOL = new PrettyArrayList();
    look = new PrettyArrayList();
    lines = new PrettyArrayList();
  }

  public int insert(int line, ArrayList stateList, RegExp regExp, Action action, 
                     Boolean isBOL, RegExp lookAhead) {      
    if (Options.DEBUG) {
      Out.debug("Inserting regular expression with statelist :"+Out.NL+stateList);  //$NON-NLS-1$
      Out.debug("and action code :"+Out.NL+action.content+Out.NL);     //$NON-NLS-1$
      Out.debug("expression :"+Out.NL+regExp);  //$NON-NLS-1$
    }

    states.Add(stateList);
    regExps.Add(regExp);
    actions.Add(action);
    BOL.Add(isBOL);
    look.Add(lookAhead);
    lines.Add(line);
    
    return states.Count-1;
  }

  public int insert(ArrayList stateList, Action action) {

    if (Options.DEBUG) {
      Out.debug("Inserting eofrule with statelist :"+Out.NL+stateList);   //$NON-NLS-1$
      Out.debug("and action code :"+Out.NL+action.content+Out.NL);      //$NON-NLS-1$
    }

    states.Add(stateList);
    regExps.Add(null);
    actions.Add(action);
    BOL.Add(null);
    look.Add(null);
    lines.Add(null);
    
    return states.Count-1;
  }

  public void addStates(int regNum, ArrayList newStates) {
    IEnumerator s = newStates.GetEnumerator();
    
    while (s.MoveNext()) 
      ((ArrayList)states[regNum]).Add(s.Current);      
  }

  public int getNum() {
    return states.Count;
  }

  public bool isBOL(int num) {
    return ((Boolean) BOL[num]).booleanValue();
  }
  
  public RegExp getLookAhead(int num) {
    return (RegExp) look[num];
  }

  public bool isEOF(int num) {
    return BOL[num] == null;
  }

  public ArrayList getStates(int num) {
    return (ArrayList) states[num];
  }

  public RegExp getRegExp(int num) {
    return (RegExp) regExps[num];
  }

  public int getLine(int num) {
    return ((Integer) lines[num]).intValue();
  }

  public void checkActions() {
    if ( actions[actions.Count-1] == null ) {
      Out.error(ErrorMessages.NO_LAST_ACTION);
      throw new GeneratorException();
    }
  }

  public Action getAction(int num) {
    while ( num < actions.Count && actions[num] == null )
      num++;

    return (Action) actions[num];
  }

  public int NFASize(Macros macros) {
    int size = 0;
    IEnumerator e = regExps.GetEnumerator();
    while (e.MoveNext()) {
      RegExp r = (RegExp) e.Current;
      if (r != null) size += r.size(macros);
    }
    e = look.GetEnumerator();
    while (e.MoveNext()) {
      RegExp r = (RegExp) e.Current;
      if (r != null) size += r.size(macros);
    }
    return size;
  }
}
}