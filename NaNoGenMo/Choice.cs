using System;
using System.Collections.Generic;

namespace NaNoGenMo {
	public class Choice : SectionEnd {

		public Choice (Architect inbound) : base (inbound) {
		}

		public override void BuildOptions (Architect inbound) {
			int number;

			number = Rand.Next (2, 4);
			for (int i = 1; i <= number; i++) {
				myOptions.Add (new Option (builder, i));
			}
		}

//		List <Choice> options = new List <Choice> (); //Where you can go from here
//		List <Choice> priors = new List <Choice> (); //How you could have gotten here
//		int depth;
//		int id;
//		static int maxID = 1;
//		Outline outline;
//		SkillCheck check;
//
//		public List <Choice> Priors {
//			get { return priors;}
//		}
//
//		public Choice (Choice priorChoice) {
//			Choice option;
//
//			id = maxID;
//			maxID++;
//			depth = priorChoice.depth + 1;
//			outline = priorChoice.outline;
//			priors.Add (priorChoice);
//
//			Console.WriteLine ("Created choice #" + id + ".");
//
//			for (int i = 0; i < 3; i++) {
//				if (depth <= outline.Max) {
//					option = PickOption ();
//					options.Add (option);
//					option.priors.Add (this);
//				}
//			}
//		}
//
//		public Choice (Outline thisOutline) {
//			Choice option;
//
//			id = maxID;
//			maxID++;
//			depth = 1;
//			outline = thisOutline;
//
//			Console.WriteLine ("Created the first choice!");
//
//			for (int i = 0; i < 3; i++) {
//				option = PickOption ();
//				options.Add (option);
//			}
//		}
//
//		Choice PickOption () {
//			Choice option = null;
//
//			if ((depth > 2) && (outline.Rand.Next (0, 5) == 0)) { //once more than 2 deep, 1 in 5 chance of looping
//				int distance = outline.Rand.Next(1, depth - 1);
//
//				Console.WriteLine ("Preparing to bounce " + distance + " to find a branch option for #" + id + ".");
//				option = BounceToChoice (distance);
//				if (option == null) {
//					Console.WriteLine ("Bounced to a null. Making a new option instead.");
//				}
//				else {
//					Console.WriteLine ("Discovered #" + option.id + " at distance " + distance + " from #" + id + ".");
//				}
//			}
//			if (option == null) {
//				option = new Choice (this);
//			}
//			return option;
//		}
//
//		Choice BounceToChoice (int distance) {
//			Choice marker;
//
//			Console.Write("Bouncing " + distance + " to a new choice. Start point: #" + id + "... ");
//
//			marker = this.TraceUp (distance, 0);
//			if (marker == null) {
//				return null;
//			}
//			return marker.TraceDown (distance, 0);
//		}
//
//		Choice TraceUp (int distance, int progress) { //Moves upward on a random string of priors
//			Choice marker;
//
//			Console.Write ("Up to #" + id + "; ");
//			if (distance - progress == 0) {
//				return this;
//			}
//			marker = RandomPrior ();
//			if (marker == null) {
//				return null;
//			}
//			else {
//				return marker.TraceUp (distance, progress + 1);
//			}
//		}
//
//		Choice TraceDown (int distance, int progress) { //Moves downward on a random string of options
//			Choice marker;
//
//			Console.Write ("Down to #" + id + "; ");
//			if (distance - progress == 0) {
//				return this;
//			}
//			marker = RandomOption ();
//			if (marker == null) {
//				return null;
//			}
//			else {
//				return marker.TraceDown (distance, progress + 1);
//			}
//		}
//
//		Choice RandomChoice (List <Choice> choices) {
//			int count, selection;
//
//			count = choices.Count;
//			if (count == 0) {
//				return null;
//			}
//			selection = outline.Rand.Next (0, count);
//			return choices [selection];
//		}
//
//		Choice RandomPrior () {
//			return RandomChoice (priors);
//		}
//
//		Choice RandomOption () {
//			return RandomChoice (options);
//		}
//
//		public string ReportChoices () {
//			string myReport;
//
//			myReport = ("\n\nChoice #" + id + " options (depth " + depth + "): ");
//			foreach (Choice option in options) {
//				myReport = myReport + ("\n#" + option.id + "...");
//			}
//			foreach (Choice option in options) {
//				myReport = myReport + option.ReportChoices ();
//			}
//			if (options.Count == 0) {
//				myReport = (myReport + "(ending)");
//			}
//			return myReport;
//		}
//
//		public string GetText () {
//			string text;
//
//			text = ("*label label" + id + "\n\nThis is the text for Label" + id + ". ");
//			if (options.Count > 0) {
//				text = (text + "\n\n*choice\t");
//				foreach (Choice option in options) {
//					text = (text + "\n\t#Jump to Label" + option.id + ".\n\t\t*goto label" + option.id);
//				}
//				text = (text + "\n");
//				foreach (Choice option in options) {
//					text = (text + option.GetText ());
//				}
//			}
//			else {
//				text = (text + "The story optionally ends here (at depth " + depth + ").\n\n");
//			}
//			return text;
//		}
	}
}

