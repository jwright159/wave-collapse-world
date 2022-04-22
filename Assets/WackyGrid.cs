using System.Linq;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class WackyGrid : MonoBehaviour
{
	private Mesh mesh;
	private Cell[,,,] cells;

	private const int xways = 2, yways = 2, zways = 2;

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
		cells = new Cell[xways, yways, zways, 12];
		/* Cell rotation list:
		 * forward up
		 * forward left
		 * forward down
		 * forward right
		 * right up
		 * right forward
		 * right down
		 * right back
		 * up forward
		 * up left
		 * up back
		 * up right
		 */

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

					if (x > 0 && y > 0 && z > 0 && z < zways)
						cells[x - 1, y - 1, z - 1, 0] = new Cell
						{
							mesh = mesh,
							vertices = new int[]
							{
								vertexGrid[x - 1, y - 1, z - 1, 1],
								vertexGrid[x - 1, y - 1, z, 1],
								vertexGrid[x - 1, y, z, 0],
								vertexGrid[x, y, z, 0],
							},
							neighbors = new (int, int, int, int)[]
							{
								(x - 1, y - 1, z - 1, 1),
								(x - 1, y - 1, z - 1, 3),
							}
						};

					if (x < xways && y > 0 && z > 0 && z < zways)
						cells[x, y - 1, z - 1, 1] = new Cell
						{
							mesh = mesh,
							vertices = new int[]
							{
								vertexGrid[x, y - 1, z - 1, 1],
								vertexGrid[x, y - 1, z, 0],
								vertexGrid[x, y - 1, z, 1],
								vertexGrid[x, y, z, 0],
							},
						};

					if (x > 0 && y < yways && z > 0 && z < zways)
						cells[x - 1, y, z - 1, 2] = new Cell
						{
							mesh = mesh,
							vertices = new int[]
							{
								vertexGrid[x - 1, y, z - 1, 1],
								vertexGrid[x - 1, y, z, 0],
								vertexGrid[x - 1, y, z, 1],
								vertexGrid[x, y, z, 0],
							},
						};

					if (x > 0 && y > 0 && z > 0 && z < zways)
						cells[x - 1, y - 1, z - 1, 3] = new Cell
						{
							mesh = mesh,
							vertices = new int[]
							{
								vertexGrid[x - 1, y - 1, z - 1, 1],
								vertexGrid[x - 1, y - 1, z, 1],
								vertexGrid[x, y - 1, z, 0],
								vertexGrid[x, y, z, 0],
							},
						};
				}
			}
		}

		mesh.vertices = vertices.ToArray();
		mesh.subMeshCount = 2;
		mesh.SetIndices(edges.SelectMany(((int a, int b) edge) => new int[] { edge.a, edge.b }).ToArray(), MeshTopology.Lines, 0);
		mesh.SetIndices(edgesValidForRemoval.SelectMany(((int a, int b) edge) => new int[] { edge.a, edge.b }).ToArray(), MeshTopology.Lines, 1);
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.blue;
		foreach (Cell cell in cells)
		{
			if (cell.mesh == null)
				continue;

			Gizmos.DrawWireSphere(cell.Center, 0.05f);
			if (cell.neighbors != null)
				foreach ((int x, int y, int z, int w) in cell.neighbors)
					Gizmos.DrawRay(cell.Center, (cells[x, y, z, w].Center - cell.Center) * 0.4f);
		}
	}
}

public struct Cell
{
	public Mesh mesh;
	public int[] vertices;
	public (int, int)[] edges;
	public (int, int, int, int)[] neighbors;

	private Vector3 center;
	public Vector3 Center {
		get
		{
			if (center == Vector3.zero)
			{
				Mesh mesh = this.mesh;
				center = vertices.Select(vertex => mesh.vertices[vertex]).Aggregate((vertex1, vertex2) => vertex1 + vertex2) / vertices.Length;
			}
			return center;
		}
	}
}