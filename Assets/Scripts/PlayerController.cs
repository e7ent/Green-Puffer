using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public partial class PlayerController : MonoBehaviour
{
	public Stat stat;
	public GameObject nextGenerationPrefab;
	public Color hurtColor = Color.red;

	private PufferAnimator animator;
	private new Rigidbody2D rigidbody;

	void Awake()
	{
		animator = GetComponent<PufferAnimator>();
		rigidbody = GetComponent<Rigidbody2D>();
	}

	void Start()
	{
		StartState();
	}

	void Update()
	{
		var velocity = rigidbody.velocity;
		transform.rotation = Quaternion.Lerp(transform.rotation,
			Quaternion.AngleAxis(velocity.y * 10 * Mathf.Sign(transform.localScale.x), transform.forward),
			Time.deltaTime * 5);

		UpdateState();
	}

	public void Move(Vector2 movement)
	{
		Vector3 theScale = transform.localScale;
		theScale.x = Mathf.Abs(theScale.x) * Mathf.Sign(movement.x);
		transform.localScale = theScale;

		rigidbody.AddForce(movement * stat.force);

		if (GetCurrentState() != null)
		{
			if (GetStateType() > StateType.Behavior)
				return;
			else if (CompareStateAnimationType(StateAnimationType.Move))
				return;
		}
		SetActionState(new MoveState());
	}

	public void Feed(int exp, int fat)
	{
		stat.exp += exp;
		stat.fat += fat;

		SetActionState(new EatState());

		transform.DOKill(true);
		transform.DOPunchScale(Vector3.one * 0.2f, 0.4f, 5);

		if (stat.IsAdult() && nextGenerationPrefab != null)
		{
			var newPuffer = Instantiate(nextGenerationPrefab, transform.position, Quaternion.identity) as GameObject;
			GameManager.instance.SetPuffer(newPuffer.GetComponent<PlayerController>());
			Destroy(gameObject);
		}
	}

	public void Attack(Creature creature)
	{
		SetActionState(new AttachState());
		creature.Hurt(stat.attack);
	}

	public void Hurt(Creature creature)
	{
		creature.Attack();
		stat.Hurt();
		if (GetCurrentState().GetStateType() <= StateType.Behavior)
			SetActionState(new HurtState());
		//if (stat.IsAlive() == false)
		//	Destroy(gameObject);
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
	}

	void OnCollisionEnter2D(Collision2D coll)
	{
		var colliedObject = coll.gameObject;
		if (!colliedObject.CompareTag("Creature")) return;

		var creature = colliedObject.GetComponent<Creature>();
		if (creature == null)
			return;

		if (stat.GetSize() >= creature.size)
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

	public void SetFat(int fat)
	{
		this.stat.fat = fat;
	}
}
