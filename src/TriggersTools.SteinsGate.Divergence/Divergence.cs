using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using TriggersTools.SteinsGate.Internal;

namespace TriggersTools.SteinsGate {
	/// <summary>The static class for drawing divergence meters.</summary>
	public static class Divergence {

		/// <summary>The first of FontA sprite sheet's characters.</summary>
		internal const char FontAStart = (char) 32;
		/// <summary>The last of FontA sprite sheet's characters.</summary>
		internal const char FontAEnd = (char) 126;
		/// <summary>The first of FontB sprite sheet's characters.</summary>
		internal const char FontBStart = (char) 161;
		/// <summary>The last of FontB sprite sheet's characters.</summary>
		internal const char FontBEnd = (char) 255;

		/// <summary>These characters are ignored because they're too wide for the
		/// tubes.</summary>
		internal const string Unsupported = @"¤§¬¯µ¶¼½¾Þð";

		/// <summary>If true, an <see cref="ArgumentException"/> will be thrown if
		/// <see cref="MaxLength"/> or <see cref="MaxLines"/> are exceeded.</summary>
		public static bool EnableLimits { get; set; } = false;
		/// <summary>The maximum number of characters-per-line allowed when
		/// <see cref="EnableLimits"/> is true.</summary>
		public static int MaxLength { get; set; } = 24;
		/// <summary>The maximum number lines allowed when <see cref="EnableLimits"/>
		/// is true.</summary>
		public static int MaxLines { get; set; } = 3;

		/// <summary>Gets all characters that are supported by the font.</summary>
		public static IEnumerable<char> SupportedCharacters {
			get {
				yield return '\n';
				for (char c = FontAStart; c <= FontAEnd; c++) {
					yield return c;
				}
				for (char c = FontBStart; c <= FontBEnd; c++) {
					if (!Unsupported.Contains(new string(c, 1)))
						yield return c;
				}
			}
		}

		/// <summary>Gets all characters that are supported by the authentic font.</summary>
		public static IEnumerable<char> AuthenticCharacters {
			get {
				yield return '\n';
				yield return ' ';
				yield return '.';
				for (char c = '0'; c <= '9'; c++)
					yield return c;
			}
		}
		
		internal static bool IsUnsupported(char c) {
			return (c != '\n' && c < FontAStart) || (c > FontAEnd && c < FontBStart) ||
				c > FontBEnd || Unsupported.Contains(new string(c, 1));
		}

		internal static bool IsAuthentic(char c) {
			return c == '.' || (c >= '0' && c <= '9');
		}

		internal static bool IsFontA(char c) {
			// Ignore space
			return (c > FontAStart && c <= FontAEnd);
		}

		internal static bool IsFontB(char c) {
			return (c >= FontBStart && c <= FontBEnd);
		}
		
		/// <summary>Formats the text to how it will be output based on the arguments.</summary>
		/// <param name="text">The text to format.</param>
		/// <param name="args">The arguments to base the formatting on.</param>
		/// <returns>Returns the formatting text.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="text"/> is null.</exception>
		/// <exception cref="ArgumentException"><paramref name="args"/>.Authenticity 
		/// or <paramref name="args"/>.Alignment is invalid.</exception>
		public static string Format(string text, DivergenceArgs args) {
			if (text == null)
				throw new ArgumentNullException(text);
			if (args.Authenticity < DivergenceAuthenticity.Lax ||
				args.Authenticity > DivergenceAuthenticity.Decide)
				throw new ArgumentException("Invalid DivergenceAuthenticity!");
			if (args.Alignment < StringAlignment.Near ||
				args.Alignment > StringAlignment.Far)
				throw new ArgumentException("Invalid StringAlignment!");
			StringBuilder str = new StringBuilder();
			// Remove carriage returns and unescape newlines
			text = text.Replace("\r\n", "\n").Replace('\r', '\n');

			bool escape = false;
			for (int i = 0; i < text.Length; i++) {
				char c = text[i];
				if (c == '\\' && !escape && args.Escape != DivergenceEscape.None) {
					// If using only NewLines, don't escape
					// if the next character isn't 'n' or 'r'.
					char next = (i + 1 < text.Length ? text[i + 1] : '\0');
					if (args.Escape != DivergenceEscape.NewLines ||
						next == 'n' || next == 'r')
					{
						escape = true;
						continue;
					}
				}

				if ((c == 'n' || c == 'r') && escape)
					str.Append('\n');
				else if (c == '\t' || (c == 't' && escape))
					str.Append(' ');
				else if (IsUnsupported(c))
					str.Append('?');
				else
					str.Append(c);
				escape = false;
			}
			if (args.UsePadding) {
				string[] lines = str.ToString().Split('\n');
				int maxLength = lines.Max(l => l.Length);
				for (int i = 0; i < lines.Length; i++) {
					string line = lines[i];
					if (line.Length == maxLength)
						continue;

					switch (args.Alignment) {
					case StringAlignment.Near:
						lines[i] = line.PadRight(maxLength, ' ');
						break;
					case StringAlignment.Far:
						lines[i] = line.PadLeft(maxLength, ' ');
						break;
					case StringAlignment.Center:
						// Get the right padding (should always add 1 extra when odd)
						int right = (int) Math.Ceiling((maxLength - line.Length) / 2d);
						lines[i] = line.PadLeft(maxLength - right, ' ')
									   .PadRight(maxLength, ' ');
						break;
					}
				}
				return string.Join("\n", lines);
			}
			return str.ToString();
		}

