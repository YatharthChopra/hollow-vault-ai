using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEditor;
using TMPro;

// Hollow Vault scene setup tool
// Window -> Hollow Vault -> Setup Scene
// Click the button and the whole scene gets built automatically
public class HollowVaultSetup : EditorWindow
{
    [MenuItem("Window/Hollow Vault/Setup Scene")]
    public static void ShowWindow()
    {
        GetWindow<HollowVaultSetup>("Hollow Vault Setup");
    }

    void OnGUI()
    {
        GUILayout.Label("Hollow Vault — Scene Setup", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        GUILayout.Label("Click the button to build the full scene from scratch.", EditorStyles.wordWrappedLabel);
        EditorGUILayout.Space();

        if (GUILayout.Button("Build Scene", GUILayout.Height(40)))
        {
            BuildScene();
        }

        EditorGUILayout.Space();
        GUILayout.Label("Note: bake the NavMesh manually after running this\n(Window > AI > Navigation > Bake)", EditorStyles.helpBox);
    }

    static void BuildScene()
    {
        // ----------------------------------------------------------------
        // 1. Floor
        // ----------------------------------------------------------------
        GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
        floor.name = "Floor";
        floor.transform.localScale = new Vector3(4f, 1f, 4f);

        // mark floor as Navigation Static so NavMesh can bake on it
        GameObjectUtility.SetStaticEditorFlags(floor, StaticEditorFlags.NavigationStatic);

        // ----------------------------------------------------------------
        // 2. Walls (four sides of a room)
        // ----------------------------------------------------------------
        GameObject walls = new GameObject("Walls");
        CreateWall(walls, "WallNorth", new Vector3(0f,  1f,  20f), new Vector3(40f, 2f,  1f));
        CreateWall(walls, "WallSouth", new Vector3(0f,  1f, -20f), new Vector3(40f, 2f,  1f));
        CreateWall(walls, "WallEast",  new Vector3(20f, 1f,   0f), new Vector3(1f,  2f, 40f));
        CreateWall(walls, "WallWest",  new Vector3(-20f,1f,   0f), new Vector3(1f,  2f, 40f));

        // ----------------------------------------------------------------
        // 3. Obstacles (pillars) — give the stealth some geometry to use
        // ----------------------------------------------------------------
        GameObject obstacles = new GameObject("Obstacles");
        Vector3[] pillarPositions = new Vector3[]
        {
            new Vector3(-8f,  1f, -8f),
            new Vector3( 8f,  1f, -8f),
            new Vector3(-8f,  1f,  8f),
            new Vector3( 8f,  1f,  8f),
            new Vector3( 0f,  1f,  0f),
        };
        foreach (Vector3 pos in pillarPositions)
        {
            GameObject pillar = GameObject.CreatePrimitive(PrimitiveType.Cube);
            pillar.name = "Pillar";
            pillar.transform.position = pos;
            pillar.transform.localScale = new Vector3(1.5f, 2f, 1.5f);
            pillar.transform.parent = obstacles.transform;
            GameObjectUtility.SetStaticEditorFlags(pillar, StaticEditorFlags.NavigationStatic);
        }

        // ----------------------------------------------------------------
        // 4. Sentinel patrol waypoints
        // ----------------------------------------------------------------
        GameObject sentinelWaypointRoot = new GameObject("SentinelWaypoints");
        Vector3[] sentinelWPs = new Vector3[]
        {
            new Vector3(-14f, 0f, -14f),
            new Vector3( 14f, 0f, -14f),
            new Vector3( 14f, 0f,  14f),
            new Vector3(-14f, 0f,  14f),
        };
        Transform[] sentinelWaypointTransforms = new Transform[sentinelWPs.Length];
        for (int i = 0; i < sentinelWPs.Length; i++)
        {
            GameObject wp = new GameObject("SentinelWP_" + i);
            wp.transform.position = sentinelWPs[i];
            wp.transform.parent = sentinelWaypointRoot.transform;
            sentinelWaypointTransforms[i] = wp.transform;
        }

        // ----------------------------------------------------------------
        // 5. Shade patrol waypoints (perimeter, inside the sentinel route)
        // ----------------------------------------------------------------
        GameObject shadeWaypointRoot = new GameObject("ShadeWaypoints");
        Vector3[] shadeWPs = new Vector3[]
        {
            new Vector3(-18f, 0f, -18f),
            new Vector3(  0f, 0f, -18f),
            new Vector3( 18f, 0f, -18f),
            new Vector3( 18f, 0f,   0f),
            new Vector3( 18f, 0f,  18f),
            new Vector3(  0f, 0f,  18f),
            new Vector3(-18f, 0f,  18f),
            new Vector3(-18f, 0f,   0f),
        };
        Transform[] shadeWaypointTransforms = new Transform[shadeWPs.Length];
        for (int i = 0; i < shadeWPs.Length; i++)
        {
            GameObject wp = new GameObject("ShadeWP_" + i);
            wp.transform.position = shadeWPs[i];
            wp.transform.parent = shadeWaypointRoot.transform;
            shadeWaypointTransforms[i] = wp.transform;
        }

        // ----------------------------------------------------------------
        // 6. Player
        // ----------------------------------------------------------------
        GameObject player = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        player.name = "Player";
        player.transform.position = new Vector3(0f, 1f, -10f);

        // tag player so raycasts can find them
        player.tag = "Player";

        // add CharacterController (required by PlayerController)
        player.AddComponent<CharacterController>();
        player.AddComponent<PlayerHealth>();
        player.AddComponent<PlayerController>();

        // torch light
        GameObject torchObj = new GameObject("Torch");
        torchObj.transform.parent = player.transform;
        torchObj.transform.localPosition = new Vector3(0.3f, 0.5f, 0.5f);
        Light torchLight = torchObj.AddComponent<Light>();
        torchLight.type = LightType.Point;
        torchLight.range = 8f;
        torchLight.color = new Color(1f, 0.7f, 0.3f);
        torchLight.enabled = false;

        TorchController torch = player.AddComponent<TorchController>();
        torch.torchLight = torchLight;

        // ----------------------------------------------------------------
        // 7. Camera — top-down follow
        // ----------------------------------------------------------------
        GameObject camObj = new GameObject("MainCamera");
        camObj.tag = "MainCamera";
        Camera cam = camObj.AddComponent<Camera>();
        camObj.AddComponent<AudioListener>();
        camObj.AddComponent<TopDownCamera>();
        camObj.transform.position = new Vector3(0f, 20f, 0f);
        camObj.transform.rotation = Quaternion.Euler(90f, 0f, 0f);

        TopDownCamera camScript = camObj.GetComponent<TopDownCamera>();
        camScript.target = player.transform;

        // ----------------------------------------------------------------
        // 8. Canvas + State Labels (TMP)
        // ----------------------------------------------------------------
        GameObject canvas = new GameObject("Canvas");
        Canvas c = canvas.AddComponent<Canvas>();
        c.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.AddComponent<UnityEngine.UI.CanvasScaler>();
        canvas.AddComponent<UnityEngine.UI.GraphicRaycaster>();

        TextMeshProUGUI sentinelLabel = CreateLabel(canvas, "SentinelLabel", new Vector2(10, -10));
        TextMeshProUGUI shadeLabel    = CreateLabel(canvas, "ShadeLabel",    new Vector2(10, -40));

        // ----------------------------------------------------------------
        // 9. Crypt Sentinel
        // ----------------------------------------------------------------
        GameObject sentinelObj = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        sentinelObj.name = "CryptSentinel";
        sentinelObj.transform.position = new Vector3(-5f, 1f, 5f);

        NavMeshAgent sentinelAgent = sentinelObj.AddComponent<NavMeshAgent>();
        sentinelAgent.speed = 1.8f;
        sentinelAgent.radius = 0.4f;
        sentinelAgent.height = 2f;

        VisionCone sentinelVision = sentinelObj.AddComponent<VisionCone>();
        sentinelVision.viewRadius = 9f;
        sentinelVision.viewAngle = 60f;

        CryptSentinel sentinel = sentinelObj.AddComponent<CryptSentinel>();
        sentinel.playerTransform = player.transform;
        sentinel.patrolWaypoints = sentinelWaypointTransforms;
        sentinel.stateLabel = sentinelLabel;

        // wire torch to sentinel
        torch.sentinel = sentinel;

        // ----------------------------------------------------------------
        // 10. The Shade
        // ----------------------------------------------------------------
        GameObject shadeObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        shadeObj.name = "TheShade";
        shadeObj.transform.position = new Vector3(10f, 1f, 10f);

        // make it look a bit ghostly by scaling slightly
        shadeObj.transform.localScale = new Vector3(0.8f, 1.2f, 0.8f);

        NavMeshAgent shadeAgent = shadeObj.AddComponent<NavMeshAgent>();
        shadeAgent.speed = 2.5f;
        shadeAgent.radius = 0.3f;
        shadeAgent.height = 1.8f;

        ShadeBrain shade = shadeObj.AddComponent<ShadeBrain>();
        shade.playerTransform = player.transform;
        shade.patrolWaypoints = shadeWaypointTransforms;
        shade.stateLabel = shadeLabel;

        // ----------------------------------------------------------------
        // 11. Lighting
        // ----------------------------------------------------------------
        GameObject dirLight = new GameObject("DirectionalLight");
        Light dl = dirLight.AddComponent<Light>();
        dl.type = LightType.Directional;
        dl.intensity = 0.3f; // dim — it's a vault
        dirLight.transform.rotation = Quaternion.Euler(50f, -30f, 0f);

        // ----------------------------------------------------------------
        // Done
        // ----------------------------------------------------------------
        Debug.Log("[HollowVault] Scene built! Remember to bake the NavMesh: Window > AI > Navigation > Bake");
        EditorUtility.DisplayDialog("Done!", "Scene built successfully!\n\nNext step: bake the NavMesh\nWindow > AI > Navigation > Bake", "OK");
    }

    // helper — create a wall cube
    static void CreateWall(GameObject parent, string name, Vector3 pos, Vector3 scale)
    {
        GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wall.name = name;
        wall.transform.position = pos;
        wall.transform.localScale = scale;
        wall.transform.parent = parent.transform;
        GameObjectUtility.SetStaticEditorFlags(wall, StaticEditorFlags.NavigationStatic);
    }

    // helper — create a TMP label on the canvas
    static TextMeshProUGUI CreateLabel(GameObject canvas, string name, Vector2 anchoredPos)
    {
        GameObject obj = new GameObject(name);
        obj.transform.parent = canvas.transform;

        TextMeshProUGUI tmp = obj.AddComponent<TextMeshProUGUI>();
        tmp.fontSize = 18;
        tmp.color = Color.white;
        tmp.text = name;

        RectTransform rt = obj.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0, 1);
        rt.anchorMax = new Vector2(0, 1);
        rt.pivot = new Vector2(0, 1);
        rt.anchoredPosition = anchoredPos;
        rt.sizeDelta = new Vector2(300, 30);

        return tmp;
    }
}
