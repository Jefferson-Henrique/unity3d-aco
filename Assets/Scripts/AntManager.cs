using JH.ACO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AntManager : MonoBehaviour {

    private Vector3 markerOffset = new Vector3(0, 0.1f, 0);

    public GameObject markerPrefab;

    public GameObject antPrefab;

    public Material lineMaterial;

    public Slider numberAntsSlider;

    public Text numberAntsText;

    public Slider maxStepsSlider;

    public Text maxStepsText;

    public float speed = 2f;

    [HideInInspector]
    public List<Transform> markers = new List<Transform>();

    private IEnumerator<AntSystemResultIteration> iterationEnumerator;

    private AntSystem antSystem;

    private List<AntPath> ants;

    private AntSystemResult antSystemResult;

    private double[,] nextTrail;

    private int layerUI;

    // Use this for initialization
    void Start () {
        layerUI = LayerMask.NameToLayer("UI");
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject()) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 10)) {
                Transform newMarker = (Instantiate(markerPrefab, hit.point + markerOffset, Quaternion.identity) as GameObject).transform;
                markers.Add(newMarker);
            }
        }

        if (iterationEnumerator != null) {
            bool allDone = true;
            for (int index = 0; index < antSystem.NumberOfAnts; index++) {
                if (!ants[index].Done) {
                    allDone = false;
                    break;
                }
            }

            if (allDone) {
                nextTrail = iterationEnumerator.Current.afterEvaporation;
                MoveNextAntIteration(iterationEnumerator);

                if (iterationEnumerator == null) {
                    nextTrail = new double[antSystem.NumberOfNodes, antSystem.NumberOfNodes];
                    for (int index = 0; index < antSystemResult.bestPath.Count-1; index++) {
                        nextTrail[antSystemResult.bestPath[index], antSystemResult.bestPath[index + 1]] = antSystemResult.maxPheromone;
                        nextTrail[antSystemResult.bestPath[index+1], antSystemResult.bestPath[index]] = antSystemResult.maxPheromone;
                    }
                }
            }
        }
	}

    public void OnSliderValuesChange() {
        numberAntsText.text = numberAntsSlider.value + "";
        maxStepsText.text = maxStepsSlider.value + "";
    }

    public void Run() {
        antSystem = new AntSystem(markers.Count, (int) numberAntsSlider.value, 0.1, 1, 0.5, 1, 1);

        for (int index1 = 0; index1 < markers.Count; index1++) {
            for (int index2 = 0; index2 < markers.Count; index2++) {
                if (index1 <= index2) {
                    antSystem.AddPathCost(index1, index2, Vector3.Distance(markers[index1].position, markers[index2].position));
                }
            }
        }

        ants = new List<AntPath>(GameObject.FindObjectsOfType<AntPath>());
        int diffCount = antSystem.NumberOfAnts - ants.Count;

        for (int index = 0; index < Mathf.Abs(diffCount); index++) {
            if (diffCount > 0) {
                ants.Add((Instantiate(antPrefab, markers[0].position, Quaternion.identity) as GameObject).GetComponent<AntPath>());
            } else {
                GameObject.Destroy(ants[0].gameObject);
                ants.RemoveAt(0);
            }
        }

        antSystemResult = antSystem.Execute((int) maxStepsSlider.value, 0);
        IEnumerator<AntSystemResultIteration> iterator = antSystemResult.iterations.GetEnumerator();
        MoveNextAntIteration(iterator);

        nextTrail = null;
        iterationEnumerator = iterator;
    }

    private void MoveNextAntIteration(IEnumerator<AntSystemResultIteration> iterator) {
        if (!iterator.MoveNext()) {
            iterationEnumerator = null;
            return;
        }

        AntSystemResultIteration nextIt = iterator.Current;
        for (int index = 0; index < antSystem.NumberOfAnts; index++) {
            ants[index].antManager = this;
            ants[index].path = nextIt.ants[index].visitedPath;
            ants[index].Reset();
        }
    }

    public void OnPostRender() {
        float lineWidth = 0.01f;

        GL.PushMatrix();
        lineMaterial.SetPass(0);

        for (int index1 = 0; index1 < markers.Count; index1++) {
            for (int index2 = 0; index2 < markers.Count; index2++) {
                Transform marker1 = markers[index1];
                Transform marker2 = markers[index2];

                if (marker1 == marker2) continue;

                if (nextTrail != null) {
                    lineWidth = (float) (0.01f + 0.1f * nextTrail[index1, index2] / antSystemResult.maxPheromone);
                }

                GL.Begin(GL.QUADS);

                if (iterationEnumerator == null) {
                    GL.Color(Color.blue);
                }

                Vector3 normal = Vector3.up;
                Vector3 side = Vector3.Cross(normal, marker2.position - marker1.position);
                side.Normalize();
                Vector3 a = marker1.position + side * (lineWidth / 2);
                Vector3 b = marker1.position + side * (lineWidth / -2);
                Vector3 c = marker2.position + side * (lineWidth / 2);
                Vector3 d = marker2.position + side * (lineWidth / -2);

                GL.Vertex(a);
                GL.Vertex(b);
                GL.Vertex(c);
                GL.Vertex(d);

                GL.End();
            }
        }

        GL.PopMatrix();
    }

}