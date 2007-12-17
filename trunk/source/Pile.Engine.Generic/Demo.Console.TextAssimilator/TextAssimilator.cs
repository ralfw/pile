using System;
using System.Collections.Generic;
using System.Text;

using Pile.Engine.Generic;

namespace Demo.Console.TextAssimilator
{
    internal class TextAssimilator
    {
        MemoryPile<RootRelation, InnerRelation> pile;


        public TextAssimilator()
        {
            this.pile = new MemoryPile<RootRelation, InnerRelation>();
        }


        public RelationBase Assimilate(string text)
        {
            RelationBase relTop = null;

            foreach (char c in text)
            {
                RootRelation relC = this.pile.Create(c.ToString());
                if (relTop != null)
                    relTop = this.pile.Create(relTop, relC);
                else
                    relTop = relC;
            }

            return relTop;
        }


        public string Generate(RelationBase top)
        {
            StringBuilder text = new StringBuilder();

            do
            {
                if (top is RootRelation)
                    text.Insert(0, (top as RootRelation).Key);
                else
                {
                    InnerRelation inner = top as InnerRelation;
                    text.Insert(0, ((RootRelation)inner.AssocParent).Key);
                    top = inner.NormParent;
                }
            } while (top is InnerRelation);
            text.Insert(0, ((RootRelation)top).Key);

            return text.ToString();
        }


        public string GenerateWithRecursion(RelationBase top)
        {
            if (top is RootRelation)
                return (top as RootRelation).Key;
            else
            {
                InnerRelation iRel = top as InnerRelation;
                return GenerateWithRecursion(iRel.NormParent) + ((RootRelation)iRel.AssocParent).Key;
            }
        }
    }
}
