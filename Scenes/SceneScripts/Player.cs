using Godot;
using Godot.Collections;
using System;

public class Player : KinematicBody
{

	public static readonly string ABILITY_Q_INDICATOR_SCENE_PATH = "res://Scenes/Indicators/AbilityIndicator.tscn";
	public static readonly string ABILITY_W_INDICATOR_SCENE_PATH = "res://Scenes/Indicators/BarrelPlacementIndicator.tscn";

	public static readonly string PARENT_CAMERA_NODE_PATH = "CameraPivot/Camera";


	public static readonly string INPUT_ABILIT_Q = "ability_q";
	public static readonly string INPUT_ABILIT_W = "ability_w";


	[Signal] public delegate void PlayerShootBulletSignal(Vector3 Source, Spatial Target);
	[Signal] public delegate void PlayerSpawnBarrel(Vector3 spawnPosition);

	[Export] public int Speed = 13;
	[Export] public int FallAcceleration = 75;
	

	//camera -- TODO fix this shit
	Camera camera;


	//Generic Targets
	private Vector3 _target = Vector3.Zero;
	private Basis _targetLookAtRotation = new Basis();
	private float _rotationLerp = 0.0f;
	private float _rotationSpeed = 4.0f;
	

	//walk targets
	private Vector3 _velocity = Vector3.Zero;
	

	//shooting
	private bool _isShooting = false;
	private bool _isBulletShot = false; 
	private Spatial _shootTarget;


	//Dash stuff.
	private float _dashSpeed = 40;
	private float dashDistance = 8;
	private bool _isDashing = false;
	private Spatial _dashIndicator;
	//dashing - timer
	private float dashTmp = 0;
	private float _dashTotalTime = 0.5f;


	private PackedScene _barrelScene;

	private Spatial _BarrelPlacementIndicator;




	public override void _Ready() {

		camera = GetParent().GetNode<Camera>(PARENT_CAMERA_NODE_PATH);

		//GetNode<AnimationPlayer>("Pivot/Character1/AnimationPlayer").Stop(); //kill default animPlayer.. TODO delete this later.
		_dashIndicator =  ResourceLoader.Load<PackedScene>(ABILITY_Q_INDICATOR_SCENE_PATH).Instance<Spatial>();
		AddChild(_dashIndicator);
		_dashIndicator.Hide();


		_BarrelPlacementIndicator = ResourceLoader.Load<PackedScene>(ABILITY_W_INDICATOR_SCENE_PATH).Instance<Spatial>();
		AddChild(_BarrelPlacementIndicator);
		_BarrelPlacementIndicator.Hide();
	}



	//TODO this and walk share code
	private void _dash_physicsProcess(float delta) { 
		if(dashTmp >= _dashTotalTime){
			_isDashing = false;
			_target = Translation;
			dashTmp = 0;
			return;
		}

		 Vector3 tmpTarget = _target;
		 tmpTarget.y = Transform.origin.y;
		
		_velocity = (tmpTarget - Translation).Normalized() * _dashSpeed;

		if ( Transform.origin.DistanceTo(tmpTarget) < 0.4f) {
			_target = Translation;
			_velocity = Vector3.Zero;
			dashTmp = _dashTotalTime;
		}
		

		_velocity = MoveAndSlideWithSnap(_velocity, Vector3.Down, Vector3.Up, true);

		dashTmp += delta;

	}
	
	private void _DashIndicator_PhysicsProcess(float delta) {
		Vector3 pos =  rayTraceMousePointer();
		pos.y = Translation.y;
		_dashIndicator.LookAt(pos, Vector3.Up);
	}



	

	private void lerpRotation(float delta) {
		 
		Vector3 tmpTarget = _target;
		tmpTarget.y = Transform.origin.y;
		
		if( _rotationLerp < 1) {
			_rotationLerp += delta * _rotationSpeed;
		}
		else if(_rotationLerp > 1) {
			_rotationLerp = 1;
		}

		if(_targetLookAtRotation.Equals(new Basis())){
			return;
		}
		Basis tmp = Transform.basis.Slerp(_targetLookAtRotation, _rotationLerp).Orthonormalized();
		Transform = new Transform(tmp , Transform.origin);
	}


	public override void _PhysicsProcess(float delta)
	{

		if(_rotationLerp != 1) {
			lerpRotation(delta);
		}

		if(_dashIndicator.Visible) {
			_DashIndicator_PhysicsProcess(delta);
		}

		if(_BarrelPlacementIndicator.Visible) {
			_BarrelPlacementIndicator_ProcessPhysics(delta);
		}

		if(_isDashing) {
			_dash_physicsProcess(delta);
			return;
		}

		if(_isShooting && _shootTarget != null ) {
			if(!IsInstanceValid(_shootTarget)){
				_isShooting = false;
			}
			else{
				_targetLookAtRotation = Transform.LookingAt(_shootTarget.Translation, Vector3.Up).basis;
				_rotationLerp = 0.5f; // keep tracking going incase enemy moves.
			}
		}




		Vector3 tmpTarget = _target;
		tmpTarget.y = Transform.origin.y;
		
		if ( Vector3.Zero != _target ) {

			_velocity = (tmpTarget - Translation).Normalized() * Speed;

			if ( Transform.origin.DistanceTo(tmpTarget) < 0.4f) {
				_target = Vector3.Zero;
				_velocity = Vector3.Zero;
			}
		}
		else {
			_velocity.x = 0;
			_velocity.z = 0;
		}
		
		_velocity.y -= FallAcceleration * delta; //apply "gravity"... 
		
		_velocity = MoveAndSlideWithSnap(_velocity, Vector3.Down, Vector3.Up, true);


		//Animations
		if (_velocity != Vector3.Zero) {
			GetNode<AnimationPlayer>("Pivot/Character1/AnimationPlayer2").PlaybackSpeed = 4;
			GetNode<AnimationPlayer>("Pivot/Character1/AnimationPlayer2").Play("Walk");
		}
		else {
			if(!_isShooting) {
			GetNode<AnimationPlayer>("Pivot/Character1/AnimationPlayer2").PlaybackSpeed = 1;
			GetNode<AnimationPlayer>("Pivot/Character1/AnimationPlayer2").Play("IdleWGun");
			}
		}
	}




