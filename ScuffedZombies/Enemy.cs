using System;
using Microsoft.Xna.Framework;

namespace ScuffedZombies;

public class Enemy
{
	private Vector2 pos;

	private Vector2 speed;

	private float rotation;

	private float velocity;

	private int health;

	private int damage;

	private Rectangle healthRec;

	private float attackTime;

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

	public Vector2 Speed
	{
		get
		{
			return speed;
		}
		set
		{
			speed = value;
		}
	}

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

	public Rectangle HealthRec => healthRec;

	public float AttackTime
	{
		get
		{
			return attackTime;
		}
		set
		{
			attackTime = value;
		}
	}

	public Enemy(Vector2 p, float v, int h, int d)
	{
		pos = p;
		velocity = v;
		health = h;
		damage = d;
		attackTime = 1000f;
	}

	public void UpdtHealth()
	{
		healthRec = new Rectangle((int)pos.X, (int)pos.Y, health * 5, 5);
	}

	public void SetSpeed(Vector2 target, Vector2 origin)
	{
		Vector2 vector = new Vector2(pos.X + origin.X - target.X, pos.Y + origin.Y - target.Y);
		float num = (float)Math.Pow(Math.Pow(vector.X, 2.0) + Math.Pow(vector.Y, 2.0), 0.5);
		speed = new Vector2(velocity * vector.X / num, velocity * vector.Y / num);
	}

	public void SetRotation(Vector2 target, Vector2 origin)
	{
		Vector2 vector = new Vector2(pos.X + origin.X - target.X, pos.Y + origin.Y - target.Y);
		rotation = (float)Math.Atan2(vector.X, vector.Y);
	}
}
