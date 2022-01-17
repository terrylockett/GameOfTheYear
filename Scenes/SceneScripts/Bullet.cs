using Godot;
using System;

public class Bullet : Area {
	private Spatial _target;
	private float bulletSpeed = 60;

	private float bulletDamage = 10;
	

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		LookAt(_target.Translation, Vector3.Up);

		Connect("body_entered", this, nameof(_on_Bullet_body_entered));

	}


	public void init(Spatial Target) {
		this._target = Target;
	}

	public override void _PhysicsProcess(float delta)
	{		
		Vector3 velocity = Translation - _target.Translation;
		velocity.y -= 1.35f;
		velocity = velocity.Normalized();

		Translation -= velocity * delta * bulletSpeed;
	
	}

	private void _on_Bullet_body_entered(object body) {
		
		if(body != _target) {
			return;
		}

		bool hit = false;
			
		if(body is EnemyBase) {
			hit = true;
			EnemyBase tmp = (EnemyBase) body;
			tmp.takeDamage(bulletDamage);
			
		}
		else if(body is Barrel) {
			hit = true;
			Barrel tmp = (Barrel) body;
			tmp.shotByBullet();
		}

		if(hit) {
			QueueFree();
		}

	}
	




//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}


