using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Simple class that holds data for game obstacles
/// </summary>
public class Obstacle : MonoBehaviour {
	public bool isPickup = false;
	public int score = 100;
	public int laneIndex = 0;
	public AudioClip sound;
}
