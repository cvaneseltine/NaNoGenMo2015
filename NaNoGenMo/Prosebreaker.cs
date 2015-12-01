using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;

namespace NaNoGenMo {
	public class Prosebreaker {
		string prose;
		Vocabulary vocabulary;

		Dictionary <string, Word> words = new Dictionary<string, Word> ();
		Dictionary <Word, int> firstWords = new Dictionary<Word, int> ();
		Dictionary <Vocabulary.SpeechPart, Grammar> grammars = new Dictionary<Vocabulary.SpeechPart, Grammar> ();
		Dictionary <Vocabulary.SpeechPart, List <Word>> crossReference = new Dictionary <Vocabulary.SpeechPart, List<Word>> ();
		Dictionary <string, DiscoveredChar> discoveredChars = new Dictionary <string, DiscoveredChar> ();

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

		public Dictionary <string, DiscoveredChar> DiscoveredChars {
			get { return discoveredChars;}
		}

		public string Prose {
			get {return prose;}
		}

		public Prosebreaker (string dictPath) {
			vocabulary = new Vocabulary (dictPath);
		}

		public void BreakProse (string path) {
			//Breaks the original prose down into individual sentences and passes them for processing.

			byte [] sourceBytes;
			byte [] unicodeBytes;
			int sentenceCount = 0;
			string sentence = "";
			int lastBreakpoint = 0;
			bool openQuote = true;

			if (!File.Exists (path)) {
				Console.WriteLine ("Can't find a file at " + path + ".");
				return;
			}

			sourceBytes = File.ReadAllBytes (path);

			unicodeBytes = Encoding.Convert (Encoding.GetEncoding(1252), Encoding.Unicode, sourceBytes);
			prose = Encoding.Unicode.GetString (unicodeBytes);

			Console.WriteLine ("Successfully read from " + path + ".");

			prose.Trim();
			prose = prose.Replace ("\r", " ").Replace("\n", " ");

			for (int i = 0; i < prose.Length; i++) { //Run through the entire prose.
				bool sentenceEnd = false;

				if ((prose[i] == '"') && (openQuote)) {
					sentenceEnd = true;
					openQuote = false;
				}
				else if ((prose[i] == '.') || (prose[i] == '!') || (prose[i] == '?') || (prose[i] == '"')) {
					sentenceEnd = true;
				}
				if (sentenceEnd) {
					sentence = (prose.Substring (lastBreakpoint, i - lastBreakpoint + 1)).Trim ();
					//Console.WriteLine ("Breaking sentence #" + sentenceCount + ": " + sentence);
					BreakSentence (sentence);
					sentenceCount++;
					lastBreakpoint = i + 1;
				}
			}
		}

		public void BreakSentence (string input) {
			//Breaks down an inbound sentence.
			string [] stringArray;
			List <string> validStrings = new List <string> (); //First string is words, second string is punctuation.
			Word nextWord = null;
			Word priorWord = null;
			bool isFirst = true;
			string sentenceReport = "";
			Dictionary <Vocabulary.SpeechPart, Grammar> grammarTarget = new Dictionary<Vocabulary.SpeechPart, Grammar> ();
			Grammar nextGrammar = null;

			int wordCount = 1;
			DiscoveredChar lastCharacter = null;

			stringArray = input.Split (null); //splits to whitespace characters

			foreach (string testString in stringArray) { //Test each string to ensure it will process correctly, and kick out strings that won't.
				bool foundValid = false;

				foreach (char c in testString) {
					if (Char.IsLetter (c)) {
						foundValid = true;
					}
				}

				if (testString.Length == 0) { //Zero length strings get booted
					foundValid = false;
				}

				if (foundValid) {
					validStrings.Add (testString);
				}
			}

			foreach (string s in validStrings) {
				string nextString = "";
				string punctuation = "";

				nextString = SplitPunctuation (s, out punctuation);
				//Console.WriteLine ("Recovered " + punctuation + " after " + nextString);

				nextString = ReduceToBase (nextString);

				if (words.ContainsKey (nextString)) { //If the word is already in the dictionary, just increment the frequency.
					nextWord = words[nextString];
					nextWord.Frequency++;
				}
				else { //If the word isn't in the dictionary yet, add it in.
					nextWord = new Word (nextString, isFirst, vocabulary);
					words.Add (nextString, nextWord);
					if (!crossReference.ContainsKey (nextWord.Part)) {
						crossReference.Add (nextWord.Part, new List <Word> ());
					}
					crossReference [nextWord.Part].Add (nextWord);
				}

				if (isFirst) { //Add the new first word into the first words dictionary
					if (firstWords.ContainsKey (nextWord)) {
						firstWords[nextWord] = firstWords[nextWord]++;
					}
					else {
						firstWords.Add (nextWord, 1);
					}
					grammarTarget = grammars; //Begin at the top of the grammar map
				}
				else {
					//Console.WriteLine (nextWord.Name + " is not first (pos: " + nextWord.Part);
					if (priorWord != null) {
						priorWord.AddFollower (nextWord);
					}
					if ((nextWord.Part == Vocabulary.SpeechPart.None)) { //Searching for characters in the source text 
						if (VerifyProperNoun (nextWord.Name)) {
							nextWord.Part |= Vocabulary.SpeechPart.ProperNoun;
							lastCharacter = new DiscoveredChar (nextWord.Name);
							discoveredChars.Add (nextWord.Name, lastCharacter);
						}
						if (!crossReference.ContainsKey (nextWord.Part)) {
							crossReference.Add (nextWord.Part, new List <Word> ());
						}
						crossReference [nextWord.Part].Add (nextWord);
					}
				}
					
				sentenceReport = (sentenceReport + "[" + nextWord.Name + "-->" + nextWord.Part);
				if (grammarTarget.ContainsKey (nextWord.Part)) { //If there's an existing grammar map, just continue along it
					sentenceReport = (sentenceReport + ", existing]\n");
					nextGrammar = grammarTarget [nextWord.Part];
				}
				else { //Otherwise, start a new grammar map
					sentenceReport = (sentenceReport + "-->new branch]\n");
					nextGrammar = new Grammar (nextWord, wordCount);
					grammarTarget.Add (nextWord.Part, nextGrammar);
				}
				nextGrammar.AddPunctation (punctuation); //Record the punctuation following this word
				//Console.WriteLine ("Recorded " + punctuation + " after " + nextGrammar.Part);
				grammarTarget = nextGrammar.Followers;

				wordCount++; //Tracking this point on the grammar chain

				isFirst = false;
			}
			//Console.WriteLine (sentenceReport);
		}

