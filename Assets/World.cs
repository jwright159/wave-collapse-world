using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
	public int width = 50;
	public int height = 50;
	public int depth = 50;

	public Space[,,] map;

	private bool done = false;

	private void Start()
	{
		map = new Space[width, height, depth];
		for (int i = 0; i < width; i++)
			for (int j = 0; j < height; j++)
				for (int k = 0; k < depth; k++)
					map[i, j, k] = new Space(this, i, j, k);
	}

	private void Update()
	{
		if (!done)
			Iterate();
	}

	private void Iterate()
	{
		// Find a space to work on
		IEnumerable<Space> uncollapsedSpaces = from space in map.Cast<Space>() where space.Piece == null orderby space.Possibilities select space;
		if (uncollapsedSpaces.Count() == 0)
		{
			done = true;
			return;
		}
		int possibilities = uncollapsedSpaces.First().Possibilities;
		Space startSpace = WeightSelector.RandomOf(from space in uncollapsedSpaces where space.Possibilities == possibilities select space);

		// Decide that space
		startSpace.CollapseFully();

		// Propagate
		List<Space> spacesToCollapse = new List<Space>();
		spacesToCollapse.AddAllIf(space => IsSpaceCollapsable(space, spacesToCollapse),
			startSpace.Right, startSpace.Left, startSpace.Forward, startSpace.Back, startSpace.Up, startSpace.Down);

		while (spacesToCollapse.Count > 0)
		{
			Space collapsingSpace = spacesToCollapse.Pop();
			if (collapsingSpace.Collapse())
				spacesToCollapse.AddAllIf(space => IsSpaceCollapsable(space, spacesToCollapse),
					collapsingSpace.Right, collapsingSpace.Left, collapsingSpace.Forward, collapsingSpace.Back, collapsingSpace.Up, collapsingSpace.Down);
		}
	}

	public Space this[int x, int y, int z]
	{
		get
		{
			if (x < 0 || x >= width || y < 0 || y >= height || z < 0 || z >= depth)
				return null;
			return map[x, y, z];
		}
	}

	public static bool IsSpaceCollapsable(Space space, List<Space> list) => space != null && space.Piece == null && !list.Contains(space);
}
