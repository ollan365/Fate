using UnityEngine;


namespace Fate.Utilities
{
    public class CameraSmoother : MonoBehaviour
    {
        public static CameraSmoother Instance { get; private set; }

        [Header("Smoothing")]
        [SerializeField] float posSmoothTime = 0.4f;
        [SerializeField] float sizeSmoothTime = 0.4f;

        [Header("Links")]
        [SerializeField] Camera mainCam;
        [SerializeField] Camera cameraAfterBlur; // FollowManager.Instance.CameraAfterBlur 대신 직접 참조

        // 현재/목표 상태
        Vector3 _targetPos;
        float _targetSize;

        // 스무딩용 속도
        Vector3 _posVel;
        float _sizeVel;

        // 오버라이드(줌 등 최우선 요청)
        bool _hasOverride;
        float _overrideUntil; // Time.time 기준
        Vector3 _overridePos;
        float _overrideSize;

        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;

            if (mainCam == null) mainCam = Camera.main;
            if (mainCam != null)
            {
                _targetPos = mainCam.transform.position;
                _targetSize = mainCam.orthographicSize;
            }
        }

        void LateUpdate()
        {
            if (mainCam == null) return;

            // 오버라이드 유효성 갱신
            if (_hasOverride && Time.time > _overrideUntil)
                _hasOverride = false;

            // 이번 프레임 목표
            Vector3 desiredPos = _hasOverride ? _overridePos : _targetPos;
            float desiredSize = _hasOverride ? _overrideSize : _targetSize;

            // 스무딩 적용
            var nextPos = new Vector3(
                Mathf.SmoothDamp(mainCam.transform.position.x, desiredPos.x, ref _posVel.x, posSmoothTime),
                Mathf.SmoothDamp(mainCam.transform.position.y, desiredPos.y, ref _posVel.y, posSmoothTime),
                Mathf.SmoothDamp(mainCam.transform.position.z, desiredPos.z, ref _posVel.z, posSmoothTime)
            );
            float nextSize = Mathf.SmoothDamp(mainCam.orthographicSize, desiredSize, ref _sizeVel, sizeSmoothTime);

            mainCam.transform.position = nextPos;
            mainCam.orthographicSize = nextSize;

            if (cameraAfterBlur != null)
                cameraAfterBlur.orthographicSize = mainCam.orthographicSize;
        }

        // 일반 목표 등록(매 프레임 호출해도 됨)
        public void SetTarget(Vector3 pos, float size)
        {
            _targetPos = pos;
            _targetSize = size;
        }

        // 오버라이드 요청: duration 동안 최우선 목표로 사용
        public void SetOverride(Vector3 pos, float size, float duration, float? posTime = null, float? sizeTime = null)
        {
            _hasOverride = true;
            _overrideUntil = Time.time + Mathf.Max(0f, duration);
            _overridePos = pos;
            _overrideSize = size;
            if (posTime.HasValue) posSmoothTime = Mathf.Max(0.0001f, posTime.Value);
            if (sizeTime.HasValue) sizeSmoothTime = Mathf.Max(0.0001f, sizeTime.Value);
        }

        // 강제로 오버라이드 해제(게임오버 연출 종료 등)
        public void ClearOverride()
        {
            _hasOverride = false;
        }
    }
}
