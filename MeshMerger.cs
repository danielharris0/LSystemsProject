using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This Component merges the meshes of all its object's children into one.
//[Taken initially from the unity documentation: https://docs.unity3d.com/ScriptReference/Mesh.CombineMeshes.html]

public static class MeshMerger{
    public static void MergeMeshes(GameObject parent, Material material) {
        MeshFilter[] meshFilters = parent.GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];

        for (int i = 0; i < meshFilters.Length; i++) {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;

            //Set Inactive
            meshFilters[i].gameObject.SetActive(false);
            if (meshFilters[i].gameObject.transform.parent != null) {
                meshFilters[i].gameObject.transform.parent.gameObject.SetActive(false);
            }
        }

        Mesh mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        mesh.CombineMeshes(combine);
        parent.transform.gameObject.AddComponent<MeshFilter>().sharedMesh = mesh;
        parent.transform.gameObject.AddComponent<MeshRenderer>().material = material;
        parent.transform.gameObject.SetActive(true);

        foreach (Transform child in parent.transform) {
            Object.Destroy(child.gameObject);
        }
    }

}
