using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

namespace TriggersTools.SteinsGate.DivergenceDemo {
	static class BitmapExtensions {
		/// <summary>This only works in Windows. :(</summary>
		public static void OpenInMSPaint(this Bitmap bitmap) {
			bitmap.Save("image.png");
			if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
				ProcessStartInfo start = new ProcessStartInfo {
					UseShellExecute = false,
					FileName = "mspaint",
					Arguments = "image.png",
				};
				Process.Start(start);
				Thread.Sleep(1000);
				try {
					File.Delete("image.png");
				}
				catch { }
			}
			bitmap.Dispose();
		}
		public static void SaveAndDispose(this Bitmap bitmap, string path) {
			try {
				bitmap.Save(path);
			}
			catch { }
			bitmap.Dispose();
		}
	}
	class Program {
		static void Main(string[] cmdArgs) {
			
			// Draw Figure A
			var args = new DivergenceArgs {
				Scale = DivergenceScale.Medium,
				Spacing = new DivergenceSpacing(8, 8),
				Background = Color.FromArgb(224, 224, 224),
			};
			string text = "Oslo II";
			using (Bitmap bmp = Divergence.Draw(text, args))
				bmp.Save("OsloII.png");

			// Draw Figure B
			args.Scale = DivergenceScale.Small;
			args.Spacing = new DivergenceSpacing(5, 5);
			DateTime date = DateTime.Now;
			text = $"{date:MM\\/dd\\/yy}\n{date.TimeOfDay:hh\\:mm\\:ss}";
			using (Bitmap bmp = Divergence.Draw(text, args))
				bmp.Save("DateTime.png");

			// Draw Figure C
			args = new DivergenceArgs {
				Scale = DivergenceScale.Small,
				Background = "EV_Z02A.PNG", // The CG background
			};
			text = "1.130426";
			Divergence.CalculateSpacingFor(1920 / 2, 1080 / 2, text, ref args, left: 5, top: 2);
			using (Bitmap bmp = Divergence.Draw(text, args))
				bmp.Save("Original Worldline.png");
			
			// Draw Figure D
			args = new DivergenceArgs {
				Scale = DivergenceScale.Small,
				Background = Color.Black,
				Escape = DivergenceEscape.NewLines,
			};
			text = @"#1\n#2";
			using (Bitmap bmp = Divergence.Draw(text, args))
				bmp.Save("Command Line Example.png");
		}
		
		static void Test(string text, DivergenceArgs args) {
			Divergence.Draw(text, args).OpenInMSPaint();
		}
		
		static void Save(string text, DivergenceArgs args, string path) {
			Divergence.Draw(text, args).SaveAndDispose(path);
		}
	}
}
