using Godot;
using System;

///
/// <summary>
///     BaseClass for my enemies
///     Scenes root node must be KinematicBody
/// </summary>
public abstract class EnemyBase : KinematicBody
{
    public Camera camera; //TODO - singleton this so it doesnt have to be set by the creator.
    
    protected static readonly string RED_OUTLINE_MATERIAL_SCENE_PATH ="res://Materials/RedOutlineMaterial.tres";
    protected static readonly string ENEMY_HEALTHBAR_SCENE_PATH = "res://Scenes/Hud/EnemyHealthBar.tscn";
    protected static readonly string DEFAULT_MESH_NODE_PATH = "Spatial/Mesh/Cube"; //TODO make property.. also a list for multuiple meshes in one enemy??


    //Mesh and outlining
    protected string _meshNodePath = DEFAULT_MESH_NODE_PATH;
    protected SpatialMaterial _redOutlineMaterial;

    //health
    protected abstract float MaxHealth {get;}
    protected float _health;
    protected EnemyHealthBar _HpBar;
    protected float _hpBarHeight = 3.5f;

    

    public override void _Ready() {  
       
        _health = MaxHealth;

        //mesh hover higlighting
		_redOutlineMaterial = ResourceLoader.Load<SpatialMaterial>(RED_OUTLINE_MATERIAL_SCENE_PATH);
        this.Connect("mouse_entered",this, "MouseEntered" );
		this.Connect("mouse_exited",this, "MouseExited" );

        //create hp bar
		_HpBar = ResourceLoader.Load<PackedScene>(ENEMY_HEALTHBAR_SCENE_PATH).Instance<EnemyHealthBar>();
		_HpBar.RectPosition = camera.UnprojectPosition(Translation);
		AddChild(_HpBar);
    }


    public override void _Process(float delta) {
		updateHpBarPosition();
    }


    protected virtual void updateHpBarPosition() {
        _HpBar.RectPosition = camera.UnprojectPosition(
                new Vector3(Translation.x, (Translation.y + _hpBarHeight), Translation.z)
        );
    }

    public void MouseEntered() {
		getMeshSurfaceMaterial().NextPass = _redOutlineMaterial;
	}

	public void MouseExited() {
		getMeshSurfaceMaterial().NextPass = null;
	}

    private SpatialMaterial getMeshSurfaceMaterial() {
        return GetNode<MeshInstance>(_meshNodePath).GetSurfaceMaterial(0) as SpatialMaterial;
    }

    public void takeDamage(float damage){
        subtractHealth(damage);
    }

    protected void subtractHealth(float amount){
		
        _health -=amount;
		
        if(_health > 0) {
			_HpBar.setHealthPercentage( (_health/MaxHealth) * 100);
		}
		else{
			//TODO kill this mf'r
			QueueFree();
		}
	}

    
}
