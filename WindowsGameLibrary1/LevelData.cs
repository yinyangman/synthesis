using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace SynthesisGameLibrary
{
    public class LevelData
    {
        public string s_LevelName;
        public int i_EnemySpawnRate;
        public int i_MaxNumberEnemies;
        public int i_PhotonSpawnRate;
        public int i_MaxNumberPhotons;
        public int i_ChloroSpawnRate;
        public int i_MaxNumberChloro;
        public int i_MaxNumberFused;
        public int i_TargetFusions;
        public String s_StartBackground;
        public String s_Background;
        public String s_LevelBounds;
        public Rectangle levelTop;
        public Rectangle levelBottom;
        public Rectangle levelLeft;
        public Rectangle levelRight;
        public int i_EnemyMinorChance;
        public int i_EnemyMajorChance;
        
    }
}
