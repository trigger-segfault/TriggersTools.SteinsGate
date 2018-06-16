
namespace TriggersTools.SteinsGate {
	/// <summary>The type of font used for drawing.</summary>
	public enum DivergenceAuthenticity {
		/// <summary>Use Steins;Gate characters when possible, otherwise use Oslo II.</summary>
		Lax,
		/// <summary>Only use characters taken from Steins;Gate. (.0123456789)
		/// Throws an exception when invalid characters are present.</summary>
		Strict,
		/// <summary>Only use Oslo II.</summary>
		None,
		/// <summary>Decide based on the characters present in the text.</summary>
		Decide,
	}
}
