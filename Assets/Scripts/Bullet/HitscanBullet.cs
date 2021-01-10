using System.Collections.Generic;
using MEC;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class HitscanBullet : Bullet
{
    private LineRenderer _lineRenderer;
    private RaycastHit2D _hit;

    private void Awake() {
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.enabled = false;
    }

    private void OnEnable() {
        if(data == null)    return;
        
        _lineRenderer.colorGradient = data.color;
        _lineRenderer.startWidth = data.size;
        _lineRenderer.endWidth = data.size;
        
        _hit = Physics2D.Raycast(transform.position, transform.right, data.range, data.collisionMask);
        if(_hit.collider != null)
            TryDamage(_hit.collider.gameObject);
        DoOnHitFX();
        Timing.RunCoroutine(DrawLine(transform.position,
                                     _hit.collider == null
                                         ? transform.position + transform.right * data.range
                                         : (Vector3) _hit.point));
    }

    IEnumerator<float> DrawLine(Vector3 start, Vector3 end) {
        _lineRenderer.SetPositions(new[] {start, end});
        _lineRenderer.enabled = true;
        yield return Timing.WaitForSeconds(0.2f);
        _lineRenderer.enabled = false;
        ObjectPooler.Return(data.poolTag, gameObject);
    }
}