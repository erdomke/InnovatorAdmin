using CommandLine;
using Innovator.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace InnovatorAdmin.Cmd
{
  internal class SharedOptions
  {
    [Option('l', "url", HelpText = "server's URL (e.g. server=http://localhost/InnovatorServer)", Required = true)]
    public string Url { get; set; }

    [Option('d', "database", HelpText = "database's name (e.g. database=dbWithoutSolution)", Required = true)]
    public string Database { get; set; }

    [Option('u', "user", HelpText = "user's login (e.g. login=root); NOTE: login must have root or admin privileges")]
    public string Username { get; set; }

    [Option('p', "password", HelpText = "user's password (e.g. password=xxxx )")]
    public string Password { get; set; }

    [Option('f', "inputfile", HelpText = "Path to package file containing the items to import/export")]
    public string InputFile { get; set; }

    public async Task<IAsyncConnection> GetConnection()
    {
      var conn = await Factory.GetConnection(new ConnectionPreferences()
      {
        Headers =
        {
          UserAgent = "InnovatorAdmin.Cmd v" + Assembly.GetExecutingAssembly().GetName().Version.ToString()
        },
        Url = Url,
        DefaultTimeout = (int)TimeSpan.FromMinutes(3).TotalMilliseconds
      }, true).ConfigureAwait(false);
      await conn.Login(GetCredentials(), true).ConfigureAwait(false);
      return conn;
    }

    public ICredentials GetCredentials()
    {
      if (string.IsNullOrEmpty(Password) || string.IsNullOrEmpty(Username))
        return new WindowsCredentials(Database);
      if (IsGuid(Password))
        return new ExplicitHashCredentials(Database, Username, Password);
      return new ExplicitCredentials(Database, Username, Password);
    }

    private static bool IsGuid(string value)
    {
      if (value?.Length != 32) return false;
      for (var i = 0; i < value.Length; i++)
      {
        switch (value[i])
        {
          case '0':
          case '1':
          case '2':
          case '3':
          case '4':
          case '5':
          case '6':
          case '7':
          case '8':
          case '9':
          case 'A':
          case 'B':
          case 'C':
          case 'D':
          case 'E':
          case 'F':
          case 'a':
          case 'b':
          case 'c':
          case 'd':
          case 'e':
          case 'f':
            break;
          default:
            return false;
        }
      }
      return true;
    }

    public static IEnumerable<IDiffDirectory> GetDirectories(params string[] paths)
    {
      var repos = new Dictionary<string, GitRepo>(StringComparer.OrdinalIgnoreCase);
      foreach (var path in paths)
      {
        if (path.StartsWith("git://", StringComparison.OrdinalIgnoreCase))
        {
          var query = new QueryString("file://" + path.Substring(6));

          var filePath = query.Uri.LocalPath;
          if (!repos.TryGetValue(filePath, out var repo))
          {
            repo = new GitRepo(filePath);
            repos[filePath] = repo;
          }

          var options = new GitDirectorySearch()
          {
            Sha = query["commit"].ToString(),
            Path = query["path"].ToString()
          };
          foreach (var branch in query["branch"])
            options.BranchNames.Add(branch);
          if (string.Equals(options.Sha, "tip", StringComparison.OrdinalIgnoreCase))
            options.Sha = null;

          yield return repo.GetDirectory(options);
        }
        else if (string.Equals(Path.GetExtension(path), ".innpkg", StringComparison.OrdinalIgnoreCase))
        {
          yield return InnovatorPackage.Load(path).Read();
        }
        else if (string.Equals(Path.GetExtension(path), ".mf", StringComparison.OrdinalIgnoreCase))
        {
          yield return new FileSysDiffDirectory(Path.GetDirectoryName(path));
        }
        else
        {
          yield return new FileSysDiffDirectory(path);
        }
      }
    }

    public static void WritePackage(ConsoleTask console, InstallScript script, string output, bool multipleDirectories, bool cleanOutput)
    {
      multipleDirectories = multipleDirectories || string.Equals(Path.GetExtension(output), ".mf", StringComparison.OrdinalIgnoreCase);

      if (cleanOutput)
      {
        console.Write("Cleaning output... ");
        if (multipleDirectories)
        {
          var dir = new DirectoryInfo(Path.GetDirectoryName(output));
          if (dir.Exists)
          {
            Parallel.ForEach(dir.EnumerateFileSystemInfos(), fs =>
            {
              if (fs is DirectoryInfo di)
                di.Delete(true);
              else
                fs.Delete();
            });
          }
          else
          {
            dir.Create();
          }
        }
        else
        {
          File.Delete(output);
        }
        console.WriteLine("Done.");
      }

      console.Write("Writing package... ");
      var outputDir = Path.GetDirectoryName(output);
      if (!Directory.Exists(outputDir))
        Directory.CreateDirectory(outputDir);

      switch (Path.GetExtension(output).ToLowerInvariant())
      {
        case ".mf":
          var manifest = new ManifestFolder(output);
          manifest.Write(script);
          break;
        case ".innpkg":
          if (multipleDirectories)
          {
            using (var pkgFolder = new InnovatorPackageFolder(output))
              pkgFolder.Write(script);
          }
          else
          {
            if (File.Exists(output))
              File.Delete(output);
            using (var pkgFile = new InnovatorPackageFile(output))
              pkgFile.Write(script);
          }
          break;
        default:
          throw new NotSupportedException("Output file type is not supported");
      }
      console.WriteLine("Done.");
    }
  }
}
