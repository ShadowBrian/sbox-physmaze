using Sandbox;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
public static class BezierCurveGenerator
{
	public static List<Vector3> GenerateBezierCurve( Vector3 startPoint, Vector3 endPoint, Vector3 controlPoint1, Vector3 controlPoint2, int segments )
	{
		List<Vector3> points = new List<Vector3>();

		for ( int i = 0; i <= segments; i++ )
		{
			float t = i / (float)segments;
			Vector3 point = CalculateBezierPoint( t, startPoint, endPoint, controlPoint1, controlPoint2 );
			points.Add( point );
		}

		return points;
	}

	private static Vector3 CalculateBezierPoint( float t, Vector3 p0, Vector3 p3, Vector3 p1, Vector3 p2 )
	{
		float u = 1 - t;
		float tt = t * t;
		float uu = u * u;
		float uuu = uu * u;
		float ttt = tt * t;

		Vector3 p = uuu * p0;
		p += 3 * uu * t * p1;
		p += 3 * u * tt * p2;
		p += ttt * p3;

		return p;
	}
}

public sealed class BezierAnimationComponent : BaseComponent, BaseComponent.ExecuteInEditor
{
	[Property] GameObject Point1 { get; set; }
	[Property] GameObject Point2 { get; set; }

	[Property] int segments { get; set; }

	[Property] bool AnimateOnStart { get; set; }

	[Property] float AnimationTime { get; set; } = 2f;

	[Property] GameObject ObjectToAnimate { get; set; }

	[Property] bool LerpToPointAngles { get; set; }

	[Property] bool DelayStart { get; set; } = false;

	private List<Vector3> bezierPoints;

	public override void OnStart()
	{
		if ( AnimateOnStart && GameManager.IsPlaying )
		{
			AnimateObject( ObjectToAnimate, AnimationTime, LerpToPointAngles, DelayStart );
		}
	}

	public override void DrawGizmos()
	{
		if ( Point1 == null || Point2 == null )
			return;

		Vector3 startPoint = Point1.Transform.Position;
		Vector3 endPoint = Point2.Transform.Position;

		bezierPoints = BezierCurveGenerator.GenerateBezierCurve( startPoint, endPoint, startPoint + Point1.Transform.Rotation.Forward * Point1.Transform.Scale.x, endPoint + Point2.Transform.Rotation.Forward * Point2.Transform.Scale.x, segments );

		// Draw the Bezier curve using Gizmos (you may need to implement a Gizmos drawing method).
		// For simplicity, you can use Debug.DrawLines to visualize the curve.
		for ( int i = 1; i < bezierPoints.Count; i++ )
		{
			Gizmo.Draw.Color = Color.Red;
			Gizmo.Draw.Line( bezierPoints[i - 1], bezierPoints[i] );
			Gizmo.Draw.SolidBox( new BBox( bezierPoints[i], 1f ) );
		}
	}

	public async Task AnimateObject( GameObject animatable, float time = 2f, bool LerpToPointAngles = false, bool DelayStart = false )
	{
		Vector3 startPoint = Point1.Transform.Position;
		Vector3 endPoint = Point2.Transform.Position;

		bezierPoints = BezierCurveGenerator.GenerateBezierCurve( startPoint, endPoint,
			startPoint + Point1.Transform.Rotation.Forward * Point1.Transform.Scale.x,
			endPoint + Point2.Transform.Rotation.Forward * Point2.Transform.Scale.x, segments );

		if ( bezierPoints.Count < 2 )
		{
			return;
		}


		float totalTime = time; // Total time for the animation (adjust as needed).
		float currentTime = 0.0f;
		float rotationSpeed = 25.0f; // Adjust the rotation speed as needed.

		float realtime = 0f;

		int currentIndex = 0;


		if ( DelayStart )
		{
			await GameTask.DelayRealtimeSeconds( totalTime );
		}

		while ( currentIndex < bezierPoints.Count - 1 )
		{
			realtime += Time.Delta;
			float t = currentTime / (totalTime / segments);
			Vector3 currentPosition = Vector3.Lerp( bezierPoints[currentIndex], bezierPoints[currentIndex + 1], t );
			animatable.Transform.Position = currentPosition;

			if ( !LerpToPointAngles )
			{
				// Calculate the rotation to look at the next point.
				Vector3 nextPosition = bezierPoints[currentIndex + 1];
				Rotation lookRotation = Rotation.LookAt( nextPosition - currentPosition, Vector3.Up );

				// Smoothly interpolate the rotation.
				Rotation currentRotation = animatable.Transform.Rotation;
				animatable.Transform.Rotation = Rotation.Slerp( currentRotation, lookRotation, rotationSpeed * Time.Delta );

			}
			else
			{
				float t2 = realtime / totalTime;

				animatable.Transform.Rotation = Rotation.Slerp( Point1.Transform.Rotation * Rotation.FromYaw( 180f ), Point2.Transform.Rotation, t2 );
			}
			await GameTask.DelaySeconds( Time.Delta );

			currentTime += Time.Delta;

			if ( currentTime >= totalTime / segments )
			{
				currentTime = 0f;
				currentIndex++;
			}
		}

		// Ensure the final position is exactly at the end of the curve.
		animatable.Transform.Position = bezierPoints[bezierPoints.Count - 1];
	}
}
