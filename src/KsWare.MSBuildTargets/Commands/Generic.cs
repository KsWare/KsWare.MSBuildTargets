using System.Diagnostics;
using System.IO;
using System.Reflection;
using KsWare.MSBuildTargets.Internal;

namespace KsWare.MSBuildTargets.Commands {

	internal static class Generic {

		public static int Call(string cmd, string[] args) {
			switch (cmd.ToLowerInvariant()) {
				case "buildtools": case "ksware.buildtools":
					cmd = "KsWare.BuildTools.exe"; break; //TODO make alias configurable
			}

			//TODO find cmd 
			var toolsPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			var cmdPath = (string) null;
			switch (true) {
				case bool _ when cmd.Eq("KsWare.BuildTools.exe"):
					cmdPath = Path.Combine(toolsPath, cmd); // tools
					if(File.Exists(cmdPath)) break;
					cmdPath = Path.Combine(toolsPath, "..\\.."); // packages
					var l=Directory.GetDirectories(cmdPath, "KsWare.BuildTools.*", SearchOption.TopDirectoryOnly);
					cmdPath = Helper.GetNameWithHighestSemanticVersion("KsWare.BuildTools", l);
					if(cmdPath!=null) break;
					cmdPath = Helper.GetFullPath(cmd);
					if (cmdPath != null) break;
					cmdPath = null;
					break;
				default:
					cmdPath = Helper.GetFullPath(cmd+(cmd.Ew(".exe")?"":".exe"));
					break;
				
			}


			var p = new Process {
				StartInfo = new ProcessStartInfo {
					FileName = cmdPath ?? cmd,
					Arguments = Helper.JoinCommandLineArgs(args)
				}
			};
			p.Start();
			p.WaitForExit();//TODO timeout
			return p.ExitCode;
		}

	}

}
