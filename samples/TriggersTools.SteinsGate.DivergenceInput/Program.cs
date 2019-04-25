using System;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace TriggersTools.SteinsGate.DivergenceInput {
	class BackException : Exception { }
	class ExitException : Exception { }
	class Program {
		static void Main(string[] args) {
			Console.Title = "Divergence Meter Input";
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine("======== Divergence Input ========");
			Console.ResetColor();
			Console.Write("Enter ");
			Console.ForegroundColor = ConsoleColor.Magenta;
			Console.Write("back");
			Console.ResetColor();
			Console.Write(" during any input to go to the previous input. Enter ");
			Console.ForegroundColor = ConsoleColor.Magenta;
			Console.Write("exit");
			Console.ResetColor();
			Console.WriteLine(" to exit. (case-sensitive)");

			string input = null;
			Divergence.EnableLimits = false;
			char[] invalidNameChars = Path.GetInvalidFileNameChars();
			int inputIndex = 0;
			DivergenceArgs dargs = new DivergenceArgs {
				Alignment = System.Drawing.StringAlignment.Near,
				Authenticity = DivergenceAuthenticity.Lax,
				Background = DivergenceBackground.None,
				Escape = DivergenceEscape.NewLines,
				Scale = DivergenceScale.Small,
				AlignTubes = false,
				UsePadding = false,
			};
			string text = null;
			string file = null;
			WriteHeader();
			while (true) {
				try {
					switch (inputIndex) {
					case 0:
						do {
							input = GetInput("Size (s/m/l)", ConsoleColor.Cyan, "l");
						} while (input != "s" && input != "m" && input != "l");
						if (input == "s") dargs.Scale = DivergenceScale.Small;
						if (input == "m") dargs.Scale = DivergenceScale.Medium;
						if (input == "l") dargs.Scale = DivergenceScale.Large;
						break;
					case 1:
						do {
							text = GetInput("  Meter Text", ConsoleColor.Yellow);
						} while (text.Length == 0);
						break;
					case 2:
						do {
							file = GetInput("Save to File", ConsoleColor.White, GetNextFile());
							if (file.Any(c => Array.IndexOf(invalidNameChars, c) != -1)) {
								WriteError("  Invalid path characters!");
								file = null;
							}
						} while (file == null);

						using (var bitmap = Divergence.Draw(text, dargs))
							bitmap.Save(file, ImageFormat.Png);
						Console.ForegroundColor = ConsoleColor.Green;
						Console.WriteLine($"  Saved text \"{text}\" to file \"{file}\"!");
						Console.ResetColor();

						do {
							input = GetInput(" Open? (y/n)", ConsoleColor.White, "n");
						} while (input != "y" && input != "yes" && input != "n" && input != "no");
						if (input == "yes" || input == "y") {
							ProcessStartInfo startInfo = new ProcessStartInfo {
								FileName = file,
								Verb = "open",
								UseShellExecute = true,
							};
							Process.Start(startInfo)?.Dispose();
						}

						WriteHeader();

						break;
					}
					inputIndex = (inputIndex + 1) % 3;
				} catch (BackException) {
					inputIndex = Math.Max(0, inputIndex - 1);
				} catch (ExitException) {
					return;
				} catch (Exception ex) {
					WriteError("  An error occurred!");
					WriteError(ex);
					File.WriteAllText("error.txt", ex.ToString());

					WriteHeader();
				}
			}
		}

		private static void WriteHeader() {
			Console.WriteLine();
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine("Draw a Divergence Meter:");
			Console.ResetColor();
		}
		private static void WriteError(object line) {
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine("  An error occurred!");
			Console.WriteLine(line);
			Console.ResetColor();
		}

		private static string GetInput(string display, string watermark = null) {
			return GetInput(display, Console.ForegroundColor, watermark);
		}
		private static string GetInput(string display, ConsoleColor foreground, string watermark = null) {
			Console.Write($"{display}: ");
			if (watermark != null) {
				int left = Console.CursorLeft;
				int top = Console.CursorTop;
				Console.ForegroundColor = ConsoleColor.DarkGray;
				Console.Write(watermark);
				Console.CursorLeft = left;
				Console.CursorTop = top;
			}
			Console.ForegroundColor = foreground;
			string input = Console.ReadLine();
			Console.ResetColor();
			if (input == "exit")
				Environment.Exit(0);
			else if (input == "back")
				throw new BackException();
			return (input.Length == 0 && watermark != null ? watermark : input);
		}

		private static string GetNextFile() {
			int index = 1;
			string file;
			do {
				file = $"divergence{index.ToString().PadLeft(4, '0')}.png";
				index++;
			} while (File.Exists(file));
			return file;
		}
	}
}
