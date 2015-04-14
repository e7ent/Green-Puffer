using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using E7;

public class PlayerController : MonoSingleton<PlayerController>
{
    public enum RankType
    {
        Baby,
        Kid,
        Adult,
        Old
    }

    [SerializeField]
    private bool isAlive = true;
    [SerializeField]
    private RankType rank;
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
	[SerializeField]
	private float luck = 0;
	[SerializeField]
	private bool isSick = false;

    public bool IsAlive
    {
        get { return isAlive; }
        set { isAlive = value; }
    }

    public RankType Rank
    {
        get { return rank; }
    }

    public string Id
    {
        get { return id; }
    }

	public string Name
	{
		get { return Localization.GetString(id) + " " + Localization.GetString("puffer"); }
    }

    public float Hp
    {
        get { return hp; }
        set
        {
            hp = Mathf.Clamp(value, 0, MaxHp);
			PlayerPrefs.SetFloat("player_hp", hp);
        }
    }

    public float MaxHp
    {
        get { return maxHp + UpgradeSystem.instance.GetLevel("hp") * 10; }
    }

    public float Exp
    {
        get { return exp; }
        set
        {
            exp = Mathf.Clamp(value, 0, MaxExp);
            if (exp >= MaxExp && Rank < RankType.Old)
			{
				GameManager.instance.Finish(GameManager.FinishType.RankUp);
			}
			PlayerPrefs.SetFloat("player_exp", exp);
        }
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
			PlayerPrefs.SetFloat("player_satiety", satiety);
		}
	}

	public float Luck
	{
		get { return luck + UpgradeSystem.instance.GetLevel("luck"); }
	}

	public bool IsSick
	{
		get { return isSick; }
		set { isSick = value; }
	}

	public float Strength
	{
		get { return strength + UpgradeSystem.instance.GetLevel("strength"); }
	}

	public float MoveForce
	{
		get { return moveForce + UpgradeSystem.instance.GetLevel("moveforce") * 5; }
	}

	public float Size
	{
		get
		{
			var upgradeStat = UpgradeSystem.instance.GetLevel("size");
			return minSize + ((maxSize - minSize) * (this.Exp / this.MaxExp)) + upgradeStat;
		}
	}

    public Action<PlayerController> onEat;
    public Action<PlayerController> onAttack;
    public Action<PlayerController> onHurt;
    public Action<PlayerController> onKill;
    public Action<PlayerController> onDestroy;

	private PlayerAnimator animator;
	private PlayerStateMachine stateMachine;
	private new Rigidbody2D rigidbody;

	protected override void Awake()
	{
		if (instance != null)
		{
			GameObject.Destroy(this);
			return;
		}

		animator = GetComponent<PlayerAnimator>();
		stateMachine = GetComponent<PlayerStateMachine>();
		rigidbody = GetComponent<Rigidbody2D>();

		base.Awake();
	}

	private void Update()
	{
        if (Hp <= 0 && IsAlive)
        {
            stateMachine.Change(new DeathState());
            IsAlive = false;
        }

		UpdateSatiety();
		UpdateMovement();
	}

	private void UpdateSatiety()
    {
        if (IsAlive == false)
            return;

		this.Satiety -= Time.deltaTime * Size * 0.1f;

		if (this.Hp <= (this.MaxHp * 0.1f))
			animator.ChangeBodyAnimation(PlayerAnimator.Type.Blow);
		else if (Mathf.Abs(satiety) <= 0.5f)
			animator.ChangeBodyAnimation(PlayerAnimator.Type.Normal);
		else if (satiety < 0)
			animator.ChangeBodyAnimation(PlayerAnimator.Type.Thin);
		else
			animator.ChangeBodyAnimation(PlayerAnimator.Type.Fat);
	}

	private void UpdateMovement()
	{
        if (IsAlive == false)
            return;

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
        if (!IsAlive) return;

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
        if (!IsAlive) return;

		this.Exp += exp;
		this.Satiety += satiety;

		stateMachine.Change(new EatState());

		transform.DOKill(true);
		transform.DOPunchScale(Vector3.one * 0.2f, 0.4f, 5);
	}

	public void Attack(Creature creature)
    {
        if (!IsAlive) return;

		creature.Hurt(this.Strength);
		stateMachine.Change(new AttackState());
	}

	public void Hurt(Creature creature)
    {
        if (!IsAlive) return;

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
				seq.Append(mat.DOColor(new Color(1.0f, 0.8f, 0.8f), 0.1f));
				seq.Append(mat.DOColor(Color.white, 0.1f));
			}
		}
	}

	public void Hurt(float strength)
	{
		if (!IsAlive) return;

		Hp -= strength;

		stateMachine.Change(new HurtState());

		transform.DOKill(true);
		foreach (var renderer in GetComponentsInChildren<Renderer>())
		{
			var mat = renderer.material;
			if (mat.HasProperty("_Color") == false) continue;

			var seq = DOTween.Sequence();
			for (int i = 0; i < 3; i++)
			{
				seq.Append(mat.DOColor(new Color(1.0f, 0.8f, 0.8f), 0.1f));
				seq.Append(mat.DOColor(Color.white, 0.1f));
			}
		}
	}

	public void Kill()
	{
		if (onDestroy != null)
			onDestroy(this);

		rigidbody.isKinematic = true;
		foreach (var col in GetComponentsInChildren<Collider2D>())
			col.isTrigger = true;

        if (onKill != null)
            onKill(this);

        GameManager.instance.Finish();
	}

	public void Destroy()
	{
		if (onDestroy != null)
			onDestroy(this);

		GameObject.Destroy(gameObject);
		instance = null;
	}

}