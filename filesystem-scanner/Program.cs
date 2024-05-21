namespace filesystem_scanner
{
    public class Program
    {
        static void Main(string[] args)
        {
            FAT fat = new FAT();
            bool quit = false;
            int input;

            while (!quit)
            {
                Console.WriteLine("1 - Показать кластеры\n" +
                                  "2 - Показать файлы\n" +
                                  "3 - Поиск кластеров файлов\n" +
                                  "4 - Поиск BAD кластеров\n" +
                                  "5 - Поиск потерянных файлов\n" +
                                  "6 - Исправление дублирующихся файлов\n" +
                                  "0 - Выход\n");

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
                        Console.WriteLine();
                        break;
                    case 4:
                        Console.WriteLine("Поиск BAD файлов");
                        foreach (int i in fat.getIndexesOfBadFiles())
                        {
                            Console.Write(i + " ");
                        }
                        Console.WriteLine('\n');
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
                        Console.WriteLine();
                        break;
                    case 6:
                        Console.WriteLine("Исправление дублирующихся кластеров");
                        fat.fixDuplicateClusters();
                        Console.WriteLine("Дубликаты исправлены\n");
                        break;
                }
            }
            Console.WriteLine("Пока");
        }
    }
}
