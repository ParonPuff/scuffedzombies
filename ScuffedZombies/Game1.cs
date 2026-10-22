using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ScuffedZombies;

public class Game1 : Game
{
	private GraphicsDeviceManager graphics;

	private SpriteBatch spriteBatch;

	private Rectangle[] optionBars = new Rectangle[2];

	private Texture2D[] barTexture = new Texture2D[2];

	private SpriteFont headLiner;

	private SpriteFont barFont;

	private bool[] hovering = new bool[2];

	private Player player;

	private Texture2D ship;

	private bool rPressed;

	private float radie;

	private Texture2D background;

	private Texture2D healthTexture;

	private Rectangle healthRect;

	private Texture2D bullet;

	private float bulletSpeed;

	private List<Bullet> bulletsPosList = new List<Bullet>();

	private bool shot;

	private int magSize;

	private int shotsTaken;

	private float reloadTime = 0f;

	private float reload;

	private Texture2D zombie;

	private Vector2 zombieOrigin;

	private List<Enemy> zombieList = new List<Enemy>();

	private int enemyAmount;

	private List<float> zombieTempVel = new List<float>();

	private Texture2D vendor;

	private Rectangle vendorArea;

	private Vector2 vendorPos;

	private int itemNr = 1;

	private List<Rectangle> itemRecs = new List<Rectangle>();

	private bool keyPressed = false;

	private bool keyPressed2 = false;

	private bool ePressed = false;

	private bool enterPressed = false;

	private Vector2 textPos;

	private string textString;

	private Matrix ViewPort;

	private SpriteFont gameFont;

	private int wave = 1;

	private bool timerSet = false;

	private float cooldowntime = 0f;

	private List<Coin> coinPosList = new List<Coin>();

	private Texture2D coin;

	private int money;

	private int gameState = 10;

	public Game1()
	{
		graphics = new GraphicsDeviceManager(this);
		base.Content.RootDirectory = "Content";
		graphics.PreferredBackBufferWidth = 1920;
		graphics.PreferredBackBufferHeight = 1080;
		graphics.IsFullScreen = true;
		graphics.ApplyChanges();
	}

	protected override void Initialize()
	{
		Random random = new Random();
		enemyAmount = 4;
		for (int i = 0; i < enemyAmount; i++)
		{
			Vector2 p = random.Next(0, 7) switch
			{
				1 => new Vector2(100f, 0f), 
				2 => new Vector2(200f, base.Window.ClientBounds.Height), 
				3 => new Vector2(300f, 0f), 
				4 => new Vector2(400f, base.Window.ClientBounds.Height), 
				5 => new Vector2(base.Window.ClientBounds.Width, 100f), 
				6 => new Vector2(base.Window.ClientBounds.Width, 400f), 
				_ => new Vector2(500f, 0f), 
			};
			float num = 1f;
			zombieList.Add(new Enemy(p, num, 6, 5));
			zombieTempVel.Add(num);
		}
		for (int j = 0; j < 6; j++)
		{
			int x = 100 * j + 100;
			itemRecs.Add(new Rectangle(x, 250, 75, 75));
		}
		itemRecs[itemNr] = new Rectangle(itemRecs[itemNr].X - 25, itemRecs[itemNr].Y - 25, itemRecs[itemNr].Width + 50, itemRecs[itemNr].Height + 50);
		for (int k = 0; k < itemRecs.Count; k++)
		{
			if (k < itemNr)
			{
				itemRecs[k] = new Rectangle(itemRecs[k].X - 25, itemRecs[k].Y, itemRecs[k].Width, itemRecs[k].Height);
			}
			else if (k > itemNr)
			{
				itemRecs[k] = new Rectangle(itemRecs[k].X + 25, itemRecs[k].Y, itemRecs[k].Width, itemRecs[k].Height);
			}
		}
		optionBars[0] = new Rectangle(base.Window.ClientBounds.Width / 2 - 150, 150, 300, 50);
		optionBars[1] = new Rectangle(base.Window.ClientBounds.Width / 2 - 150, 210, 300, 50);
		textString = "placeHolder";
		textPos = Vector2.Zero;
		shot = false;
		bulletSpeed = 10f;
		reload = 5000f;
		rPressed = false;
		vendorArea = new Rectangle(0, 50, 50, 300);
		vendorPos = new Vector2(-250f, 100f);
		magSize = 3;
		shotsTaken = 0;
		money = 0;
		base.Initialize();
	}

