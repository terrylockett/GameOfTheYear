using Godot;
using System;

public class BarrelFireBall : Area
{
	private Barrel destination;

    private Vector3 velocity = Vector3.Zero;
    private float acceleration = 200;
    private float maxSpeed = 60;
    private float initalSpeed = 20;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		GetNode<CPUParticles>("CPUParticles").Emitting = true;
        GetNode<CPUParticles>("CPUParticles").OneShot = false;

        GD.Print("dest: " + destination.Translation);
        GD.Print("tran: " + ToGlobal(Translation));


        velocity = destination.Translation;
        velocity.y += 1.5f;
        velocity = ToLocal(velocity) - Translation;
        velocity = velocity.Normalized() ;


        Connect("body_entered", this, nameof(barrelHit));

	}

    public void barrelHit(Node node) {
        if(node.Equals(destination)){
            destination.shotByBullet();
            QueueFree();
        }
    }

	public void setDestination(Barrel destination) {
		this.destination = destination;

       
      //  tmp = tmp.Normalized();

	}

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(float delta) {
		
        if(destination.IsExploded) {
            //jobs done.
            QueueFree();
        }


       Translation += velocity * maxSpeed * delta;


        // if(velocity != Vector3.Zero) {
            
        //     velocity += velocity.Normalized() * acceleration * delta;
        //     if(velocity.Length() > maxSpeed) {
        //         velocity = velocity.Normalized() * maxSpeed;
        //     }
        //     Translation += velocity * delta;
        // }
	}


}
