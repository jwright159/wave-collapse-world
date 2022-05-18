using System;
using System.Linq;
using UnityEngine;

[Serializable]
public class PieceMaterialID
{
	[SerializeField]
	private int width;

	[SerializeField]
	private PieceMaterial[] id;

	public PieceMaterialID Complement
	{
		get
		{
			PieceMaterial[] newid = new PieceMaterial[width * width];
			for (int x = 0; x < width; x++)
				for (int y = 0; y < width; y++)
					newid[(width - x - 1) + y * width] = id[x + y * width];
			return new(width, newid);
		}
	}

	public PieceMaterialID RotatedRight
	{

		get
		{
			PieceMaterial[] newid = new PieceMaterial[width * width];
			for (int x = 0; x < width; x++)
				for (int y = 0; y < width; y++)
					newid[(width - y - 1) + x * width] = id[x + y * width];
			return new(width, newid);
		}
	}

	public PieceMaterialID RotatedHalf
	{

		get
		{
			PieceMaterial[] newid = new PieceMaterial[width * width];
			for (int x = 0; x < width; x++)
				for (int y = 0; y < width; y++)
					newid[(width - x - 1) + (width - y - 1) * width] = id[x + y * width];
			return new(width, newid);
		}
	}

	public PieceMaterialID RotatedLeft
	{

		get
		{
			PieceMaterial[] newid = new PieceMaterial[width * width];
			for (int x = 0; x < width; x++)
				for (int y = 0; y < width; y++)
					newid[y + (width - x - 1) * width] = id[x + y * width];
			return new(width, newid);
		}
	}

	public PieceMaterialID(int width, PieceMaterial[] id)
	{
		Debug.Assert(width * width == id.Length);
		this.width = width;
		this.id = id;
	}

	public void Resize(int width)
	{
		PieceMaterial[] id = new PieceMaterial[width * width];

		for (int x = 0; x < width; x++)
			for (int y = 0; y < width; y++)
				id[x + y * width] = this.width == 0 ? 0 : this.id[(int)((float)x / width * this.width) + (int)((float)y / width * this.width) * this.width];

		this.width = width;
		this.id = id;
	}

	public override bool Equals(object obj)
	{
		if (obj is null || GetType() != obj.GetType())
		{
			return false;
		}

		PieceMaterialID other = (PieceMaterialID)obj;
		return id.Cast<PieceMaterial>().SequenceEqual(other.id.Cast<PieceMaterial>());
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