	public override void _Input(InputEvent inputEvent)
	{
		if(inputEvent.IsActionPressed(INPUT_ABILIT_Q)) {
			_dashIndicator.Show();
		}
		else if (inputEvent.IsActionReleased(INPUT_ABILIT_Q)) {
			abilityQueueReleased(inputEvent);
		}


		if (inputEvent.IsActionPressed(INPUT_ABILIT_W)) {
			_BarrelPlacementIndicator.Show();
		}
		else if (inputEvent.IsActionReleased(INPUT_ABILIT_W)) {
			abilityWReleased(inputEvent);
		}
		
	}


	public Vector3 rayTraceMousePointer() {

		PhysicsDirectSpaceState physicsState =  GetWorld().DirectSpaceState;
		Vector2 mousePosition = GetViewport().GetMousePosition();
		Vector3 rayOrigin = camera.ProjectRayOrigin(mousePosition);
		Vector3 rayEnd = rayOrigin + camera.ProjectRayNormal(mousePosition) * 1000;

		Godot.Collections.Array exclusionArr = new Godot.Collections.Array();
		exclusionArr.Add(this);

		

		Dictionary dict =  physicsState.IntersectRay(rayOrigin, rayEnd, exclusionArr);

		if( dict.Count != 0) { 
			if( dict["position"] != null){
				return  (Vector3)dict["position"];
			}
		}

		GD.Print("Mouse in the void... help!");
		return Vector3.Zero;	
	}


	private void abilityQueueReleased(InputEvent inputEvent) {
		
		if(!_dashIndicator.Visible) {
				return;
		}

		_dashIndicator.Hide();

		if(_isDashing){
			return;
		}

		_rotationLerp = 0;
		_isDashing = true;


		Vector3 mouseLocation =rayTraceMousePointer();
		mouseLocation.y = Translation.y;
		mouseLocation = (mouseLocation - Translation).Normalized() * dashDistance;	
		_target = mouseLocation + Translation;
		
		 _targetLookAtRotation = Transform.LookingAt(_target, Vector3.Up).basis;
	}


	private void _BarrelPlacementIndicator_ProcessPhysics(float delta) {

		Vector3 mouseLocation = rayTraceMousePointer();
		
		if(mouseLocation == Vector3.Zero) {
			return; // mouse in the void.
		}	
		
		mouseLocation.y = 0;
		mouseLocation = ToLocal(mouseLocation);
			
		_BarrelPlacementIndicator.Translation = mouseLocation;
		_BarrelPlacementIndicator.Rotation = Vector3.Zero;	
	}

	private void abilityWReleased (InputEvent inputEvent) { 
		_BarrelPlacementIndicator.Hide();
		
		Vector3 mouseLocation =  rayTraceMousePointer();
		mouseLocation.y = 0;
		SpawnBarrel(mouseLocation);
	}

// ****** Barrel ******

	private void SpawnBarrel(Vector3 spawn_position){
		EmitSignal(nameof(PlayerSpawnBarrel), spawn_position);
	}


// ****** Walking ******
	public void _on_Ground_input_event(object camera, object @event, Vector3 position, Vector3 normal, int shape_idx) {
		if ( @event is InputEventMouseButton mouseEvent ) {
			if(mouseEvent.ButtonIndex == 2 && mouseEvent.IsPressed()) {
				
				_target = position;
				_rotationLerp = 0;

				_isShooting = false;
				_isBulletShot = false;
				_shootTarget = null;


				_targetLookAtRotation = Transform.LookingAt(_target, Vector3.Up).basis;

				if(_dashIndicator.Visible) {
					_dashIndicator.Hide();
				}
			}
		}
	}


//******  Shooting ******

	public void _shoot_input_event(object camera, object @event, Vector3 position, Vector3 normal, int shape_idx, Spatial binds) {
		if ( @event is InputEventMouseButton mouseEvent ) {
			if(mouseEvent.ButtonIndex == 2 && mouseEvent.IsPressed()) {
				shootTarget(binds);
			}
		}
	}

	
	public void shootTarget(Spatial target) {
	
		if(target == _shootTarget) {
			if(!_isBulletShot) {
				return;
			}
		}

		_isShooting = true;
		_isBulletShot = false;

		_target = Vector3.Zero;
		_shootTarget = target;
		_rotationLerp = 0;

		GetNode<AnimationPlayer>("Pivot/Character1/AnimationPlayer2").PlaybackSpeed = 1.5f;
		GetNode<AnimationPlayer>("Pivot/Character1/AnimationPlayer2").Stop();
		GetNode<AnimationPlayer>("Pivot/Character1/AnimationPlayer2").Play("ShootGun");

		
	}

	
	public void shootBulletFromAnimation() {
		Skeleton skele = (Skeleton) GetNode<Skeleton>("Pivot/Character1/Armature/Skeleton/");
		int bulletSpawnTransNum = skele.FindBone("Bone..Gun.Bullet.Spawn");
		Vector3 pos2 =  skele.ToGlobal( skele.GetBoneGlobalPose(bulletSpawnTransNum).origin);
		
		EmitSignal(nameof(PlayerShootBulletSignal), pos2, _shootTarget);

		_isBulletShot = true;
		_shootTarget = null;
	}

}



