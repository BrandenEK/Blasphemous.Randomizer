using System.Collections.Generic;
using BlasphemousRandomizer.Config;
using BlasphemousRandomizer.Structures;

namespace BlasphemousRandomizer.Fillers
{
    public class EnemyFiller : Filler
    {
        public EnemyFiller()
        {
            // Load enemy location data from json
        }

        public override bool isValid()
        {
            return base.isValid();
        }

        public void Fill(int seed, EnemyConfig config, Dictionary<string, string> output)
        {
            initialize(seed);

			// Get lists
            List<EnemyLocation> locations = new List<EnemyLocation>();
            List<string> enemyIds = new List<string>(EnemyLoader.enemyIds);
			fillLocations(locations);

			// Fill vanilla enemy locations
			fillVanillaLocations(output, locations, enemyIds);

			if (config.type == 1)
            {
				// Randomize enemies only with locations of the matching type
				List<EnemyLocation> typeLocations = new List<EnemyLocation>();
				List<string> typeEnemies = new List<string>();
				while (locations.Count > 0)
                {
					int type = locations[0].enemyType;
					getLocationsOfType(locations, enemyIds, typeLocations, typeEnemies, type);
					fillRandomLocations(output, typeLocations, typeEnemies);
					typeLocations.Clear();
					typeEnemies.Clear();
                }
            }
            else
            {
				// Randomize all remaining enemies with all locations
				fillRandomLocations(output, locations, enemyIds);
            }
        }

		// Transfer locations & enemies of certain type to new lists
		private void getLocationsOfType(List<EnemyLocation> allLocations, List<string> allEnemies, List<EnemyLocation> typeLocations, List<string> typeEnemies, int type)
		{
			for (int i = 0; i < allLocations.Count; i++)
			{
				if (allLocations[i].enemyType == type)
				{
					typeLocations.Add(allLocations[i]);
					typeEnemies.Add(allEnemies[i]);
					allLocations.RemoveAt(i);
					allEnemies.RemoveAt(i);
					i--;
				}
			}
		}

		// Fill output with vanilla locations & enemies
		private void fillVanillaLocations(Dictionary<string, string> output, List<EnemyLocation> locations, List<string> enemies)
		{
			List<EnemyLocation> vanillaLocations = new List<EnemyLocation>();
			List<string> vanillaEnemies = new List<string>();
			getLocationsOfType(locations, enemies, vanillaLocations, vanillaEnemies, -1);
			for (int i = 0; i < vanillaLocations.Count; i++)
			{
				output.Add(vanillaLocations[i].enemy, vanillaEnemies[i]);
			}
		}

		// Fill output by matching random enemies with the location
		private void fillRandomLocations(Dictionary<string, string> output, List<EnemyLocation> locations, List<string> enemies)
		{
			while (locations.Count > 0)
			{
				int index = rand(locations.Count);
				output.Add(locations[index].enemy, enemies[enemies.Count - 1]);
				locations.RemoveAt(index);
				enemies.RemoveAt(enemies.Count - 1);
			}
		}

		// Temporary until data is loaded from json
		private void fillLocations(List<EnemyLocation> locations)
        {
			locations.Clear();
			locations.Add(new EnemyLocation(0, "EN01", 1, false));
			locations.Add(new EnemyLocation(0, "EN02", 0, false));
			locations.Add(new EnemyLocation(0, "EN03", 0, false));
			locations.Add(new EnemyLocation(0, "EN04", 0, false));
			locations.Add(new EnemyLocation(0, "EN05", 1, false));
			locations.Add(new EnemyLocation(0, "EN06", 1, false));
			locations.Add(new EnemyLocation(0, "EN07", 0, false));
			locations.Add(new EnemyLocation(0, "EN08", 1, false));
			locations.Add(new EnemyLocation(0, "EN09", 0, false));
			locations.Add(new EnemyLocation(0, "EN10", -1, false));
			locations.Add(new EnemyLocation(0, "EN11", 0, false));
			locations.Add(new EnemyLocation(0, "EN12", 0, false));
			locations.Add(new EnemyLocation(0, "EN13", 0, false));
			locations.Add(new EnemyLocation(0, "EN14", 0, false));
			locations.Add(new EnemyLocation(0, "EN15", 2, false));
			locations.Add(new EnemyLocation(0, "EN16", 2, false));
			locations.Add(new EnemyLocation(0, "EN17", 0, false));
			locations.Add(new EnemyLocation(0, "EN18", 0, false));
			locations.Add(new EnemyLocation(0, "EN20", 0, false));
			locations.Add(new EnemyLocation(0, "EN21", 0, false));
			locations.Add(new EnemyLocation(0, "EN22", 1, false));
			locations.Add(new EnemyLocation(0, "EN23", 0, false));
			locations.Add(new EnemyLocation(0, "EN24", 0, false));
			locations.Add(new EnemyLocation(0, "EN26", 0, false));
			locations.Add(new EnemyLocation(0, "EN27", -1, false));
			locations.Add(new EnemyLocation(0, "EN28", 0, false));
			locations.Add(new EnemyLocation(0, "EN29", 0, false));
			locations.Add(new EnemyLocation(0, "EN31", 0, false));
			locations.Add(new EnemyLocation(0, "EN32", 0, false));
			locations.Add(new EnemyLocation(0, "EN33", 0, false));
			locations.Add(new EnemyLocation(0, "EV01", 0, false));
			locations.Add(new EnemyLocation(0, "EV02", 0, false));
			locations.Add(new EnemyLocation(0, "EV03", 0, false));
			locations.Add(new EnemyLocation(0, "EV05", -1, false));
			locations.Add(new EnemyLocation(0, "EV08", 1, false));
			locations.Add(new EnemyLocation(0, "EV10", 0, false));
			locations.Add(new EnemyLocation(0, "EV11", 1, false));
			locations.Add(new EnemyLocation(0, "EV12", 1, false));
			locations.Add(new EnemyLocation(0, "EV13", 0, false));
			locations.Add(new EnemyLocation(0, "EV14", 0, false));
			locations.Add(new EnemyLocation(0, "EV15", 0, false));
			locations.Add(new EnemyLocation(0, "EV17", -1, false));
			locations.Add(new EnemyLocation(0, "EV18", -1, false));
			locations.Add(new EnemyLocation(0, "EV19", 2, false));
			locations.Add(new EnemyLocation(0, "EV20", -1, false));
			locations.Add(new EnemyLocation(0, "EV21", 0, false));
			locations.Add(new EnemyLocation(0, "EV22", 0, false));
			locations.Add(new EnemyLocation(0, "EV23", 2, false));
			locations.Add(new EnemyLocation(0, "EV24", 0, false));
			locations.Add(new EnemyLocation(0, "EV26", 2, false));
			locations.Add(new EnemyLocation(0, "EV27", 0, false));
			locations.Add(new EnemyLocation(0, "EV29", -1, false));
			locations.Add(new EnemyLocation(0, "EN201", 0, false));
			locations.Add(new EnemyLocation(0, "EN202", 2, false));
			locations.Add(new EnemyLocation(0, "EN203", -1, false));
		}
    }
}
