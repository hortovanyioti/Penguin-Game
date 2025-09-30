/// <summary>
/// Do not change values!
/// </summary>
public struct LOD
{
	public const int CHUNK_SIZE = EndlessTerrain.CHUNK_SIZE;
	public const int LOD_LEVELS = 20;

	/// <summary>
	/// Increase loop variable by this value for a given LOD level. Divisors of chunkSize-1.
	/// </summary>
	public static readonly int[] MeshScale =
		new int[LOD_LEVELS] { 1, 2, 3, 4, 5, 6, 8, 10, 12, 15, 16, 20, 24, 30, 40, 48, 60, 80, 120, 240 };
}
