using Godot;
using System;

public class SpiderEnemy : EnemyBase
{
	protected override float MaxHealth { get{ return 50; } }


	private float moveSpeed = 2f;
	

	//tmp things for walking back and forth forever
	private bool dir = true; 
	private RandomNumberGenerator rng;
	


	public override void _Ready()
	{
		base._Ready();


		//tmp walking stuff
		rng = new RandomNumberGenerator();
		rng.Randomize();
		moveSpeed = rng.RandfRange(2,4);
		setMeshColor(new Color(0, rng.RandfRange(0,1), rng.RandfRange(0,1)));
	}


	// public override void _Process(float delta) {
	// 	base._Process(delta);
	// }

	public override void _PhysicsProcess(float delta)
	{
		Vector3 tmp = new Vector3(0,0,1);
		if(!dir){
			tmp = new Vector3(0,0,-1f);
		}


		if(Translation.z > 10) {
			 dir = false;
		}
		if(Translation.z < -10) {
			 dir = true;
		}
		

		 tmp *= moveSpeed;
		 MoveAndSlide(tmp, Vector3.Up);

	}

	
	private void setMeshColor(Color color) {
		MeshInstance mesh = GetNode<MeshInstance>(DEFAULT_MESH_NODE_PATH);
		SpatialMaterial mat = new SpatialMaterial();
		mat.AlbedoColor = color;
		mesh.SetSurfaceMaterial(0, mat);
	}

	
}
