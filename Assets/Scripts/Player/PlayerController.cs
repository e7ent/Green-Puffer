using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class PlayerController : MonoBehaviour
{
	public delegate void PlayerEventHandler(PlayerController player);

	public PlayerEventHandler onEat;
	public PlayerEventHandler onAttack;
	public PlayerEventHandler onHurt;
	public PlayerEventHandler onDestroy;

	public GameObject nextGenerationPrefab;
	public Color hurtColor = Color.red;

	[SerializeField]
	private string id;
	[SerializeField]
	private float hp = 0;
	[SerializeField]
	private float maxHp = 10;
	[SerializeField]
	private float exp = 0;
	[SerializeField]
	private float maxExp = 0;
	[SerializeField]
	private float strength = 1;
	[SerializeField]
	private float moveForce = 5;
	[SerializeField]
	private float minSize = 1;
	[SerializeField]
	private float maxSize = 1;
	[SerializeField]
	private float satiety = 0;


	public string Name
	{
		get { return LocalizationString.GetString(id); }
	}

	public float Hp
	{
		get { return hp; }
		set { hp = Mathf.Clamp(value, 0, MaxHp); }
	}

	public float MaxHp
	{
		get { return maxHp + UpgradeSystem.instance.Get("hp").currentLevel * 10; }
	}
	
	public float Exp
	{
		get { return exp; }
		set { exp = value; }
	}

	public float MaxExp
	{
		get { return maxExp; }
	}

	public float Satiety
	{
		get { return satiety; }
		set {
			satiety = Mathf.Clamp(value, -1.0f, 1.0f);
		}
	}

	public float Strength
	{
		get { return strength + UpgradeSystem.instance.Get("strength").currentLevel; }
	}
	
	public float MoveForce
	{
		get { return moveForce + UpgradeSystem.instance.Get("moveforce").currentLevel * 5; }
	}

	public float Size
	{
		get
		{
			var upgradeStat = UpgradeSystem.instance.Get("size").currentLevel;
			return minSize + ((maxSize - minSize) * (this.Exp / this.MaxExp)) + upgradeStat;
		}
	}

	public bool IsAlive()
	{
		return hp > 0;
	}

	private PlayerAnimator animator;
	private PlayerStateMachine stateMachine;
	private new Rigidbody2D rigidbody;

	void Awake()
	{
		animator = GetComponent<PlayerAnimator>();
		stateMachine = GetComponent<PlayerStateMachine>();
		rigidbody = GetComponent<Rigidbody2D>();

		Hp = MaxHp;
	}

	private void Update()
	{
		if (IsAlive() == false)
			return;

		UpdateSatiety();
		UpdateMovement();
	}

	private void UpdateSatiety()
	{
		this.Satiety -= Time.deltaTime * Size * 0.1f;

		if (this.Hp <= (this.MaxHp * 0.1f))
			animator.Change(PlayerAnimator.Type.Blow);
		else if (Mathf.Abs(satiety) <= 0.5f)
			animator.Change(PlayerAnimator.Type.Normal);
		else if (satiety < 0)
			animator.Change(PlayerAnimator.Type.Thin);
		else
			animator.Change(PlayerAnimator.Type.Fat);
	}

	private void UpdateMovement()
	{
		var movement = JoystickSystem.instance.axis * JoystickSystem.instance.strength;
		var localScale = transform.localScale;

		if (Mathf.Abs(movement.x) > float.Epsilon)
		{
			localScale.x = Mathf.Abs(localScale.x) * Mathf.Sign(movement.x);
			transform.localScale = localScale;
		}

		var rotation = Quaternion.AngleAxis(rigidbody.velocity.y * 10 * Mathf.Sign(localScale.x), transform.forward);
		transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * 5);

		rigidbody.AddForce(movement * MoveForce * Time.deltaTime);
	}

	private void OnCollisionEnter2D(Collision2D coll)
	{
		var colliedObject = coll.gameObject;
		if (!colliedObject.CompareTag("Creature")) return;

		var creature = colliedObject.GetComponent<Creature>();
		if (creature == null)
			return;

		if (this.Size >= creature.size)
			Attack(creature);
		else
			Hurt(creature);

		var diff = (creature.gameObject.transform.position - transform.position).normalized;
		var otherRigidbody = creature.gameObject.GetComponent<Rigidbody2D>();
		var totalMass = rigidbody.mass + otherRigidbody.mass;
		var force = Mathf.Clamp(rigidbody.velocity.magnitude + otherRigidbody.velocity.magnitude, 0.1f, 1.0f);
		rigidbody.AddForce(-diff * force * (otherRigidbody.mass / totalMass), ForceMode2D.Impulse);
		otherRigidbody.AddForce(diff * force * (rigidbody.mass / totalMass) * otherRigidbody.mass, ForceMode2D.Impulse);
	}

	public void Eat(float exp, float satiety)
	{
		this.Exp += exp;
		this.Satiety += satiety;

		stateMachine.Change(new EatState());

		transform.DOKill(true);
		transform.DOPunchScale(Vector3.one * 0.2f, 0.4f, 5);

		if ((this.Exp / this.MaxExp) >= 1.0f && nextGenerationPrefab != null)
		{
			RebirthNextGeneration();
		}
	}

	public void Attack(Creature creature)
	{
		creature.Hurt(this.Strength);
		stateMachine.Change(new AttackState());
	}

	public void Hurt(Creature creature)
	{
		Hp -= creature.strength;
		creature.Attack();
		stateMachine.Change(new HurtState());

		transform.DOKill(true);
		foreach (var renderer in GetComponentsInChildren<Renderer>())
		{
			var mat = renderer.material;
			if (mat.HasProperty("_Color") == false) continue;

			var seq = DOTween.Sequence();
			for (int i = 0; i < 3; i++)
			{
				seq.Append(mat.DOColor(hurtColor, 0.1f));
				seq.Append(mat.DOColor(Color.white, 0.1f));
			}
		}

		if (IsAlive() == false)
			Kill();
	}

	public void RebirthNextGeneration()
	{
		Instantiate(nextGenerationPrefab, transform.position, Quaternion.identity);
		Destroy();
	}

	public void Kill()
	{
		if (onDestroy != null)
			onDestroy(this);

		stateMachine.Change(new DeathState());
		rigidbody.isKinematic = true;
		foreach (var col in GetComponentsInChildren<Collider2D>())
			col.isTrigger = true;

		//Destroy();
	}

	public void Destroy()
	{
		if (onDestroy != null)
			onDestroy(this);

		GameObject.Destroy(gameObject);
	}

}