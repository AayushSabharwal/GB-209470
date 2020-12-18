using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class MapGenerator : SerializedMonoBehaviour
{
    [SerializeField]
    private GameObject tilemapPrefab;
    [SerializeField]
    private Dictionary<TileType, TileBase> tileTypeMap;
    [SerializeField]
    private Vector2Int worldDimensions;
    [SerializeField]
    public Vector2Int chunkSize;
    [SerializeField]
    private GenerationLayer[] generationLayers;

    private Tilemap[,] _tilemaps;

    public TilemapRenderer[,] Tilemaps { get; private set; }
    public Bounds[,] TilemapBounds { get; private set; }
    public Vector2Int ChunkCount { get; private set; }
    public Vector2Int ChunkSize => chunkSize;

    public TileType[] Map { get; private set; }
    public List<Vector2Int> Walkable { get; private set; }

    private void Awake() {
        ChunkCount = new Vector2Int(Mathf.CeilToInt(worldDimensions.x / (float) chunkSize.x),
                                    Mathf.CeilToInt(worldDimensions.y / (float) chunkSize.y));
        _tilemaps = new Tilemap[ChunkCount.x, ChunkCount.y];
        Tilemaps = new TilemapRenderer[ChunkCount.x, ChunkCount.y];
        TilemapBounds = new Bounds[ChunkCount.x, ChunkCount.y];
        for (int i = 0; i < ChunkCount.x; i++)
            for (int j = 0; j < ChunkCount.y; j++) {
                _tilemaps[i, j] = Instantiate(tilemapPrefab,
                                              Vector3.zero,
                                              Quaternion.identity,
                                              transform)
                    .GetComponent<Tilemap>();
                Tilemaps[i, j] = _tilemaps[i, j].GetComponent<TilemapRenderer>();
                TilemapBounds[i, j] = new Bounds(new Vector3((i + 0.5f) * ChunkSize.x, (j + 0.5f) * chunkSize.y, 0f),
                                                 new Vector3(chunkSize.x, chunkSize.y, 0f));
            }
    }

    private void Start() {
        GenerateTiles();
    }

    private IEnumerator AstarScan() {
        yield return new WaitForEndOfFrame();


        AstarPath.active.Scan(AstarPath.active.graphs);
    }

    [ButtonGroup("TileGroup")]
    private void GenerateTiles() {
        Map = new TileType[worldDimensions.x * worldDimensions.y];
        for (int i = 0; i < Map.Length; i++) Map[i] = TileType.Wall;

        foreach (GenerationLayer gl in generationLayers) {
            HandleFloorLayer(gl);
        }

        GenerateWalkableComponents(out List<List<Vector2Int>> walkableComponents);

        MergeWalkableComponents(in walkableComponents, out List<Vector2Int> allWalkable);

        Walkable = new List<Vector2Int>(allWalkable);
        for (int i = 0; i < walkableComponents.Count; i++)
            Walkable.AddRange(walkableComponents[i]);

        allWalkable.Clear();

        ReferenceManager.Inst.SharedDataManager.PlayerStartPosition = Walkable[Random.Range(0, Walkable.Count)];

        ClearTilemaps();

        for (int x = 0; x < worldDimensions.x; x++)
            for (int y = 0; y < worldDimensions.y; y++)
                if (x == 0 || x == worldDimensions.x - 1 || y == 0 || y == worldDimensions.y - 1)
                    _tilemaps[x / chunkSize.x, y / chunkSize.y]
                        .SetTile(new Vector3Int(x, y, 0), tileTypeMap[TileType.Wall]);
                else
                    _tilemaps[x / chunkSize.x, y / chunkSize.y]
                        .SetTile(new Vector3Int(x, y, 0), tileTypeMap[Map[PositionToIndex(x, y)]]);

        StartCoroutine(AstarScan());
    }

    [ButtonGroup("TileGroup")]
    private void ClearTilemaps() {
        for (int i = 0; i < ChunkCount.x; i++)
            for (int j = 0; j < ChunkCount.y; j++)
                _tilemaps[i, j].ClearAllTiles();
    }

    private void HandleFloorLayer(GenerationLayer gl) {
        for (int i = 0; i < gl.count; i++) {
            switch (gl.shape) {
                case GenerationShape.Circle: {
                    Vector2Int center = new Vector2Int(Random.Range(0, worldDimensions.x),
                                                       Random.Range(0, worldDimensions.y));
                    int radius = Random.Range(gl.radiusRange.x, gl.radiusRange.y);
                    CircleLayer(center, radius, TileType.Floor);
                    break;
                }

                case GenerationShape.Rectangle: {
                    Vector2Int llc = new Vector2Int(Random.Range(0, worldDimensions.x),
                                                    Random.Range(0, worldDimensions.y));
                    Vector2Int dimensions = new Vector2Int(Random.Range(gl.xRange.x, gl.xRange.y),
                                                           Random.Range(gl.yRange.x, gl.yRange.y));
                    RectLayer(llc, dimensions, TileType.Floor);
                    break;
                }

                case GenerationShape.Square: {
                    Vector2Int llc = new Vector2Int(Random.Range(0, worldDimensions.x),
                                                    Random.Range(0, worldDimensions.y));
                    int side = Random.Range(gl.sideRange.x, gl.sideRange.y);
                    SquareLayer(llc, side, TileType.Floor);
                    break;
                }

                default:
                    throw new
                        ArgumentException($"MapGenerator/GenerateTiles: GenerationShape {gl.shape} not implemented");
            }
        }
    }

    private void CircleLayer(Vector2Int center, int radius, TileType type) {
        for (int x = -radius; x <= radius; x++) {
            for (int y = -radius; y <= radius; y++) {
                if (center.x + x < 0 || center.x + x >= worldDimensions.x || center.y + y < 0 ||
                    center.y + y >= worldDimensions.y || x * x + y * y > radius * radius)
                    continue;
                Map[PositionToIndex(center.x + x, center.y + y)] = type;
            }
        }
    }

    private void RectLayer(Vector2Int llc, Vector2Int dimensions, TileType type) {
        for (int x = llc.x; x < llc.x + dimensions.x; x++) {
            for (int y = llc.y; y < llc.y + dimensions.y; y++) {
                if (x < 0 || x >= worldDimensions.x || y < 0 || y >= worldDimensions.y)
                    continue;
                Map[PositionToIndex(x, y)] = type;
            }
        }
    }

    private void SquareLayer(Vector2Int llc, int side, TileType type) {
        for (int x = llc.x; x < llc.x + side; x++) {
            for (int y = llc.y; y < llc.y + side; y++) {
                if (x < 0 || x >= worldDimensions.x || y < 0 || y >= worldDimensions.y)
                    continue;
                Map[PositionToIndex(x, y)] = type;
            }
        }
    }

    private void MergeWalkableComponents(in List<List<Vector2Int>> walkableComponents,
                                         out List<Vector2Int> extraWalkable) {
        extraWalkable = new List<Vector2Int>();
        for (int i = 0; i < walkableComponents.Count - 1; i++) {
            Vector2Int posA = walkableComponents[i][Random.Range(0, walkableComponents[i].Count)];
            Vector2Int posB = walkableComponents[i + 1][Random.Range(0, walkableComponents[i + 1].Count)];

            if (posA.x == posB.x) {
                int yInc = (posB.y - posA.y) / Mathf.Abs(posB.y - posA.y);
                for (int y = posA.y; y != posB.y; y += yInc) {
                    if (Map[PositionToIndex(posA.x, y)] != TileType.Wall) continue;

                    Map[PositionToIndex(posA.x, y)] = TileType.Floor;
                    extraWalkable.Add(new Vector2Int(posA.x, y));
                }
            }
            else if (posA.y == posB.y) {
                int xInc = (posB.x - posA.x) / Mathf.Abs(posB.x - posA.x);
                for (int x = posA.x; x != posB.x; x += xInc) {
                    if (Map[PositionToIndex(x, posA.y)] != TileType.Wall) continue;

                    Map[PositionToIndex(x, posA.y)] = TileType.Floor;
                    extraWalkable.Add(new Vector2Int(x, posA.y));
                }
            }
            else {
                int xInc = (posB.x - posA.x) / Mathf.Abs(posB.x - posA.x);
                int yInc = (posB.y - posA.y) / Mathf.Abs(posB.y - posA.y);

                for (int x = posA.x; x != posB.x; x += xInc)
                    for (int y = posA.y; y != posB.y; y += yInc) {
                        if (Map[PositionToIndex(x, y)] != TileType.Wall) continue;

                        Map[PositionToIndex(x, y)] = TileType.Floor;
                        extraWalkable.Add(new Vector2Int(x, y));
                    }
            }
        }
    }

    private void GenerateWalkableComponents(out List<List<Vector2Int>> walkableComponents) {
        walkableComponents = new List<List<Vector2Int>>();

        bool[] vis = new bool[worldDimensions.x * worldDimensions.y];
        for (int i = 0; i < vis.Length; i++) vis[i] = false;
        for (int i = 0; i < Map.Length; i++) {
            if (Map[i] != TileType.Floor || vis[i]) continue;

            walkableComponents.Add(FindComponent(ref vis, i));
        }
    }

    private List<Vector2Int> FindComponent(ref bool[] vis, int start) {
        Queue<int> queue = new Queue<int>();
        List<Vector2Int> ret = new List<Vector2Int>();

        queue.Enqueue(start);
        vis[start] = true;
        while (queue.Count > 0) {
            int cur = queue.Dequeue();
            Vector2Int cpos = IndexToPosition(cur);
            ret.Add(cpos);
            (bool valid, int ind)[] neighbors =
            {
                (cpos.x > 1, PositionToIndex(cpos.x - 1, cpos.y)),
                (cpos.y > 1, PositionToIndex(cpos.x, cpos.y - 1)),
                (cpos.x < worldDimensions.x - 1, PositionToIndex(cpos.x + 1, cpos.y)),
                (cpos.y < worldDimensions.y - 1, PositionToIndex(cpos.x, cpos.y + 1))
            };
            for (int i = 0; i < 4; i++) {
                if (!neighbors[i].valid || vis[neighbors[i].ind] ||
                    Map[neighbors[i].ind] != TileType.Floor) continue;
                vis[neighbors[i].ind] = true;
                queue.Enqueue(neighbors[i].ind);
            }
        }

        return ret;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int PositionToIndex(int x, int y) {
        return x * worldDimensions.y + y;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector2Int IndexToPosition(int ind) {
        return new Vector2Int(ind / worldDimensions.y, ind % worldDimensions.y);
    }
}

[Serializable]
public class GenerationLayer
{
    public GenerationShape shape;
    [ShowIf("shape", Value = GenerationShape.Square),
     ValidateInput("@sideRange.x<=sideRange.y && sideRange.x > 0 && sideRange.y > 0")]
    public Vector2Int sideRange;
    [ShowIf("shape", Value = GenerationShape.Rectangle),
     ValidateInput("@xRange.x<=xRange.y && xRange.x > 0 && xRange.y > 0")]
    public Vector2Int xRange;
    [ShowIf("shape", Value = GenerationShape.Rectangle),
     ValidateInput("@yRange.x<=yRange.y && yRange.x > 0 && yRange.y > 0")]
    public Vector2Int yRange;

    [ShowIf("shape", Value = GenerationShape.Circle),
     ValidateInput("@radiusRange.x<=radiusRange.y && radiusRange.x > 0 && radiusRange.y > 0")]
    public Vector2Int radiusRange;

    [ValidateInput("@count >= 0")]
    public int count;
}

public enum GenerationShape
{
    Square,
    Rectangle,
    Circle
}

public enum TileType
{
    Floor,
    Wall
}