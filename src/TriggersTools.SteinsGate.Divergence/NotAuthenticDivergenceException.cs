using System;

namespace TriggersTools.SteinsGate {
	/// <summary>Thrown when input text contains non-authentic characters while
	/// <see cref="DivergenceAuthenticity.Strict"/> is used.</summary>
	public class NotAuthenticDivergenceException : Exception {
		/// <summary>The text that contained non-authentic characters.</summary>
		public string Text { get; }

		public NotAuthenticDivergenceException(string text)
			: base($"The following text does not contain only numbers and decimals: " +
				  text) {
			Text = text;
		}
	}
}