		/// <summary>Returns true if the text contains only authentic characters.</summary>
		/// <param name="text">The text to test.</param>
		/// <returns>Returns true if the text contains only authentic characters.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="text"/> is null.</exception>
		public static bool IsAuthentic(string text) {
			foreach (char c in text) {
				if (c == '\n' || c == ' ')
					continue;
				if (!IsAuthentic(c))
					return false;
			}
			return true;
		}

		/// <summary>Returns true if the text contains only authentic characters OR
		/// certain punctuation for use with clocks (i.e. ':', '\', '/', '-'.</summary>
		/// <param name="text">The text to test.</param>
		/// <returns>Returns true if the text contains only authentic characters OR
		/// certain punctuation for use with clocks (i.e. ':', '\', '/', '-'.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="text"/> is null.</exception>
		public static bool IsSemiAuthentic(string text) {
			foreach (char c in text) {
				if (c == '\n' || c == ' ' || c == ':' ||
					c == '\\' || c == '/' || c == '-')
					continue;
				if (!IsAuthentic(c))
					return false;
			}
			return true;
		}

		/// <summary>Draws the nixie tubes to a bitmap with the specified text and
		/// default drawing arguments.</summary>
		/// <param name="text">The text to use.</param>
		/// <returns>A bitmap that has been drawn to. Remember to Dispose of it when
		/// done.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="text"/> is null.</exception>
		/// <exception cref="ArgumentException"><see cref="EnableLimits"/> is on and
		/// limits are exceeded.</exception>
		public static Bitmap Draw(string text) {
			return Draw(text, DivergenceArgs.Default);
		}

		/// <summary>Draws the nixie tubes to a bitmap with the specified text and
		/// drawing arguments.</summary>
		/// <param name="text">The text to use.</param>
		/// <param name="args">The arguments for how to draw the nixie tubes.</param>
		/// <returns>A bitmap that has been drawn to. Remember to Dispose of it when
		/// done.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="text"/> is null.</exception>
		/// <exception cref="ArgumentException"><paramref name="args"/>.Scale,
		/// <paramref name="args"/>.Authenticity, or <paramref name="args"/>.Alignment
		/// is invalid -or- <see cref="EnableLimits"/> is on and limits are exceeded.</exception>
		/// <exception cref="NotAuthenticDivergenceException">
		/// <paramref name="args"/>.Authenticity is set to Strict and non-authentic
		/// characters were found.</exception>
		public static Bitmap Draw(string text, DivergenceArgs args) {
			return new Drawer(args.Scale).Draw(text, args);
		}

		/// <summary>Calculates the size of the bitmap based on the text and arguments.</summary>
		/// <param name="text">The text to use.</param>
		/// <param name="args">The arguments for how to draw the nixie tubes.</param>
		/// <returns>Returns the calculated size.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="text"/> is null.</exception>
		public static Size CalculateSize(string text, DivergenceArgs args) {
			return new Drawer(args.Scale).CalculateSize(text, args);
		}

