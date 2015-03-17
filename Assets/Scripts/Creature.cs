using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;


[RequireComponent(typeof(Collider2D))]
public class Creature : MonoBehaviour
{
	public float size = 1;
	public int life;

	public GameObject[] dropItems;
	public int dropItemCount;

	public GameObject hurtFx;
	public Color hurtColor = Color.white;
	public GameObject destroyFx;
	public Color warningColor = Color.gray;

	public bool facingReverse = false;
	public float randomMoveFreq = .5f;
	public AnimationCurve moveForceCurve;
	public Vector2 randomMoveMinForce;
	public Vector2 randomMoveMaxForce;
	public bool canAttack = true;
	public float attackRange = .5f;
	public float rageRange = 1;
	public float chaseForce = 1;
	public bool freezeX = false, freezeY = false;

	private Animator animator;
	private new Rigidbody2D rigidbody;

	private Transform target = null;

	private float moveElapsed;
	private Vector2 moveForce;

	void Awake()
	{
		animator = GetComponent<Animator>();
		rigidbody = GetComponent<Rigidbody2D>();
	}

	void Update()
	{
		if (Mathf.Abs(rigidbody.velocity.x) >= 0.01f)
		{
			Vector3 theScale = transform.localScale;
			theScale.x = Mathf.Abs(theScale.x) * Mathf.Sign(rigidbody.velocity.x) * (facingReverse ? -1 : 1);
			transform.localScale = theScale;
		}
		animator.SetFloat("Speed", rigidbody.velocity.magnitude);

		UpdateMovement();
	}

	void UpdateMovement()
	{
		if (canAttack)
		{
			if (target != null)
			{
				var diff = target.transform.position - transform.position;
				var distance = diff.magnitude;

				if (distance < rageRange)
					moveForce = diff.normalized * chaseForce;
				else
					target = null;
			}
			else
			{
				var collider = Physics2D.OverlapCircle(transform.position, attackRange, 1 << LayerMask.NameToLayer("Player"));
				if (collider != null)
				{
					target = collider.transform;
					moveElapsed = 0;
					if (target.GetComponent<PlayerController>().stat.CompareSize(size) < 0)
					{
						foreach (var renderer in GetComponentsInChildren<Renderer>())
						{
							var mat = renderer.material;
							if (mat.HasProperty("_Color") == false) continue;

							var seq = DOTween.Sequence();
							for (int i = 0; i < 2; i++)
							{
								seq.Append(mat.DOColor(warningColor, 0.2f));
								seq.Append(mat.DOColor(Color.white, 0.2f));
							}
						}
					}
				}
			}
		}
		if ((moveElapsed += Time.deltaTime) > (1.0f / randomMoveFreq))
		{
			moveElapsed = 0;
			moveForce = new Vector2(
				Random.Range(randomMoveMinForce.x, randomMoveMaxForce.x),
				Random.Range(randomMoveMinForce.y, randomMoveMaxForce.y));
		}
		var force = moveForce * moveForceCurve.Evaluate(moveElapsed / (1.0f / randomMoveFreq));
		if (freezeX)
			force.x = 0;
		if (freezeY)
			force.y = 0;
		rigidbody.AddForce(force);
	}

	void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position, attackRange);
		Gizmos.color = Color.magenta;
		Gizmos.DrawWireSphere(transform.position, rageRange);
	}

	public void Attack()
	{
		animator.SetTrigger("Attack");
	}

	public void Hurt(int damage = 1)
	{
		if ((life -= damage) <= 0)
		{
			life = 0;
			Kill();
		}

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

		float duration = Mathf.Clamp(0.1f / rigidbody.mass, 0, 1);
		transform.DOPunchScale(Vector3.one * 0.2f, duration * 0.4f, 5);
		transform.DOShakeRotation(duration, 45);
		
		if (hurtFx)
			Instantiate(hurtFx, transform.position, Quaternion.identity);
	}

	public void Kill()
	{
		transform.DOKill(true);
		var ani = GetComponent<Animator>();
		ani.SetBool("Dead", true);
		GetComponent<Collider2D>().isTrigger = true;
		Invoke("Disappear", 0.95f);
		Destroy(gameObject, 1);
	}

	public void Disappear()
	{
		if (destroyFx)
			Instantiate(destroyFx, transform.position, Quaternion.identity);
		for (int i = 0; i < dropItemCount; i++)
		{
			var size = GetComponent<Collider2D>().bounds.size.magnitude;
			Vector3 randomForce = Random.insideUnitCircle * size;
			var dropedItem = Instantiate(dropItems[Random.Range(0, dropItems.Length)],
				transform.position, Quaternion.identity) as GameObject;
			dropedItem.GetComponent<Rigidbody2D>().AddForce(randomForce, ForceMode2D.Impulse);
			dropedItem.GetComponent<Rigidbody2D>().AddTorque(Random.Range(-1, 1), ForceMode2D.Impulse);
		}
	}
}
