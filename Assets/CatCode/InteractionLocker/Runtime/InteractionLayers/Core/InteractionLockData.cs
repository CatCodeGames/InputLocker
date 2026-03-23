using UnityEngine;

namespace CatCode.InteractionLocking
{
    /// <summary>
    /// Пара данных для слоя взаимодействия:
    /// содержит битовую маску взаимодействия и соответствующий Unity LayerMask.
    /// Используется как связка логического слоя и физического слоя Unity.
    /// </summary>
    public struct InteractionLockData
    {
        public InteractionLayerMask InteractionMask;
        public LayerMask LayerMask;

        public InteractionLockData(InteractionLayerMask interactionMask, LayerMask layerMask)
        {
            InteractionMask = interactionMask;
            LayerMask = layerMask;
        }
    }
}