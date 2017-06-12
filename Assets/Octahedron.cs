/*
 * Copyright (c) Jari Senhorst. All rights reserved.  
 * 
 * Licensed under the MIT License. See LICENSE file in the project root for full license information.  
 * 
 *
 * This class (Octahedron) handles ..
 * 
 */

using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System;
using System.IO;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class Octahedron : MonoBehaviour 
{
    private List<Vector3> m_vertices = new List<Vector3>();
    private List<int> m_triangles = new List<int>();

    private Mesh m_mesh;

    [SerializeField]
    private float m_radius;

    [SerializeField]
    private int m_subDivisions;

	/// <summary>
    /// Script entry point
    /// </summary>
	private void Start () 
	{
        if(m_subDivisions > 6)
        {
            Debug.LogWarning("Subdivisions set to 6, this is the maximum (you set " + m_subDivisions + ")!");
            m_subDivisions = 6;
        }

        SetupBase();
        SubdivisionManage();
        UpdateMesh();
        ExportMesh();
    }

    private void ExportMesh()
    {
        DateTime dt = DateTime.Now;
        StringBuilder sb = new StringBuilder();
        sb.Append("# Jstylezzz Wavefront OBJ Exporter V0.1b - (c)2017 Jari Senhorst\n");
        sb.Append("# File Created: " + dt.Day + "." + dt.Month + "." + dt.Year + " " + dt.Hour + ":" + dt.Minute + ":" + dt.Second + "\n");
        sb.Append("\n#\n# " + m_mesh.name + "\n#\n");

        for (int i = 0; i < m_vertices.Count; i++)
        {
            sb.Append("\nv  " + m_vertices[i].ToString().Replace("(", "").Replace(")", "").Replace(",", ""));
        }
        sb.Append("\n# " + m_vertices.Count + " vertices\n");
        sb.Append("\ng " + m_mesh.name);

        for (int i = 0; i < m_triangles.Count; i+=3)
        {
            sb.Append("\nf " + (m_triangles[i]+1) + " " + (m_triangles[i + 1] + 1) + " " + (m_triangles[i + 2] + 1));
        }
        sb.Append("\n# " + m_triangles.Count + " faces\n\n");

        File.WriteAllText(m_mesh.name + ".obj", sb.ToString());
    }

    private void SubdivisionManage()
    {
        for (int i = 0; i < m_subDivisions; i++)
        {
            DoSubdivide();
        }
    }

    private void UpdateMesh()
    {
        m_mesh = new Mesh();
        m_mesh.SetVertices(m_vertices);
        m_mesh.SetTriangles(m_triangles.ToArray(), 0);
        m_mesh.name = "Octading";
        m_mesh.RecalculateBounds();
        m_mesh.RecalculateNormals();
        m_mesh.RecalculateTangents();

        GetComponent<MeshFilter>().mesh = m_mesh;
    }

    private void SetupBase()
    {
        //vertizenses
        AddVertex(0, 1, 0);

        AddVertex(-1, 0, 1);
        AddVertex(1, 0, 1);

        AddVertex(-1, 0, -1);
        AddVertex(1, 0, -1);

        AddVertex(0, -1, 0);

        //triangelen
        AddTriangle(0, 1, 2); 
        AddTriangle(0, 3, 1); 
        AddTriangle(0, 4, 3); 
        AddTriangle(0, 2, 4); 
        AddTriangle(2, 1, 5);
        AddTriangle(1, 3, 5);
        AddTriangle(3, 4, 5);
        AddTriangle(4, 2, 5);
    }

    private void DoSubdivide()
    {
        List<int> newTriangleList = new List<int>();
        for (int i = 0; i < m_triangles.Count; i+=3)
        {
            int vidx1 = m_triangles[i];
            int vidx2 = m_triangles[i+1];
            int vidx3 = m_triangles[i+2];

            Vector3 v1 = m_vertices[vidx1];
            Vector3 v2 = m_vertices[vidx2];
            Vector3 v3 = m_vertices[vidx3];

            Vector3 newV1 = ((v1 + v2) / 2).normalized * m_radius;
            Vector3 newV2 = ((v2 + v3) / 2).normalized* m_radius;
            Vector3 newV3 = ((v3 + v1) / 2).normalized* m_radius;

            m_vertices.Add(newV1);
            int vidx4 = m_vertices.Count - 1;
            
            m_vertices.Add(newV2);
            int vidx5 = m_vertices.Count - 1;

            m_vertices.Add(newV3);
            int vidx6 = m_vertices.Count - 1;

            newTriangleList.Add(vidx1); //A
            newTriangleList.Add(vidx4); //D
            newTriangleList.Add(vidx6); //F

            newTriangleList.Add(vidx4); //D
            newTriangleList.Add(vidx2); //B
            newTriangleList.Add(vidx5); //E

            newTriangleList.Add(vidx6); //F
            newTriangleList.Add(vidx5); //E
            newTriangleList.Add(vidx3); //C

            newTriangleList.Add(vidx4); //D
            newTriangleList.Add(vidx5); //E
            newTriangleList.Add(vidx6); //F
        }

        m_triangles.Clear();

        for (int i = 0; i < newTriangleList.Count; i += 3)
        {
            AddTriangle(newTriangleList[i], newTriangleList[i + 1], newTriangleList[i + 2]);
        }
    }
	
    private void AddTriangle(int vi1, int vi2, int vi3)
    {
        m_triangles.Add(vi1);
        m_triangles.Add(vi2);
        m_triangles.Add(vi3);
    }

    private void AddVertex(float x, float y, float z)
    {
        m_vertices.Add(new Vector3(x, y, z).normalized * m_radius);
    }

	/// <summary>
    /// Script main update loop
    /// </summary>
	private void Update () 
	{
        transform.Rotate(Vector3.up, 40 * Time.deltaTime);
	}
}