	protected override void LoadContent()
	{
		spriteBatch = new SpriteBatch(base.GraphicsDevice);
		ship = base.Content.Load<Texture2D>("Sprite/ship");
		bullet = base.Content.Load<Texture2D>("Sprite/bullet");
		zombie = base.Content.Load<Texture2D>("Sprite/tripod");
		vendor = base.Content.Load<Texture2D>("Sprite/zombie1");
		healthTexture = base.Content.Load<Texture2D>("Sprite/red");
		coin = base.Content.Load<Texture2D>("Sprite/coin");
		barTexture[0] = base.Content.Load<Texture2D>("oB/grey");
		barTexture[1] = base.Content.Load<Texture2D>("oB/darkGrey");
		headLiner = base.Content.Load<SpriteFont>("Utskrift/HeadLiner");
		barFont = base.Content.Load<SpriteFont>("Utskrift/BarFont");
		gameFont = base.Content.Load<SpriteFont>("Utskrift/GameFont");
		background = base.Content.Load<Texture2D>("Background/background");
		player = new Player(new Vector2(300f, 300f), 2f, 100, ship, 3);
		radie = (float)Math.Pow(Math.Pow(player.PTexture.Width / 2, 2.0) + Math.Pow(player.PTexture.Height / 2, 2.0), 0.5);
	}

	protected override void UnloadContent()
	{
	}

	public bool CheckCollision(Rectangle r1, Rectangle r2)
	{
		return r1.Intersects(r2);
	}

	public Matrix UpdateTMatrix(Vector2 _origin, Vector2 pos, float rotation)
	{
		return Matrix.CreateTranslation(new Vector3(-_origin, 0f)) * Matrix.CreateScale(1f) * Matrix.CreateRotationZ(rotation) * Matrix.CreateTranslation(new Vector3(pos, 0f));
	}

	public Rectangle UpdateRectangle(Texture2D _texture, Vector2 origin, Vector2 pos, float rotation)
	{
		Matrix matrix = UpdateTMatrix(origin, pos, rotation);
		Vector2 vector = Vector2.Transform(Vector2.Zero, matrix);
		Vector2 vector2 = Vector2.Transform(new Vector2(_texture.Width, 0f), matrix);
		Vector2 vector3 = Vector2.Transform(new Vector2(0f, _texture.Height), matrix);
		Vector2 vector4 = Vector2.Transform(new Vector2(_texture.Width, _texture.Height), matrix);
		Vector2 vector5 = new Vector2(MathEx.Min(vector.X, vector2.X, vector3.X, vector4.X), MathEx.Min(vector.Y, vector2.Y, vector3.Y, vector4.Y));
		Vector2 vector6 = new Vector2(MathEx.Max(vector.X, vector2.X, vector3.X, vector4.X), MathEx.Max(vector.Y, vector2.Y, vector3.Y, vector4.Y));
		return new Rectangle((int)vector5.X, (int)vector5.Y, (int)(vector6.X - vector5.X), (int)(vector6.Y - vector5.Y));
	}

