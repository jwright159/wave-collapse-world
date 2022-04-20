using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
	public int width = 50;
	public int height = 50;

	public Dictionary<Vector2, List<WorldPiece>> map = new Dictionary<Vector2, List<WorldPiece>>();

	private void Update()
	{
		Iterate();
	}

	private void Iterate()
	{
		// Find a space to work on
		Vector2 start = Vector2.zero; // TODO

		// Decide that space
		List<WorldPiece> possibilities = map.ContainsKey(start) ? map[start] : new List<WorldPiece>(WorldPiece.pieces);
		WorldPiece chosenPiece = WeightSelector.RandomWeighted(possibilities);

		map[start] = new List<WorldPiece>();
		map[start].Add(chosenPiece);

		// Propagate
		Propagate(start);
	}

	private void Propagate(Vector2 space)
	{

	}
}
