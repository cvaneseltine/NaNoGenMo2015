using System;
using System.Collections.Generic;

namespace NaNoGenMo {
	public class DiscoveredChar : Character {

		Dictionary <PronounSet, int> pronounFrequency = new Dictionary<PronounSet, int> ();

		public DiscoveredChar (string inbound) { //Discovered characters
			Name = inbound;
		}

		public void PronounUsed (PronounSet inbound) { //Document how often various pronoun sets were used immediately after a name in the source
			if (pronounFrequency.ContainsKey (inbound)) {
				pronounFrequency[inbound] ++;
				//Console.WriteLine ("Increasing " + inbound + " on " + Name + "'s pronoun list.");
			}
			else {
				pronounFrequency.Add (inbound, 1);
				//Console.WriteLine ("Adding " + inbound + " to " + Name + "'s pronoun list.");
			}
		}

		public void ResolvePronouns () {
			int freq = 0;

			foreach (PronounSet check in pronounFrequency.Keys) {
				//Console.WriteLine ("Considering " + check + " as " + Name + "'s pronoun (frequency " + pronounFrequency[check] + ").");
				if (pronounFrequency[check] > freq) {
					Pronoun = check;
					freq = pronounFrequency[check];
				}
			}
			//Console.WriteLine (Name + " pronoun resolved to " + Pronoun + ".");
		}
	}
}

