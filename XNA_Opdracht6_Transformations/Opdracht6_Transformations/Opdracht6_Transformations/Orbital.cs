using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Opdracht6_Transformations
{
    class Orbital : Sphere
    {
        protected Sphere orbiting;
        public Orbital(Matrix transform, Color color, int numVertices, Vector3 Scalar, Vector3 Position, Vector3 RotationAxisSpeed, Vector3 RotationRadiansSpeed, Sphere Orbiting) : 
            base(transform,  color,  numVertices,  Scalar,  Position,  RotationAxisSpeed,  RotationRadiansSpeed)
        {
            this.orbiting = Orbiting;
        }

        public override void Draw(GameTime gameTime)
        {
            rotationAxis += rotationAxisSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
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

            this.Transform *= Matrix.CreateTranslation(orbiting.Transform.Translation); ;

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
        }
    }
}
