using Godot;
using System;
using System.Collections;

public class Barrel : KinematicBody
{
	
	private SpatialMaterial _redOutlineMaterial;
	private PackedScene _explosionScene;
	
	public bool IsExploded = false;

	private bool toExplodeOnPhysics = false;

	private ArrayList overlappingBarrelList = new ArrayList();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {

		this.Connect("mouse_entered",this, "MouseEntered" );
		this.Connect("mouse_exited",this, "MouseExited" );

		//load resources
		_redOutlineMaterial = ResourceLoader.Load<SpatialMaterial>("res://Materials/RedOutlineMaterial.tres");

		//_explosionScene = ResourceLoader.Load<PackedScene>("res://Scenes/ParticleEffects/FireExplosion.tscn"); 
		_explosionScene = ResourceLoader.Load<PackedScene>("res://Scenes/ParticleEffects/FireExplosion2.tscn"); 

		GetNode<Area>("ExplosionArea").InputRayPickable = false;
		
		
		//todo
		GetNode<Area>("ExplosionArea").Connect("area_entered", this, nameof(addBarrelOverlap));
		GetNode<Area>("ExplosionArea").Connect("area_exited", this, nameof(removeBarrelOverlap));


	}

	private void addBarrelOverlap(Area area) {
		if(area.GetParent() is Barrel) {
			Barrel b = area.GetParent<Barrel>();
			if( b.IsExploded) {
				return;
			}
			overlappingBarrelList.Add(area.GetParent<Barrel>());
			GD.Print("Added " +area.GetParent<Barrel>().Name);
		}
	}
	private void removeBarrelOverlap(Area area) {
		if(area.GetParent() is Barrel) {
			
			overlappingBarrelList.Remove(area.GetParent<Barrel>());
			GD.Print("removed " +area.GetParent<Barrel>().Name);
		}
	}


	public override void _PhysicsProcess(float delta){
		
		if(toExplodeOnPhysics) {
			Area area = GetNode<Area>("ExplosionArea");
			Godot.Collections.Array arr = area.GetOverlappingBodies();
			foreach(PhysicsBody body in arr) {
				if(body is EnemyBase) {
					EnemyBase enemy = (EnemyBase) body;
					enemy.takeDamage(15); //TODO scale damage on range
				}
			}

			//GetNode<CollisionShape>("ExplosionArea/CollisionShape").Disabled = true;
			//GetNode<Area>("ExplosionArea").Monitorable = false;
			GetNode<Area>("ExplosionArea").QueueFree();

			
			toExplodeOnPhysics = false;
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

		IsExploded = true;
		toExplodeOnPhysics = true;


		//play explosion
		Spatial tmp = _explosionScene.Instance<Spatial>();
		tmp.Translation = new Vector3(tmp.Translation.x, tmp.Translation.y+1, tmp.Translation.z);
		CPUParticles parfticles = tmp.GetNode<CPUParticles>("CPUParticles");
		AddChild(tmp);
		tmp.Translation = new Vector3(tmp.Translation.x, tmp.Translation.y+1, tmp.Translation.z);
		parfticles.Restart(); //TODO figure how to properly play particles ??

		//Queue barrel deletion timer.
		Timer timer = new Timer();
		AddChild(timer);
		timer.Connect("timeout", this, "queue_free");
		timer.WaitTime = 2f;
		timer.Start();


		//spawn fire balls
		foreach (Barrel barrel in overlappingBarrelList) {
			if(barrel.IsExploded) {
				continue;
			}
			BarrelFireBall fireball = ResourceLoader.Load<PackedScene>("res://Scenes/ParticleEffects/BarrelFireBall.tscn").Instance<BarrelFireBall>();
			fireball.Translation = new Vector3(fireball.Translation.x, fireball.Translation.y+1.5f, fireball.Translation.z);

			fireball.setDestination(barrel);
			//fireball.Connect("body_entered", barrel, nameof(hitByBarrelFireBall));
			AddChild(fireball);
		}

		GetNode<Area>("ExplosionArea").Disconnect("area_entered", this, nameof(addBarrelOverlap));
		GetNode<Area>("ExplosionArea").Disconnect("area_exited", this, nameof(removeBarrelOverlap));
	}



}
