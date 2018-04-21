using System;

namespace MonogameTetris
{
	public class Arena
	{
		public int[][] Matrix;
		
		public void CreateMatrix(int width, int height)
		{
			int[][] matrix = new int[height][];
			for (int row = 0; row < height; row++)
			{
				matrix[row] = new int[width];
			}
			Matrix = matrix;
		}
	}
}
