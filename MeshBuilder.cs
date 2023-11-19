using UnityEngine;

public static class MeshBuilder {

    public class TubeOptions {
        public bool hasEnds = true;
        public bool hasNormals = true;
        public bool hasTangents = true;
        public bool hasColours = false;

        public Color colA;
        public Color colB;
    }

    public static Mesh Tube(float length, int n, float rA, float rB, TubeOptions options) {

        /*
         
        Returns a cylindrical mesh facing in the (Vector3.forward) z-direction
        'Circle A' centered on (0,0,0), radius rA
        'Circle B' centered on (0,0,1), radius rB
        Each circle has n vertices
        If !hasEnds then we will only generate the 'body' of the cylinder

        Mesh Structure:
            vertices[0..n-1] are 'Circle A'
            vertices[n..2n-1] are 'Circle B'
            vertices[2n, 2n+1] are the uv-wapping duplicates
            vertices[2n+2, 2n+3] are end verts

            normals, tangents and uvs are associated with the corresponding vertex

            triangles[] has 3 consecutive integers per triangle, giving the vertex index of each vertex in the triangle
            Cylinder body:
                triangles[0..3n-1] are those body triangles pointing towards 'End A'
                triangles[3n..6n-1] are those pointing towards 'End B'
            Cylinder ends:
                triangles[6n..9n-1] are those comprising 'End A'
                triangles[9n..12n-1] are those comprising 'End B'
        */


        int NUM_VERTS = 2 * n + 2; //Two n-point circles, 2 duplicates for the uv-wrapping

        int ID_DUPLICATE_VERT_A = 2 * n + 0;
        int ID_DUPLICATE_VERT_B = 2 * n + 1;

        if (options.hasEnds) { NUM_VERTS += 2; } //Two end verts
        int ID_END_VERT_A = 2 * n + 2;
        int ID_END_VERT_B = 2 * n + 3;

        int NUM_TRIS = 2 * n;
        if (options.hasEnds) { NUM_TRIS += 2 * n; } //Each end has n triangles

        //Per-Vertex:
        Vector3[] vertices = new Vector3[NUM_VERTS]; 
        Vector3[] normals = new Vector3[NUM_VERTS]; //The duplicate vertices have no normals/tangents
        Vector3[] tangents = new Vector3[NUM_VERTS];
        Vector2[] uvs = new Vector2[NUM_VERTS];
        Color[] colours = new Color[NUM_VERTS];

        //Important: triangle vertex winding order determines 'outside' viewing direction
        int[] triangles = new int[NUM_TRIS * 3];

        float deltaTheta = 360 / n;
        float radius = rA;
        Color colour = options.colA;
        for (int z = 0; z < 2; z++) { //Two z-layers: start and end
            if (z == 1) { radius = rB; colour = options.colB; }
            for (int i = 0; i < n; i++) { //n vertices per layer
                //We proceed anticlockwise (pov end A) around the cylinder, from uv.x=n/n, uv.x=(n-1)/n to uv.x=1/n, but never reaching uv.x=0: that is what our duplicate vertices fix

                float theta = i * deltaTheta + deltaTheta * z * 0.5f;
                int CURRENT_VERT = i + n * z;

                vertices[CURRENT_VERT] = Quaternion.AngleAxis(theta, Vector3.forward) * Vector3.up * radius + new Vector3(0, 0, z*length);

                colours[CURRENT_VERT] = colour;

                //Since we're centered about 0,0,0:
                normals[CURRENT_VERT] = vertices[CURRENT_VERT];
                normals[CURRENT_VERT].z = 0;
                normals[CURRENT_VERT].Normalize();

                tangents[CURRENT_VERT] = FindPerp(normals[CURRENT_VERT]);

                uvs[CURRENT_VERT] = new Vector2((float)(n - i) / n, z);

                //'Body' triange with tip at the current vertex
                triangles[3 * CURRENT_VERT + 0] = CURRENT_VERT;
                triangles[3 * CURRENT_VERT + 1] = i + n * (1 - z); 
                triangles[3 * CURRENT_VERT + 2] = (n + i + (2 * z - 1)) % n + n * (1 - z);
            }
        }
        
        //Add ends
        if (options.hasEnds) {
            //Two end vertices
            vertices[ID_END_VERT_A] = new Vector3(0, 0, 0);
            colours[ID_END_VERT_A] = options.colA;
            normals[ID_END_VERT_A] = Vector3.back;
            tangents[ID_END_VERT_A] = Vector3.forward;
            uvs[ID_END_VERT_A] = new Vector2(0.5f, 1);

            vertices[ID_END_VERT_B] = new Vector3(0, 0, length);
            colours[ID_END_VERT_B] = options.colB;
            normals[ID_END_VERT_B] = Vector3.back;
            tangents[ID_END_VERT_B] = Vector3.forward;
            uvs[ID_END_VERT_B] = new Vector2(0.5f, 0);

            //Triangles
            for (int z = 0; z < 2; z++) { //Two z-layers: start and end
                int CURRENT_END_VERT = 2 * n + 2 + z;
                if (z == 1) { radius = rB; }
                for (int i = 0; i < n; i++) { //n vertices per layer
                    int CURRENT_VERT = i + n * z;
                    int CURRENT_NEXT_VERT = (i + 1) % n + n * z; //(i+1)%n to loop us around
                    triangles[3 * (2 * n + CURRENT_VERT) + 0] = CURRENT_END_VERT;
                    triangles[3 * (2 * n + CURRENT_VERT) + 1 + (1 - z)] = CURRENT_VERT; //using (1-z) to change the winding order depending on which end we're on
                    triangles[3 * (2 * n + CURRENT_VERT) + 2 - (1 - z)] = CURRENT_NEXT_VERT;
                }
            }
        }

        //Fix uv-wrapping issue
        //Add duplicate vertices, copying the 0th of each circle, but with uv.x=0
         vertices[ID_DUPLICATE_VERT_A] = vertices[0];
        colours[ID_DUPLICATE_VERT_A] = colours[0];
        uvs[ID_DUPLICATE_VERT_A] = new Vector2(0, 0);
         normals[ID_DUPLICATE_VERT_A] = normals[0];
         tangents[ID_DUPLICATE_VERT_A] = tangents[0];

         vertices[ID_DUPLICATE_VERT_B] = vertices[n];
        colours[ID_DUPLICATE_VERT_B] = colours[n];
        uvs[ID_DUPLICATE_VERT_B] = new Vector2(0, 1);
         normals[ID_DUPLICATE_VERT_B] = normals[n];
         tangents[ID_DUPLICATE_VERT_B] = tangents[n];

         //Fix the last triangles of each circle to map to these duplicates instead
         triangles[0] = ID_DUPLICATE_VERT_A; triangles[1] = ID_DUPLICATE_VERT_B;  //Blue case
         triangles[3 * (2 * n - 1) + 2] = ID_DUPLICATE_VERT_A; //Yellow case

        if (options.hasEnds) {
             //Fix the last triangles of each face
             triangles[3 * (2 * n + n - 1) + 0] = ID_END_VERT_A;
             triangles[3 * (2 * n + n - 1) + 1] = ID_DUPLICATE_VERT_A;

             triangles[3 * (2 * n + 2 * n - 1) + 0] = ID_END_VERT_B;
             triangles[3 * (2 * n + 2 * n - 1) + 2] = ID_DUPLICATE_VERT_B;
        }

        //Create mesh
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        if (options.hasNormals) { mesh.normals = normals; }
        mesh.uv = uvs;
        if (options.hasColours) { mesh.colors = colours; }
        return mesh;
    }

    private static Vector3 FindPerp(Vector3 v) {
        //Pick unit vector w/ edge-case v=(0,0,k) aka parallel to our default unit
        Vector3 unit = Vector3.forward;
        if (Mathf.Abs(v.y) < 0.01f && Mathf.Abs(v.x) < 0.01f) { unit = Vector3.up; }

        return Vector3.Cross(v, unit).normalized;
    }

}