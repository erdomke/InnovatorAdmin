using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.IO;

namespace InnovatorAdmin
{
  public class ClientInstall
  {
    public void InstallClient(string url)
    {
      var session = new Session();
      var dotNetVers = Session.GetDotNetVersions(true);
      if (session.OperatingSystem == "Win7")
      {
        string dotNetDirectory;
        if (dotNetVers.Any(v => !string.IsNullOrEmpty(v) && char.IsNumber(v[0]) && int.Parse(v.Substring(0, 1)) > 1))
        {
          dotNetDirectory = Path.Combine(Environment.SystemDirectory, @"Microsoft.NET\Framework\v2.0.50727");
        }
        else 
        {
          dotNetDirectory = Path.Combine(Environment.SystemDirectory, @"Microsoft.NET\Framework\v1.1.4322");
        }

        if (!url.EndsWith(@"/")) url += @"/";

      }



    }

    public int ExecCmd(string fileName, string args, string workingDir, System.Text.StringBuilder output = null, bool echo = true)
    {
      var startInfo = new ProcessStartInfo();
      startInfo.FileName = fileName;
      startInfo.Arguments = args;
      startInfo.WorkingDirectory = workingDir;
      startInfo.RedirectStandardOutput = true;
      startInfo.UseShellExecute = false;
      var proc = Process.Start(startInfo);
      char chr = '\0';
      while (!proc.StandardOutput.EndOfStream)
      {
        chr = Convert.ToChar(proc.StandardOutput.Read());
        if (echo)
          Console.Write(chr);
        if (output != null)
          output.Append(chr);
      }
      proc.WaitForExit();
      return proc.ExitCode;
    }

    /// <summary>
    /// Returns relevant computer information
    /// </summary>
    /// <remarks>Some code taken from <a href="http://1code.codeplex.com">All-In-One Code Framework</a></remarks>
    public class Session
    {
      private System.DateTime _timeStarted;

      private System.DateTime _timeLastAction;
      public short Bits {
        get {
          try {
            if (Is64BitOperatingSystem()) {
              return 64;
            } else {
              return 32;
            }
          } catch (Exception ex) {
            Debug.Print(ex.ToString());
            return 0;
          }
        }
      }
      public string ComputerName {
        get {
          try {
            return System.Environment.MachineName;
          } catch (Exception ex) {
            Debug.Print(ex.ToString());
            return "ERROR";
          }
        }
      }
      public bool IsDebugSession {
        // vshost quit working for KVK... Appending second check for debugger.isattached
        get { return System.Diagnostics.Process.GetCurrentProcess().ProcessName.EndsWith(".vshost") || System.Diagnostics.Debugger.IsAttached; }
      }
      public string OperatingSystem {
        get {
          try {
            return GetOsName(System.Environment.OSVersion.Version);
          } catch (Exception ex) {
            Debug.Print(ex.ToString());
            return "ERROR";
          }
        }
      }
      public string OperatingSystemDescription {
        get { return string.Format("{0}, {1} bit", OperatingSystem, Bits); }
      }
      public System.DateTime TimeLastAction {
        get { return _timeLastAction; }
        set { _timeLastAction = value; }
      }
      public System.DateTime TimeStarted {
        get { return _timeStarted; }
        set { _timeStarted = value; }
      }

      public string ToLongString()
      {
        return string.Format("Computer Name: {1}{0}Operating System: {2}, {3} bit", Environment.NewLine, ComputerName, OperatingSystem, Bits);
      }
      public override string ToString()
      {
        return string.Format("{0}: {1}, {2} bit", ComputerName, OperatingSystem, Bits);
      }

      public string GetAppPath(Type fromType = null)
      {
        string codeBase = null;
        if (fromType == null) {
          codeBase = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;
        } else {
          codeBase = fromType.Assembly.CodeBase;
        }
        UriBuilder uri = new UriBuilder(codeBase);
        return System.Uri.UnescapeDataString(uri.Path);
      }
      private string GetOsName(Version version)
      {
        switch (version.Major)
        {
          case 1:
            return "Win1.0";
          case 2:
            return "Win2.0";
          case 3:
            if (version.Minor == 0)
            {
              return "Win3.0";
            } else {
              return "WinNT3." + version.Minor;
            }
          case 4:
            switch (version.Minor)
            {
              case 0:
                if (version.Build < 1000)
                {
                  return "Win95";
                } else {
                  return "WinNT4.0";
                }
              case 1:
                if (version.Build < 2000)
                {
                  return "Win98";
                } else {
                  return "Win98SE";
                }
              case 9:
              case 90:
                return "WinME";
            }
            break;
          case 5:
            switch (version.Minor)
            {
              case 0:
                return "Win2000";
              case 1:
                return "WinXP";
              case 2:
                return "WinXP64";
            }
            break;
          case 6:
            switch (version.Minor)
            {
              case 0:
                return "WinVista";
              case 1:
                return "Win7";
              case 2:
                return "Win8";
            }
            break;
        }

        return string.Empty;
      }

