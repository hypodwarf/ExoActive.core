using System;
using System.Collections.Generic;
using ExoActive;
using JsonNetConverters;
using Newtonsoft.Json;
using NUnit.Framework;
using Object = ExoActive.Object;

namespace Tests
{
    public class Serialization
    {
        private readonly JsonSerializerSettings  jsonSerializeSettings = new JsonSerializerSettings
        {
            Converters = new List<JsonConverter>()
            {
                new KeyValuePairEnumConverter(), 
                new ListDictionaryConverter()
            },
            TypeNameHandling = TypeNameHandling.Auto
        };
        
        public T Deserialize<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json, jsonSerializeSettings);
        }

        public string Serialize<T>(T obj)
        {
            return JsonConvert.SerializeObject(obj, Formatting.Indented, jsonSerializeSettings);
        }
        
        [Test]
        public void AttributesSerialization()
        {
            var g1 = new Attributes();
            var g2 = new Attributes();
            
            g1.Add(AttributeGroupTests.types.T0);
            g1.Add(AttributeGroupTests.types.T1, 1);
            g1.Add(AttributeGroupTests.types.T2, 2, "T-Two");
            
            g2.Add(AttributeGroupTests.types.T0, 10);
            g2.Add(AttributeGroupTests.types.T1, 11);
            g2.Add(AttributeGroupTests.types.T2, 12, "Tootsie");
            
            g1.Apply(g2);
            
            string jsonString = Serialize(g1);
            // Console.WriteLine(jsonString);
            var dg1 = Deserialize<Attributes>(jsonString);
            
            Assert.True(Attributes.DefaultComparer.Equals(g1,dg1));
            Assert.That(dg1, Is.EqualTo(g1).Using(Attributes.DefaultComparer));
        }
        
        
        [Test]
        public void CharacteristicsSerialization()
        {
            var c = new Characteristics();
            c.Add(TestFlag.Min);

            string jsonString = Serialize(c);

            Console.WriteLine(jsonString);

            var dc = Deserialize<Characteristics>(jsonString);
            
            Assert.That(dc, Is.EqualTo(c).Using(Characteristics.DefaultComparer));
        }
        
        [Test]
        public void StateSerialization()
        {
            var cup = new Cup();
            cup.Fire(Cup.Trigger.Fill);
            string jsonString = Serialize(cup);

            Console.WriteLine(jsonString);

            var dCup = Deserialize<Cup>(jsonString);
            
            Assert.That(dCup, Is.EqualTo(cup).Using(State.DefaultComparer));
        }

        [Test]
        public void ObjectSerialization()
        {
            var obj = new TestObj();
            string jsonString = Serialize(obj);
            
            // Console.WriteLine(jsonString);

            // var dObj = Deserialize<TestObj>(jsonString);
            
            // Assert.That(dObj, Is.EqualTo(obj).Using(Object.DefaultComparer));

            var fill = new TestCapabilityFill();
            var drink = new TestCapabilityDrink();

            fill.PerformAction(new List<Object>(){obj});
            jsonString = Serialize(obj);
            
            Console.WriteLine(jsonString);

            var dObj = Deserialize<TestObj>(jsonString);
            
            Assert.That(dObj, Is.EqualTo(obj).Using(Object.DefaultComparer));
        }

        [Test, Ignore("This test shows how interfaces don't work correctly")]
        public void DictState()
        {
            IDictionary<string, State> states = new Dictionary<string, State>();
            string jsonString = Serialize(states);
            
            Console.WriteLine(jsonString);

            var dObj = Deserialize<IDictionary<string, State>>(jsonString);
        }
    }
}