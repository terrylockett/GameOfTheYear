using Godot;
using System;

public class DingleEnemy : EnemyBase
{
	public static readonly string MESH_NODE_PATH = "FloatingNode/dingle/Cube";
	public static readonly string FLOATING_NODE_PATH = "FloatingNode";

	protected override float MaxHealth { get {return 100;} }

	

	private Spatial _floatingNode;

	private float _floatCeiling = 1f;
	private float _floatFloor = 0f;
	private float _floatingAcceleration = .025f;

	private float _floatingMaxSpeed = 4f;
	private float _floatingMinSpeed = .5f;


	private bool _isFloatingUp = true;
	private Vector3 _floatVelocity = Vector3.Zero;
	float Floatindex = 0;
	


	public override void _Ready() {
		base._Ready();

		_meshNodePath = MESH_NODE_PATH;
		_hpBarHeight = 6;

		_floatingNode = GetNode<Spatial>(FLOATING_NODE_PATH);
	}


	protected override void updateHpBarPosition() {
		Vector3 tmp = Translation + _floatingNode.Translation;
		_HpBar.RectPosition = camera.UnprojectPosition( new Vector3	( tmp.x, (tmp.y + _hpBarHeight), tmp.z ));
	}
	

	public override void _Process(float delta) {
		base._Process(delta);

		//Bob up and down via sin wave.
		Floatindex += delta;
		float yTmp =  (.5f*Mathf.Sin (1.5f*Floatindex));
		
		_floatingNode.Translation = new Vector3(
			_floatingNode.Translation.x, 
			yTmp,
			_floatingNode.Translation.z);



		//tmp roaion
		_floatingNode.RotateY(.7f*delta);

	}
}
