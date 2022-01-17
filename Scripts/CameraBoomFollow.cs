using Godot;
using System;

public class CameraBoomFollow : Position3D
{
	[Export] public NodePath TargetPath;
	private Spatial TargetTranslation;
	

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		TargetTranslation = GetNode<Spatial>(TargetPath);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(float delta)
	{
		Vector3 tmp = TargetTranslation.Translation;
		//tmp.y = Translation.y;

		Translation = tmp;
	}
}
