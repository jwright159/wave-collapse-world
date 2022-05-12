using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public interface IWeighted
{
	public float Weight { get; }
}

public static class WeightSelector
{
	public static T RandomWeighted<T>(IEnumerable<T> source) where T : IWeighted
	{
		float totalWeight = source.Sum(part => part.Weight);
		float chosenWeight = UnityEngine.Random.Range(0, totalWeight);
		foreach (T part in source)
		{
			chosenWeight -= part.Weight;
			if (chosenWeight < 0)
				return part;
		}
		throw new InvalidOperationException($"Couldn't get a random value from {source}, left with {chosenWeight} weight");
	}

	public static T RandomOf<T>(IEnumerable<T> source)
	{
		return source.ElementAt(UnityEngine.Random.Range(0, source.Count()));
	}
}
