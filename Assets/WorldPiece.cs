using System.Collections.Generic;
using UnityEngine;

public class WorldPiece : MonoBehaviour, IWeighted
{
	public static List<WorldPiece> pieces;

	public WorldPieceDefinition definition;

	public PieceMaterialID rightID;
	public PieceMaterialID leftID;
	public PieceMaterialID upID;
	public PieceMaterialID downID;
	public PieceMaterialID forwardID;
	public PieceMaterialID backID;
	public float weight;

	public float Weight => weight;
}
