using System.Linq;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class WackyGrid : MonoBehaviour
{
	private Mesh mesh;

	private const int xways = 5, yways = 5, zways = 5;

	private void OnValidate()
	{
		if (!mesh)
			mesh = GetComponent<MeshFilter>().mesh = new Mesh();
		else
			mesh.Clear();
		GenerateRhombuses();
	}

	private void GenerateRhombuses()
	{
		List<Vector3> vertices = new List<Vector3>();
		int[,,,] vertexGrid = new int[xways + 1, yways + 1, zways + 1, 2];
		List<(int, int)> edges = new List<(int, int)>();
		List<(int, int)> edgesValidForRemoval = new List<(int, int)>();

		for (int z = 0; z <= zways; z++)
		{
			for (int y = 0; y <= yways; y++)
			{
				for (int x = 0; x <= xways; x++)
				{
					vertices.Add(new Vector3(x, y, z));
					vertexGrid[x, y, z, 0] = vertices.Count - 1;

					if (x < xways && y < yways && z < zways)
					{
						vertices.Add(new Vector3(x + 0.5f, y + 0.5f, z + 0.5f));
						vertexGrid[x, y, z, 1] = vertices.Count - 1;
					}

					if (x < xways && y < yways && z < zways)
					{
						edgesValidForRemoval.Add((vertexGrid[x, y, z, 0], vertexGrid[x, y, z, 1]));
					}

					if (x > 0)
					{
						edges.Add((vertexGrid[x - 1, y, z, 0], vertexGrid[x, y, z, 0]));

						if (x < xways)
							edges.Add((vertexGrid[x - 1, y, z, 1], vertexGrid[x, y, z, 1]));

						if (y < yways && z < zways)
							edgesValidForRemoval.Add((vertexGrid[x - 1, y, z, 1], vertexGrid[x, y, z, 0]));
					}

					if (y > 0)
					{
						edges.Add((vertexGrid[x, y - 1, z, 0], vertexGrid[x, y, z, 0]));

						if (y < yways)
							edges.Add((vertexGrid[x, y - 1, z, 1], vertexGrid[x, y, z, 1]));
						
						if (x < xways && z < zways)
							edgesValidForRemoval.Add((vertexGrid[x, y - 1, z, 1], vertexGrid[x, y, z, 0]));
					}

					if (z > 0)
					{
						edges.Add((vertexGrid[x, y, z - 1, 0], vertexGrid[x, y, z, 0]));

						if (z < zways)
							edges.Add((vertexGrid[x, y, z - 1, 1], vertexGrid[x, y, z, 1]));

						if (x < xways && y < yways)
							edgesValidForRemoval.Add((vertexGrid[x, y, z - 1, 1], vertexGrid[x, y, z, 0]));
					}

					if (x > 0 && y > 0 && z < zways)
					{
						edgesValidForRemoval.Add((vertexGrid[x - 1, y - 1, z, 1], vertexGrid[x, y, z, 0]));
					}

					if (x > 0 && y < yways && z > 0)
					{
						edgesValidForRemoval.Add((vertexGrid[x - 1, y, z - 1, 1], vertexGrid[x, y, z, 0]));
					}

					if (x < xways && y > 0 && z > 0)
					{
						edgesValidForRemoval.Add((vertexGrid[x, y - 1, z - 1, 1], vertexGrid[x, y, z, 0]));
					}

					if (x > 0 && y > 0 && z > 0)
					{
						edgesValidForRemoval.Add((vertexGrid[x - 1, y - 1, z - 1, 1], vertexGrid[x, y, z, 0]));
					}
				}
			}
		}

		mesh.vertices = vertices.ToArray();
		mesh.subMeshCount = 2;
		mesh.SetIndices(edges.SelectMany(a => new int[] { a.Item1, a.Item2 }).ToArray(), MeshTopology.Lines, 0);
		mesh.SetIndices(edgesValidForRemoval.SelectMany(a => new int[] { a.Item1, a.Item2 }).ToArray(), MeshTopology.Lines, 1);
	}
}
