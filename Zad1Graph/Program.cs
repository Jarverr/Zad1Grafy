using System;
using System.Collections.Generic;
using System.IO;
using Graphviz4Net.Dot;
using Graphviz4Net.Dot.AntlrParser;
//using NUnit.Framework;
using System.Linq;
using Antlr.Runtime;
using System.Text.RegularExpressions;
using Graphviz4Net.Graphs;
using System.Threading;
using System.Diagnostics;

namespace Zad1Graph
{
    class Program
    {
        static void Main(string[] args)
        {
            bool success = true;
            Stopwatch sw = new Stopwatch();
            if (args.Length == 1)
            {

                //args[0] = "GraphToTest.dt";
                using var sr = new StreamReader(args[0]);
                sw.Start();
                int counter = 0;
                //Console.WriteLine(sr.ReadToEnd());
                var graph = Parse(sr.ReadToEnd());
                //Console.WriteLine(graph+"\n\n");
                //Dictionary<List<int>,List<int>> edgesConnection= new Dictionary<List<int>, List<int>>();
                Dictionary<int, int> edgesSource = new Dictionary<int, int>();
                Dictionary<int, int> edgesDestination = new Dictionary<int, int>();
                Dictionary<int, List<int>> edgesConnection = new Dictionary<int, List<int>>();
                List<int> vGroup = new List<int>();
                List<int> uGroup = new List<int>();
                Tuple<int, int> transformed;
                bool isInUGroup = false;
                ///System zbierania danych o grafie
                foreach (var item in graph.AllVertices.ToList())
                {
                    //vGroup.Add(item.Id);
                    edgesDestination.Add(item.Id, 0);
                    edgesSource.Add(item.Id, 0);
                    edgesConnection.Add(item.Id, new List<int>());
                }

                foreach (var item in graph.Edges.ToList())
                {
                    transformed = transformEdgeIdToInt(item, "Destination");
                    edgesSource[transformed.Item1]++;
                    edgesDestination[transformed.Item2]++;
                    edgesConnection[transformed.Item1].Add(transformed.Item2);
                }

                ///System sprawdzania krawedzi

                //foreach (var item in graph.GetAllVertices().ToList())
                //sprawdzenie braku wyjść z wierzchołka -> dodaj do U
                foreach (var item in graph.AllVertices.ToList())
                {
                    if (edgesSource[item.Id] == 0)
                    {
                        uGroup.Add(item.Id);

                    }
                    else
                    {
                        //sprawdzenie braku wejść do wierzchołka dodja do V
                        if (edgesDestination[item.Id] == 0)
                        {
                            //   vGroup.Add(item.Id);
                        }
                    }

                }

                //proram

                do
                {
                    //pewniak
                    hasConnectionWithUGroupe(graph, edgesConnection, vGroup, uGroup);
                    //pewniak
                    iCannotBeU(graph, edgesConnection, vGroup, uGroup);
                    //pewniak
                    checkMyVGroupNeighbours(graph, uGroup, vGroup, edgesConnection, isInUGroup);
                    //iHaveJustGetIntoV(graph,uGroup,vGroup,edgesConnection);


                   //los
                    //groupVHasConnnectionTO(vGroup, edgesConnection, uGroup);
                    

                    if (counter == vGroup.Count + uGroup.Count)
                    {
                        //taki pewniak ale nie przetestowany na wszystkie sposoby
                        checkSpecialSit(graph, uGroup, vGroup, edgesConnection);

                        //TakeOneVertexFromTheRest(graph, vGroup, uGroup, edgesConnection, isInUGroup);

                        if (counter == vGroup.Count + uGroup.Count)
                        {
                            success = false;
                            break;
                        }
                    }
                  
                    counter = vGroup.Count + uGroup.Count;
                } while (vGroup.Count + uGroup.Count < graph.Vertices.Count());
                


                ///Wypisywanie danych
                sw.Stop();
                vGroup.Sort();
                uGroup.Sort();
                if (success)
                {
                    Console.WriteLine("\nV Group:");
                    foreach (var item in vGroup)
                    {
                        Console.Write("{0} ", item);
                    }
                    Console.WriteLine("\nU Group:");
                    foreach (var item in uGroup)
                    {
                        Console.Write("{0} ", item);

                    }
                }
                else
                {
                    Console.WriteLine("Nie udało się pogrupować wierzchołków grafu");
                }
                Console.WriteLine("\nEllapsed time is {0}", sw.Elapsed);
            }
            else
            {
                Console.WriteLine("Podaj arguemnt wejściowy przy uruchamianiu programu.");
            }
        }

