using UnityEngine;
using System.Collections;

public class Laser : MonoBehaviour {

    public const int LASER_LENGTH = 4;
    private LineRenderer lineRenderer;

	// Use this for initialization
	void Start () {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.enabled = false;
        lineRenderer.useWorldSpace = true;
	}
	
    public void DisplayLaser(Transform own, Transform player) {
        lineRenderer.enabled = true;
        lineRenderer.SetPosition(0, own.position);
        lineRenderer.SetPosition(1, player.position);
    }
}
