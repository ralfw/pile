using System;
using System.Collections.Generic;
using System.Text;

using Pile.Contracts;

namespace TextAssimilator
{
    public class TextAssimilator
    {
        #region vars, ctor
        IPile pile;
        long rootOfAllStrings;

        public TextAssimilator()
        {
            this.pile = new Pile.Engine.Transient.TransientPile();

            // create relation to connect all assimilated strings to
            this.rootOfAllStrings = this.pile.Create();
        }
        #endregion

        #region public functional interface
        public int Assimilate(string text)
        {
            byte[] bytes = System.Text.Encoding.Default.GetBytes(text);
            if (bytes.Length > 0)
            {
                long currTextTop = -(long)bytes[0];
                for (int i = 1; i < bytes.Length; i++)
                    currTextTop = this.pile.Create(currTextTop, -(long)bytes[i]);

                this.pile.Create(this.rootOfAllStrings, currTextTop);
            }
            else
                this.pile.Create(this.rootOfAllStrings, this.pile.Create());

            return new List<long>(this.pile.GetChildren(this.rootOfAllStrings, ParentModes.normative)).Count - 1;
        }


        public string Regenerate(int index)
        {
            List<long> stringRelations = new List<long>(this.pile.GetChildren(this.rootOfAllStrings, ParentModes.normative));
            if (index >= 0 && index < stringRelations.Count)
            {
                long nParent, currTextTop;
                this.pile.TryGetParents(stringRelations[index], out nParent, out currTextTop);
                if (!this.pile.IsRoot(currTextTop))
                {
                    List<byte> text = new List<byte>();
                    Regenerate(text, currTextTop);
                    return System.Text.Encoding.Default.GetString(text.ToArray());
                }
                else if (currTextTop < 0)
                    return System.Text.Encoding.Default.GetString(new byte[] { (byte)-currTextTop }); // single letter string
                else
                    return ""; // empty string
            }
            else
                return ""; // invalid index
        }

        private void Regenerate(List<byte> text, long currTextTop)
        {
            if (currTextTop < 0)
            {
                // leftmost char reached
                text.Add((byte)-currTextTop);
            }
            else
            {
                long nParent, aParent;
                this.pile.TryGetParents(currTextTop, out nParent, out aParent);
                Regenerate(text, nParent);
                text.Add((byte)-aParent);
            }
        }


        public int Count
        {
            get
            {
                return new List<long>(this.pile.GetChildren(this.rootOfAllStrings, ParentModes.normative)).Count;
            }
        }
        #endregion
    }
}
