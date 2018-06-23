using CodeStack.Community.StockFit.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeStack.Community.StockFit.Base.Math.Structures;
using CodeStack.Community.StockFit.Base.Math;

namespace CodeStack.Community.StockFit.Stocks.Cylinder
{
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

        private List<Point> GetPerimeterPoints(IGeometry geom, TransformationMaxtrix alignTransform)
        {
            var perPoints = new List<Point>();

            var vectors = new Vector[]
            {
                m_DefaultVectors[VectorType_e.X],
                m_DefaultVectors[VectorType_e.Z]
            };

            foreach (var vec in vectors)
            {
                Point startPt;
                Point endPt;

                geom.GetExtremePoints(vec, out startPt, out endPt);

                if (alignTransform != null)
                {
                    startPt = m_VecMathServ.TransformPoint(startPt, alignTransform);
                    endPt = m_VecMathServ.TransformPoint(endPt, alignTransform);
                }

                perPoints.Add(startPt);
                perPoints.Add(endPt);
            }

            return perPoints;
        }

        private double GetHeight(IGeometry geom, Vector dir, TransformationMaxtrix alignTransform,
            out Vector newDir, out Point rootPt)
        {
            Point endPt;
            geom.GetExtremePoints(dir, out rootPt, out endPt);

            if (alignTransform != null)
            {
                rootPt = m_VecMathServ.TransformPoint(rootPt, alignTransform);
                endPt = m_VecMathServ.TransformPoint(endPt, alignTransform);
            }

            double height = Math.Abs(endPt.Y - rootPt.Y);

            //direction might change depending on root point so recalculating the direction to be towards geometry
            newDir = new Vector(0, endPt.Y - rootPt.Y, 0);

            return height;
        }
    }
}
