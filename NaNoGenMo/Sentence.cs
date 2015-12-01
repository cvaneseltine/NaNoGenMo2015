using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace NaNoGenMo { 
	public class Sentence {

		string text;
		Novel novel;
		//Tense.Time time;
		//Tense.Person person;
		List <Word> words = new List<Word> ();
		
		public Sentence (Novel inbound) {
			//Console.WriteLine ("Building a sentence.");
			novel = inbound;
			BuildSentence ();
		}

//		public Sentence (Novel inboundNovel, Tense.Time inboundTime) {
//			novel = inboundNovel;
//			time = inboundTime;
//			BuildSentence ();
//		}

		public int WordCount {
			get { return words.Count;}
		}

		public override string ToString () {
			return text;
		}

		Word ChooseFirstWord () {
			int total = 0;
			int current = 0;
			int choice;

			foreach (Word word in novel.FirstWords.Keys) { //Add up all the frequencies.
				total = total + novel.FirstWords[word];
			}

			choice = novel.Rand.Next (1, total + 1); //Choose a number between 0 and the frequencies.

			//Console.WriteLine ("Choosing first word: total is " + total + ", choice is " + choice + ".");

			foreach (Word word in novel.FirstWords.Keys) {
				current = current + novel.FirstWords[word];
				if (current >= choice) {
					return word;
				}
			}
			return null;
		}

		void BuildSentence () {
			Word nextWord, priorWord;
			string output = "";
			Grammar priorGrammar;
			Grammar nextGrammar;
			int count = 0;

			nextWord = ChooseFirstWord ();
			priorWord = nextWord;
			priorGrammar = novel.Grammars[priorWord.Part];

			output = (char.ToUpper (nextWord.Name[0]) + nextWord.Name.Substring (1));
			do {
				string punctuation;

				punctuation = priorGrammar.ChoosePunctuation(novel.Rand);
				output = (output + punctuation + " ");

				nextGrammar = priorGrammar.ChooseFollower (novel.Rand); //Find an appropriate word for this part of the grammar train.
				if (nextGrammar != null) {
					nextWord = ChooseByPart (nextGrammar.Part);
					words.Add (nextWord);
					if (nextWord.Part.HasFlag (Vocabulary.SpeechPart.ProperNoun)) {
						output = (output + nextWord.Name);
					}
					else {
						output = (output + char.ToLower (nextWord.Name[0]) + nextWord.Name.Substring (1));
					}
				}
				priorGrammar = nextGrammar;
				count++;
				if (count == 1000) {
					Console.WriteLine ("Sentence.BuildSentence: Loop detected.");
				}
				priorWord = nextWord;
			} while ((nextWord != null) && (priorGrammar != null));
			text = output;
		}

		Word ChooseByPart (Vocabulary.SpeechPart part) {
			List <Word> options;
			int count, choice;

			if (!novel.CrossReference.ContainsKey (part)) {
				Console.WriteLine ("No " + part + " found in CrossReference.");
				return null;
			}
			options = novel.CrossReference[part];
			count = options.Count;
			choice = novel.Rand.Next (0, count);
			return options[choice];
		}
	}
}

