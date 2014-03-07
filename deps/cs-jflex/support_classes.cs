/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * C# Flex 1.4                                                             *
 * Copyright (C) 2004-2005  Jonathan Gilbert <logic@deltaq.org>            *
 * All rights reserved.                                                    *
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
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CSFlex
{
  public class RuntimeException : Exception
  {
    public RuntimeException()
    {
    }

    public RuntimeException(string msg)
      : base(msg)
    {
    }
  }

  public class File
  {
    string name;

    public File(string parent, string child)
    {
      name = Path.Combine(parent, child);
    }

    public File(string filename)
    {
      name = filename;
    }

    public static implicit operator string(File file)
    {
      return file.name;
    }

    public string getParent()
    {
      string ret = Path.GetDirectoryName(name);

      if (ret.Length == 0)
        return null;
      else
        return ret;
    }

    public bool exists()
    {
      FileInfo info = new FileInfo(name);

      return info.Exists;
    }

    public bool delete()
    {
      FileInfo info = new FileInfo(name);

      try
      {
        info.Delete();
        return true;
      }
      catch
      {
        return false;
      }
    }

    public bool renameTo(File dest)
    {
      FileInfo info = new FileInfo(name);

      try
      {
        info.MoveTo(dest.name);
        return true;
      }
      catch
      {
        return false;
      }
    }

    public static readonly char separatorChar = Path.DirectorySeparatorChar;

    public bool canRead()
    {
      FileStream stream = null;

      try
      {
        stream = new FileStream(name, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        return true;
      }
      catch
      {
        return false;
      }
      finally
      {
        if (stream != null)
          stream.Close();
      }
    }

    public bool isFile()
    {
      FileInfo info = new FileInfo(name);

      return (info.Attributes & FileAttributes.Directory) != FileAttributes.Directory;
    }

    public bool isDirectory()
    {
      FileInfo info = new FileInfo(name);

      return (info.Attributes & FileAttributes.Directory) == FileAttributes.Directory;
    }

    public bool mkdirs()
    {
      FileInfo info = new FileInfo(name);

      Stack needed = new Stack();

      DirectoryInfo parent = info.Directory;

      try
      {
        while (!parent.Exists)
        {
          needed.Push(parent);
          parent = parent.Parent;
        }

        while (needed.Count > 0)
        {
          DirectoryInfo dir = (DirectoryInfo)needed.Pop();
          dir.Create();
        }

        return true;
      }
      catch
      {
        return false;
      }
    }

    public override string ToString()
    {
      return name;
    }

  }

  public class Integer
  {
    int v;

    public Integer(int value)
    {
      v = value;
    }

    public int intValue()
    {
      return v;
    }

    public static string toOctalString(int c)
    {
      StringBuilder ret = new StringBuilder();

      while (c > 0)
      {
        int unit_place = (c & 7);
        c >>= 3;

        ret.Insert(0, (char)(unit_place + '0'));
      }

      if (ret.Length == 0)
        return "0";
      else
        return ret.ToString();
    }

    public static string toHexString(int c)
    {
      StringBuilder ret = new StringBuilder();

      while (c > 0)
      {
        int unit_place = (c & 15);
        c >>= 4;

        if (unit_place >= 10)
          ret.Insert(0, (char)(unit_place + 'a' - 10));
        else
          ret.Insert(0, (char)(unit_place + '0'));
      }

      if (ret.Length == 0)
        return "0";
      else
        return ret.ToString();
    }

    public static int parseInt(string s)
    {
      return parseInt(s, 10);
    }

    public static int parseInt(string s, int @base)
    {
      const string alpha = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

      if ((@base < 2) || (@base > 36))
        throw new ArgumentException("Number base cannot be less than 2 or greater than 36", "base");

      s = s.ToUpper();

      int value = 0;

      for (int i=0; i < s.Length; i++)
      {
        int idx = alpha.IndexOf(s[i]);

        if ((idx < 0) || (idx >= @base))
          throw new FormatException("'" + s[i] + "' is not a valid base-" + @base + " digit");

        value = (value * @base) + idx;
      }

      return value;
    }

    public override int GetHashCode()
    {
      return v.GetHashCode();
    }

    public override bool Equals(object obj)
    {
      if (obj == null)
        return false;

      if (obj is int)
        return (v == (int)obj);
      if (obj is Integer)
        return (v == ((Integer)obj).v);

      return false;
    }

    public override string ToString()
    {
      return v.ToString();
    }
  }

  public class Boolean
  {
    bool v;

    public Boolean(bool value)
    {
      v = value;
    }

    public bool booleanValue()
    {
      return v;
    }
  }

  class PrettyArrayList : ArrayList
  {
    public PrettyArrayList(ICollection c)
      : base(c)
    {
    }

    public PrettyArrayList(int capacity)
      : base(capacity)
    {
    }

    public PrettyArrayList()
    {
    }

    public override string ToString()
    {
      StringBuilder builder = new StringBuilder();

      builder.Append("[");

      for (int i=0; i < Count; i++)
      {
        if (i > 0)
          builder.Append(", ");
        builder.Append(this[i]);
      }

      builder.Append("]");

      return builder.ToString();
    }
  }

  class PrettyHashtable : Hashtable
  {
	  public PrettyHashtable(IDictionary d, float loadFactor, IEqualityComparer comparer)
      : base(d, loadFactor, comparer)
    {
    }

	public PrettyHashtable(IDictionary d, IEqualityComparer comparer)
      : base(d, comparer)
    {
    }

    public PrettyHashtable(IDictionary d, float loadFactor)
      : base(d, loadFactor)
    {
    }

    public PrettyHashtable(IDictionary d)
      : base(d)
    {
    }

	public PrettyHashtable(int capacity, IEqualityComparer comparer)
      : base(capacity, comparer)
    {
    }

	public PrettyHashtable(int capacity, float loadFactor, IEqualityComparer comparer)
      : base(capacity, loadFactor, comparer)
    {
    }

    public PrettyHashtable(int capacity, float loadFactor)
      : base(capacity, loadFactor)
    {
    }

    public PrettyHashtable(int capacity)
      : base(capacity)
    {
    }

    public PrettyHashtable()
    {
    }

    public override string ToString()
    {
      StringBuilder builder = new StringBuilder();

      builder.Append("{");

      IDictionaryEnumerator enumerator = GetEnumerator();

      if (enumerator.MoveNext())
        builder.AppendFormat("{0}={1}", enumerator.Key, enumerator.Value);
      while (enumerator.MoveNext())
        builder.AppendFormat(",{0}={1}", enumerator.Key, enumerator.Value);

      builder.Append("}");

      return builder.ToString();
    }

  }
}
