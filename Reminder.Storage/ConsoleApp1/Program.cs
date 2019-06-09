using System;

namespace ConsoleApp1
{
	public enum SomeThings
	{
		ThingOne,
		ThingTwo,
		ThingThree
	}

	class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine(SomeThings.ThingThree.ToString());
		}
	}
}
