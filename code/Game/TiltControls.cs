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
	float yaw;

	private float lerpSpeed = 2.0f; // Adjust the speed of the interpolation

	public override void OnStart()
	{
		startPosition = Transform.Position;
		targetPosition = startPosition;
		startRotation = Transform.Rotation;
	}


	public bool UseGamepadTilt;

	public override void Update()
	{

		if ( !UseGamepadTilt )
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

			if ( Input.UsingController && Input.MotionData.Rotation != Rotation.Identity )
			{
				UseGamepadTilt = true;
			}
		}

		if ( UseGamepadTilt )
		{
			var gamepadRotation = Input.MotionData.Rotation;

			var rotation = Transform.Rotation;

			roll = gamepadRotation.Roll();

			yaw = gamepadRotation.Yaw();

			pitch = gamepadRotation.Pitch();

			roll = MathX.Clamp( roll, -20f, 20f );

			yaw = MathX.Clamp( yaw, -20f, 20f );

			pitch = MathX.Clamp( pitch, -20f, 20f );

			rotation = new Angles( pitch, yaw, roll ).ToRotation();

			//rotation = Rotation.Slerp( rotation, startRotation, Time.Delta * 10f );

			Transform.Rotation = Rotation.Slerp( Transform.Rotation, rotation, Time.Delta * 10f );
		}


	}
}
