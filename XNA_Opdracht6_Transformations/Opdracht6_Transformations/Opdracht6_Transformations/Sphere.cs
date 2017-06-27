using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Opdracht6_Transformations
{
    public class Sphere
    {

        public struct VertexPositionColorNormal : IVertexType
        {
            public Vector3 Position;
            public Color Color;
            public Vector3 Normal;

            VertexDeclaration IVertexType.VertexDeclaration
            {
                get { return new VertexDeclaration
            (
                new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
                new VertexElement(sizeof(float) * 3, VertexElementFormat.Color, VertexElementUsage.Color, 0),
                new VertexElement(sizeof(float) * 3 + 4, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0)
            ); }
            }
        }

        public Vector3 scalar;
        public Vector3 position;
        public Vector3 rotationAxisSpeed;//x,y,z
        public Vector3 rotationRadiansSpeed;//x,y,z
        public Vector3 rotationAxis;//x,y,z
        public Vector3 rotationRadians;//x,y,z

        public VertexPositionColorNormal[] vertices;
        protected VertexBuffer vbuffer;
        protected short[] indices;
        protected IndexBuffer ibuffer;
        protected int nvertices, nindices;
        protected BasicEffect effect;
        protected GraphicsDevice graphics;
        protected int numVertices;
        public Matrix Transform;
        public Color color;

        public Sphere(Matrix transform, Color color, int numVertices, Vector3 Scalar, Vector3 Position, Vector3 RotationAxisSpeed, Vector3 RotationRadiansSpeed)
        {
            this.Transform = transform;
            this.color = color;
            this.numVertices = numVertices;
            graphics = SimPhyGameWorld.World.GraphicsDevice;
            effect = new BasicEffect(SimPhyGameWorld.Graphics);
            effect.EnableDefaultLighting();
            nvertices = numVertices * numVertices;
            nindices = numVertices * numVertices * 6;
            vbuffer = new VertexBuffer(graphics, typeof(VertexPositionColorNormal), nvertices, BufferUsage.WriteOnly);
            ibuffer = new IndexBuffer(graphics, IndexElementSize.SixteenBits, nindices, BufferUsage.WriteOnly);
            createVertices();
            createIndices();
            vbuffer.SetData<VertexPositionColorNormal>(vertices);
            ibuffer.SetData<short>(indices);
            effect.VertexColorEnabled = true;
            this.scalar = Scalar;
            this.position = Position;
            this.rotationAxisSpeed = RotationAxisSpeed;
            this.rotationRadiansSpeed = RotationRadiansSpeed;
        }
        void createVertices() // Note: This can be done a lot more efficiently if multiple spheres have the same number of vertices
        {
            vertices = new VertexPositionColorNormal[nvertices];
            Vector3 center = new Vector3(0, 0, 0);
            Vector3 rad = new Vector3(1, 0, 0);
            for (int x = 0; x < numVertices; x++)
            {
                float difx = 360.0f / (float)numVertices;
                for (int y = 0; y < numVertices; y++)
                {
                    float dify = 360.0f / (float)numVertices;
                    Matrix zrot = Matrix.CreateRotationZ(MathHelper.ToRadians(y * dify));
                    Matrix yrot = Matrix.CreateRotationY(MathHelper.ToRadians(x * difx));
                    Vector3 point = Vector3.Transform(Vector3.Transform(rad, zrot), yrot);
                    Vector3 normal = point;
                    point.Normalize();

                    vertices[x + y * numVertices] = new VertexPositionColorNormal() { Position = point, Color = color, Normal = normal };
                }
            }
        }

        void createIndices()
        {
            indices = new short[nindices];
            int i = 0;
            for (int x = 0; x < numVertices; x++)
            {
                for (int y = 0; y < numVertices; y++)
                {
                    int s1 = x == numVertices-1 ? 0 : x + 1;
                    int s2 = y == numVertices-1 ? 0 : y + 1;
                    short upperLeft = (short)(x * numVertices + y);
                    short upperRight = (short)(s1 * numVertices + y);
                    short lowerLeft = (short)(x * numVertices + s2);
                    short lowerRight = (short)(s1 * numVertices + s2);
                    indices[i++] = upperLeft;
                    indices[i++] = upperRight;
                    indices[i++] = lowerLeft;
                    indices[i++] = lowerLeft;
                    indices[i++] = upperRight;
                    indices[i++] = lowerRight;
                }
            }
        }
        public virtual void Draw(GameTime gameTime)
        {
            rotationAxis += rotationAxisSpeed *(float)gameTime.ElapsedGameTime.TotalSeconds;
            rotationRadians += rotationRadiansSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;

            effect.View = SimPhyGameWorld.World.View;
            effect.Projection = SimPhyGameWorld.World.Projection;

            this.Transform = Matrix.Identity;

            this.Transform *= Matrix.CreateScale(this.scalar);
            this.Transform *= Matrix.CreateRotationX(this.rotationAxis.X);
            this.Transform *= Matrix.CreateRotationY(this.rotationAxis.Y);
            this.Transform *= Matrix.CreateRotationZ(this.rotationAxis.Z);

            this.Transform *= Matrix.CreateTranslation(this.position);

            this.Transform *= Matrix.CreateRotationX(this.rotationRadians.X);
            this.Transform *= Matrix.CreateRotationY(this.rotationRadians.Y);
            this.Transform *= Matrix.CreateRotationZ(this.rotationRadians.Z);
            effect.World = this.Transform;
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                try {
                    graphics.DrawUserIndexedPrimitives<VertexPositionColorNormal>(PrimitiveType.TriangleList, vertices, 0, nvertices, indices, 0, indices.Length / 3);
                }catch (Exception e){
                    Console.WriteLine(e.StackTrace);
                }
            }
        }
    }
}
