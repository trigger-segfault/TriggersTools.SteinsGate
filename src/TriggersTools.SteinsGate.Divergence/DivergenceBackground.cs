using System.Drawing;
using System.IO;

namespace TriggersTools.SteinsGate {
	/// <summary>Instructions on how to draw the background.</summary>
	public struct DivergenceBackground {

		public static readonly DivergenceBackground None = new DivergenceBackground();

		/// <summary>If set, the background will be loaded from file and drawn.
		/// Overrides <see cref="Bitmap"/>.</summary>
		public string BitmapFile { get; set; }
		/// <summary>If set, the background to draw.
		/// Overridden by <see cref="BitmapFile"/>.</summary>
		public Bitmap Bitmap { get; set; }
		/// <summary>If set, the background color to use.</summary>
		public Color? Color { get; set; }

		/// <summary>If true, the bitmap will be scaled to fit in the output with at
		/// least one dimension.</summary>
		public bool ScaleBitmap { get; set; }

		/// <summary>Gets the file name of <see cref="BitmapFile"/>. Returns null if
		/// <see cref="BitmapFile"/> is nll.</summary>
		public string BitmapFileName {
			get {
				if (BitmapFile == null)
					return null;
				return Path.GetFileName(BitmapFile);
			}
		}

		public DivergenceBackground(Color color) : this() {
			Color = color;
		}

		public DivergenceBackground(int r, int g, int b, int a = 255) : this() {
			Color = System.Drawing.Color.FromArgb(a, r, g, b);
		}

		public DivergenceBackground(Bitmap bitmap, bool scale = true) : this() {
			Bitmap = bitmap;
			ScaleBitmap = scale;
		}

		public DivergenceBackground(string bitmapFile, bool scale = true) : this() {
			BitmapFile = bitmapFile;
			ScaleBitmap = scale;
		}

		public static implicit operator DivergenceBackground(Color color) {
			return new DivergenceBackground {
				Color = color,
			};
		}

		public static implicit operator DivergenceBackground(Bitmap bitmap) {
			return new DivergenceBackground {
				Bitmap = bitmap,
				ScaleBitmap = true,
			};
		}

		public static implicit operator DivergenceBackground(string bitmapFile) {
			return new DivergenceBackground {
				BitmapFile = bitmapFile,
				ScaleBitmap = true,
			};
		}

		public override string ToString() {
			string str = "";
			if (Color.HasValue) {
				str += $"Color={Color.Value}";
			}
			if (Bitmap != null || BitmapFile != null) {
				if (!string.IsNullOrEmpty(str))
					str += " ";
				str += "Background=";
				if (Bitmap != null)
					str += $"{Bitmap.Width}x{Bitmap.Height}";
				if (BitmapFile != null)
					str += $"{BitmapFileName}";
				if (ScaleBitmap)
					str += " (scaled)";
			}
			if (string.IsNullOrEmpty(str))
				str += "None";
			return str;
		}
	}
}
