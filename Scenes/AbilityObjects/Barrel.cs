using Godot;
using System;

public class Barrel : KinematicBody
{
	
	private SpatialMaterial _redOutlineMaterial;
	private PackedScene _explosionScene;
	
	private bool _isExplosion = false;


	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {

		this.Connect("mouse_entered",this, "MouseEntered" );
		this.Connect("mouse_exited",this, "MouseExited" );

		//load resources
		_redOutlineMaterial = ResourceLoader.Load<SpatialMaterial>("res://Materials/RedOutlineMaterial.tres");

		_explosionScene = ResourceLoader.Load<PackedScene>("res://Scenes/ParticleEffects/FireExplosion.tscn"); 

		GetNode<Area>("ExplosionArea").InputRayPickable = false;
		
		
	}


	public override void _PhysicsProcess(float delta){
		
		if(_isExplosion) {
			Area area = GetNode<Area>("ExplosionArea");
			Godot.Collections.Array arr = area.GetOverlappingBodies();
			foreach(PhysicsBody body in arr) {
				if(body is EnemyBase) {
					EnemyBase enemy = (EnemyBase) body;
					enemy.takeDamage(15); //TODO scale damage on range
				}
			}
			_isExplosion = false;
		}

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(float delta) {
		GetNode<Spatial>("BarrelExplosionindicatorNode").RotateY(.5f * delta);
	}


	public void MouseEntered() {
		MeshInstance mesh = GetNode<MeshInstance>("Cube");
		SpatialMaterial mat = (SpatialMaterial) mesh.GetSurfaceMaterial(0);
		mat.NextPass = _redOutlineMaterial;
		//mat.NextPass.
	}

	public void MouseExited() {
		MeshInstance mesh = GetNode<MeshInstance>("Cube");
		SpatialMaterial mat = (SpatialMaterial) mesh.GetSurfaceMaterial(0);
		mat.NextPass = null;
	}




	public void shotByBullet() {
		//Hide barrel
		GetNode<Spatial>("Cube").Hide();
		GetNode<CollisionShape>("CollisionShape").Hide();
		GetNode<CollisionShape>("CollisionShape").Disabled = true;
		GetNode<Spatial>("BarrelExplosionindicatorNode").Hide();

		_isExplosion = true;

		//GetNode<Area>("ExplosionArea").GetChild<CollisionShape>(0).Disabled = false;

		//play explosion
		Spatial tmp = _explosionScene.Instance<Spatial>();
		tmp.Translation = new Vector3(tmp.Translation.x, tmp.Translation.y+1, tmp.Translation.z);
		CPUParticles parfticles = tmp.GetNode<CPUParticles>("CPUParticles");
		AddChild(tmp);
		parfticles.Restart(); //TODO figure how to properly play particles ??

		//Queue barrel deletion timer.
		Timer timer = new Timer();
		AddChild(timer);
		timer.Connect("timeout", this, "queue_free");
		timer.WaitTime = 2f;
		timer.Start();
	}

}
