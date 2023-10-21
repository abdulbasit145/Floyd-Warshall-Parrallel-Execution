using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Assignment
{
	/// <summary>
	/// This program find All Pairs Shortest Paths using Floyd Warshall Algorithm in an n-vertex graph using the following techniques.
	/// Data Parallelism (using Parallel.For)
	/// Data Parallelism (using Parallel.ForEach)
	/// Task Parallelism (using Implicit Task creation)
	/// Task Parallelism (using Explicit Task creation)
	/// </summary>
	internal class Program
	{
		static Random random;
		static readonly Stopwatch stopwatch = new Stopwatch();
		const int INF = int.MaxValue;
		static readonly int vertices = 5;

		/// <summary>
		///  Main entry point of the program.
		/// </summary>
		/// <param name="args">Command line arguments</param>
		static void Main(string[] args)
		{
			int[,] graph = new int[vertices, vertices];
			GenerateRandomGraph(graph);

			/*int[,] graph = {
				{0, 3, INF, 5},
				{2, 0, INF, 4},
				{INF, 1, 0, INF},
				{INF, INF, 2, 0}
			};*/

			/*Console.WriteLine("----- Initial Graph -----");
			PrintGraph(graph);*/

			/*Console.WriteLine("\n----- Using Serialized Program -----");*/
			stopwatch.Start();
			APSP_UsingSerializedProgram(graph);
			stopwatch.Stop();

			TimeSpan elapsed_time1 = stopwatch.Elapsed;
			Console.WriteLine($"Time taken by Serial Application is { elapsed_time1.TotalMilliseconds} milliseconds");
			stopwatch.Reset();


			/*Console.WriteLine("\n----- Data Parallelism (using Parallel.For) -----");*/
			stopwatch.Start();
			APSP_UsingParrallelFor(graph);
			stopwatch.Stop();

			TimeSpan elapsed_time2 = stopwatch.Elapsed;
			Console.WriteLine($"Time taken by Data Parallelism (using Parallel.For) Application is {elapsed_time2.TotalMilliseconds} milliseconds");
			stopwatch.Reset();

			/*Console.WriteLine("\n----- Data Parallelism (using Parallel.ForEach) -----");*/
			stopwatch.Start();
			APSP_UsingParrallelForEach(graph);
			stopwatch.Stop();

			TimeSpan elapsed_time3 = stopwatch.Elapsed;
			Console.WriteLine($"Time taken by Data Parallelism (using Parallel.ForEach) Application is {elapsed_time3.TotalMilliseconds} milliseconds");
			stopwatch.Reset();

			/*Console.WriteLine("\n----- Task Parallelism (using Implicit Task creation) -----");*/
			stopwatch.Start();
			APSP_UsingImplictTaskCreation(graph);
			stopwatch.Stop();

			TimeSpan elapsed_time4 = stopwatch.Elapsed;
			Console.WriteLine($"Time taken by Task Parallelism (using Implicit Task creation) Application is {elapsed_time4.TotalMilliseconds} milliseconds");
			stopwatch.Reset();

			/*Console.WriteLine("\n----- Task Parallelism (using Explicit Task creation) -----");*/
			stopwatch.Start();
			APSP_UsingExplicitTaskCreation(graph);
			stopwatch.Stop();

			TimeSpan elapsed_time5 = stopwatch.Elapsed;
			Console.WriteLine($"Time taken by Task Parallelism (using Explicit Task creation) Application is {elapsed_time5.TotalMilliseconds} milliseconds");
			stopwatch.Reset();

			Console.ReadKey();
		}

		/// <summary>
		/// Method <c>GenerateRandomGraph</c> create a 2-D graph filled with random Integers.
		/// </summary>
		/// <param name="graph">It is a 2-D Array representing graph.</param>
		static void GenerateRandomGraph(int[,] graph)
		{
			random = new Random();
			for (int i = 0; i < vertices; i++)
			{
				for (int j = 0; j < vertices; j++)
				{
					// Generate a graph with random number between 1 and 20 with 10% probability to add INF in between.
					int random_value = random.Next(1, 21);
					if (random_value <= 2)
						graph[i, j] = INF;
					else
						graph[i, j] = random_value;
				}
			}
		}

		/// <summary>
		/// Method <c>PrintGraph</c> print the 2-D graph on console.
		/// </summary>
		/// <param name="graph">It is a 2-D Array representing graph.</param>
		static void PrintGraph(int[,] graph)
		{
			for (int i = 0; i < vertices; i++)
			{
				for (int j = 0; j < vertices; j++)
				{
					if (graph[i, j] == INF)
						Console.Write("INF".PadRight(10));
					else
						Console.Write(graph[i, j].ToString().PadRight(10));
				}
				Console.WriteLine();
			}
			Console.WriteLine();
		}

		/// <summary>
		/// Method <c>APSP_UsingSerializedProgram</c> calculate the all pair shortest path using Flyod Warshall Algorithm
		/// </summary>
		/// <param name="graph">It is a 2-D Array representing graph.</param>
		static void APSP_UsingSerializedProgram(int[,] graph)
		{
			int[,] shortest_path_graph = new int[vertices, vertices];
			for (int i = 0; i < vertices; i++)
				for (int j = 0; j < vertices; j++)
					shortest_path_graph[i, j] = graph[i, j];

			for (int k = 0; k < vertices; k++)
				UpdateRow(shortest_path_graph, k);

			/*PrintGraph(shortest_path_graph);*/
		}

		/// <summary>
		/// Method <c>APSP_UsingParrallelFor</c> calculate the all pair shortest path using Data Parrallelism (Parrallel.For).
		/// </summary>
		/// <param name="graph">It is a 2-D Array representing graph.</param>
		static void APSP_UsingParrallelFor(int[,] graph)
		{
			int[,] shortest_path_graph = new int[vertices, vertices];
			for (int i = 0; i < vertices; i++)
				for (int j = 0; j < vertices; j++)
					shortest_path_graph[i, j] = graph[i, j];

			Parallel.For(0, vertices, k =>
			{
				UpdateRow(shortest_path_graph, k);
			});

			/*PrintGraph(shortest_path_graph);*/
		}

		/// <summary>
		/// Method <c>APSP_UsingParrallelForEach</c> calculate the all pair shortest path using Data Parrallelism (Parrallel.ForEach).
		/// </summary>
		/// <param name="graph">It is a 2-D Array representing graph.</param>
		static void APSP_UsingParrallelForEach(int[,] graph)
		{
			int[,] shortest_path_graph = new int[vertices, vertices];
			for (int i = 0; i < vertices; i++)
				for (int j = 0; j < vertices; j++)
					shortest_path_graph[i, j] = graph[i, j];

			Parallel.ForEach(Partitioner.Create(0, vertices), range =>
			{
				for (int k = range.Item1; k < range.Item2; k++)
				{
					UpdateRow(shortest_path_graph, k);
				}
			});

			/*PrintGraph(shortest_path_graph);*/
		}

		/// <summary>
		/// Method <c>APSP_UsingImplictTaskCreation</c> calculate the all pair shortest path using Task Parrallelism (implicit task creation).
		/// </summary>
		/// <param name="graph">It is a 2-D Array representing graph.</param>
		static void APSP_UsingImplictTaskCreation(int[,] graph)
		{
			int[,] shortest_path_graph = new int[vertices, vertices];
			for (int i = 0; i < vertices; i++)
				for (int j = 0; j < vertices; j++)
					shortest_path_graph[i, j] = graph[i, j];

			for (int k = 0; k < vertices; k++)
			{
				Parallel.Invoke(
					() => UpdateRow(shortest_path_graph, k)
				);
			}

			/*PrintGraph(shortest_path_graph);*/
		}

		/// <summary>
		/// Method <c>APSP_UsingExplicitTaskCreation</c> calculate the all pair shortest path using Task Parrallelism (explicit task creation).
		/// </summary>
		/// <param name="graph"></param>
		static void APSP_UsingExplicitTaskCreation(int[,] graph)
		{
			int[,] shortest_path_graph = new int[vertices, vertices];
			for (int i = 0; i < vertices; i++)
				for (int j = 0; j < vertices; j++)
					shortest_path_graph[i, j] = graph[i, j];

			Task[] tasks = new Task[vertices];

			for (int k = 0; k < vertices; k++)
			{
				int index = k;
				tasks[k] = Task.Factory.StartNew(() =>
				{
					UpdateRow(shortest_path_graph, index);
				});
			}

			Task.WaitAll(tasks);

			/*PrintGraph(shortest_path_graph);*/
		}

		/// <summary>
		/// Method <c>UpdateRow</c> calcullate the value for the Floyd shortest distance matrix.
		/// </summary>
		/// <param name="shortest_path_graph">>It is a 2-D Array representing Floyd shortest distance graph.</param>
		/// <param name="k">It is the outer loop of Floyd Algorithm that handle the update o each row value.</param>
		static void UpdateRow(int[,] shortest_path_graph, int k)
		{
			for (int i = 0; i < vertices; i++)
			{
				for (int j = 0; j < vertices; j++)
				{
					if (shortest_path_graph[i, k] < INF && shortest_path_graph[k, j] < INF)
					{
						if (shortest_path_graph[i, k] + shortest_path_graph[k, j] < shortest_path_graph[i, j])
							shortest_path_graph[i, j] = shortest_path_graph[i, k] + shortest_path_graph[k, j];
					}
				}
			}
		}
	}
}

