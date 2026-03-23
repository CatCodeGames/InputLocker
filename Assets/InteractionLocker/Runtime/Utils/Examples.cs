//using Cysharp.Threading.Tasks;
//using System;
//using UnityEngine;
//using UnityEngine.EventSystems;
//namespace CatCode.InteractionLocking
//{
//    public class Examples
//    {
//        private IInteractionLockerService _interactionLocker;
//        [SerializeField] private InteractionLayerAsset _interactionLayer;

//        private InteractionLayersAsset _layers;
        
//        public void Create() 
//        {
//            IInteractionLockerService service =  InteractionLockerService.Create(_layers);
//        }

//        public void Example()
//        {
//            // заблокировать интерактивный слой
//            var lockHandle = _interactionLocker.Lock(_interactionLayer);
//            // или
//            var lockHandle = _interactionLocker.Lock(_interactionLayer.InteractionMask);

//            // снять/отменить блокирвоку
//            lockHandle.Dispose();


//            // получить текущие значение масок
//            var data = _interactionLocker.LockData;
//            InteractionLayerMask interactionMask = data.Value.InteractionMask;
//            LayerMask layerMask = data.Value.LayerMask;

//            // подписка на изменение маски
//            var data = _interactionLocker.LockData;
//            data.Changed += OnLockDataChanged;
//        }

//        private void OnLockDataChanged(InteractionLockData data)
//        {
//            InteractionLayerMask interactionMask = data.InteractionMask;
//            LayerMask layerMask = data.LayerMask;
//        }

//        public async UniTask ExampleAsync()
//        {
//            using (var lockHandle = _interactionLocker.Lock(_interactionLayer))
//                await DoSomething();
//        }


//        private UniTask DoSomething() { }
//    }

//    public sealed class RaycasterInteractionLockerTest : MonoBehaviour
//    {
//        private LayerMask _originalEventMask;
//        private IInteractionLockerService _interactionLocker;

//        [SerializeField] private PhysicsRaycaster _raycaster;

//        private void Awake()
//        {
//            _originalEventMask = _raycaster.eventMask;
//            InteractionLocker.LockData.Changed += OnLockLayerMaskChanged;
//        }


//        private void OnLockLayerMaskChanged(InteractionLockData data)
//        {
//            // Исключить из исходной маски текущую заблокированную
//            var eventMask = _originalEventMask & ~data.LayerMask;
//            _raycaster.eventMask = eventMask;
//        }

//        private void Example3(Vector3 origin, Vector3 direction, int distance, LayerMask originalLayerMask)
//        {
//            var data = _interactionLocker.LockData;
//            var blockedLayerMask = data.Value.LayerMask;
//            var layerMask = originalLayerMask & ~blockedLayerMask;

//            var hit = Physics2D.Raycast(origin, direction, distance, layerMask);
//        }
//    }
//}