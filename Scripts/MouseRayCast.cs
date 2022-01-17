using Godot;
using Godot.Collections;
using System;

public class MouseRayCast : RayCast
{
	private Resource _greenMouseCursor;
	private Resource _blueMouseCursor;
	private Resource _redMouseCursor;

	//private RayCast _rayCast;
	private Camera _camera;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_greenMouseCursor =  ResourceLoader.Load("res://Images/MouseCursors/16/cursor_rough_green_16.png");
		_blueMouseCursor = ResourceLoader.Load("res://Images/MouseCursors/16/cursor_rough_blue_16.png");
		_redMouseCursor = ResourceLoader.Load("res://Images/MouseCursors/16/cursor_rough_red_16.png");

		Input.SetCustomMouseCursor(_greenMouseCursor);


		_camera = GetParent<Camera>();

		// _rayCast = new RayCast();
		// _rayCast.CastTo = _camera.ProjectRayNormal(GetViewport().GetMousePosition()) * 1000;
		
	}


	public override void _PhysicsProcess(float delta)
	{
		PhysicsDirectSpaceState physicsState =  GetWorld().DirectSpaceState;

		Vector2 mousePosition = GetViewport().GetMousePosition();

		Vector3 rayOrigin = _camera.ProjectRayOrigin(mousePosition);
		Vector3 rayEnd = rayOrigin + _camera.ProjectRayNormal(mousePosition) * 1000;

		Dictionary dict = physicsState.IntersectRay(rayOrigin, rayEnd);

		if( dict.Count != 0) { 
			
			object o = dict["collider"];
			if( null != o){
				if(o.GetType().Equals(typeof(SpiderEnemy))){
					Input.SetCustomMouseCursor(_redMouseCursor);
				}
				else{
					Input.SetCustomMouseCursor(_greenMouseCursor);
				}
			}
		}

	}


 // Called every frame. 'delta' is the elapsed time since the previous frame.
 	public override void _Process(float delta)
 	{

		 
		// var camera = GetParent<Camera>();
		// var from = camera.ProjectRayOrigin(GetViewport().GetMousePosition());
		// var to = from + camera.ProjectRayNormal(GetViewport().GetMousePosition()) * 1000;

		// GD.Print(to);
 	}
}
