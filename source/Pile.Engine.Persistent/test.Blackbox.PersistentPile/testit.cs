using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;

using Pile.Contracts;
using Pile.Engine.Persistent.VistaDb;

namespace test.Blackbox.PersistentPile
{
    [TestFixture]
    public class testit
    {
        [TearDown]
        public void afterEachTest()
        {
            System.IO.File.Delete("default.piledb");
            System.IO.File.Delete("test.piledb");
        }


        [Test]
        public void testOpenClose()
        {
            IPersistentPile pile;
            pile = new VistaDbPersistentPile("test.piledb");
            Assert.IsTrue(System.IO.File.Exists("test.piledb"));
            pile.Close();
            System.IO.File.Delete("test.piledb");

            pile = new VistaDbPersistentPile();
            pile.Open("test.piledb");
            Assert.IsTrue(System.IO.File.Exists("test.piledb"));
            pile.Close();

            pile = new VistaDbPersistentPile();
            pile.Open();
            Assert.IsTrue(System.IO.File.Exists("default.piledb"));
            pile.Close();
        }


        [Test]
        public void testCreateRoot()
        {
            using (IPersistentPile pile = new VistaDbPersistentPile("test.piledb"))
            {
                bool isNew;
                long r = pile.Create("a", out isNew);
                Assert.AreEqual(-1, r);
                Assert.IsTrue(isNew);
                Assert.AreEqual(r, pile.Create("a", out isNew));
                Assert.IsFalse(isNew);

                r = pile.Create();
                Assert.AreEqual(-2, r);
            }
        }

        [Test]
        public void testLookupRoot()
        {
            using (IPersistentPile pile = new VistaDbPersistentPile("test.piledb"))
            {
                long r = pile.Create("a");
                Assert.AreEqual(r, pile.Lookup("a"));

                Assert.AreEqual(0, pile.Lookup("b1234567890"));
            }
        }


        [Test]
        public void testCountOfRoots()
        {
            using (IPersistentPile pile = new VistaDbPersistentPile("test.piledb"))
            {
                Assert.AreEqual(0, pile.CountOfRoots);
                pile.Create();
                Assert.AreEqual(1, pile.CountOfRoots);
                pile.Create("a");
                Assert.AreEqual(2, pile.CountOfRoots);
            }
        }


        [Test]
        public void testIsRoot()
        {
            using (IPersistentPile pile = new VistaDbPersistentPile("test.piledb"))
            {
                Assert.IsTrue(pile.IsRoot(pile.Create()));
                Assert.IsFalse(pile.IsRoot(pile.Create(-1, -2)));
            }
        }


        [Test]
        public void testCreateInnerRelation()
        {
            using (IPersistentPile pile = new VistaDbPersistentPile("test.piledb"))
            {
                bool isNew;

                long r1 = pile.Create();
                long r2 = pile.Create();
                Assert.AreEqual(1, pile.Create(r1, r2, out isNew));
                Assert.IsTrue(isNew);
                Assert.AreEqual(1, pile.Create(r1, r2, out isNew));
                Assert.IsFalse(isNew);

                Assert.AreEqual(2, pile.Create(r2, r1, out isNew));
                Assert.IsTrue(isNew);
            }
        }


        [Test]
        public void testLookupInner()
        {
            using (IPersistentPile pile = new VistaDbPersistentPile("test.piledb"))
            {
                long r = pile.Create(-1, -2);
                Assert.AreEqual(r, pile.Lookup(-1, -2));

                Assert.AreEqual(0, pile.Lookup(-98, -99));
            }
        }


        [Test]
        public void testCountOfRelations()
        {
            using (IPersistentPile pile = new VistaDbPersistentPile("test.piledb"))
            {
                Assert.AreEqual(0, pile.CountOfRelations);
                pile.Create();
                Assert.AreEqual(1, pile.CountOfRelations);

                pile.Create(-1, -2);
                Assert.AreEqual(2, pile.CountOfRelations);
                Assert.AreEqual(1, pile.CountOfRoots);
            }
        }


        [Test]
        public void testTryGetParents()
        {
            using (IPersistentPile pile = new VistaDbPersistentPile("test.piledb"))
            {
                long nParent, aParent;
                Assert.IsFalse(pile.TryGetParents(15, out nParent, out aParent));

                long r = pile.Create(-1, -2);
                Assert.IsTrue(pile.TryGetParents(r, out nParent, out aParent));
                Assert.AreEqual(-1, nParent);
                Assert.AreEqual(-2, aParent);
            }
        }


        [Test]
        public void testGetQualifier()
        {
            using (IPersistentPile pile = new VistaDbPersistentPile("test.piledb"))
            {
                Assert.AreEqual(0, pile.GetQualifier(pile.Create()));
                Assert.AreEqual(1, pile.GetQualifier(pile.Create(1)));
                Assert.AreEqual(0, pile.GetQualifier(pile.Create(-1, -2)));
                Assert.AreEqual(2, pile.GetQualifier(pile.Create(-2, -3, 2)));
            }
        }


        [Test]
        public void testGetQualified()
        {
            using (IPersistentPile pile = new VistaDbPersistentPile("test.piledb"))
            {
                List<long> qualified;

                qualified = new List<long>(pile.GetQualified(99));
                Assert.AreEqual(0, qualified.Count);

                pile.Create(); // -1
                pile.Create(1); // -2
                pile.Create(2); // -3
                qualified = new List<long>(pile.GetQualified(1));
                Assert.AreEqual(1, qualified.Count);
                Assert.AreEqual(-2, qualified[0]);

                pile.Create(-1, -2); // 1
                pile.Create(-1, -3, 1); // 2
                pile.Create(-2, -1, 2); // 3
                pile.Create(-1, -4, 1); // 4
                qualified = new List<long>(pile.GetQualified(1));
                Assert.AreEqual(3, qualified.Count);
                Assert.AreEqual(-2, qualified[0]);
                Assert.AreEqual(2, qualified[1]);
                Assert.AreEqual(4, qualified[2]);

                qualified = new List<long>(pile.GetQualified(2));
                Assert.AreEqual(2, qualified.Count);
                Assert.AreEqual(-3, qualified[0]);
                Assert.AreEqual(3, qualified[1]);
            }
        }


        [Test]
        public void testGetChildren()
        {
            using (IPersistentPile pile = new VistaDbPersistentPile("test.piledb"))
            {
                List<long> children;

                pile.Create(1, 2);
                pile.Create(1, 3, 10);
                pile.Create(2, 3);
                pile.Create(2, 4, 10);
                pile.Create(1, 4, 11);
                pile.Create(5, 1, 10);

                children = new List<long>(pile.GetChildren(1, ParentModes.normative));
                Assert.AreEqual(3, children.Count);
                children = new List<long>(pile.GetChildren(1, ParentModes.normative, 10));
                Assert.AreEqual(1, children.Count);
                children = new List<long>(pile.GetChildren(1, ParentModes.associative));
                Assert.AreEqual(1, children.Count);
                children = new List<long>(pile.GetChildren(1, ParentModes.both));
                Assert.AreEqual(4, children.Count);
                children = new List<long>(pile.GetChildren(1, ParentModes.both, 10));
                Assert.AreEqual(2, children.Count);

                children = new List<long>(pile.GetChildren(2, ParentModes.normative));
                Assert.AreEqual(2, children.Count);
                children = new List<long>(pile.GetChildren(2, ParentModes.normative, 10));
                Assert.AreEqual(1, children.Count);
            }
        }
    }
}
