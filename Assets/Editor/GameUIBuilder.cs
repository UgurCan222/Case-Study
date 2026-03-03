using UnityEngine;
using UnityEditor;

public class PawnGenerator : Editor
{
    [MenuItem("Tools/Piyon Oluþtur")]
    public static void CreatePawn()
    {
        // 1. Ana Objeyi Oluþtur
        GameObject pawnRoot = new GameObject("Piyon");

        // 2. Materyalleri Oluþtur ve Kaydet
        string matPath = "Assets/PawnAssets";
        if (!AssetDatabase.IsValidFolder(matPath))
        {
            AssetDatabase.CreateFolder("Assets", "PawnAssets");
        }

        // Materyaller zaten varsa yenisini yaratmak yerine olaný kullanýyoruz (hata vermemesi için)
        Material bodyMat = LoadOrCreateMaterial($"{matPath}/BodyMat.mat", new Color(0.96f, 0.53f, 0.14f), 0.3f);
        Material headMat = LoadOrCreateMaterial($"{matPath}/HeadMat.mat", new Color(0.96f, 0.81f, 0.67f), 0.1f);
        Material eyeMat = LoadOrCreateMaterial($"{matPath}/EyeMat.mat", new Color(0.2f, 0.18f, 0.17f), 0.0f);

        // 3. Gövdeyi Oluþtur
        GameObject body = new GameObject("Gövde");
        body.transform.SetParent(pawnRoot.transform);
        MeshFilter bodyMF = body.AddComponent<MeshFilter>();
        MeshRenderer bodyMR = body.AddComponent<MeshRenderer>();

        Mesh bodyMesh = CreateTruncatedConeMesh(0.55f, 0.35f, 1.3f, 32);
        bodyMF.mesh = bodyMesh;
        bodyMR.sharedMaterial = bodyMat;

        // Mesh için de üzerine yazma kontrolü
        Mesh existingMesh = AssetDatabase.LoadAssetAtPath<Mesh>($"{matPath}/BodyMesh.asset");
        if (existingMesh != null)
        {
            existingMesh.Clear();
            EditorUtility.CopySerialized(bodyMesh, existingMesh);
        }
        else
        {
            AssetDatabase.CreateAsset(bodyMesh, $"{matPath}/BodyMesh.asset");
        }

        // 4. Kafayý Oluþtur (Küre)
        GameObject head = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        head.name = "Kafa";
        head.transform.SetParent(pawnRoot.transform);
        head.transform.localPosition = new Vector3(0, 1.3f, 0);

        // YENÝ KAFA ÖLÇEÐÝ: Y ekseni 0.85'ten 0.95'e çýkarýldý (Daha az basýk)
        head.transform.localScale = new Vector3(1.1f, 0.95f, 1.1f);
        head.GetComponent<MeshRenderer>().sharedMaterial = headMat;
        DestroyImmediate(head.GetComponent<Collider>());

        // 5. Gözleri Oluþtur
        // YENÝ GÖZ DERÝNLÝÐÝ: Z ekseni 0.38 ile 0.46 arasý dengelendi (0.43f)
        GameObject eyeL = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        eyeL.name = "SolGoz";
        eyeL.transform.SetParent(head.transform);
        eyeL.transform.localPosition = new Vector3(-0.15f, 0.15f, 0.43f);
        eyeL.transform.localScale = new Vector3(0.12f, 0.15f, 0.12f);
        eyeL.GetComponent<MeshRenderer>().sharedMaterial = eyeMat;
        DestroyImmediate(eyeL.GetComponent<Collider>());

        GameObject eyeR = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        eyeR.name = "SagGoz";
        eyeR.transform.SetParent(head.transform);
        eyeR.transform.localPosition = new Vector3(0.15f, 0.15f, 0.43f);
        eyeR.transform.localScale = new Vector3(0.12f, 0.15f, 0.12f);
        eyeR.GetComponent<MeshRenderer>().sharedMaterial = eyeMat;
        DestroyImmediate(eyeR.GetComponent<Collider>());

        // 6. Hepsini Prefab Olarak Kaydet
        string prefabPath = "Assets/PawnAssets/Piyon.prefab";
        PrefabUtility.SaveAsPrefabAsset(pawnRoot, prefabPath);

        DestroyImmediate(pawnRoot);
        AssetDatabase.SaveAssets();
        Debug.Log($"Piyon baþarýyla güncellendi! Þuradan bulabilirsiniz: {prefabPath}");
    }

    // Materyal üzerine yazma hatalarýný önleyen yardýmcý fonksiyon
    static Material LoadOrCreateMaterial(string path, Color color, float glossiness)
    {
        Material mat = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (mat == null)
        {
            mat = new Material(Shader.Find("Standard"));
            mat.color = color;
            mat.SetFloat("_Glossiness", glossiness);
            AssetDatabase.CreateAsset(mat, path);
        }
        else
        {
            mat.color = color;
            mat.SetFloat("_Glossiness", glossiness);
        }
        return mat;
    }

    static Mesh CreateTruncatedConeMesh(float bottomRadius, float topRadius, float height, int segments)
    {
        Mesh mesh = new Mesh();
        mesh.name = "KesikKoni";

        int numVertices = segments * 4 + 2;
        int numTriangles = segments * 12;

        Vector3[] vertices = new Vector3[numVertices];
        int[] triangles = new int[numTriangles];

        int bottomCenterIndex = segments * 4;
        int topCenterIndex = segments * 4 + 1;

        vertices[bottomCenterIndex] = new Vector3(0, 0, 0);
        vertices[topCenterIndex] = new Vector3(0, height, 0);

        for (int i = 0; i < segments; i++)
        {
            float angle = (float)i / segments * Mathf.PI * 2f;
            float cos = Mathf.Cos(angle);
            float sin = Mathf.Sin(angle);

            Vector3 bottomPos = new Vector3(cos * bottomRadius, 0, sin * bottomRadius);
            Vector3 topPos = new Vector3(cos * topRadius, height, sin * topRadius);

            vertices[i] = bottomPos;
            vertices[i + segments] = topPos;
            vertices[i + segments * 2] = bottomPos;
            vertices[i + segments * 3] = topPos;

            int nextI = (i + 1) % segments;

            // Yan yüzeyler
            int t = i * 6;
            triangles[t] = i;
            triangles[t + 1] = i + segments;
            triangles[t + 2] = nextI;
            triangles[t + 3] = nextI;
            triangles[t + 4] = i + segments;
            triangles[t + 5] = nextI + segments;

            // Alt kapak
            int tb = segments * 6 + i * 3;
            triangles[tb] = bottomCenterIndex;
            triangles[tb + 1] = nextI + segments * 2;
            triangles[tb + 2] = i + segments * 2;

            // Üst kapak
            int tt = segments * 9 + i * 3;
            triangles[tt] = topCenterIndex;
            triangles[tt + 1] = i + segments * 3;
            triangles[tt + 2] = nextI + segments * 3;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        return mesh;
    }
}