using System;
using System.IO;

namespace NaNoGenMo {
	class MainClass {

		public static void Main () {
			string outputPath = @"choicescript-master\web\mygame\scenes";
			//string dictPath = "smalldict.i";
			string dictPath = @"mpos\mobyposi.i";
			Prosebreaker breaker;
			Novel novel;
			Outline outline;

			breaker = new Prosebreaker (dictPath);
			breaker.BreakProse ("sample.txt");

			novel = new Novel (breaker);

			outline = new Outline (novel);

			outline.Build ();

			outline.WriteFiles(outputPath);
		}
	}
}
