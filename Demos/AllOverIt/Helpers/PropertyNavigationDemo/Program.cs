using AllOverIt.Extensions;
using AllOverIt.Helpers.PropertyNavigation;
using AllOverIt.Helpers.PropertyNavigation.Extensions;
using AllOverIt.Reflection;
using PropertyNavigationDemo.Models;
using System;
using System.Collections;
using System.Linq;

namespace PropertyNavigationDemo
{
    class Program
    {
        static void Main()
        {
            ProcessNodes(PropertyNavigator.For<Root>()
                .Navigate(c => c.Level1)
                .Navigate(c => c.Level2)
                .Navigate(c => c.Level3)
                .Navigate(c => c.Level4a)
                .Navigate(c => c.Values));

            Console.WriteLine();

            ProcessNodes(PropertyNavigator.For<Root>()
                .Navigate(c => c.Level1.Level2.Level3)
                .Navigate(c => c.Level4b)
                .Navigate(c => c.Value));

            Console.WriteLine();

            ProcessNodes(PropertyNavigator.For<Root>()
                .Navigate(c => c.Level1.Level2.Level3)
                .Navigate(c => c.Level4c)
                .Navigate(c => c.Values));

            Console.WriteLine();

            ProcessNodes(PropertyNavigator.For<Root>()
                .Navigate(c => c.Level1.Level2.Level3)
                .Navigate(c => c.Level4d)
                .Navigate(c => c.Value));

            Console.WriteLine();

            ProcessNodes(PropertyNavigator.For<Root>()
                .Navigate(c => c.Level1.Level2.Level3)
                .Navigate(c => c.Level4e)
                .Navigate(c => c.Values));

            Console.WriteLine();
            Console.WriteLine("All Over It.");
            Console.ReadKey();
        }

        private static void ProcessNodes<TType>(IPropertyNodes<TType> typeNodes)
        {
            Console.WriteLine($"Property Path is {typeNodes.GetFullNodePath()}");

            var member = typeNodes.Nodes.Last().Expression;

            var leafNodeType = member.Member.GetMemberType();

            Type elementType = null;

            if (leafNodeType.IsArray)
            {
                elementType = leafNodeType.GetElementType();
                Console.WriteLine($"with a leaf node of type {elementType.GetFriendlyName()}[].");
            }

            if (typeof(IEnumerable).IsAssignableFrom(leafNodeType))
            {
                if (leafNodeType.IsGenericType)
                {
                    elementType = leafNodeType.GetGenericArguments()[0];
                    Console.WriteLine($"with a leaf node of type IEnumerable<{elementType.GetFriendlyName()}>.");
                }
            }

            if (elementType == null)
            {
                // not an array or IEnumerable<T>
                Console.WriteLine($"with a leaf node of type {leafNodeType.GetFriendlyName()}.");
            }

            var typeToCheck = elementType ?? leafNodeType;

            if (typeToCheck.IsClassType() && typeToCheck != CommonTypes.StringType)
            {
                Console.WriteLine("The leaf node/element is a class type");
            }
            else
            {
                Console.WriteLine("The leaf node/element is not a class type");
            }
        }
    }
}