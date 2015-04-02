public interface IState
{
	void Begin(PlayerStateMachine owner);
	void Update(PlayerStateMachine owner);
	void End(PlayerStateMachine owner);
}