        //private static void iHaveJustGetIntoV(DotGraph<int> graph, List<int> uGroup, List<int> vGroup, Dictionary<int, List<int>> edgesConnection)
        //{
            
        //    int counter;
        //    foreach (var item in graph.AllVertices.ToList())
        //    {
        //        if (!uGroup.Contains(item.Id)||!vGroup.Contains(item.Id))
        //        {
        //            counter = 0;
        //            foreach (var vert in edgesConnection)
        //            {
        //                if (vert.Key==item.Id)
        //                {
        //                    foreach (var vert2 in vert.Value)
        //                    {
        //                        if (!uGroup.Contains(vert2)&&vGroup.Contains(vert2))
        //                        {
        //                            counter++;
        //                            if (vert.Value.Count==counter)
        //                            {
        //                                uGroup.Add(item.Id);
        //                            }
        //                        }
        //                        else
        //                        {
        //                            break;
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}

        private static void checkSpecialSit(DotGraph<int> graph, List<int> uGroup, List<int> vGroup, Dictionary<int, List<int>> edgesConnection)
        {
            Random rnd = new Random();
            int counter;
            int numToCheck;
            bool doIHaveResponse;
            foreach (var item in edgesConnection)
            {
                if (!uGroup.Contains(item.Key) && !vGroup.Contains(item.Key))
                {
                    numToCheck = int.MaxValue;
                    doIHaveResponse = false;
                    counter = item.Value.Count;
                    foreach (var vertex in item.Value)
                    {
                        if (vGroup.Contains(vertex) && !uGroup.Contains(vertex))
                        {
                            counter--;
                        }
                        else
                        {
                            numToCheck = vertex;
                        }
                    }
                    foreach (var edges in edgesConnection)
                    {
                        if (edges.Key==numToCheck)
                        {
                            if (edges.Value.Contains(item.Key))
                            {
                                doIHaveResponse = true;
                            }
                        }
                    }
                    if (counter == 1&&doIHaveResponse)
                    {

                        if (rnd.Next(0, 100) > 50)
                        {
                            uGroup.Add(item.Key);
                            break;
                        }
                        else
                        {
                            vGroup.Add(item.Key);
                            break;
                        }
                    }
                }
            }
        }

        private static void iCannotBeU(DotGraph<int> graph, Dictionary<int, List<int>> edgesConnection, List<int> vGroup, List<int> uGroup)
        {
            foreach (var item in edgesConnection)
            {
                if(uGroup.Contains(item.Key))
                {
                    foreach (var dest in item.Value)
                    {
                        if (!uGroup.Contains(dest)&&!vGroup.Contains(dest))
                        {
                            vGroup.Add(dest);
                        }
                    }
                }
            }
        }

