using System.Drawing;

namespace TriggersTools.SteinsGate {
	/// <summary>The spacing to use when drawing the nixie tubes.</summary>
	public struct DivergenceSpacing {

		/// <summary>All spacing is set to zero.</summary>
		public static readonly DivergenceSpacing Empty = new DivergenceSpacing();

		/// <summary>The spacing between the left nixie tubes and the border.</summary>
		public int Left { get; set; }
		/// <summary>The spacing between the top nixie tubes and the border.</summary>
		public int Top { get; set; }
		/// <summary>The spacing between the right nixie tubes and the border.</summary>
		public int Right { get; set; }
		/// <summary>The spacing between the bottom nixie tubes and the border.</summary>
		public int Bottom { get; set; }
		/// <summary>The spacing between nixie tube lines.</summary>
		public int Line { get; set; }

		/// <summary>The spacing between the top left nixie tubes and the border.</summary>
		public Size TopLeft {
			get => new Size(Left, Top);
			set {
				Left = value.Width;
				Top = value.Height;
			}
		}

		/// <summary>The spacing between the bottom right nixie tubes and the border.</summary>
		public Size BottomRight {
			get => new Size(Right, Bottom);
			set {
				Right = value.Width;
				Bottom = value.Height;
			}
		}

		/// <summary>The spacing between the bottom left nixie tubes and the border.</summary>
		public Size BottomLeft {
			get => new Size(Left, Bottom);
			set {
				Left = value.Width;
				Bottom = value.Height;
			}
		}

		/// <summary>The spacing between the top right nixie tubes and the border.</summary>
		public Size TopRight {
			get => new Size(Right, Top);
			set {
				Right = value.Width;
				Top = value.Height;
			}
		}

		/// <summary>The total spacing between the all nixie tubes and the border.</summary>
		public Size Total => new Size(Left + Right, Top + Bottom);

		/// <summary>The total spacing between the horizontal nixie tubes and the
		/// border.</summary>
		public int Horizontal => Left + Right;

		/// <summary>The total spacing between the vertical nixie tubes and the border.</summary>
		public int Vertical => Top + Bottom;

		public DivergenceSpacing(int total, int line) {
			Left = total;
			Top = total;
			Right = total;
			Bottom = total;
			Line = line;
		}

		public DivergenceSpacing(int horizontal, int vertical, int line) {
			Left = horizontal;
			Top = vertical;
			Right = horizontal;
			Bottom = vertical;
			Line = line;
		}

		public DivergenceSpacing(int left, int top, int right, int bottom, int line) {
			Left = left;
			Top = top;
			Right = right;
			Bottom = bottom;
			Line = line;
		}

		public DivergenceSpacing(Size sides, int line) {
			Left = sides.Width;
			Top = sides.Height;
			Right = sides.Width;
			Bottom = sides.Height;
			Line = line;
		}

		public DivergenceSpacing(Size topLeft, Size bottomRight, int line) {
			Left = topLeft.Width;
			Top = topLeft.Height;
			Right = bottomRight.Width;
			Bottom = bottomRight.Height;
			Line = line;
		}

		public override string ToString() {
			return	$"Left={Left} Top={Top} " +
					$"Right={Right} Bottom={Bottom} " +
					$"Line={Line}";
		}
	}
}
