using OpenTK.Mathematics;
using Template;

namespace Rasterizer
{
    public class SceneGraph
    {
        // MEMBER VARIABLES
        private Node world;
        private Shader shader;
        private Texture wood;
        private Shader postProcess;
        private Texture lut;

        public Shader Shader { get { return shader; } }
        public Texture Wood { get { return wood; } }
        public Shader PostProcess { get { return postProcess; } }
        public Texture LUT { get { return lut; } set { lut = value; } }

        // CONSTRUCTOR
        public SceneGraph()
        {
            world = new Node(null!);
            shader = new Shader("../../../shaders/vs.glsl", "../../../shaders/fs.glsl");
            wood = new Texture("../../../assets/wood.jpg");
            postProcess = new Shader("../../../shaders/vs_post.glsl", "../../../shaders/fs_post.glsl");
            lut = new Texture("../../../assets/luts/lut0.png");
        }

        // CLASS METHODS

        // add an object directly to the world node
        public void AddWorldObject(WorldObject mesh)
        {
            world.AddChild(mesh);
        }

        // add an object to another object as a child
        public void AddWorldObjectToObject(WorldObject mesh, WorldObject parent)
        {
            var parentNode = FindWorldObjectNode(parent);
            parentNode.AddChild(mesh);
        }

        // find a specific world object in the tree
        Node FindWorldObjectNode(WorldObject mesh, Node next = null!)
        {
            if (world.Children.Count != 0 && next == null)
            {
                foreach(var child in world.Children)
                {
                    if(child.Data == mesh)
                    {
                        return child;
                    }
                    else
                    {
                        return FindWorldObjectNode(mesh, child);
                    }
                }
            }
            if(next != null)
            {
                foreach (var child in next.Children)
                {
                    if (child.Data == mesh)
                    {
                        return child;
                    }
                    else
                    {
                        return FindWorldObjectNode(mesh, child);
                    }
                }
            }
            return null!;
        }

        // render from the world node as start point
        public void Render(Matrix4 worldToScreen, Camera camera)
        {
            if(world.Children.Count != 0) 
            {
                world.Children.ForEach(child => { ProcessNode(worldToScreen, child, camera); });
            }
            else
            {
                throw new Exception("There are no meshes to be rendered");
            }
        }

        // render each node not directly under the world node and its children
        public void ProcessNode(Matrix4 worldToScreen, Node node, Camera camera)
        {
            // render the mesh if the node contains a mesh. Lights don't need to be rendered.
            if (node.Data is Light)
            {
                return;
            }

            Mesh? mesh = node.Data as Mesh;
            Matrix4 view = Matrix4.CreatePerspectiveFieldOfView(1.2f, 1.3f, .1f, 1000);
            Matrix4 transform = mesh.objectToWorld * worldToScreen * view;

            mesh?.Render(shader, transform, wood, camera);

            if (node.Children.Count == 0)
            { 
                return;
            }
            node.Children.ForEach(child => { ProcessNode(worldToScreen, child, camera); });
        }
    }





    public class Node
    {
        // MEMBER VARIABLES
        private Node parent;
        public Node Parent { get { return parent; } }
        private WorldObject data;

        public WorldObject Data { get { return data; } }

        private List<Node> children = new List<Node>();

        public List<Node> Children { get { return children; } }
        
        // CONSTRUCTOR
        public Node(WorldObject data, Node parent = null!)
        {
            this.parent = parent;
            this.data = data;
        }

        // CLASS METHODS

        // add a child to this node
        public void AddChild(WorldObject mesh)
        {
            var node = new Node(mesh, this);
            children.Add(node);
            if (node.Data is Mesh q)
            {
                q.objectToWorld = node.CalculateRelativeModelMatrix();
            }
            
        }

        // calculate the relative modelmatrix of this mesh in relation to the world node
        public Matrix4 CalculateRelativeModelMatrix()
        {
            Matrix4 result = Matrix4.Identity;
            List<Matrix4> parentMatrices = new List<Matrix4>();
            Node above = parent;
            while(above != null && above.Data != null)
            {
                Mesh? mesh = above.Data as Mesh;
                parentMatrices.Add(mesh.modelMatrix);
                above = above.parent;
            }
            for(int i = parentMatrices.Count - 1; i >= 0;i--)
            {
                if(result == Matrix4.Identity)
                {
                    result = parentMatrices[i];
                }
                else
                {
                    result *= parentMatrices[i];
                }
            }

            Mesh? q = Data as Mesh;

            return result * q.modelMatrix;
        }
    }
}
