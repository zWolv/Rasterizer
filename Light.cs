

using OpenTK.Mathematics;
using Template;

namespace Rasterizer
{
    public class Light : WorldObject
    {
        public Vector3 Position;
        public Vector3 Intensity;
        private static int lightCount = 0;

        public Light(Vector3 position, Vector3 intensity)
        {
            Position = position;
            Intensity = intensity / 255;
            MyApplication.lightData[lightCount++] = this;
        }
    }
}
