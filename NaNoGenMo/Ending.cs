using System;

namespace NaNoGenMo {
	public class Ending : SectionEnd {

		string text = "";

		public string Text {
			get { return text;}
		}

		public Ending (Architect inbound) : base (inbound) {}

		public override void BuildOptions (Architect inbound) {
			//Okay, it isn't actually building options! Or not in the normal sense. But the concept of "what happens next" is still valid.
			Sentence mySentence;

			new Paragraph (inbound.MyNovel).ToString();

			if (inbound.MyChapter.Number < inbound.MyOutline.PlannedChapters) {
				mySentence = new Sentence (inbound.MyNovel);
				text = (text + "\n\n*finish Chapter " + (inbound.MyChapter.Number + 1));
				if (mySentence.WordCount < 5) {
					text = (text + ": " + mySentence.ToString ());
				}
			}
			else {
				text = (text + "*finish THE END");
			}
		}

	}
}

