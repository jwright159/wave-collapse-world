using System.Linq;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class WackyGrid : MonoBehaviour
{
	private Mesh mesh;
	private Dictionary<CellDirection, Cell>[,,] cells;

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
		if (cells == null)
		{
			cells = new Dictionary<CellDirection, Cell>[xways, yways, zways];
			for (int x = 0; x < xways; x++)
				for (int y = 0; y < yways; y++)
					for (int z = 0; z < zways; z++)
						cells[x, y, z] = new Dictionary<CellDirection, Cell>();
		}
		else
			for (int x = 0; x < xways; x++)
				for (int y = 0; y < yways; y++)
					for (int z = 0; z < zways; z++)
						cells[x, y, z].Clear();

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

					if (z > 0 && z < zways)
					{
						if (x > 0 && y > 0)
							cells[x - 1, y - 1, z - 1][CellDirection.ForwardUp] = new Cell(
								new int[]
								{
									vertexGrid[x - 1, y - 1, z - 1, 1],
									vertexGrid[x - 1, y - 1, z, 1],
									vertexGrid[x - 1, y, z, 0],
									vertexGrid[x, y, z, 0],
								},
								new (int, int, int, CellDirection)[]
								{
									(x - 1, y - 1, z - 1, CellDirection.ForwardLeft),
									(x - 1, y - 1, z - 1, CellDirection.ForwardRight),
									y < yways ? (x - 1, y - 1, z - 1, CellDirection.UpForward) : Cell.invalid,
									y < yways ? (x - 1, y - 1, z, CellDirection.UpBack) : Cell.invalid,
								}.Where(coords => coords.Item4 != CellDirection.None).ToArray()
							);

						if (x < xways && y > 0)
							cells[x, y - 1, z - 1][CellDirection.ForwardLeft] = new Cell(
								new int[]
								{
									vertexGrid[x, y - 1, z - 1, 1],
									vertexGrid[x, y - 1, z, 0],
									vertexGrid[x, y - 1, z, 1],
									vertexGrid[x, y, z, 0],
								},
								new (int, int, int, CellDirection)[]
								{
									(x, y - 1, z - 1, CellDirection.ForwardUp),
									(x, y - 1, z - 1, CellDirection.ForwardDown),
									x > 0 ? (x - 1, y - 1, z - 1, CellDirection.RightForward) : Cell.invalid,
									x > 0 ? (x - 1, y - 1, z, CellDirection.RightBack) : Cell.invalid,
								}.Where(coords => coords.Item4 != CellDirection.None).ToArray()
							);

						if (x > 0 && y < yways)
							cells[x - 1, y, z - 1][CellDirection.ForwardDown] = new Cell(
								new int[]
								{
									vertexGrid[x - 1, y, z - 1, 1],
									vertexGrid[x - 1, y, z, 0],
									vertexGrid[x - 1, y, z, 1],
									vertexGrid[x, y, z, 0],
								},
								new (int, int, int, CellDirection)[]
								{
									(x - 1, y, z - 1, CellDirection.ForwardLeft),
									(x - 1, y, z - 1, CellDirection.ForwardRight),
									y > 0 ? (x - 1, y - 1, z - 1, CellDirection.UpForward) : Cell.invalid,
									y > 0 ? (x - 1, y - 1, z, CellDirection.UpBack) : Cell.invalid,
								}.Where(coords => coords.Item4 != CellDirection.None).ToArray()
							);

						if (x > 0 && y > 0)
							cells[x - 1, y - 1, z - 1][CellDirection.ForwardRight] = new Cell(
								new int[]
								{
									vertexGrid[x - 1, y - 1, z - 1, 1],
									vertexGrid[x - 1, y - 1, z, 1],
									vertexGrid[x, y - 1, z, 0],
									vertexGrid[x, y, z, 0],
								},
								new (int, int, int, CellDirection)[]
								{
									(x - 1, y - 1, z - 1, CellDirection.ForwardUp),
									(x - 1, y - 1, z - 1, CellDirection.ForwardDown),
									x < xways ? (x - 1, y - 1, z - 1, CellDirection.RightForward) : Cell.invalid,
									x < xways ? (x - 1, y - 1, z, CellDirection.RightBack) : Cell.invalid,
								}.Where(coords => coords.Item4 != CellDirection.None).ToArray()
							);
					}

					if (x > 0 && x < xways)
					{
						if (y > 0 && z > 0)
							cells[x - 1, y - 1, z - 1][CellDirection.RightUp] = new Cell(
								new int[]
								{
									vertexGrid[x - 1, y - 1, z - 1, 1],
									vertexGrid[x, y - 1, z - 1, 1],
									vertexGrid[x, y, z - 1, 0],
									vertexGrid[x, y, z, 0],
								},
								new (int, int, int, CellDirection)[]
								{
									(x - 1, y - 1, z - 1, CellDirection.RightForward),
									(x - 1, y - 1, z - 1, CellDirection.RightBack),
									y < yways ? (x - 1, y - 1, z - 1, CellDirection.UpRight) : Cell.invalid,
									y < yways ? (x, y - 1, z - 1, CellDirection.UpLeft) : Cell.invalid,
								}.Where(coords => coords.Item4 != CellDirection.None).ToArray()
							);

						if (y > 0 && z > 0)
							cells[x - 1, y - 1, z - 1][CellDirection.RightForward] = new Cell(
								new int[]
								{
									vertexGrid[x - 1, y - 1, z - 1, 1],
									vertexGrid[x, y - 1, z - 1, 1],
									vertexGrid[x, y - 1, z, 0],
									vertexGrid[x, y, z, 0],
								},
								new (int, int, int, CellDirection)[]
								{
									(x - 1, y - 1, z - 1, CellDirection.RightDown),
									(x - 1, y - 1, z - 1, CellDirection.RightUp),
									z < zways ? (x - 1, y - 1, z - 1, CellDirection.ForwardRight) : Cell.invalid,
									z < zways ? (x, y - 1, z - 1, CellDirection.ForwardLeft) : Cell.invalid,
								}.Where(coords => coords.Item4 != CellDirection.None).ToArray()
							);

						if (y < yways && z > 0)
							cells[x - 1, y, z - 1][CellDirection.RightDown] = new Cell(
								new int[]
								{
									vertexGrid[x - 1, y, z - 1, 1],
									vertexGrid[x, y, z - 1, 0],
									vertexGrid[x, y, z - 1, 1],
									vertexGrid[x, y, z, 0],
								},
								new (int, int, int, CellDirection)[]
								{
									(x - 1, y, z - 1, CellDirection.RightForward),
									(x - 1, y, z - 1, CellDirection.RightBack),
									y > 0 ? (x - 1, y - 1, z - 1, CellDirection.UpRight) : Cell.invalid,
									y > 0 ? (x, y - 1, z - 1, CellDirection.UpLeft) : Cell.invalid,
								}.Where(coords => coords.Item4 != CellDirection.None).ToArray()
							);

						if (y > 0 && z < zways)
							cells[x - 1, y - 1, z][CellDirection.RightBack] = new Cell(
								new int[]
								{
									vertexGrid[x - 1, y - 1, z, 1],
									vertexGrid[x, y - 1, z, 0],
									vertexGrid[x, y - 1, z, 1],
									vertexGrid[x, y, z, 0],
								},
								new (int, int, int, CellDirection)[]
								{
									(x - 1, y - 1, z, CellDirection.RightDown),
									(x - 1, y - 1, z, CellDirection.RightUp),
									z > 0 ? (x - 1, y - 1, z - 1, CellDirection.ForwardRight) : Cell.invalid,
									z > 0 ? (x, y - 1, z - 1, CellDirection.ForwardLeft) : Cell.invalid,
								}.Where(coords => coords.Item4 != CellDirection.None).ToArray()
							);
					}

					if (y > 0 && y < yways)
					{
						if (x > 0 && z > 0)
							cells[x - 1, y - 1, z - 1][CellDirection.UpForward] = new Cell(
								new int[]
								{
									vertexGrid[x - 1, y - 1, z - 1, 1],
									vertexGrid[x - 1, y, z - 1, 1],
									vertexGrid[x - 1, y, z, 0],
									vertexGrid[x, y, z, 0],
								},
								new (int, int, int, CellDirection)[]
								{
									(x - 1, y - 1, z - 1, CellDirection.UpLeft),
									(x - 1, y - 1, z - 1, CellDirection.UpRight),
									z < zways ? (x - 1, y - 1, z - 1, CellDirection.ForwardUp) : Cell.invalid,
									z < zways ? (x - 1, y, z - 1, CellDirection.ForwardDown) : Cell.invalid,
								}.Where(coords => coords.Item4 != CellDirection.None).ToArray()
							);

						if (x < xways && z > 0)
							cells[x, y - 1, z - 1][CellDirection.UpLeft] = new Cell(
								new int[]
								{
									vertexGrid[x, y - 1, z - 1, 1],
									vertexGrid[x, y, z - 1, 0],
									vertexGrid[x, y, z - 1, 1],
									vertexGrid[x, y, z, 0],
								},
								new (int, int, int, CellDirection)[]
								{
									(x, y - 1, z - 1, CellDirection.UpForward),
									(x, y - 1, z - 1, CellDirection.UpBack),
									x > 0 ? (x - 1, y - 1, z - 1, CellDirection.RightUp) : Cell.invalid,
									x > 0 ? (x - 1, y, z - 1, CellDirection.RightDown) : Cell.invalid,
								}.Where(coords => coords.Item4 != CellDirection.None).ToArray()
							);

						if (x > 0 && z < zways)
							cells[x - 1, y - 1, z][CellDirection.UpBack] = new Cell(
								new int[]
								{
									vertexGrid[x - 1, y - 1, z, 1],
									vertexGrid[x - 1, y, z, 1],
									vertexGrid[x - 1, y, z, 0],
									vertexGrid[x, y, z, 0],
								},
								new (int, int, int, CellDirection)[]
								{
									(x - 1, y - 1, z, CellDirection.UpLeft),
									(x - 1, y - 1, z, CellDirection.UpRight),
									z > 0 ? (x - 1, y - 1, z - 1, CellDirection.ForwardUp) : Cell.invalid,
									z > 0 ? (x - 1, y, z - 1, CellDirection.ForwardDown) : Cell.invalid,
								}.Where(coords => coords.Item4 != CellDirection.None).ToArray()
							);

						if (x > 0 && z > 0)
							cells[x - 1, y - 1, z - 1][CellDirection.UpRight] = new Cell(
								new int[]
								{
									vertexGrid[x - 1, y - 1, z - 1, 1],
									vertexGrid[x - 1, y, z - 1, 1],
									vertexGrid[x, y, z - 1, 0],
									vertexGrid[x, y, z, 0],
								},
								new (int, int, int, CellDirection)[]
								{
									(x - 1, y - 1, z - 1, CellDirection.UpForward),
									(x - 1, y - 1, z - 1, CellDirection.UpBack),
									x < xways ? (x - 1, y - 1, z - 1, CellDirection.RightUp) : Cell.invalid,
									x < xways ? (x - 1, y, z - 1, CellDirection.RightDown) : Cell.invalid,
								}.Where(coords => coords.Item4 != CellDirection.None).ToArray()
							);
					}
				}
			}
		}

		mesh.vertices = vertices.ToArray();
		mesh.subMeshCount = 2;
		mesh.SetIndices(edges.SelectMany(((int a, int b) edge) => new int[] { edge.a, edge.b }).ToArray(), MeshTopology.Lines, 0);
		mesh.SetIndices(edgesValidForRemoval.SelectMany(((int a, int b) edge) => new int[] { edge.a, edge.b }).ToArray(), MeshTopology.Lines, 1);

		foreach (Cell cell in from Dictionary<CellDirection, Cell> cellDict in cells
							  from entry in cellDict
							  select entry.Value)
			cell.ValidateMesh(mesh, cells);
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.blue;
		foreach (Cell cell in from Dictionary<CellDirection, Cell> cellDict in cells
							  from entry in cellDict
							  select entry.Value)
		{
			Gizmos.DrawWireSphere(cell.center, 0.05f);
			if (cell.neighbors != null)
				foreach (Cell neighbor in cell.neighbors)
					Gizmos.DrawRay(cell.center, (neighbor.center - cell.center) * 0.4f);
		}
	}
}

