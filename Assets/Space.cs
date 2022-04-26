using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Space
{
	private List<WorldPiece> possibilities;

	public WorldPiece Piece => possibilities.Count == 1 ? possibilities[0] : null;
	
	public Space()
	{
		possibilities = new List<WorldPiece>(WorldPiece.pieces);
	}

	public void CollapseFully()
	{
		WorldPiece chosenPiece = WeightSelector.RandomWeighted(possibilities);
		possibilities.RemoveAll(piece => piece != chosenPiece);
	}

	/// <returns>True if the possibilities changed</returns>
	public bool Collapse(Space forward, Space right, Space back, Space left)
	{

	}
}