		string SplitPunctuation (string inbound, out string punctuation) {
			string outbound = "";
			int position;
			List<string> words = new List<string>();

			if (inbound.Length <= 1) {
				punctuation = "";
				return inbound;
			}
			position = inbound.Length - 1;
			while (!Char.IsLetterOrDigit (inbound[position])) {
				position--;
				if (position == 0) {
					punctuation = "";
					return "";
				}
			}
			if (position == (inbound.Length - 1)) { //No punctuation discovered.
				outbound = inbound;
				punctuation = "";
			}
			else {
				outbound = inbound.Substring (0, position + 1);
				punctuation = inbound.Substring (position + 1);
				//Console.WriteLine ("SplitPunctuation divided '" + inbound + "' into '" + outbound + "' and '" + punctuation + "'");
			}
			return outbound;
		}

		string ReduceToBase (string inbound) { //Attempts to reduce words into more useful forms
			string tail, reduced, modified;

			if (vocabulary.WordExists (inbound)) { //No changes needed.
				return inbound;
			}

			modified = inbound.ToLower ();
			if (vocabulary.WordExists (modified)) { //Already useful!
				return modified;
			}

			if (modified.Length <= 1) {
				return modified;
			}

			while (!Char.IsLetterOrDigit (modified [modified.Length - 1])) {
				modified = modified.Remove (modified.Length - 1);
			}
		
			tail = modified.Substring (modified.Length - 1);
			if ((tail == "s") || (tail == "d")) {
				reduced = modified.Remove (modified.Length - 1);

				if (vocabulary.WordExists (reduced)) {
					//Console.WriteLine (inbound + " reduced to " + reduced + ".");
					return reduced;
				}
			}

			if (modified.Length <= 2) {
				return modified;
			}

			tail = modified.Substring (modified.Length - 2);
			if ((tail == "ed") || (tail == "es") || (tail == "'s") || (tail == "ly")) {
				reduced = modified.Remove (modified.Length - 2);

				if (vocabulary.WordExists (reduced)) {
					//Console.WriteLine (inbound + " reduced to " + reduced + ".");
					return reduced;
				}
			}

			if (modified.Length <= 3) {
				return modified;
			}

			tail = modified.Substring (modified.Length - 3);
			if ((tail == "ing")) {
				reduced = modified.Remove (modified.Length - 3);

				if (vocabulary.WordExists (reduced)) {
					//Console.WriteLine (inbound + " reduced to " + reduced + ".");
					return reduced;
				}
			}
			//Console.WriteLine ("Unable to reduce '" + modified + "' to a more useful form. Returning " + inbound);
			return inbound;
		}

		bool VerifyProperNoun (string inbound) {
			Regex regex;

			regex = new Regex ("[A-Z]");

			//Console.Write ("\nTesting '" + inbound + "' for proper noun status... ");

			if (inbound.Length <= 1) {
				//Console.WriteLine ("Rejected: too short.\n");
				return false;
			}
			if (!regex.Match (inbound.Remove (1)).Success) {
				//Console.WriteLine ("Rejected: doesn't start with a capital letter.\n");
				return false;
			}
			if (vocabulary.WordExists (inbound.ToLower())) {
				//Console.WriteLine ("Rejected: appears in dictionary.");
				return false;
			}
			//Console.WriteLine ("VerifyProperNoun: " + inbound + " accepted.\n");
			return true;
		}

		DiscoveredChar.PronounSet DeterminePronoun (string inbound) {
			switch (inbound) {
				case "he":
				case "him":
				case "his":
				case "himself":
					return DiscoveredChar.PronounSet.He;
				case "she":
				case "her":
				case "hers":
				case "herself":
					return DiscoveredChar.PronounSet.She;
				case "they":
				case "them":
				case "theirs":
				case "themselves":
				case "themself":
					return DiscoveredChar.PronounSet.They;
				case "it":
				case "its":
				case "itself":
					return DiscoveredChar.PronounSet.It;
				default:
					return DiscoveredChar.PronounSet.Unassigned;
			}
		}
	}
}

