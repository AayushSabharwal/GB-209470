using UnityEngine;

public class ChunkCuller : MonoBehaviour
{
    private Camera _cam;
    private MapGenerator _mapGenerator;
    
    private void Start() {
        _cam = GetComponent<Camera>();
        _mapGenerator = ReferenceManager.Inst.MapGenerator;
    }

    // private void LateUpdate() {
    //     for (int i = 0; i < _mapGenerator.ChunkCount.x; i++) {
    //         for (int j = 0; j < _mapGenerator.ChunkCount.y; j++) {
    //             Vector3 closestPoint = _mapGenerator.TilemapBounds[i, j].ClosestPoint(transform.position);
    //             if (Mathf.Abs(closestPoint.x - transform.position.x) < _cam.orthographicSize * _cam.aspect &&
    //                 Mathf.Abs(closestPoint.y - transform.position.y) < _cam.orthographicSize)
    //                 _mapGenerator.Tilemaps[i, j].enabled = true;
    //             else
    //                 _mapGenerator.Tilemaps[i, j].enabled = false;
    //         }
    //     }
    // }
}