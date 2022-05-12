using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldPieceLoader : MonoBehaviour
{
	public WorldPieceDefinition[] pieces;

	private void Awake()
	{
		WorldPiece.pieces = new List<WorldPiece>();
		foreach (WorldPieceDefinition piece in pieces)
			piece.CreatePieces();
		Destroy(gameObject);
	}
}