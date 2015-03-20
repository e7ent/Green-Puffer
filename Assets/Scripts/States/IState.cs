public enum StateAnimationType
{
	None = -1,
	Normal = 0,
	Thin = 1,
	Fat = 2,
	Rage = 3,
	Eat = 4,
	Attack = 5,
	Fear = 6,
	Stress = 7,
	Sweat = 8,
	Laze = 9,
	Pinch = 10,
	Sleep = 11,
	Hungry = 12,
	SpaceOut = 13,
	Full = 14,
	Blow = 15,
	Move = 16,
	Die = 17,
	Hayyp = 18,
	Fun = 19,
}

public enum StateType
{
	Action = 1000,
	Behavior = 100,
	Feel = 10,
	Body = 0
}

public interface IState
{

	void Begin(PlayerController owner);
	void Update();
	void End();
	bool IsEnd();
	StateAnimationType GetAnimationType();
	string Description();
	StateType GetStateType();
}