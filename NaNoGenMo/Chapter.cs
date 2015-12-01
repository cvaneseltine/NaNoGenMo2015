using System;
using System.Collections.Generic;

namespace NaNoGenMo {
	public class Chapter {

		Outline myOutline;

		int number;
		string name;

		List <Section> mySections = new List<Section> ();
		int architectCount = 0; //how many architects have been created

		Dictionary <Stat, int> minStats = new Dictionary<Stat, int> ();
		Dictionary <Stat, int> maxStats = new Dictionary<Stat, int> ();

		public Outline MyOutline {
			get { return myOutline;}
		}

		public Random Rand {
			get { return myOutline.Rand;}
		}
			
		public int Number {
			get { return number;}
		}

		public string Name {
			get { return name;}
		}

		public List <Section> MySections {
			get { return mySections;}
		}
			
		public Novel MyNovel {
			get { return myOutline.MyNovel;}
		}

//		public Tense.Time Time {
//			get { return myOutline.Time;}
//		}

		public int PlannedChapters {
			get { return myOutline.PlannedChapters;}
		} 

		public int PlannedSections {
			get { return myOutline.PlannedSections;}
		}

		public int PlannedParagraphs {
			get { return myOutline.PlannedParagraphs;}
		}

		public List <Stat> Stats {
			get { return myOutline.Stats;}
		}

		public int ArchitectCount {
			get { return architectCount;}
			set { architectCount = value;}
		}

		public Chapter (Outline inbound) {
			myOutline = inbound;
			number = myOutline.ChapterCount + 1;
			name = "chapter" + number;
		}

		public void Build () { //Builds out the chapter.
			Architect architect;

			architect = new Architect (this);
			architect.Build ();
		}
			
		public void PassBackSections (List <Section> sections) {
			//Console.Write ("Handing in sections ");
			foreach (Section section in sections) {
				//Console.Write (section.Report () + "; ");
				if (!mySections.Contains (section)) {
					mySections.Add (section);
				}
			}
			//Console.WriteLine ("");
		}

		public void PassBackStats (Dictionary <Stat, int> passedBack) {
			foreach (Stat stat in passedBack.Keys) {
				if (minStats.ContainsKey (stat)) {
					if (minStats[stat] > passedBack[stat]) {
						minStats[stat] = passedBack[stat];
					}
					if (maxStats [stat] < passedBack[stat]) {
						maxStats[stat] = passedBack[stat];
					}
				}
				else {
					minStats[stat] = passedBack[stat];
					maxStats[stat] = passedBack[stat];
				}
			}
		}

		public void ReceiveStats (Chapter prior) { //Passes the min and max stats from the last chapter to the current chapter.
			foreach (Stat stat in prior.minStats.Keys) {
				minStats.Add (stat, prior.minStats[stat]);
			}
			foreach (Stat stat in prior.maxStats.Keys) {
				maxStats.Add (stat, prior.maxStats[stat]);
			}
		}

		public string Report () {
			string report = "";

			foreach (Section section in mySections) {
				report = (report + section.ChoiceScript());
			}
			foreach (Stat stat in MyNovel.Stats) {
				report = (report + stat.Name + " range: " + minStats[stat] + " to " + maxStats[stat] + "\n");
			}
			return report;
		}

	}
}

