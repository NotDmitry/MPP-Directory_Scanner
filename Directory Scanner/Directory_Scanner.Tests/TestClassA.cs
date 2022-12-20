using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Directory_Scanner.Tests;

public class TestClassA
{
    public string Name { get; private set; } 

	public TestClassA(string name)
	{
		Name = name;
	}

	public string WaitAndPrint()
	{
		var rand = new Random();
		int mls = rand.Next(1000, 5000);
		Thread.Sleep(mls);
		return Name;
	}
}
