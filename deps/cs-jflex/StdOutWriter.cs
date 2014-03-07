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
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace CSFlex
{


/**
 * Convenience class for C# Flex stdout, redirects output to a RichTextBox
 * if in GUI mode.
 *
 * @author Gerwin Klein
 * @version JFlex 1.4, $Revision: 2.3 $, $Date: 2004/04/12 10:07:48 $
 * @author Jonathan Gilbert
 * @version CSFlex 1.4
 */
public sealed class StdOutWriter: TextWriter {
  
  public override Encoding Encoding
  {
    get
    {
      return Encoding.UTF8;
    }
  }


  /** text area to write to if in gui mode, gui mode = (text != null) */
  private RichTextBox text;

  /** text accumulated to be appended to the textbox */
  private StringBuilder text_queue;

  /** timeout to actually append incoming text to the textbox */
  private System.Threading.Timer tmrTimeout;

  /** 
   * approximation of the current column in the text area
   * for auto wrapping at <code>wrap</code> characters
   **/
  private int col;
 
  /** auto wrap lines in gui mode at this value */
  private const int wrap = 78;

  TextWriter actual_stream;

  /** A StdOutWriter, attached to System.out, no gui mode */
  public StdOutWriter() {
    actual_stream = Console.Out;
  }
  
  /** A StdOutWrite, attached to the specified output stream, no gui mode */
  public StdOutWriter(Stream @out) {
    actual_stream = new StreamWriter(@out, Encoding.UTF8);
  }

  /**
   * Set the RichTextBox to write text to. Will continue
   * to write to System.out if text is <code>null</code>.
   *
   * @param text  the RichTextBox to write to
   */
  public void setGUIMode(RichTextBox text) {
    this.text = text;
  }

  delegate void SimpleDelegate();

  object sync_root = new object();

  void append_string_in_gui_mode(string str)
  {
    lock (sync_root)
    {
      if (text_queue == null)
      {
        text_queue = new StringBuilder();
        tmrTimeout = new System.Threading.Timer(new TimerCallback(tmrTimeout_callback), null, 300, Timeout.Infinite);
      }

      text_queue.Append(str);
    }
  }

  private void tmrTimeout_callback(object sender)
  {
    if (text_queue != null)
      text.Invoke(new SimpleDelegate(append_string_in_gui_mode_proxy));
  }

  void append_string_in_gui_mode_proxy()
  {
    lock (sync_root)
    {
      text.AppendText(text_queue.ToString());
      text.SelectionStart = text.TextLength;

      text_queue = null;

      tmrTimeout.Dispose();
      tmrTimeout = null;
    }
  }

  /** Write a single character. */
  public override void Write(char c)
  {
    if (text != null) {
      append_string_in_gui_mode(c.ToString());
      if (++col > wrap) WriteLine();
    }
    else
      actual_stream.Write(c);
  }

  /** Write a portion of an array of characters. */
  public override void Write(char[] buf, int off, int len)
  {
    if (text != null) {
      append_string_in_gui_mode(new String(buf,off,len));
      if ((col+=len) > wrap) WriteLine();
    }
    else
      actual_stream.Write(buf, off, len);
  }
  
  /** Write a portion of a string. */
  public void Write(String s, int off, int len) {
    if (text != null) {
      append_string_in_gui_mode(s.Substring(off,len));
      if ((col+=len) > wrap) WriteLine();
    }
    else {
      actual_stream.Write(s.Substring(off,len)); 
      Flush();
    }
  }

  /**
   * Begin a new line. Which actual character/s is/are written 
   * depends on the runtime platform.
   */
  public override void WriteLine()
  {
    if (text != null) {
      append_string_in_gui_mode(Environment.NewLine);
      col = 0;
    }
    else
      actual_stream.WriteLine();
  }

  public override void WriteLine(string str)
  {
    Write(str, 0, str.Length);
    WriteLine();
  }
}
}