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
		UpdateState();

		if (stat.IsAlive() == false)
			return;
		var velocity = rigidbody.velocity;
		transform.rotation = Quaternion.Lerp(transform.rotation,
			Quaternion.AngleAxis(velocity.y * 10 * Mathf.Sign(transform.localScale.x), transform.forward),
			Time.deltaTime * 5);

	}

	public void Move(Vector2 movement)
	{
		Vector3 theScale = transform.localScale;
		theScale.x = Mathf.Abs(theScale.x) * Mathf.Sign(movement.x);
		transform.localScale = theScale;

		rigidbody.AddForce(movement * stat.force * Time.deltaTime);

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

		if (stat.IsCompletion() && nextGenerationPrefab != null)
		{
			var newPuffer = Instantiate(nextGenerationPrefab, transform.position, Quaternion.identity) as GameObject;
			GameManager.instance.SetPlayer(newPuffer.GetComponent<PlayerController>());
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
		transform.DOKill(true);
		creature.Attack();
		stat.Hurt();
		if (stat.GetHPPercent() <= 0.2f && !CompareStateAnimationType(StateAnimationType.Blow))
			SetBodyState(new BlowState());
		if (GetCurrentState().GetStateType() <= StateType.Behavior)
			SetActionState(new HurtState());

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

		if (stat.IsAlive() == false)
		{
			GameManager.instance.joystick.valueChange.RemoveAllListeners();
			SetActionState(new DeathState());
			rigidbody.isKinematic = true;
			foreach (var col in GetComponentsInChildren<Collider2D>())
			{
				col.isTrigger = true;
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

		if (stat.CompareSize(creature.size) >= 0)
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
