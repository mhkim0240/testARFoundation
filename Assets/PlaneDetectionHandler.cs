using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections.Generic;
using UnityEngine.UI;

public class PlaneDetectionHandler : MonoBehaviour
{
    [SerializeField]
    private ARPlaneManager planeManager;

    public Text[] txtStatPlane;


    void Awake()
    {
        // 자동 참조
        //planeManager = GetComponent<ARPlaneManager>();

        if (planeManager == null)
        {
            Debug.LogError("❌ ARPlaneManager 컴포넌트를 찾을 수 없습니다!");
        }

        txtStatPlane[0].text = "1";
        txtStatPlane[1].text = "2";
        txtStatPlane[2].text = "3";
    }

    void OnEnable()
    {
        if (planeManager != null)
        {
            planeManager.planesChanged += OnPlanesChanged;
        }
    }

    void OnDisable()
    {
        if (planeManager != null)
        {
            planeManager.planesChanged -= OnPlanesChanged;
        }
    }

    private void OnPlanesChanged(ARPlanesChangedEventArgs args)
    {

        if (args.added.Count > 0)
        {

            // 새롭게 감지된 평면
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
                txtStatPlane[1].text = $"[Plane Updated] Plane Added: {plane.trackableId}";
            }

            // 제거된 평면
            foreach (ARPlane plane in args.removed)
            {
                Debug.Log($"[Plane Removed] {plane.trackableId}");
                txtStatPlane[2].text = $"[Plane Removed] Plane Added: {plane.trackableId}";
            }
        }
    }
}
