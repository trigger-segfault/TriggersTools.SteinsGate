
namespace TriggersTools.SteinsGate {
	/// <summary>The scale to use when drawing the nixie tubes.</summary>
	public enum DivergenceScale {
		/// <summary>Each nixie tube will be 132x428px, with an additional 20px on
		/// either end.</summary>
		Large = 0,
		/// <summary>Each nixie tube will be 66x214px, with an additional 10px on
		/// either end.</summary>
		Medium = 1,
		/// <summary>Each nixie tube will be 33x107px, with an additional 6px on
		/// either end.</summary>
		Small = 2,
	}
}
