using CatCode.InteractionLocking;
using System;
using System.Text;
using UnityEngine;

public class InteractionLocker : MonoBehaviour
{
    private StringBuilder _sb = new StringBuilder();

    private IInteractionLockerService _service;
    [SerializeField, TextArea] private string _info;

    [SerializeField] private InteractionLayersAsset _layers;

    public IInteractionLockerService Service
    {
        get
        {
            if (_service == null)
            {
                _service = InteractionLockerService.Create(_layers);
                _service.LockDataChanged += OnLockDataChanged;
            }
            return _service;
        }
    }

    public event Action<InteractionLockData> LockDataChanged
    {
        add => Service.LockDataChanged += value;
        remove => Service.LockDataChanged -= value;
    }

    public InteractionLockData LockData => Service.LockData;

    public InteractionLockHandle Lock(InteractionLayerAsset layer)
        => Service.Lock(layer);

    public InteractionLockSwitch GetLockSwitch(InteractionLayerAsset layer)
        => Service.GetLockSwitch(layer.InteractionMask);

    public InteractionLockSwitch GetLockSwitch(InteractionLayerMask layerMask)
        => Service.GetLockSwitch(layerMask);


    private void OnLockDataChanged(InteractionLockData lockData)
    {
        _sb.Clear();
        var interactionMask = (int)lockData.InteractionMask;
        for (int i = 0; i < 32; i++)
        {
            int bitMask = 1 << i;
            if ((interactionMask & bitMask) != 0)
            {
                if (TryGetLayerName(i, out var name))
                    _sb.Append(name).AppendLine();
                else
                    _sb.Append("Bit index - ").Append(i).AppendLine();
            }
        }
        _info = _sb.ToString();
    }

    private bool TryGetLayerName(int bitIndex, out string name)
    {
        for (int i = 0; i < _layers.Layers.Count; i++)
        {
            var layer = _layers.Layers[i];
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
