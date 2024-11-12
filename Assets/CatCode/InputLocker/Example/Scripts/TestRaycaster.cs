using UnityEngine;
using CatCode;

public class TestRaycaster : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private float _maxDistance = 10;
    [SerializeField] private LayerMask _layerMask;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var ray = _camera.ScreenPointToRay(Input.mousePosition);
            var mask = InputLockManager.Instance.ApplyMask(_layerMask);
            if (Physics.Raycast(ray, out var hit, _maxDistance, mask))
                Debug.Log("Clicked on:" + hit.collider.gameObject.name);
        }
    }
}
