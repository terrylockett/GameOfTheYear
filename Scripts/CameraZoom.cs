using Godot;
using System;

public class CameraZoom : Camera {
	[Export] public float ZoomSpeed = 50;
	[Export] public float MaxZoomValue = 25;
	[Export] public float MinZoomValue = 10;

	// Called when the node enters the scene tree for the first time.
	// public override void _Ready() {
	// }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(float delta) {
		
		if (Input.IsActionJustReleased("zoom_in")){
			Vector3 tmpTrans = Transform.origin;
			tmpTrans.z -= delta *50;
			if(tmpTrans.z < MinZoomValue) {
				tmpTrans.z = MinZoomValue;
			}
			Translation = tmpTrans;


			//Size -= delta * ZoomSpeed; // orthographic cam

		}
		else if(Input.IsActionJustReleased("zoom_out")) {
			Vector3 tmpTrans = Transform.origin;
			tmpTrans.z += delta *50;
			if(tmpTrans.z > MaxZoomValue) {
				tmpTrans.z = MaxZoomValue;
			}

			Translation = tmpTrans;
		//	Size += delta * ZoomSpeed; // orthographic cam
		}
	}
}
