using Sandbox;

public sealed class HazardSelector : BaseComponent
{
	[Property] GameObject regular { get; set; }
	[Property] GameObject cross { get; set; }
	public override void OnStart()
	{
		if ( MazeCreator.Instance.Rand.Float() > 0.9f )
		{
			//Have to destroy the other collider object?? Disabling them keeps them around invisibly?
			regular.Destroy();

			/*foreach ( var item in regular.GetComponents<ColliderBaseComponent>() )
			{
				item.Enabled = false;
				item.OnPhysicsChanged();
			}*/

			cross.Enabled = true;

		}
	}
}