	public void MainGameUpdate(GameTime gameTime)
	{
		KeyboardState state = Keyboard.GetState();
		MouseState state2 = Mouse.GetState();
		Vector2 position = new Vector2(state2.X, state2.Y);
		int num = 0;
		bool[] array = new bool[4] { true, true, true, true };
		array[0] = !(player.Pos.Y <= 0f);
		array[1] = !(player.Pos.Y >= (float)background.Height - radie);
		array[2] = !(player.Pos.X <= 0f);
		array[3] = !(player.Pos.X >= (float)background.Width - radie);
		foreach (Enemy zombie in zombieList)
		{
			if (zombie.Velocity == 0f)
			{
				num++;
			}
		}
		if (num > 0)
		{
			foreach (Enemy zombie2 in zombieList)
			{
				if (zombie2.Velocity == 0f)
				{
					Vector2 vector = player.Pos + player.Origin - zombie2.Pos - zombieOrigin;
					array[0] = vector.Y < 0f && array[0];
					array[1] = vector.Y > 0f && array[1];
					array[2] = vector.X < 0f && array[2];
					array[3] = vector.X > 0f && array[3];
				}
			}
		}
		if (state.IsKeyDown(Keys.W) && array[0])
		{
			player.Pos -= new Vector2(0f, player.Velocity);
		}
		else if (state.IsKeyDown(Keys.S) && array[1])
		{
			player.Pos += new Vector2(0f, player.Velocity);
		}
		if (state.IsKeyDown(Keys.A) && array[2])
		{
			player.Pos -= new Vector2(player.Velocity, 0f);
		}
		else if (state.IsKeyDown(Keys.D) && array[3])
		{
			player.Pos += new Vector2(player.Velocity, 0f);
		}
		if (state.IsKeyDown(Keys.E) && !ePressed)
		{
			ePressed = true;
			Rectangle r = UpdateRectangle(player.PTexture, player.Origin, player.Pos, player.Rotation);
			bool flag = CheckCollision(r, vendorArea);
			gameState = (flag ? 1 : 0);
		}
		ePressed = !state.IsKeyUp(Keys.E) && ePressed;
		if (state.IsKeyDown(Keys.R) && !rPressed)
		{
			shotsTaken = magSize;
			shot = true;
			rPressed = true;
		}
		rPressed = !state.IsKeyUp(Keys.R) && rPressed;
		ViewPort = Matrix.CreateTranslation(0f - player.Pos.X - (float)(player.PTexture.Width / 2), 0f - player.Pos.Y - (float)(player.PTexture.Height / 2), 0f) * Matrix.CreateTranslation(base.Window.ClientBounds.Width / 2, base.Window.ClientBounds.Height / 2, 0f);
		Vector2 rotation = Vector2.Transform(position, Matrix.Invert(ViewPort));
		player.SetRotation(rotation);
		base.IsMouseVisible = true;
		if (state2.LeftButton == ButtonState.Pressed && !shot)
		{
			float num2 = (float)Math.PI - (float)Math.Asin((float)(player.PTexture.Height / 2) / radie);
			Vector2 p = new Vector2(player.Pos.X + (float)Math.Cos(0f - player.Rotation - num2) * radie, player.Pos.Y + (float)Math.Sin(0f - player.Rotation - num2) * radie);
			float num3 = rotation.X - p.X;
			float num4 = rotation.Y - p.Y;
			float num5 = (float)Math.Pow(Math.Pow(num3, 2.0) + Math.Pow(num4, 2.0), 0.5);
			Vector2 s = default(Vector2);
			s.X = bulletSpeed * num3 / num5;
			s.Y = bulletSpeed * num4 / num5;
			bulletsPosList.Add(new Bullet(s, p));
			shot = true;
			shotsTaken++;
		}
		if (shotsTaken >= magSize)
		{
			reloadTime += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
			if (reloadTime >= reload)
			{
				shotsTaken = 0;
				reloadTime = 0f;
				shot = false;
			}
		}
		else if (state2.LeftButton != ButtonState.Pressed)
		{
			shot = false;
		}
		for (int i = 0; i < bulletsPosList.Count; i++)
		{
			if (bulletsPosList[i].Position.Y <= 0f)
			{
				bulletsPosList.RemoveAt(i);
			}
			else if (bulletsPosList[i].Position.X <= 0f)
			{
				bulletsPosList.RemoveAt(i);
			}
			else if (bulletsPosList[i].Position.Y >= (float)background.Height)
			{
				bulletsPosList.RemoveAt(i);
			}
			else if (bulletsPosList[i].Position.X >= (float)background.Width)
			{
				bulletsPosList.RemoveAt(i);
			}
			else
			{
				bulletsPosList[i].Position += bulletsPosList[i].Speed;
			}
		}
		zombieOrigin = new Vector2(this.zombie.Width / 2, this.zombie.Height / 2);
		for (int j = 0; j < zombieList.Count; j++)
		{
			zombieList[j].SetRotation(player.Origin + player.Pos, zombieOrigin);
			zombieList[j].SetSpeed(player.Origin + player.Pos, zombieOrigin);
		}
		for (int k = 0; k < zombieList.Count; k++)
		{
			zombieList[k].Pos -= zombieList[k].Speed;
		}
		foreach (Enemy item in zombieList.ToList())
		{
			Rectangle r2 = UpdateRectangle(this.zombie, zombieOrigin, item.Pos, item.Rotation);
			foreach (Bullet item2 in bulletsPosList.ToList())
			{
				Rectangle r3 = new Rectangle((int)item2.Position.X, (int)item2.Position.Y, bullet.Width, bullet.Height);
				if (CheckCollision(r3, r2))
				{
					bulletsPosList.Remove(item2);
					item.Health -= player.Damage;
					bool flag2 = false;
				}
			}
		}
		for (int l = 0; l < zombieList.Count; l++)
		{
			Random random = new Random();
			if (zombieList[l].Health <= 0)
			{
				coinPosList.Add(new Coin(zombieList[l].Pos, random.Next(2, 8)));
				zombieList.Remove(zombieList[l]);
				zombieTempVel.Remove(zombieTempVel[l]);
			}
			else
			{
				zombieList[l].UpdtHealth();
			}
		}
		for (int m = 0; m < zombieList.Count; m++)
		{
			Rectangle r4 = UpdateRectangle(this.zombie, zombieOrigin, zombieList[m].Pos, zombieList[m].Rotation);
			Rectangle r5 = UpdateRectangle(player.PTexture, player.Origin, player.Pos, player.Rotation);
			zombieList[m].AttackTime += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
			bool flag3 = CheckCollision(r5, r4);
			if (flag3)
			{
				zombieList[m].AttackTime += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
				if (zombieList[m].AttackTime >= 1000f)
				{
					player.Health -= zombieList[m].Damage;
					zombieList[m].AttackTime = 0f;
				}
				flag3 = false;
				zombieList[m].Velocity = 0f;
			}
			else if (!flag3)
			{
				zombieList[m].AttackTime = 1000f;
				zombieList[m].Velocity = zombieTempVel[m];
			}
		}
		foreach (Coin item3 in coinPosList.ToList())
		{
			Rectangle r6 = new Rectangle((int)item3.Pos.X, (int)item3.Pos.Y, coin.Width, coin.Height);
			Rectangle r7 = UpdateRectangle(player.PTexture, player.Origin, player.Pos, player.Rotation);
			if (CheckCollision(r6, r7))
			{
				money += item3.Amount;
				coinPosList.Remove(item3);
				bool flag4 = false;
			}
		}
		if (player.Health > 0)
		{
			healthRect = new Rectangle(Convert.ToInt32(player.Pos.X) - 250, Convert.ToInt32(player.Pos.Y) + base.Window.ClientBounds.Height / 2, player.Health * 500 / player.MaxHealth, 20);
		}
		else
		{
			gameState = 2;
		}
		if (zombieList.Count > 0)
		{
			return;
		}
		cooldowntime += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
		timerSet = true;
		if (cooldowntime >= 15000f)
		{
			timerSet = false;
			wave++;
			enemyAmount *= 2;
			Random random2 = new Random();
			for (int n = 0; n < enemyAmount; n++)
			{
				Vector2 p2 = random2.Next(0, 7) switch
				{
					1 => new Vector2(100f, 0f), 
					2 => new Vector2(200f, background.Height - this.zombie.Height), 
					3 => new Vector2(300f, 0f), 
					4 => new Vector2(400f, background.Height - this.zombie.Height), 
					5 => new Vector2(background.Width - this.zombie.Width, 100f), 
					6 => new Vector2(background.Width - this.zombie.Width, 400f), 
					_ => new Vector2(500f, 0f), 
				};
				float num6 = (float)random2.NextDouble() + 1.5f;
				zombieList.Add(new Enemy(p2, num6, 6, 5));
				zombieTempVel.Add(num6);
				cooldowntime = 0f;
			}
		}
	}

