using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleTreeGen : MonoBehaviour {
    public Material material;

    List<int> sequence = new List<int> { 0, 3, 0, 3, 0 };
    int[][] productionRules = new int[][] {
            new int[] {0,3,1,2,0,2,1,3,0},
            new int[] { 1, 1 },
            new int[] { 2 },
            new int[] { 3 }
     };

    public void Iterate() {
        List<int> newSequence = new List<int>();
        for (int i = 0; i < sequence.Count; i++) {
            int predecessor = sequence[i];
            int[] sucessors = productionRules[predecessor];
            foreach (int sucessor in sucessors) {
                newSequence.Add(sucessor);
            }
        }
        sequence = newSequence;
    }

    public void Turtle() {
        Vector3 position = Vector3.zero;
        Vector3 direction = Vector3.right;
        float length = 1;

        foreach (int item in sequence) {
            switch (item) {
                case 0:
                    CreateCylinderBetweenPoints(10, false, position, position + direction * length);
                    position += direction * length;
                    break;
                case 1:
                    position += direction * length;
                    break;
                case 2:
                    direction = Quaternion.AngleAxis(120, Vector3.up) * direction;
                    break;
                case 3:
                    direction = Quaternion.AngleAxis(-120, Vector3.up) * direction;
                    break;
            }
        }
    }

    public void CreateCylinderBetweenPoints(int n, bool hasEnds, Vector3 p1, Vector3 p2) {
        GameObject branch = new GameObject();
        MeshFilter meshFilter = branch.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = branch.AddComponent<MeshRenderer>();
        meshRenderer.material = material;
        meshFilter.mesh = MeshBuilder.Tube(hasEnds, n, 0.1f, 0.1f);

        branch.transform.position = p1;
        branch.transform.localScale = new Vector3(1,1,(p2 - p1).magnitude);
        branch.transform.LookAt(p2);
        branch.transform.parent = gameObject.transform;

    }

    public void GenerateCylinderTest() {
        for (int x = 0; x<10; x++) {
            for (int z = 0; z < 10; z++) {
                CreateCylinderBetweenPoints(3 + x, true, new Vector3(0.2f * x, 0, 0.2f * z), new Vector3(0.2f * x, (z+1)*0.2f, 0.2f * z));
            }
        }
    }

    void Start() {
        //GenerateCylinderTest();

        Iterate();
        Iterate();
        Iterate();

        Turtle();

    }
}
