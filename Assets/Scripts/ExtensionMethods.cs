using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Few simple extension methods to make other code more clear
/// </summary>
public static class ExtensionMethods {
	/// <summary>
	/// Returns true if any key from keyCode array is pressed
	/// </summary>
	/// <param name="keys">The keys to be checked</param>
	/// <returns>If any key is pressed</returns>
	public static bool AnyKeyDown(this KeyCode[] keys) {
		foreach (KeyCode key in keys) {
			if (Input.GetKeyDown(key)) {
				return true;
			}
		}

		return false;
	}

	/// <summary>
	/// Get a random element from an Array
	/// </summary>
	/// <param name="array">Array to select from</param>
	/// <returns>A random element</returns>
	public static T GetRandomElement<T>(this T[] array) {
		return array[Random.Range(0, array.Length)];
	}

	/// <summary>
	/// Get a random element from an List
	/// </summary>
	/// <param name="array">List to select from</param>
	/// <returns>A random element</returns>
	public static T GetRandomElement<T>(this IList<T> list) {
		return list[Random.Range(0, list.Count)];
	}
}
