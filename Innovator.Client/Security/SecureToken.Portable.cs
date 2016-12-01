#if !SECURESTRING
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Innovator.Client
{
  public sealed class SecureToken : IDisposable
  {
    private char[] _encoded;

    private const int Cipher = 35863;

    public delegate TR FuncRef<T1, TR>(ref T1 value);

    public int Length { get { return _encoded.Length; } }

    public SecureToken(ArraySegment<byte> unencrypted)
    {
      var array = unencrypted.Array;
      FromBytes(ref array, unencrypted.Offset, unencrypted.Count);
    }
    public SecureToken(ref byte[] unencrypted, int start, int length)
    {
      FromBytes(ref unencrypted, start, length);
    }
    public SecureToken(ref string unencrypted)
    {
      var chars = unencrypted.ToCharArray();
      FromChars(ref chars);
    }
    public SecureToken(Stream data)
    {
      try
      {
        using (var reader = new StreamReader(data))
        {
          var list = new List<char>();
          var value = reader.Read();
          while (value > 0)
          {
            list.Add((char)(value ^ Cipher));
            value = reader.Read();
          }
          _encoded = list.ToArray();
        }
      }
      finally
      {
      }
    }

    private void FromBytes(ref byte[] unencrypted, int start, int length)
    {
      var chars = Encoding.UTF8.GetChars(unencrypted, start, length);
      FromChars(ref chars);
    }
    private void FromChars(ref char[] chars)
    {
      _encoded = new char[chars.Length];
      for (var i = 0; i < chars.Length; i++)
      {
        _encoded[i] = (char)(chars[i] ^ Cipher);
        chars[i] = '\0';
      }
    }

    /// <summary>
    /// Use the password as a string in a secure fashion
    /// </summary>
    public T UseString<T>(FuncRef<string, T> callback)
    {
      var chars = new char[_encoded.Length];
      string str;
      try
      {
        for (var i = 0; i < chars.Length; i++)
        {
          chars[i] = (char)(_encoded[i] ^ Cipher);
        }
        str = new string(chars);
        return callback(ref str);
      }
      finally
      {
        str = null;
        for (var i = 0; i < chars.Length; i++)
        {
          chars[i] = '\0';
        }
      }
    }

    /// <summary>
    /// Use the password as a string in a secure fashion
    /// </summary>
    public T UseBytes<T>(FuncRef<byte[], T> callback)
    {
      var chars = new char[_encoded.Length];
      byte[] data = null;
      try
      {
        for (var i = 0; i < chars.Length; i++)
        {
          chars[i] = (char)(_encoded[i] ^ Cipher);
        }
        data = Encoding.UTF8.GetBytes(chars);
        return callback(ref data);
      }
      finally
      {
        for (var i = 0; i < chars.Length; i++)
        {
          chars[i] = '\0';
        }
        if (data != null)
        {
          for (var i = 0; i < data.Length; i++)
          {
            data[i] = 0;
          }
        }
      }
    }

    public void Dispose()
    {
      for (var i = 0; i < _encoded.Length; i++)
      {
        _encoded[i] = '\0';
      }
    }

    public static implicit operator SecureToken(string val)
    {
      return new SecureToken(ref val);
    }
    public static implicit operator SecureToken(ArraySegment<byte> val)
    {
      return new SecureToken(val);
    }
    public static implicit operator SecureToken(byte[] val)
    {
      return new SecureToken(ref val, 0, val.Length);
    }
  }
}
#endif
