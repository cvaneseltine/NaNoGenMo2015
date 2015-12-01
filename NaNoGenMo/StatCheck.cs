using System;
using System.Collections.Generic;

namespace NaNoGenMo {
	
	public class StatCheck : SectionEnd {
		//A statcheck is an if/then choice that directs traffic between two different labels based on a particular stat.

		Stat myStat;
		int threshold;

		public Stat MyStat {
			get { return myStat;}
		}

		public int Threshold {
			get { return threshold;}
		}

		public StatCheck (Architect inbound) : base (inbound) {
		}

		public override void BuildOptions (Architect builder) {
			myStat = builder.MyOutline.PickStat ();
			threshold = builder.CurrentStats[myStat] + (builder.MyNovel.Rand.Next (-2, 3));
			myOptions.Add (new Option (builder, 1)); //One option for passing
			myOptions.Add (new Option (builder, 2)); //One option for failing
		}


//
//		public string GetText () {
//			string text = "";
//
//			text = (text + "*if (Stat <= " + threshold + ")\n\tprint some text\n");
//			text = (text + "*if (Stat > " + threshold + ")\n\tprint some other text\n");
//			return text;
//		}
	}

}

