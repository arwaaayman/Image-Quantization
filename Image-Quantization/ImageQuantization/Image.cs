using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImageQuantization
{
    class Image
    {
        
        public static List<RGBPixel> distinct;                                    //0(1)
        public static int distinctCount = 0;                                     //0(1)
       public static bool[] visited;                                           //0(1)
        //--------------------------------
        public static List<int>[] MSTlist;                                    //0(1)
        public static double MSTsum = 0;                                     //0(1)
        public static List<int>[] tree;                                     //0(1)
        public static List<Tuple<double, int, int>> edges;                 //0(1)
        public static Dictionary<int, HashSet<int>> vis;                  //0(1)
        public static int[] parentIndex;                                //0(1)
        public static double[] cost;                                   //0(1)
        public static char[] visit;                                   //0(1)
        //--------------------------------
        public static List<List<int>> clusterData;                          //0(1)
        public static List<int> curdata;                            //0(1)
        //-------------------------------
        public static RGBPixel[] colorMap;                        //0(1)
        //-----------------------------------
        public static RGBPixel[,] Quantized_Image;               //0(1)

        public static void Distinct(RGBPixel[,] img)    //O(L*H) => O(N^2)
        {
            bool[,,] visited = new bool[256, 256, 256]; //0(1)
            distinct = new List<RGBPixel>();           //0(1)

            for (int i = 0; i < img.GetLength(0); i++)   //O(L*H) => O(N^2)
            {
                for (int j = 0; j < img.GetLength(1); j++) //O(H) => O(N)
                {
                    if (visited[img[i, j].red, img[i, j].green, img[i, j].blue] == false)   //0(1)
                    {
                        visited[img[i, j].red, img[i, j].green, img[i, j].blue] = true; //0(1)
                        distinct.Add(img[i, j]);            //0(1)

                    }
                }
            }
            distinctCount = distinct.Count;                 //O(N)
        }
        public static double distance(int i, int j) //0(1)
        {
            double difred = distinct[i].red - distinct[j].red,
                difblue = distinct[i].blue - distinct[j].blue,
                difgreen = distinct[i].green - distinct[j].green;
            return Math.Sqrt((difred * difred) + (difblue * difblue) + (difgreen * difgreen));
        }


        public static void MST()                           //O(D^2)
        {
            MSTsum = 0;                                     //0(1)
            MSTlist = new List<int>[distinctCount];         //0(1)
            parentIndex = new int[distinctCount];           //0(1)
            visit = new char[distinctCount];                //0(1)
            cost = new double[distinctCount];               //0(1)

            for (int i = 0; i < distinctCount; i++)         //O(D)
            {
                visit[i] = 'N';                            //0(1)
                cost[i] = 1000000000;                       //0(1)
                if (i == distinctCount - 1)                 //0(1)
                {
                    cost[0] = 0;                            //0(1)
                    parentIndex[0] = -1;                    //0(1)
                }
            }
            for (int i = 0; i < distinctCount - 1; i++)     //O(D^2)
            {
                double max = 1000000000;                    //0(1)
                int smallest = -1;                          //0(1)
                ///get smallest node
                for (int j = 0; j < distinctCount; j++)         //O(D)
                {
                    if (visit[j] == 'N' && cost[j] < max)        //0(1)
                    {
                        max = cost[j];                           //0(1)
                        smallest = j;                            //0(1)
                    }
                }

                visit[smallest] = 'Y';                                  //0(1)
                for (int node = 0; node < distinctCount; node++)        //O(D)
                {
                    MSTlist[node] = new List<int>();                   //0(1)
                    if (visit[node] == 'Y')
                        continue;
                    if (distance(smallest, node) < cost[node])
                    {
                        parentIndex[node] = smallest;                //0(1)
                        cost[node] = distance(smallest, node);      //0(1)
                    }
                }
            }
            for (int j = 1; j < distinctCount; j++)         //O(D)
            {
                MSTlist[parentIndex[j]].Add(j);             //0(1)
                MSTsum += distance(parentIndex[j], j);      //0(1)
            }
        }

        public static void UpdateTree(int k)         //O(D*E)
        {
            /// get edges from MSTlist and sort them descending 
            tree = new List<int>[distinctCount];                 //O(1)
            edges = new List<Tuple<double, int, int>>();         //O(1)
            vis = new Dictionary<int, HashSet<int>>();          //O(1)
            for (int i = 0; i < distinctCount; i++)            //O(D)
            {
                tree[i] = new List<int>();                    //O(1)
                vis.Add(i, new HashSet<int>());              //O(1)
            }
            for (int i = 0; i < distinctCount; i++)         //O(D*E)
            {

                for (int j = 0; j < MSTlist[i].Count; j++)      //O(E)
                {
                    if (!vis[i].Contains(MSTlist[i][j]))
                    {
                        vis[i].Add(MSTlist[i][j]);               //O(1)
                        vis[MSTlist[i][j]].Add(i);               //O(1)
                        edges.Add(new Tuple<double, int, int>(distance(i, MSTlist[i][j]), i, MSTlist[i][j]));     //O(1)
                    }
                    else 
                    { 
                        continue;
                    }
                }
            }
            edges.Sort();                       //O(N log N)
            edges.Reverse();                     ///O(N)
            ///////////////////
                
            for (int j = (k - 1); j < edges.Count; j++)       //O(E)
            {
                tree[edges[j].Item2].Add(edges[j].Item3);     //O(1)    
                tree[edges[j].Item3].Add(edges[j].Item2);     //O(1)  
            }    
        }
        private static void DFS(int node, List<int> curdata)     //O(V)
        {
            curdata.Add(node);                           //O(1)
            visited[node] = true;                       //O(1)
            foreach (var newNode in tree[node])         //O(V)
            {
                if (visited[newNode] == false)          //O(1)
                {
                    DFS(newNode, curdata);           
                }
            }

        }

        public static void Clusters()                //O(D)
        {
            curdata = new List<int>();                    //O(1)
            clusterData = new List<List<int>>();          //O(1)
            visited = new bool[distinctCount];            //O(1)
            for (int i = 0; i < distinctCount; i++)       //O(D)
            {
                if (!visited[i])
                {
                    curdata = new List<int>();             //O(1)
                    DFS(i, curdata);
                    clusterData.Add(curdata);              //O(1)
                }
                else
                    continue;
            }
        }
              
        public static void Pallete()                            //O(K*N)
        {
            RGBPixel c;                                        //0(1)
            colorMap = new RGBPixel[distinctCount];            //0(1)
            int j;                                             //0(1)
            int red;                                           //0(1)
            int blue;                                          //0(1)
            int green;                                         //0(1)
            for (int i = 0; i < clusterData.Count; i++)        //O(K)
            {
                red = 0;                                       //0(1)
                blue = 0;                                     //0(1)
                green = 0;                                   //0(1)
                j = 0;
                while (j < clusterData[i].Count)                       //O(N)
                {
                    red += distinct[clusterData[i][j]].red;           //0(1)
                    blue += distinct[clusterData[i][j]].blue;        //0(1)
                    green += distinct[clusterData[i][j]].green;     //0(1)
                    j++;
                }
                c.red = (byte)(red / clusterData[i].Count);             //0(1)
                c.blue = (byte)(blue / clusterData[i].Count);          //0(1)
                c.green = (byte)(green / clusterData[i].Count);       //0(1)
                for (int k = 0; k < clusterData[i].Count; k++)       //O(N)
                {
                    colorMap[clusterData[i][k]] = c;                //0(1)
                }
            }
        }
        public static void UpdateColors(RGBPixel[,] img)                            //O(N^2)
        {

            RGBPixel[,,] NewColors = new RGBPixel[260, 260, 260];                   //0(1) 
            Quantized_Image = new RGBPixel[img.GetLength(0), img.GetLength(1)];     //0(1)

            for (int x = 0; x < distinctCount; x++)                                 //O(D)
            {
                NewColors[distinct[x].red, distinct[x].green, distinct[x].blue] = colorMap[x];  //0(1)
            }
            for (int i = 0; i < img.GetLength(0); i++)                  //O(L*H) => O(N^2)
            {
                for (int j = 0; j < img.GetLength(1); j++)              //O(L) => O(N)
                {
                    Quantized_Image[i, j] = NewColors[img[i, j].red, img[i, j].green, img[i, j].blue]; //0(1)
                }
            }
        }

    }
}