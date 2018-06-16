using System;
using System.Drawing;

namespace TriggersTools.SteinsGate.Internal {
	internal class BitmapRef : IDisposable {

		public Bitmap Bitmap { get; }
		public ResourceTypes Type { get; }
		public DivergenceScale Scale { get; }

		public int RefCount { get; private set; }
		public bool IsDisposed { get; private set; }

		public bool IsReferenced => RefCount > 0;

		public BitmapRef(Bitmap bitmap, ResourceTypes type, DivergenceScale scale) {
			Bitmap = bitmap;
			Type = type;
			Scale = scale;
			RefCount = 0;
			IsDisposed = false;
		}

		public void AddRef() {
			RefCount++;
		}

		public void RemoveRef() {
			if (RefCount == 0)
				throw new InvalidOperationException("Reference count cannot go below zero!");
			RefCount--;
			if (!IsReferenced)
				Resources.DisposeOf(this);
		}

		public void Dispose() {
			if (!IsDisposed) {
				Bitmap.Dispose();
				IsDisposed = true;
			}
		}
	}
}
