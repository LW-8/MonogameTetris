using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics;

namespace MonogameTetris
{
	public class TetrisGame : Game
	{
		const int SCALE = 25;
		const double DROP_INTERVAL = 1000;
		readonly Color[] COLORS = new Color[] {
			Color.Black,
			Color.Red,
			Color.LightBlue,
			Color.Violet,
			Color.LightGreen,
			Color.WhiteSmoke,
			Color.Orange,
			Color.Pink
		};
		readonly Color[] COLOR_DATA = new Color[SCALE * SCALE];

		GraphicsDeviceManager graphics;
		SpriteBatch spriteBatch;

		Player player;
		Arena arena;		
		
		double lastTime = 0;
		double dropCounter = 0;		
		KeyboardState _currentKeyboardState;
		KeyboardState _previousKeyboardState;

		public TetrisGame()
		{
			player = new Player();
			arena = new Arena();
			graphics = new GraphicsDeviceManager(this);

			Content.RootDirectory = "Content";
			
			player.Position = new Vector2() { X = 6, Y = 0 };
			player.CreateMatrix();

			arena.CreateMatrix(15, 20);
		}		

		protected override void Initialize()
		{
			graphics.PreferredBackBufferHeight = 500;
			graphics.PreferredBackBufferWidth = 375;
			graphics.ApplyChanges();			

			base.Initialize();
		}

		protected override void LoadContent()
		{
			spriteBatch = new SpriteBatch(GraphicsDevice);			
		}

		protected override void UnloadContent()
		{
			
		}		
		
		protected override void Update(GameTime gameTime)
		{
			_currentKeyboardState = Keyboard.GetState();

			bool isCurrentKeyDownLeft = _currentKeyboardState.IsKeyDown(Keys.Left);
			bool isPreviousKeyUpLeft = _previousKeyboardState.IsKeyUp(Keys.Left);
			bool isCurrentKeyDownRight = _currentKeyboardState.IsKeyDown(Keys.Right);
			bool isPreviousKeyUpRight = _previousKeyboardState.IsKeyUp(Keys.Right);
			bool isCurrentKeyDownDown = _currentKeyboardState.IsKeyDown(Keys.Down);
			bool isPreviousKeyUpDown = _previousKeyboardState.IsKeyUp(Keys.Down);
			bool isCurrentKeyDownQ = _currentKeyboardState.IsKeyDown(Keys.Q);
			bool isPreviousKeyUpQ = _previousKeyboardState.IsKeyUp(Keys.Q);
			bool isCurrentKeyDownW = _currentKeyboardState.IsKeyDown(Keys.W);
			bool isPreviousKeyUpW = _previousKeyboardState.IsKeyUp(Keys.W);

			double time = gameTime.TotalGameTime.TotalMilliseconds;
			double deltaTime =  time - lastTime;
			lastTime = time;

			Debug.WriteLine(deltaTime);

			dropCounter += deltaTime;
			if (dropCounter >= DROP_INTERVAL)
			{
				PlayerDrop();
			}

			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
				Exit();

			if (isCurrentKeyDownLeft && isPreviousKeyUpLeft)
			{
				PlayerMove(-1);
			}
			else if (isCurrentKeyDownRight && isPreviousKeyUpRight)
			{
				PlayerMove(1);
			}
			else if (isCurrentKeyDownDown && isPreviousKeyUpDown)
			{
				PlayerDrop();
			}
			else if (isCurrentKeyDownQ && isPreviousKeyUpQ)
			{
				PlayerRotate(-1);
			}
			else if (isCurrentKeyDownW && isPreviousKeyUpW)
			{
				PlayerRotate(1);
			}

			_previousKeyboardState = _currentKeyboardState;

			base.Update(gameTime);
		}

		public void ArenaSweep()
		{
			for (int y = arena.Matrix.Length - 1; y > 0; y--)
			{
				bool blankSpaceFound = false;

				for (int x = 0; x < arena.Matrix[y].Length; x++)
				{
					if (arena.Matrix[y][x] == 0)
					{
						blankSpaceFound = true;
						break; 
					}
				}

				if (blankSpaceFound) { continue; }

				int[] sweptArray = new int[arena.Matrix[y].Length];
				for (int shiftedY = y; shiftedY > 1; shiftedY--)
				{
					arena.Matrix[shiftedY] = arena.Matrix[shiftedY - 1];
				}
				arena.Matrix[0] = sweptArray;
				y++;
			}
		}

