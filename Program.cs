using System;
using System.Collections.Generic;
using System.Collections.Generic;

/*
TODO:
* Winning animation

* Multi level?
	* Exit one maze, enter another
*/

namespace The_aMazement
{
	struct Coordinate
	{
		public int x, y;
		public Coordinate(int _x, int _y)
		{
			x = _x;
			y = _y;
		}

		public bool Equals(Coordinate pos)
		{
			return (x == pos.x && y == pos.y);
		}
	}

	class Program
	{
		static void Main()
		{
			Random r = new Random();

			/* ======== SETUP ======== */
			/* ======== SET UP SCREEN ======== */
			Console.Clear();
			Screen screen = new Screen();
			string alert = "";

			/* ======== SET UP MAZE ======== */
			Maze maze = new Maze(
				(screen.width - 2) / 2 * 2 + 1,
				(screen.height - 2) / 2 * 2 + 1
			);
			maze.Generate();

			/* ======== SET UP PLAYER ======== */
			Player player = new Player(maze);
			player.Pos = maze.GetRandomCell();

			maze.exit = maze.GetRandomCell();

			/* ======== SET UP ITEMS ======== */
			List<Item> Items = new List<Item>();

			// Battery packs
			for (int i = 0; i < r.Next(maze.width * maze.height / 150, maze.width * maze.height / 100); i++)
				Items.Add(new BatteryPack(maze.GetRandomCell()));

			// Map pieces
			// Generate map pieces
			List<Coordinate[]> map_pieces = new List<Coordinate[]>();
			for (int y = 0; y < 3; y++)
				for (int x = 0; x < 3; x++)
					map_pieces.Add(new Coordinate[] {
						new Coordinate((int)(maze.width / 3.0 * x), (int)(maze.height / 3.0 * y)),
						new Coordinate((int)(maze.width / 3.0 * (x + 1)), (int)(maze.height / 3.0 * (y + 1)))
					});
			if (r.NextDouble() > 1 / 2)
				map_pieces.Add(new Coordinate[] {
					new Coordinate(0, 0),
					new Coordinate(maze.width, maze.height)
				});
			// Create map pieces
			for (int i = 0; i < r.Next(2, 6); i++)
			{
				Coordinate[] map_piece = map_pieces[r.Next(map_pieces.Count)];
				map_pieces.Remove(map_piece);
				Items.Add(new MapPiece(maze.GetRandomCell(), map_piece[0], map_piece[1]));
			}

			// Portals
			for (int i = 0; i < r.Next(3); i++)
				Items.Add(new Portal(maze.GetRandomCell(), maze.GetRandomCell()));

			// Oblivion
			for (int i = 0; i < r.Next(2); i++)
				Items.Add(new Oblivion(maze.GetRandomCell()));

			// Flashbang
			for (int i = 0; i < r.Next(5); i++)
				Items.Add(new Flashbang(maze.GetRandomCell()));

			// SleepyPotions
			for (int i = 0; i < r.Next(3); i++)
				Items.Add(new SleepyPotion(maze.GetRandomCell()));

			/* ======== SET UP EFFECTS ======== */
			List<Effect> effects = new List<Effect>();

			// Add starting batteries
			effects.Add(new Batteries(player, maze));

			/* ======== GAME LOOP ======== */
			bool playing = true;
			while (playing)
			{
				/* ======== DRAW ======== */
				ConsoleCell[,] frame = new ConsoleCell[screen.width, screen.height];
				frame.Initialize();

				frame = maze.Print(frame);
				foreach (Item item in Items)
					frame = item.Print(frame);
				frame = player.Observe(frame, maze);
				frame = player.Print(frame);
				screen.Print(frame);

				if (alert != "")
				{
					screen.Alert(alert);
					alert = "";
				}

				/* ======== AWAIT INPUT ======== */
				Coordinate newPos = new Coordinate(0, 0);
				ConsoleKey key = Console.ReadKey(true).Key;
				switch (key)
				{
					case ConsoleKey.UpArrow:
						newPos = new Coordinate(player.Pos.x, player.Pos.y - 1);
						break;

					case ConsoleKey.DownArrow:
						newPos = new Coordinate(player.Pos.x, player.Pos.y + 1);
						break;

					case ConsoleKey.LeftArrow:
						newPos = new Coordinate(player.Pos.x - 1, player.Pos.y);
						break;

					case ConsoleKey.RightArrow:
						newPos = new Coordinate(player.Pos.x + 1, player.Pos.y);
						break;
				}

				/* ======== UPDATE ======== */
				// Move
				if (!maze.map[newPos.x, newPos.y]) player.Pos = newPos;

				if (player.Pos.Equals(maze.exit))
				{
					playing = false;
					Console.Clear();
					screen.Alert("You're aMazeing!");
				}

				// Use items
				foreach (Item item in Items)
					if (player.Pos.Equals(item.Pos))
					{
						item.Use(player, maze, effects);
						alert = item.Message;
						Items.Remove(item);
						break;
					}

				// Reset player effects
				player = player.Reset();

				// Apply effects
				for (int i = effects.Count - 1; i >= 0; i--)
				{
					Effect effect = effects[i];
					effect.Action(player, maze);
					if (effect.Steps > 1) effect.Steps--;
					else
					{
						alert = effect.Message;
						effects.Remove(effect);
					}
				}
			}
		}
	}
}
