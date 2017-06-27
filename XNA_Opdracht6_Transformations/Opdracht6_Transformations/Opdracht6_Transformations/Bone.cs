using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Opdracht6_Transformations
{
    class Bone : Sphere
    {
        Bone parent;
        List<Sphere> children = new List<Sphere>();
        Vector3 anchor;


        public Bone(Matrix transform, Color color, int numVertices, Vector3 Scalar, Vector3 Position, Vector3 RotationAxisSpeed, Vector3 RotationRadiansSpeed, Bone Parent = null, Vector3 Anchor = default(Vector3)) :
            base(transform, color, numVertices, Scalar, Position, RotationAxisSpeed, RotationRadiansSpeed)
        {
            this.parent = Parent;
            this.anchor = Vector3.Normalize(Anchor);
            if (parent != null)
            {
                this.parent.children.Add(this);
                //this.position = new Vector3(0f);
                this.rotationAxisSpeed += parent.rotationAxisSpeed;
                this.rotationRadiansSpeed += parent.rotationRadiansSpeed;
                //this.rotationAxis += parent.rotationAxis;
                //this.rotationRadians += parent.rotationRadians;
            }

        }

        public override void Draw(GameTime gameTime)
        {
            rotationAxis += rotationAxisSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            rotationRadians += rotationRadiansSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;

            effect.View = SimPhyGameWorld.World.View;
            effect.Projection = SimPhyGameWorld.World.Projection;

            this.Transform = Matrix.Identity;

            this.Transform *= Matrix.CreateScale(this.scalar);

            if(parent != null)
                this.Transform *= Matrix.CreateTranslation(this.anchor * parent.scalar);

            //this.Transform *= Matrix.CreateRotationX(this.rotationAxis.X);
            //this.Transform *= Matrix.CreateRotationY(this.rotationAxis.Y);
            //this.Transform *= Matrix.CreateRotationZ(this.rotationAxis.Z);
            this.Transform *= Matrix.CreateFromYawPitchRoll(this.rotationAxis.X, this.rotationAxis.Y, rotationAxis.Z);

            this.Transform *= Matrix.CreateTranslation(this.position);



            this.Transform *= Matrix.CreateRotationX(this.rotationRadians.X);
            this.Transform *= Matrix.CreateRotationY(this.rotationRadians.Y);
            this.Transform *= Matrix.CreateRotationZ(this.rotationRadians.Z);


            //this.Transform *= Matrix.CreateTranslation(this.anchor);
            //if (parent != null)
            //this.Transform *= Matrix.CreateFromYawPitchRoll(parent.rotationRadians.X, parent.rotationRadians.Y, parent.rotationRadians.Z);


            if (parent != null)
                this.Transform *= Matrix.CreateTranslation(parent.Transform.Translation);

            effect.World = this.Transform;
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                try
                {
                    graphics.DrawUserIndexedPrimitives<VertexPositionColorNormal>(PrimitiveType.TriangleList, vertices, 0, nvertices, indices, 0, indices.Length / 3);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.StackTrace);
                }
            }
            foreach(Bone child in children)
            {
                child.Draw(gameTime);
            }
        }
    }
}
