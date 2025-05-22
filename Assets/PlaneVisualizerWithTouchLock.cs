using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

using UniRx;

[RequireComponent(typeof(ARPlaneManager), typeof(ARRaycastManager))]
public class PlaneVisualizerWithTouchLock : MonoBehaviour
{
    private ARPlaneManager planeManager;
    private ARRaycastManager raycastManager;

    private HashSet<ARPlane> initializedPlanes = new HashSet<ARPlane>();
    private Dictionary<ARPlane, Material> planeMaterials = new Dictionary<ARPlane, Material>();
    private static List<ARRaycastHit> hits = new List<ARRaycastHit>();

    public Font textFont;
    public float textScale = 0.03f;


    public Button[] btnOnOff;

    private bool planeLocked = false;


    public Text[] txtStatPlane;

    public event Action<bool> OnPlaneManagerEnabledChanged;
    private bool lastState;

    public MeshRenderer renderer;

    void Awake()
    {
        planeManager = GetComponent<ARPlaneManager>();
        raycastManager = GetComponent<ARRaycastManager>();

        lastState = planeManager.enabled;

        txtStatPlane[0].text = "1";
        txtStatPlane[1].text = "2";
        txtStatPlane[2].text = "3";
    }

    void Start()
    {
        AddButtonEvtListener();
    }

    void ApplyRandomColorRenderer(MeshRenderer renderer)
    {
        //var renderer = plane.GetComponent<MeshRenderer>();
        if (renderer != null)
        {
            Color randColor = new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value, 0.7f);
            Material newMat = new Material(renderer.material); // 복사해서 따로 사용
            newMat.color = randColor;
            renderer.material = newMat;
            //planeMaterials[plane] = newMat; // 추후 조정용 저장
        }
    }

    void AddButtonEvtListener()
    {
        foreach (Button btn in btnOnOff)
        {
            int index = Array.IndexOf(btnOnOff, btn);

            btn.onClick.AsObservable()
           .Subscribe(_ =>
           {
               //Debug.Log("Click");
               if (index == 0) TogglePlaneEnable();
               else if (index == 1) RemovePlanes();
           });
        }
    }

    private void TogglePlaneEnable()
    {
        planeManager.enabled = !planeManager.enabled;
    }

    private void RemovePlanes()
    {
        //planeManager.enabled = false;

        foreach (var plane in planeManager.trackables)
        {
            Destroy(plane.gameObject);
        }

        Debug.Log("📴  Plane 제거 완료");
    }

    void OnEnable()
    {
        if (planeManager != null)
        {
            planeManager.planesChanged += OnPlanesChanged;

            this.OnPlaneManagerEnabledChanged += HandlePlaneDetectionToggle;
        }
    }

    void OnDisable()
    {
        if (planeManager != null)
        {
            planeManager.planesChanged -= OnPlanesChanged;
            this.OnPlaneManagerEnabledChanged -= HandlePlaneDetectionToggle;
        }
    }

    void HandlePlaneDetectionToggle(bool isEnabled)
    {
        Debug.Log($"📡 Plane 감지 상태: {(isEnabled ? "활성화" : "비활성화")}");
        
        txtStatPlane[2].text = $"📡 Plane 감지 상태: {(isEnabled ? "활성화" : "비활성화")}";
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            //SceneManager.LoadScene("testEmptySingleton");
            //permission.PressBtnCapture();
             ApplyRandomColorRenderer(renderer);
        }

        //if (planeLocked) return;

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            Vector2 touchPos = Input.GetTouch(0).position;
            if (raycastManager.Raycast(touchPos, hits, TrackableType.Planes))
            {
                var planeHit = planeManager.GetPlane(hits[0].trackableId);
                if (planeHit != null)
                {
                    LockThisPlane(planeHit);
                }
            }
        }

        if (planeManager.enabled != lastState)
        {
            lastState = planeManager.enabled;
            Debug.Log($"[PlaneManager] 상태 변경됨: {lastState}");

            txtStatPlane[2].text = $"[PlaneManager] enable 상태 변경됨: {lastState}";

            OnPlaneManagerEnabledChanged?.Invoke(lastState);
        }
    }

    void OnPlanesChanged(ARPlanesChangedEventArgs args)
    {

        if (args.added.Count > 0)
        {

            foreach (var plane in args.added)
            {
                if (!initializedPlanes.Contains(plane))
                {
                    ApplyRandomColor(plane);
                    CreatePlaneLabel(plane);
                    initializedPlanes.Add(plane);

                }
            }
            
            foreach (ARPlane plane in args.added)
            {
                Debug.Log($"[Plane Detected] Plane Added: {plane.trackableId}");
                // 여기에 'Plane이 준비되었다'는 로직 삽입

                txtStatPlane[0].text = $"[Plane Detected] Plane Added: {plane.trackableId}";
            }

            // 업데이트된 평면
            foreach (ARPlane plane in args.updated)
            {
                Debug.Log($"[Plane Updated] {plane.trackableId}");
                txtStatPlane[1].text = $"[Plane Updated] Plane Updated: {plane.trackableId}";
            }

            // 제거된 평면
            foreach (ARPlane plane in args.removed)
            {
                //Debug.Log($"[Plane Removed] {plane.trackableId}");
                //txtStatPlane[2].text = $"[Plane Removed] Plane Removed: {plane.trackableId}";
            }
        }
    }

    void ApplyRandomColor(ARPlane plane)
    {
        var renderer = plane.GetComponent<MeshRenderer>();
        if (renderer != null)
        {
            Color randColor = new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value, 0.7f);
            Material newMat = new Material(renderer.material); // 복사해서 따로 사용
            newMat.color = randColor;
            renderer.material = newMat;
            //planeMaterials[plane] = newMat; // 추후 조정용 저장
        }
    }

    void CreatePlaneLabel(ARPlane plane)
    {
        GameObject textObj = new GameObject("PlaneID_Label");
        textObj.transform.SetParent(plane.transform);
        textObj.transform.localPosition = Vector3.zero;

        TextMesh textMesh = textObj.AddComponent<TextMesh>();
        textMesh.text = plane.trackableId.ToString().Substring(0, 8);
        textMesh.fontSize = 100;
        textMesh.characterSize = 0.05f;
        textMesh.anchor = TextAnchor.MiddleCenter;
        textMesh.alignment = TextAlignment.Center;
        textMesh.color = Color.black;

        if (textFont != null)
            textMesh.font = textFont;

        textObj.transform.localScale = Vector3.one * textScale;
    }

    void LockThisPlane(ARPlane selectedPlane)
    {
        Debug.Log($"🔒 Plane Locked: {selectedPlane.trackableId}");
        planeLocked = true;

        /*
        foreach (var plane in planeManager.trackables)
        {
            if (plane != selectedPlane)
            {
                plane.gameObject.SetActive(false);
            }
        }*/

        // 알파값 0.9로 강조
        if (planeMaterials.ContainsKey(selectedPlane))
        {
            var mat = planeMaterials[selectedPlane];
            Color c = mat.color;
            c.a = 0.9f;
            mat.color = c;
            selectedPlane.GetComponent<MeshRenderer>().material = mat;
        }

        // Plane 감지 종료
        //planeManager.enabled = false;
    }
}

