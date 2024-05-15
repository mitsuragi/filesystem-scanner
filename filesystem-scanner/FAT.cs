using System.Text.RegularExpressions;

namespace filesystem_scanner
{
    public class FAT
    {
        private enum Type
        {
            EMPTY = -1,
            EOF = -2,
            BAD = -3
        };

        private int[] clusters = {-1, -1, 3, 4, 5, 6, -2, -1, 9, 26,
                    -3, -2, -1, -1, 16, -3, 26, -1, -1, -1,
                    -1, -3, 23, 26, -1, -1, 27, 28, -2, -1,
                    -1, -1, -2, 32, 35, 36, 37, 38, -2, -1,
                    -1, -1, -1, -3, -1, -1, -1, -1, -1, -1};

        private Dictionary<char, int> files = new Dictionary<char, int>()
        {
            ['A'] = 2,
            ['B'] = 14,
            ['C'] = 22,
            ['D'] = 8
        };

        private Dictionary<char, List<int>> filesClusters = new Dictionary<char, List<int>>();
        private Dictionary<int, List<int>> lostFilesClusters = new Dictionary<int, List<int>>();
        private bool isFixed = false; 

        public Dictionary<char, List<int>> getFilesClusters()
        {
            findFilesClusters();
            return filesClusters;
        }

        public Dictionary<int, List<int>> getLostFilesClusters()
        {
            findLostFilesClusters();
            return lostFilesClusters;
        }

        public List<int> getIndexesOfBadFiles()
        {
            List<int> indexes = new List<int>();
            
            for(int i = 0; i < clusters.Length; i++)
            {
                if (clusters[i] == (int)Type.BAD)
                {
                    indexes.Add(i);
                }
            }

            return indexes;
        }

        public void displayClusters()
        {
            string str = "";
            for (int i = 0; i < 10; i++)
            {
                str += $"{i}: {clusters[i]}\t{i + 10}: {clusters[i + 10]}\t{i + 20}: {clusters[i + 20]}\t{i + 30}: {clusters[i + 30]}\t{i + 40}: {clusters[i + 40]}\n";
                str = str.Replace("-1", "");
                str = str.Replace("-2", "EOF");
                str = str.Replace("-3", "BAD");
            }
            Console.WriteLine(str);
        }
        public void displayFiles()
        {
            foreach (var file in files)
            {
                Console.WriteLine($"{file.Key} - {file.Value}");
            }
        }

        private void findFilesClusters()
        {
            if (filesClusters.Count != 0)
            {
                return;
            }

            foreach(var file in files)
            {
                List<int> tempClusters = new List<int>();
                int i = file.Value;

                while (i != (int)Type.EOF)
                {
                    tempClusters.Add(i);
                    i = clusters[i];
                }

                filesClusters.Add(file.Key, tempClusters);
            }
        }

        private void findLostFilesClusters()
        {
            if (lostFilesClusters.Count != 0)
            {
                return;
            }

            if (filesClusters.Count != 0)
            {
                findFilesClusters();
            }

            List<int> tempClusters = new List<int>();

            foreach(var file in filesClusters)
            {
                tempClusters.AddRange(file.Value);
            }

            for(int i = 0; i < clusters.Length; i++)
            {
                if (clusters[i] == (int)Type.EOF)
                {
                    if (tempClusters.Contains(i))
                    {
                        continue;
                    }

                    int currentIndex = i;
                    lostFilesClusters.Add(i, new List<int>());
                    lostFilesClusters[i].Add(currentIndex);

                    bool flag = true;
                    while (flag)
                    {
                        flag = false;

                        for (int j = 0; j < clusters.Length; j++)
                        {
                            if (clusters[j] == currentIndex)
                            {
                                flag = true;
                                currentIndex = j;
                                lostFilesClusters[i].Add(j);
                                break;
                            }
                        }
                    }

                    lostFilesClusters[i].Reverse();
                }
            }
        }

        public void fixDuplicateClusters()
        {
            if (isFixed)
            {
                return;
            }

            if (filesClusters.Count != 0)
            {
                findFilesClusters();
            }

            HashSet<int> usedClasters = new HashSet<int>();

            foreach (var file in filesClusters)
            {
                int counter = 0;
                int firstDuplicateIndex = -1;
                bool flag = false;
                for (int i = 0; i < file.Value.Count; i++)
                {
                    if (usedClasters.Contains(file.Value[i]))
                    {
                        counter++;
                        if (firstDuplicateIndex == -1)
                        {
                            firstDuplicateIndex = i;
                        }
                    }
                    else
                    {
                        usedClasters.Add(file.Value[i]);
                    }
                }
                for (int i = firstDuplicateIndex; counter != 0 && i < file.Value.Count; i++)
                {
                    flag = true;
                    if (usedClasters.Contains(file.Value[i]))
                    {
                        for (int j = file.Value[i] + 1; j < clusters.Length; j++)
                        {
                            if (clusters[j] == (int)Type.EMPTY)
                            {
                                counter--;
                                file.Value[i] = j;
                                clusters[j] = 0;
                                break;
                            }
                        }
                    }
                }
                
                if (flag)
                {
                    for (int i = firstDuplicateIndex - 1; i < file.Value.Count - 1; i++)
                    {
                        clusters[file.Value[i]] = file.Value[i + 1];
                    }
                    clusters[file.Value[file.Value.Count - 1]] = (int)Type.EOF;
                }
            }
            isFixed = true;
        }
    }
}