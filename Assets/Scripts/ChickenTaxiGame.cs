using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

// Chicken Taxi - self-contained 2D endless gameplay prototype.
// Compatible with Unity 2022.3 LTS and the built-in uGUI / legacy Input Manager.
// No scene objects or third-party packages are required: it bootstraps itself at runtime.
public sealed class ChickenTaxiGame : MonoBehaviour
{
    enum Mode { Menu, Playing, Paused }
    const float Cell = 7f;
    const int Grid = 17;
    Mode mode = Mode.Menu;
    Camera cam;
    Transform taxi;
    SpriteRenderer[,] ground = new SpriteRenderer[Grid, Grid];
    Sprite white, circle;
    GameObject pickup, destination, menuPanel, pausePanel, hud;
    Text scoreLabel, missionLabel, tipLabel, arrowLabel, upgradeLabel;
    Button upgradeButton;
    AudioSource sound;
    int lastCellX = int.MinValue, lastCellY = int.MinValue;
    int coins, score, jobs, upgradeLevel;
    float speed, heading, missionClock;
    bool turnLeft, turnRight, delivering;
    string missionName;
    Vector3 goal;
    float feedbackClock;
    string feedback = "";
    
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Bootstrap()
    {
        if (FindObjectOfType<ChickenTaxiGame>() == null)
        {
            GameObject root = new GameObject("Chicken Taxi - Runtime");
            root.AddComponent<ChickenTaxiGame>();
        }
    }

    void Awake()
    {
        Application.targetFrameRate = 60;
        Screen.orientation = ScreenOrientation.Portrait;
        coins = PlayerPrefs.GetInt("ChickenTaxiCoins", 0);
        upgradeLevel = PlayerPrefs.GetInt("ChickenTaxiUpgrade", 0);
        white = MakeSolid();
        circle = MakeCircle();
        GameObject cameraGO = new GameObject("Camera");
        cameraGO.transform.SetParent(transform);
        cam = cameraGO.AddComponent<Camera>();
        cam.tag = "MainCamera";
        cam.orthographic = true;
        cam.orthographicSize = 14f;
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = new Color(.48f, .69f, .43f);
        cameraGO.AddComponent<AudioListener>();
        cameraGO.transform.position = new Vector3(0, 0, -30);
        sound = gameObject.AddComponent<AudioSource>();
        MakeWorld();
        MakeTaxi();
        MakeInterface();
        ShowMenu();
    }

