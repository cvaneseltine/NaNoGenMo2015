using System;

namespace NaNoGenMo {
	public class VocabWord {
		
		string self;
		Vocabulary.SpeechPart part = Vocabulary.SpeechPart.None;

		public Vocabulary.SpeechPart Part {
			get { return part; }
		}

		public string Self {
			get { return self;}
		}

		public VocabWord (string input) {
			int splitPosition;
			char separator = '×';
			string remaining;

			splitPosition = input.IndexOf (separator);

			if (splitPosition <= 0) {
				Console.WriteLine ("Error: splitPosition <= 0 in " + input + ".");
			}
			else {
				self = input.Substring (0, splitPosition);
				remaining = input.Substring (splitPosition + 1);

				if (remaining.Contains ("N")) {
					part |= Vocabulary.SpeechPart.Noun;
				}
				if (remaining.Contains ("p")) {
					part |= Vocabulary.SpeechPart.Plural;
				}
				if (remaining.Contains ("V")) {
					part |= Vocabulary.SpeechPart.Verb;
				}
				if (remaining.Contains ("t")) {
					part |= Vocabulary.SpeechPart.TransVerb;
				}
				if (remaining.Contains ("i")) {
					part |= Vocabulary.SpeechPart.IntransVerb;
				}
				if (remaining.Contains ("A")) {
					part |= Vocabulary.SpeechPart.Adjective;
				}
				if (remaining.Contains ("v")) {
					part |= Vocabulary.SpeechPart.Adverb;
				}
				if (remaining.Contains ("C")) {
					part |= Vocabulary.SpeechPart.Conjunction;
				}
				if (remaining.Contains ("P")) {
					part |= Vocabulary.SpeechPart.Preposition;
				}
				if (remaining.Contains ("!")) {
					part |= Vocabulary.SpeechPart.Interjection;
				}
				if (remaining.Contains ("r")) {
					part |= Vocabulary.SpeechPart.Pronoun;
				}
				if (remaining.Contains ("D")) {
					part |= Vocabulary.SpeechPart.DefArticle;
				}
				if (remaining.Contains ("I")) {
					part |= Vocabulary.SpeechPart.IndefArticle;
				}
				if (remaining.Contains ("o")) {
					part |= Vocabulary.SpeechPart.Nominative;
				}
				//Console.WriteLine ("Vocab word created: " + self + ". Part of speech: " + part);
			}
		}
	}
}

