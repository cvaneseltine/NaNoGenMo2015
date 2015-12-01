using System;
using System.Collections.Generic;

namespace NaNoGenMo {
	public class Word {

		string name;
		Vocabulary.SpeechPart part;
		int frequency;
		int firstCount;
		Dictionary<Word, int> followers = new Dictionary<Word, int> ();

		public Vocabulary.SpeechPart Part {
			get { return part;}
			set { part = value;}
		}

		public string Name {
			get {return name;}
		}

		public int Frequency {
			get {return frequency;}
			set {frequency = value;}
		}
		
		public Word (string inbound, bool first, Vocabulary vocabulary) {
			if (inbound.Length == 0) {
				Console.WriteLine ("STOP STOP STOP STOP STOP");
			}
			name = inbound;
			part = vocabulary.GetSpeechPart (name);
			frequency++;
			if (first) {
				firstCount++;
			}
		}

		public void AddFollower (Word follower) {
			if (follower == null) {
				Console.WriteLine ("Word.AddFollower called on " + name + " with a null.");
			}
			if (followers.ContainsKey (follower)) {
				followers[follower]++;
			}
			else {
				followers.Add (follower, 1);
			}
		}

		public string ReportFollowers () {
			string output = "followers: ";
			foreach (Word follower in followers.Keys) {
				output = (output + " " + follower.Name + "(" + followers[follower] + "); ");
			}
			return output;
		}

//		public string ReportPunctuation () {
//			string output = "punctuation: ";
//			if (punctuations.Count > 0) {
//				foreach (string punctuation in punctuations.Keys) {
//					output = (output + " '" + punctuation + "' (" + punctuations[punctuation] + "); ");
//				}
//			}
//			else {
//				output = (output + " none");
//			}
//			return output;
//		}

		public Word ChooseFollower (Dictionary <Word, int> possibleFollowers, Random random) {
			int total = 0;
			int current = 0;
			int choice;

			if (possibleFollowers == null) {
				possibleFollowers = followers;
			}

			if (possibleFollowers.Count == 0) { //End of sentence.
				return null;
			}

			foreach (Word word in possibleFollowers.Keys) { //Add up all the frequencies.
				total = total + possibleFollowers[word];
			}

			choice = random.Next (1, total + 1); //Choose a number between 0 and the frequencies.

			//Console.WriteLine ("Choosing word to follow " + this.self + ": total is " + total + ", choice is " + choice + ".");

			foreach (Word word in possibleFollowers.Keys) {
				current = current + possibleFollowers[word];
				if (current >= choice) {
					return word;
				}
			}

			return null;
		}

		public Word ChooseByGrammar (Vocabulary.SpeechPart nextPart, Random random) {
			Dictionary <Word, int> possibleFollowers = new Dictionary<Word, int> ();

			foreach (Word word in followers.Keys) {
				foreach (Vocabulary.SpeechPart part in Enum.GetValues (typeof(Vocabulary.SpeechPart))) {
					if (word.Part.HasFlag (part)) {
						//Console.Write (" YES " + word.Self + " as " + part + ".");
						possibleFollowers.Add (word, followers [word]);
						break;
					}
//					else {
//						Console.Write (" not " + word.Self + ".");
//					}
				}
			}
				
			if (possibleFollowers.Count == 0) {
				//Console.WriteLine ("Word.ChooseByGrammar: There is no " + nextPart + " that can follow " + this.Self + ".");
				return null;
			}

			return ChooseFollower (possibleFollowers, random);
		}
	}
}

