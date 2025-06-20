using RimWorld;
using RimWorld.Planet;
using System;
using UnityEngine;
using VEF;
using Verse;
using Verse.Sound;

namespace VanillaPersonaWeaponsExpanded
{
    [HotSwappable]
    public class PsycastFloatMenuOption : FloatMenuOption
    {
        public PsycastFloatMenuOption(string label, Action action, Texture2D itemIcon, Color iconColor, 
            MenuOptionPriority priority = MenuOptionPriority.Default, Action<Rect> mouseoverGuiAction = null, 
            Thing revalidateClickTarget = null, float extraPartWidth = 0f, Func<Rect, bool> extraPartOnGUI = null, 
            WorldObject revalidateWorldClickTarget = null, bool playSelectionSound = true, int orderInPriority = 0, 
            HorizontalJustification iconJustification = HorizontalJustification.Left, bool extraPartRightJustified = false)
            : base(label, action, itemIcon, iconColor, priority, mouseoverGuiAction, revalidateClickTarget, extraPartWidth, extraPartOnGUI,
                  revalidateWorldClickTarget, playSelectionSound, orderInPriority, iconJustification, extraPartRightJustified)
        {

        }

        public override bool DoGUI(Rect rect, bool colonistOrdering, FloatMenu floatMenu)
        {
            Rect rect2 = rect;
            rect2.height--;
            bool flag = !Disabled && Mouse.IsOver(rect2);
            bool flag2 = false;
            Text.Font = CurrentFont;
            if (tooltip.HasValue)
            {
                TooltipHandler.TipRegion(rect, tooltip.Value);
            }
            Rect rect3 = rect;
            if (iconJustification == HorizontalJustification.Left)
            {
                rect3.xMin += 4f;
                rect3.xMax = rect.x + 27f;
                rect3.yMin += 4f;
                rect3.yMax = rect.y + 27f;
                if (flag)
                {
                    rect3.x += 4f;
                }
            }
            Rect rect4 = rect;
            rect4.xMin += HorizontalMargin;
            rect4.xMax -= HorizontalMargin;
            rect4.xMax -= 4f;
            rect4.xMax -= extraPartWidth + IconOffset;
            if (iconJustification == HorizontalJustification.Left)
            {
                rect4.x += IconOffset;
            }
            if (flag)
            {
                rect4.x += 4f;
            }
            float num = Mathf.Min(Text.CalcSize(Label).x, rect4.width - 4f);
            float num2 = rect4.xMin + num;
            if (iconJustification == HorizontalJustification.Right)
            {
                rect3.x = num2 + 4f;
                rect3.width = 27f;
                rect3.yMin += 4f;
                rect3.yMax = rect.y + 27f;
                num2 += 27f;
            }
            Rect rect5 = default(Rect);
            if (extraPartWidth != 0f)
            {
                if (extraPartRightJustified)
                {
                    num2 = rect.xMax - extraPartWidth;
                }
                rect5 = new Rect(num2, rect4.yMin, extraPartWidth, 30f);
                flag2 = Mouse.IsOver(rect5);
            }
            if (!Disabled)
            {
                MouseoverSounds.DoRegion(rect2);
            }
            Color color = GUI.color;
            if (Disabled)
            {
                GUI.color = ColorBGDisabled * color;
            }
            else if (flag && !flag2)
            {
                GUI.color = ColorBGActiveMouseover * color;
            }
            else
            {
                GUI.color = ColorBGActive * color;
            }
            GUI.DrawTexture(rect, BaseContent.WhiteTex);
            GUI.color = ((!Disabled) ? ColorTextActive : ColorTextDisabled) * color;
            if (sizeMode == FloatMenuSizeMode.Tiny)
            {
                rect4.y += 1f;
            }
            Widgets.DrawAtlas(rect, TexUI.FloatMenuOptionBG);
            Text.Anchor = TextAnchor.MiddleLeft;
            Widgets.Label(rect4, Label);
            Text.Anchor = TextAnchor.UpperLeft;
            GUI.color = new Color(iconColor.r, iconColor.g, iconColor.b, iconColor.a * GUI.color.a);
            if (shownItem != null || drawPlaceHolderIcon)
            {
                ThingStyleDef thingStyleDef = thingStyle ?? ((shownItem == null || Find.World == null) ? null : Faction.OfPlayer.ideos?.PrimaryIdeo?.GetStyleFor(shownItem));
                if (forceBasicStyle)
                {
                    thingStyleDef = null;
                }
                Color value = (forceThingColor.HasValue ? forceThingColor.Value : ((shownItem == null) ? Color.white : (shownItem.MadeFromStuff ? shownItem.GetColorForStuff(GenStuff.DefaultStuffFor(shownItem)) : shownItem.uiIconColor)));
                value.a *= color.a;
                Widgets.DefIcon(rect3, shownItem, null, 1f, thingStyleDef, drawPlaceHolderIcon, value, null, graphicIndexOverride);
            }
            else if ((bool)iconTex)
            {
                rect3.y -= 5f;
                Widgets.DrawTextureFitted(rect3, iconTex, 0.8f, new Vector2(1f, 1f), iconTexCoords);
            }
            else if (iconThing != null)
            {
                if (iconThing is Pawn)
                {
                    rect3.xMax -= 4f;
                    rect3.yMax -= 4f;
                }
                Widgets.ThingIcon(rect3, iconThing);
            }
            GUI.color = color;
            if (extraPartOnGUI != null)
            {
                bool num3 = extraPartOnGUI(rect5);
                GUI.color = color;
                if (num3)
                {
                    return true;
                }
            }
            if (flag && mouseoverGuiAction != null)
            {
                mouseoverGuiAction(rect);
            }
            if (tutorTag != null)
            {
                UIHighlighter.HighlightOpportunity(rect, tutorTag);
            }
            if (Widgets.ButtonInvisible(rect2))
            {
                if (tutorTag != null && !TutorSystem.AllowAction(tutorTag))
                {
                    return false;
                }
                Chosen(colonistOrdering, floatMenu);
                if (tutorTag != null)
                {
                    TutorSystem.Notify_Event(tutorTag);
                }
                return true;
            }
            return false;
        }
    }
}
