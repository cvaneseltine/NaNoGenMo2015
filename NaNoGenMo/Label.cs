using System;

namespace NaNoGenMo {
	public class Label {
		//A label is a jump-to point in ChoiceScript.

		int id;
		string text;
		static int maxID;
		Section section;

		public string Text {
			get { return text;}
		}

		public int ID {
			get { return id;}
		}

		public Label (Section thisSection) {
			id = maxID;
			maxID++;
			text = ("label_" + id);
			section = thisSection;
		}
	}
}

