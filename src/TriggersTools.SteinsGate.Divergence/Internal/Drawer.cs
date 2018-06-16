using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using TriggersTools.SteinsGate.Internal;
using static TriggersTools.SteinsGate.Divergence;

namespace TriggersTools.SteinsGate.Internal {
	internal class Drawer {
		private struct DrawArgs {
			public string Text { get; }
			public string[] Lines { get; }
			public DivergenceSpacing Spacing { get; set; }
			public DivergenceBackground Background { get; }
			public StringAlignment Alignment { get; }
			public bool UsePadding { get; }
			public bool AlignTubes { get; }
			public bool Authentic { get; }
			public Bitmap Bitmap { get; set; }

			public int LineCount => Lines.Length;
			public int MaxLength => Lines.Max(l => l.Length);
			public bool NeedsAlignment => !UsePadding &&
				Alignment != StringAlignment.Near;
			
			public DrawArgs(string text, DivergenceArgs args) {
				Text = text;
				Lines = text.Split('\n');
				Spacing = args.Spacing;
				Background = args.Background;
				Alignment = args.Alignment;
				UsePadding = args.UsePadding;
				AlignTubes = args.AlignTubes;
				Authentic = args.Authenticity != DivergenceAuthenticity.None;
				Bitmap = null;
			}
		}

		private const int Columns = 19;
		private const int Rows = 5;
		
		private DivergenceScale Scale { get; }
		private double Ratio { get; }

		private int SideWidth => GetSize(10);
		private int TubeWidth => GetSize(132);
		private int TubeHeight => GetSize(428);
		private Size TubeSize => new Size(TubeWidth, TubeHeight);
		private int CharOffsetX => GetSize(6);
		private int CharOffsetY => GetSize(58);
		private Size CharOffset => new Size(CharOffsetX, CharOffsetY);
		private int CharWidth => GetSize(120);
		private int CharHeight => GetSize(290);
		private Size CharSize => new Size(CharWidth, CharHeight);

		private int GetSize(int value) {
			return (int) Math.Ceiling(value * Ratio);
		}
		
		public Drawer(DivergenceScale scale) {
			Scale = scale;
			switch (scale) {
			case DivergenceScale.Large:  Ratio = 1.00; break;
			case DivergenceScale.Medium: Ratio = 0.50; break;
			case DivergenceScale.Small:  Ratio = 0.25; break;
			default: throw new ArgumentException("Invalid DivergenceScale!");
			}
		}

		public Bitmap Draw(string text, DivergenceArgs divArgs) {
			if (text == null)
				throw new ArgumentNullException(text);
			text = Format(text, divArgs);

			if (divArgs.Authenticity == DivergenceAuthenticity.Decide) {
				if (IsAuthentic(text))
					divArgs.Authenticity = DivergenceAuthenticity.Strict;
				else if (IsSemiAuthentic(text))
					divArgs.Authenticity = DivergenceAuthenticity.Lax;
				else
					divArgs.Authenticity = DivergenceAuthenticity.None;
			}
			else if (divArgs.Authenticity == DivergenceAuthenticity.Strict &&
				!IsAuthentic(text))
			{
				throw new NotAuthenticDivergenceException(text);
			}

			DrawArgs args = new DrawArgs(text, divArgs);

			if (args.LineCount > MaxLines && EnableLimits) {
				throw new ArgumentException($"{nameof(text)} has {args.LineCount} " +
					$"lines, which is greater than {nameof(MaxLines)} ({MaxLines})!");
			}
			for (int i = 0; i < args.LineCount && EnableLimits; i++) {
				string line = args.Lines[i];
				if (line.Length > MaxLength) {
					throw new ArgumentException($"Line {i + 1} is " +
						$"longer than {nameof(MaxLength)} ({MaxLength})!");
				}
			}
			
			Size size = CalculateSize(args);

			args.Bitmap = new Bitmap(size.Width, size.Height,
				PixelFormat.Format32bppArgb);
			try {
				using (Graphics g = Graphics.FromImage(args.Bitmap))
				using (Resources res = GetResources(text, args.Authentic)) {
					g.InterpolationMode = InterpolationMode.NearestNeighbor;
					DrawBackground(g, args);
					for (int i = 0; i < args.LineCount; i++)
						DrawLine(g, i, res, args);
				}
			}
			catch {
				args.Bitmap.Dispose();
				throw;
			}
			return args.Bitmap;
		}

		public Size CalculateSize(string text, DivergenceArgs divArgs) {
			if (text == null)
				throw new ArgumentNullException(text);
			text = Format(text, divArgs);

			DrawArgs args = new DrawArgs(text, divArgs);
			return CalculateSize(args);
		}

		private Size CalculateSize(DrawArgs args) {
			return new Size(
				Math.Max(1, args.Spacing.Horizontal + SideWidth * 2 +
					TubeWidth * args.MaxLength),
				Math.Max(1, args.Spacing.Vertical + TubeHeight * args.LineCount +
					args.Spacing.Line * (args.LineCount - 1)));
		}

		public void CalculateSpacingFor(Size size, string text,
			ref DivergenceArgs divArgs, int? left, int? top, int? right, int? bottom,
			int? line)
		{
			if (left.HasValue && right.HasValue) {
				throw new ArgumentException("Left and Right spacing cannot both be " +
					"specified!");
			}
			else if (top.HasValue && bottom.HasValue) {
				throw new ArgumentException("Top and Bottom spacing cannot both be " +
					"specified!");
			}
			else if (text == null) {
				throw new ArgumentNullException(text);
			}
			text = Format(text, divArgs);

			DrawArgs args = new DrawArgs(text, divArgs) {
				Spacing = new DivergenceSpacing(
					left ?? 0, top ?? 0, right ?? 0, bottom ?? 0, line ?? 0),
			};
			
			Size difference = size - CalculateSize(args);
			DivergenceSpacing spacing = args.Spacing;
			if (left.HasValue) {
				spacing.Right = difference.Width;
			}
			else if (right.HasValue) {
				spacing.Left = difference.Width;
			}
			else {
				spacing.Left = difference.Width / 2;
				spacing.Right = difference.Width - spacing.Left;
			}
			if (top.HasValue) {
				spacing.Bottom = difference.Height;
			}
			else if (bottom.HasValue) {
				spacing.Top = difference.Height;
			}
			else {
				spacing.Top = difference.Height / 2;
				spacing.Bottom = difference.Height - spacing.Top;
			}

			divArgs.Spacing = spacing;
		}

