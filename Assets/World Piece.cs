using System.Linq;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "World Piece")]
public class WorldPiece : ScriptableObject
{
	public static List<WorldPiece> pieces = new List<WorldPiece>();

	public MeshRenderer piece;
	public PieceMaterial[] northID;
	public PieceMaterial[] eastID;
	public PieceMaterial[] southID;
	public PieceMaterial[] westID;
	public PieceMaterial[] upID;
	public PieceMaterial[] downID;

	public bool quarterRotationSymmetry;
	public bool halfRotationSymmetry;
	public bool threeQuarterRotationSymmetry;

	private bool awoken;

	private void Awake()
	{
		pieces.Add(this);

		if (!awoken)
		{
			if (quarterRotationSymmetry)
			{
				WorldPiece newPiece = Instantiate(this);
				newPiece.Singlify();
				newPiece.piece.transform.Rotate(0, 90, 0);
				newPiece.northID = westID;
				newPiece.eastID = northID;
				newPiece.southID = eastID;
				newPiece.westID = southID;
			}

			if (halfRotationSymmetry)
			{
				WorldPiece newPiece = Instantiate(this);
				newPiece.Singlify();
				newPiece.piece.transform.Rotate(0, 180, 0);
				newPiece.northID = southID;
				newPiece.eastID = westID;
				newPiece.southID = northID;
				newPiece.westID = eastID;
			}

			if (threeQuarterRotationSymmetry)
			{
				WorldPiece newPiece = Instantiate(this);
				newPiece.Singlify();
				newPiece.piece.transform.Rotate(0, 270, 0);
				newPiece.northID = eastID;
				newPiece.eastID = southID;
				newPiece.southID = westID;
				newPiece.westID = northID;
			}

			Singlify();
		}
	}

	public void Singlify()
	{
		piece = Instantiate(piece);
		awoken = true;
		quarterRotationSymmetry = false;
		halfRotationSymmetry = false;
		threeQuarterRotationSymmetry = false;
	}
}

public enum PieceMaterial
{
	WATER,
	GRASS,
}
