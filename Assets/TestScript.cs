using UnityEngine;
using System.Collections;
using JH.ACO;
using System.Collections.Generic;

public class TestScript : MonoBehaviour {

    public float speed = 10;

	// Use this for initialization
	void Start () {
        AntSystem antSystem = new AntSystem(6, 5, 0.1, 100, 0.5, 1, 1);
        antSystem.AddPathCost(0, 1, 150);
        antSystem.AddPathCost(0, 2, 150);
        antSystem.AddPathCost(0, 3, 353.5534);
        antSystem.AddPathCost(0, 4, 287.9236);
        antSystem.AddPathCost(0, 5, 418.8078);

        antSystem.AddPathCost(1, 2, 212.1320);
        antSystem.AddPathCost(1, 3, 269.2582);
        antSystem.AddPathCost(1, 4, 274.5906);
        antSystem.AddPathCost(1, 5, 359.0265);

        antSystem.AddPathCost(2, 3, 269.2582);
        antSystem.AddPathCost(2, 4, 156.2050);
        antSystem.AddPathCost(2, 5, 304.7950);

        antSystem.AddPathCost(3, 4, 151.3274);
        antSystem.AddPathCost(3, 5, 101.9804);

        antSystem.AddPathCost(4, 5, 152.6434);

        List<int> bestPath = antSystem.Execute(10, 0).bestPath;

        double sum = 0;
        for (int index = 0; index < bestPath.Count-1; index++) {
            sum += antSystem.PathCosts[bestPath[index], bestPath[index + 1]];
        }

        Debug.Log(">>> " + sum);
    }
	
	// Update is called once per frame
	void Update () {
        this.transform.Translate(Vector3.forward * speed * Time.deltaTime);
	}
}