		private void DrawBackground(Graphics g, DrawArgs args) {
			Bitmap target = args.Bitmap;

			Color? color = args.Background.Color;
			bool scaleBitmap = args.Background.ScaleBitmap;
			string bitmapFile = args.Background.BitmapFile;
			Bitmap bitmap = args.Background.Bitmap;
			if (bitmapFile != null)
				bitmap = (Bitmap) Image.FromFile(bitmapFile);
			try {
				if (color.HasValue)
					g.Clear(color.Value);
				else
					g.Clear(Color.Transparent);
				if (bitmap != null) {
					if (scaleBitmap && (bitmap.Width != target.Width ||
							bitmap.Height != target.Height))
					{
						g.InterpolationMode = InterpolationMode.HighQualityBicubic;
						g.SmoothingMode = SmoothingMode.HighQuality;
						double widthScale = (double) target.Width / bitmap.Width;
						double heightScale = (double) target.Height / bitmap.Height;
						double scale = Math.Max(widthScale, heightScale);
						
						int newWidth = Math.Max(1,
							(int) Math.Round(bitmap.Width * widthScale));
						int newHeight = Math.Max(1,
							(int) Math.Round(bitmap.Height * heightScale));
						int x = (newWidth - target.Width) / 2;
						int y = (newHeight - target.Height) / 2;
						g.DrawImage(bitmap, x, y, newWidth, newHeight);
						g.InterpolationMode = InterpolationMode.NearestNeighbor;
						g.SmoothingMode = SmoothingMode.Default;
					}
					else {
						int x = (bitmap.Width - target.Width) / 2;
						int y = (bitmap.Height - target.Height) / 2;
						g.DrawImage(bitmap, x, y);
					}
				}
			}
			finally {
				if (bitmapFile != null)
					args.Background.Bitmap?.Dispose();
			}
		}

		private void DrawLine(Graphics g, int index, Resources res, DrawArgs args) {
			string line = args.Lines[index];
			int maxLength = args.MaxLength;
			int length = line.Length;

			Point point = new Point(
				args.Spacing.Left + SideWidth,
				args.Spacing.Top + index * (TubeHeight + args.Spacing.Line));

			// Should we offset the nixie tubes for the alignment?
			int dif = maxLength - length;
			if (args.NeedsAlignment && dif != 0) {
				if (args.Alignment == StringAlignment.Far)
					point.X += (dif * TubeWidth);
				else if (args.AlignTubes)
					point.X += (dif / 2) * TubeWidth;
				else
					point.X += (dif * TubeWidth) / 2;
			}

			for (int i = 0; i < line.Length; i++, point.X += TubeWidth) {
				char c = line[i];
				DrawTube(g, i, length, point, res);
				DrawChar(g, c, point, res, args);
			}
		}

		private void DrawTube(Graphics g, int index, int length, Point point,
			Resources res)
		{
			Bitmap tube = null;
			if (index == 0) {
				point.X -= SideWidth;
				tube = (length == 1 ? res.TubeSingle : res.TubeLeft);
			}
			else if (index + 1 < length) {
				tube = res.TubeMid;
			}
			else {
				tube = res.TubeRight;
			}
			g.DrawImage(tube, point);
		}

		private void DrawChar(Graphics g, char c, Point point, Resources res,
			DrawArgs args)
		{
			point += CharOffset;
			Rectangle sourceRect = new Rectangle(Point.Empty, CharSize);
			Bitmap source = null;
			int index = 0;
			if (c == ' ') {
				// Skip
				return;
			}
			else if (args.Authentic && IsAuthentic(c)) {
				source = res.Authentic;
				if (char.IsDigit(c))
					index = ((c - '0') + 1);
			}
			else if (IsFontA(c)) {
				source = res.FontA;
				index = c - FontAStart;
			}
			else if (IsFontB(c)) {
				source = res.FontB;
				index = c - FontBStart;
			}
			else {
				return;
			}
			sourceRect.X += (index % Columns) * CharWidth;
			sourceRect.Y += (index / Columns) * CharHeight;
			g.DrawImage(source, point.X, point.Y, sourceRect, GraphicsUnit.Pixel);
		}

		private Resources GetResources(string text, bool authentic) {
			ResourceTypes types = ResourceTypes.None;

			// Determine which tubes are needed
			string[] lines = text.Split('\n');
			int maxLength = lines.Max(l => l.Length);
			int minLength = lines.Min(l => l.Length);
			if (maxLength >= 3)
				types |= ResourceTypes.TubesFull;
			else if (maxLength >= 2)
				types |= ResourceTypes.TubesSides;
			if (minLength == 1)
				types |= ResourceTypes.TubeSingle;

			// Determine which fonts are needed
			foreach (char c in text) {
				if (authentic && IsAuthentic(c))
					types |= ResourceTypes.Authentic;
				else if (IsFontA(c))
					types |= ResourceTypes.FontA;
				else if (IsFontB(c))
					types |= ResourceTypes.FontB;
			}

			return new Resources(types, Scale);
		}
	}
}
