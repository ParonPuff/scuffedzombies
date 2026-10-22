using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ScuffedZombies;

public class Player
{
	private Vector2 pos;

	private float rotation;

	private float velocity;

	private int health;

	private int maxHealth;

	private Texture2D playerTexture;

	private Vector2 origin;

	private int damage;

	public Vector2 Pos
	{
		get
		{
			return pos;
		}
		set
		{
			pos = value;
		}
	}

	public Vector2 Origin => origin;

	public Texture2D PTexture => playerTexture;

	public float Rotation => rotation;

	public float Velocity
	{
		get
		{
			return velocity;
		}
		set
		{
			velocity = value;
		}
	}

	public int Health
	{
		get
		{
			return health;
		}
		set
		{
			health = value;
		}
	}

	public int MaxHealth
	{
		get
		{
			return maxHealth;
		}
		set
		{
			maxHealth = value;
		}
	}

	public int Damage
	{
		get
		{
			return damage;
		}
		set
		{
			damage = value;
		}
	}

	public Player(Vector2 p, float v, int h, Texture2D t, int d)
	{
		pos = p;
		velocity = v;
		maxHealth = h;
		playerTexture = t;
		damage = d;
		origin = new Vector2(t.Width / 2, t.Height / 2);
		health = maxHealth;
	}

	public void SetRotation(Vector2 target)
	{
		Vector2 vector = new Vector2(pos.X + origin.X - target.X, pos.Y + origin.Y - target.Y);
		rotation = (float)Math.Atan2(vector.X, vector.Y);
	}
}
