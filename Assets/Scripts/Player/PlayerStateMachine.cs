using UnityEngine;

public class PlayerStateMachine : MonoBehaviour
{
	[HideInInspector]
	public PlayerController controller;
	[HideInInspector]
	public PlayerAnimator animator;
	[HideInInspector]
	public new Rigidbody2D rigidbody;

	private IState currentState;

	private void Awake()
	{
		controller = GetComponent<PlayerController>();
		animator = GetComponent<PlayerAnimator>();
		rigidbody = GetComponent<Rigidbody2D>();
	}

    private void OnDestroy()
    {
        if (currentState != null)
            currentState.End(this);
        currentState = null;
    }

	private void Start()
	{
		Change(new NormalState());
	}

	private void Update()
	{
		currentState.Update(this);
	}

	public void Change(IState state)
	{
		if (currentState != null)
			currentState.End(this);

		currentState = state;
		currentState.Begin(this);
	}
}
