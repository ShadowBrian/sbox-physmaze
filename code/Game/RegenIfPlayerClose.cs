using Sandbox;
using System.Linq;

public sealed class RegenIfPlayerClose : BaseComponent
{
	public GameObject Player;

	int framespassed;

	bool GavePhysics;

	public override void Update()
	{
		if ( Player != null )
		{
			if ( framespassed > 60 && !GavePhysics )
			{
				//Player.AddComponent<PhysicsComponent>().Gravity = true;
				GavePhysics = true;
			}
			else if ( !GavePhysics )
			{
				framespassed++;
			}
			if ( Vector3.DistanceBetween( Transform.Position, Player.Transform.Position ) < 50f )
			{
				Scene.FindAllComponents<MazeCreator>().FirstOrDefault().Regenerate();
			}
		}
	}
}
