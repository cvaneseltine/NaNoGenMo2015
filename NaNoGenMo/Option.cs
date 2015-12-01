using System;
using System.Collections.Generic;

namespace NaNoGenMo {
	public class Option {
		//Choices or StatChecks lead to a series of options, which point in turn to the next section.
		//Options include the data about what stats are adjusted and by what amount when a player reaches this point.

		Architect builder;
		Section nextSection;
		Dictionary <Stat, int> adjustments = new Dictionary<Stat, int> ();
		bool increaseStat = true;
		int position;
		string text;

		public Section NextSection {
			get { return nextSection;}
		}

		public Dictionary <Stat, int> Adjustments {
			get { return adjustments;}
		}

		public bool IncreaseStat {
			get { return increaseStat;}
		}

		public string Text {
			get { return text;}
		}
			
		public Option (Architect inbound, int pos) {
			int adjustment;

			builder = inbound;
			position = pos;
			adjustment = builder.MyOutline.Rand.Next (1, 3);
			adjustments.Add (builder.MyOutline.PickStat (), adjustment);
			if (builder.MyOutline.Rand.Next (0, 4) == 1) {
				increaseStat = false;
			}
			text = new Sentence (builder.MyNovel).ToString();
		}

		public Section BuildSection () {
			nextSection = new Section (builder);
			//Console.WriteLine (nextSection.Report () + " created for architect #" + builder.ID + ".");
			return nextSection;
		}

		public string Report () {
			return ("*" + position);
		}
	}
}