	public void Vendor()
	{
		KeyboardState state = Keyboard.GetState();
		MouseState state2 = Mouse.GetState();
		Vector2 vector = new Vector2(state2.X, state2.Y);
		if (state.IsKeyDown(Keys.Left) && !keyPressed)
		{
			keyPressed = true;
			itemNr = ((itemNr > 0) ? itemNr : 0);
			if (itemNr > 0)
			{
				for (int i = 0; i < itemRecs.Count; i++)
				{
					if (i < itemNr)
					{
						itemRecs[i] = new Rectangle(itemRecs[i].X + 25, itemRecs[i].Y, itemRecs[i].Width, itemRecs[i].Height);
					}
					else if (i > itemNr)
					{
						itemRecs[i] = new Rectangle(itemRecs[i].X - 25, itemRecs[i].Y, itemRecs[i].Width, itemRecs[i].Height);
					}
				}
				itemRecs[itemNr] = new Rectangle(itemRecs[itemNr].X + 25, itemRecs[itemNr].Y + 25, itemRecs[itemNr].Width - 50, itemRecs[itemNr].Height - 50);
				itemNr--;
				itemRecs[itemNr] = new Rectangle(itemRecs[itemNr].X - 25, itemRecs[itemNr].Y - 25, itemRecs[itemNr].Width + 50, itemRecs[itemNr].Height + 50);
				for (int j = 0; j < itemRecs.Count; j++)
				{
					if (j < itemNr)
					{
						itemRecs[j] = new Rectangle(itemRecs[j].X - 25, itemRecs[j].Y, itemRecs[j].Width, itemRecs[j].Height);
					}
					else if (j > itemNr)
					{
						itemRecs[j] = new Rectangle(itemRecs[j].X + 25, itemRecs[j].Y, itemRecs[j].Width, itemRecs[j].Height);
					}
				}
			}
		}
		else if (state.IsKeyDown(Keys.Right) && !keyPressed2)
		{
			keyPressed2 = true;
			itemNr = ((itemNr >= itemRecs.Count - 1) ? (itemRecs.Count - 1) : itemNr);
			if (itemNr < itemRecs.Count - 1)
			{
				for (int k = 0; k < itemRecs.Count; k++)
				{
					if (k < itemNr)
					{
						itemRecs[k] = new Rectangle(itemRecs[k].X + 25, itemRecs[k].Y, itemRecs[k].Width, itemRecs[k].Height);
					}
					else if (k > itemNr)
					{
						itemRecs[k] = new Rectangle(itemRecs[k].X - 25, itemRecs[k].Y, itemRecs[k].Width, itemRecs[k].Height);
					}
				}
				itemRecs[itemNr] = new Rectangle(itemRecs[itemNr].X + 25, itemRecs[itemNr].Y + 25, itemRecs[itemNr].Width - 50, itemRecs[itemNr].Height - 50);
				itemNr++;
				itemRecs[itemNr] = new Rectangle(itemRecs[itemNr].X - 25, itemRecs[itemNr].Y - 25, itemRecs[itemNr].Width + 50, itemRecs[itemNr].Height + 50);
				for (int l = 0; l < itemRecs.Count; l++)
				{
					if (l < itemNr)
					{
						itemRecs[l] = new Rectangle(itemRecs[l].X - 25, itemRecs[l].Y, itemRecs[l].Width, itemRecs[l].Height);
					}
					else if (l > itemNr)
					{
						itemRecs[l] = new Rectangle(itemRecs[l].X + 25, itemRecs[l].Y, itemRecs[l].Width, itemRecs[l].Height);
					}
				}
			}
		}
		textPos = new Vector2(itemRecs[itemNr].X, itemRecs[itemNr].Y - 20);
		if (state.IsKeyUp(Keys.Left))
		{
			keyPressed = false;
		}
		if (state.IsKeyUp(Keys.Right))
		{
			keyPressed2 = false;
		}
		if (state.IsKeyDown(Keys.E) && !ePressed)
		{
			ePressed = true;
			gameState = 0;
		}
		else if (state.IsKeyUp(Keys.E))
		{
			ePressed = false;
		}
		if (state.IsKeyDown(Keys.Enter) && !enterPressed)
		{
			enterPressed = true;
			switch (itemNr)
			{
			case 0:
				if (money < 50)
				{
					textString = "Not Enough Money";
					break;
				}
				money -= 50;
				magSize += 5;
				break;
			case 1:
				if (money < 50)
				{
					textString = "Not Enough Money";
					break;
				}
				money -= 50;
				player.Health = ((player.Health + 10 > player.MaxHealth) ? player.MaxHealth : (player.Health + 10));
				break;
			case 2:
				if (money < 50)
				{
					textString = "Not Enough Money";
					break;
				}
				money -= 50;
				player.Damage++;
				break;
			case 3:
				if (money < 50)
				{
					textString = "Not Enough Money";
					break;
				}
				money -= 50;
				player.Velocity += 1f;
				break;
			case 4:
				if (money < 100)
				{
					textString = "Not Enough Money";
					break;
				}
				money -= 100;
				reload *= 0.75f;
				break;
			case 5:
				if (money < 1000)
				{
					textString = "Not Enough Money";
					break;
				}
				money -= 1000;
				player.MaxHealth += 10;
				break;
			}
		}
		if (state.IsKeyUp(Keys.Enter))
		{
			enterPressed = false;
			switch (itemNr)
			{
			case 0:
				textString = "+5 mag size: 50 coins";
				break;
			case 1:
				textString = "+10 heal: 50 coins";
				break;
			case 2:
				textString = "+1 damage: 50 coins";
				break;
			case 3:
				textString = "+1 speed: 50 coins";
				break;
			case 4:
				textString = "-25% reload speed: 100 coins";
				break;
			case 5:
				textString = "+10 Max Health: 1000 coins";
				break;
			}
		}
	}

