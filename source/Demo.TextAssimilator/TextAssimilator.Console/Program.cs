using System;
using System.Collections.Generic;
using System.Text;

namespace TextAssimilator.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            TextAssimilator ta = new TextAssimilator();

            while (true)
            {
                System.Console.Write("A(ssimilate text, R(egenerate text, C(ount: ");
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
                }
            }
        }
    }
}
