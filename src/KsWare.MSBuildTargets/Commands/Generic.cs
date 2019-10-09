using System.Diagnostics;
using KsWare.MSBuildTargets.Internal;

namespace KsWare.MSBuildTargets.Commands {

	internal static class Generic {

		public static int Call(string cmd, string[] args) {
			//TODO find cmd 
			var p = new Process {
				StartInfo = new ProcessStartInfo {
					FileName = cmd,
					Arguments = Helper.JoinCommandLineArgs(args)
				}
			};
			p.Start();
			p.WaitForExit();//TODO timeout
			return p.ExitCode;
		}

	}

}
