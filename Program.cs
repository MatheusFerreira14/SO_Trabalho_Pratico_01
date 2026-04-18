using System;
using System.Threading;

class Banker
{
    const int NUMBER_OF_CUSTOMERS = 5;
    const int NUMBER_OF_RESOURCES = 3;

    static int[] available = new int[NUMBER_OF_RESOURCES];
    static int[,] maximum = new int[NUMBER_OF_CUSTOMERS, NUMBER_OF_RESOURCES];
    static int[,] allocation = new int[NUMBER_OF_CUSTOMERS, NUMBER_OF_RESOURCES];
    static int[,] need = new int[NUMBER_OF_CUSTOMERS, NUMBER_OF_RESOURCES];

    static object mutex = new object();
    static Random rand = new Random();

    // Imprimir Estado
    static void PrintState()
    {
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine("\n===== ESTADO ATUAL =====");
        Console.ResetColor();

        Console.Write("Available: ");
        for (int i = 0; i < NUMBER_OF_RESOURCES; i++)
            Console.Write(available[i] + " ");

        Console.WriteLine("\n\nAllocation:");
        for (int i = 0; i < NUMBER_OF_CUSTOMERS; i++)
        {
            for (int j = 0; j < NUMBER_OF_RESOURCES; j++)
                Console.Write(allocation[i, j] + " ");
            Console.WriteLine();
        }

        Console.WriteLine("\nNeed:");
        for (int i = 0; i < NUMBER_OF_CUSTOMERS; i++)
        {
            for (int j = 0; j < NUMBER_OF_RESOURCES; j++)
                Console.Write(need[i, j] + " ");
            Console.WriteLine();
        }

        Console.WriteLine("========================");
    }

    // Verifica Estado Seguro
    static bool IsSafe()
    {
        int[] work = new int[NUMBER_OF_RESOURCES];
        bool[] finish = new bool[NUMBER_OF_CUSTOMERS];

        Array.Copy(available, work, NUMBER_OF_RESOURCES);

        int count = 0;

        while (count < NUMBER_OF_CUSTOMERS)
        {
            bool found = false;

            for (int i = 0; i < NUMBER_OF_CUSTOMERS; i++)
            {
                if (!finish[i])
                {
                    int j;
                    for (j = 0; j < NUMBER_OF_RESOURCES; j++)
                    {
                        if (need[i, j] > work[j])
                            break;
                    }

                    if (j == NUMBER_OF_RESOURCES)
                    {
                        for (int k = 0; k < NUMBER_OF_RESOURCES; k++)
                            work[k] += allocation[i, k];

                        finish[i] = true;
                        found = true;
                        count++;
                    }
                }
            }

            if (!found)
                return false;
        }

        return true;
    }

    // Solicitar Recursos
    static int RequestResources(int customer, int[] request)
    {
        lock (mutex)
        {
            Console.Write($"\nCliente {customer} solicitando: ");
            foreach (var r in request) Console.Write(r + " ");

            for (int i = 0; i < NUMBER_OF_RESOURCES; i++)
            {
                if (request[i] > need[customer, i])
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(" -> ERRO (excede necessidade)");
                    Console.ResetColor();
                    return -1;
                }

                if (request[i] > available[i])
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(" -> ERRO (indisponível)");
                    Console.ResetColor();
                    return -1;
                }
            }

            // Simula
            for (int i = 0; i < NUMBER_OF_RESOURCES; i++)
            {
                available[i] -= request[i];
                allocation[customer, i] += request[i];
                need[customer, i] -= request[i];
            }

            if (!IsSafe())
            {
                // Desfaz
                for (int i = 0; i < NUMBER_OF_RESOURCES; i++)
                {
                    available[i] += request[i];
                    allocation[customer, i] -= request[i];
                    need[customer, i] += request[i];
                }

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(" -> NEGADO (estado inseguro)");
                Console.ResetColor();
                return -1;
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(" -> ACEITO");
            Console.ResetColor();
            PrintState();
            return 0;
        }
    }

    // Liberar Recursos
    static int ReleaseResources(int customer, int[] release)
    {
        lock (mutex)
        {
            Console.Write($"\nCliente {customer} liberando: ");
            foreach (var r in release) Console.Write(r + " ");

            for (int i = 0; i < NUMBER_OF_RESOURCES; i++)
            {
                if (release[i] > allocation[customer, i])
                    release[i] = allocation[customer, i];

                available[i] += release[i];
                allocation[customer, i] -= release[i];
                need[customer, i] += release[i];
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(" -> LIBERADO");
            Console.ResetColor();
            PrintState();
            return 0;
        }
    }

    // Thread do Cliente
    static void CustomerThread(object obj)
    {
        int id = (int)obj;

        while (true)
        {
            int[] request = new int[NUMBER_OF_RESOURCES];

            for (int i = 0; i < NUMBER_OF_RESOURCES; i++)
            {
                request[i] = need[id, i] > 0 ? rand.Next(need[id, i] + 1) : 0;
            }

            RequestResources(id, request);

            Thread.Sleep(1000);

            int[] release = new int[NUMBER_OF_RESOURCES];

            for (int i = 0; i < NUMBER_OF_RESOURCES; i++)
            {
                release[i] = allocation[id, i] > 0 ? rand.Next(allocation[id, i] + 1) : 0;
            }

            ReleaseResources(id, release);

            Thread.Sleep(1000);
        }
    }

    static void Main(string[] args)
    {
        Console.Clear();

        if (args.Length != NUMBER_OF_RESOURCES)
        {
            Console.WriteLine("Uso: dotnet run r1 r2 r3");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\nEx.: dotnet run 10 5 7");
            Console.ResetColor();
            return;
        }

        for (int i = 0; i < NUMBER_OF_RESOURCES; i++)
            available[i] = int.Parse(args[i]);

        // Inicializa Maximum
        for (int i = 0; i < NUMBER_OF_CUSTOMERS; i++)
        {
            for (int j = 0; j < NUMBER_OF_RESOURCES; j++)
            {
                maximum[i, j] = rand.Next(available[j] + 1);
                allocation[i, j] = 0;
                need[i, j] = maximum[i, j];
            }
        }

        Thread[] threads = new Thread[NUMBER_OF_CUSTOMERS];

        for (int i = 0; i < NUMBER_OF_CUSTOMERS; i++)
        {
            threads[i] = new Thread(CustomerThread!);
            threads[i].Start(i);
        }

        foreach (var t in threads)
            t.Join();

        Console.ReadKey();
    }
}