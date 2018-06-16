using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;

namespace TriggersTools.SteinsGate.Divergence.Generator {
	class DisposableBitmapData : IDisposable {

		public Bitmap Bitmap { get; }
		public BitmapData Data { get; }

		public DisposableBitmapData(Bitmap bitmap, ImageLockMode mode, PixelFormat? format = null) {
			Bitmap = bitmap;
			Data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), mode, format ?? bitmap.PixelFormat);
		}

		public DisposableBitmapData(Bitmap bitmap, BitmapData data) {
			Bitmap = bitmap;
			Data = data;
		}

		public static implicit operator BitmapData(DisposableBitmapData data)
			=> data.Data;

		public void Dispose() {
			Bitmap.UnlockBits(Data);
		}
	}
	
	static class BitmapExtensions {

		public static DisposableBitmapData LockRead(this Bitmap bitmap, PixelFormat? format = null) {
			return new DisposableBitmapData(bitmap, ImageLockMode.ReadOnly, format);
		}

		public static DisposableBitmapData LockWrite(this Bitmap bitmap, PixelFormat? format = null) {
			return new DisposableBitmapData(bitmap, ImageLockMode.WriteOnly, format);
		}

		public static DisposableBitmapData LockReadWrite(this Bitmap bitmap, PixelFormat? format = null) {
			return new DisposableBitmapData(bitmap, ImageLockMode.ReadWrite, format);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static void SetColor(this BitmapData data, Point point, Color color)
			=> data.SetColor(point.X, point.Y, color);

		public unsafe static void SetColor(this BitmapData data, int x, int y, Color color) {
			bool alpha = data.HasAlpha();
			byte* ptr = data.PtrAt(x, y);
			if (alpha) {
				ptr[2] = color.R;
				ptr[1] = color.G;
				ptr[0] = color.B;
				ptr[3] = color.A;
			}
			else {
				ptr[2] = color.R;
				ptr[1] = color.G;
				ptr[0] = color.B;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static Color GetColor(this BitmapData data, Point point)
			=> GetColor(data, point.X, point.Y);

		public unsafe static Color GetColor(this BitmapData data, int x, int y) {
			bool alpha = data.HasAlpha();
			byte* ptr = data.PtrAt(x, y);
			if (alpha)
				return Color.FromArgb(ptr[3], ptr[2], ptr[1], ptr[0]);
			else
				return Color.FromArgb(ptr[2], ptr[1], ptr[0]);
		}

		public unsafe static bool HasAlpha(this BitmapData data) {
			return Image.IsAlphaPixelFormat(data.PixelFormat);
		}

		public static int GetBytesPerPixel(this BitmapData data)
			=> Image.GetPixelFormatSize(data.PixelFormat) / 8;
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private unsafe static byte* PtrAt(this BitmapData data, Point point)
			=> data.PtrAt(point.X, point.Y);

		private unsafe static byte* PtrAt(this BitmapData data, int x, int y) {
			int byteDepth = data.GetBytesPerPixel();
			return ((byte*) data.Scan0) + (y * data.Stride) + x * byteDepth;
		}

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
		}
	}
}
