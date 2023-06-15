

using OpenTK.Mathematics;
using Template;

namespace Rasterizer
{
    public class Light : WorldObject
    {
        public Vector3 Position;
        public Vector3 Intensity;

        public Light(Vector3 position, Vector3 intensity)
        {
            Position = position;
            Intensity = intensity / 255;
        }
    }
}
