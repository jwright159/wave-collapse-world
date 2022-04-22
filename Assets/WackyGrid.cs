using System.Linq;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class WackyGrid : MonoBehaviour
{
	private Mesh mesh;
	private Cell[] cells;

	private const int size = 5;
	private const int xways = size, yways = size, zways = size;

	private void OnValidate()
	{
		if (!mesh)
			mesh = GetComponent<MeshFilter>().mesh = new Mesh();
		else
			mesh.Clear();

		GenerateGrid(out Vector3[] vertices, out List<(int, int)> edges, out List<(int, int)> edgesValidForRemoval, out List<Cell> cells);
		while (edgesValidForRemoval.Count > 0)
			IterateEdgeRemoval(edges, edgesValidForRemoval, cells);
		this.cells = cells.ToArray();

		FinalizeMesh(mesh, vertices, edges, edgesValidForRemoval, this.cells);
	}

	private static void GenerateGrid(out Vector3[] vertices, out List<(int, int)> edges, out List<(int, int)> edgesValidForRemoval, out List<Cell> cells)
	{
		List<Vector3> vertexList = new List<Vector3>();
		edges = new List<(int, int)>();
		edgesValidForRemoval = new List<(int, int)>();

		int[,,,] vertexGrid = new int[xways + 1, yways + 1, zways + 1, 2];

		Dictionary<CellDirection, Cell>[,,] cellGrid = new Dictionary<CellDirection, Cell>[xways, yways, zways];
		for (int x = 0; x < xways; x++)
			for (int y = 0; y < yways; y++)
				for (int z = 0; z < zways; z++)
					cellGrid[x, y, z] = new Dictionary<CellDirection, Cell>();

		for (int z = 0; z <= zways; z++)
		{
			for (int y = 0; y <= yways; y++)
			{
				for (int x = 0; x <= xways; x++)
				{
					vertexList.Add(new Vector3(x, y, z));
					vertexGrid[x, y, z, 0] = vertexList.Count - 1;

					if (x < xways && y < yways && z < zways)
					{
						vertexList.Add(new Vector3(x + 0.5f, y + 0.5f, z + 0.5f));
						vertexGrid[x, y, z, 1] = vertexList.Count - 1;
					}

					if (x < xways && y < yways && z < zways)
					{
						(x > 0 && y > 0 && z > 0 ? edgesValidForRemoval : edges).Add((vertexGrid[x, y, z, 0], vertexGrid[x, y, z, 1]));
					}

					if (x > 0)
					{
						edges.Add((vertexGrid[x - 1, y, z, 0], vertexGrid[x, y, z, 0]));

						if (x < xways)
							edges.Add((vertexGrid[x - 1, y, z, 1], vertexGrid[x, y, z, 1]));

						if (y < yways && z < zways)
							(x < xways && y > 0 && z > 0 ? edgesValidForRemoval : edges).Add((vertexGrid[x - 1, y, z, 1], vertexGrid[x, y, z, 0]));
					}

					if (y > 0)
					{
						edges.Add((vertexGrid[x, y - 1, z, 0], vertexGrid[x, y, z, 0]));

						if (y < yways)
							edges.Add((vertexGrid[x, y - 1, z, 1], vertexGrid[x, y, z, 1]));
						
						if (x < xways && z < zways)
							(x > 0 && y < yways && z > 0 ? edgesValidForRemoval : edges).Add((vertexGrid[x, y - 1, z, 1], vertexGrid[x, y, z, 0]));
					}

					if (z > 0)
					{
						edges.Add((vertexGrid[x, y, z - 1, 0], vertexGrid[x, y, z, 0]));

						if (z < zways)
							edges.Add((vertexGrid[x, y, z - 1, 1], vertexGrid[x, y, z, 1]));

						if (x < xways && y < yways)
							(x > 0 && y > 0 && z < zways ? edgesValidForRemoval : edges).Add((vertexGrid[x, y, z - 1, 1], vertexGrid[x, y, z, 0]));
					}

					if (x > 0 && y > 0 && z < zways)
					{
						(x < xways && y < yways && z > 0 ? edgesValidForRemoval : edges).Add((vertexGrid[x - 1, y - 1, z, 1], vertexGrid[x, y, z, 0]));
					}

					if (x > 0 && y < yways && z > 0)
					{
						(x < xways && y > 0 && z < zways ? edgesValidForRemoval : edges).Add((vertexGrid[x - 1, y, z - 1, 1], vertexGrid[x, y, z, 0]));
					}

					if (x < xways && y > 0 && z > 0)
					{
						(x > 0 && y < yways && z < zways ? edgesValidForRemoval : edges).Add((vertexGrid[x, y - 1, z - 1, 1], vertexGrid[x, y, z, 0]));
					}

					if (x > 0 && y > 0 && z > 0)
					{
						(x < xways && y < yways && z < zways ? edgesValidForRemoval : edges).Add((vertexGrid[x - 1, y - 1, z - 1, 1], vertexGrid[x, y, z, 0]));
					}

					if (z > 0 && z < zways)
					{
						if (x > 0 && y > 0)
							cellGrid[x - 1, y - 1, z - 1][CellDirection.ForwardUp] = new Cell(
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
							cellGrid[x, y - 1, z - 1][CellDirection.ForwardLeft] = new Cell(
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
							cellGrid[x - 1, y, z - 1][CellDirection.ForwardDown] = new Cell(
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
							cellGrid[x - 1, y - 1, z - 1][CellDirection.ForwardRight] = new Cell(
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
							cellGrid[x - 1, y - 1, z - 1][CellDirection.RightUp] = new Cell(
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
							cellGrid[x - 1, y - 1, z - 1][CellDirection.RightForward] = new Cell(
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
							cellGrid[x - 1, y, z - 1][CellDirection.RightDown] = new Cell(
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
							cellGrid[x - 1, y - 1, z][CellDirection.RightBack] = new Cell(
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
							cellGrid[x - 1, y - 1, z - 1][CellDirection.UpForward] = new Cell(
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
							cellGrid[x, y - 1, z - 1][CellDirection.UpLeft] = new Cell(
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
							cellGrid[x - 1, y - 1, z][CellDirection.UpBack] = new Cell(
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
							cellGrid[x - 1, y - 1, z - 1][CellDirection.UpRight] = new Cell(
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

		vertices = vertexList.ToArray();

		cells = (from Dictionary<CellDirection, Cell> cellDict in cellGrid
				 from entry in cellDict
				 select entry.Value).ToList();

		foreach (Cell cell in cells)
			cell.FinalizeNeighbors(cellGrid);
	}

	private static void IterateEdgeRemoval(List<(int, int)> edges, List<(int, int)> edgesValidForRemoval, List<Cell> cells)
	{
		int edgeIndex = Random.Range(0, edgesValidForRemoval.Count);
		(int, int) edge = edgesValidForRemoval[edgeIndex];
		edgesValidForRemoval.RemoveAt(edgeIndex);

		Cell[] contents = (from cell in cells
						   where cell.vertices.Contains(edge.Item1) && cell.vertices.Contains(edge.Item2)
						   select cell).ToArray();

		int[] newVertices = (from cell in contents
							 from vertex in cell.vertices
							 select vertex).Distinct().ToArray();

		Cell[] newNeighbors = (from cell in contents
							   from neighbor in cell.neighbors
							   where !contents.Contains(neighbor)
							   select neighbor).ToArray();

		Cell newCell = new Cell(newVertices, newNeighbors);
		cells.Add(newCell);

		(int, int)[] invalidatedEdges = (from validEdge in edgesValidForRemoval
										 from vertex1 in newVertices
										 from vertex2 in newVertices
										 where validEdge == (Mathf.Min(vertex1, vertex2), Mathf.Max(vertex1, vertex2))
										 select validEdge).ToArray();

		foreach ((int, int) invalidatedEdge in invalidatedEdges)
		{
			edgesValidForRemoval.Remove(invalidatedEdge);
			edges.Add(invalidatedEdge);
		}

		foreach (Cell cell in contents)
			cells.Remove(cell);

		foreach (Cell newNeighbor in newNeighbors)
		{
			newNeighbor.neighbors.Add(newCell);
			newNeighbor.neighbors.RemoveAll(neighbor2 => contents.Contains(neighbor2));
		}
	}

	private static void FinalizeMesh(Mesh mesh, Vector3[] vertices, List<(int, int)> edges, List<(int, int)> edgesValidForRemoval, Cell[] cells)
	{
		mesh.vertices = vertices;
		mesh.subMeshCount = 2;
		mesh.SetIndices(edges.SelectMany(((int a, int b) edge) => new int[] { edge.a, edge.b }).ToArray(), MeshTopology.Lines, 0);
		mesh.SetIndices(edgesValidForRemoval.SelectMany(((int a, int b) edge) => new int[] { edge.a, edge.b }).ToArray(), MeshTopology.Lines, 1);

		foreach (Cell cell in cells)
			cell.FinalizeMesh(mesh);
	}

	private void OnDrawGizmos()
	{
		if (cells == null)
			return;

		foreach (Cell cell in cells)
		{
			Gizmos.color =
				cell.vertices.Length == 4 ? Color.blue :
				cell.vertices.Length == 8 ? Color.green :
				Color.red;
			Gizmos.DrawWireSphere(cell.center, 0.05f);
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
	public List<Cell> neighbors { get; private set; }
	public Vector3 center { get; private set; }

	public Cell(int[] vertices, (int, int, int, CellDirection)[] neighborCellCoords)
	{
		this.vertices = vertices;
		neighbors = new List<Cell>();
		this.neighborCellCoords = neighborCellCoords;
	}

	public Cell(int[] vertices, Cell[] neighbors)
	{
		this.vertices = vertices;
		this.neighbors = neighbors.ToList();
	}

	public void FinalizeNeighbors(Dictionary<CellDirection, Cell>[,,] cellGrid)
	{
		foreach ((int x, int y, int z, CellDirection w) in neighborCellCoords)
			neighbors.Add(cellGrid[x, y, z][w]);
		neighborCellCoords = null;
	}

	public void FinalizeMesh(Mesh mesh)
	{
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