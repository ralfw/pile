using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;

using Pile.Contracts;

namespace test.Blackbox.TransientPile
{

    [TestFixture]
    public class testit
    {
        [Test]
        public void testCreateRootAndCountRoot()
        {
            IPile p = new Pile.Engine.Transient.TransientPile();

            Assert.AreEqual(1, p.Create());
            Assert.AreEqual(1, p.CountOfRoots);
            Assert.AreEqual(2, p.Create());
            Assert.AreEqual(2, p.CountOfRoots);
        }


        [Test]
        public void testCreateQualifiedRoot()
        {
            IPile p = new Pile.Engine.Transient.TransientPile();

            List<long> qualified;

            p.Create();
            qualified = new List<long>(p.GetQualified(1));
            Assert.AreEqual(0, qualified.Count);

            p.Create(1);
            qualified = new List<long>(p.GetQualified(1));
            Assert.AreEqual(1, qualified.Count);

            p.Create(1);
            qualified = new List<long>(p.GetQualified(1));
            Assert.AreEqual(2, qualified.Count);

            p.Create(2);
            qualified = new List<long>(p.GetQualified(1));
            Assert.AreEqual(2, qualified.Count);
            qualified = new List<long>(p.GetQualified(2));
            Assert.AreEqual(1, qualified.Count);

            try
            {
                p.Create(-1);
                Assert.Fail();
            }
            catch (ArgumentOutOfRangeException)
            {
            }
            catch
            {
                Assert.Fail();
            }

            try
            {
                p.Create(99);
                Assert.Fail();
            }
            catch (ArgumentOutOfRangeException)
            {
            }
            catch
            {
                Assert.Fail();
            }
        }


        [Test]
        public void testCreateAndCount()
        {
            IPile p = new Pile.Engine.Transient.TransientPile();

            p.Create();
            p.Create();
            Assert.AreEqual(2, p.CountOfRelations);

            bool isNew;
            Assert.AreEqual(3, p.Create(1, 2, out isNew));
            Assert.IsTrue(isNew);
            Assert.AreEqual(3, p.CountOfRelations);
            Assert.AreEqual(2, p.CountOfRoots);

            Assert.AreEqual(3, p.Create(1, 2, out isNew));
            Assert.IsFalse(isNew);
            Assert.AreEqual(4, p.Create(2, 1));

            List<long> qualified;
            p.Create(3, 4, 1);
            qualified = new List<long>(p.GetQualified(1));
            Assert.AreEqual(1, qualified.Count);
        }


        [Test]
        public void testLookup()
        {
            IPile p = new Pile.Engine.Transient.TransientPile();

            p.Create();
            p.Create();

            Assert.AreEqual(0, p.Lookup(1, 2));

            p.Create(1, 2);
            Assert.AreEqual(3, p.Lookup(1, 2));
            Assert.AreEqual(0, p.Lookup(2, 1));
        }

        
        [Test]
        public void testIsRoot()
        {
            IPile p = new Pile.Engine.Transient.TransientPile();

            Assert.IsFalse(p.IsRoot(-1));
            Assert.IsFalse(p.IsRoot(1));
            p.Create();
            Assert.IsTrue(p.IsRoot(1));

            p.Create(1, 1);
            Assert.IsFalse(p.IsRoot(2));
        }


        [Test]
        public void testTryGetParents()
        {
            IPile p = new Pile.Engine.Transient.TransientPile();

            p.Create();
            p.Create();
            p.Create(1, 2);

            long n, a;
            Assert.IsTrue(p.TryGetParents(3, out n, out a));
            Assert.AreEqual(1, n);
            Assert.AreEqual(2, a);

            Assert.IsFalse(p.TryGetParents(1, out n, out a));
        }


        [Test]
        public void testGetChildren()
        {
            IPile p = new Pile.Engine.Transient.TransientPile();

            p.Create();
            p.Create();

            List<long> children;
            children = new List<long>(p.GetChildren(1, ParentModes.normative));
            Assert.AreEqual(0, children.Count);

            p.Create(1, 2);
            children = new List<long>(p.GetChildren(1, ParentModes.normative));
            Assert.AreEqual(1, children.Count);
            children = new List<long>(p.GetChildren(1, ParentModes.associative));
            Assert.AreEqual(0, children.Count);
            children = new List<long>(p.GetChildren(2, ParentModes.associative));
            Assert.AreEqual(1, children.Count);
            children = new List<long>(p.GetChildren(2, ParentModes.normative));
            Assert.AreEqual(0, children.Count);

            p.Create(1, 3);
            children = new List<long>(p.GetChildren(1, ParentModes.normative));
            Assert.AreEqual(2, children.Count);
            children = new List<long>(p.GetChildren(1, ParentModes.associative));
            Assert.AreEqual(0, children.Count);
            children = new List<long>(p.GetChildren(3, ParentModes.associative));
            Assert.AreEqual(1, children.Count);
            children = new List<long>(p.GetChildren(3, ParentModes.normative));
            Assert.AreEqual(0, children.Count);

            p.Create(2, 3);
            children = new List<long>(p.GetChildren(2, ParentModes.normative));
            Assert.AreEqual(1, children.Count);
            children = new List<long>(p.GetChildren(2, ParentModes.associative));
            Assert.AreEqual(1, children.Count);
            children = new List<long>(p.GetChildren(3, ParentModes.associative));
            Assert.AreEqual(2, children.Count);
            children = new List<long>(p.GetChildren(3, ParentModes.normative));
            Assert.AreEqual(0, children.Count);

            p.Create(4, 2, 1);
            children = new List<long>(p.GetChildren(4, ParentModes.normative));
            Assert.AreEqual(1, children.Count);
            children = new List<long>(p.GetChildren(2, ParentModes.associative));
            Assert.AreEqual(2, children.Count);

            children = new List<long>(p.GetChildren(2, ParentModes.associative, 1));
            Assert.AreEqual(1, children.Count);

            children = new List<long>(p.GetChildren(2, ParentModes.both));
            Assert.AreEqual(3, children.Count);
            children = new List<long>(p.GetChildren(2, ParentModes.both, 1));
            Assert.AreEqual(1, children.Count);
        }
    }
}
