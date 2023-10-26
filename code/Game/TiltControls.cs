using Sandbox;
using System;

namespace BrickJam.Game;

public class TiltControls : BaseComponent
{
	private Vector3 startPosition;
	private Vector3 targetPosition;
	private Rotation startRotation;

	float roll;
	float pitch;

	private float lerpSpeed = 2.0f; // Adjust the speed of the interpolation

	public override void OnStart()
	{
		startPosition = Transform.Position;
		targetPosition = startPosition;
		startRotation = Transform.Rotation;
	}


	public override void Update()
	{
		var mouseDelta = Input.MouseDelta;

		var rotation = Transform.Rotation;

		roll = rotation.Roll();

		pitch = rotation.Pitch();

		roll += mouseDelta.x;// * 0.05f; // Adjust the rotation based on the horizontal mouse movement
		pitch -= mouseDelta.y;// * 0.05f; // Adjust the rotation based on the vertical mouse movement

		roll = MathX.Clamp( roll, -20f, 20f );
		pitch = MathX.Clamp( pitch, -20f, 20f );

		rotation = new Angles( pitch, 0, roll ).ToRotation();

		//rotation = Rotation.Slerp( rotation, startRotation, Time.Delta * 10f );

		Transform.Rotation = Rotation.Slerp( Transform.Rotation, rotation, Time.Delta * 10f );


	}
}
