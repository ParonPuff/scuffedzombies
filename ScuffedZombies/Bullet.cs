using Microsoft.Xna.Framework;

namespace ScuffedZombies;

public class Bullet
{
	private Vector2 speed;

	private Vector2 position;

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

	public Vector2 Position
	{
		get
		{
			return position;
		}
		set
		{
			position = value;
		}
	}

	public Bullet(Vector2 s, Vector2 p)
	{
		speed = s;
		position = p;
	}
}