		/// <summary>Calculates the spacing required for the output bitmap to match
		/// the dimensions of <paramref name="bitmap"/>.</summary>
		/// <param name="bitmap">The size for the bitmap to match.</param>
		/// <param name="text">The text to test the size with.</param>
		/// <param name="args">The arguments to set the spacing for and use
		/// for calculating the spacing.</param>
		/// <param name="left">The optional forced left spacing. If non-null,
		/// <paramref name="right"/> must be null.</param>
		/// <param name="right">The optional forced right spacing. If non-null,
		/// <paramref name="left"/> must be null.</param>
		/// <param name="top">The optional forced top spacing. If non-null,
		/// <paramref name="bottom"/> must be null.</param>
		/// <param name="bottom">The optional forced bottom spacing. If non-null,
		/// <paramref name="top"/> must be null.</param>
		/// <param name="line">The spacing between lines.</param>
		/// <remarks>The spacing returned can be less than zero.</remarks>
		/// <exception cref="ArgumentNullException"><paramref name="text"/> is null.</exception>
		/// <exception cref="ArgumentException">Both <paramref name="left"/> and
		/// <paramref name="right"/>, or <paramref name="top"/> and
		/// <paramref name="bottom"/> are non-null.</exception>
		public static void CalculateSpacingFor(Bitmap bitmap, string text,
			ref DivergenceArgs args, int? left = null, int? top = null,
			int? right = null, int? bottom = null, int? line = null)
		{
			if (bitmap == null)
				throw new ArgumentNullException(nameof(bitmap));
			CalculateSpacingFor(bitmap.Width, bitmap.Height, text, ref args,
				left, top, right, bottom, line);
		}

		/// <summary>Calculates the spacing required for the output bitmap to match
		/// the dimensions of <paramref name="width"/> and <paramref name="height"/>.</summary>
		/// <param name="width">The width for the bitmap to match.</param>
		/// <param name="height">The height for the bitmap to match.</param>
		/// <param name="text">The text to test the size with.</param>
		/// <param name="args">The arguments to set the spacing for and use
		/// for calculating the spacing.</param>
		/// <param name="left">The optional forced left spacing. If non-null,
		/// <paramref name="right"/> must be null.</param>
		/// <param name="right">The optional forced right spacing. If non-null,
		/// <paramref name="left"/> must be null.</param>
		/// <param name="top">The optional forced top spacing. If non-null,
		/// <paramref name="bottom"/> must be null.</param>
		/// <param name="bottom">The optional forced bottom spacing. If non-null,
		/// <paramref name="top"/> must be null.</param>
		/// <param name="line">The spacing between lines.</param>
		/// <remarks>The spacing returned can be less than zero.</remarks>
		/// <exception cref="ArgumentNullException"><paramref name="text"/> is null.</exception>
		/// <exception cref="ArgumentException">Both <paramref name="left"/> and
		/// <paramref name="right"/>, or <paramref name="top"/> and
		/// <paramref name="bottom"/> are non-null.</exception>
		public static void CalculateSpacingFor(int width, int height, string text,
			ref DivergenceArgs args, int? left = null, int? top = null,
			int? right = null, int? bottom = null, int? line = null)
		{
			CalculateSpacingFor(new Size(width, height), text, ref args,
				left, top, right, bottom, line);
		}

		/// <summary>Calculates the spacing required for the output bitmap to match
		/// the dimensions of <paramref name="size"/>.</summary>
		/// <param name="size">The size for the bitmap to match.</param>
		/// <param name="text">The text to test the size with.</param>
		/// <param name="args">The arguments to set the spacing for and use
		/// for calculating the spacing.</param>
		/// <param name="left">The optional forced left spacing. If non-null,
		/// <paramref name="right"/> must be null.</param>
		/// <param name="right">The optional forced right spacing. If non-null,
		/// <paramref name="left"/> must be null.</param>
		/// <param name="top">The optional forced top spacing. If non-null,
		/// <paramref name="bottom"/> must be null.</param>
		/// <param name="bottom">The optional forced bottom spacing. If non-null,
		/// <paramref name="top"/> must be null.</param>
		/// <param name="line">The spacing between lines.</param>
		/// <remarks>The spacing returned can be less than zero.</remarks>
		/// <exception cref="ArgumentNullException"><paramref name="text"/> is null.</exception>
		/// <exception cref="ArgumentException">Both <paramref name="left"/> and
		/// <paramref name="right"/>, or <paramref name="top"/> and
		/// <paramref name="bottom"/> are non-null.</exception>
		public static void CalculateSpacingFor(Size size, string text,
			ref DivergenceArgs args, int? left = null, int? top = null,
			int? right = null, int? bottom = null, int? line = null)
		{
			new Drawer(args.Scale).CalculateSpacingFor(size, text, ref args,
				left, top, right, bottom, line);
		}
	}
}
