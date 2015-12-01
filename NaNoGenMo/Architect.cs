using System;
using System.Collections.Generic;

namespace NaNoGenMo {
	public class Architect {
		//An architect is responsible for building a path through a chapter. It creates one section at a time and tracks current stats for wherever it happens to be at that time.

		Dictionary <Stat, int> currentStats = new Dictionary<Stat, int> ();
		Section current;
		List <Section> sections = new List<Section> ();
		Chapter myChapter;
		//int depth;
		int id;

		public Novel MyNovel {
			get { return myChapter.MyNovel;}
		}

		public Outline MyOutline {
			get { return myChapter.MyOutline;}
		}

		public Chapter MyChapter {
			get { return myChapter;}
		}

		public int PlannedSections {
			get { return myChapter.PlannedSections;}
		}

		public Dictionary <Stat, int> CurrentStats {
			get { return currentStats;}
		}

		public int ID { 
			get { return id;}
		}

		public int Depth {
			get { return sections.Count;}
		}

		public int ChapterNumber {
			get { return myChapter.Number;}
		}

		public string LastLabel {
			get {
				Section lastSection = sections [sections.Count - 1];
				return lastSection.MyLabel;
			}
		}

		public Architect (Chapter inbound) {
			myChapter = inbound;
			myChapter.ArchitectCount++;
			id = myChapter.ArchitectCount;
			//depth = 0;
			foreach (Stat thisStat in myChapter.Stats) {
				currentStats.Add (thisStat, 0);
			}
		}

		public void Build () {

			while (Depth < PlannedSections) {
				if (current == null) {
					current = new Section (this);
					sections.Add (current);
				}
				if (!(current.MyEnd is Ending)) {
					SpawnMany (current);
				}
			}
			if (current == null) {
				Console.WriteLine ("Current is null.");
			}

			myChapter.PassBackSections (sections);
			myChapter.PassBackStats (currentStats);
			Console.Write ("Architect #" + id + " build complete, turning in " + sections.Count + " sections (");
			foreach (Section section in sections) {
				Console.Write (section.MyLabel + " ");
			}
			Console.Write (") Final: " + sections[sections.Count - 1].MyLabel);
			if (current.MyEnd is Choice) {
				Console.WriteLine (" - Choice");
			}
			else if (current.MyEnd is StatCheck) {
				Console.WriteLine (" - StatCheck");
			}
			else if (current.MyEnd is Ending) {
				Console.WriteLine (" - Ending");
			}
			else { 
				Console.WriteLine (" - but it isn't a valid ending kind");
			}
		}

		void SpawnMany (Section thisSection) {
			if (thisSection.MyEnd.MyOptions == null) {
				Console.WriteLine ("Architects.SpawnMany has been called with a null list of options. This is all wrong.");
			}
			if (thisSection.MyEnd is Ending) {
				Console.WriteLine ("SpawnMany called from an Ending.");
			}
			current = null;
			foreach (Option option in thisSection.MyEnd.MyOptions) {
				if (current == null) {
					AdjustStats (option);
					current = option.BuildSection ();
					sections.Add (current);
					Console.WriteLine ("Architect #" + ID + " following " + thisSection.Report () + " " + option.Report () + " to " + current.Report () + ".");
				}
				else {
					Architect nextArchitect;

					nextArchitect = Spawn ();
					nextArchitect.AdjustStats (option);
					nextArchitect.current = option.BuildSection ();
					nextArchitect.sections.Add (current);
					Console.WriteLine ("Created architect #" + nextArchitect.ID + " to follow " + thisSection.Report () + " " + option.Report () + " to " + nextArchitect.current.Report () + ".");
					nextArchitect.Build ();
				}
			}
		}

		Architect Spawn () {
			Architect offspring;

			offspring = (Architect) this.MemberwiseClone ();

			myChapter.ArchitectCount = myChapter.ArchitectCount + 1;
			offspring.id = myChapter.ArchitectCount;

			offspring.currentStats = new Dictionary <Stat, int> (); //Copy the old stats dictionary for the new architect.
			foreach (Stat thisStat in currentStats.Keys) {
				offspring.currentStats.Add (thisStat, currentStats[thisStat]);
			}

			offspring.sections = new List<Section> (); //Copy the old list of sections for the new architect.
			offspring.sections.AddRange (sections);

			Console.WriteLine ("Architect #" + id + " has spawned Architect #" + offspring.id + " at depth " + offspring.Depth + ".");
			return offspring;
		}

		void AdjustStats (Option option) {
			foreach (Stat stat in option.Adjustments.Keys) {
				if (option.IncreaseStat) {
					currentStats[stat] = currentStats[stat] + option.Adjustments[stat];
				}
				else {
					currentStats[stat] = currentStats[stat] - option.Adjustments[stat];
				}
			}
		}
	}
}

