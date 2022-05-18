using System.Linq;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "World Piece")]
public class WorldPieceDefinition : ScriptableObject
{
	public GameObject pieceMesh;
	public PieceMaterialID rightID;
	public PieceMaterialID leftID;
	public PieceMaterialID upID;
	public PieceMaterialID downID;
	public PieceMaterialID forwardID;
	public PieceMaterialID backID;
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
				definition = this,
				pieceMesh = Instantiate(pieceMesh, Vector3.zero, Quaternion.Euler(0, 0, 0)),
				rightID = rightID,
				leftID = leftID,
				upID = upID,
				downID = downID,
				forwardID = forwardID,
				backID = backID,
				weight = weight / numSymmetry,
			});
		}

		if (quarterRotationSymmetry)
		{
			WorldPiece.pieces.Add(new WorldPiece
			{
				definition = this,
				pieceMesh = Instantiate(pieceMesh, Vector3.zero, Quaternion.Euler(0, 90, 0)),
				rightID = forwardID,
				leftID = backID,
				upID = upID.RotatedRight,
				downID = downID.RotatedLeft,
				forwardID = leftID,
				backID = rightID,
				weight = weight / numSymmetry,
			});
		}

		if (halfRotationSymmetry)
		{
			WorldPiece.pieces.Add(new WorldPiece
			{
				definition = this,
				pieceMesh = Instantiate(pieceMesh, Vector3.zero, Quaternion.Euler(0, 180, 0)),
				rightID = leftID,
				leftID = rightID,
				upID = upID.RotatedHalf,
				downID = downID.RotatedHalf,
				forwardID = backID,
				backID = forwardID,
				weight = weight / numSymmetry,
			});
		}

		if (threeQuarterRotationSymmetry)
		{
			WorldPiece.pieces.Add(new WorldPiece
			{
				definition = this,
				pieceMesh = Instantiate(pieceMesh, Vector3.zero, Quaternion.Euler(0, 270, 0)),
				rightID = backID,
				leftID = forwardID,
				upID = upID.RotatedLeft,
				downID = downID.RotatedRight,
				forwardID = rightID,
				backID = leftID,
				weight = weight / numSymmetry,
			});
		}
	}
}

public enum PieceMaterial
{
	WATER = 0x3B5BE7,
	GROUND = 0x026100,
	AIR = 0xA6DBFF,
}

public class WorldPiece : IWeighted
{
	public WorldPieceDefinition definition;

	public static List<WorldPiece> pieces;

	public GameObject pieceMesh;
	public PieceMaterialID rightID;
	public PieceMaterialID leftID;
	public PieceMaterialID upID;
	public PieceMaterialID downID;
	public PieceMaterialID forwardID;
	public PieceMaterialID backID;
	public float weight;

	public float Weight => weight;
}
