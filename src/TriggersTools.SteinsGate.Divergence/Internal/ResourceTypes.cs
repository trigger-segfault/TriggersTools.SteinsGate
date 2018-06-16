using System;

namespace TriggersTools.SteinsGate.Internal {
	[Flags]
	internal enum ResourceTypes {
		None = 0,
		Authentic = (1 << 0),
		FontA = (1 << 1),
		FontB = (1 << 2),
		TubeLeft = (1 << 3),
		TubeMid = (1 << 4),
		TubeRight = (1 << 5),
		TubeSingle = (1 << 6),
		TubesSides = TubeLeft | TubeRight,
		TubesFull = TubeLeft | TubeMid | TubeRight,
		TubesAll = TubesSides | TubeMid | TubeSingle,
		Tubes = TubeLeft | TubeMid | TubeRight | TubeRight,
		FontsAB = FontA | FontB,
		FontsAll = Authentic | FontsAB,
	}
}
