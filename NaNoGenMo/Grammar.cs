using System;
using System.Collections.Generic;

namespace NaNoGenMo {
	public class Grammar {
		
		Vocabulary.SpeechPart part;
		int depth;
		int id;
		static int maxID = 0;

		Dictionary <Vocabulary.SpeechPart, Grammar> followers = new Dictionary<Vocabulary.SpeechPart, Grammar> ();
		Dictionary<string, int> punctuations = new Dictionary<string, int> ();

		public Vocabulary.SpeechPart Part {
			get { return part;}
		}

		public Dictionary <Vocabulary.SpeechPart, Grammar> Followers {
			get { return followers;}
		}

		public int Depth {
			get { return depth;}
		}

		public Grammar (Word word, int count) {
			part = word.Part;
			depth = count;

			maxID++;
			id = maxID;
			//Console.Write (" (grammar node " + id + ") ");
		}

		public Grammar ChooseFollower (Random random) {
			int choice = 0;
			int position = 0;

			//Console.Write ("Depth " + depth + " " + this + " choosing. ");

			if (followers.Count == 0) {
				return null; //End of grammar sequence.
			}

			choice = random.Next (0, followers.Count - 1);
			foreach (Vocabulary.SpeechPart follower in followers.Keys) {
				if (position == choice) {
					return followers[follower];
				}
				position++;
			}

			Console.WriteLine ("Grammar.Follower: Failed to choose a follower successfully. Choice: " + choice + ", Position: " + position + ".");
			return null;
		}

		public void AddPunctation (string punctuation) { //Punctuation can be a null - this is intentional.
			if (punctuations.ContainsKey (punctuation)) {
				punctuations[punctuation]++;
			}
			else {
				punctuations.Add (punctuation, 1);
			}
		}

		public string ChoosePunctuation (Random random) {
			int total = 0;
			int current = 0;
			int choice;

			if (punctuations.Count == 0) { //No punctuation has followed this word in any instance.
				return "";
			}

			foreach (string punctuation in punctuations.Keys) { //Add up all the frequencies.
				total = total + punctuations[punctuation];
			}

			choice = random.Next (1, total + 1); //Choose a number between 0 and the frequencies.

			//Console.WriteLine ("Choosing word to follow " + this.self + ": total is " + total + ", choice is " + choice + ".");

			foreach (string punctuation in punctuations.Keys) {
				current = current + punctuations[punctuation];
				if (current >= choice) {
					return punctuation;
				}
			}
			return "";
		}

		public override string ToString () {
			return Vocabulary.PartToString (part);
		}
	}
}

