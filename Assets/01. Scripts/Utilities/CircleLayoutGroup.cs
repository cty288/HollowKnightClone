using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace HollowKnight
{
    [AddComponentMenu("Layout/Circle Layout Group", 150)]
    public class CircleLayoutGroup : LayoutGroup
    {
        public enum LayoutMode
        {
            Hypodispersion = 0,
            Sector = 1,
        }

        [Header("Layout Mode")]
        public LayoutMode mode = LayoutMode.Hypodispersion;

        [Header("Radius")]
        public float radius = 0;

        [Header("Init Angle")]
        public float initAngle = 0;

        [Header("Fixed Radian")]
        public bool keepRadLen = false;
        [Header("Fixed Radian Value")]
        public float keepRadLenVal = 0f;
        [Header("Sector Angle")]
        public float sectorAngle = 0;
        [Header("Sector Align Center")]
        public bool sectorAlignCenter = false;
        [Header("Is Sector Clockwise")]
        public bool sectorClockwise = true;


        protected override void OnEnable()
        {
            base.OnEnable();
            CalculateRadial();
        }
        public override void SetLayoutHorizontal()
        {
            CalculateRadial();
        }
        public override void SetLayoutVertical()
        {
            CalculateRadial();
        }
        public override void CalculateLayoutInputHorizontal()
        {
            base.CalculateLayoutInputHorizontal();
            CalculateRadial();
        }
        public override void CalculateLayoutInputVertical()
        {
            // Util.Print("CalculateLayoutInputVertical");
            CalculateRadial();
        }
#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            CalculateRadial();
        }
#endif

        protected void CalculateRadial()
        {
            this.m_Tracker.Clear();
            if (transform.childCount == 0)
                return;

            if (this.mode == LayoutMode.Hypodispersion)
            {
                this.Hypodispersion();
            }
            else if (this.mode == LayoutMode.Sector)
            {
                this.Sector();
            }
        }


        private void Hypodispersion()
        {
            if (this.rectChildren.Count <= 0) return;

            float perRad = 2 * Mathf.PI / rectChildren.Count;
            float initRad = this.initAngle * Mathf.Deg2Rad;

            this.SetLayout(initRad, perRad);
        }


        private void Sector()
        {
            if (rectChildren.Count <= 0) return;

            float perRad = 0;
            float initRad = this.initAngle * Mathf.Deg2Rad;

            var sectorRad = this.sectorAngle * Mathf.Deg2Rad;

            if (this.keepRadLen)
            {

                perRad = keepRadLenVal / this.radius;
                if (sectorAlignCenter)
                {

                    initRad += (sectorClockwise ? sectorRad * .5f : -sectorRad * .5f);
                    if (rectChildren.Count > 1)
                    {

                        float _radOff = perRad * ((rectChildren.Count - 1) * 0.5f);
                        initRad -= (sectorClockwise ? _radOff : -_radOff);
                    }
                }
                else
                {
                    perRad = keepRadLenVal / this.radius;
                }
            }
            else
            {

                if (sectorAlignCenter)
                {
                    perRad = sectorRad / (rectChildren.Count + 1);
                    initRad += sectorClockwise ? perRad : -perRad;
                }
                else
                {
                    perRad = rectChildren.Count == 1 ? 0 : sectorRad / (rectChildren.Count - 1);
                }
            }

            if (!sectorClockwise)
            {
                perRad *= -1;
            }

            this.SetLayout(initRad, perRad);
        }

        private void SetLayout(float initRad, float perRad)
        {

            float totalMin = 0;
            float totalPreferred = 0;
            float totalFlexible = 0;

            float minX = float.MaxValue;
            float maxX = float.MinValue;
            float minY = float.MaxValue;
            float maxY = float.MinValue;
            for (int i = 0; i < rectChildren.Count; i++)
            {
                var child = rectChildren[i];


                m_Tracker.Add(this, child,
                DrivenTransformProperties.Anchors |
                DrivenTransformProperties.AnchoredPosition |
                DrivenTransformProperties.Pivot);

                var size = child.rect.size;
                child.pivot = new Vector2(0.5f, 0.5f);
                child.anchorMin = new Vector2(0.5f, 0.5f);
                child.anchorMax = new Vector2(0.5f, 0.5f);
                child.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.x);
                child.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.y);



                size *= 0.5f * child.localScale;
                Vector3 vPos = child.localPosition;
                var rad = initRad + perRad * i;
                vPos.x = this.radius * Mathf.Cos(rad);
                vPos.y = this.radius * Mathf.Sin(rad);
                child.transform.DOLocalMove(vPos,0.5f);

                var left = vPos.x - size.x;
                if (left < minX) minX = left;
                var right = vPos.x + size.x;
                if (right > maxX) maxX = right;

                var bottom = vPos.y - size.y;
                if (bottom < minY) minY = bottom;
                var top = vPos.y + size.y;
                if (top > maxY) maxY = top;
            }


            var w = Mathf.Abs(maxX - minX);
            var h = Mathf.Abs(maxY - minY);
            totalMin = this.radius;
            totalPreferred = w;
            if (this.mode == LayoutMode.Sector)
            {

            }
            SetLayoutInputForAxis(totalMin, totalPreferred, totalFlexible, 0);
            totalPreferred = h;
            if (this.mode == LayoutMode.Sector)
            {

            }
            SetLayoutInputForAxis(totalMin, totalPreferred, totalFlexible, 1);
        }
    }
}
