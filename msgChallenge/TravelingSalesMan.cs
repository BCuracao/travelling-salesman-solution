using msgChallenge.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

internal class TravelingSalesMan
{
    //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
    //::  Class references                                              :::
    //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
    private DistanceCalculator dc;

    private CityFactory cFac;

    //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
    //::  Local data                                                    :::
    //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

    // Holds all location names and their respectiv number in the .csv file
    private Dictionary<int, string> destinations;

    // Holds objects for class CityFactory
    private List<CityFactory> cityNodes;

    // 2D matrix that holds all possible connections for each individual city
    private int[,] graph;

    // List that represents the correct order to visit the cites
    private List<int> solutionRoute;

    // Data structure used to calculate the best route
    private Stack<int> stack;

    // Saves the total distance of the journey
    private int totalDistance;

    // Saves the distances between the start of the route and last destination.
    // Needed to calculate the total distance traveled.
    int distanceBetweenLastAndFirstCity;

    //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
    //::  Constructor                                                   :::
    //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
    public TravelingSalesMan()
    {
        // Initialize data structures
        stack = new Stack<int>();
        dc = new DistanceCalculator();
        destinations = new Dictionary<int, string>();
        cityNodes = new List<CityFactory>();

        // Load the entire contents of the CSV file stored in the Resources folder
        string inputData = Resources.msg_standorte_deutschland;

        // Read data line by line and prepare it for further processing
        StringReader strReader = new StringReader(inputData);

        List<string> csvData = new List<string>();


        string line = "";

        while ((line = strReader.ReadLine()) != null)
        {
            csvData.Add(line);
        }

        string[] arrData = csvData.ToArray();
        string[] arrReduced = new string[arrData.Length - 1];

        // Left shift to remove the first line as we dont need it
        Array.Copy(arrData, 1, arrReduced, 0, arrData.Length - 1);

        XElement xmlData = createXmlFromCsv(arrReduced);

        getUsefulInformation(xmlData);
        calculateDistances();
    }

    //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
    //::  Function to calcuate the distances all cities using the       :::
    //::  CityFactory class                                             :::
    //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
    private void calculateDistances()
    {
        // Matrix of all possible destinations of all cities
        graph = new int[cityNodes.Count, cityNodes.Count];

        // Holds the shortest travel route
        solutionRoute = new List<int>();

        for (int i = 0; i < cityNodes.Count; i++)
        {
            for (int j = 0; j < cityNodes.Count; j++)
            {
                double d = dc.distance(cityNodes[i].lat, cityNodes[i].lon, cityNodes[j].lat, cityNodes[j].lon, 'K');
                int distance = (int)d;
                graph[i, j] = distance;
            }
            destinations.Add(i, cityNodes[i].location);
        }

        // Print the distance matrix
        Console.WriteLine("Distance Matrix in the same order as in the provided .csv file: ");
        for (int i = 0; i < cityNodes.Count; i++)
        {
            Console.WriteLine();
            for (int j = 0; j < cityNodes.Count; j++)
            {
                Console.Write(graph[i, j] +" ");
            }
        }
        Console.WriteLine();
        Console.WriteLine();
        solveTheTsp(graph);
    }

    //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
    //::  Main function to determin the best route                      :::
    //::  using "nearest neighbour" algorithm                           :::
    //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
    private void solveTheTsp(int[,] graph)
    {
        int numberOfNodes = cityNodes.Count - 1;
        // Check if location has already been visited
        int[] visited = new int[numberOfNodes + 1];
        // Start is visited by default
        visited[0] = 1;
        // Push first value on the stack
        stack.Push(0);
        int element, dst = 0, i;
        // Keep track of the total distance traveled
        totalDistance = 0;
        // Holds the minimum distance
        int min = int.MaxValue;
        // Holds if we found a new minimum
        bool minFlag = false;
        // Add the start of the route (Munich) to the solution
        solutionRoute.Add(dst);

        while (stack.Count != 0)
        {
            element = stack.Peek();
            i = 0;
            min = int.MaxValue;

            while (i <= numberOfNodes)
            {
                if (graph[element, i] > 1 && visited[i] == 0)
                {
                    if (min > graph[element, i])
                    {
                        min = graph[element, i];
                        dst = i;
                        minFlag = true;
                    }
                }
                i++;
            }
            if (minFlag)
            {
                visited[dst] = 1;
                stack.Push(dst);
                solutionRoute.Add(dst);
                totalDistance += min;
                minFlag = false;
                continue;
            }
            stack.Pop();
            distanceBetweenLastAndFirstCity = graph[0, dst];
        }
 
        solutionRoute.Add(0);
        printSolution();
    }

    //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
    //::  Function to print the solution                                :::
    //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
    private void printSolution()
    {
        Console.WriteLine();
        Console.WriteLine("TOTAL DISTANCE TRAVELED: " + (totalDistance + distanceBetweenLastAndFirstCity) + " km" + "\n");
        Console.WriteLine("The shortest possible route using the 'nearest neightbour' algorithm is: " + "\n");
        for (int i = 0; i < solutionRoute.Count; i++)
        {
            Console.WriteLine(destinations[solutionRoute[i]]);
        }
    }

    //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
    //::  Function to transform the CSV file format to a XML format     :::
    //::  for further processing                                        :::
    //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
    private XElement createXmlFromCsv(string[] arrReduced)
    {
        XElement e = new XElement("Staedte",
            from str in arrReduced
            let fields = str.Split(',')
            select new XElement("Stadt",
              new XAttribute("Nr", fields[0]),
              new XAttribute("Standort", fields[1]),
              new XAttribute("Strasse", fields[2]),
              new XAttribute("Hausnummer", fields[3]),
              new XAttribute("PLZ", fields[4]),
              new XAttribute("Ort", fields[5]),
              new XAttribute("Breitengrad", fields[6]),
              new XAttribute("Laengengrad", fields[7])
            )
        );
        return e;
    }

    //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
    //::  Function to extract the useful information from the newly     :::
    //::  generated XML format. The only important information is the   :::
    //::  name of the destination and the lat / lon coordinates         :::
    //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
    private void getUsefulInformation(XElement xmlData)
    {
        IEnumerable<XElement> elements =
            from el in xmlData.Elements()
            select el;
        foreach (XElement el in elements)
        {
            cFac = new CityFactory(el);
            cityNodes.Add(cFac);
        }
    }

    //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
    //::  MAIN                                                          :::
    //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
    private static void Main(string[] args)
    {
        TravelingSalesMan tsm = new TravelingSalesMan();
    }
}