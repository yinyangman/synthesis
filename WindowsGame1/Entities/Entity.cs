using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Synthesis
{
    public class Entity
    {
        protected Color color;                //colour (tint) of the particle
        protected Texture2D tTexture;          //tTexture applied to particle
        protected Color[] tTextureData;         //per pixel texture data
        protected Vector2 vPosition;           //vector defining the vPosition of the particle
        protected float f_Radius;               //once a tTexture is set this is = to tTexture width/2
        protected Vector2 vVelocity;           //vVelocity of the particle
        protected Rectangle rectangle;          //Rectangle used for collision
        protected Vector2 origin;               //Origin of texture, used for collision
        protected Matrix rectanglePosTransform;    //Matrix used for collisions

        public enum entityState               //holds the state of the particle
        {
            stationary,                     //true if the particle has not been pocketed
            moving                          //true if the particle is moving
        }

        protected entityState state;            //create object for enum

        #region set/get
        ///////////////////////////////////////////////////////////
        // rectanglePosTransform Set/Get
        public Matrix RectanglePosTransform
        {
            get
            {
                return rectanglePosTransform;
            }
            set
            {
                rectanglePosTransform = value;
            }
        }
        ///////////////////////////////////////////////////////////

        ///////////////////////////////////////////////////////////
        // rectanglePosTransform Set/Get
        public Color[] TextureData
        {
            get
            {
                return tTextureData;
            }
            set
            {
                tTextureData = value;
            }
        }
        ///////////////////////////////////////////////////////////

        ///////////////////////////////////////////////////////////
        //Color Set/Get
        public Color Color
        {
            get
            {
                return color;
            }
            set
            {
                color = value;
            }
        }
        ///////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////
        //Position Set/Get
        public Vector2 Position
        {
            get
            {
                return vPosition;
            }
            set
            {
                vPosition = value;
            }
        }
        ///////////////////////////////////////////////////////////


        ///////////////////////////////////////////////////////////
        //entityState Set/Get
        public entityState State
        {
            get
            {
                return state;
            }
            set
            {
                state = value;
            }
        }
        ///////////////////////////////////////////////////////////


        ///////////////////////////////////////////////////////////
        //Texture Set/Get
        public Texture2D Texture
        {
            get
            {
                return tTexture;
            }
            set
            {
                tTexture = value;
            }
        }
        ///////////////////////////////////////////////////////////


        ///////////////////////////////////////////////////////////
        //Velocity Set/Get
        public Vector2 Velocity
        {
            get
            {
                return vVelocity;
            }
            set
            {
                vVelocity = value;
            }
        }
        ///////////////////////////////////////////////////////////

        ///////////////////////////////////////////////////////////
        //Color Set/Get
        public Rectangle Rectangle
        {
            get
            {
                return rectangle;
            }
            set
            {
                rectangle = value;
            }
        }
        ///////////////////////////////////////////////////////////
        #endregion

        ///////////////////////////////////////////////////////////
        //updatePosition - update particles vPosition when called
        //NOTE - this also updates the bounding sphere
        public void updatePosition(float Time, float Friction)
        {
            vVelocity *= Friction;
            vPosition += vVelocity * Time;

            if (vVelocity.Length() <= Friction)
            {
                vVelocity *= 0;
            }

            // Updates rectangle for per-pixel collision
            rectangle = new Rectangle(((int)vPosition.X - (Texture.Width / 2)), ((int)vPosition.Y - (Texture.Height / 2)), Texture.Width, Texture.Height);


            if (tTexture != null)
            {
                f_Radius = tTexture.Height / 2;
                origin = new Vector2((tTexture.Width / 2), (tTexture.Height / 2));
            }

            //Updates the rectangle position in a matrix, this is used for everything that doesn't have rotation.
            rectanglePosTransform = Matrix.CreateTranslation(new Vector3(vPosition, 0.0f));
        }
        ///////////////////////////////////////////////////////////

    }
}
