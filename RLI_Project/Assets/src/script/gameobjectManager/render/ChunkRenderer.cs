using System.Collections;
using System.Collections.Generic;
using Unity.Loading;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ChunkRenderer : MonoBehaviour
{
    public Vector3Int Pivot;
    public Vector2Int ChunkSize;
    public ContentsInfo InteractionTileInfo;
    public ContentsInfo BackgroundTileInfo;

    public List<GameObject> InteractionTilePrefap;
    public List<GameObject> BackgroundTilePrefap;

    public Tilemap InteractionTilemap;
    public Tilemap BackgroundTilemap;
    public TileBase tileToPlace;
    TilemapCollider2D Colider;

    List<Tilemap> tilemaps = new List<Tilemap>();

    private void Awake()
    {
        // 수정 필요
        tilemaps.Add(InteractionTilemap);
        tilemaps.Add(BackgroundTilemap);
        tilemaps.ForEach((tm) => { if (!tm.TryGetComponent<TilemapCollider2D>(out Colider)) Debug.LogError($" {tm.ToString()}의 타일맵 적용 실패"); });
            
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}