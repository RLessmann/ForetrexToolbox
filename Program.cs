﻿using System.Diagnostics.CodeAnalysis;
using System.IO.Compression;
using System.Reflection;

[assembly: AssemblyTitleAttribute("ForetrexToolbox")]
[assembly: AssemblyCompanyAttribute("Not Sure")]
[assembly: AssemblyProductAttribute("ForetrexToolbox")]

namespace ForetrexToolbox
{
  internal class Program
  {
    static void PrintUsage()
    {
      Console.WriteLine(string.Format("ForetrexToolbox.exe <command> ..."));
      Console.WriteLine(string.Format("Commands:"));
      Console.WriteLine(string.Format("   airports"));
      Console.WriteLine(string.Format("       --type=<comma seperated list, e.g.:small,medium,large>"));
      Console.WriteLine(string.Format("       --continents=<comma seperated list, e.g.:EU,NA>"));
      Console.WriteLine(string.Format("       --output=<gpx file>"));
      Console.WriteLine(string.Format("Utility commands:"));
      Console.WriteLine(string.Format("   version"));
      Console.WriteLine(string.Format("   deploy"));

    }
    private static string CreateZipArchive()
    {
      // target dir
      DirectoryInfo target = new DirectoryInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestRailCli"));
      if (target.Exists)
      {
        target.Delete(true);
      }
      target.Create();
      // files
      string[] files = {
        };
      foreach (var file in files)
      {
        File.Copy(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, file), Path.Combine(target.FullName, file), true);
      }
      // config        
      string config = "FortrexToolbox.dll.config";
      string[] lines = File.ReadAllLines(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, config));
      for (int i = 0; i < lines.Length; i++)
      {
        if (lines[i].Contains("value", StringComparison.OrdinalIgnoreCase))
        {
          string line = lines[i].Substring(0, lines[i].IndexOf("value"));
          if (line.Contains("access", StringComparison.OrdinalIgnoreCase)
            || line.Contains("project", StringComparison.OrdinalIgnoreCase))
          {
            lines[i] = line + "value=\"\"/>";
          }
        }
      }
      File.WriteAllLines(Path.Combine(target.FullName, config), lines);
      // archive zip
      string archive = String.Format("FortrexToolbox-{0}-{1}.zip", Assembly.GetEntryAssembly()!.GetName().Version!, DetectTargetPlatform());
      FileInfo fz = new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, archive));
      if (fz.Exists)
      {
        fz.Delete();
      }
      using (ZipArchive zip = ZipFile.Open(fz.FullName, ZipArchiveMode.Create))
      {
        foreach (FileInfo fi in target.GetFiles())
        {
          zip.CreateEntryFromFile(fi.FullName, fi.Name);
        }
      }
      target.Delete(true);
      return archive;
    }
    private static string DetectTargetPlatform()
    {
      Assembly.GetExecutingAssembly().Modules.First().GetPEKind(out var pekind, out var machine);
      switch (pekind)
      {
        case PortableExecutableKinds.ILOnly:
          return "AnyCPU";
        case PortableExecutableKinds.ILOnly | PortableExecutableKinds.PE32Plus:
          return "x64";
        case PortableExecutableKinds.ILOnly | PortableExecutableKinds.Required32Bit:
          return "x86";
        default:
          return "Unknown";
      }
    }

    [UnconditionalSuppressMessage("Trimming", "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code", Justification = "<Pending>")]
    static void Main(string[] args)
    {
      if( args.Length > 1 && args[0].Equals("airports", StringComparison.OrdinalIgnoreCase))
      {
        List<string> selectedAirports = new List<string>();
        List<string> selectedContinents = new List<string>();
        FileInfo? output = null;
        foreach (string arg in args)
        {
          if (arg.StartsWith("--type=", StringComparison.OrdinalIgnoreCase))
          {
            string[] items = arg.Substring(11).Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string item in items)
            {
              selectedAirports.Add(item);
            }
          }
          if ( arg.StartsWith("--continents=", StringComparison.OrdinalIgnoreCase))
          {
            string[] items = arg.Substring(13).Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach(string item in items)
            {
              selectedContinents.Add(item);
            }
          }
          if (arg.StartsWith("--output=", StringComparison.OrdinalIgnoreCase))
          {
            output = new FileInfo(arg.Substring(9));
          }
        }
        Airports airports = new Airports(400, selectedAirports, selectedContinents);
        GpxData data = GpxData.FromList(airports.WayPoints);
        data.ToFile(output);
        Console.WriteLine( "Saved " + airports.WayPoints.Count + " airports to file " + output!.Name);
        return;
      }
      if (args.Length == 1 && args[0].Equals("version", StringComparison.OrdinalIgnoreCase))
      {
        Console.WriteLine(string.Format("Version: {0}", Assembly.GetEntryAssembly()!.GetName().Version!.ToString()));
        return;
      }
      if (args.Length == 1 && args[0].Equals("deploy", StringComparison.OrdinalIgnoreCase))
      {
        Console.WriteLine("Created deployment archive " + CreateZipArchive());
        return;
      }
      PrintUsage();
      Environment.ExitCode = 1;
    }
  }
}
