using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#pragma warning disable CS0626
namespace TowerFall.Editor
{
    class patch_ActorsLayerUI : ActorsLayerUI
    {
        public extern void orig_Render();
        public override void Render()
        {
            orig_Render();
            if (lastMousePos != null)
            {
                // Draw coordinates for easier Co-Op spawn portal node creation
                Draw.Text(TFGame.Font, $"X: {((Vector2)lastMousePos).X}", new Vector2(270f, 00f), Color.Yellow);
                Draw.Text(TFGame.Font, $"Y: {((Vector2)lastMousePos).Y}", new Vector2(270f, 10f), Color.Yellow);
                lastMousePos = null;
            }
        }

        Vector2? lastMousePos;
        public extern void orig_OnMouseOver(Vector2 localPosition);
        public override void OnMouseOver(Vector2 localPosition)
        {
            orig_OnMouseOver(localPosition);
            lastMousePos = localPosition;
        }
    }
}
