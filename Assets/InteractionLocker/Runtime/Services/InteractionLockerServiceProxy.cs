using CatCode.Pools;
using UnityEngine;

namespace CatCode.InteractionLocking
{
    /// <summary>
    /// Компонент-прокси для сервиса блокировок. 
    /// Предоставляет в редакторе простой способ отслеживать состояние сервиса.
    /// </summary>
    public sealed class InteractionLockerServiceProxy : MonoBehaviour
    {
        [SerializeField, TextArea] private string _info;
        private InteractionLayersAsset _layersAsset;

        private IInteractionLockerService _service;

        public void Init(IInteractionLockerService service, InteractionLayersAsset layersAsset)
        {
            if (_service != null)
                _service.LockDataChanged -= OnMaskChanged;
            _service = service;
            _service.LockDataChanged += OnMaskChanged;
            _layersAsset = layersAsset;
        }

        private void OnDestroy()
        {
            if (_service != null)
                _service.LockDataChanged -= OnMaskChanged;
        }

        private void OnMaskChanged(InteractionLockData lockData)
        {
            using var handle = StringBuilderPool.Get(out var sb);

            var interactionMask = (int)lockData.InteractionMask;
            for (int i = 0; i < 32; i++)
            {
                int bitMask = 1 << i;
                if ((interactionMask & bitMask) != 0)
                {
                    if (TryGetLayerName(i, out var name))
                        sb.Append(name).AppendLine();
                    else
                        sb.Append("Bit index - ").Append(i).AppendLine();
                }
            }
            _info = sb.ToString();
        }

        private bool TryGetLayerName(int bitIndex, out string name)
        {
            for (int i = 0; i < _layersAsset.Layers.Count; i++)
            {
                var layer = _layersAsset.Layers[i];
                if (layer.BitIndex == bitIndex)
                {
                    name = layer.name;
                    return true;
                }
            }
            name = string.Empty;
            return false;
        }
    }
}