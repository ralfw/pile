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
                Assert.AreEqual(1, r);
                Assert.IsTrue(isNew);
                Assert.AreEqual(r, pile.Create("a", out isNew));
                Assert.IsFalse(isNew);

                r = pile.Create();
                Assert.AreEqual(2, r);
            }
        }

        [Test]
        public void testLookupRoot()
        {
            using (IPersistentPile pile = new VistaDbPersistentPile("test.piledb"))
            {
                long r = pile.Create("a");
                Assert.AreEqual(r, pile.Lookup("a"));

                try
                {
                    pile.Lookup("b1234567890");
                    Assert.Fail();
                }
                catch (ApplicationException ex)
                {
                    Console.WriteLine("*** error msg: {0}", ex.Message);
                }
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
                pile.Create("a");
                Assert.AreEqual(2, pile.CountOfRelations);
            }
        }
    }
}
