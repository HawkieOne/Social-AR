using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

[RequireComponent(typeof(ARPlaneMeshVisualizer), typeof(MeshRenderer), typeof(ARPlane))]
public class ARFeatheredPlaneMeshVisualizer2 : MonoBehaviour
{
    static List<Vector3> s_FeatheringsUVs = new List<Vector3>();
    static List<Vector3> s_Vertices = new List<Vector3>();
    ARPlaneMeshVisualizer m_PlaneMeshVisualizer;
    ARPlane m_Plane;
    Material m_FeatheredPlaneMaterial;
    [SerializeField]
    float m_FeatheringWidth = 0.2f;
    public float feathingWidth
    {
        get { return m_FeatheringWidth;  }
        set { m_FeatheringWidth = value; }
    }
    // Start is called before the first frame update
    void Awake()
    {
        m_PlaneMeshVisualizer = GetComponent<ARPlaneMeshVisualizer>();
        m_FeatheredPlaneMaterial = GetComponent<MeshRenderer>().material;
        m_Plane = GetComponent<ARPlane>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnEnable()
    {
        m_Plane.boundaryChanged += ARPlane_boundaryUpdated;
    }

    void OnDisable()
    {
        m_Plane.boundaryChanged -= ARPlane_boundaryUpdated;
    }

    void ARPlane_boundaryUpdated(ARPlaneBoundaryChangedEventArgs eventArgs)
    {
        GenerateBoundaryUVs(m_PlaneMeshVisualizer.mesh);
    }

    void GenerateBoundaryUVs(Mesh mesh)
    {
        int vertexCount = mesh.vertexCount;
        s_FeatheringsUVs.Clear();
        if (s_FeatheringsUVs.Capacity < vertexCount) { s_FeatheringsUVs.Capacity = vertexCount; }
        mesh.GetVertices(s_Vertices);
        Vector3 centerInPlaneSpace = s_Vertices[s_Vertices.Count - 1];
        Vector3 uv = new Vector3(0, 0, 0);
        float shortestUVMapping = float.MaxValue;
        for (int i = 0; i < vertexCount - 1; i++)
        {
            float vertexDist = Vector3.Distance(s_Vertices[i], centerInPlaneSpace);
            float uvMapping = vertexDist / Mathf.Max(vertexDist - feathingWidth, 0.001f);
            uv.x = uvMapping;
            if (shortestUVMapping > uvMapping) { shortestUVMapping = uvMapping; }
            s_FeatheringsUVs.Add(uv);
        }
        m_FeatheredPlaneMaterial.SetFloat("_ShortestUVMapping", shortestUVMapping);
        uv.Set(0, 0, 0);
        s_FeatheringsUVs.Add(uv);
        mesh.SetUVs(1, s_FeatheringsUVs);
        mesh.UploadMeshData(false);
    }
}
