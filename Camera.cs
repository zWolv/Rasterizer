using OpenTK.Mathematics;

namespace Rasterizer
{
    public class Camera
    {
        // MEMBER VARIABLES
        public Vector3 position, frontDirection, rightDirection, upDirection;
        public float pitch = 0, yaw = 0;

        // CONSTRUCTOR
        public Camera(Vector3 position, Vector3 frontDirection, Vector3 rightDirection, Vector3 upDirection) 
        {
            this.position = position;
            this.frontDirection = frontDirection;
            this.rightDirection = rightDirection;
            this.upDirection = upDirection;
        }

        // CLASS METHODS

        // update the direction the camera is looking at
        public void UpdateFrontDirection()
        {
            frontDirection.X = (float)Math.Cos(MathHelper.DegreesToRadians(pitch)) * (float)-Math.Sin(MathHelper.DegreesToRadians(yaw));
            frontDirection.Y = (float)Math.Sin(MathHelper.DegreesToRadians(pitch));
            frontDirection.Z = (float)Math.Cos(MathHelper.DegreesToRadians(pitch)) * (float)-Math.Cos(MathHelper.DegreesToRadians(yaw));
            frontDirection.Normalize();
        }

        // update the right direction of the camera
        public void UpdateRightDirection()
        {
            rightDirection.X = (float)-Math.Cos(MathHelper.DegreesToRadians(yaw));
            rightDirection.Y = 0f;
            rightDirection.Z = (float)Math.Sin(MathHelper.DegreesToRadians(yaw));
            rightDirection.Normalize();
        }

        // update the vertical direction of the camera
        public void UpdateUpDirection()
        {
            upDirection = Vector3.Cross(frontDirection, rightDirection);
            upDirection.Normalize();
        }
    }
}
