using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Weighted random extension methods
/// </summary>
public static class WeightedRandom {
	/// <summary>
	/// Get's an element from the collection based on their weight
	/// </summary>
	/// <param name="collection">Collection to select from</param>
	/// <param name="remove">Should element be taken out of the collection?</param>
	/// <returns>A weighted random element</returns>
    public static T GetWeighted<T>(this ICollection<T> collection, bool remove = false) where T : IWeighted {
        int totalWeight = 0;

		// Get total weight
        foreach (IWeighted weighted in collection) {
            totalWeight += weighted.GetWeight();
        }

        int randomNumber = Random.Range(0, totalWeight);

		// Loop trough all elements and stop at random weight
        foreach (IWeighted weighted in collection) {
            if (randomNumber < weighted.GetWeight()) {
				if (remove) {
					collection.Remove((T)weighted);
				}

				return (T)weighted;
            }

            randomNumber = randomNumber - weighted.GetWeight();
        }

        return default(T);
    }
}

/// <summary>
/// Interface to use the GetWeighted method
/// </summary>
public interface IWeighted {
    int GetWeight();
}
