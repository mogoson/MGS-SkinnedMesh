﻿/*************************************************************************
 *  Copyright © 2018 Mogoson. All rights reserved.
 *------------------------------------------------------------------------
 *  File         :  CurvePipe.cs
 *  Description  :  Define CurvePipe to render dynamic pipe mesh base on
 *                  center curve.
 *------------------------------------------------------------------------
 *  Author       :  Mogoson
 *  Version      :  0.1.0
 *  Date         :  3/20/2018
 *  Description  :  Initial development version.
 *************************************************************************/

using Developer.SkinnedMesh;
using System.Collections.Generic;
using UnityEngine;

namespace Developer.FlexiblePipe
{
    public abstract class CurvePipe : Skin
    {
        #region Field and Property
        /// <summary>
        /// Segment of around pipe.
        /// </summary>
        public int aroundSegment = 8;

        /// <summary>
        /// Segment of extend pipe.
        /// </summary>
        public int extendSegment = 16;

        /// <summary>
        /// Radius of pipe mesh.
        /// </summary>
        public float radius = 0.1f;

        /// <summary>
        /// Max time of center curve.
        /// </summary>
        public abstract float CurveMaxTime { get; }

        /// <summary>
        /// Radian of circle.
        /// </summary>
        protected const float CircleRadian = Mathf.PI * 2;

        /// <summary>
        /// Delta to calculate tangent.
        /// </summary>
        protected const float Delta = 0.001f;
        #endregion

        #region Protected Method
        /// <summary>
        /// Create the vertices of pipe mesh.
        /// </summary>
        /// <returns>Vertices of pipe mesh.</returns>
        protected override Vector3[] CreateVertices()
        {
            var vertices = new List<Vector3>();
            var space = 1.0f / extendSegment;
            for (int i = 0; i < extendSegment; i++)
            {
                var t = i * space;
                var center = GetPointFromCurveAt(t);
                var tangent = (GetPointFromCurveAt(t + Delta) - center).normalized;
                vertices.AddRange(CreateSegmentVertices(center, Quaternion.LookRotation(tangent)));
            }

            var lastCenter = GetPointFromCurveAt(1.0f);
            var lastTangent = (lastCenter - GetPointFromCurveAt(1.0f - Delta)).normalized;

            vertices.AddRange(CreateSegmentVertices(lastCenter, Quaternion.LookRotation(lastTangent)));
            return vertices.ToArray();
        }

        /// <summary>
        /// Create triangles of pipe mesh.
        /// </summary>
        /// <returns>Triangles array.</returns>
        protected override int[] CreateTriangles()
        {
            var triangles = new List<int>();
            for (int i = 0; i < extendSegment; i++)
            {
                for (int j = 0; j < aroundSegment - 1; j++)
                {
                    triangles.Add(aroundSegment * i + j);
                    triangles.Add(aroundSegment * i + j + 1);
                    triangles.Add(aroundSegment * (i + 1) + j + 1);

                    triangles.Add(aroundSegment * i + j);
                    triangles.Add(aroundSegment * (i + 1) + j + 1);
                    triangles.Add(aroundSegment * (i + 1) + j);
                }

                triangles.Add(aroundSegment * i);
                triangles.Add(aroundSegment * (i + 1));
                triangles.Add(aroundSegment * (i + 2) - 1);

                triangles.Add(aroundSegment * i);
                triangles.Add(aroundSegment * (i + 2) - 1);
                triangles.Add(aroundSegment * (i + 1) - 1);
            }
            return triangles.ToArray();
        }

        /// <summary>
        /// Create vertices of current segment base pipe.
        /// </summary>
        /// <param name="center">Center point of segment.</param>
        /// <param name="rotation">Rotation of segment vertices.</param>
        /// <returns>Segment vertices.</returns>
        protected virtual Vector3[] CreateSegmentVertices(Vector3 center, Quaternion rotation)
        {
            var vertices = new Vector3[aroundSegment];
            for (int i = 0; i < aroundSegment; i++)
            {
                var angle = CircleRadian / aroundSegment * i;
                var vertice = center + rotation * new Vector3(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
                vertices[i] = vertice;
            }
            return vertices;
        }

        /// <summary>
        /// Get point from center curve of pipe at normalized time.
        /// </summary>
        /// <param name="t">Normalized time in the range(0~1).</param>
        /// <returns>Point on pipe curve at t.</returns>
        protected virtual Vector3 GetPointFromCurveAt(float t)
        {
            return GetPointFromCurve(CurveMaxTime * t);
        }
        #endregion

        #region Public Method
        /// <summary>
        /// Get point from center curve of pipe at time.
        /// </summary>
        /// <param name="time">Time of pipe center curve.</param>
        /// <returns>Point on pipe curve at time.</returns>
        public abstract Vector3 GetPointFromCurve(float time);
        #endregion
    }
}