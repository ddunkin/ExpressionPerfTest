using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ExpressionPerfTest.Core
{
	public static class PerfTest
	{
		public const int TestRounds = 2000;

		public static readonly ReadOnlyCollection<Tuple<LambdaExpression, string>> TestExpressions = CreateTestExpressions();

		public static void TestReflection()
		{
			Test(ConvertMemberAccessToConstantReflection);
		}

		public static void TestCompile()
		{
			Test(ConvertMemberAccessToConstantCompile);
		}

		private static void Test(Func<Expression, Expression> convert)
		{
			for (int i = 0; i < TestRounds; i++)
			{
				foreach (var pair in TestExpressions)
				{
					var me = (MethodCallExpression) pair.Item1.Body;
					var argument = me.Arguments[0];
					var result = convert(argument);
					if (result.ToString() != pair.Item2)
						throw new InvalidOperationException();
				}
			}
		}

		private static ReadOnlyCollection<Tuple<LambdaExpression, string>> CreateTestExpressions()
		{
			int index = 0;
			var obj = new { Index = 0 };
			string key = "0";
			return new ReadOnlyCollection<Tuple<LambdaExpression, string>>(new[]
			{
				Tuple.Create(CreateExpression<List<object>>(source => source[Index]), "0"),
				Tuple.Create(CreateExpression<List<object>>(source => source[obj.Index]), "0"),
				Tuple.Create(CreateExpression<List<object>>(source => source[index]), "0"),
				Tuple.Create(CreateExpression<List<object>>(source => source[m_index]), "0"),
				Tuple.Create(CreateExpression<Dictionary<string, object>>(source => source[key]), "\"0\""),
			});
		}

		private static int m_index = 0;

		private static int Index { get { return 0; } }

		private static LambdaExpression CreateExpression<T>(Expression<Func<T, object>> expr)
		{
			return expr;
		}

		private static Expression ConvertMemberAccessToConstantReflection(Expression argument)
		{
			var memberExpr = argument as MemberExpression;
			if (memberExpr == null)
				return argument;

			try
			{
				var constExpr = ConvertMemberAccessToConstantReflection(memberExpr.Expression) as ConstantExpression;
				object value = constExpr != null ? constExpr.Value : null;
				
				var property = memberExpr.Member as PropertyInfo;
				if (property != null)
				{
					var constant = property.GetValue(value);
					return Expression.Constant(constant);
				}

				var field = memberExpr.Member as FieldInfo;
				if (field != null)
				{
					var constant = field.GetValue(value);
					return Expression.Constant(constant);
				}
			}
			catch
			{
			}

			return argument;
		}

		private static Expression ConvertMemberAccessToConstantCompile(Expression argument)
		{
			if (argument.NodeType != ExpressionType.MemberAccess)
				return argument;

			try
			{
				var boxed = Expression.Convert(argument, typeof(object));
				var constant = Expression.Lambda<Func<object>>(boxed)
					                    .Compile()
					                    ();
				var constExpr = Expression.Constant(constant);

				return constExpr;
			}
			catch
			{
			}

			return argument;
		}

	}
}