      #region "Is64BitOperatingSystem (IsWow64Process)"

      /// <summary>
      /// The function determines whether the current operating system is a 
      /// 64-bit operating system.
      /// </summary>
      /// <returns>
      /// The function returns true if the operating system is 64-bit; 
      /// otherwise, it returns false.
      /// </returns>
      public bool Is64BitOperatingSystem()
      {
        // 64-bit programs run only on Win64
        if ((IntPtr.Size == 8)) {
          return true;
        // 32-bit programs run on both 32-bit and 64-bit Windows
        } else {
          // Detect whether the current process is a 32-bit process running 
          // on a 64-bit system.
          bool flag = false;
          return ((DoesWin32MethodExist("kernel32.dll", "IsWow64Process") && IsWow64Process(GetCurrentProcess(), ref flag)) && flag);
        }
      }

      /// <summary>
      /// The function determins whether a method exists in the export table of 
      /// a certain module.
      /// </summary>
      /// <param name="moduleName">The name of the module</param>
      /// <param name="methodName">The name of the method</param>
      /// <returns>
      /// The function returns true if the method specified by methodName 
      /// exists in the export table of the module specified by moduleName.
      /// </returns>
      private bool DoesWin32MethodExist(string moduleName, string methodName)
      {
        IntPtr moduleHandle = GetModuleHandle(moduleName);
        if ((moduleHandle == IntPtr.Zero)) {
          return false;
        }
        return (GetProcAddress(moduleHandle, methodName) != IntPtr.Zero);
      }

      [DllImport("kernel32.dll")]
      private static extern IntPtr GetCurrentProcess();

      [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
      private static extern IntPtr GetModuleHandle(string moduleName);

      [DllImport("kernel32", CharSet = CharSet.Auto, SetLastError = true)]
      private static extern IntPtr GetProcAddress(IntPtr hModule, [MarshalAs(UnmanagedType.LPStr)]string procName);

      [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
      [return: MarshalAs(UnmanagedType.Bool)]
      private static extern bool IsWow64Process(IntPtr hProcess, ref bool wow64Process);

      #endregion

      public static string GetIeVersion()
      {
        try {
          using (var ieKey = RegistryKey.OpenRemoteBaseKey(RegistryHive.LocalMachine, "").OpenSubKey("SOFTWARE\\Microsoft\\Internet Explorer\\")) {
            return GetMajorMinorRev(ieKey.GetValue("Version", "").ToString());
          }
        } catch {
          //Do Nothing
        }
        return "?";
      }

      public static IEnumerable<string> GetDotNetVersions(bool mainRevOnly)
      {
        List<string> versions = new List<string>();

        try {
          // Opens the registry key for the .NET Framework entry. 
          using (var ndpKey = RegistryKey.OpenRemoteBaseKey(RegistryHive.LocalMachine, "").OpenSubKey("SOFTWARE\\Microsoft\\NET Framework Setup\\NDP\\")) {

            RegistryKey versionKey = null;
            string name = null;
            string sp = null;
            string install = null;

            foreach (var versionKeyName in ndpKey.GetSubKeyNames()) {
              if (versionKeyName.StartsWith("v")) {
                versionKey = ndpKey.OpenSubKey(versionKeyName);
                name = (string)versionKey.GetValue("Version", "");
                sp = versionKey.GetValue("SP", "").ToString();
                install = versionKey.GetValue("Install", "").ToString();

                if (string.IsNullOrEmpty(name)) {
                  foreach (string subKeyName in versionKey.GetSubKeyNames()) {
                    RegistryKey subKey = versionKey.OpenSubKey(subKeyName);
                    name = (string)subKey.GetValue("Version", "");
                    if (!string.IsNullOrEmpty(name)) {
                      sp = subKey.GetValue("SP", "").ToString();
                    }
                    install = subKey.GetValue("Install", "").ToString();

                    if (mainRevOnly) {
                      versions.Add(GetMajorMinorRev(name) + " " + subKeyName);
                    } else if (!string.IsNullOrEmpty(sp) && install == "1") {
                      versions.Add(name + " SP" + sp + " " + subKeyName);
                    } else if (install == "1") {
                      versions.Add(name + " " + subKeyName);
                    }
                  }
                } else if (mainRevOnly) {
                  versions.Add(GetMajorMinorRev(name));
                } else if (string.IsNullOrEmpty(install)) {
                  //no install info, ust be later
                  versions.Add(name);
                } else if (!string.IsNullOrEmpty(sp) && install == "1") {
                  versions.Add(name + " SP" + sp);
                }
              }
            }

          }
        } catch {
          //Do Nothing
        }
        return versions;
      }

      private static string GetMajorMinorRev(string version)
      {
        var parts = version.Split('.');
        if (parts.Length == 1) {
          return parts[0];
        } else if (parts.Length > 1) {
          return parts[0] + "." + parts[1];
        }
        return string.Empty;
      }
    }

  }
}
