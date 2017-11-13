using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
///  A game world management class
/// This class moves and spawns objects in the game
/// </summary>
[RequireComponent(typeof(Game))]
public class GameWorld : MonoBehaviour {
	public float despawnZ = -4f;

	[Header("Obstacles")]
	public ObstacleData[] obstaclePrefabs;
	public float obstacleSpawnZ = 100f;
	public float minObstacleSpawnDelay = 0.5f;
	public float maxObstacleSpawnDelay = 1.5f;

	[Header("Scenery")]
	public GameObject[] sceneryPrefabs;
	public Transform[] scenerySpawnPoints;
	public float minScenerySpawnDelay = 0.5f;
	public float maxScenerySpawnDelay = 1.5f;

	private Game game;

	private List<Obstacle> obstacleObjects = new List<Obstacle>();
	private List<Transform> sceneryObjects = new List<Transform>();

	void Awake() {
		game = GetComponent<Game>();
	}

	void Start() {
		SpawnScenery();
	}

	void Update() {
		if (game.gameActive) {
			MoveObjects(obstacleObjects);
		}
		if (game.sceneryActive) {
			MoveObjects(sceneryObjects);
		}
	}

	/// <summary>
	/// Generic method for moving objects, this way we can give it either Transform or a custom class
	/// </summary>
	/// <param name="objects">List of T components to update</param>
	void MoveObjects<T>(List<T> objects) where T : Component {
		// Reverse loop over objects by index to prevent iteration errors
		for (int i = objects.Count - 1; i >= 0; i--) {
			Component component = objects[i];

			// Remove item from the list if it's empty or null
			if (!component) {
				objects.RemoveAt(i);
				continue;
			}

			Vector3 position = component.transform.position;

			// Destroy object if Z is lower than despawnZ
			if (position.z < despawnZ) {
				Destroy(component.gameObject);
				continue;
			}

			position.z = position.z - (game.speed * Time.deltaTime);
			component.transform.position = position;
		}
	}

	/// <summary>
	/// Spawns obstacles on different lanes, will call itself afterwards (loop)
	/// </summary>
	public void SpawnObstacle() {
		// Pick a amount of obstacles and a start index
		int amount = Random.Range(1, game.lanes.Length + 1);
		int startIndex = Random.Range(0, game.lanes.Length);
		bool hasSpawnedPickup = false;

		for (int i = 0; i < amount; i++) {
			// Make sure we can only spawn a full obstacle row if there is atleast one pickup
			if (i == amount - 1 && amount == game.lanes.Length) {
				if (!hasSpawnedPickup)
					continue;
			}

			// Pick a lane index and create appropiate position
			int laneIndex = startIndex++ % game.lanes.Length;
			Vector3 spawnPosition = new Vector3(game.lanes[laneIndex], 0, obstacleSpawnZ);

			// Pick a obstacle prefab and spawn it
			ObstacleData obstaclePrefab = obstaclePrefabs.GetWeighted();
			Obstacle obstacle = Instantiate(obstaclePrefab.obstacle, spawnPosition, obstaclePrefab.obstacle.transform.rotation);

			if (obstacle.isPickup) {
				hasSpawnedPickup = true;
			}

			obstacle.laneIndex = laneIndex;
			obstacleObjects.Add(obstacle);
		}

		Invoke("SpawnObstacle", Random.Range(minObstacleSpawnDelay, maxObstacleSpawnDelay));
	}

	/// <summary>
	/// Spawns scenery objects, will call itself afterwards (loop)
	/// </summary>
	public void SpawnScenery() {
		// Pick a spawnpoint
		Transform spawnPoint = scenerySpawnPoints.GetRandomElement();

		// Pick a scenery prefab and spawn it
		GameObject sceneryPrefab = sceneryPrefabs.GetRandomElement();
		GameObject scenery = Instantiate(sceneryPrefab, spawnPoint.position, sceneryPrefab.transform.rotation);

		sceneryObjects.Add(scenery.transform);

		Invoke("SpawnScenery", Random.Range(minScenerySpawnDelay, maxScenerySpawnDelay));
	}

	/// <summary>
	/// Get all current obstacles in the game (moving game objects)
	/// </summary>
	/// <returns>List of obstacles</returns>
	public List<Obstacle> GetObstacles() {
		return obstacleObjects;
	}

	/// <summary>
	/// Simple struct that implements IWeighted for weighted method
	/// </summary>
	[System.Serializable]
	public struct ObstacleData : IWeighted {
		public Obstacle obstacle;
		public int weight;

		public int GetWeight() {
			return weight;
		}
	}
}