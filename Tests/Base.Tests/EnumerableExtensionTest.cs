using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Base.Tests
{
    [TestClass]
    public class EnumerableExtensionTest
    {
        [TestMethod]
        public void TestAreEqualItemWise()
        {
            var arr1 = new double[] { 1, 2, 3 };
            var arr2 = new double[] { 1, 2, 3 };
            var arr3 = new double[] { 1, 4, 3 };
            var arr4 = new double[] { 1, 4, 3, 6 };
            var arr5 = new double[0];
            var arr6 = new double[0];
            double[] arr7 = null;
            double[] arr8 = null;

            var eq1 = arr1.AreEqualItemWise(arr2);
            var eq2 = arr2.AreEqualItemWise(arr1);
            var notEq1 = arr1.AreEqualItemWise(arr3);
            var notEq2 = arr1.AreEqualItemWise(arr4);
            var notEq3 = arr4.AreEqualItemWise(arr1);
            var eq3 = arr5.AreEqualItemWise(arr6);
            var notEq4 = arr1.AreEqualItemWise(arr7);
            var notEq5 = arr7.AreEqualItemWise(arr8);

            Assert.IsTrue(eq1);
            Assert.IsTrue(eq2);
            Assert.IsFalse(notEq1);
            Assert.IsFalse(notEq2);
            Assert.IsFalse(notEq3);
            Assert.IsTrue(eq3);
            Assert.IsFalse(notEq4);
            Assert.IsFalse(notEq5);
        }
    }
}
