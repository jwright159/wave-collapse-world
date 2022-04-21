using System.Linq;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class WackyGrid : MonoBehaviour
{
	private Mesh mesh;

	private int xways = 20, yways = 20, zways = 20;

	private void OnValidate()
	{
		if (mesh)
			DestroyImmediate(mesh);
		mesh = new Mesh();
		GetComponent<MeshFilter>().mesh = mesh;
		GenerateRhombuses();
	}

	private void GenerateRhombuses()
	{
		List<Vector3> vertices = new List<Vector3>();
		List<(int, int)> edges = new List<(int, int)>();
		List<(int, int)> edgesValidForRemoval = new List<(int, int)>();

		for (int z = 0; z < zways; z++)
		{
			for (int y = 0; y < yways; y++)
			{
				for (int x = 0; x < xways; x++)
				{
					vertices.Add(new Vector3(x, y, z));
					vertices.Add(new Vector3(x + 0.5f, y + 0.5f, z + 0.5f));

					int thisCorner = vertices.Count - 2;
					int thisCenter = vertices.Count - 1;

					bool isOnNearWall = x == 0 || y == 0 || z == 0;
					bool isOnFarWall = x == xways - 1 || y == yways - 1 || z == zways - 1;
					bool isOnWall = isOnNearWall || isOnFarWall;

					if (((x > 0 && y > 0 && z > 0) || (!(x > 0 ^ y > 0 ^ z > 0) && (x > 0 || y > 0 || z > 0))) &&
						!((x == xways - 1 && y == yways - 1) || (x == xways - 1 && z == zways - 1) || (y == yways - 1 && z == zways - 1)))
						(isOnWall ? edges : edgesValidForRemoval).Add((thisCorner, thisCenter));

					if (x > 0)
					{
						int leftCorner = thisCorner - (1) * 2;
						int leftCenter = thisCenter - (1) * 2;

						//edges.Add((leftCorner, thisCorner));
						//edges.Add((leftCenter, thisCenter));
						if ((y > 0 || z > 0) && (y < yways - 1 || z < zways - 1))
							(x == 0 || y == 0 || z == 0 || y == yways - 1 || z == zways - 1 ? edges : edgesValidForRemoval).Add((leftCenter, thisCorner));
					}

					if (y > 0)
					{
						int downCorner = thisCorner - (xways) * 2;
						int downCenter = thisCenter - (xways) * 2;

						//edges.Add((downCorner, thisCorner));
						//edges.Add((downCenter, thisCenter));
						if ((x > 0 || z > 0) && (x < xways - 1 || z < zways - 1))
							(x == 0 || y == 0 || z == 0 || x == xways - 1 || z == zways - 1 ? edges : edgesValidForRemoval).Add((downCenter, thisCorner));
					}

					if (z > 0)
					{
						int backCorner = thisCorner - (xways * yways) * 2;
						int backCenter = thisCenter - (xways * yways) * 2;

						//edges.Add((backCorner, thisCorner));
						//edges.Add((backCenter, thisCenter));
						if ((x > 0 || y > 0) && (x < xways - 1 || y < yways - 1))
							(x == 0 || y == 0 || z == 0 || x == xways - 1 || y == yways - 1 ? edges : edgesValidForRemoval).Add((backCenter, thisCorner));
					}

					if (x > 0 && y > 0)
					{
						int downLeftCorner = thisCorner - (xways + 1) * 2;
						int downLeftCenter = thisCenter - (xways + 1) * 2;

						(z == 0 || z == zways - 1 ? edges : edgesValidForRemoval).Add((downLeftCenter, thisCorner));
					}

					if (x > 0 && z > 0)
					{
						int backLeftCorner = thisCorner - (xways * yways + 1) * 2;
						int backLeftCenter = thisCenter - (xways * yways + 1) * 2;

						(y == 0 || y == yways - 1 ? edges : edgesValidForRemoval).Add((backLeftCenter, thisCorner));
					}

					if (y > 0 && z > 0)
					{
						int backDownCorner = thisCorner - (xways * yways + xways) * 2;
						int backDownCenter = thisCenter - (xways * yways + xways) * 2;

						(x == 0 || x == xways - 1 ? edges : edgesValidForRemoval).Add((backDownCenter, thisCorner));
					}

					if (x > 0 && y > 0 && z > 0)
					{
						int backDownLeftCorner = thisCorner - (xways * yways + xways + 1) * 2;
						int backDownLeftCenter = thisCenter - (xways * yways + xways + 1) * 2;

						edgesValidForRemoval.Add((backDownLeftCenter, thisCorner));
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
