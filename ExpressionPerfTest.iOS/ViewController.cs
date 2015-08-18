using System;

using UIKit;
using System.Threading.Tasks;
using ExpressionPerfTest.Core;
using System.Diagnostics;

namespace ExpressionPerfTest.iOS
{
	public partial class ViewController : UIViewController
	{
		public ViewController(IntPtr handle) : base(handle)
		{
		}

		public async override void ViewDidLoad()
		{
			base.ViewDidLoad();
			// Perform any additional setup after loading the view, typically from a nib.

			await Task.Delay(100);

			var sw = new Stopwatch();

			const int runs = 10;
			long totalCompileTime = 0;
			long totalReflectionTime = 0;

			for (int i = 0; i < runs + 1; i++)
			{
				sw.Restart();
				PerfTest.TestCompile();
				sw.Stop();

				if (i != 0)
					totalCompileTime += sw.ElapsedMilliseconds;

				sw.Restart();
				PerfTest.TestReflection();
				sw.Stop();

				if (i != 0)
					totalReflectionTime += sw.ElapsedMilliseconds;

			}

			Console.WriteLine(UIDevice.CurrentDevice.Model);
			var expressionCount = PerfTest.TestRounds * PerfTest.TestExpressions.Count;
			Console.WriteLine("Compile took {0}ms per {1} expressions", totalCompileTime / runs, expressionCount);
			Console.WriteLine("Reflection took {0}ms per {1} expressions", totalReflectionTime / runs, expressionCount);
		}

		public override void DidReceiveMemoryWarning()
		{
			base.DidReceiveMemoryWarning();
			// Release any cached data, images, etc that aren't in use.
		}
	}
}

