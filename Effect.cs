using System;
using System.Threading;

namespace The_aMazement
{
	class Effect
	{
		public Action<Player, Maze> Action;
		public int Steps;
		public string Message;
	}

	class Batteries : Effect
	{
		public Batteries(Player player, Maze maze)
		{
			Action = (player, maze) =>
			{
				//Update discovered areas
				foreach (Coordinate direction in new Coordinate[] {
					new Coordinate(0, 1),
					new Coordinate(-1, 0),
					new Coordinate(0, -1),
					new Coordinate(1, 0)
				})
				{
					for (
						int i = 0,
						x = player.Pos.x + direction.x * i,
						y = player.Pos.y + direction.y * i;

						x > 0 && x < maze.width &&
						y > 0 && y < maze.height &&
						!maze.map[x, y];

						i++,
						x = player.Pos.x + direction.x * i,
						y = player.Pos.y + direction.y * i
					)
						for (int ry = -1; ry <= 1; ry++)
							for (int rx = -1; rx <= 1; rx++)
								if (
									x + rx >= 0 && x + rx < maze.width &&
									y + ry >= 0 && y + ry < maze.height
								) player.Uncovered[x + rx, y + ry] = true;
				}
			};
			Steps = 100;
			Message = "You're out of batteries for your flashlight!";
		}
	}

	class Sleepyness : Effect
	{
		public Sleepyness(Player player, Maze maze)
		{
			Action = (player, maze) => { Thread.Sleep(250); };
			Steps = 50;
			Message = "Finally you wore off the tiredness,\nmovement speed restored";
		}
	}
}
