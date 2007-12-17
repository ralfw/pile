using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;
using Pile.Engine.Generic;

namespace test.blackbox.pile
{
    [TestFixture]
    public class tests
    {
        [Test]
        public void testCreateTV()
        {
            MemoryPile<RootRelation, InnerRelation> p;
            p = new MemoryPile<RootRelation, InnerRelation>();

            Assert.AreEqual(0, p.CountTV);

            RootRelation tv;
            tv = p.Create("a");
            Assert.AreEqual(1, p.CountTV);
            Assert.AreEqual("a", tv.Key);
            Assert.IsTrue(tv.Id > 0);

            tv = p.Create("b");
            Assert.AreEqual(2, p.CountTV);
            Assert.AreEqual("b", tv.Key);

            bool isNew;
            tv = p.Create("c", out isNew);
            Assert.IsTrue(isNew);
            Assert.AreEqual(3, p.CountTV);

            tv = p.Create("a", out isNew);
            Assert.IsFalse(isNew);
            Assert.AreEqual(3, p.CountTV);
        }


        [Test]
        public void testCreate()
        {
            MemoryPile<RootRelation, InnerRelation> p;
            p = new MemoryPile<RootRelation, InnerRelation>();

            RootRelation tv1, tv2;
            InnerRelation ir;
            bool isNew;

            tv1 = p.Create("a");
            tv2 = p.Create("b");

            Assert.AreEqual(0, p.CountInnerRelation);
            ir = p.Create(tv1, tv2, out isNew);
            Assert.AreEqual(1, p.CountInnerRelation);
            Assert.IsTrue(ir.Id == tv2.Id+1);
            Assert.IsTrue(isNew);

            ir = p.Create(tv2, tv1, out isNew);
            Assert.AreEqual(2, p.CountInnerRelation);
            Assert.IsTrue(isNew);

            ir = p.Create(tv1, tv2, out isNew);
            Assert.AreEqual(2, p.CountInnerRelation);
            Assert.IsTrue(ir.Id == tv2.Id + 1);
            Assert.IsFalse(isNew);
        }


        [Test]
        public void testParents()
        {
            MemoryPile<RootRelation, InnerRelation> p;
            p = new MemoryPile<RootRelation, InnerRelation>();

            RootRelation tv1, tv2;
            InnerRelation ir;

            tv1 = p.Create("a");
            tv2 = p.Create("b");

            ir = p.Create(tv1, tv2);
            Assert.AreSame(tv1, ir.NormParent);
            Assert.AreSame(tv2, ir.AssocParent);
        }


        [Test]
        public void testGet()
        {
            MemoryPile<RootRelation, InnerRelation> p;
            p = new MemoryPile<RootRelation, InnerRelation>();

            RootRelation tv1, tv2;
            InnerRelation ir;

            tv1 = p.Create("a");
            tv2 = p.Create("b");
            ir = p.Create(tv1, tv2);

            Assert.AreSame(ir, p.Get(tv1, tv2));
            Assert.IsNull(p.Get(tv2, tv1));
        }


#if WITH_CHILDREN
        [Test]
        public void testChildren()
        {
            MemoryPile<RootRelation, InnerRelation> p;
            p = new MemoryPile<RootRelation, InnerRelation>();

            RootRelation tv1, tv2;
            InnerRelation ir;

            tv1 = p.Create("a");
            Assert.AreEqual(0, tv1.NormChildren.Length);
            Assert.AreEqual(0, tv1.AssocChildren.Length);

            tv2 = p.Create("b");
            Assert.AreEqual(0, tv2.NormChildren.Length);
            Assert.AreEqual(0, tv2.AssocChildren.Length);

            ir = p.Create(tv1, tv2);
            Assert.AreEqual(0, ir.NormChildren.Length);
            Assert.AreEqual(0, ir.AssocChildren.Length);
            Assert.AreEqual(1, tv1.NormChildren.Length);
            Assert.AreEqual(0, tv1.AssocChildren.Length);
            Assert.AreEqual(0, tv2.NormChildren.Length);
            Assert.AreEqual(1, tv2.AssocChildren.Length);

            ir = p.Create(tv1, tv2);
            Assert.AreEqual(1, tv1.NormChildren.Length);
            Assert.AreEqual(1, tv2.AssocChildren.Length);

            p.Create(tv1, ir);
            Assert.AreEqual(2, tv1.NormChildren.Length);
            Assert.AreEqual(0, tv1.AssocChildren.Length);
            Assert.AreEqual(0, ir.NormChildren.Length);
            Assert.AreEqual(1, ir.AssocChildren.Length);
        }
#endif

    }
}
