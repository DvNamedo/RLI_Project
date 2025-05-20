using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static UnityEditor.PlayerSettings;

public class setTile : MonoBehaviour
{
    public Tilemap tilemap;
    public TileBase tileToPlace;
    Vector3Int tilePos = new(0, 0, 0);
    TilemapCollider2D Colider;

    // Start is called before the first frame update

    private void Awake()
    {
        if( !tilemap.TryGetComponent<TilemapCollider2D>(out Colider))
        {
            Debug.LogError("타일맵 적용 실패");
        }
    }

    void Start()
    {

        tilemap.SetTile(tilePos, tileToPlace);

        tilemap.SetTileFlags(tilePos, TileFlags.None);

        tilemap.SetColor(tilePos, Color.red);
        tilemap.RefreshTile(tilePos);


    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            tilemap.SetTile(tilePos, tileToPlace);
            tilemap.SetColor(tilePos, Color.red);
            tilemap.RefreshTile(tilePos);
            //Colider.ProcessTilemapChanges();
        }   

        if (Input.GetKeyUp(KeyCode.P)) 
        {
            tilemap.SetTile(tilePos, null);
        }
    }
}
