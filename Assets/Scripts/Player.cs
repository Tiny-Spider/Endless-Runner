using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Player class is purely for player movement and control
/// It has a custom collision system in order to perform optimised colliding
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class Player : MonoBehaviour {
	public Game game;
	public AudioClip gameOverAudio;

	[Header("Movement")]
	public float playerLength = 2f;
	public float switchTime = 0.3f;

	[Header("Jump")]
	public float jumpTime = 1f;
	public float jumpDelay = 1f;
	public AnimationCurve jumpCurve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.15f, 1.2f), new Keyframe(0.9f, 1.2f), new Keyframe(1f, 0f));

	[Header("Input")]
	public KeyCode[] keysLeft = new KeyCode[] { KeyCode.A, KeyCode.LeftArrow };
	public KeyCode[] keysRight = new KeyCode[] { KeyCode.D, KeyCode.RightArrow };
	public KeyCode[] keysJump = new KeyCode[] { KeyCode.W, KeyCode.UpArrow, KeyCode.Space };

	// Private varibles
	private GameWorld world;
	private AudioSource audioSource;

	private int laneIndex = 1;
	private float nextJumpTime = 0f;

	private bool isGrounded = true;
	private bool isSwitching = false;

	void Awake() {
		audioSource = GetComponent<AudioSource>();
	}

	void Start() {
		world = game.world;

		// Center the player in the beginning of the game
		StartCoroutine(Switch());
	}

	void Update() {
		if (!game.gameActive)
			return;

		HandleInput();
		HandleCollision();
	}

	/// <summary>
	/// Handles all collision, custom implementation for performance
	/// <summary>
	void HandleCollision() {
		List<Obstacle> obstacles = world.GetObstacles();

		// Reverse loop over objects by index to prevent iteration errors
		for (int i = obstacles.Count - 1; i >= 0; i--) {
			Obstacle obstacle = obstacles[i];
			
			// Remove item from the list if it's empty or null
			if (!obstacle) {
				obstacles.RemoveAt(i);
				continue;
			}

			// Check if obstacle is in same lane as the player
			if (laneIndex != obstacle.laneIndex) {
				continue;
			}
			
			Vector3 position = obstacle.transform.position;
			// Check if obstacle Z is within the player's Z plus playerLength
			if (isGrounded && position.z < transform.position.z + playerLength && position.z > transform.position.z - playerLength) {

				// Pickup logic
				if (obstacle.isPickup) {
					game.score += obstacle.score;

					// If there is audio play it
					if (obstacle.sound) 
						audioSource.PlayOneShot(obstacle.sound);

					// Remove it from the game
					Destroy(obstacle.gameObject);
				}
				else {
					audioSource.PlayOneShot(gameOverAudio);

					game.EndGame();
				}
			}
		}
	}

	/// <summary>
	/// Handle input for the player
	/// </summary>
	void HandleInput() {
		if (!isGrounded || isSwitching) {
			return;
		}

		// Check if any left key was pressed
		if (keysLeft.AnyKeyDown()) {

			// Switch one lane to the left (-1)
			if (laneIndex - 1 >= 0) {
				laneIndex--;
				StartCoroutine(Switch());
			}
		}

		// Check if any right key was pressed
		if (keysRight.AnyKeyDown()) {

			// Switch one lane to the right (+1)
			if (laneIndex + 1 < game.lanes.Length) {
				laneIndex++;
				StartCoroutine(Switch());
			}
		}

		// Check if any jump key was pressed
		if (keysJump.AnyKeyDown()) {

			// Check if there isn't a cooldown
			if (Time.time > nextJumpTime) {
				StartCoroutine(Jump());
			}
		}
	}

	/// <summary>
	/// A switch lane coroutine
	/// isSwitching will be true as long as the player is switching lanes
	/// </summary>
	IEnumerator Switch() {
		isSwitching = true;

		// Get the start position and X position
		Vector3 position = transform.position;
		float startX = position.x;
		float activeSwitchTime = 0f;

		// While still switching lanes
		while (activeSwitchTime <= switchTime) {
			activeSwitchTime += Time.deltaTime;

			// Lerp X position from start to current lane
			position.x = Mathf.Lerp(startX, game.lanes[laneIndex], activeSwitchTime / switchTime);
			transform.position = position;

			yield return null;
		}

		isSwitching = false;
	}

	/// <summary>
	/// A jump coroutine
	/// isGrounded will be false as long as the player is jumping
	/// </summary>
	IEnumerator Jump() {
		isGrounded = false;

		float activeJumpTime = 0f;

		// While still in jump
		while (activeJumpTime <= jumpTime) {
			activeJumpTime += Time.deltaTime;

			// Instead of lerp we use animationcurve for our jump curve
			Vector3 position = transform.position;
			position.y = jumpCurve.Evaluate(activeJumpTime / jumpTime);
			transform.position = position;

			yield return null;
		}

		// Add delay the player can't jump forever
		nextJumpTime = Time.time + jumpDelay;
		isGrounded = true;
	}
}