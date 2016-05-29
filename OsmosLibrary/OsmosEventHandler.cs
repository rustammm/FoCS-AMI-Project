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
    public class OsmosEventHandler
    {
        public enum Event { onMouseClick, onCircleIntersect };

        /* Listener */
        private Action onMouseClickListener;
        private HashSet<KeyValuePair<Circle, Action<Circle>>> onCircleIntersectListener;


        /* Handler's data */

        private bool mouseWasDown;
        public List<Circle> OnCircleIntersectCircles { get; set; }
        public List<OsmosAchievment> Achievements { get; set; }
        public Circle UserCircle { get; set; } 
        /* Init and common functions */

        public OsmosEventHandler()
        {
            mouseWasDown = false;
            onCircleIntersectListener = new HashSet<KeyValuePair<Circle, Action<Circle>>>();
            Achievements = new List<OsmosAchievment>();
            for (int i = 1; i <= 5; i++)
                Achievements.Add(new OsmosAchievment(i));
        }


        public void listenAndHandle()
        {
            listenAndHandleOnMouseClick();
            listenAndHandleOnCircleIntersect();
            listenAndHandleAchievments();


        }


        // listening and handling

        public void listenAndHandleAchievments()
        {
            if (UserCircle.Radius > 70)
                Achievements[0].ShowAchievment();

            if (UserCircle.Radius > 100)
                Achievements[1].ShowAchievment();

            if (UserCircle.Absorbed >= 1)
                Achievements[2].ShowAchievment();

            if (UserCircle.Absorbed >= 3)
                Achievements[3].ShowAchievment();

            if (UserCircle.Absorbed >= 10)
                Achievements[4].ShowAchievment();
        }

        public void listenAndHandleOnMouseClick()
        {
            bool mouseIsDown = Mouse.GetState().LeftButton == ButtonState.Pressed;
            mouseIsDown |= Mouse.GetState().RightButton == ButtonState.Pressed;
            if (!mouseWasDown && mouseIsDown && onMouseClickListener != null)
                onMouseClickListener.Invoke();
            mouseWasDown = mouseIsDown;            
        }

        

        public void listenAndHandleOnCircleIntersect()
        {
            foreach (var circleAndAct in OnCircleIntersectCircles)
            {
                Circle mainCircle = circleAndAct;
                foreach (var circle in OnCircleIntersectCircles)
                {
                    if (mainCircle != circle && mainCircle.Radius >= circle.Radius && mainCircle.intersects(circle))
                        circleAndAct.OnIntersectSmaller(circle);
                }

            }
        }

        // manage listeners
        public void handleOnMouseClick(Action func)
        {
            onMouseClickListener += func;
        }

        public void stopHandleOnMouseClick(Action func)
        {
            onMouseClickListener -= func;
        }


        public void handleOnCircleIntersect(Action<Circle> func, Circle watchingCircle)
        {
            onCircleIntersectListener.Add(new KeyValuePair<Circle, Action<Circle>> (watchingCircle, func));
        }

        public void StopHandleOnCircleIntersect(Action<Circle> func, Circle watchingCircle)
        {
            onCircleIntersectListener.Remove(new KeyValuePair<Circle, Action<Circle>>(watchingCircle, func));
        }

    }
}
