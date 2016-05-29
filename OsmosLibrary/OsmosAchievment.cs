using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace OsmosLibrary
{
    public class OsmosAchievment
    {
        public bool Achieved { get; set; }
        public bool Show { get; set; }
        public int ShowLeft { get; set; }
        public Texture2D Texture { get; set; }
        public int Type { get; set; }

        public OsmosAchievment()
        {
            Achieved = false;
            Texture = null;
            Type = -1;
            Show = false;
            ShowLeft = 0;
        }

        public OsmosAchievment(int type)
        {
            Type = type;
            Achieved = Show = false;
            ShowLeft = 0;
        }

        public void ShowAchievment()
        {
            if (Show || Achieved) return;
            Show = true;
            ShowLeft = 100;

        }
        /*
        1 - bigger70
        2 - bigger100
        3 - absorbed 1
        4 - ...      3
        5 - ...     10
        */
    }
}
