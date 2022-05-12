using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Space
{
	private readonly List<WorldPiece> possibilities;
	private readonly World world;
	private readonly int x, y;

	public Space Right => world[x + 1, y];
	public Space Left => world[x - 1, y];
	public Space Forward => world[x, y + 1];
	public Space Back => world[x, y - 1];
	public Space Up => null;
	public Space Down => null;

	public WorldPiece Piece => possibilities.Count == 1 ? possibilities[0] : null;
	public int Possibilities => possibilities.Count;
	
	public Space(World world, int x, int y)
	{
		possibilities = new List<WorldPiece>(WorldPiece.pieces);
		this.world = world;
		this.x = x;
		this.y = y;

		VerifyFinality();
	}

	public void CollapseFully()
	{
		WorldPiece chosenPiece = WeightSelector.RandomWeighted(possibilities);
		possibilities.Clear();
		possibilities.Add(chosenPiece);

		VerifyFinality();
	}

	/// <returns>True if the possibilities changed</returns>
	public bool Collapse()
	{
		int possCount = possibilities.Count;

		void RemovePiece(Space space, Func<WorldPiece, PieceMaterialID> forwardID, Func<WorldPiece, PieceMaterialID> backID)
		{
			if (space == null) return;
			List<PieceMaterialID> ids = (from piece in space.possibilities select backID(piece).Complement).ToList();
			possibilities.RemoveAll(piece => !ids.Contains(forwardID(piece)));
		}

		RemovePiece(Right,		piece => piece.rightID,		piece => piece.leftID);
		RemovePiece(Left,		piece => piece.leftID,		piece => piece.rightID);
		RemovePiece(Forward,	piece => piece.forwardID,	piece => piece.backID);
		RemovePiece(Back,		piece => piece.backID,		piece => piece.forwardID);
		RemovePiece(Up,			piece => piece.upID,		piece => piece.downID);
		RemovePiece(Down,		piece => piece.downID,		piece => piece.upID);

		VerifyFinality();

		return possCount != possibilities.Count;
	}

	public void VerifyFinality()
	{
		if (Piece != null)
		{
			GameObject piece = UnityEngine.Object.Instantiate(Piece.pieceMesh);
			piece.name = $"WorldPiece<{world.gameObject.name},{x},{y}>";
			piece.transform.position = new Vector3(world.transform.position.x + x + 0.5f, world.transform.position.y, world.transform.position.z + y + 0.5f);
		}
	}
}
