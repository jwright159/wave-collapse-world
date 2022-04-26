using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
	public int width = 50;
	public int height = 50;

	public Space[,] map;

	private void Awake()
	{
		map = new Space[width, height];
	}

	private void Update()
	{
		Iterate();
	}

	private void Iterate()
	{
		// Find a space to work on
		(int x, int y) = (Random.Range(0, width), Random.Range(0, height)); // TODO

		// Decide that space
		Space space = map[x, y] ?? (map[x, y] = new Space());
		space.CollapseFully();

		// Propagate
		Propagate(start);
	}

	private void Propagate(Vector2 space)
	{

	}
}
