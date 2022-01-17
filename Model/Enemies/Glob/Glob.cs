using Godot;
using System;

public class Glob : EnemyBase
{

	public static readonly string ANIMATION_TREE_NODE_PATH = "AnimationTree";
	public static readonly string ANIMATION_PLAYER_NODE_PATH = "AnimationPlayer";
	public static readonly string MESH_NODE_PATH = "Armature/Skeleton/GlobMesh";

	protected override float MaxHealth { get{ return 75; } }

	private float jumpUpVelocity = 6f;
	private float jumpForwardVelocity = 4f;
	private float fallAcceleration = 9.81f; 



	private AnimationPlayer animationPlayer;
	private AnimationNodeStateMachinePlayback animationStateMachine;

	private Vector3 velocity = Vector3.Zero;
	
	private bool isJumping = false;


	public override void _Ready() {
		base._Ready();

		_meshNodePath = MESH_NODE_PATH;

		animationPlayer = GetNode<AnimationPlayer>(ANIMATION_PLAYER_NODE_PATH);
		animationStateMachine = (AnimationNodeStateMachinePlayback) GetNode<AnimationTree>(ANIMATION_TREE_NODE_PATH).Get("parameters/playback");
	
	
		// tmp timer for jumping
		Timer timer = new Timer();
		AddChild(timer);
		timer.Connect("timeout", this, "startJump");
		timer.WaitTime = 4f;
		timer.Start();
	
	}


	public override void _PhysicsProcess(float delta) {
		base._PhysicsProcess(delta);

		velocity.y -= fallAcceleration * delta; //apply "gravity"... 
		velocity = MoveAndSlide(velocity, Vector3.Up);
		
		if( isJumping && IsOnFloor() ) {
			isJumping = false;
			this.velocity = Vector3.Zero;
			//animationStateMachine.Travel("JumpLand"); // this animation sucks rate now so its off.
		}

 	}

	// public override void _Process(float delta) {
	// 	base._Process(delta);
	// }


	private void startJump() {
		animationStateMachine.Travel("JumpStart");
	}

	private void jumpNow() {
		
		this.velocity = GlobalTransform.basis.z * jumpForwardVelocity;
		this.velocity.y = jumpUpVelocity;
		isJumping = true;
	}

}
