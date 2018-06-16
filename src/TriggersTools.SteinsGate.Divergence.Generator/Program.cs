using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Linq;

namespace TriggersTools.SteinsGate.Divergence.Generator {
	class Program {
		
		const char StartA = (char) 32;
		const char EndA = (char) 126; // 95 total
		const char StartB = (char) 161;
		const char EndB = (char) 255; // 95 total

		const int Columns = 19;
		const int Rows = 5;
		const int TotalChars = 95;

		const string Unsupported = @"¤§¬¯µ¶¼½¾Þð";

		const string NixieDot = @"Resources\NixieFontDot.png";
		const string NixieGrid = @"Resources\NixieGrid.png";
		const string NixieBack = @"Resources\NixieTubeBack.png";


		static bool IsUnsupported(char c) {
			return Unsupported.Contains(new string(c, 1));
		}

		static IEnumerable<char> CharRangeA {
			get {
				for (char c = (char) 32; c < 128; c++)
					yield return c;
			}
		}

		static IEnumerable<char> CharRangeB {
			get {
				for (char c = (char) 161; c < 256; c++)
					yield return c;
			}
		}

		static void MeasureChar(Font font, char c) {
			string s = new string(c, 1);
			using (Bitmap bitmap = new Bitmap(1, 1))
			using (Graphics g = Graphics.FromImage(bitmap)) {
				charSizes[c] = g.MeasureString(s, font);
			}
		}

		static Dictionary<char, SizeF> charSizes = new Dictionary<char, SizeF>();

		static void Main(string[] args) {
			// REQUIRES FONT:
			// Family: Oslo II
			// Author: Antonio Rodrigues Jr
			// Link: http://www.1001fonts.com/oslo-ii-font.html

			Directory.CreateDirectory("Output");

			using (Font oslo = new Font("Oslo II", 170, FontStyle.Regular, GraphicsUnit.Point))
			using (Brush brush = new SolidBrush(Color.FromArgb(255, 185, 51)))
			using (Bitmap grid = (Bitmap) Image.FromFile(NixieGrid))
			using (Bitmap back = (Bitmap) Image.FromFile(NixieBack))
			using (Bitmap dot = (Bitmap) Image.FromFile(NixieDot)) {
				
				Bitmap[] fontOut = new Bitmap[4];
				Bitmap[] grids = new Bitmap[2] { grid, back };
				char[][] CharRanges = new char[2][] {
					CharRangeA.ToArray(), CharRangeB.ToArray(),
				};
				foreach (char c in CharRangeA)
					MeasureChar(oslo, c);
				foreach (char c in CharRangeB)
					MeasureChar(oslo, c);

				Size space = new Size(120, 290);
				for (int i = 0; i < fontOut.Length; i++)
					fontOut[i] = new Bitmap(space.Width * Columns, space.Height * Rows, PixelFormat.Format32bppArgb);

				for (int i = 0; i < 2; i++) {
					using (Graphics g = Graphics.FromImage(fontOut[i])) {
						g.TextRenderingHint = TextRenderingHint.AntiAlias;
						g.Clear(Color.Transparent);
						//for (int j = 0; j < 1; j++) {
						for (int j = 0; j < TotalChars; j++) {
							char c = CharRanges[i % 2][j];
							int row = j / Columns;
							int column = j % Columns;
							int charWidth = (int) charSizes[c].Width - 10;
							Point point = new Point(column * space.Width, row * space.Height);

							if (IsUnsupported(c))
								continue;
							if (c == '.') {
								g.DrawImage(dot, point);
							}
							else {
								point.X += (space.Width - charWidth) / 2;
								point.Y -= 21;
								g.DrawString(c.ToString(), oslo, brush, point);
							}
						}
					}
					Console.WriteLine($"Complete: {i + 1}");
				}

				for (int i = 0; i < 2; i++) {
					int i2 = i + 2;
					using (Graphics g = Graphics.FromImage(fontOut[i2])) {
						g.TextRenderingHint = TextRenderingHint.AntiAlias;
						g.InterpolationMode = InterpolationMode.NearestNeighbor;
						g.Clear(Color.Transparent);
						for (int j = 0; j < TotalChars; j++) {
							int row = j / Columns;
							int column = j % Columns;
							Point point = new Point(column * space.Width, row * space.Height);
							g.DrawImage(grids[i], point);
						}
					}
					Console.WriteLine($"Complete: {i + 3}");
				}

				fontOut[0].Save(@"Output\NixieFontA.png");
				fontOut[1].Save(@"Output\NixieFontB.png");
				fontOut[2].Save(@"Output\NixieGrid.png");
				fontOut[3].Save(@"Output\NixieTubeBacking.png");

				foreach (Bitmap bmp in fontOut)
					bmp.Dispose();

				using (Bitmap maskA = (Bitmap) Image.FromFile(@"Output\NixieFontA.png"))
				using (Bitmap maskB = (Bitmap) Image.FromFile(@"Output\NixieFontB.png"))
				using (Bitmap maskGridA = (Bitmap) Image.FromFile(@"Output\NixieGrid.png"))
				using (Bitmap maskGridB = (Bitmap) Image.FromFile(@"Output\NixieGrid.png")) {
					Bitmap[] masks = new Bitmap[2] { maskA, maskB };
					Bitmap[] maskGrids = new Bitmap[2] { maskGridA, maskGridB };

					for (int i = 0; i < 2; i++) {
						using (var bmpData = masks[i].LockRead())
						using (var bmpDataGrid = maskGrids[i].LockReadWrite()) {
							for (int y = 0; y < masks[i].Height; y++) {
								for (int x = 0; x < masks[i].Width; x++) {
									Color color = bmpData.Data.GetColor(x, y);
									Color colorGrid = bmpDataGrid.Data.GetColor(x, y);
									int outAlpha = color.A * colorGrid.A / 255;
									Color outColor = Color.FromArgb(outAlpha, colorGrid);
									bmpDataGrid.Data.SetColor(x, y, outColor);
								}
							}
						}
						Console.WriteLine($"Complete: {i + 5}");
						maskGrids[i].Save($@"Output\NixieFont{(char) ('A' + i)}GridMask.png");
					}
				}

				Console.WriteLine("Finished!");
			}
		}
	}
}
