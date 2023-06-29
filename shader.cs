    using System;
using System.IO;
using OpenTK.Graphics.OpenGL;

namespace Template
{
    public class Shader
    {
        // data members
        public int programID, vsID, fsID;
        public int attribute_vpos;
        public int attribute_vnrm;
        public int attribute_vuvs;
        public int uniform_mview;
        public int uniform_ambientLight;
        public int uniform_mworld;
        public int uniform_lightColor0;
        public int uniform_lightPosition0;
        public int uniform_camPosition;
        public int uniform_lightColor1;
        public int uniform_lightColor2;
        public int uniform_lightColor3;
        public int uniform_lightPosition1;
        public int uniform_lightPosition2;
        public int uniform_lightPosition3;

        // constructor
        public Shader(String vertexShader, String fragmentShader)
        {
            // compile shaders
            programID = GL.CreateProgram();
            Load(vertexShader, ShaderType.VertexShader, programID, out vsID);
            Load(fragmentShader, ShaderType.FragmentShader, programID, out fsID);
            GL.LinkProgram(programID);
            Console.WriteLine(GL.GetProgramInfoLog(programID));

            // get locations of shader parameters
            attribute_vpos = GL.GetAttribLocation(programID, "vPosition");
            attribute_vnrm = GL.GetAttribLocation(programID, "vNormal");
            attribute_vuvs = GL.GetAttribLocation(programID, "vUV");
            uniform_mview = GL.GetUniformLocation(programID, "objectToScreen");
            uniform_ambientLight = GL.GetUniformLocation(programID, "ambientLight");
            uniform_mworld = GL.GetUniformLocation(programID, "objectToWorld");
            uniform_lightColor0 = GL.GetUniformLocation(programID, "lightColor0");
            uniform_lightColor1 = GL.GetUniformLocation(programID, "lightColor1");
            uniform_lightColor2 = GL.GetUniformLocation(programID, "lightColor2");
            uniform_lightColor3 = GL.GetUniformLocation(programID, "lightColor3");
            uniform_lightPosition0 = GL.GetUniformLocation(programID, "lightPosition0");
            uniform_lightPosition1 = GL.GetUniformLocation(programID, "lightPosition1");
            uniform_lightPosition2 = GL.GetUniformLocation(programID, "lightPosition2");
            uniform_lightPosition3 = GL.GetUniformLocation(programID, "lightPosition3");
            uniform_camPosition = GL.GetUniformLocation(programID, "cameraPosition");
        }

        // loading shaders
        void Load(String filename, ShaderType type, int program, out int ID)
        {
            // source: http://neokabuto.blogspot.nl/2013/03/opentk-tutorial-2-drawing-triangle.html
            ID = GL.CreateShader(type);
            using (StreamReader sr = new StreamReader(filename)) GL.ShaderSource(ID, sr.ReadToEnd());
            GL.CompileShader(ID);
            GL.AttachShader(program, ID);
            Console.WriteLine(GL.GetShaderInfoLog(ID));
        }
    }
}
