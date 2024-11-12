using UnityEngine;

namespace CatCode
{
    public enum InspectorReadOnlyMode
    {
        PlayMode = 1 << 0,
        EditMode = 1 << 1,
        Always = PlayMode | EditMode,
    }

    public sealed class InspectorReadOnlyAttribute : PropertyAttribute
    {
        public InspectorReadOnlyMode Mode { get; private set; }

        public InspectorReadOnlyAttribute(InspectorReadOnlyMode mode = InspectorReadOnlyMode.Always)
        {
            Mode = mode;
        }
    }
}