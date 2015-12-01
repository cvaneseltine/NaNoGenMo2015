using System;
using System.Collections.Generic;

namespace NaNoGenMo {
	public class Page {

		string text = "";
		Chapter myChapter;
		int paraRemaining;

		public Page (Chapter inbound) {
			myChapter = inbound;
			paraRemaining = myChapter.PlannedParagraphs;
			while (paraRemaining > 0) {
				if (myChapter.Rand.Next (0, 4) == 0) {
					text = (text + PageBreak ());
				}
				else {
					text = (text + PageBody () + "\n\n");
				}
				paraRemaining--;
			}
		}

		public string PageBreak () {
			Sentence nextSentence;
			Paragraph nextParagraph;
			string text = "";

			nextSentence = new Sentence (myChapter.MyNovel);
			nextParagraph = new Paragraph (myChapter.MyNovel);
			text = ("\n*page_break");
			if (nextSentence.WordCount <= 5) {
				text = (text + " " + nextSentence);
			}
			text = (text + "\n\n" + nextParagraph.ToString());
			return text;
		}

		public string PageBody () {
			Paragraph nextParagraph;

			nextParagraph = new Paragraph (myChapter.MyNovel);
			return nextParagraph.ToString ();
		}

		public override string ToString () {
			return text;
		}
	}
}

