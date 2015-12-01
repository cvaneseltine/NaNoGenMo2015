using System;
using System.Collections.Generic;

namespace NaNoGenMo {
	public class Novel {
		//Despite the name, a "novel" actually contains the working vocabulary for the original sample text.

		Random rand;

		List <Stat> stats = new List <Stat> ();
		List <Character> characters = new List <Character> ();

		Dictionary <string, Word> words = new Dictionary<string, Word> ();
		Dictionary <Word, int> firstWords = new Dictionary<Word, int> ();
		Dictionary <Vocabulary.SpeechPart, List <Word>> crossReference = new Dictionary <Vocabulary.SpeechPart, List<Word>> ();
		Dictionary <Vocabulary.SpeechPart, Grammar> grammars = new Dictionary<Vocabulary.SpeechPart, Grammar> ();

		public Dictionary <string, Word> Words {
			get { return words;}
		}

		public Dictionary <Word, int> FirstWords {
			get { return firstWords;}
		}

		public Dictionary <Vocabulary.SpeechPart, List <Word>> CrossReference {
			get { return crossReference;}
		}

		public Dictionary <Vocabulary.SpeechPart, Grammar> Grammars {
			get { return grammars;}
		}

		public List <Stat> Stats {
			get { return stats;}
		}

		public List <Character> Characters {
			get { return characters;}
		}

		public Random Rand {
			get { return rand;}
		}

		public Novel (Prosebreaker inbound) {
			rand = new Random ();
			words = inbound.Words;
			firstWords = inbound.FirstWords;
			crossReference = inbound.CrossReference;
			grammars = inbound.Grammars;
			string statReport = "";
			int charCount = 0;

			Console.WriteLine ("Building a novel.");

			foreach (DiscoveredChar thisChar in inbound.DiscoveredChars.Values) {
				thisChar.ResolvePronouns ();
			}

			characters.AddRange (inbound.DiscoveredChars.Values);

			foreach (DiscoveredChar thisChar in characters) {
				charCount++;
				thisChar.Number = charCount;
			}

			for (int i = 0; i < 4; i++) {
				charCount++;
				characters.Add (new Character (charCount));
			}

			for (int i = 0; i < 4; i++) {
				bool duplicate = false;
				Stat stat;

				stat = new Stat (crossReference [Vocabulary.SpeechPart.Adjective], rand);

				foreach (Stat thisStat in stats) {
					if (stat.Name == thisStat.Name) {
						duplicate = true;
					}
				}

				if (!duplicate) {
					stats.Add (stat);
					if (stat.Opposed == null) {
						statReport = (statReport + stat.Name + " ");
					}
					else {
						statReport = (statReport + stat.Name + " (vs " + stat.Opposed + ") ");
					}
				}
			}
		}
	}
}

