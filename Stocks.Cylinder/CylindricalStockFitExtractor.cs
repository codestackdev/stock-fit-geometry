//**********************
//Stock Master
//Copyright(C) 2018 www.codestack.net
//Product: https://www.codestack.net/labs/solidworks/stock-fit-geometry/
//License: https://github.com/codestack-net-dev/stock-fit-geometry/blob/master/LICENSE
//**********************

using CodeStack.Community.StockFit.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeStack.Community.StockFit.Base.Math.Structures;
using CodeStack.Community.StockFit.Base.Math;
using System.Diagnostics;

namespace CodeStack.Community.StockFit.Stocks.Cylinder
{
    /// <summary>
    /// Represents the cylindrical stock
    /// </summary>
    public class CylindricalStockFitExtractor : IStockFitExtractor<CylinderParams>
    {
        private enum VectorType_e
        {
            X,
            Y,
            Z
        }

        private Dictionary<VectorType_e, Vector> m_DefaultVectors;

        private IVectorMathService m_VecMathServ;

        public CylindricalStockFitExtractor(IVectorMathService vecMathServ)
        {
            m_VecMathServ = vecMathServ;

            m_DefaultVectors = new Dictionary<VectorType_e, Vector>()
            {
                { VectorType_e.X, new Vector(1, 0, 0) },
                { VectorType_e.Y, new Vector(0, 1, 0) },
                { VectorType_e.Z, new Vector(0, 0, 1) }
            };
        }

        public CylinderParams GetStockParameters(IGeometry geom, Vector heightDirection)
        {
            var yAxis = m_DefaultVectors[VectorType_e.Y];

            bool isAligned = yAxis.IsSame(heightDirection);

            TransformationMaxtrix alignTransform = null;

            if (!isAligned)
            {
                var origin = new Point(0, 0, 0);

                alignTransform = m_VecMathServ.GetTransformBetweenVectorsAroundPoint(
                    heightDirection, yAxis, origin);
            }

            Vector dir;
            Point rootPt;
            var height = GetHeight(geom, yAxis, alignTransform, out dir, out rootPt);

            Debug.Assert(height != 0);

            var perPoints = GetPerimeterPoints(geom, alignTransform).ConvertAll(p => new Nayuki.Point(p.X, p.Z));
            
            var enclosingCirc = Nayuki.SmallestEnclosingCircle.MakeCircle(perPoints);

            var circCenter = new Point(enclosingCirc.c.x, rootPt.Y, enclosingCirc.c.y);

            if (!isAligned)
            {
                var inversedAlignTransform = m_VecMathServ.InverseTransformationMatrix(alignTransform);

                circCenter = m_VecMathServ.TransformPoint(circCenter, inversedAlignTransform);
                dir = m_VecMathServ.TransformVector(dir, inversedAlignTransform);
            }

            double radius = enclosingCirc.r;

            return new CylinderParams(height, circCenter, dir, radius);
        }

        public CylinderParams FitToStockSizeByStep(CylinderParams cylParams, double step)
        {
            if (step > 0)
            {
                var radStep = step / 2;
                cylParams.Radius = Math.Ceiling(cylParams.Radius / radStep) * radStep;
            }

            return cylParams;
        }

        public CylinderParams ReCenter(Vector heightDirection, Point point, CylinderParams parameters)
        {
            var newOrigin = m_VecMathServ.ProjectPointOnVector(
                point, heightDirection, parameters.Origin);

            parameters.Radius += GetDistance(parameters.Origin, newOrigin);

            parameters.Origin = newOrigin;

            return parameters;
        }

        private double GetDistance(Point pt1, Point pt2)
        {
            return Math.Sqrt(Math.Pow(pt1.X - pt2.X, 2) + Math.Pow(pt1.Y - pt2.Y, 2) + Math.Pow(pt1.Z - pt2.Z, 2));
        }

        private List<Point> GetPerimeterPoints(IGeometry geom, TransformationMaxtrix alignTransform)
        {
            var perPoints = new List<Point>();

            var vectors = new Vector[]
            {
                m_DefaultVectors[VectorType_e.X],
                m_DefaultVectors[VectorType_e.Y],
                m_DefaultVectors[VectorType_e.Z]
            };

            foreach (var vec in vectors)
            {
                Point startPt;
                Point endPt;

                GetExtremePointsAligned(geom, vec, alignTransform, out startPt, out endPt);
                
                perPoints.Add(startPt);
                perPoints.Add(endPt);
            }

            return perPoints;
        }

        private double GetHeight(IGeometry geom, Vector dir, TransformationMaxtrix alignTransform,
            out Vector newDir, out Point rootPt)
        {
            Point endPt;
            dir = GetExtremePointsAligned(geom, dir, alignTransform, out rootPt, out endPt);

            double height = Math.Abs(endPt.Y - rootPt.Y);

            //direction might change depending on root point so recalculating the direction to be towards geometry
            newDir = new Vector(0, endPt.Y - rootPt.Y, 0);

            return height;
        }

        private Vector GetExtremePointsAligned(IGeometry geom, Vector dir,
            TransformationMaxtrix alignTransform, out Point rootPt, out Point endPt)
        {
            if (alignTransform != null)
            {
                dir = m_VecMathServ.TransformVector(dir, alignTransform);
            }

            geom.GetExtremePoints(dir, out rootPt, out endPt);

            if (alignTransform != null)
            {
                rootPt = m_VecMathServ.TransformPoint(rootPt, alignTransform);
                endPt = m_VecMathServ.TransformPoint(endPt, alignTransform);
            }

            return dir;
        }
    }
}
