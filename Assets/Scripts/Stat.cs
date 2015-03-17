using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class Stat
{
	public enum Rank
	{
		Baby,
		Kid,
		Adult,
		Old
	}
	public Rank rank = 0;
	public bool isAlive = true;
	public int hp;
	public int maxHp = 10;
	public int exp = 0;
	public int maxExp = 100;
	public int attack = 1;
	public int luck = 30;
	public float force = 5;
	public int money = 0;
	public float fat = 0;
	public int maxSize = 10;

	public Stat()
	{
		hp = maxHp;
	}

	public void Hurt(int damage = 1)
	{
		if ((hp -= damage) <= 0)
			isAlive = false;
	}

	public bool IsAlive()
	{
		return isAlive;
	}

	public int GetSize()
	{
		return Mathf.FloorToInt(
			Mathf.Clamp(maxSize * ((float)exp / maxExp), maxSize * .1f, maxSize)
			);
	}

	public bool IsAdult()
	{
		return exp >= maxExp;
	}
}
