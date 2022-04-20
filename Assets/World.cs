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
		Vector2 start = map.Count == 0 ? Vector2.zero : map.OrderBy(space => space.Value.Count).First().Key;
	}
}
