using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using Rasterizer;

namespace Template
{
    // mesh and loader based on work by JTalton; http://www.opentk.com/node/642

    public class Mesh : WorldObject
    {
        // data members
        public ObjVertex[]? vertices;            // vertex positions, model space
        public ObjTriangle[]? triangles;         // triangles (3 vertex indices)
        public ObjQuad[]? quads;                 // quads (4 vertex indices)
        int vertexBufferId;                     // vertex buffer
        int triangleBufferId;                   // triangle buffer
        int quadBufferId;                       // quad buffer (not in Modern OpenGL)
        public Matrix4 modelMatrix;
        public Matrix4 objectToWorld;
        public Matrix4 scale = Matrix4.Identity;
        public Matrix4 rotation = Matrix4.Identity;
        public Matrix4 translation = Matrix4.Identity;
        private static Matrix4 worldCenter = Matrix4.CreateFromAxisAngle(new Vector3(0, 1, 0), 0);

        // constructor
        public Mesh(string fileName, Matrix4 scale, Matrix4 rotation, Matrix4 translation)
        {
            MeshLoader loader = new();
            loader.Load(this, fileName);
            this.scale = scale;
            this.rotation = rotation;
            this.translation = translation;
            modelMatrix = scale * rotation * translation * worldCenter;
        }

        // initialization; called during first render
        public void Prepare()
        {
            if (vertexBufferId == 0 && vertices != null && triangles != null && quads != null)
            {
                // generate interleaved vertex data (uv/normal/position (total 8 floats) per vertex)
                GL.GenBuffers(1, out vertexBufferId);
                GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferId);
                GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(vertices.Length * Marshal.SizeOf(typeof(ObjVertex))), vertices, BufferUsageHint.StaticDraw);

                // generate triangle index array
                GL.GenBuffers(1, out triangleBufferId);
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, triangleBufferId);
                GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(triangles.Length * Marshal.SizeOf(typeof(ObjTriangle))), triangles, BufferUsageHint.StaticDraw);

                if (OpenTKApp.allowPrehistoricOpenGL)
                {
                    // generate quad index array
                    GL.GenBuffers(1, out quadBufferId);
                    GL.BindBuffer(BufferTarget.ElementArrayBuffer, quadBufferId);
                    GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(quads.Length * Marshal.SizeOf(typeof(ObjQuad))), quads, BufferUsageHint.StaticDraw);
                }
            }
        }

        // render the mesh using the supplied shader and matrix
        public void Render(Shader shader, Matrix4 transform, Texture texture, Camera camera)
        {
            // on first run, prepare buffers
            Prepare();

            // enable shader
            GL.UseProgram(shader.programID);

            // enable texture
            int texLoc = GL.GetUniformLocation(shader.programID, "pixels");
            GL.Uniform1(texLoc, 0);
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, texture.id);

            // pass transform to vertex shader
            GL.UniformMatrix4(shader.uniform_mview, false, ref transform);
            GL.Uniform3(shader.uniform_ambientLight, ref MyApplication.ambientLight);
            GL.UniformMatrix4(shader.uniform_mworld, false, ref objectToWorld);
            GL.Uniform3(shader.uniform_lightPosition0, MyApplication.lightData[0].Position);
            GL.Uniform3(shader.uniform_lightColor0, MyApplication.lightData[0].Intensity);
            GL.Uniform3(shader.uniform_lightPosition1, MyApplication.lightData[1].Position);
            GL.Uniform3(shader.uniform_lightColor1, MyApplication.lightData[1].Intensity);
            GL.Uniform3(shader.uniform_lightPosition2, MyApplication.lightData[2].Position);
            GL.Uniform3(shader.uniform_lightColor2, MyApplication.lightData[2].Intensity);
            GL.Uniform3(shader.uniform_lightPosition3, MyApplication.lightData[3].Position);
            GL.Uniform3(shader.uniform_lightColor3, MyApplication.lightData[3].Intensity);
            GL.Uniform3(shader.uniform_camPosition, camera.position);
            

            // enable position, normal and uv attributes
            GL.EnableVertexAttribArray(shader.attribute_vpos);
            GL.EnableVertexAttribArray(shader.attribute_vnrm);
            GL.EnableVertexAttribArray(shader.attribute_vuvs);

            // bind vertex data
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferId);

            // link vertex attributes to shader parameters 
            GL.VertexAttribPointer(shader.attribute_vuvs, 2, VertexAttribPointerType.Float, false, 32, 0);
            GL.VertexAttribPointer(shader.attribute_vnrm, 3, VertexAttribPointerType.Float, true, 32, 2 * 4);
            GL.VertexAttribPointer(shader.attribute_vpos, 3, VertexAttribPointerType.Float, false, 32, 5 * 4);

            // bind triangle index data and render
            if (triangles != null && triangles.Length > 0)
            {
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, triangleBufferId);
                GL.DrawArrays(PrimitiveType.Triangles, 0, triangles.Length * 3);
            }

            // bind quad index data and render
            if (quads != null && quads.Length > 0)
            {
                if (OpenTKApp.allowPrehistoricOpenGL)
                {
                    GL.BindBuffer(BufferTarget.ElementArrayBuffer, quadBufferId);
                    GL.DrawArrays(PrimitiveType.Quads, 0, quads.Length * 4);
                }
                else throw new Exception("Quads not supported in Modern OpenGL");
            }

            // restore previous OpenGL state
            GL.UseProgram(0);
        }

        // layout of a single vertex
        [StructLayout(LayoutKind.Sequential)]
        public struct ObjVertex
        {
            public Vector2 TexCoord;
            public Vector3 Normal;
            public Vector3 Vertex;
        }

        // layout of a single triangle
        [StructLayout(LayoutKind.Sequential)]
        public struct ObjTriangle
        {
            public int Index0, Index1, Index2;
        }

        // layout of a single quad
        [StructLayout(LayoutKind.Sequential)]
        public struct ObjQuad
        {
            public int Index0, Index1, Index2, Index3;
        }
    }
}