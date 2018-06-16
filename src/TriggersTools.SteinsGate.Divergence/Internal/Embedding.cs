using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace TriggersTools.SteinsGate.Internal {
	internal static class Embedding {

		/// <summary>Loads the specified manifest resource from the executing assembly.</summary>
		public static Stream Get(params string[] paths) {
			string path = Combine(paths);
			path = Combine(nameof(TriggersTools), nameof(SteinsGate), path);
			return Assembly.GetExecutingAssembly().GetManifestResourceStream(path);
		}

		/// <summary>Combines the paths for a resource entry.</summary>
		public static string Combine(params string[] paths) {
			return string.Join(".", paths.Select(p => p.Trim('.')));
		}
	}
}
