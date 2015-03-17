using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;


[RequireComponent(typeof(Collider2D))]
public class Creature : MonoBehaviour
{
	public int size = 1;
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
	private Material[] materials;
	private Color prevColor = Color.white;

	private Transform attackTarget = null;

	private float moveElapsed;
	private Vector2 moveForce;

	void Awake()
	{
		animator = GetComponent<Animator>();
		rigidbody = GetComponent<Rigidbody2D>();

		List<Material> matList = new List<Material>();
		foreach (var renderer in GetComponentsInChildren<Renderer>())
		{
			foreach (var mat in renderer.materials)
				matList.Add(mat);
		}

		materials = new Material[matList.Count];
		for (int i = 0; i < matList.Count; i++)
			materials[i] = matList[i];
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
		UpdateColor();
	}

	void UpdateMovement()
	{
		if (canAttack)
		{
			if (attackTarget != null)
			{
				var diff = attackTarget.transform.position - transform.position;
				var distance = diff.magnitude;

				if (distance < rageRange)
					moveForce = diff.normalized * chaseForce;
				else
					attackTarget = null;
			}
			else
			{
				var collider = Physics2D.OverlapCircle(transform.position, attackRange, 1 << LayerMask.NameToLayer("Player"));
				if (collider != null)
				{
					attackTarget = collider.transform;
					moveElapsed = 0;
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

	void UpdateColor()
	{
		Color curColor = Color.white;

		if (GameManager.instance.puffer.stat.GetSize() < size)
			curColor = warningColor;

		curColor = Color.Lerp(prevColor, curColor, Time.deltaTime);
		for (int i = 0; i < materials.Length; i++)
			materials[i].color = curColor;

		prevColor = curColor;
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
		foreach (var mat in materials)
		{
			if (mat.HasProperty("_Color") == false) continue;

			var seq = DOTween.Sequence();
			for (int i = 0; i < 3; i++)
			{
				seq.Append(mat.DOColor(hurtColor, 0.1f));
				seq.Append(mat.DOColor(Color.white, 0.1f));
			}
		}
		
		transform.DOKill(true);
		transform.DOPunchScale(Vector3.one * 0.2f, 0.4f, 5);
		transform.DOShakeRotation(1, 45);
		
		if (hurtFx)
			Instantiate(hurtFx, transform.position, Quaternion.identity);
	}

	public void Kill()
	{
		var ani = GetComponent<Animator>();
		GetComponent<Collider2D>().isTrigger = true;
		ani.SetBool("Dead", true);
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
