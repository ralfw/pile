using System;
using System.Collections.Generic;
using System.Text;

namespace TextAssimilator.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            using (PersistentTextAssimilator ta = new PersistentTextAssimilator("texts.piledb"))
            {
                bool stopped = false;
                do
                {
                    System.Console.Write("A(ssimilate text, R(egenerate text, C(ount, eX(it: ");
                    switch (System.Console.ReadLine().ToLower())
                    {
                        case "a":
                            System.Console.Write(" text: ");
                            string text = System.Console.ReadLine();
                            System.Console.WriteLine(" Text #{0} assimilated", ta.Assimilate(text));
                            break;
                        case "r":
                            System.Console.Write(" index: ");
                            int index = int.Parse(System.Console.ReadLine());
                            System.Console.WriteLine(" Text '{0}'", ta.Regenerate(index));
                            break;
                        case "c":
                            System.Console.WriteLine(" {0} texts assimilated", ta.Count);
                            break;
                        case "x":
                            stopped = true;
                            break;
                    }
                } while (!stopped);
            }
        }
    }
}
