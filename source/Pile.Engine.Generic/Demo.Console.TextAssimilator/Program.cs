using System;
using System.Collections.Generic;
using System.Text;

using Pile.Engine.Generic;

namespace Demo.Console.TextAssimilator
{
    class Program
    {
        static void Main(string[] args)
        {
            TextAssimilator ta = new TextAssimilator();
            List<RelationBase> texts = new List<RelationBase>();

            while (true)
            {
                System.Console.Write("A(ssimilate, R(egenerate, C(ount: ");
                switch (System.Console.ReadLine().ToLower())
                {
                    case "a":
                        System.Console.Write(" Text: ");
                        string text = System.Console.ReadLine();
                        texts.Add(ta.Assimilate(text));
                        System.Console.WriteLine(" Text #{0} assimilated!", texts.Count-1);
                        break;

                    case "r":
                        System.Console.Write(" Index: ");
                        int index = int.Parse(System.Console.ReadLine());
                        System.Console.WriteLine(" Text '{0}'", ta.Generate(texts[index]));
                        break;

                    case "c":
                        System.Console.WriteLine(" {0} texts assimilated", texts.Count);
                        break;
                }
            }
        }
    }
}
