using System;
using System.Collections.Generic;

/*
	[*] Map pieces - unlocks a part of the map - Dark Yellow
	[ ] Magical compass - shows the direction of the maze's exit
	[*] Oblivion - you forget everything you can't see - Dark magenta
	[ ] Oblivion but worse - you forget where you are, and maze regenerates - Magenta
	[*] Teleporters - Integrate into map generation? Or not?? ;))) - Green
	[*] Flashbang - Instantly see a 9 tiles around - Yellow
	[ ] Bomb - destroy walls 3 tiles around
	[ ] Pickaxe - destroy the next wall you try to climb
	[*] Sleepy potion or something - System.sleep(100) after each step - Dark blue;
*/

namespace The_aMazement
{
	class Item
	{
		public Coordinate Pos;
		public ConsoleColor Color;
		public string Message;
		public ConsoleCell[,] Print(ConsoleCell[,] frame)
		{
			frame[Pos.x, Pos.y] = new ConsoleCell(Color);
			return frame;
		}

		public Action<Player, Maze, List<Effect>> Use;
	}

	class BatteryPack : Item
	{
		public BatteryPack(Coordinate pos)
		{
			Color = ConsoleColor.DarkGreen;
			Pos = pos;
			Message = "You found batteries for your flashlight";
			Use = (Player player, Maze maze, List<Effect> effects) =>
			{
				int index = effects.FindIndex(effect => effect.GetType().Equals(typeof(Batteries)));
				if (index >= 0)
					effects[index].Steps += 100;
				else
					effects.Add(new Batteries(player, maze));
			};
		}
	}

	// IDEA: only uncover the region the player is in
	class MapPiece : Item
	{
		public Coordinate MapStart, MapEnd;

		public MapPiece(Coordinate pos, Coordinate mapStart, Coordinate mapEnd)
		{
			Color = ConsoleColor.DarkYellow;
			Pos = pos;
			MapStart = mapStart;
			MapEnd = mapEnd;
			Message = "You found a piece of a map!";
			Use = (Player player, Maze maze, List<Effect> effects) =>
			{
				for (int y = MapStart.y; y < MapEnd.y; y++)
					for (int x = MapStart.x; x < MapEnd.x; x++)
						player.Uncovered[x, y] = true;
			};
		}
	}

	class Oblivion : Item
	{
		public Oblivion(Coordinate pos)
		{
			Color = ConsoleColor.DarkMagenta;
			Pos = pos;
			Message = "You start feeling dizzy.\nYou close your eyes for a bit,\nand when you open them agin\nyou realize you don't remember\nwhere you came from.";
			Use = (Player player, Maze maze, List<Effect> effects) =>
			{
				player.Uncovered = new bool[player.Uncovered.GetLength(0), player.Uncovered.GetLength(1)];
			};
		}
	}

	class Oblivion2 : Item
	{
		public Oblivion2(Coordinate pos)
		{
			Color = ConsoleColor.Magenta;
			Pos = pos;
			Message = "The walls around you start to rumble.\nEverything goes black.\nYou're very confused.";
			Use = (Player player, Maze maze, List<Effect> effects) =>
			{
				maze.Generate();
			};
		}
	}

	class Portal : Item
	{
		public Coordinate Destination;

		public Portal(Coordinate pos, Coordinate destination)
		{
			Color = ConsoleColor.Green;
			Pos = pos;
			Destination = destination;
			Message = "You stumbled upon a portal!";
			Use = (Player player, Maze maze, List<Effect> effects) =>
			{
				player.Pos = Destination;
			};
		}
	}

	class SleepyPotion : Item
	{
		public SleepyPotion(Coordinate pos)
		{
			Color = ConsoleColor.DarkBlue;
			Pos = pos;
			Message = "You suddenly feel tired and you start moving slower";
			Use = (Player player, Maze maze, List<Effect> effects) =>
			{
				effects.Add(new Sleepyness(player, maze));
			};
		}
	}

	class Flashbang : Item
	{
		public Flashbang(Coordinate pos)
		{
			Color = ConsoleColor.Yellow;
			Pos = pos;
			Message = "A sudden flash reveals your surroundings";
			Use = (Player player, Maze maze, List<Effect> effects) =>
			{
				// Flash radius
				int r = 5;
				for (int ry = -r; ry <= r; ry++)
					for (int rx = -r; rx <= r; rx++)
						if (
							player.Pos.x + rx >= 0 && player.Pos.x + rx < maze.width &&
							player.Pos.y + ry >= 0 && player.Pos.y + ry < maze.height
						) player.Uncovered[player.Pos.x + rx, player.Pos.y + ry] = true;
			};
		}
	}

	class MagicalCompass : Item
	{
		public MagicalCompass(Coordinate pos)
		{

		}
	}
}
