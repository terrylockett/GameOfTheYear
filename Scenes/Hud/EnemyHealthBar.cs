using Godot;
using System;

public class EnemyHealthBar : Control
{
	
	private ColorRect _healthBar;

	private float _maxHealthRectX;
	private float _healthPercentage = 100;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_healthBar = GetNode<ColorRect>("HealthBox");
		_maxHealthRectX = _healthBar.RectSize.x;
	}

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }


	public void setHealthPercentage(float percentage) {
		_healthPercentage = percentage;
		updateHealthBarLength();
	}

	private void updateHealthBarLength() {
		Vector2 tmp =_healthBar.RectSize;
		tmp.x = (_healthPercentage/100) * _maxHealthRectX;

		_healthBar.RectSize = tmp;
	}

}
