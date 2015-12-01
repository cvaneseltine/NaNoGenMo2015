using System;
using System.Collections.Generic;

namespace NaNoGenMo {
	public class Paragraph {

		string text = "";
		List <Sentence> sentences = new List<Sentence> ();

		public Paragraph (Novel inbound) {
			int quotes = 0;

			for (int i = 0; i <= 3; i++) {
				Sentence nextSentence = new Sentence (inbound);
				sentences.Add (nextSentence);
				text = (text + " " + nextSentence.ToString ());
			}
			Smooth ();
		}

		void Smooth () {
			string output = "";
			bool capitalFound = false;
			bool quoteOpen = false;

			for (int i = 0; i < text.Length; i++) {
				char nextChar;

				nextChar = text[i];
				if (Char.IsLetter(nextChar) && !capitalFound) {
					nextChar = Char.ToUpper (nextChar);
					capitalFound = true;
				}
				if ((nextChar == '.') || (nextChar == '?') || (nextChar == '!') || (nextChar == '"')) {
					capitalFound = false;
				}
				output = (output + nextChar);
			}
			output = output.Trim();
			text = output;
		}

		public override string ToString () {
			return text;
		}
	}
}

