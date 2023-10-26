using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Editor;
using Sandbox;


public partial class MazeCreator : BaseComponent
{
	public int LevelsPassed;

	public static MazeCreator Instance { get; set; }

	public WallState[,] maze;

	public int width = 3;
	public int height = 3;

	[Property] public GameObject Wall { get; set; }
	[Property] public GameObject Floor { get; set; }
	[Property] public GameObject Player { get; set; }

	List<GameObject> GeneratedItems { get; set; } = new List<GameObject> { };

	public int SeedToUse { get; set; } = 0;

	public Random Rand { get; set; }

	public override void OnStart()
	{
		Instance = this;

		SeedToUse = (int)MathF.Floor( float.Parse( (System.DateTime.Now.DayOfYear / 7f) + "" + (System.DateTime.Now.Year) ) );
		Rand = new Random( SeedToUse );

		GenerateMaze();
	}

	public override void Update()
	{
		base.Update();
		if ( CurrentPlayerBall != null )
		{
			if ( Transform.World.ToLocal( CurrentPlayerBall.Transform.World ).Position.z < -100f )
			{
				CurrentPlayerBall.SetParent( GameObject.Children[0] );
				CurrentPlayerBall.Transform.LocalPosition = StartPos;
				CurrentPlayerBall.SetParent( Scene );
			}
		}
	}

	public void Regenerate()
	{
		foreach ( var obj in GeneratedItems )
		{
			obj.Destroy();
		}

		GeneratedItems.Clear();

		Log.Info( "Regenerating Maze..." );

		width += 1;
		height += 1;

		LevelsPassed += 1;

		width = MathX.FloorToInt( MathX.Clamp( width, 0, 11 ) );
		height = MathX.FloorToInt( MathX.Clamp( height, 0, 11 ) );

		Sandbox.Services.Stats.Increment( "levels", 1 );

		if ( LevelsPassed > Sandbox.Services.Stats.GetPlayerStats( "shadb.physmaze", Game.SteamId ).Get( "levelhighscores" ).Value )
		{
			Sandbox.Services.Stats.SetValue( "levelhighscores", LevelsPassed );//Not sure this actually works but it should?
		}

		Transform.Rotation = Rotation.Identity;
		GameObject.Children[0].Transform.LocalPosition = Vector3.Zero;

		GenerateMaze();

		Log.Info( "Maze Regenerated!" );


		Scene.GetAllObjects( true ).Where( X => X.GetComponent<CameraComponent>() != null ).FirstOrDefault().Transform.Position = Vector3.Up * (200f + (200f * width));
	}

	GameObject CurrentPlayerBall;

	public Vector3 StartPos = Vector3.Zero;

	public void GenerateMaze()
	{
		maze = MazeGenerator.Generate( width, height );

		bool PickedStart = false;
		bool PickedEnd = false;

		bool ThisIsEnd = false;
		bool ThisIsStart = false;


		float scale = 100f;

		RegenIfPlayerClose End = null;

		for ( int i = 0; i < width; ++i )
		{
			for ( int j = 0; j < height; ++j )
			{
				var cell = maze[i, j];
				var position = Transform.Position + new Vector3( -width / 2f + i * scale, -height / 2f + j * scale, 0f );

				GameObject floorModel = SceneUtility.Instantiate( Floor );
				floorModel.SetParent( GameObject.Children[0] );
				floorModel.Transform.Position = position;
				floorModel.Transform.Rotation = Rotation.Identity;
				GeneratedItems.Add( floorModel );

				position += Vector3.Up * scale / 2f;

				if ( i == 0 )
				{
					if ( !PickedEnd && j == 0 )
					{
						PickedEnd = true;
						End = floorModel.AddComponent<RegenIfPlayerClose>();
						foreach ( var item in floorModel.GetComponents<ModelComponent>( false, true ) )
						{
							item.Tint = Color.Green;
						}

						ThisIsEnd = true;
					}
				}

				if ( cell.HasFlag( WallState.UP ) )
				{


					//DebugOverlay.Line( position + (new Vector3( 0.5f, 0.5f, 0 ) * scale), position + (new Vector3( -0.5f, 0.5f, 0 ) * scale) );

					GameObject wall = SceneUtility.Instantiate( Wall );
					wall.SetParent( GameObject.Children[0] );
					GeneratedItems.Add( wall );

					wall.Transform.Position = position + (new Vector3( 0, 0.5f, 0 ) * scale);
					wall.Transform.Rotation = Rotation.From( new Angles( 0, -90, 0 ) );


					ThisIsEnd = false;

				}

				if ( cell.HasFlag( WallState.LEFT ) )
				{
					GameObject wall = SceneUtility.Instantiate( Wall );
					wall.SetParent( GameObject.Children[0] );
					GeneratedItems.Add( wall );

					wall.Transform.Position = position + (new Vector3( -0.5f, 0, 0 ) * scale);
				}

				if ( i == width - 1 )
				{

					if ( cell.HasFlag( WallState.RIGHT ) )
					{
						GameObject wall = SceneUtility.Instantiate( Wall );
						wall.SetParent( GameObject.Children[0] );
						GeneratedItems.Add( wall );

						wall.Transform.Position = position + (new Vector3( 0.5f, 0, 0 ) * scale);
					}
				}

				if ( j == 0 )
				{

					if ( !PickedStart && i == width - 1 )
					{
						PickedStart = true;
						StartPos = position + Vector3.Up * scale * 0.5f;
						ThisIsStart = true;
					}

					if ( cell.HasFlag( WallState.DOWN ) )
					{

						GameObject wall = SceneUtility.Instantiate( Wall );
						wall.SetParent( GameObject.Children[0] );
						GeneratedItems.Add( wall );

						wall.Transform.Position = position + (new Vector3( 0, -0.5f, 0 ) * scale);
						wall.Transform.Rotation = Rotation.From( new Angles( 0, 90, 0 ) );

						if ( ThisIsStart )
						{
							var pl = SceneUtility.Instantiate( Player );
							pl.Transform.Position = position + Vector3.Up * scale * 0.5f;
							//pl.SetParent( GameObject.Children[0] );
							End.Player = pl;
							GeneratedItems.Add( pl );
							CurrentPlayerBall = pl;
						}

						ThisIsStart = false;
					}
				}
			}
		}

		GameObject.Children[0].Transform.LocalPosition = -new Vector3( scale * width - 100f, scale * height - 100f, 0 ) / 2f;
	}

}

