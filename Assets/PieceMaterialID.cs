using System;
using System.Linq;
using UnityEngine;

[Serializable]
public struct PieceMaterialID
{
	[SerializeField]
	private PieceMaterial[] id;

	public PieceMaterialID Complement => new(id.Reverse().ToArray());

	public static PieceMaterialID none = new(new PieceMaterial[0]);

	public PieceMaterialID(params PieceMaterial[] id)
	{
		this.id = id;
	}

	public override bool Equals(object obj)
	{
		if (obj is null || GetType() != obj.GetType())
		{
			return false;
		}

		PieceMaterialID other = (PieceMaterialID)obj;
		return id.SequenceEqual(other.id);
	}

	public override int GetHashCode()
	{
		return id.GetHashCode();
	}

	public static bool operator ==(PieceMaterialID obj1, PieceMaterialID obj2) => obj1.Equals(obj2);
	public static bool operator !=(PieceMaterialID obj1, PieceMaterialID obj2) => !(obj1 == obj2);

	public override string ToString()
	{
		return string.Join("|", id);
	}
}
