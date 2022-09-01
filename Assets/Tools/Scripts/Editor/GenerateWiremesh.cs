using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;



public class GenerateWiremesh : EditorWindow
{
    GameObject currentSelectTarget;
    bool isSkinnedMesh = true;
    private bool isQuad = true;
    public struct Edge
    {
        public Vector3 v1;
        public Vector3 v2;

        public Edge(Vector3 v1, Vector3 v2)
        {

           this.v1 = v2;
           this.v2 = v1;

        }
    }


    [MenuItem("Tools/Create Wiremesh")]
    static void CreateWire()
    {
        GenerateWiremesh window = EditorWindow.GetWindow<GenerateWiremesh>("创建Wiremesh");
        window.minSize = new Vector2(300, 200);
        window.Show();


    }

    void OnGUI()
    {
        GUILayout.Label("【第一步】指定网格对象", GUILayout.Width(150));
        currentSelectTarget = (GameObject)EditorGUILayout.ObjectField("网格对象", currentSelectTarget, typeof(GameObject), true);
        isSkinnedMesh = EditorGUILayout.Toggle("SkinnedMesh", isSkinnedMesh);
        isQuad = EditorGUILayout.Toggle("TryQuad", isQuad);
        if (GUILayout.Button("处理"))
        {
            OnCreate();
        }


    }

    void OnCreate()
    {
        //Path of the file
        string path = Application.dataPath+ "/" + currentSelectTarget.name+ ".wire";
        //Create File
         File.WriteAllText(path, "WIREMESH\n");

        //Read mesh data
        //Content of the file
        //string content = "0.5 -0.5 0.0 0.5 0.5 0.0" + "\n";
        string content = ReadMesh();

        //Add some to text to it
        File.AppendAllText(path, content);
    }

    string ReadMesh()
    {
        Mesh mesh = new Mesh();
        if (isSkinnedMesh)
        {
            mesh = currentSelectTarget.GetComponent<SkinnedMeshRenderer>().sharedMesh;
        }
        else
        {
            mesh = currentSelectTarget.GetComponent<MeshFilter>().sharedMesh;
        }

        //GetMeshEdges(mesh);
        Edge[] edges = GetMeshEdges(mesh);
        string wireMeshData="";
        for (int i = 0; i < edges.Length; i++)
        {
            wireMeshData += VectorToString(edges[i].v1) + " " + VectorToString(edges[i].v2) + "\n";
            //Debug.Log(i + ": " + VectorToString(edges[i].v1) + ", " + VectorToString(edges[i].v2));
        }
        return wireMeshData;
    }

    string VectorToString(Vector3 vector)
    {
        string vectorString = string.Format("{0} {1} {2}", vector.x, vector.y, vector.z);
        return vectorString;
    }
    /*

    void GetVertices()
    {
        Mesh newMesh = new Mesh();

        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;
        int triCount = triangles.Length;

        Vector3[] newVertices = new Vector3[triCount];
        for (int i = 0, imax = triCount; i < imax; i++)
        {
            newVertices[i] = vertices[triangles[i]];
        }
        newMesh.vertices = newVertices;
    }
    */


    //三角面三条线条，统计总和，并没有去除重复的线条
    private Edge[] GetMeshEdges(Mesh mesh)
    {
        List<Edge> edges = new List<Edge>();

        for (int i = 0; i < mesh.triangles.Length; i += 3)
        {
            Vector3 v1 = mesh.vertices[mesh.triangles[i]];
            Vector3 v2 = mesh.vertices[mesh.triangles[i + 1]];
            Vector3 v3 = mesh.vertices[mesh.triangles[i + 2]];


            if (isQuad)
            {
                float d1 = Vector3.Distance(v1,v2);
                float d2 = Vector3.Distance(v2,v3);
                float d3 = Vector3.Distance(v3,v1);
                if (d1 > d2 && d1 > d3)
                {
                    edges.Add(new Edge(v1, v3));
                    edges.Add(new Edge(v2, v3));
                }

                if (d2 > d1 && d2 > d3)
                {
                    edges.Add(new Edge(v1, v2));
                    edges.Add(new Edge(v1, v3));
                }

                if (d3 > d1 && d3 > d2)
                {
                    edges.Add(new Edge(v1, v2));
                    edges.Add(new Edge(v2, v3));
                }
            }
            else
            {
                edges.Add(new Edge(v1, v2));
                edges.Add(new Edge(v1, v3));
                edges.Add(new Edge(v2, v3));
            }




        }

        return edges.ToArray();
    }

}
