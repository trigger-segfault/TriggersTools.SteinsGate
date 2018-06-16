using System;
using System.Drawing;
using System.Linq;
using System.Text;

namespace TriggersTools.SteinsGate {
	/// <summary>Arguments used to customize the nixie tube drawing.</summary>
	public struct DivergenceArgs {
		/// <summary>The default divergence arguments.</summary>
		public static readonly DivergenceArgs Default = new DivergenceArgs();

		/// <summary>The default divergence arguments for a large black background
		/// output.</summary>
		public static readonly DivergenceArgs LargeBlack = new DivergenceArgs {
			Background = Color.Black,
			Scale = DivergenceScale.Large,
		};

		/// <summary>The default divergence arguments for a medium black background
		/// output.</summary>
		public static readonly DivergenceArgs MediumBlack = new DivergenceArgs {
			Background = Color.Black,
			Scale = DivergenceScale.Medium,
		};

		/// <summary>The default divergence arguments for a small black background
		/// output.</summary>
		public static readonly DivergenceArgs SmallBlack = new DivergenceArgs {
			Background = Color.Black,
			Scale = DivergenceScale.Small,
		};

		/// <summary>The default divergence arguments for a medium output.</summary>
		public static readonly DivergenceArgs Medium = new DivergenceArgs {
			Scale = DivergenceScale.Medium,
		};

		/// <summary>The default divergence arguments for a small output.</summary>
		public static readonly DivergenceArgs Small = new DivergenceArgs {
			Scale = DivergenceScale.Small,
		};

		// General:
		/// <summary>The spacing to use between nixie tubes.</summary>
		public DivergenceSpacing Spacing { get; set; }
		/// <summary>The scale of the output nixie tubes.</summary>
		public DivergenceScale Scale { get; set; }
		/// <summary>The authenticity of the nixie tube font.</summary>
		public DivergenceAuthenticity Authenticity { get; set; }
		/// <summary>The optional background for the nixie tubes.</summary>
		public DivergenceBackground Background { get; set; }

		// Alignment:
		/// <summar>The alignment of the nixie tube lines that are less than the max
		/// length.</summar>
		public StringAlignment Alignment { get; set; }
		/// <summary>If true, all nixie tube lines will have the same width by padding
		/// them with spaces. The padding will follow the rules of
		/// <see cref="Alignment"/>.</summary>
		public bool UsePadding { get; set; }
		/// <summary>If true, when using Center Alignment, tubes will always line up
		/// vertically. This has no effect when <see cref="UsePadding"/> is true.</summary>
		public bool AlignTubes { get; set; }

		// Parsing:
		/// <summary>How escaping is handled while formatting the text.</summary>
		public DivergenceEscape Escape { get; set; }
	}
}
