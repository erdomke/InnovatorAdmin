using System;
using System.Security.Cryptography;
using System.Text;

///''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
// SAMPLE: Symmetric key encryption and decryption using Rijndael algorithm.
// 
// To run this sample, create a new Visual Basic.NET project using the Console 
// Application template and replace the contents of the Module1.vb file with 
// the code below.
// 
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
// WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// 
// Copyright (C) 2002 Obviex(TM). All rights reserved.
//http://www.obviex.com/samples/Encryption.aspx
namespace Aras.Tools.InnovatorAdmin.Connections
{
  /// <summary>
  /// This class uses a symmetric key algorithm (Rijndael/AES) to encrypt and 
  /// decrypt data.
  /// </summary>
  public class RijndaelSimple
  {
    /// <summary>
    /// Generates a unique salt from the password.
    /// </summary>
    /// <param name="password">The password to use.</param><returns></returns>
    private static byte[] GenerateSalt(string password)
    {
      return Encoding.Default.GetBytes("@58d0b1415cbb4fb3993ac95b8c6b3461Z/=34#\\" + password);
    }

    /// <summary>
    /// Encrypts the value <paramref name="toEncrypt"/> using the <paramref name="password"/>.
    /// </summary>
    /// <param name="password">The password.</param>
    /// <param name="toEncrypt">The value to encrypt.</param>
    /// <returns>Encrypted value</returns>
    public static string Encrypt(string password, string toEncrypt)
    {
      Rijndael rinedal = null;
      byte[] toEncBytes = null;
      try
      {
        toEncBytes = Encoding.Default.GetBytes(toEncrypt);

        Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(password, GenerateSalt(password));

        rinedal = Rijndael.Create();
        rinedal.Padding = PaddingMode.ISO10126;

        // our key
        ICryptoTransform tx = rinedal.CreateEncryptor(pdb.GetBytes(32), pdb.GetBytes(16));
        // our IV
        byte[] encrypted = tx.TransformFinalBlock(toEncBytes, 0, toEncBytes.Length);

        return Convert.ToBase64String(encrypted);
      }
      finally
      {
        // this clears out any secret data
        if (rinedal != null)
        {
          rinedal.Clear();
        }
        // zeroes out our array
        if (toEncBytes != null)
        {
          for (int i = 0; i <= toEncBytes.Length - 1; i++)
          {
            toEncBytes[i] = 0;
          }
        }
      }
    }
    /// <summary>
    /// Decrypts the value <paramref name="toDecrypt"/> using the <paramref name="password"/>.
    /// </summary>
    /// <param name="password">The password.</param>
    /// <param name="toDecrypt">The value to decrypt.</param>
    /// <returns>Decrypted value</returns>
    public static string Decrypt(string password, string toDecrypt)
    {
      Rijndael rinedal = null;
      byte[] toDecBytes = Convert.FromBase64String(toDecrypt);
      try
      {
        Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(password, GenerateSalt(password));

        rinedal = Rijndael.Create();
        rinedal.Padding = PaddingMode.ISO10126;

        ICryptoTransform tx = rinedal.CreateDecryptor(pdb.GetBytes(32), pdb.GetBytes(16));

        byte[] decrypted = tx.TransformFinalBlock(toDecBytes, 0, toDecBytes.Length);

        return Encoding.Default.GetString(decrypted);
      }
      finally
      {
        if (rinedal != null)
        {
          rinedal.Clear();
        }
      }
    }

    public static string OEncode(string value)
    {
      int key = value.Length;
      StringBuilder builder = new StringBuilder(value.Length);

      for (int i = 0; i <= value.Length - 1; i++)
      {
        builder.Append((char)((int)value[i] ^ key));
      }

      return Convert.ToBase64String(Encoding.UTF8.GetBytes(builder.ToString()));
    }
    public static string ODecode(string value)
    {
      string decoded = Encoding.UTF8.GetString(Convert.FromBase64String(value));
      int key = decoded.Length;
      StringBuilder builder = new StringBuilder(decoded.Length);

      for (int i = 0; i <= decoded.Length - 1; i++)
      {
        builder.Append((char)((int)decoded[i] ^ key));
      }

      return builder.ToString();
    }
  }
}
