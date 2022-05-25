using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Space
{
	private readonly List<WorldPiece> possibilities;
	private readonly World world;
	private readonly int x, y, z;

	public Space Right => world[x + 1, y, z];
	public Space Left => world[x - 1, y, z];
	public Space Up => world[x, y + 1, z];
	public Space Down => world[x, y - 1, z];
	public Space Forward => world[x, y, z + 1];
	public Space Back => world[x, y, z - 1];

	public WorldPiece Piece => possibilities.Count == 1 ? possibilities[0] : null;
	public int Possibilities => possibilities.Count;
	
	public Space(World world, int x, int y, int z)
	{
		possibilities = //world.edgePiece != null && (x == 0 || x == world.width - 1 || y == 0 || y == world.height - 1 || z == 0 || z == world.depth - 1) ?
			//new List<WorldPiece>(new WorldPiece[] { world.edgePiece }) :
			new List<WorldPiece>(WorldPiece.pieces);
		this.world = world;
		this.x = x;
		this.y = y;
		this.z = z;

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
		RemovePiece(Up,			piece => piece.upID,		piece => piece.downID);
		RemovePiece(Down,		piece => piece.downID,		piece => piece.upID);
		RemovePiece(Forward,	piece => piece.forwardID,	piece => piece.backID);
		RemovePiece(Back,		piece => piece.backID,		piece => piece.forwardID);

		VerifyFinality();

		return possCount != possibilities.Count;
	}

	public void VerifyFinality()
	{
		if (Piece != null)
		{
			GameObject piece = UnityEngine.Object.Instantiate(Piece.gameObject, world.transform);
			piece.name = $"WorldPiece<{world.gameObject.name},{x},{y},{z}>";
			piece.transform.localPosition = new Vector3(x + 0.5f, y + 0.5f, z + 0.5f);
		}
	}
}