    Sprite MakeSolid()
    {
        Texture2D tex = new Texture2D(1, 1, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Point;
        tex.SetPixel(0, 0, Color.white);
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, 1, 1), new Vector2(.5f, .5f), 1f);
    }
    Sprite MakeCircle()
    {
        const int n = 64;
        Texture2D tex = new Texture2D(n, n, TextureFormat.RGBA32, false);
        for (int y = 0; y < n; y++)
            for (int x = 0; x < n; x++)
            {
                float dx = (x - 31.5f) / 31.5f, dy = (y - 31.5f) / 31.5f;
                tex.SetPixel(x, y, dx * dx + dy * dy < 1 ? Color.white : Color.clear);
            }
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, n, n), new Vector2(.5f, .5f), n);
    }
    SpriteRenderer Shape(string name, Transform parent, Sprite image, Vector3 local, Vector3 scale, Color color, int sorting)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);
        go.transform.localPosition = local;
        go.transform.localScale = scale;
        SpriteRenderer renderer = go.AddComponent<SpriteRenderer>();
        renderer.sprite = image;
        renderer.color = color;
        renderer.sortingOrder = sorting;
        return renderer;
    }

    void MakeWorld()
    {
        GameObject tiles = new GameObject("Repeating Procedural Road Grid");
        tiles.transform.SetParent(transform);
        for (int x = 0; x < Grid; x++)
            for (int y = 0; y < Grid; y++)
                ground[x, y] = Shape("Tile", tiles.transform, white, Vector3.zero, new Vector3(Cell - .08f, Cell - .08f, 1), Color.gray, -20);
        RefreshWorld(true);
    }
    static bool Road(int x, int y) { return Mod(x, 3) == 0 || Mod(y, 3) == 0; }
    static int Mod(int x, int d) { return ((x % d) + d) % d; }
    void RefreshWorld(bool force = false)
    {
        Vector3 p = taxi ? taxi.position : Vector3.zero;
        int cx = Mathf.RoundToInt(p.x / Cell), cy = Mathf.RoundToInt(p.y / Cell);
        if (!force && cx == lastCellX && cy == lastCellY) return;
        lastCellX = cx; lastCellY = cy;
        for (int x = 0; x < Grid; x++)
            for (int y = 0; y < Grid; y++)
            {
                int gx = cx + x - Grid / 2, gy = cy + y - Grid / 2;
                bool road = Road(gx, gy);
                SpriteRenderer tile = ground[x, y];
                tile.transform.localPosition = new Vector3(gx * Cell, gy * Cell, 0);
                tile.color = road ? new Color(.23f, .28f, .32f) :
                    ((gx + gy) & 1) == 0 ? new Color(.37f, .67f, .39f) : new Color(.32f, .61f, .34f);
            }
    }
    void MakeTaxi()
    {
        taxi = new GameObject("Yellow Chicken Taxi").transform;
        taxi.SetParent(transform);
        Shape("Body", taxi, white, Vector3.zero, new Vector3(2.15f, 3.45f, 1), new Color(1f, .76f, .04f), 5);
        Shape("Windshield", taxi, white, new Vector3(0, .75f, 0), new Vector3(1.65f, .78f, 1), new Color(.25f, .72f, .91f), 6);
        Shape("Rear window", taxi, white, new Vector3(0, -.91f, 0), new Vector3(1.65f, .61f, 1), new Color(.11f, .37f, .55f), 6);
        Shape("Roof sign", taxi, white, new Vector3(0, -.08f, 0), new Vector3(1.12f, .5f, 1), Color.white, 7);
        for (int s = -1; s <= 1; s += 2)
        {
            Shape("Front wheel", taxi, white, new Vector3(1.13f * s, 1.08f, 0), new Vector3(.35f, .79f, 1), Color.black, 4);
            Shape("Rear wheel", taxi, white, new Vector3(1.13f * s, -1.04f, 0), new Vector3(.35f, .79f, 1), Color.black, 4);
        }
    }

    Font BuiltinFont()
    {
        Font font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        if (font == null) font = Font.CreateDynamicFontFromOSFont("Arial", 28);
        return font;
    }
    RectTransform Place(GameObject go, Vector2 anchor, Vector2 at, Vector2 size)
    {
        RectTransform rect = go.AddComponent<RectTransform>();
        rect.anchorMin = rect.anchorMax = anchor;
        rect.pivot = new Vector2(.5f, .5f);
        rect.anchoredPosition = at;
        rect.sizeDelta = size;
        return rect;
    }
    Text Label(Transform parent, string name, string contents, int fontSize, Vector2 anchor, Vector2 pos, Vector2 size)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);
        Place(go, anchor, pos, size);
        Text label = go.AddComponent<Text>();
        label.font = BuiltinFont();
        label.text = contents;
        label.fontSize = fontSize;
        label.color = Color.white;
        label.alignment = TextAnchor.MiddleCenter;
        label.horizontalOverflow = HorizontalWrapMode.Wrap;
        return label;
    }
    Button MakeButton(Transform parent, string label, Vector2 anchor, Vector2 pos, Vector2 size, Color color, Action action)
    {
        GameObject go = new GameObject(label);
        go.transform.SetParent(parent, false);
        Place(go, anchor, pos, size);
        Image image = go.AddComponent<Image>();
        image.color = color;
        Button button = go.AddComponent<Button>();
        button.targetGraphic = image;
        if (action != null) button.onClick.AddListener(() => action());
        Text caption = Label(go.transform, "Caption", label, 36, new Vector2(.5f, .5f), Vector2.zero, size);
        caption.raycastTarget = false;
        return button;
    }
    GameObject MakePanel(Transform canvas, string name, Color background)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(canvas, false);
        RectTransform rect = go.AddComponent<RectTransform>();
        rect.anchorMin = Vector2.zero; rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero; rect.offsetMax = Vector2.zero;
        Image bg = go.AddComponent<Image>(); bg.color = background;
        return go;
    }
    void AddHold(Button button, bool left)
    {
        EventTrigger trigger = button.gameObject.AddComponent<EventTrigger>();
        EventTrigger.Entry down = new EventTrigger.Entry { eventID = EventTriggerType.PointerDown };
        down.callback.AddListener(data => { if (left) turnLeft = true; else turnRight = true; });
        trigger.triggers.Add(down);
        EventTrigger.Entry up = new EventTrigger.Entry { eventID = EventTriggerType.PointerUp };
        up.callback.AddListener(data => { if (left) turnLeft = false; else turnRight = false; });
        trigger.triggers.Add(up);
        EventTrigger.Entry exit = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
        exit.callback.AddListener(data => { if (left) turnLeft = false; else turnRight = false; });
        trigger.triggers.Add(exit);
    }

    void MakeInterface()
    {
        GameObject canvasGO = new GameObject("Canvas");
        canvasGO.transform.SetParent(transform, false);
        Canvas canvas = canvasGO.AddComponent<Canvas>(); canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1080, 1920);
        scaler.matchWidthOrHeight = .5f;
        canvasGO.AddComponent<GraphicRaycaster>();
        GameObject events = new GameObject("UI EventSystem");
        events.transform.SetParent(transform);
        events.AddComponent<EventSystem>();
        events.AddComponent<StandaloneInputModule>();

        hud = MakePanel(canvasGO.transform, "Gameplay HUD", new Color(0,0,0,0));
        scoreLabel = Label(hud.transform, "Score", "0", 42, new Vector2(.5f, 1), new Vector2(0, -72), new Vector2(550, 80));
        missionLabel = Label(hud.transform, "Mission", "Find a chicken!", 33, new Vector2(.5f, 1), new Vector2(0, -173), new Vector2(900, 120));
        tipLabel = Label(hud.transform, "Help", "Drive to the yellow chicken", 30, new Vector2(.5f, 1), new Vector2(0, -263), new Vector2(800, 75));
        arrowLabel = Label(hud.transform, "Navigation", "▲", 82, new Vector2(.5f, .5f), new Vector2(0, 320), new Vector2(170, 170));
        MakeButton(hud.transform, "PAUSE", new Vector2(0, 1), new Vector2(140, -72), new Vector2(230, 96), new Color(.18f, .28f, .36f), Pause);
        upgradeButton = MakeButton(hud.transform, "UPGRADE", new Vector2(1, 1), new Vector2(-158, -72), new Vector2(295, 96), new Color(.12f, .46f, .35f), Upgrade);
        upgradeLabel = upgradeButton.GetComponentInChildren<Text>();
        Button left = MakeButton(hud.transform, "◀", new Vector2(0, 0), new Vector2(175, 180), new Vector2(270, 245), new Color(.08f,.14f,.23f,.85f), null);
        Button right = MakeButton(hud.transform, "▶", new Vector2(1, 0), new Vector2(-175, 180), new Vector2(270, 245), new Color(.08f,.14f,.23f,.85f), null);
        AddHold(left, true); AddHold(right, false);

        menuPanel = MakePanel(canvasGO.transform, "Main Menu", new Color(.045f, .10f, .16f, .96f));
        Label(menuPanel.transform, "Title", "CHICKEN\nTAXI", 115, new Vector2(.5f,.5f), new Vector2(0, 360), new Vector2(850, 330));
        Label(menuPanel.transform, "Description", "Endless chicken deliveries\nEarn coins. Upgrade. Keep driving!", 38, new Vector2(.5f,.5f), new Vector2(0, 80), new Vector2(860, 200));
        MakeButton(menuPanel.transform, "PLAY", new Vector2(.5f,.5f), new Vector2(0, -205), new Vector2(650, 145), new Color(.98f,.69f,.08f), StartGame);
        Label(menuPanel.transform, "Controls", "Steer with left / right buttons or keyboard arrows", 30, new Vector2(.5f,0), new Vector2(0, 125), new Vector2(900, 120));

        pausePanel = MakePanel(canvasGO.transform, "Pause Overlay", new Color(.045f, .10f, .16f, .93f));
        Label(pausePanel.transform, "Paused", "PAUSED", 90, new Vector2(.5f,.5f), new Vector2(0, 280), new Vector2(850, 140));
        MakeButton(pausePanel.transform, "RESUME", new Vector2(.5f,.5f), new Vector2(0, 10), new Vector2(650, 142), new Color(.12f,.53f,.43f), Resume);
        MakeButton(pausePanel.transform, "MAIN MENU", new Vector2(.5f,.5f), new Vector2(0, -215), new Vector2(650, 140), new Color(.36f,.41f,.49f), ShowMenu);
        UpdateLabels();
    }

    void StartGame()
    {
        mode = Mode.Playing;
        menuPanel.SetActive(false); pausePanel.SetActive(false); hud.SetActive(true);
        taxi.position = Vector3.zero;
        taxi.rotation = Quaternion.identity;
        heading = speed = 0; jobs = 0; score = 0;
        delivering = false;
        ClearMarker(ref pickup); ClearMarker(ref destination);
        SpawnPassenger();
        RefreshWorld(true);
        UpdateLabels();
    }
    void Pause()
    {
        if (mode != Mode.Playing) return;
        mode = Mode.Paused; pausePanel.SetActive(true); hud.SetActive(false);
        turnLeft = turnRight = false;
    }
    void Resume()
    {
        mode = Mode.Playing; pausePanel.SetActive(false); hud.SetActive(true);
    }
    void ShowMenu()
    {
        mode = Mode.Menu; menuPanel.SetActive(true); hud.SetActive(false); pausePanel.SetActive(false);
        ClearMarker(ref pickup); ClearMarker(ref destination);
    }

    Vector3 RoadPositionNear(Vector3 around, float distance)
    {
        float angle = UnityEngine.Random.Range(0f, Mathf.PI * 2);
        Vector3 raw = around + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * distance;
        float x, y;
        if (UnityEngine.Random.value < .5f)
        {
            x = Mathf.Round(raw.x / (Cell * 3)) * (Cell * 3);
            y = Mathf.Round(raw.y / Cell) * Cell;
        }
        else
        {
            x = Mathf.Round(raw.x / Cell) * Cell;
            y = Mathf.Round(raw.y / (Cell * 3)) * (Cell * 3);
        }
        return new Vector3(x, y, 0);
    }
    GameObject MakeMarker(string name, Vector3 position, Color color, bool passenger)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(transform);
        go.transform.position = position;
        Shape("Highlight", go.transform, circle, Vector3.zero, new Vector3(4.0f,4.0f,1), new Color(color.r,color.g,color.b,.45f), 2);
        Shape("Body", go.transform, circle, new Vector3(0,0,-.1f), new Vector3(2.6f,2.6f,1), color, 3);
        if (passenger)
        {
            Shape("Eye left", go.transform, circle, new Vector3(-.5f,.36f,-.2f), new Vector3(.35f,.35f,1), Color.black, 5);
            Shape("Eye right", go.transform, circle, new Vector3(.5f,.36f,-.2f), new Vector3(.35f,.35f,1), Color.black, 5);
            Shape("Beak", go.transform, white, new Vector3(0,-.36f,-.2f), new Vector3(.8f,.36f,1), new Color(1,.45f,0), 6);
        }
        return go;
    }
    void ClearMarker(ref GameObject marker)
    {
        if (marker != null) Destroy(marker);
        marker = null;
    }
    void SpawnPassenger()
    {
        delivering = false;
        goal = RoadPositionNear(taxi.position, UnityEngine.Random.Range(21f,45f));
        if (Vector3.Distance(goal,taxi.position)<12) goal += new Vector3(21,0,0);
        pickup = MakeMarker("Waiting Chicken", goal, new Color(1f,.83f,.2f), true);
        missionName = "PICK UP THE CHICKEN";
        UpdateLabels();
    }
    void PickUp()
    {
        ClearMarker(ref pickup);
        delivering = true;
        string[] names = { "EGG EXPRESS", "RAIN RACE", "VIP CHICKEN", "SEED DELIVERY" };
        missionName = names[UnityEngine.Random.Range(0,names.Length)];
        goal = RoadPositionNear(taxi.position, UnityEngine.Random.Range(35f,72f));
        if (Vector3.Distance(goal,taxi.position)<20) goal += new Vector3(28,0,0);
        destination = MakeMarker("Delivery Destination", goal, new Color(.18f,1f,.55f), false);
        missionClock = Mathf.Max(25f, Vector3.Distance(taxi.position,goal)*2.5f + 18f - jobs*.5f);
        Sound(700f);
        feedback = "Passenger on board!"; feedbackClock = 2.5f;
        UpdateLabels();
    }
    void Deliver()
    {
        ClearMarker(ref destination);
        delivering = false; jobs++; score += 100 + Mathf.RoundToInt(missionClock);
        int earned = 10 + Mathf.Min(15, jobs / 3);
        coins += earned;
        PlayerPrefs.SetInt("ChickenTaxiCoins", coins);
        PlayerPrefs.Save();
        Sound(950f);
        feedback = "DELIVERED! +" + earned + " COINS";
        feedbackClock = 2.8f;
        SpawnPassenger();
    }
    void Failed()
    {
        ClearMarker(ref destination);
        delivering = false;
        feedback = "TIME UP - NEXT PASSENGER!";
        feedbackClock = 2.7f;
        SpawnPassenger();
    }
    void Upgrade()
    {
        int cost = 30 + 25 * upgradeLevel;
        if (coins < cost) { feedback = "Need " + cost + " coins to upgrade"; feedbackClock = 2; return; }
        coins -= cost;
        upgradeLevel++;
        PlayerPrefs.SetInt("ChickenTaxiCoins", coins);
        PlayerPrefs.SetInt("ChickenTaxiUpgrade", upgradeLevel);
        PlayerPrefs.Save();
        feedback = "TAXI UPGRADED!";
        feedbackClock = 2;
        Sound(1100f);
        UpdateLabels();
    }
    void Sound(float freq)
    {
        int samples = 3500;
        AudioClip clip = AudioClip.Create("Chicken Taxi blip", samples, 1, 22050, false);
        float[] data = new float[samples];
        for (int i = 0; i < samples; i++)
        {
            float k = i / (float)samples;
            data[i] = Mathf.Sin(2*Mathf.PI*freq*i/22050f) * .2f * (1f-k);
        }
        clip.SetData(data,0);
        sound.PlayOneShot(clip);
        Destroy(clip, 2);
    }

    void UpdateLabels()
    {
        if (scoreLabel == null) return;
        scoreLabel.text = "COINS " + coins + "   SCORE " + score;
        if (missionLabel != null)
            missionLabel.text = missionName + (delivering ? "   " + Mathf.CeilToInt(missionClock) + "s" : "");
        if (upgradeLabel != null)
            upgradeLabel.text = "UPGRADE " + (30 + upgradeLevel * 25);
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (mode == Mode.Playing) Pause();
            else if (mode == Mode.Paused) Resume();
        }
        if (mode != Mode.Playing) return;
        float steering = 0;
        if (turnLeft || Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A)) steering += 1;
        if (turnRight || Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D)) steering -= 1;
        heading += steering * 138f * Time.deltaTime;
        float maxSpeed = 8f + upgradeLevel * 1.0f;
        speed = Mathf.MoveTowards(speed, maxSpeed, (5.2f + upgradeLevel*.35f)*Time.deltaTime);
        taxi.rotation = Quaternion.Euler(0,0,heading);
        taxi.position += taxi.up * speed * Time.deltaTime;
        cam.transform.position = Vector3.Lerp(cam.transform.position, taxi.position + Vector3.back * 30, Time.deltaTime * 6);
        RefreshWorld();
        if (delivering)
        {
            missionClock -= Time.deltaTime;
            if (missionClock <= 0) Failed();
            else if (Vector3.Distance(taxi.position, goal)<3.6f) Deliver();
        }
        else if (Vector3.Distance(taxi.position, goal)<3.6f) PickUp();
        if (feedbackClock > 0) feedbackClock -= Time.deltaTime;
        UpdateLabels();
        tipLabel.text = feedbackClock > 0 ? feedback : (delivering ? "Deliver the chicken to GREEN" : "Pick up the YELLOW chicken");
        Vector3 delta = goal - taxi.position;
        float angle = Mathf.Atan2(delta.y, delta.x)*Mathf.Rad2Deg - 90f;
        arrowLabel.rectTransform.localRotation = Quaternion.Euler(0,0,angle);
    }
    void OnApplicationPause(bool paused)
    {
        if (paused && mode == Mode.Playing) Pause();
    }
}