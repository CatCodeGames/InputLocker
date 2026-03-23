using System.Collections.Generic;
using UnityEngine;

namespace CatCode.InteractionLocking
{
    /// <summary>
    /// Набор слоёв взаимодействия.
    /// 
    /// Содержит список InteractionLayerAsset и используется как общий
    /// конфигурационный контейнер для группировки слоёв.
    /// </summary>
    [CreateAssetMenu(menuName = "CatCode/InteractionLocker/InteractionLayers")]
    public class InteractionLayersAsset : ScriptableObject
    {
        [SerializeField] private List<InteractionLayerAsset> _layers = new();
        public IReadOnlyList<InteractionLayerAsset> Layers => _layers;
    }
}