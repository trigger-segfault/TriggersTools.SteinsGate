using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace TriggersTools.SteinsGate.Internal {
	internal class Resources : IDisposable {
		
		private static Dictionary<ResourceTypes, BitmapRef>[] resourceScales;
		
		static Resources() {
			resourceScales = new Dictionary<ResourceTypes, BitmapRef>[3];
			for (int i = 0; i < resourceScales.Length; i++)
				resourceScales[i] = new Dictionary<ResourceTypes, BitmapRef>();
		}

		private static Dictionary<ResourceTypes, BitmapRef> GetDictionary(DivergenceScale scale) {
			return resourceScales[(int) scale];
		}

		public static string GetResource(DivergenceScale scale, ResourceTypes type) {
			return Embedding.Combine("Resources", $"{scale}", $"Nixie{type}.png");
		}
		
		private static BitmapRef Load(ResourceTypes type, DivergenceScale scale) {
			Stream stream = Embedding.Get(GetResource(scale, type));
			Bitmap bitmap = (Bitmap) Image.FromStream(stream);
			return new BitmapRef(bitmap, type, scale);
		}

		private static BitmapRef Get(ResourceTypes type, DivergenceScale scale) {
			BitmapRef bitmapRef;
			var resources = GetDictionary(scale);
			lock (resources) {
				if (!resources.TryGetValue(type, out bitmapRef)) {
					bitmapRef = Load(type, scale);
					resources.Add(type, bitmapRef);
				}
				bitmapRef.AddRef();
			}
			return bitmapRef;
		}

		public static bool DisposeOf(BitmapRef bitmapRef) {
			var resources = GetDictionary(bitmapRef.Scale);
			// Prevent other operations from getting accessing the bitmap
			lock (resources) {
				// In the time it took to acquire the lock,
				// make sure the bitmap was not referenced again.
				if (!bitmapRef.IsReferenced) {
					resources.Remove(bitmapRef.Type);
					bitmapRef.Dispose();
					return true;
				}
			}
			return false;
		}
		
		public Resources(ResourceTypes types, DivergenceScale scale) {
			Types = types;
			Scale = scale;
			authentic = GetIf(ResourceTypes.Authentic);
			fontA = GetIf(ResourceTypes.FontA);
			fontB = GetIf(ResourceTypes.FontB);
			tubeLeft = GetIf(ResourceTypes.TubeLeft);
			tubeMid = GetIf(ResourceTypes.TubeMid);
			tubeRight = GetIf(ResourceTypes.TubeRight);
			tubeSingle = GetIf(ResourceTypes.TubeSingle);
		}

		private BitmapRef GetIf(ResourceTypes flag) {
			return (Types.HasFlag(flag) ? Get(flag, Scale) : null);
		}

		public DivergenceScale Scale { get; }
		public ResourceTypes Types { get; }

		private readonly BitmapRef authentic;
		private readonly BitmapRef fontA;
		private readonly BitmapRef fontB;
		private readonly BitmapRef tubeLeft;
		private readonly BitmapRef tubeMid;
		private readonly BitmapRef tubeRight;
		private readonly BitmapRef tubeSingle;

		public Bitmap Authentic => authentic.Bitmap;
		public Bitmap FontA => fontA.Bitmap;
		public Bitmap FontB => fontB.Bitmap;
		public Bitmap TubeLeft => tubeLeft.Bitmap;
		public Bitmap TubeMid => tubeMid.Bitmap;
		public Bitmap TubeRight => tubeRight.Bitmap;
		public Bitmap TubeSingle => tubeSingle.Bitmap;

		public void Dispose() {
			authentic?.RemoveRef();
			fontA?.RemoveRef();
			fontB?.RemoveRef();
			tubeLeft?.RemoveRef();
			tubeMid?.RemoveRef();
			tubeRight?.RemoveRef();
			tubeSingle?.RemoveRef();
		}
	}
}