        private static void checkMyVGroupNeighbours(DotGraph<int> graph, List<int> uGroup, List<int> vGroup, Dictionary<int, List<int>> edgesConnection, bool sthGoesWrong)
        {
            foreach (var item in graph.AllVertices.ToList())
            {
                sthGoesWrong = false;
                if (!uGroup.Contains(item.Id) && !vGroup.Contains(item.Id))
                {
                    foreach(int vertex in edgesConnection[item.Id])
                    {
                        if (!vGroup.Contains(vertex))
                        {
                            sthGoesWrong = true;
                            break;
                        }
                        
                    }
                    if (sthGoesWrong)
                    {
                        continue;
                    }

                    foreach (var vertex in uGroup)
                    {
                        if (edgesConnection[item.Id].Contains(vertex)||edgesConnection[vertex].Contains(item.Id))
                        {
                            sthGoesWrong = true;
                            break;
                        }
                    }
                    if (sthGoesWrong)
                    {
                        continue;
                    }
                    uGroup.Add(item.Id);
                }
            }
        }

        private static void TakeOneVertexFromTheRest(DotGraph<int> graph, List<int> vGroup, List<int> uGroup, Dictionary<int, List<int>> edgesConnection, bool isInUGroup)
        {
            isInUGroup = false;
            foreach (var item in graph.AllVertices.ToList())
            {
                if (!uGroup.Contains(item.Id) && !vGroup.Contains(item.Id))
                {
                    foreach (var vertex in uGroup)
                    {
                        if (edgesConnection[item.Id].Contains(vertex))
                        {
                            isInUGroup = true;
                        }

                    }


                }
                if (!uGroup.Contains(item.Id) && !vGroup.Contains(item.Id) && !isInUGroup)
                {
                    uGroup.Add(item.Id);
                    break;
                }
            }
        }

        private static void groupVHasConnnectionTO(List<int> vGroup, Dictionary<int, List<int>> edgesConnection, List<int> uGroup)
        {
            bool can = true;
            foreach (var item in vGroup)
            {
                foreach (var connected in edgesConnection[item])//elementy z którymi jest połączony gość z v groupy
                {
                    if (vGroup.Contains(connected)||uGroup.Contains(connected))
                    {
                        continue;
                    }
                    can = true;
                    //sprawdzenie czy  wierzchołek ten connected ma połaczenie z ugrupia
                    foreach (var vertex in uGroup)
                    {
                        if (edgesConnection[vertex].Contains(connected) || edgesConnection[connected].Contains(vertex))
                        {
                            can = false;
                            break;
                        }
                    }

                    //dodja wierzchołki z którymi łącza się goście z V grupy do U grupy
                    //TU
                        if (!uGroup.Contains(connected) && !vGroup.Contains(connected) && can)
                        {
                            uGroup.Add(connected);
                        }
                }
            }
        }

        private static void hasConnectionWithUGroupe(DotGraph<int> graph, Dictionary<int, List<int>> edgesConnection, List<int> vGroup, List<int> uGroup)
        {
            foreach (var item in graph.AllVertices.ToList())
            {
                if (uGroup.Contains(item.Id))
                {
                    continue;
                }
                foreach (var connected in edgesConnection[item.Id])
                {
                    //jezeli cos ma połączenie z wierzcholkiem z U  dodja go do V
                    if (uGroup.Contains(connected))
                    {
                        if (!vGroup.Contains(item.Id))
                        {
                            vGroup.Add(item.Id);
                            break;
                        }
                    }
                }
            }
        }

        private static Tuple<int, int> transformEdgeIdToInt(IEdge edge, string whichEndOfEdge)
        {
            int charLoc;
            int idS, idD;

            charLoc = edge.ToString().IndexOf('-');
            Int32.TryParse(edge.Source.ToString().Trim(new char[] { '{', '}' }), out idS);
            Int32.TryParse(edge.Destination.ToString().Trim(new char[] { '{', '}' }), out idD);
            return new Tuple<int, int>(idS, idD);
        }

        private static DotGraph<int> Parse(string content)
        {
            var antlrStream = new ANTLRStringStream(content);
            var lexer = new DotGrammarLexer(antlrStream);
            var tokenStream = new CommonTokenStream(lexer);
            var parser = new DotGrammarParser(tokenStream);
            var builder = new IntDotGraphBuilder();
            parser.Builder = builder;
            parser.dot();
            return builder.DotGraph;
        }
    }
}
