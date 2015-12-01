using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace NaNoGenMo {
	public class Vocabulary {

		string rawtext;
		Dictionary <string, VocabWord> vocabWords = new Dictionary<string, VocabWord> ();

		[Flags]
		public enum SpeechPart {
			None = 0,
			Noun =       1<<0,
			Plural =      1<<1,
			Verb =        1<<2,
			TransVerb =   1<<3,
			IntransVerb = 1<<4, 
			Adjective =   1<<5, 
			Adverb =      1<<6,
			Conjunction = 1<<7, 
			Preposition = 1<<8, 
			Interjection = 1<<9,
			Pronoun =     1<<10,
			DefArticle =  1<<11,
			IndefArticle = 1<<12,
			Nominative =  1<<13,
			ProperNoun =  1<<14
		}

		public Vocabulary (string path) {
			byte [] sourceBytes;
			byte [] unicodeBytes;

			sourceBytes = File.ReadAllBytes (path);
			unicodeBytes = Encoding.Convert (Encoding.GetEncoding(1252), Encoding.Unicode, sourceBytes);

			rawtext = Encoding.Unicode.GetString (unicodeBytes);

			PrepVocab ();
		}

		void PrepVocab () {
			StringReader reader;
			string nextLine;

			reader = new StringReader (rawtext);

			while (true) {
				nextLine = reader.ReadLine ();
				if (nextLine == null) {
					break;
				}
				if (!nextLine.Contains (" ")) {
					VocabWord vocab;

					vocab = new VocabWord (nextLine);
					if (!vocabWords.ContainsKey (vocab.Self)) { //Original dictionary has at least one duplicate.
						vocabWords.Add (vocab.Self, vocab);
					}
				}
			}
			//WriteToFile (); //This writes a copy of all the vocab words back to a file. It takes forever, don't activate it without a reason.
		}

		public SpeechPart GetSpeechPart (string input) {

			if (vocabWords.ContainsKey (input)) {
				return vocabWords [input].Part;
			}
			return Vocabulary.SpeechPart.None;
		}

		public bool WordExists (string input) {
			if (vocabWords.ContainsKey (input)) {
				//Console.WriteLine ("'" + input + "' not contained in vocabWords.");
				return true;
			}
			if (vocabWords.ContainsKey (input.ToLower())) {
				return true;
			}
			return false;
		}

		void WriteToFile () {
			string path = "vocabulary.txt";
			string record = "";
			int count = 0;

			foreach (VocabWord thisVocab in vocabWords.Values) {
				record = (record + thisVocab.Self + " - " + PartToString (thisVocab.Part) + "\n");
				count++;
				if ((count % 10000) == 0) {
					Console.WriteLine (count + " vocab words processed for vocabulary.txt.");
					File.AppendAllText (path, record, System.Text.Encoding.Default);
					record = "";
				}
			}
			File.AppendAllText (path, record, System.Text.Encoding.Default);
		}


		static public string PartToString (SpeechPart part) {
			string output = "";

			if (part.HasFlag (Vocabulary.SpeechPart.Adjective)) {
				if (output != "") {
					output = output + "/";
				}
				output = output + "Adjective";
			}
			if (part.HasFlag (Vocabulary.SpeechPart.Adverb)) {
				if (output != "") {
					output = output + "/";
				}
				output = output + "Adverb";
			}
			if (part.HasFlag (Vocabulary.SpeechPart.Conjunction)) {
				if (output != "") {
					output = output + "/";
				}
				output = output + "Conjunction";
			}
			if (part.HasFlag (Vocabulary.SpeechPart.DefArticle)) {
				if (output != "") {
					output = output + "/";
				}
				output = output + "DefArticle";
			}
			if (part.HasFlag (Vocabulary.SpeechPart.IndefArticle)) {
				if (output != "") {
					output = output + "/";
				}
				output = output + "IndefArticle";
			}
			if (part.HasFlag (Vocabulary.SpeechPart.Interjection)) {
				if (output != "") {
					output = output + "/";
				}
				output = output + "Interjection";
			}
			if (part.HasFlag (Vocabulary.SpeechPart.IntransVerb)) {
				if (output != "") {
					output = output + "/";
				}
				output = output + "Intransverb";
			}
			if (part.HasFlag (Vocabulary.SpeechPart.Nominative)) {
				if (output != "") {
					output = output + "/";
				}
				output = output + "Nominative";
			}
			if (part.HasFlag (Vocabulary.SpeechPart.Noun)) {
				if (output != "") {
					output = output + "/";
				}
				output = output + "Noun";
			}
			if (part.HasFlag (Vocabulary.SpeechPart.Plural)) {
				if (output != "") {
					output = output + "/";
				}
				output = output + "Plural";
			}
			if (part.HasFlag (Vocabulary.SpeechPart.Preposition)) {
				if (output != "") {
					output = output + "/";
				}
				output = output + "Preposition";
			}
			if (part.HasFlag (Vocabulary.SpeechPart.Pronoun)) {
				if (output != "") {
					output = output + "/";
				}
				output = output + "Pronoun";
			}
			if (part.HasFlag (Vocabulary.SpeechPart.TransVerb)) {
				if (output != "") {
					output = output + "/";
				}
				output = output + "TransVerb";
			}
			if (part.HasFlag (Vocabulary.SpeechPart.Verb)) {
				if (output != "") {
					output = output + "/";
				}
				output = output + "Verb";
			}
			if (output == "") {
				output = "Unknown";
			}

			return output;
		}
	}
}