public class Cell
{
	public static readonly (int, int, int, CellDirection) invalid = (-1, -1, -1, CellDirection.None);

	public int[] vertices { get; private set; }
	//public (int, int)[] edges;
	private (int, int, int, CellDirection)[] neighborCellCoords;
	public Cell[] neighbors { get; private set; }
	public Vector3 center { get; private set; }

	public Cell(int[] vertices, (int, int, int, CellDirection)[] neighborCellCoords)
	{
		this.vertices = vertices;
		this.neighborCellCoords = neighborCellCoords;
		neighbors = new Cell[neighborCellCoords.Length];
		center = Vector3.zero;
	}

	public void ValidateMesh(Mesh mesh, Dictionary<CellDirection, Cell>[,,] cells)
	{
		int i = 0;
		foreach ((int x, int y, int z, CellDirection w) in neighborCellCoords)
			neighbors[i++] = cells[x, y, z][w];

		center = vertices.Select(vertex => mesh.vertices[vertex]).Aggregate((vertex1, vertex2) => vertex1 + vertex2) / vertices.Length;
	}
}

public enum CellDirection
{
	None,
	ForwardUp,
	ForwardLeft,
	ForwardDown,
	ForwardRight,
	RightUp,
	RightForward,
	RightDown,
	RightBack,
	UpForward,
	UpLeft,
	UpBack,
	UpRight,
}