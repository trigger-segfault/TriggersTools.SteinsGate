
namespace TriggersTools.SteinsGate {
	/// <summary>How escaping is used for divergence.</summary>
	public enum DivergenceEscape {
		/// <summary>Escaping is disabled.</summary>
		None,
		/// <summary>Only '\r' and '\n' is escaped.</summary>
		NewLines,
		/// <summary>All characters are escaped.</summary>
		All,
	}
}
