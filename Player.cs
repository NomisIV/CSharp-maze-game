using System;
using System.Collections.Generic;

namespace The_aMazement
{
	class Player
	{
		public Coordinate Pos;

		public bool[,] Uncovered;

		Maze maze;

		public Player(Maze _maze)
		{
			this.maze = _maze;
			Uncovered = new bool[_maze.width * 2, _maze.height];
			Uncovered.Initialize();
		}

		public ConsoleCell[,] Observe(ConsoleCell[,] frame, Maze maze)
		{
			// Discover surroundings
			for (int ry = -1; ry <= 1; ry++)
				for (int rx = -1; rx <= 1; rx++)
					if (
						Pos.x + rx >= 0 && Pos.x + rx < maze.width &&
						Pos.y + ry >= 0 && Pos.y + ry < maze.height
					) Uncovered[Pos.x + rx, Pos.y + ry] = true;

			// Only show discovered areas
			for (int y = 0; y < maze.height; y++)
				for (int x = 0; x < maze.width; x++)
					frame[x, y] = Uncovered[x, y] ? frame[x, y] : new ConsoleCell((ConsoleColor)(-1));
			return frame;
		}

		public ConsoleCell[,] Print(ConsoleCell[,] frame)
		{
			frame[Pos.x, Pos.y].Background = ConsoleColor.Red;
			return frame;
		}

		public Player Reset()
		{
			Player p = new Player(maze);
			p.Pos = Pos;
			p.Uncovered = this.Uncovered;
			return p;
		}
	}
}
