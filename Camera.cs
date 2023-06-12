using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INFOGR2023TemplateP2
{
    public class Camera
    {
        public Vector3 position, frontDirection, rightDirection, upDirection;
        public float pitch = 0, yaw = 180;

        public Camera(Vector3 position, Vector3 frontDirection, Vector3 rightDirection, Vector3 upDirection) 
        {
            this.position = position;
            this.frontDirection = frontDirection;
            this.rightDirection = rightDirection;
            this.upDirection = upDirection;
        }

        public void UpdateFrontDirection()
        {
            frontDirection.X = (float)Math.Cos(MathHelper.DegreesToRadians(pitch)) * (float)-Math.Sin(MathHelper.DegreesToRadians(yaw));
            frontDirection.Y = (float)Math.Sin(MathHelper.DegreesToRadians(pitch));
            frontDirection.Z = (float)Math.Cos(MathHelper.DegreesToRadians(pitch)) * (float)-Math.Cos(MathHelper.DegreesToRadians(yaw));
            frontDirection.Normalize();
        }

        public void UpdateRightDirection()
        {
            rightDirection.X = (float)-Math.Cos(MathHelper.DegreesToRadians(yaw));
            rightDirection.Y = 0f;
            rightDirection.Z = (float)Math.Sin(MathHelper.DegreesToRadians(yaw));
            rightDirection.Normalize();
        }

        public void UpdateUpDirection()
        {
            upDirection = Vector3.Cross(frontDirection, rightDirection);
            upDirection.Normalize();
        }
    }
}
