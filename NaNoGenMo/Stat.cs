using System;
using System.Collections.Generic;

namespace NaNoGenMo {
	public class Stat {

		string name = "";
		string opposed = "";
		bool percentile = false;

		public string Name {
			get { return name;}
		}

		public string Opposed {
			get { return opposed;}
		}

		public bool Percentile {
			get { return percentile;}
		}

		public Stat (List <Word> options, Random rand) { //To build a random stat
			int count, choice;

			count = options.Count;
			choice = rand.Next (0, count - 1);
			name = FixName (options [choice].Name);

			if (rand.Next (0, 2) == 0) { //Percentile stat.
				percentile = true;
				if (rand.Next (0, 2) == 0) { //Opposed stat.
					choice = rand.Next (0, count - 1);
					opposed = FixName (options [choice].Name);
				}
			}
		}

		public Stat (Character inbound) { //To build a character relationship
			name = FixName (inbound.Name);
			percentile = true;
		}

		public Stat (string inboundName, string inboundOpposed) {
			name = FixName (inboundName);
			opposed = FixName (inboundOpposed);
		}

		string FixName (string inbound) {
			string outbound = "";

			foreach (char c in inbound) { //ChoiceScript can't handle apostrophes, etc.
				if (Char.IsLetterOrDigit (c)) {
					outbound = (outbound + c);
				}
				else {
					outbound = (outbound + '_');
				}
			}
			return outbound;
		}
	}
}

