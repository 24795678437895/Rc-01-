using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class RC01GameBootstrap : MonoBehaviour
{
    RC01FlightController flight;
    RC01CameraRig cameraRig;
    Text hud;
    Button cameraButton;
    Button throttleUp, throttleDown;
    Button pitchUp, pitchDown, rollLeft, rollRight;
    Material bodyMat, darkMat, glassMat;
    RC01MissileLauncher launcher;
    RC01CombatSystem combatSystem;
    RC01TargetHealth selectedTarget;
    RC01Targeting targeting;
    Text combatText;
    Text radarText;
    Text missionText;
    Text scoreText;
    RC01MissionManager mission;
    RC01TargetHealth playerHealth;
    bool pitchUpHeld, pitchDownHeld, rollLeftHeld, rollRightHeld;
    readonly System.Collections.Generic.List<Material> ownedMaterials =
        new System.Collections.Generic.List<Material>();
    float nextHudRefreshTime;
    const float HudRefreshInterval = 0.1f;

    void Start()
    {
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;
        // RC-01 is designed for landscape touch controls.
        Screen.orientation = ScreenOrientation.LandscapeLeft;
        Screen.autorotateToPortrait = false;
        Screen.autorotateToPortraitUpsideDown = false;
        Screen.autorotateToLandscapeLeft = true;
        Screen.autorotateToLandscapeRight = true;
        CreateMaterials();
        CreateWorld();
        CreateAircraft();

        if (!flight)
        {
            Debug.LogError("RC-01: Flight controller was not created. Bootstrap stopped.");
            enabled = false;
            return;
        }
        combatSystem = gameObject.AddComponent<RC01CombatSystem>();
        targeting = gameObject.AddComponent<RC01Targeting>();
        if (targeting != null)
            targeting.aircraft = flight.transform;

        mission = gameObject.AddComponent<RC01MissionManager>();
        CreateEnemies();
        CreateCamera();
        CreateUI();
    }

    void CreateMaterials()
    {
        bodyMat = MakeMat(new Color(0.20f, 0.25f, 0.30f), 0.65f);
        darkMat = MakeMat(new Color(0.045f, 0.055f, 0.065f), 0.35f);
        glassMat = MakeMat(new Color(0.08f, 0.22f, 0.30f), 0.18f);
    }

    Material MakeMat(Color c, float metallic)
    {
        Shader shader = Shader.Find("Universal Render Pipeline/Lit");
        if (shader == null) shader = Shader.Find("Standard");
        if (shader == null) shader = Shader.Find("Sprites/Default");

        if (shader == null)
        {
            Debug.LogError("RC-01: No compatible shader was found. Material was not created.");
            return null;
        }

        Material material = new Material(shader);
        material.name = "RC-01 Runtime Material";
        material.color = c;

        if (material.HasProperty("_Metallic"))
            material.SetFloat("_Metallic", metallic);

        ownedMaterials.Add(material);
        return material;
    }

    void CreateWorld()
    {
        RenderSettings.ambientLight = new Color(0.55f, 0.60f, 0.68f);
        GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
        ground.name = "Large training ground";
        ground.transform.position = new Vector3(0, -8, 0);
        ground.transform.localScale = new Vector3(250, 1, 250);
        Renderer groundRenderer = ground.GetComponent<Renderer>();
        Material groundMaterial = MakeMat(new Color(0.22f, 0.30f, 0.20f), 0f);
        if (groundRenderer != null && groundMaterial != null)
            groundRenderer.sharedMaterial = groundMaterial;

        GameObject sun = new GameObject("Sun");
        Light l = sun.AddComponent<Light>();
        l.type = LightType.Directional;
        l.intensity = 1.25f;
        sun.transform.rotation = Quaternion.Euler(48, -30, 0);
    }

    GameObject Part(PrimitiveType type, string n, Transform parent, Vector3 pos, Vector3 scale, Material mat, Vector3 rot)
    {
        GameObject p = GameObject.CreatePrimitive(type);
        p.name = n;
        p.transform.SetParent(parent, false);
        p.transform.localPosition = pos;
        p.transform.localScale = scale;
        p.transform.localRotation = Quaternion.Euler(rot);
        Renderer renderer = p.GetComponent<Renderer>();
        if (renderer != null && mat != null)
            renderer.sharedMaterial = mat;

        Collider c = p.GetComponent<Collider>();
        if (c) Destroy(c);
        return p;
    }

    void CreateAircraft()
    {
        GameObject jet = new GameObject("RC-01 | F-35 inspired prototype");
        jet.transform.position = new Vector3(0, 80, 0);
        jet.transform.rotation = Quaternion.identity;
        Rigidbody rb = jet.AddComponent<Rigidbody>();
        flight = jet.AddComponent<RC01FlightController>();
        playerHealth = jet.AddComponent<RC01TargetHealth>();
        playerHealth.maxHealth = 100f;
        playerHealth.health = 100f;
        playerHealth.scoreValue = 0;

        Part(PrimitiveType.Capsule, "Fuselage", jet.transform, new Vector3(0,0,0), new Vector3(0.72f, 0.9f, 7.2f), bodyMat, new Vector3(90,0,0));
        Part(PrimitiveType.Cube, "Left delta wing", jet.transform, new Vector3(-2.15f,0,-0.15f), new Vector3(3.8f,0.16f,2.7f), bodyMat, new Vector3(0,-15,0));
        Part(PrimitiveType.Cube, "Right delta wing", jet.transform, new Vector3(2.15f,0,-0.15f), new Vector3(3.8f,0.16f,2.7f), bodyMat, new Vector3(0,15,0));
        Part(PrimitiveType.Cube, "Left tailplane", jet.transform, new Vector3(-0.9f,0.25f,-2.7f), new Vector3(1.6f,0.12f,1.2f), darkMat, new Vector3(0,-20,0));
        Part(PrimitiveType.Cube, "Right tailplane", jet.transform, new Vector3(0.9f,0.25f,-2.7f), new Vector3(1.6f,0.12f,1.2f), darkMat, new Vector3(0,20,0));
        Part(PrimitiveType.Cube, "Vertical tail left", jet.transform, new Vector3(-0.65f,0.8f,-2.65f), new Vector3(0.12f,1.5f,1.7f), bodyMat, new Vector3(0,0,-25));
        Part(PrimitiveType.Cube, "Vertical tail right", jet.transform, new Vector3(0.65f,0.8f,-2.65f), new Vector3(0.12f,1.5f,1.7f), bodyMat, new Vector3(0,0,25));
        Part(PrimitiveType.Sphere, "Cockpit canopy", jet.transform, new Vector3(0,0.62f,1.0f), new Vector3(0.82f,0.55f,2.0f), glassMat, Vector3.zero);
        Part(PrimitiveType.Cylinder, "Engine nozzle", jet.transform, new Vector3(0,0,-3.65f), new Vector3(0.42f,0.35f,0.42f), darkMat, new Vector3(90,0,0));

        // Main collision body only.
        CapsuleCollider col = jet.AddComponent<CapsuleCollider>();
        col.direction = 2;
        col.radius = 0.55f;
        col.height = 7.4f;
        launcher = jet.AddComponent<RC01MissileLauncher>();
        GameObject mount = new GameObject("Missile launch point");
        mount.transform.SetParent(jet.transform, false);
        mount.transform.localPosition = new Vector3(0, -0.25f, 3.9f);
        mount.transform.localRotation = Quaternion.identity;
        launcher.launchPoint = mount.transform;
    }

    void CreateEnemies()
    {
        for (int i = 0; i < 3; i++)
        {
            GameObject enemy = GameObject.CreatePrimitive(PrimitiveType.Cube);
            enemy.name = "Enemy aircraft " + (i + 1);
            enemy.transform.position = new Vector3((i - 1) * 90f, 100f + i * 18f, 260f + i * 35f);
            enemy.transform.localScale = new Vector3(4f, 1.2f, 8f);
            Renderer enemyRenderer = enemy.GetComponent<Renderer>();
            if (enemyRenderer != null && darkMat != null)
                enemyRenderer.sharedMaterial = darkMat;
            Rigidbody rb = enemy.AddComponent<Rigidbody>(); rb.useGravity = false; rb.isKinematic = false;
            RC01TargetHealth health = enemy.AddComponent<RC01TargetHealth>();
            health.mission = mission;
            health.scoreValue = 100;
            mission.RegisterTarget(health);
            RC01EnemyAI ai = enemy.AddComponent<RC01EnemyAI>(); ai.player = flight.transform;
            if (i == 0)
            {
                selectedTarget = enemy.GetComponent<RC01TargetHealth>();
                if (launcher != null)
                    launcher.SetTarget(enemy.transform);
            }
        }
    }

    void CreateCamera()
    {
        GameObject camObj = new GameObject("Main Camera");
        Camera cam = camObj.AddComponent<Camera>();
        cam.tag = "MainCamera";
        cam.fieldOfView = 68;
        cameraRig = camObj.AddComponent<RC01CameraRig>();
        if (cameraRig != null)
        {
            cameraRig.aircraft = flight.transform;
            cameraRig.viewCamera = cam;
            cameraRig.cockpitView = false;
        }
        camObj.transform.position = new Vector3(0, 84, -15);
    }

    void CreateUI()
    {
        GameObject canvasObj = new GameObject("RC01 HUD");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;
        canvasObj.AddComponent<GraphicRaycaster>();
        canvasObj.AddComponent<RC01SafeArea>();

        GameObject eventObj = new GameObject("EventSystem");
        eventObj.AddComponent<EventSystem>();
        eventObj.AddComponent<StandaloneInputModule>();

        hud = MakeText(canvas.transform, "HUD", new Vector2(0,1), new Vector2(0,1), new Vector2(24,-24), new Vector2(650,120), 30, TextAnchor.UpperLeft);
        MakeButton(canvas.transform, "CAMERA", new Vector2(1,1), new Vector2(-150,-65), new Vector2(230,85),
            () => { if (cameraRig != null) cameraRig.ToggleView(); });
        MakeButton(canvas.transform, "FIRE MISSILE", new Vector2(1,0), new Vector2(-155,270), new Vector2(250,90), () => { if (combatSystem) combatSystem.TryFire(launcher); });
        MakeButton(canvas.transform, "LOCK TARGET", new Vector2(1,0), new Vector2(-155,370), new Vector2(250,90), () => { if (targeting && targeting.CurrentTarget) launcher.SetTarget(targeting.CurrentTarget); else if (selectedTarget && !selectedTarget.IsDestroyed) launcher.SetTarget(selectedTarget.transform); });
        combatText = MakeText(canvas.transform, "Combat status", new Vector2(0.5f,1), new Vector2(0.5f,1), new Vector2(0,-24), new Vector2(520,70), 26, TextAnchor.UpperCenter);
        missionText = MakeText(canvas.transform, "Mission status", new Vector2(0.5f,1), new Vector2(0.5f,1), new Vector2(0,-82), new Vector2(520,55), 22, TextAnchor.UpperCenter);
        scoreText = MakeText(canvas.transform, "Mission score", new Vector2(0.5f,1), new Vector2(0.5f,1), new Vector2(0,-125), new Vector2(520,45), 20, TextAnchor.UpperCenter);
        if (mission != null)
        {
            mission.missionText = missionText;
            mission.scoreText = scoreText;
            scoreText.text = "SCORE: " + mission.score;
            missionText.text = "TARGETS LEFT: " + mission.targetsRemaining;
        }
        radarText = MakeText(canvas.transform, "Radar", new Vector2(0,1), new Vector2(0,1), new Vector2(24,-155), new Vector2(760,190), 19, TextAnchor.UpperLeft);
        RC01Radar radar = gameObject.AddComponent<RC01Radar>();
        if (radar != null)
        {
            radar.aircraft = flight.transform;
            radar.radarText = radarText;
            radar.rangeMetres = 100000f;
            radar.aesaAzimuthDegrees = 120f;
            radar.sensorFusion360 = true;
            radar.displayedTracks = 5;
        }
        MakeButton(canvas.transform, "THROTTLE +", new Vector2(1,0), new Vector2(-155,160), new Vector2(250,90), () => { if (flight != null) flight.throttle = Mathf.Clamp01(flight.throttle + 0.1f); });
        MakeButton(canvas.transform, "THROTTLE -", new Vector2(1,0), new Vector2(-155,60), new Vector2(250,90), () => { if (flight != null) flight.throttle = Mathf.Clamp01(flight.throttle - 0.1f); });
        MakeHoldButton(canvas.transform, "PITCH UP", new Vector2(0,0), new Vector2(155,160), new Vector2(250,90),
            () => { pitchUpHeld = true; UpdateFlightTouchAxes(); }, () => { pitchUpHeld = false; UpdateFlightTouchAxes(); });
        MakeHoldButton(canvas.transform, "PITCH DOWN", new Vector2(0,0), new Vector2(155,60), new Vector2(250,90),
            () => { pitchDownHeld = true; UpdateFlightTouchAxes(); }, () => { pitchDownHeld = false; UpdateFlightTouchAxes(); });
        MakeHoldButton(canvas.transform, "ROLL LEFT", new Vector2(0,0), new Vector2(420,160), new Vector2(220,90),
            () => { rollLeftHeld = true; UpdateFlightTouchAxes(); }, () => { rollLeftHeld = false; UpdateFlightTouchAxes(); });
        MakeHoldButton(canvas.transform, "ROLL RIGHT", new Vector2(0,0), new Vector2(420,60), new Vector2(220,90),
            () => { rollRightHeld = true; UpdateFlightTouchAxes(); }, () => { rollRightHeld = false; UpdateFlightTouchAxes(); });
    }

    Text MakeText(Transform parent, string n, Vector2 anchor, Vector2 pivot, Vector2 pos, Vector2 size, int fontSize, TextAnchor align)
    {
        GameObject go = new GameObject(n);
        go.transform.SetParent(parent, false);
        RectTransform rt = go.AddComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = anchor; rt.pivot = pivot; rt.anchoredPosition = pos; rt.sizeDelta = size;
        Text t = go.AddComponent<Text>();
        t.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        t.fontSize = fontSize; t.color = Color.white; t.alignment = align;
        t.text = "RC-01 ALPHA";
        return t;
    }

    Button MakeButton(Transform parent, string label, Vector2 anchor, Vector2 pos, Vector2 size, UnityEngine.Events.UnityAction action)
    {
        GameObject go = new GameObject(label);
        go.transform.SetParent(parent, false);
        RectTransform rt = go.AddComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = anchor; rt.pivot = new Vector2(0.5f,0.5f); rt.anchoredPosition = pos; rt.sizeDelta = size;
        Image img = go.AddComponent<Image>(); img.color = new Color(0.04f,0.12f,0.18f,0.78f);
        Button b = go.AddComponent<Button>(); b.targetGraphic = img; b.onClick.AddListener(action);
        Text txt = MakeText(go.transform, "Label", new Vector2(0.5f,0.5f), new Vector2(0.5f,0.5f), Vector2.zero, size, 24, TextAnchor.MiddleCenter);
        txt.text = label;
        return b;
    }

    void MakeHoldButton(Transform parent, string label, Vector2 anchor, Vector2 pos, Vector2 size, System.Action onDown, System.Action onUp)
    {
        Button button = MakeButton(parent, label, anchor, pos, size, () => { });
        EventTrigger trigger = button.gameObject.AddComponent<EventTrigger>();
        EventTrigger.Entry down = new EventTrigger.Entry { eventID = EventTriggerType.PointerDown };
        down.callback.AddListener((data) => onDown());
        trigger.triggers.Add(down);
        EventTrigger.Entry up = new EventTrigger.Entry { eventID = EventTriggerType.PointerUp };
        up.callback.AddListener((data) => onUp());
        trigger.triggers.Add(up);
        EventTrigger.Entry exit = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
        exit.callback.AddListener((data) => onUp());
        trigger.triggers.Add(exit);
    }

    void UpdateFlightTouchAxes()
    {
        if (flight == null) return;

        flight.pitchInput = (pitchUpHeld ? 1f : 0f) - (pitchDownHeld ? 1f : 0f);
        flight.rollInput = (rollRightHeld ? 1f : 0f) - (rollLeftHeld ? 1f : 0f);
    }

    void ResetTouchControls()
    {
        pitchUpHeld = false;
        pitchDownHeld = false;
        rollLeftHeld = false;
        rollRightHeld = false;
        UpdateFlightTouchAxes();
    }

    void OnApplicationPause(bool paused)
    {
        if (paused) ResetTouchControls();
    }

    void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus) ResetTouchControls();
    }

    void OnDisable()
    {
        ResetTouchControls();
    }

    void OnDestroy()
    {
        for (int i = 0; i < ownedMaterials.Count; i++)
        {
            if (ownedMaterials[i] != null)
                Destroy(ownedMaterials[i]);
        }
        ownedMaterials.Clear();
    }

    void Update()
    {
        // Refresh UI text at 10 Hz instead of rebuilding strings every rendered frame.
        if (Time.unscaledTime >= nextHudRefreshTime)
        {
            nextHudRefreshTime = Time.unscaledTime + HudRefreshInterval;

            if (hud != null && flight != null)
            {
                bool cockpit = cameraRig != null && cameraRig.cockpitView;
                float hull = playerHealth != null ? playerHealth.HealthPercent * 100f : 0f;
                hud.text = "RC-01 ALPHA  |  " + (cockpit ? "COCKPIT" : "CHASE") +
                           "\\nSPEED: " + Mathf.RoundToInt(flight.SpeedKmh) + " km/h" +
                           "     ALT: " + Mathf.RoundToInt(flight.Altitude) + " m" +
                           "\\nTHROTTLE: " + Mathf.RoundToInt(flight.throttle * 100) + "%" +
                           "   |   HULL: " + Mathf.RoundToInt(hull) + "%";
            }

            if (combatText != null && launcher != null)
            {
                float hull = playerHealth != null ? playerHealth.HealthPercent * 100f : 0f;
                combatText.text = "MISSILES: " + launcher.missilesAvailable +
                                  "   |   TARGET: " + (launcher.target != null ? "LOCKED" : "NONE") +
                                  "   |   HULL: " + Mathf.RoundToInt(hull) + "%";
            }

            if (missionText != null && mission != null)
            {
                missionText.text = mission.targetsRemaining > 0
                    ? "TARGETS LEFT: " + mission.targetsRemaining
                    : "MISSION COMPLETE";
            }
        }

        // Keep the player's manual lock; only clear it when the target is invalid.
        if (launcher != null && launcher.target != null)
        {
            RC01TargetHealth lockedHealth = launcher.target.GetComponent<RC01TargetHealth>();
            if (lockedHealth == null || lockedHealth.IsDestroyed)
                launcher.SetTarget(null);
        }
    }
}
