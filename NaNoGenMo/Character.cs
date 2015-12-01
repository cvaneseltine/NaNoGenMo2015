using System;

namespace NaNoGenMo {
	public class Character {
		string name = "";
		int number = -1;
		PronounSet pronoun = PronounSet.Unassigned;

		public string Name {
			get { return name;}
			set { name = value;}
		}

		public int Number {
			get { return number;}
			set { number = value;}
		}

		public PronounSet Pronoun {
			get { return pronoun;}
			set { pronoun = value;}
		}

		public enum PronounSet {
			Unassigned,
			Ey,
			He,
			It,
			Per,
			She,
			They
		}

		public Character (int num) { //Basic characters
			number = num;
		}

		protected Character () { //Basic characters
		}

	}
}

