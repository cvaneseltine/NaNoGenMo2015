using System;
using System.Collections.Generic;
using System.IO;

namespace NaNoGenMo {
	
	public class Outline {

		int plannedChapters = 10; //The number of chapters that should be in the outline
		int plannedSections = 3; //The number of sections that a given architect should build
		int plannedParagraphs = 8; //The number of paragraphs that should be in any given section

		Novel myNovel;

//		Tense.Time time;

		List <Stat> myStats = new List <Stat> (); //Includes both the novel's original stats and character relationship stats.

		List <Chapter> chapters = new List<Chapter> ();

		string startup = "";
		string statsPage = "";

		public int PlannedChapters {
			get { return plannedChapters;}
		}

		public int PlannedSections {
			get { return plannedSections;}
		}

		public int PlannedParagraphs {
			get { return plannedParagraphs;}
		}

		public Novel MyNovel {
			get { return myNovel;}
		}

		public Random Rand {
			get { return myNovel.Rand;}
		}

//		public Tense.Time Time {
//			get { return time;}
//		}

		public List <Stat> Stats {
			get { return myStats;}
		}
			
		public int ChapterCount {
			get { return chapters.Count;}
		}

		public Outline (Novel inboundNovel) {
			myNovel = inboundNovel;

			myStats.AddRange (myNovel.Stats); //Add the novel stats
			foreach (Character character in myNovel.Characters) { //Add character relationships as stats
				if ((character.Name != null) && (character.Name != "")) {
					myStats.Add (new Stat (character));

				}
			}
		}

		public void Build () {
			Console.WriteLine ("Commencing outline build.");
			BuildChapters ();
			BuildStartup ();
			BuildStatsPage ();
			Console.WriteLine ("Outline build complete.");
		}

		void BuildChapters () {
			Chapter priorChapter = null;
			Chapter nextChapter = null;

			for (int i = 1; i <= PlannedChapters; i++) {

				Console.WriteLine ("Building chapter " + i + ".");
				nextChapter = new Chapter (this);
				if (priorChapter != null) {
					nextChapter.ReceiveStats (priorChapter);
				}
				nextChapter.Build();
				chapters.Add (nextChapter);
				priorChapter = nextChapter;
			}
		}

		void BuildStartup () {
			startup = (startup + "*title " + new Sentence (MyNovel).ToString() + "\n");
			startup = (startup + "*author Carolyn VanEseltine's 2015 NaNoGenMo project\n");
			startup = (startup + "*scene_list\n");
			foreach (Chapter chapter in chapters) {
				startup = (startup + "\t" + chapter.Name + "\n");
			}
			foreach (Stat stat in Stats) {
				if (stat.Percentile) {
					startup = (startup + "\n*create " + stat.Name + " 50");
				}
				else {
					startup = (startup + "\n*create " + stat.Name + " 0");
				}
			}
		}

		void BuildStatsPage () {
			statsPage = (statsPage + "Your current status....\n*stat_chart\n");
			foreach (Stat stat in Stats) {
				statsPage = (statsPage + "\t");
				if (stat.Percentile) {
					if (stat.Opposed != "") {
						statsPage = (statsPage + "opposed_pair " + stat.Name + "\n\t\t" + stat.Opposed);
					}
					else {
						statsPage = (statsPage + "percent " + stat.Name);
					}
				}
				else {
					statsPage = (statsPage + "text " + stat.Name);
				}
				statsPage = (statsPage + "\n");
			}
		}

		public Stat PickStat () {
			int count, selection;

			count = Stats.Count;
			if (count == 0) { //This should never happen.
				Console.WriteLine ("StatCheck has no stats available.");
				return null;
			}
			selection = Rand.Next (0, count);
			return Stats [selection];
		}

		public void WriteFiles (string path) {
			string destination;

			destination = (path + @"\startup.txt");
			Console.WriteLine ("Writing startup file to " + destination);
			File.WriteAllText (destination, startup, System.Text.Encoding.Default);

			destination = (path + @"\choicescript_stats.txt");
			Console.WriteLine ("Writing stats file to " + destination);
			File.WriteAllText (destination, statsPage, System.Text.Encoding.Default);

			foreach (Chapter chapter in chapters) {
				destination = (path + @"\" + chapter.Name + ".txt");

				Console.WriteLine ("Writing " + chapter.Name + " to " + destination);
				File.WriteAllText (destination, chapter.Report (), System.Text.Encoding.Default);
			}
		}

		public string Report () {
			string report = "";

			foreach (Chapter chapter in chapters) {
				report = (report + "***CHAPTER " + chapter.Number + "***\n\n" + chapter.Report () + "\n\n");
			}
			return report;
		}
	}
}

