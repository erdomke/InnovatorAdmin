using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Web;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Text;
using System.Runtime.Serialization;
using System.Security.Principal;
using System.IO;

namespace Innovator.Client
{
  /// <summary>
  /// http://web.archive.org/web/20090928112609/http://dotnet.org.za/markn/archive/2008/10/04/handling-passwords.aspx
  /// </summary>
  public sealed class SecureToken : IDisposable
  {
    private SecureString _encrypted;

    public delegate TR FuncRef<T1, TR>(ref T1 value);

    public int Length { get { return _encrypted.Length; } }
    public SecureString Token { get { return _encrypted; } }

    private SecureToken(SecureString encrypted)
    {
      _encrypted = encrypted;
    }
    public SecureToken(ref string unencrypted)
    {
      _encrypted = new SecureString();
      try
      {
        for (var i = 0; i < unencrypted.Length; i++)
        {
          _encrypted.AppendChar(unencrypted[i]);
        }
      }
      finally
      {
        _encrypted.MakeReadOnly();
      }
    }
    public SecureToken(ArraySegment<byte> unencrypted)
    {
      var array = unencrypted.Array;
      FromBytes(ref array, unencrypted.Offset, unencrypted.Count);
    }
    public SecureToken(ref byte[] unencrypted, int start, int length)
    {
      FromBytes(ref unencrypted, start, length);
    }
    public SecureToken(Stream data)
    {
      _encrypted = new SecureString();
      try
      {
        using (var reader = new StreamReader(data))
        {
          var value = reader.Read();
          while (value > 0)
          {
            _encrypted.AppendChar((char)value);
            value = reader.Read();
          }
        }
      }
      finally
      {
        _encrypted.MakeReadOnly();
      }
    }

    private void FromBytes(ref byte[] unencrypted, int start, int length)
    {
      var chars = new char[length];
      var gch = new GCHandle();
      RuntimeHelpers.PrepareConstrainedRegions();
      try
      {
        // Pin the character array in memory to keep it from being moved
        RuntimeHelpers.PrepareConstrainedRegions();
        try { }
        finally
        {
          gch = GCHandle.Alloc(chars, GCHandleType.Pinned);
        }

        // Get the characters from the byte array into the character array
        var stringLength = Encoding.UTF8.GetChars(unencrypted, start, length, chars, 0);

        // Add the characters to the secure string while clearing out the character array
        _encrypted = new SecureString();
        for (var i = 0; i < stringLength; i++)
        {
          _encrypted.AppendChar(chars[i]);
          chars[i] = '\0';
        }
      }
      finally
      {
        // Release the character array
        if (gch.IsAllocated) gch.Free();
        // Finalize the secure string
        _encrypted.MakeReadOnly();
      }
    }

    /// <summary>
    /// Use the password as a string in a secure fashion
    /// </summary>
    public T UseString<T>(FuncRef<string, T> callback)
    {
      T result = default(T);

      unsafe
      {
        int length = _encrypted.Length;
        var insecurePassword = new string('\0', length);

        var gch = new GCHandle();
        RuntimeHelpers.PrepareConstrainedRegions();
        try
        {
          // Pin the string in memory
          RuntimeHelpers.PrepareConstrainedRegions();
          try { }
          finally
          {
            gch = GCHandle.Alloc(insecurePassword, GCHandleType.Pinned);
          }

          IntPtr passwordPtr = IntPtr.Zero;
          try
          {
            // Get a pointer to an insecure version of the string
            RuntimeHelpers.PrepareConstrainedRegions();
            try { }
            finally
            {
              passwordPtr = Marshal.SecureStringToBSTR(_encrypted);
            }

            // Copy the unmanaged insecure version to managed one
            var pPassword = (char*)passwordPtr;
            var pInsecurePassword = (char*)gch.AddrOfPinnedObject();
            for (int index = 0; index < length; index++)
            {
              pInsecurePassword[index] = pPassword[index];
            }

            // Use the string
            result = callback(ref insecurePassword);
          }
          finally
          {
            // Zero out the unmanaged insecure version
            if (passwordPtr != IntPtr.Zero)
            {
              Marshal.ZeroFreeBSTR(passwordPtr);
            }
          }
        }
        finally
        {
          if (gch.IsAllocated)
          {
            // Zero the managed insecure string and free the memory
            var pInsecurePassword = (char*)gch.AddrOfPinnedObject();
            for (int index = 0; index < length; index++)
            {
              pInsecurePassword[index] = '\0';
            }
            gch.Free();
          }
        }
      }

      return result;
    }

    /// <summary>
    /// Use the password as a string in a secure fashion
    /// </summary>
    public T UseBytes<T>(FuncRef<byte[], T> callback)
    {
      T result = default(T);

      unsafe
      {
        int length = _encrypted.Length;
        var insecureBuffer = new byte[length * 2 + 2];
        byte[] param = null;

        var gch = new GCHandle();
        RuntimeHelpers.PrepareConstrainedRegions();
        try
        {
          // Pin the string in memory
          RuntimeHelpers.PrepareConstrainedRegions();
          try { }
          finally
          {
            gch = GCHandle.Alloc(insecureBuffer, GCHandleType.Pinned);
          }

          IntPtr passwordPtr = IntPtr.Zero;
          try
          {
            // Get a pointer to an insecure version of the string
            RuntimeHelpers.PrepareConstrainedRegions();
            try { }
            finally
            {
              passwordPtr = Marshal.SecureStringToBSTR(_encrypted);
            }

            // Copy the unmanaged insecure version to managed one
            var pPassword = (char*)passwordPtr;
            var pInsecurePassword = (byte*)gch.AddrOfPinnedObject();
            var byteLength = Encoding.UTF8.GetBytes(pPassword, _encrypted.Length, pInsecurePassword, insecureBuffer.Length);
            param = new byte[byteLength];

            for (int index = 0; index < param.Length; index++)
            {
              param[index] = pInsecurePassword[index];
              pInsecurePassword[index] = 0;
            }
            gch.Free();

            // Use the byte array
            result = callback(ref param);
          }
          finally
          {
            // Zero out the unmanaged insecure version
            if (passwordPtr != IntPtr.Zero)
            {
              Marshal.ZeroFreeBSTR(passwordPtr);
            }
          }
        }
        finally
        {
          if (param != null)
          {
            // Zero the managed array and free the memory
            for (int index = 0; index < param.Length; index++)
            {
              param[index] = 0;
            }
          }
        }
      }

      return result;
    }

    public void Write(Stream stream)
    {
      UseBytes<bool>((ref byte[] p) => {
        stream.Write(p, 0, p.Length);
        return true;
      });
    }

    public override string ToString()
    {
      return new string('*', _encrypted.Length);
    }

    public void Dispose()
    {
      _encrypted.Dispose();
    }

    public static implicit operator SecureString(SecureToken val)
    {
      return val == null ? null : val._encrypted;
    }
    public static implicit operator SecureToken(string val)
    {
      return new SecureToken(ref val);
    }
    public static implicit operator SecureToken(SecureString val)
    {
      return new SecureToken(val);
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
