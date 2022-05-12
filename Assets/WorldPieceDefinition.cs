using System.Linq;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "World Piece")]
public class WorldPieceDefinition : ScriptableObject
{
	public GameObject pieceMesh;
	public PieceMaterialID rightID;
	public PieceMaterialID leftID;
	public PieceMaterialID forwardID;
	public PieceMaterialID backID;
	public PieceMaterialID upID;
	public PieceMaterialID downID;
	public float weight = 1;

	public bool quarterRotationSymmetry;
	public bool halfRotationSymmetry;
	public bool threeQuarterRotationSymmetry;

	public void CreatePieces()
	{
		int numSymmetry = 1 +
			(quarterRotationSymmetry ? 1 : 0) +
			(halfRotationSymmetry ? 1 : 0) +
			(threeQuarterRotationSymmetry ? 1 : 0);

		{
			WorldPiece.pieces.Add(new WorldPiece
			{
				pieceMesh = Instantiate(pieceMesh, Vector3.zero, Quaternion.Euler(0, 0, 0)),
				rightID = rightID,
				leftID = leftID,
				forwardID = forwardID,
				backID = backID,
				upID = upID,
				downID = downID,
				weight = weight / numSymmetry,
			});
		}

		if (quarterRotationSymmetry)
		{
			WorldPiece.pieces.Add(new WorldPiece
			{
				pieceMesh = Instantiate(pieceMesh, Vector3.zero, Quaternion.Euler(0, 90, 0)),
				rightID = forwardID,
				leftID = backID,
				forwardID = leftID,
				backID = rightID,
				upID = upID,
				downID = downID,
				weight = weight / numSymmetry,
			});
		}

		if (halfRotationSymmetry)
		{
			WorldPiece.pieces.Add(new WorldPiece
			{
				pieceMesh = Instantiate(pieceMesh, Vector3.zero, Quaternion.Euler(0, 180, 0)),
				rightID = leftID,
				leftID = rightID,
				forwardID = backID,
				backID = forwardID,
				upID = upID,
				downID = downID,
				weight = weight / numSymmetry,
			});
		}

		if (threeQuarterRotationSymmetry)
		{
			WorldPiece.pieces.Add(new WorldPiece
			{
				pieceMesh = Instantiate(pieceMesh, Vector3.zero, Quaternion.Euler(0, -90, 0)),
				rightID = backID,
				leftID = forwardID,
				forwardID = rightID,
				backID = leftID,
				upID = upID,
				downID = downID,
				weight = weight / numSymmetry,
			});
		}
	}
}

public enum PieceMaterial
{
	WATER,
	GRASS,
	AIR,
}

public class WorldPiece : IWeighted
{
	public static List<WorldPiece> pieces;

	public GameObject pieceMesh;
	public PieceMaterialID rightID;
	public PieceMaterialID leftID;
	public PieceMaterialID forwardID;
	public PieceMaterialID backID;
	public PieceMaterialID upID;
	public PieceMaterialID downID;
	public float weight;

	public float Weight => weight;
}
