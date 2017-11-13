using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

/// <summary>
/// Game class contains all unified data for the game and manages UI
/// </summary>
public class Game : MonoBehaviour {

	public bool gameActive = false;
	public bool sceneryActive = true;

	[Header("Game UI")]
	public RectTransform menuPanel;
	public RectTransform gamePanel;
	public RectTransform endPanel;
	public Button startButton;
	public Button restartButton;

	[Header("Score")]
	public float scorePerSecond = 10f;
	public float score = 0;
	public Text scoreText;
	public Text scoreTextEnd;
	public string scoreFormat = "{0}";

	[Header("Game Settings")]
	public float speed = 50f;
	// Unified lanes values so player and obstacles are always on right point
	public float[] lanes = new float[] { -2f, 0f, 2f };

	public Player player;
	public GameWorld world { get; private set; }

	void Awake() {
		world = GetComponent<GameWorld>();

		gameActive = false;
		sceneryActive = true;
	}

	void Update() {
		// Show the score as a integer
		scoreText.text = string.Format(scoreFormat, (int)score);

		// While game is active add score per second to the score
		if (gameActive) {
			score += scorePerSecond * Time.deltaTime;
		}
	}

	void Start() {
		// Display the right UI panel
		menuPanel.gameObject.SetActive(true);
		gamePanel.gameObject.SetActive(false);
		endPanel.gameObject.SetActive(false);

		// Make it so you can Enter key to continue
		EventSystem.current.SetSelectedGameObject(startButton.gameObject);
	}

	/// <summary>
	/// Start the game, will apply the correct Actives and show right panel
	/// </summary>
	public void StartGame() {
		gameActive = true;
		sceneryActive = true;

		// Start obstacle spawning
		world.SpawnObstacle();

		// Display the right UI panel
		menuPanel.gameObject.SetActive(false);
		gamePanel.gameObject.SetActive(true);
		endPanel.gameObject.SetActive(false);
	}

	/// <summary>
	/// End the game, will apply the correct Actives and show right panel
	/// </summary>
	public void EndGame() {
		gameActive = false;
		sceneryActive = false;

		// Show score on the end screen
		scoreTextEnd.text = string.Format(scoreFormat, (int)score);

		// Stop all spawning on world object
		world.CancelInvoke();

		// Display the right UI panel
		menuPanel.gameObject.SetActive(false);
		gamePanel.gameObject.SetActive(false);
		endPanel.gameObject.SetActive(true);

		// Make it so you can Enter key to continue
		EventSystem.current.SetSelectedGameObject(restartButton.gameObject);
	}
 
	/// <summary>
	/// Restart the game by reloading the current scene
	/// </summary>
	public void Restart() {
		// Reload the current scene
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}
}
