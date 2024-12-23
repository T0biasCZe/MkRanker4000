using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace MkRenderer2 {
	public static class Utils {
		public static int CompareRGB(Color c1, Color c2) {
			Vector3 c1v = new Vector3(c1.R, c1.G, c1.B);
			Vector3 c2v = new Vector3(c2.R, c2.G, c2.B);
			return (int)Vector3.Distance(c1v, c2v);
		}

		public static string ACToString(AnimalCrossingVerze ac) {
			switch(ac) {
				case AnimalCrossingVerze.Jaro:
					return " (Jaro)";
				case AnimalCrossingVerze.Leto:
					return " (Leto)";
				case AnimalCrossingVerze.Podzim:
					return " (Podzim)";
				case AnimalCrossingVerze.Zima:
					return " (Zima)";
				default:
					return "";
			}
		}
	}
}
