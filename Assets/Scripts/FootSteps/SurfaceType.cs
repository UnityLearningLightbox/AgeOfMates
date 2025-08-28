using UnityEngine;

public class SurfaceType : MonoBehaviour
{
    public enum Surface { Default, Grass, water, Wood, Metal, Stone }
    public Surface surfaceType = Surface.Default;
}
