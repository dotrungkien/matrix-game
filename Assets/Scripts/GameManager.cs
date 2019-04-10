using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;

public class GameManager : Singleton<GameManager>
{
    [HideInInspector]
    public Color myColor;
    [HideInInspector]
    public bool isWatching = false;

    public Tile tilePrefab;

    public Tile SpawnNewTile(int[] piece, Vector3 tileSpawnPos, Transform parent, bool clearChild = false)
    {
        if (clearChild)
        {
            foreach (Transform child in parent.transform)
            {
                if (child != null) GameObject.Destroy(child.gameObject);
            }
        }

        Tile tile = Instantiate(tilePrefab, tileSpawnPos, Quaternion.identity, parent);
        SpriteRenderer render = tile.GetComponent<SpriteRenderer>();
        tile.SetVal(piece);
        if (!isWatching) tile.SetColor(myColor);
        return tile;
    }
}
