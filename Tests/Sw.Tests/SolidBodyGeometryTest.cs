using CodeStack.Community.DevTools.Sw.Testing.TempDisplay;
using CodeStack.Community.StockFit.Base.Math.Structures;
using CodeStack.Community.StockFit.Sw;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SolidWorks.Interop.sldworks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sw.Tests
{
    [TestClass]
    public class SolidBodyGeometryTest : SwUnitTest
    {
        [TestMethod]
        public void TestGetExtremePointsXYZAligned()
        {
            var points = Sw.App.WithDocument("Data\\t6.sldprt",
                m =>
                {
                    var body = Sw.Doc.GetBodyByName(m as IPartDoc, "Cut-Extrude1");

                    var pts = new List<double[]>();
                    var geom = new SolidBodyGeometry(body);

                    Point pt1;
                    Point pt2;

                    geom.GetExtremePoints(new Vector(1, 0, 0), out pt1, out pt2);
                    pts.Add(pt1.ToArray());
                    pts.Add(pt2.ToArray());

                    geom.GetExtremePoints(new Vector(0, 1, 0), out pt1, out pt2);
                    pts.Add(pt1.ToArray());
                    pts.Add(pt2.ToArray());

                    geom.GetExtremePoints(new Vector(0, 0, 1), out pt1, out pt2);
                    pts.Add(pt1.ToArray());
                    pts.Add(pt2.ToArray());

                    //TempDisplayPoints(m, pts);

                    return pts;
                });

            var comp = new DoubleEqualityComparerWithTolerance();
            
            Assert.IsTrue(points[0].AreEqualItemWise(new double[] { 0.00966608998524698, 0.019696737014236, 0.08 }, comp));
            Assert.IsTrue(points[1].AreEqualItemWise(new double[] { -0.0337478821675263, 0.019696737014236, 0.08 }, comp));
            Assert.IsTrue(points[2].AreEqualItemWise(new double[] { 0.00966608998524698, 0.019696737014236, 0.08 }, comp));
            Assert.IsTrue(points[3].AreEqualItemWise(new double[] { 0.00966608998524698, -0.0315523907373247, 0.08 }, comp));
            Assert.IsTrue(points[4].AreEqualItemWise(new double[] { 0.00966608998524698, 0.019696737014236, 0.08 }, comp));
            Assert.IsTrue(points[5].AreEqualItemWise(new double[] { -0.0207842984279311, 0.00525494529478507, 0 }, comp));
        }

        [TestMethod]
        public void TestGetExtremePointsXYZNotAligned()
        {
            var points = Sw.App.WithDocument("Data\\t2.sldprt",
                m =>
                {
                    var body = Sw.Doc.GetBodyByName(m as IPartDoc, "Body-Move/Copy2");

                    var pts = new List<double[]>();
                    var geom = new SolidBodyGeometry(body);

                    Point pt1;
                    Point pt2;

                    geom.GetExtremePoints(new Vector(-0.35659160468757367, -0.86064621310564815, -0.36349762493481108), out pt1, out pt2);
                    pts.Add(pt1.ToArray());
                    pts.Add(pt2.ToArray());

                    geom.GetExtremePoints(new Vector(-0.86064621310564815, -0.45399049973946992, -0.230609457770521), out pt1, out pt2);
                    pts.Add(pt1.ToArray());
                    pts.Add(pt2.ToArray());

                    geom.GetExtremePoints(new Vector(-0.363497624934811, -0.23060945777052097, 0.90260110494810375), out pt1, out pt2);
                    pts.Add(pt1.ToArray());
                    pts.Add(pt2.ToArray());

                    //TempDisplayPoints(m, pts);

                    return pts;
                });

            var comp = new DoubleEqualityComparerWithTolerance();

            Assert.IsTrue(points[0].AreEqualItemWise(new double[] { -0.110883845818614, 0.0605238203210351, -0.0239825852813666 }, comp));
            Assert.IsTrue(points[1].AreEqualItemWise(new double[] { -0.0412497473005035, 0.179859854385059, 0.0275862063054218 }, comp));
            Assert.IsTrue(points[2].AreEqualItemWise(new double[] { -0.142959784835457, 0.106453187928056, 0.00530736214458289 }, comp));
            Assert.IsTrue(points[3].AreEqualItemWise(new double[] { -0.0405325336202925, 0.171216718860278, 0.0419248817887173 }, comp));
            Assert.IsTrue(points[4].AreEqualItemWise(new double[] { -0.13321429487189, 0.0821558975737544, 0.0167696845654225 }, comp));
            Assert.IsTrue(points[5].AreEqualItemWise(new double[] { -0.0104076774276791, 0.149982352301203, -0.0286994667274928 }, comp));
        }

        private void TempDisplayPoints(IModelDoc2 m, List<double[]> pts)
        {
            foreach (var pt in pts)
            {
                Debug.Print($"{pt[0]}, {pt[1]}, {pt[2]}");
            }

            Sw.Doc.TempDisplayPoints(m, pts.Select(p => p), new SyncFormDisposeToken());
        }
    }
}
