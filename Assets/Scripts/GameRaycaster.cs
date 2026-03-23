using UnityEngine;
using CatCode.InteractionLocking;
using System;

public class GameRaycaster : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private float _maxDistance = 10;
    [SerializeField] private LayerMask _layerMask;
    [Space]
    [SerializeField] private InteractionLocker _locker;
    [SerializeField] private InteractionLayerAsset _layer;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var ray = _camera.ScreenPointToRay(Input.mousePosition);

            var blockedMask = _locker.LockData.LayerMask;
            var mask = _layerMask & ~blockedMask;

            if (Physics.Raycast(ray, out var hit, _maxDistance, mask))
            {
                if (hit.collider.gameObject.TryGetComponent<InteractionObject>(out var comp))
                    comp.Play();
            }
        }
    }
}
