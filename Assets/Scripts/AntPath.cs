using System.Collections.Generic;
using UnityEngine;

public class AntPath : MonoBehaviour {

    [HideInInspector]
    public AntManager antManager;

    [HideInInspector]
    public List<int> path;

    private int index = 0;

    private float timer = 0;

    private float distance = 0;

    public bool Done {
        get {
            return index == -1;
        }
    }

    public void Reset() {
        index = 0;
        timer = 0;
        transform.position = antManager.markers[0].position;
        LookAt(1);
    }

    public void Update() {
        if (Done) {
            return;
        }

        timer += Time.deltaTime / distance * antManager.speed;
        transform.position = Vector3.Lerp(antManager.markers[path[index]].position, antManager.markers[path[index + 1]].position, timer);

        if (timer >= 1) {
            timer = 0;
            index++;

            if (index < path.Count - 1) {
                LookAt(index + 1);
            } else {
                index = -1;
            }
        }
    }

    private void LookAt(int index) {
        Vector3 rotation = Quaternion.LookRotation(antManager.markers[path[index]].position - transform.position).eulerAngles;
        rotation.z = 0;
        rotation.x = 0;

        this.transform.rotation = Quaternion.Euler(rotation);

        distance = Vector3.Distance(antManager.markers[path[index - 1]].position, antManager.markers[path[index]].position);
    }

}