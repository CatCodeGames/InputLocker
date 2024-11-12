namespace CatCode
{
    public static class InputLayerUtils
    {
        public static bool HasFlag(InputLayer mask, InputLayer value)
            => (mask & value) == value;
    }
}