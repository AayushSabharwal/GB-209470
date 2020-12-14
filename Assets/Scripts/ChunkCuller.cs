using UnityEngine;

public class ChunkCuller : MonoBehaviour
{
    private Camera _cam;

    private void Start() {
        _cam = GetComponent<Camera>();
    }

    private void LateUpdate() {
        for (int i = 0; i < MapGenerator.Inst.ChunkCount.x; i++) {
            for (int j = 0; j < MapGenerator.Inst.ChunkCount.y; j++) {
                Vector3 closestPoint = MapGenerator.Inst.TilemapBounds[i, j].ClosestPoint(transform.position);
                if (Mathf.Abs(closestPoint.x - transform.position.x) < _cam.orthographicSize * _cam.aspect &&
                    Mathf.Abs(closestPoint.y - transform.position.y) < _cam.orthographicSize)
                    MapGenerator.Inst.Tilemaps[i, j].enabled = true;
                else
                    MapGenerator.Inst.Tilemaps[i, j].enabled = false;
            }
        }
    }
}