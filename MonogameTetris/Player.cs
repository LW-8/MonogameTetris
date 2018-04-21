using Microsoft.Xna.Framework;
using System;

namespace MonogameTetris
{
	public class Player
	{
		private readonly string[] PIECES = new string[7] { "I", "L", "J", "O", "T", "S", "Z" };

		public Vector2 Position;
		public int[][] Matrix;
		private Random rnd;

		public Player()
		{
			rnd = new Random((int)(DateTime.Now.Ticks % int.MaxValue));
		}

		public void RotateMatrix(int dir)
		{
			for (int y = 0; y < Matrix.Length; y++)
			{
				for (int x = 0; x < y; x++)
				{
					int temp1;
					int temp2;
					temp1 = Matrix[x][y];
					temp2 = Matrix[y][x];
					Matrix[y][x] = temp1;
					Matrix[x][y] = temp2;
				}
			}

			if (dir > 0)
			{
				for (int y = 0; y < Matrix.Length; y++)
				{
					Array.Reverse(Matrix[y]);
				}
			}
			else
			{
				Array.Reverse(Matrix);
			}
		}

		public void CreateMatrix()
		{
			Matrix = CreatePiece(PIECES[rnd.Next(7)]);
		}

		private int[][] CreatePiece(string type)
		{
			if (type == "T")
			{
				return new int[][] {
					new int[] { 0, 0, 0 },
					new int[] { 1, 1, 1 },
					new int[] { 0, 1, 0 }
				};
			}
			else if (type == "O")
			{
				return new int[][]
				{
					new int[] { 2, 2 },
					new int[] { 2, 2 }
				};
			}
			else if (type == "L")
			{
				return new int[][] {
					new int[] { 0, 3, 0 },
					new int[] { 0, 3, 0 },
					new int[] { 0, 3, 3 }
				};
			}
			else if (type == "J")
			{
				return new int[][] {
					new int[] { 0, 4, 0 },
					new int[] { 0, 4, 0 },
					new int[] { 4, 4, 0 }
				};
			}
			else if (type == "I")
			{
				return new int[][] {
					new int[] { 0, 5, 0, 0 },
					new int[] { 0, 5, 0, 0 },
					new int[] { 0, 5, 0, 0 },
					new int[] { 0, 5, 0, 0 }
				};
			}
			else if (type == "S")
			{
				return new int[][] {
					new int[] { 0, 6, 6 },
					new int[] { 6, 6, 0 },
					new int[] { 0, 0, 0 }
				};
			}
			else if (type == "Z")
			{
				return new int[][] {
					new int[] { 7, 7, 0 },
					new int[] { 0, 7, 7 },
					new int[] { 0, 0, 0 }
				};
			}

			return null;
		}
	}
}
