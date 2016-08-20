using JH.ACO;
using System.Collections.Generic;
using UnityEngine;

public class AntManager : MonoBehaviour {

    private Vector3 markerOffset = new Vector3(0, 0.1f, 0);

    public GameObject markerPrefab;

    public GameObject antPrefab;

    public Material lineMaterial;

    [HideInInspector]
    public List<Transform> markers = new List<Transform>();

    // Use this for initialization
    void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButtonDown(0)) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 10)) {
                UpdateMarkers((Instantiate(markerPrefab, hit.point + markerOffset, Quaternion.identity) as GameObject).transform);
            }
        }
	}

    public void Run() {
        AntSystem antSystem = new AntSystem(markers.Count, 5, 0.1, 100, 0.5, 1, 1);

        for (int index1 = 0; index1 < markers.Count; index1++) {
            for (int index2 = 0; index2 < markers.Count; index2++) {
                if (index1 <= index2) {
                    antSystem.AddPathCost(index1, index2, Vector3.Distance(markers[index1].position, markers[index2].position));
                }
            }
        }

        List<AntPath> ants = new List<AntPath>(GameObject.FindObjectsOfType<AntPath>());
        int diffCount = antSystem.NumberOfAnts - ants.Count;

        for (int index = 0; index < Mathf.Abs(diffCount); index++) {
            if (diffCount > 0) {
                ants.Add((Instantiate(antPrefab, markers[0].position, Quaternion.identity) as GameObject).GetComponent<AntPath>());
            } else {
                GameObject.Destroy(ants[0].gameObject);
                ants.RemoveAt(0);
            }
        }

        AntSystemResult antSystemResult = antSystem.Execute(10, 0);

        for (int index = 0; index < antSystem.NumberOfAnts; index++) {
            ants[index].antManager = this;
            ants[index].path = antSystemResult.iterations[0].ants[index].visitedPath;
            ants[index].Reset();
        }
    }

    private void UpdateMarkers(Transform newMarker) {
        if (markers.Count > 0) {
            LineRenderer lineRenderer = newMarker.gameObject.AddComponent<LineRenderer>();
            lineRenderer.SetWidth(0.01f, 0.01f);
            lineRenderer.material = lineMaterial;
            lineRenderer.SetVertexCount(2 * markers.Count);

            for (int index = 0; index < markers.Count; index++) {
                lineRenderer.SetPosition(2 * index, newMarker.position);
                lineRenderer.SetPosition(2 * index + 1, markers[index].position);
            }
        }

        markers.Add(newMarker);
    }

}