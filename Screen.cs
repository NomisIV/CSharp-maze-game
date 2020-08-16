using System;
using System.Linq;
using System.Threading;

namespace The_aMazement
{
	/* ======== NEEDS FIXING ======== */
	struct ConsoleCell
	{
		public ConsoleColor Foreground, Background;
		public char Char;

		public ConsoleCell(char @char)
		{
			Char = @char;
			Foreground = (ConsoleColor)(-1);
			Background = (ConsoleColor)(-1);
		}

		public ConsoleCell(char @char, ConsoleColor foreground)
		{
			Char = @char;
			Foreground = foreground;
			Background = (ConsoleColor)(-1);
		}

		public ConsoleCell(char @char, ConsoleColor foreground, ConsoleColor background)
		{
			Char = @char;
			Foreground = foreground;
			Background = background;
		}

		public ConsoleCell(ConsoleColor background)
		{
			Char = ' ';
			Foreground = (ConsoleColor)(-1);
			Background = background;
		}

		public bool Equals(ConsoleCell c)
		{
			return
			(
				Foreground == c.Foreground &&
				Background == c.Background &&
				Char == c.Char
			);
		}
	}

	class Screen
	{
		public ConsoleCell[,] cache;
		public readonly int width, height;
		public Screen()
		{
			width = Console.BufferWidth / 2 + 1;
			height = Console.BufferHeight + 1;

			cache = new ConsoleCell[width, height];
			cache.Initialize();
		}
		public void Print(ConsoleCell[,] frame)
		{
			Console.CursorVisible = false;
			for (int y = 0; y < frame.GetLength(1) - 1; y++)
			{
				for (int x = 0; x < frame.GetLength(0) - 1; x++)
				{
					ConsoleCell cell = frame[x, y];
					if (!cell.Equals(cache[x, y]))
					{
						Console.SetCursorPosition(x * 2, y);
						Console.ForegroundColor = cell.Foreground;
						Console.BackgroundColor = cell.Background;
						Console.Write(cell.Char.ToString() + cell.Char.ToString());
						Console.ResetColor();
					}
				}
			}
			cache = frame;
		}

		public void Alert(string message)
		{
			// Calculate position and size of alert
			string[] msg = message.Split("\n");
			int msg_height = msg.GetLength(0);
			int msg_width = msg.OrderByDescending(s => s.Length).First().Length;
			int margin_left = width - msg_width / 2; // Width is already halved to make square pixels
			int margin_top = (height - msg_height) / 2;

			// Draw alert
			Console.SetCursorPosition(margin_left - 3, margin_top - 1);
			Console.WriteLine(" ┌" + new string('─', msg_width + 2) + "┐ ");
			for (int y = 0; y < msg_height; y++)
			{
				Console.SetCursorPosition(margin_left - 3, margin_top + y);
				Console.WriteLine(" │ " + msg[y].PadRight(msg_width) + " │ ");
			}
			Console.SetCursorPosition(margin_left - 3, margin_top + msg_height);
			Console.WriteLine(" └" + new string('─', msg_width + 2) + "┘ ");

			// Await input
			Thread.Sleep(500);

			// Add the box to the screen cache (to make Print(frame) redraw that area next frame)
			for (int y = margin_top - 1; y < margin_top + msg_height + 2; y++)
				for (int x = margin_left / 2 - 2; x < (margin_left + msg_width) / 2 + 2; x++)
					cache[x, y] = new ConsoleCell(ConsoleColor.Red); // Color Null
			Print(cache);
		}
	}
}
