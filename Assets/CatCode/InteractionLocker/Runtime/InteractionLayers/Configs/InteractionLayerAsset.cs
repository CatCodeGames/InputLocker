using UnityEngine;

namespace CatCode.InteractionLocking
{
    /// <summary>
    /// Описание одного слоя взаимодействия.
    /// 
    /// Хранит индекс бита для маски взаимодействия и связанный Unity LayerMask.
    /// Кратко - связь логического слоя взаимодействия и физического слоя Unity.
    /// </summary>
    [CreateAssetMenu(menuName = "CatCode/InteractionLocker/InteractionLayer")]
    public class InteractionLayerAsset : ScriptableObject
    {
        [SerializeField] private int _bitIndex = -1;
        [SerializeField] private LayerMask _layerMask;
        
        public int BitIndex => _bitIndex;
        public InteractionLayerMask InteractionMask => 1 << _bitIndex;
        public LayerMask LayerMask => _layerMask;
    }
}