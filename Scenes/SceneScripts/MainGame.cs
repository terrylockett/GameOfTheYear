using Godot;
using System;
using System.Collections;

public class MainGame : Node
{
	public static readonly string CAMERA_NODE_PATH = "CameraPivot/Camera";
	public static readonly string MAP_SCENE_SCENE_PATH = "res://Scenes/Levels/Map1.tscn";

	public static readonly string BULLET_SCENE_SCENE_PATH = "res://Scenes/AbilityObjects/Bullet.tscn";
	public static readonly string BARREL_SCENE_SCENE_PATH = "res://Scenes/AbilityObjects/Barrel.tscn";
	
	public static readonly string SPIDER_ENEMY_SCENE_PATH = "res://Scenes/Enemies/SpiderEnemy.tscn";
	public static readonly string DINGLE_ENEMY_SCENE_PATH = "res://Scenes/Enemies/DingleEnemy.tscn";
	public static readonly string GLOB_ENEMY_SCENE_PATH = "res://Model/Enemies/Glob/Glob.tscn";

	
	public static readonly string PLAYER_NODE_NAME = "Player";
	public static readonly string PLAYER_TARGET_NODE_NAME = "PlayerTarget";
	public static readonly string ENEMIES_LIST_NODE_NAME = "Enemies";


	[Export] public NodePath PlayerNodePath = new NodePath(PLAYER_NODE_NAME);	


	private Player _playerNode;
	private PackedScene _packed_scene ;
	private PackedScene barrelScene;


	private ArrayList _bullets = new ArrayList();
	private Node BarrelsList = new Node();
	
	

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		this._playerNode = GetNode<Player>(PlayerNodePath);
		this._packed_scene = (PackedScene) ResourceLoader.Load(BULLET_SCENE_SCENE_PATH);

		LoadLevel(MAP_SCENE_SCENE_PATH);
		
		createEnemy(SPIDER_ENEMY_SCENE_PATH, new Vector3(-3,0,2));
		createEnemy(SPIDER_ENEMY_SCENE_PATH, new Vector3(-5,0,6));
		createEnemy(SPIDER_ENEMY_SCENE_PATH, new Vector3(-9,0,-2));
		
		createEnemy(DINGLE_ENEMY_SCENE_PATH, new Vector3(4,0,-8));
		createEnemy(GLOB_ENEMY_SCENE_PATH, new Vector3(9,0,-6)).RotateY(5);


		AddChild(BarrelsList);
		barrelScene = ResourceLoader.Load<PackedScene>(BARREL_SCENE_SCENE_PATH);
		GetNode<Player>(PLAYER_NODE_NAME).Connect(nameof(Player.PlayerSpawnBarrel), this, nameof(_on_Player_PlayerSpawnBarrel));
		
	}


	private void LoadLevel(string mapScene) {
		
		PackedScene tmpPacked = (PackedScene) ResourceLoader.Load(mapScene);
		Map1 map = tmpPacked.Instance() as Map1;
		map.Translation = new Vector3(0,0,0);
		AddChild(map);

		Node navTerrain = map.GetNode<Node>("NavigatableTerrain");

		foreach( Node node in navTerrain.GetChildren() ) {
			//TODO move target into player?
			node.Connect("input_event", _playerNode, nameof(Player._on_Ground_input_event));
			node.Connect("input_event", GetNode(PLAYER_TARGET_NODE_NAME),  nameof(PlayerTarget._on_Ground_input_event));
		}

	}



	private EnemyBase createEnemy(string enemyScenePath, Vector3 spawnTranslation) {
		PackedScene tmp = (PackedScene) ResourceLoader.Load(enemyScenePath);
		
		EnemyBase enemy = tmp.Instance<EnemyBase>();
		enemy.Translation = spawnTranslation;
		enemy.camera = GetNode<Camera>(CAMERA_NODE_PATH);

		GetNode(ENEMIES_LIST_NODE_NAME).AddChild(enemy);
		Godot.Collections.Array array1 = new Godot.Collections.Array();
		array1.Add(enemy);
		
		enemy.Connect("input_event", _playerNode, nameof(Player._shoot_input_event), array1 );

		return enemy;
	}


	private void _on_Player_PlayerShootBulletSignal(Vector3 Source, Spatial Target) {
		Spatial s = _packed_scene.Instance() as Spatial;
		s.Translation = Source;
		_bullets.Add(s);
		Bullet b = 	(Bullet)s;
		b.init(Target);
		AddChild(s);
	}


	private void _on_Player_PlayerSpawnBarrel(Vector3 Source) {
		
		PackedScene tmpScene = ResourceLoader.Load<PackedScene>(BARREL_SCENE_SCENE_PATH);
		Barrel newBarrel = tmpScene.Instance() as Barrel;
		newBarrel.Translation = Source;



		BarrelsList.AddChild(newBarrel);
		Godot.Collections.Array array1 = new Godot.Collections.Array();
		array1.Add(newBarrel);

		newBarrel.Connect("input_event", _playerNode,  nameof(Player._shoot_input_event), array1); 

		
	}

}



