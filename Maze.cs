using System;
using System.Collections.Generic;

namespace The_aMazement
{
	class Maze
	{
		/* ======== PUBLIC ======== */
		public int width, height;
		public bool[,] map;
		public Coordinate exit;

		public List<Coordinate> cells = new List<Coordinate>();

		/* ======== PRIVATE ======== */
		Coordinate[] directionlist = new Coordinate[] {
			new Coordinate(0, 1),
			new Coordinate(-1, 0),
			new Coordinate(0, -1),
			new Coordinate(1, 0)
		};

		/* ======== METHODS ======== */
		public Maze(int _width, int _height)
		{
			width = _width;
			height = _height;
			map = new bool[width + 1, height + 1];

			for (int y = 0; y <= height; y++)
				for (int x = 0; x <= width; x++)
					map[x, y] = true; // Fill with walls
		}

		public void Generate()
		{
			Random r = new Random();

			// Generate maze
			List<Coordinate> stack = new List<Coordinate> { };
			Coordinate firstcell = new Coordinate(
				r.Next((width - 1) / 2) * 2 + 1,
				r.Next((height - 1) / 2) * 2 + 1
			);
			map[firstcell.x, firstcell.y] = false;
			stack.Add(firstcell);

			while (stack.Count > 0)
			{
				// Select the last cell
				Coordinate cell = stack[stack.Count - 1];

				// Check for neighbors
				List<Coordinate> neighbors = new List<Coordinate>();

				foreach (Coordinate direction in directionlist)
				{
					Coordinate neighbor = new Coordinate(cell.x + direction.x * 2, cell.y + direction.y * 2);

					if (
						neighbor.x > 0 && neighbor.x < width &&
						neighbor.y > 0 && neighbor.y < height &&
						map[neighbor.x, neighbor.y] == true
					) neighbors.Add(neighbor);
				}
				if (neighbors.Count == 0) stack.Remove(cell);
				else
				{
					// Chose one of the neighbours and mark it as visited
					Coordinate cell2 = neighbors[r.Next(neighbors.Count)];
					map[cell2.x, cell2.y] = false;
					// Remove the wall between cell and cell2
					map[(cell.x + cell2.x) / 2, (cell.y + cell2.y) / 2] = false;
					stack.Add(cell2);
				}
			}

			// Generate the list of cells
			for (int y = 0; y < height; y++)
				for (int x = 0; x < width; x++)
					if (!map[x, y])
						cells.Add(new Coordinate(x, y));
		}

		public ConsoleCell[,] Print(ConsoleCell[,] frame)
		{
			for (int y = 0; y < height; y++)
				for (int x = 0; x < width; x++)
					frame[x, y] = map[x, y] ? new ConsoleCell(ConsoleColor.White) : new ConsoleCell(ConsoleColor.DarkGray);
			frame[exit.x, exit.y] = new ConsoleCell((ConsoleColor)(-1));
			return frame;
		}

		public Coordinate GetRandomCell()
		{
			Coordinate C = cells[new Random().Next(cells.Count)];
			cells.Remove(C);
			return C;
		}
	}
}
