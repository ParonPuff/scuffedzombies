using Microsoft.Xna.Framework;

namespace ScuffedZombies;

public class Coin
{
	private Vector2 pos;

	private int amount;

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

	public int Amount
	{
		get
		{
			return amount;
		}
		set
		{
			amount = value;
		}
	}

	public Coin(Vector2 p, int v)
	{
		pos = p;
		amount = v;
	}
}
