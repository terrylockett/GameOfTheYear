using Godot;
using System;

public class PlayerTarget : Spatial
{

	private MeshInstance _redMesh;

	private Vector3 _startingScale;
	private float _maxScale = 0.1f;


	 public override void _Ready()
	 {
		_redMesh = GetNode<MeshInstance>("RedMesh");
		_startingScale = _redMesh.Scale;
	 }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
 	public override void _Process(float delta)
 	{
		if(_redMesh.Scale.x > _maxScale) {
			Vector3 scale = _redMesh.Scale;
			scale.x -= delta * 2;
			scale.z -= delta * 2;

			_redMesh.Scale = scale;
		}
		else if(_redMesh.Visible) {
			_redMesh.Hide();
		}
 	}


	public void _on_Ground_input_event(object camera, object @event, Vector3 position, Vector3 normal, int shape_idx)
	{
		if ( @event is InputEventMouseButton mouseEvent ) {
			if(mouseEvent.ButtonIndex == 2 && mouseEvent.IsPressed()) {
				Translation = position;
				_redMesh.Scale = _startingScale;
				_redMesh.Show();
			}
		}
	}


}


