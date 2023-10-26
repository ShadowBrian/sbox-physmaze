using Sandbox;

public sealed class HazardSelector : BaseComponent
{
	[Property] GameObject regular { get; set; }
	[Property] GameObject cross { get; set; }
	public override void OnStart()
	{
		if ( MazeCreator.Instance.Rand.Float() > 0.9f )
		{
			regular.Destroy();

			/*foreach ( var item in regular.GetComponents<ColliderBaseComponent>() )
			{
				item.Enabled = false;
				item.OnPhysicsChanged();
				item.Destroy();
			}*/

			cross.Enabled = true;

		}
	}
}
