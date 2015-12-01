using System;
using System.Collections.Generic;

namespace NaNoGenMo {
	public abstract class SectionEnd {

		protected Architect builder;
		protected List <Option> myOptions = new List<Option> ();

		public List <Option> MyOptions { 
			get { return myOptions;}
			set { myOptions = value;}
		}

		protected Random Rand {
			get { return builder.MyNovel.Rand;}
		}

		protected Outline MyOutline {
			get { return builder.MyOutline;}
		}

		public SectionEnd (Architect inbound) {
			builder = inbound;
		}

		public abstract void BuildOptions (Architect inbound);

	}
}

