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
	public string name;
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
	public float minSize = 1;
	public float maxSize = 10;

	public Stat()
	{
		hp = maxHp;
	}

	public void Hurt(int damage = 1)
	{
		if ((hp -= damage) > 0)
			return;
		hp = 0;
		isAlive = false;
	}

	public bool IsAlive()
	{
		return isAlive;
	}

	public float GetSize()
	{
		var size = (maxSize - minSize) * ((float)exp / maxExp);
		return Mathf.Clamp(minSize + size, minSize, maxSize);
	}

	public bool IsCompletion()
	{
		return exp >= maxExp;
	}

	public int CompareSize(float size)
	{
		return (int)Mathf.Sign(GetSize() - size);
	}

	public float GetHPPercent()
	{
		return (float)hp / (float)maxHp;
	}
}
