using UnityEngine;
using System.Collections;

public class TargetFollowerWithTag : MonoBehaviour
{
	public enum UpdateType
	{
		FixedUpdate,
		Update,
		LateUpdate,
	}
	public string targetTag;
	public bool smooth = true;
	public bool enableArea = true;
	public Rect area;
	public Vector3 offset;
	public UpdateType updateType = UpdateType.FixedUpdate;

	private Transform target;
	
	void FollowUpdate()
	{
		if (target == null)
		{
			var findObj = GameObject.FindWithTag(targetTag);
			if (findObj == null) return;
			target = findObj.transform;
		}

		var newPosition = target.position + offset;
		newPosition.z = transform.position.z;
		if (smooth)
			newPosition = Vector3.Lerp(transform.position, newPosition, Time.deltaTime);

		if (enableArea)
		{
			newPosition.x = Mathf.Clamp(newPosition.x, area.min.x, area.max.x);
			newPosition.y = Mathf.Clamp(newPosition.y, area.min.y, area.max.y);
		}
		transform.position = newPosition;
	}

	void Update()
	{
		if (updateType == UpdateType.Update)
			FollowUpdate();
	}

	void FixedUpdate()
	{
		if (updateType == UpdateType.FixedUpdate)
			FollowUpdate();
	}

	void LateUpdate()
	{
		if (updateType == UpdateType.LateUpdate)
			FollowUpdate();
	}

	void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.blue;
		Gizmos.DrawWireCube(area.center, area.size);
	}
}
