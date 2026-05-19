using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class ElectricTilemap : MonoBehaviour
{
    public float DetectionRange = 1f;
    public TileBase electricTile;
    public TileBase offTile;

    private Tilemap tilemap;
    private GameObject big;
    private HashSet<Vector3Int> disabledTiles = new HashSet<Vector3Int>();

    private void Start()
    {
        tilemap = GetComponent<Tilemap>();
        big = GameObject.FindWithTag("Big");
    }

    private void Update()
    {
        Vector3Int bigTilePos = tilemap.WorldToCell(big.transform.position);
        int range = Mathf.CeilToInt(DetectionRange);

        // find tiles currently in range
        HashSet<Vector3Int> tilesInRange = new HashSet<Vector3Int>();
        for (int x = -range; x <= range; x++)
        {
            for (int y = -range; y <= range; y++)
            {
                Vector3Int checkPos = bigTilePos + new Vector3Int(x, y, 0);
                TileBase tile = tilemap.GetTile(checkPos);
                if (tile == null) continue;

                float distance = Vector2.Distance(
                    tilemap.CellToWorld(checkPos),
                    big.transform.position
                );

                if (distance <= DetectionRange)
                {
                    tilemap.SetTile(checkPos, offTile);
                    disabledTiles.Add(checkPos);
                    tilesInRange.Add(checkPos);
                }
            }
        }

        // restore tiles that are no longer in range
        List<Vector3Int> toRestore = new List<Vector3Int>();
        foreach (Vector3Int pos in disabledTiles)
        {
            if (!tilesInRange.Contains(pos))
                toRestore.Add(pos);
        }

        foreach (Vector3Int pos in toRestore)
        {
            tilemap.SetTile(pos, electricTile);
            disabledTiles.Remove(pos);
        }
    }
}