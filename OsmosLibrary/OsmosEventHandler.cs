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
        public HashSet<Circle> OnCircleIntersectCircles { get; set; }

        /* Init and common functions */

        public OsmosEventHandler()
        {
            mouseWasDown = false;
            onCircleIntersectListener = new HashSet<KeyValuePair<Circle, Action<Circle>>>();
        }


        public void listenAndHandle()
        {
            listenAndHandleOnMouseClick();
            listenAndHandleOnCircleIntersect();
        }


        // listening and handling

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
            foreach (var circleAndAct in onCircleIntersectListener)
            {
                Circle mainCircle = circleAndAct.Key;
                foreach (var circle in OnCircleIntersectCircles)
                {
                    if (mainCircle != circle && mainCircle.intersects(circle))
                        circleAndAct.Value.Invoke(circle);
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
