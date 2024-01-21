
using Thor;
using UnityEditor;

namespace Editor.Drawers
{
    [CustomPropertyDrawer(typeof(FastAnimator.BuiltinAnimationTypes))]
    public class BuiltinAnimationTypesDrawer : FlaggedEnumDrawer<FastAnimator.BuiltinAnimationTypes>
    {
        
    }
}