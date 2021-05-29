using System;
using ExoActive;
using Microsoft.VisualStudio.TestPlatform.Common.ExtensionFramework.Utilities;
using NUnit.Framework;
using Object = ExoActive.Object;

namespace Tests
{
    public class ObjectTest
    {
        [Test]
        public void test2()
        {
            var s = new SC();
            s.Fire(Trigger.Fill);
            s.Fire(Trigger.Fill);
            s.Fire(Trigger.Drink);
            s.Fire(Trigger.Drink);
        }
        
        [Test]
        public void testing()
        {
            var o = new Object();

            Console.WriteLine(o.GetState());
            // foreach (var t in o.GetTriggers())
            // {
            //     Console.WriteLine(t); 
            // }
            
            
        }
    }
}