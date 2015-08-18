using System;
using System.Collections.Generic;

namespace ExpressionPerfTest.Core
{
	public class Child
	{
		public string Value { get; set; }
	}

	public class Parent
	{
		public Child MyChild { get; set; }
	}

	public class GrandParent
	{
		public Parent MyChild { get; set; }
	}

	public class CollectionClass
	{
		public List<Child> MyList { get; set; }

		public Dictionary<string, Child> MyLookup { get; set; }

		public GrandParent GrandParent { get; set; }
	}

	public class TestDataContext
	{
		public CollectionClass MyCollection { get; set; }
	}
}
