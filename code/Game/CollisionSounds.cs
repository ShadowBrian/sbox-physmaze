using Sandbox;
using System;

public sealed class CollisionSounds : BaseComponent
{
	[Property] public string HitSound { get; set; }

	[Property] public string RollSound { get; set; }

	Sound Rolling;

	PhysicsComponent phys;

	ColliderBaseComponent col;

	public override void OnStart()
	{
		phys = GetComponent<PhysicsComponent>( false );
		phys.GetBody().OnTouchStart += OnTouchStart;

		phys.GetBody().OnTouchStop += OnTouchStop;

		phys.GetBody().OnTouchStay += OnTouchStay;
	}

	private void OnTouchStop( PhysicsCollisionStop stop )
	{
		if ( (stop.Other.Body.GameObject as GameObject).Name == "floor" )
		{
			OnFloor = false;
		}
	}

	bool OnFloor;

	private void OnTouchStay( PhysicsCollisionStay stay )
	{
		if ( (stay.Other.Body.GameObject as GameObject).Name == "floor" )
		{
			OnFloor = true;
		}
		else
		{
			OnFloor = false;
		}
	}

	private void OnTouchStart( PhysicsCollisionStart start )
	{
		if ( (start.Other.Body.GameObject as GameObject).Name == "wall" )
		{
			if ( start.Contact.Speed.Length > 30f )
			{
				Sound.FromWorld( HitSound, Transform.Position );
			}
		}

		if ( (start.Other.Body.GameObject as GameObject).Name == "floor" )
		{
			OnFloor = true;
		}
	}

	public override void Update()
	{
		if ( !Rolling.IsPlaying )
		{
			Rolling = Sound.FromWorld( RollSound, Transform.Position );
		}

		Rolling.SetPosition( Transform.Position );
		Rolling.SetVolume( OnFloor ? phys.Velocity.Length / 1000f : 0f );
	}

	public override void OnDestroy()
	{
		Rolling.Stop();
	}

	public override void OnDisabled()
	{
		Rolling.Stop();
	}
}