	public void DeathScreen()
	{
		MouseState state = Mouse.GetState();
		Rectangle r = new Rectangle(state.X, state.Y, 1, 1);
		base.IsMouseVisible = true;
		hovering[0] = CheckCollision(optionBars[0], r);
		if (state.LeftButton == ButtonState.Pressed && hovering[0])
		{
			Exit();
		}
	}

	public void MainMenu()
	{
		MouseState state = Mouse.GetState();
		Rectangle r = new Rectangle(state.X, state.Y, 1, 1);
		base.IsMouseVisible = true;
		hovering[0] = CheckCollision(optionBars[0], r);
		hovering[1] = CheckCollision(optionBars[1], r);
		if (state.LeftButton == ButtonState.Pressed)
		{
			if (hovering[0])
			{
				gameState = 0;
				shot = true;
			}
			else if (hovering[1])
			{
				Exit();
			}
		}
	}

	protected override void Update(GameTime gameTime)
	{
		if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
		{
			gameState = 10;
		}
		switch (gameState)
		{
		case 0:
			MainGameUpdate(gameTime);
			break;
		case 1:
			Vendor();
			break;
		case 2:
			DeathScreen();
			break;
		default:
			MainMenu();
			break;
		}
		base.Update(gameTime);
	}

	protected override void Draw(GameTime gameTime)
	{
		base.GraphicsDevice.Clear(Color.CornflowerBlue);
		switch (gameState)
		{
		case 0:
		{
			spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, ViewPort);
			spriteBatch.Draw(background, Vector2.Zero, Color.White);
			spriteBatch.Draw(player.PTexture, player.Pos, null, Color.White, 0f - player.Rotation, player.Origin, 1f, SpriteEffects.None, 0f);
			spriteBatch.Draw(vendor, vendorPos, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
			for (int i = 0; i < bulletsPosList.Count; i++)
			{
				spriteBatch.Draw(bullet, bulletsPosList[i].Position, Color.White);
			}
			foreach (Coin item in coinPosList.ToList())
			{
				spriteBatch.Draw(coin, item.Pos, Color.White);
			}
			for (int j = 0; j < zombieList.Count; j++)
			{
				spriteBatch.Draw(zombie, zombieList[j].Pos, null, Color.White, 0f - zombieList[j].Rotation + (float)Math.PI, zombieOrigin, 1f, SpriteEffects.None, 0f);
				spriteBatch.Draw(healthTexture, zombieList[j].HealthRec, Color.White);
			}
			spriteBatch.Draw(healthTexture, healthRect, Color.White);
			spriteBatch.DrawString(gameFont, "Wave:" + wave, new Vector2(player.Pos.X - (float)(base.Window.ClientBounds.Width / 2) + 25f, player.Pos.Y - (float)(base.Window.ClientBounds.Height / 2) + 30f), Color.Black);
			if (timerSet)
			{
				spriteBatch.DrawString(gameFont, "Next wave starts in:" + (15 - (int)cooldowntime / 1000), new Vector2(player.Pos.X + (float)(base.Window.ClientBounds.Width / 2) - 150f, player.Pos.Y - (float)(base.Window.ClientBounds.Height / 2) + 50f), Color.Black);
			}
			spriteBatch.DrawString(gameFont, "money: " + money, new Vector2(player.Pos.X + (float)(base.Window.ClientBounds.Width / 2) - 150f, player.Pos.Y - (float)(base.Window.ClientBounds.Height / 2) + 30f), Color.Black);
			spriteBatch.DrawString(gameFont, "ammo: " + (magSize - shotsTaken), new Vector2(player.Pos.X - (float)(base.Window.ClientBounds.Width / 2) + 25f, player.Pos.Y + (float)(base.Window.ClientBounds.Height / 2)), Color.Black);
			spriteBatch.End();
			break;
		}
		case 1:
			spriteBatch.Begin();
			foreach (Rectangle itemRec in itemRecs)
			{
				spriteBatch.Draw(healthTexture, itemRec, Color.White);
			}
			spriteBatch.DrawString(gameFont, "money: " + money, new Vector2(base.Window.ClientBounds.Width - 150, 10f), Color.Black);
			spriteBatch.DrawString(gameFont, textString, textPos, Color.Black);
			spriteBatch.DrawString(headLiner, "Use arrows and enter to select an upgrade", new Vector2(base.Window.ClientBounds.Width / 2 - 300, 100f), Color.Black);
			spriteBatch.End();
			break;
		case 2:
			spriteBatch.Begin();
			if (!hovering[0])
			{
				spriteBatch.Draw(barTexture[0], optionBars[0], Color.White);
			}
			else
			{
				spriteBatch.Draw(barTexture[1], optionBars[0], Color.White);
			}
			spriteBatch.DrawString(headLiner, "You survived to wave " + wave, new Vector2(base.Window.ClientBounds.Width / 2 - 130, 100f), Color.Black);
			spriteBatch.DrawString(barFont, "Exit", new Vector2(base.Window.ClientBounds.Width / 2 - 75, 170f), Color.Black);
			spriteBatch.End();
			break;
		default:
			spriteBatch.Begin();
			if (!hovering[0])
			{
				spriteBatch.Draw(barTexture[0], optionBars[0], Color.White);
			}
			else
			{
				spriteBatch.Draw(barTexture[1], optionBars[0], Color.White);
			}
			if (!hovering[1])
			{
				spriteBatch.Draw(barTexture[0], optionBars[1], Color.White);
			}
			else
			{
				spriteBatch.Draw(barTexture[1], optionBars[1], Color.White);
			}
			spriteBatch.DrawString(barFont, "Start/Resume", new Vector2(base.Window.ClientBounds.Width / 2 - 75, 170f), Color.Black);
			spriteBatch.DrawString(barFont, "Exit", new Vector2(base.Window.ClientBounds.Width / 2 - 75, 230f), Color.Black);
			spriteBatch.DrawString(headLiner, "Scuffed Zombies", new Vector2(base.Window.ClientBounds.Width / 2 - 100, 100f), Color.Black);
			spriteBatch.End();
			break;
		}
		base.Draw(gameTime);
	}
}
