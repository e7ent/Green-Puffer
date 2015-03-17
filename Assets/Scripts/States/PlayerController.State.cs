using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public partial class PlayerController : MonoBehaviour
{
	private IState bodyState = new NormalState();
	private Stack<IState> feelStates = new Stack<IState>();
	private IState currentState;

	private void StartState()
	{
	}

	private void UpdateState()
	{
		if (currentState == null)
		{
			if (feelStates.Count > 0)
				SetActionState(feelStates.Peek());
			else
				SetActionState(bodyState);
		}

		if (currentState.IsEnd())
		{
			currentState.End();
			currentState = null;
		}
		else
			currentState.Update();
	}

	public void SetBodyState(IState state)
	{
		if (state == null)
		{
			throw new System.NullReferenceException("value is null");
		}
		SetActionState(bodyState = state);
	}

	public void SetFeelState(IState state)
	{
		feelStates.Push(state);
		SetActionState(state);
	}

	public void SetActionState(IState state)
	{
		if (state == null) return;
		if (currentState != null)
			currentState.End();
		currentState = state;
		animator.ChangeState(state.GetAnimationType());
		currentState.Begin(this);
	}

	public void CleanAction()
	{
		currentState.End();
		currentState = null;
	}

	public IState GetCurrentState()
	{
		return currentState;
	}

	public StateType GetStateType()
	{
		if (currentState == null) return StateType.Body;
		return currentState.GetStateType();
	}

	public bool CompareStateType(StateType type)
	{
		if (currentState == null) return false;
		return currentState.GetStateType() == type;
	}

	public bool CompareStateAnimationType(StateAnimationType type)
	{
		if (currentState == null) return false;
		return currentState.GetAnimationType() == type;
	}

	public bool CurrentStateIsEnd()
	{
		if (currentState == null) return true;
		return GetCurrentState().IsEnd();
	}
}