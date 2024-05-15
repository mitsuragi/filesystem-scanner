namespace filesystem_scanner
{
    public class Program
    {
        static void Main(string[] args)
        {
            FAT fat = new FAT();
            bool quit = false;
            int input;

            Console.WriteLine("1 - display clusters\n" +
              "2 - display files\n" +
              "3 - find files clusters\n" +
              "4 - find bad clusters\n" +
              "5 - find lost files\n" +
              "6 - fix duplicates\n" +
              "0 - exit");

            while (!quit)
            {
                Console.Write("Введите пункт меню: ");
                try
                {
                    input = Convert.ToInt32(Console.ReadLine());
                }
                catch (FormatException ex)
                {
                    Console.WriteLine(ex.ToString());
                    continue;
                }

                switch (input)
                {
                    case 0:
                        quit = true;
                        break;
                    case 1:
                        Console.WriteLine("\n==FAT==");
                        fat.displayClusters();
                        break;
                    case 2:
                        fat.displayFiles();
                        break;
                    case 3:
                        Console.WriteLine("Поиск последовательности кластеров для известных файлов");
                        foreach (var file in fat.getFilesClusters())
                        {
                            Console.Write($"{file.Key}: ");
                            foreach (int i in file.Value)
                            {
                                Console.Write($"{i} ");
                            }
                            Console.WriteLine();
                        }
                        break;
                    case 4:
                        Console.WriteLine("Поиск BAD файлов");
                        foreach (int i in fat.getIndexesOfBadFiles())
                        {
                            Console.Write(i + " ");
                        }
                        Console.WriteLine();
                        break;
                    case 5:
                        Console.WriteLine("Поиск потерянных файлов");
                        foreach (var file in fat.getLostFilesClusters())
                        {
                            Console.Write($"{file.Key}: ");
                            foreach (int i in file.Value)
                            {
                                Console.Write($"{i} ");
                            }
                            Console.WriteLine();
                        }
                        break;
                    case 6:
                        Console.WriteLine("Исправление дублирующихся кластеров");
                        fat.fixDuplicateClusters();
                        Console.WriteLine("Дубликаты исправлены");
                        break;
                }
            }
            Console.WriteLine("bye");
        }
    }
}