		protected void PlayerReset()
		{
			player.CreateMatrix();
			player.Position.Y = 0;
			player.Position.X = 
				(float)((Math.Floor(arena.Matrix.Length / 2.5)) - Math.Ceiling(player.Matrix.Length / 2.0));

			if (Collide(arena, player))
			{
				for (int y = 0; y < arena.Matrix.Length; y++)
				{
					for (int x = 0; x < arena.Matrix[y].Length; x++)
					{
						arena.Matrix[y][x] = 0;
					}
				}
			}
		}

		public void Merge(int[][] arena, int[][] playerMatrix)
		{
			for (int y = 0; y < playerMatrix.Length; y++)
			{
				for (int x = 0; x < playerMatrix[y].Length; x++)
				{
					if (playerMatrix[y][x] != 0)
					{
						arena[y + (int)player.Position.Y][ x + (int)player.Position.X] = playerMatrix[y][x];
					}
				}
			}
		}

		public bool Collide(Arena arena, Player player)
		{
			int arenaYLength = arena.Matrix.Length;
			int arenaXLength = arena.Matrix[0].Length;

			for (int y = 0; y < player.Matrix.Length; y++)
			{
				for (int x = 0; x < player.Matrix[y].Length; x++)
				{
					int playerCellValue = player.Matrix[y][x];
					int yPosition = y + (int)player.Position.Y;
					int xPosition = x + (int)player.Position.X;

					if (playerCellValue != 0)
					{
						if (arenaYLength <= yPosition)
						{
							return true;
						}

						if (xPosition >= arenaXLength || xPosition < 0)
						{
							return true;
						}

						int arenaCellValue = arena.Matrix[yPosition][xPosition];
						if (arenaCellValue != 0)
						{
							return true;
						}
					}
				}
			}
			return false;
		}

		protected void PlayerDrop()
		{
			player.Position.Y = player.Position.Y + 1;
			if (Collide(arena, player))
			{
				player.Position.Y--;
				Merge(arena.Matrix, player.Matrix);
				PlayerReset();
				ArenaSweep();
			}
			dropCounter = 0;
		}

		protected void PlayerMove(int dir)
		{
			player.Position.X = player.Position.X + dir;
			if (Collide(arena, player))
			{
				player.Position.X = player.Position.X - dir;
			}
		}

		public void PlayerRotate(int dir)
		{
			int playerXPosition = (int)player.Position.X;
			int offset = 1;
			player.RotateMatrix(dir);
			while (Collide(arena, player))
			{
				player.Position.X += offset;
				offset = -(offset + (offset > 0 ? 1 : -1));
				if (offset > player.Matrix[0].Length)
				{
					player.RotateMatrix(-dir);
					player.Position.X = playerXPosition;
					return;					
				}
			}
		}

		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.Black);
						
			spriteBatch.Begin();

			DrawMatrix(arena.Matrix, new Vector2());
			DrawMatrix(player.Matrix, player.Position);
			
			spriteBatch.End();
			
			base.Draw(gameTime);
		}

		protected void DrawMatrix(int[][] matrix, Vector2 offset)
		{
			for (int y = 0; y < matrix.Length; y++)
			{
				for (int x = 0; x < matrix[y].Length; x++)
				{
					if (matrix[y][x] != 0)
					{
						Vector2 coor = new Vector2(x * SCALE + offset.X * SCALE, y * SCALE + offset.Y * SCALE);

						Color c = COLORS[matrix[y][x]];
						for (int i = 0; i < COLOR_DATA.Length; ++i) COLOR_DATA[i] = c;

						Texture2D rect = new Texture2D(graphics.GraphicsDevice, SCALE, SCALE);
						rect.SetData(COLOR_DATA);

						spriteBatch.Draw(rect, coor, c);
					}
				}
			}
		}
	}
}
