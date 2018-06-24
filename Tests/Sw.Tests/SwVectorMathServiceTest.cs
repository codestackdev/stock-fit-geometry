using CodeStack.Community.DevTools.Sw.Testing.TempDisplay;
using CodeStack.Community.StockFit.Base.Math.Structures;
using CodeStack.Community.StockFit.Sw.Math;
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
    public class SwVectorMathServiceTest : SwUnitTest
    {
        [TestMethod]
        public void TestGetTransformBetweenVectorsAroundPointYZ90Deg()
        {
            var comp = new DoubleEqualityComparerWithTolerance();

            var mathServ = new SwVectorMathService(Sw.SldWorks.IGetMathUtility());

            var transform = mathServ.GetTransformBetweenVectorsAroundPoint(new Vector(0, 0, 1),
                new Vector(0, 1, 0), new Point(0, 0, 0));

            //TestDisplayOnBody(transform, "Data\\t5.sldprt", "Boss-Extrude2");

            Assert.IsTrue(transform.Translation.ToArray().AreEqualItemWise(new double[] { 0, 0, 0 }, comp));
            Assert.IsTrue(transform.Rotation.ToArray().AreEqualItemWise(new double[] { 1, 0, 0, 0, 0, -1, 0, 1, 0 }, comp));
            Assert.AreEqual(1, transform.Scale);
        }

        [TestMethod]
        public void TestGetTransformBetweenVectorsAroundPointAllDirs()
        {
            var comp = new DoubleEqualityComparerWithTolerance();

            var mathServ = new SwVectorMathService(Sw.SldWorks.IGetMathUtility());

            var transform = mathServ.GetTransformBetweenVectorsAroundPoint(
                new Vector(-0.86064621310564815, -0.45399049973946992, -0.230609457770521),
                new Vector(0, 1, 0), new Point(0, 0, 0));

            //TestDisplayOnBody(transform, "Data\\t2.sldprt", "Body-Move/Copy2");

            foreach (var d in transform.Rotation.ToArray())
            {
                Debug.Print(d.ToString());
            }

            Assert.IsTrue(transform.Translation.ToArray().AreEqualItemWise(new double[] { 0, 0, 0 }, comp));

            Assert.IsTrue(transform.Rotation.ToArray().AreEqualItemWise(new double[]
            {
                -0.356591604687574, -0.860646213105648, -0.363497624934811,
                0.860646213105648, -0.45399049973947, 0.230609457770521,
                -0.363497624934811, -0.230609457770521, 0.902601104948104 }
            , comp));

            Assert.AreEqual(1, transform.Scale);
        }

        [TestMethod]
        public void TestTransformPoint()
        {
            var comp = new DoubleEqualityComparerWithTolerance();

            var mathServ = new SwVectorMathService(Sw.SldWorks.IGetMathUtility());

            var transform = new TransformationMaxtrix(
                new RotationMatrix(-0.356591604687574, -0.860646213105648, -0.363497624934811,
                0.860646213105648, -0.45399049973947, 0.230609457770521,
                -0.363497624934811, -0.230609457770521, 0.902601104948104), new Vector(0, 0, 0), 1);

            var pts = new Point[6];

            pts[0] = new Point(-0.110883845818614, 0.0605238203210351, -0.0239825852813666);
            pts[1] = new Point(-0.0412497473005035, 0.179859854385059, 0.0275862063054218);
            pts[2] = new Point(-0.142959784835457, 0.106453187928056, 0.00530736214458289);
            pts[3] = new Point(-0.0405325336202925, 0.171216718860278, 0.0419248817887173);
            pts[4] = new Point(-0.13321429487189, 0.0821558975737544, 0.0167696845654225);
            pts[5] = new Point(-0.0104076774276791, 0.149982352301203, -0.0286994667274928);

            var ptsTransf = new Point[6];

            for (int i = 0; i < pts.Length; i++)
            {
                ptsTransf[i] = mathServ.TransformPoint(pts[i], transform);
            }

            //TestDisplayOnBody(transform, "Data\\t2.sldprt", "Body-Move/Copy2", ptsTransf);

            Assert.IsTrue(ptsTransf[0].ToArray().AreEqualItemWise(new double[] { 0.100347458065948, 0.0734851335523633, 0.0326166720106647 }, comp));
            Assert.IsTrue(ptsTransf[1].ToArray().AreEqualItemWise(new double[] { 0.159477495676095, -0.0525148664476371, 0.0813709089599175 }, comp));
            Assert.IsTrue(ptsTransf[2].ToArray().AreEqualItemWise(new double[] { 0.140667578609346, 0.0734851335523624, 0.0813050851309728 }, comp));
            Assert.IsTrue(ptsTransf[3].ToArray().AreEqualItemWise(new double[] { 0.146570986957314, -0.0525148664476372, 0.092059119028498 }, comp));
            Assert.IsTrue(ptsTransf[4].ToArray().AreEqualItemWise(new double[] { 0.112114520796401, 0.073485133552363, 0.082505342603813 }, comp));
            Assert.IsTrue(ptsTransf[5].ToArray().AreEqualItemWise(new double[] { 0.143225221928054, -0.0525148664476371, 0.0124663445857203 }, comp));
        }

        private void TestDisplayOnBody(TransformationMaxtrix transform, string fileName, string bodyName, 
            IEnumerable<Point> points = null)
        {
            Sw.App.WithDocument<object>(fileName,
                m =>
                {
                    var body = Sw.Doc.GetBodyByName(m as IPartDoc, bodyName);

                    var mathTx = (Sw.SldWorks.IGetMathUtility() as IMathUtility).CreateTransFormTransformationMaxtrix(transform);

                    var tempBody = body.ICopy();
                    tempBody.ApplyTransform(mathTx as MathTransform);

                    ManualDisposeToken pointsDispToken = null;

                    if (points != null)
                    {
                        pointsDispToken = Sw.Doc.TempDisplayPoints(m, points.Select(p => p.ToArray()));
                    }

                    Sw.Doc.TempDisplayBody(tempBody, m, new SyncFormDisposeToken());

                    if (pointsDispToken != null)
                    {
                        pointsDispToken.Dispose();
                    }

                    return null;
                });
        }
    }
}
