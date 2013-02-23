using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace SynthesisGameLibrary
{
    public class EnemyData
    {
        public enum Type
        {
            Normal = 0,
            Destroyer = 1,
            Shooter = 2,
            Grabber = 3,
            Goliath = 4
        }

        public float f_Speed;
        public int i_MaxHealth;
        public Type t_Type;
        public String s_Texture;
    }
}
