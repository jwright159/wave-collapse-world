using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WorldPieceDefinition : MonoBehaviour
{
	private static int pieceTypes;

	public PieceMaterialID rightID;
	public PieceMaterialID leftID;
	public PieceMaterialID upID;
	public PieceMaterialID downID;
	public PieceMaterialID forwardID;
	public PieceMaterialID backID;
	public float weight = 1;

	public bool verticalRotationSymmetry;
	public bool horizontalRotationSymmetry;

	public void CreatePieces()
	{
		if (WorldPiece.pieces.Count == 0)
			pieceTypes = 0;

		int numSymmetry = 1 *
			(verticalRotationSymmetry ? 4 : 1) *
			(horizontalRotationSymmetry ? 4 : 1);
		float weight = this.weight / numSymmetry;

		int i = 0;
		foreach (Rotator horizontalRotator in horizontalRotationSymmetry ? new Rotator[] { DontRotate, RotateHorizontallyRight, RotateHorizontallyRight2, RotateHorizontallyHalf, RotateHorizontallyLeft, RotateHorizontallyLeft2 } : new Rotator[] { DontRotate })
		{
			foreach (Rotator verticalRotator in verticalRotationSymmetry ? new Rotator[] { DontRotate, RotateVerticallyRight, RotateVerticallyHalf, RotateVerticallyLeft } : new Rotator[] { DontRotate })
			{
				Quaternion rotation = Quaternion.identity;
				PieceMaterialID rightID = this.rightID;
				PieceMaterialID leftID = this.leftID;
				PieceMaterialID upID = this.upID;
				PieceMaterialID downID = this.downID;
				PieceMaterialID forwardID = this.forwardID;
				PieceMaterialID backID = this.backID;

				verticalRotator(ref rotation, ref rightID, ref leftID, ref upID, ref downID, ref forwardID, ref backID);
				horizontalRotator(ref rotation, ref rightID, ref leftID, ref upID, ref downID, ref forwardID, ref backID);

				WorldPiece piece = InstantiateWorldPiece(new Vector3(pieceTypes, 0, i++), rotation);
				piece.definition = this;
				piece.rightID = rightID;
				piece.leftID = leftID;
				piece.upID = upID;
				piece.downID = downID;
				piece.forwardID = forwardID;
				piece.backID = backID;
				piece.weight = weight;
				WorldPiece.pieces.Add(piece);
			}
		}

		pieceTypes++;
	}

	public WorldPiece InstantiateWorldPiece(Vector3 position, Quaternion rotation)
	{
		GameObject obj = Instantiate(gameObject, position, rotation);
		Destroy(obj.GetComponent<WorldPieceDefinition>());
		return obj.AddComponent<WorldPiece>();
	}

	public delegate void Rotator(ref Quaternion rotation,
		ref PieceMaterialID rightID, ref PieceMaterialID leftID,
		ref PieceMaterialID upID, ref PieceMaterialID downID,
		ref PieceMaterialID forwardID, ref PieceMaterialID backID);

	private void DontRotate(ref Quaternion rotation,
		ref PieceMaterialID rightID, ref PieceMaterialID leftID,
		ref PieceMaterialID upID, ref PieceMaterialID downID,
		ref PieceMaterialID forwardID, ref PieceMaterialID backID)
	{}

	private void RotateHorizontallyRight(ref Quaternion rotation,
		ref PieceMaterialID rightID, ref PieceMaterialID leftID,
		ref PieceMaterialID upID, ref PieceMaterialID downID,
		ref PieceMaterialID forwardID, ref PieceMaterialID backID)
	{
		rotation = Quaternion.Euler(90, 0, 0) * rotation;

		PieceMaterialID newRightID = rightID.RotatedRight;
		PieceMaterialID newLeftID = leftID.RotatedLeft;
		PieceMaterialID newUpID = backID;
		PieceMaterialID newDownID = forwardID;
		PieceMaterialID newForwardID = upID.RotatedHalf;
		PieceMaterialID newBackID = downID.RotatedHalf;

		rightID = newRightID;
		leftID = newLeftID;
		upID = newUpID;
		downID = newDownID;
		forwardID = newForwardID;
		backID = newBackID;
	}

	private void RotateHorizontallyRight2(ref Quaternion rotation,
		ref PieceMaterialID rightID, ref PieceMaterialID leftID,
		ref PieceMaterialID upID, ref PieceMaterialID downID,
		ref PieceMaterialID forwardID, ref PieceMaterialID backID)
	{
		rotation = Quaternion.Euler(0, 0, 90) * rotation;

		PieceMaterialID newRightID = downID.RotatedRight;
		PieceMaterialID newLeftID = upID.RotatedLeft;
		PieceMaterialID newUpID = rightID.RotatedLeft;
		PieceMaterialID newDownID = leftID.RotatedRight;
		PieceMaterialID newForwardID = forwardID.RotatedRight;
		PieceMaterialID newBackID = backID.RotatedLeft;

		rightID = newRightID;
		leftID = newLeftID;
		upID = newUpID;
		downID = newDownID;
		forwardID = newForwardID;
		backID = newBackID;
	}

	private void RotateHorizontallyHalf(ref Quaternion rotation,
		ref PieceMaterialID rightID, ref PieceMaterialID leftID,
		ref PieceMaterialID upID, ref PieceMaterialID downID,
		ref PieceMaterialID forwardID, ref PieceMaterialID backID)
	{
		rotation = Quaternion.Euler(180, 0, 0) * rotation;

		PieceMaterialID newRightID = rightID.RotatedHalf;
		PieceMaterialID newLeftID = leftID.RotatedHalf;
		PieceMaterialID newUpID = downID.RotatedHalf;
		PieceMaterialID newDownID = upID.RotatedHalf;
		PieceMaterialID newForwardID = backID.RotatedHalf;
		PieceMaterialID newBackID = forwardID.RotatedHalf;

		rightID = newRightID;
		leftID = newLeftID;
		upID = newUpID;
		downID = newDownID;
		forwardID = newForwardID;
		backID = newBackID;
	}

	private void RotateHorizontallyLeft(ref Quaternion rotation,
		ref PieceMaterialID rightID, ref PieceMaterialID leftID,
		ref PieceMaterialID upID, ref PieceMaterialID downID,
		ref PieceMaterialID forwardID, ref PieceMaterialID backID)
	{
		rotation = Quaternion.Euler(-90, 0, 0) * rotation;

		PieceMaterialID newRightID = rightID.RotatedLeft;
		PieceMaterialID newLeftID = leftID.RotatedRight;
		PieceMaterialID newUpID = forwardID.RotatedHalf;
		PieceMaterialID newDownID = backID.RotatedHalf;
		PieceMaterialID newForwardID = downID;
		PieceMaterialID newBackID = upID;

		rightID = newRightID;
		leftID = newLeftID;
		upID = newUpID;
		downID = newDownID;
		forwardID = newForwardID;
		backID = newBackID;
	}

	private void RotateHorizontallyLeft2(ref Quaternion rotation,
		ref PieceMaterialID rightID, ref PieceMaterialID leftID,
		ref PieceMaterialID upID, ref PieceMaterialID downID,
		ref PieceMaterialID forwardID, ref PieceMaterialID backID)
	{
		rotation = Quaternion.Euler(0, 0, -90) * rotation;

		PieceMaterialID newRightID = upID.RotatedRight;
		PieceMaterialID newLeftID = downID.RotatedLeft;
		PieceMaterialID newUpID = leftID.RotatedRight;
		PieceMaterialID newDownID = rightID.RotatedLeft;
		PieceMaterialID newForwardID = forwardID.RotatedLeft;
		PieceMaterialID newBackID = backID.RotatedRight;

		rightID = newRightID;
		leftID = newLeftID;
		upID = newUpID;
		downID = newDownID;
		forwardID = newForwardID;
		backID = newBackID;
	}

	private void RotateVerticallyRight(ref Quaternion rotation,
		ref PieceMaterialID rightID, ref PieceMaterialID leftID,
		ref PieceMaterialID upID, ref PieceMaterialID downID,
		ref PieceMaterialID forwardID, ref PieceMaterialID backID)
	{
		rotation = Quaternion.Euler(0, 90, 0) * rotation;

		PieceMaterialID newRightID = forwardID;
		PieceMaterialID newLeftID = backID;
		PieceMaterialID newUpID = upID.RotatedRight;
		PieceMaterialID newDownID = downID.RotatedLeft;
		PieceMaterialID newForwardID = leftID;
		PieceMaterialID newBackID = rightID;

		rightID = newRightID;
		leftID = newLeftID;
		upID = newUpID;
		downID = newDownID;
		forwardID = newForwardID;
		backID = newBackID;
	}

	private void RotateVerticallyHalf(ref Quaternion rotation,
		ref PieceMaterialID rightID, ref PieceMaterialID leftID,
		ref PieceMaterialID upID, ref PieceMaterialID downID,
		ref PieceMaterialID forwardID, ref PieceMaterialID backID)
	{
		rotation = Quaternion.Euler(0, 180, 0) * rotation;

		PieceMaterialID newRightID = leftID;
		PieceMaterialID newLeftID = rightID;
		PieceMaterialID newUpID = upID.RotatedHalf;
		PieceMaterialID newDownID = downID.RotatedHalf;
		PieceMaterialID newForwardID = backID;
		PieceMaterialID newBackID = forwardID;

		rightID = newRightID;
		leftID = newLeftID;
		upID = newUpID;
		downID = newDownID;
		forwardID = newForwardID;
		backID = newBackID;
	}

	private void RotateVerticallyLeft(ref Quaternion rotation,
		ref PieceMaterialID rightID, ref PieceMaterialID leftID,
		ref PieceMaterialID upID, ref PieceMaterialID downID,
		ref PieceMaterialID forwardID, ref PieceMaterialID backID)
	{
		rotation = Quaternion.Euler(0, -90, 0) * rotation;

		PieceMaterialID newRightID = backID;
		PieceMaterialID newLeftID = forwardID;
		PieceMaterialID newUpID = upID.RotatedLeft;
		PieceMaterialID newDownID = downID.RotatedRight;
		PieceMaterialID newForwardID = rightID;
		PieceMaterialID newBackID = leftID;

		rightID = newRightID;
		leftID = newLeftID;
		upID = newUpID;
		downID = newDownID;
		forwardID = newForwardID;
		backID = newBackID;
	}
}

public enum PieceMaterial
{
	WATER = 0x3B5BE7,
	GROUND = 0x026100,
	AIR = 0xA6DBFF,
}
