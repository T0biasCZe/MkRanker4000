namespace Calculator {
	class Racer {
		public string Name { get; set; }
		public int TotalPoints { get; set; }
		public bool isBot { get; set; }
		public Racer(string name) {
			Name = name;
			TotalPoints = 0;
			isBot = false;
		}
	}
	internal class Program {
		static void Main() {
			int cup = -1;
		e:;
			try {
				Console.WriteLine("Enter the cup number: ");
				cup = int.Parse(Console.ReadLine());
			}
			catch {
				goto e;
			}
			// Path to the CSV file
			string csvFilePath = "e.csv";

			// Read the CSV file
			var lines = File.ReadAllLines(csvFilePath);

			foreach(var line in lines) {
				Console.WriteLine(line);
			}

			// Points system for positions
			int[] points = { 15, 12, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1 };

			// List to store racer points
			List<Racer> racers = new List<Racer>();

			int previousRace = -1;

			foreach(var line in lines)
			{
				var columns = line.Split(';');
				if(columns.Length < 10 || string.IsNullOrWhiteSpace(columns[1])) {
					var oldcolor = Console.ForegroundColor;
					Console.ForegroundColor = ConsoleColor.Red;
					Console.WriteLine("Invalid line: " + line);
					Console.ForegroundColor = oldcolor;
					continue; // Skip invalid or empty lines
				}
				string racerName = columns[2];
				int position = int.Parse(columns[7]);
				bool isBot = bool.Parse(columns[10]);
				int race = int.Parse(columns[6]);
				if(previousRace != race) Console.WriteLine();
				previousRace = race;

				// Assign points based on position (1-based index)
				if(position >= 1 && position <= 12) {
					int earnedPoints = points[position - 1];
					Console.WriteLine($"{racerName} earned {earnedPoints} points at position {position} in race {race}");

					// Find existing racer or create a new one
					var racer = racers.FirstOrDefault(r => r.Name == racerName);
					if(racer == null) {
						racer = new Racer(racerName);
						racers.Add(racer);
						racer.isBot = isBot;
					}

					racer.TotalPoints += earnedPoints;
				}
			}



			// Sort racers by TotalPoints in descending order
			racers = racers.OrderByDescending(r => r.TotalPoints).ToList();

			// Output the results with handling ties
			Console.WriteLine("\nFinal standings:");
			int rank = 1;
			int previousPoints = -1;
			int displayedRank = 1;
			List<string> outcsv = new List<string>();
			for(int i = 0; i < racers.Count; i++) {
				var racer = racers[i];

				if(racer.TotalPoints != previousPoints) {
					displayedRank = rank;
				}
				string isBot = racer.isBot ? "[Bot]" : "";
				Console.WriteLine($"{displayedRank}. {racer.Name} - {racer.TotalPoints} points " + isBot);

				outcsv.Add($";01.01.1970 19:47:27;{racer.Name};69;False;{cup};99;{displayedRank};Wild Woods;BAG;{racer.isBot}\r");

				previousPoints = racer.TotalPoints;
				rank++;
			}
			Console.WriteLine("\nOutput CSV:");
			foreach(var line in outcsv) {
				Console.WriteLine(line);
			}
		}
	}
}
