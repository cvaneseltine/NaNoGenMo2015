using System;

namespace NaNoGenMo {
	public class Section {
		//A section begins with a label, includes some text, and ends with a sectionEnd (which may be a choice or a statCheck.)

		Label myLabel;
		SectionEnd myEnd;
		Novel myNovel;
		Page myPage;

		public string MyLabel {
			get { return (myLabel.ID).ToString ();}
		}

		public SectionEnd MyEnd {
			get { return myEnd;}
		}

		public Novel MyNovel {
			get { return myNovel;}
		}

		public Section (Architect builder) {
			myNovel = builder.MyNovel;
			myLabel = new Label (this);
			myPage = new Page (builder.MyChapter);
			builder.MyChapter.MySections.Add(this);
			Console.Write ("Section #"+myLabel.ID+" created by Builder #" + builder.ID + " at depth " + builder.Depth + " of " + builder.PlannedSections + ". Built ");
			if (builder.Depth < builder.PlannedSections-1) {
				if (myNovel.Rand.Next (0, builder.ChapterNumber) == 0) {
					myEnd = new Choice (builder);
					Console.WriteLine ("Choice.");
				}
				else {
					myEnd = new StatCheck (builder);
					Console.WriteLine ("StatCheck.");
				}
			}
			else {
				myEnd = new Ending (builder);
				Console.WriteLine ("Ending.");
			}
			myEnd.BuildOptions (builder);
		}

		public string Report () {
			string report = "";

			report = myLabel.Text;
			return report;
		}

		public string ChoiceScript () {
			string report = "";

			Console.WriteLine ("Writing ChoiceScript for section " + myLabel.Text + ".");
			report = ("*label " + myLabel.Text + "\n" + myPage.ToString ());
			if (MyEnd is Choice) {
				report = (report + "\n*choice\n");
				foreach (Option option in MyEnd.MyOptions) {
					report = (report + "\t#" + option.Text + " (" + ExplainOption (option) + ")\n");
					report = (report + "\t\t" + ReportOption (option));
					report = (report + "\t\t*goto " + option.NextSection.myLabel.Text + "\n");
				}
			}
			else if (MyEnd is StatCheck) {
				StatCheck check = MyEnd as StatCheck;

				if (check.MyOptions == null) {
					Console.WriteLine ("Error: next section is null.");
				}
				report = (report + "\n*if (" + check.MyStat.Name + " <= " + Math.Max(check.Threshold,0) + ")\n\t" + ReportOption (check.MyOptions[0]) + "\t*goto " + check.MyOptions[0].NextSection.myLabel.Text + "\n");
				report = (report + "*else\n\t" + ReportOption (check.MyOptions[1]) + "\t*goto " + check.MyOptions[1].NextSection.myLabel.Text + "\n");
			}
			else if (MyEnd is Ending) {
				Ending ending = MyEnd as Ending;

				report = (report + ending.Text + "\n");
			}
			report = (report + "\n");
			return report;
		}

		public string ExplainOption (Option option) {
			string report = "";

			foreach (Stat stat in option.Adjustments.Keys) {

				for (int i = 0; i < option.Adjustments[stat]; i++) {
					if (option.IncreaseStat) {
						report = (report + "+");
					}
					else {
						report = (report + "-");
					}
				}
				report = (report + (Char.ToUpper(stat.Name[0]) + stat.Name.Substring (1)));
			}
			return report;
		}

		public string ReportOption (Option option) {
			string report = "";

			foreach (Stat stat in option.Adjustments.Keys) {
				if (option.IncreaseStat) {
					if (stat.Percentile) {
						report = ("*set " + stat.Name + " %+ " + (option.Adjustments[stat] * 10) +"\n");
					}
					else {
						report = ("*set " + stat.Name + " + " + option.Adjustments[stat] +"\n");
					}
				}
				else {
					if (stat.Percentile) {
						report = ("*set " + stat.Name + " %- " + (option.Adjustments[stat] * 10) +"\n");
					}
					else {
						report = ("*set " + stat.Name + " - " + option.Adjustments[stat] +"\n");
					}
				}
			}
			return report;
		}
	}
}

