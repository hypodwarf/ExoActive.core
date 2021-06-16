using System;
using System.Collections.Generic;
using ExoActive;
using JsonNetConverters;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Tests
{
    public class Serialization
    {
        private readonly JsonSerializerSettings jsonSerializeSettings = new()
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

            var jsonString = Serialize(g1);
            // Console.WriteLine(jsonString);
            var dg1 = Deserialize<Attributes>(jsonString);

            Assert.True(Attributes.DefaultComparer.Equals(g1, dg1));
            Assert.That(dg1, Is.EqualTo(g1).Using(Attributes.DefaultComparer));
        }


        [Test]
        public void TraitsSerialization()
        {
            var traits = new Traits();
            traits.Add(TestFlag.Min);

            var jsonString = Serialize(traits);

            Console.WriteLine(jsonString);

            var dTraits = Deserialize<Traits>(jsonString);

            Assert.That(dTraits, Is.EqualTo(traits).Using(Traits.DefaultComparer));
        }

        [Test]
        public void StateSerialization()
        {
            var cup = new Cup();
            cup.Fire(Cup.Trigger.Fill);
            var jsonString = Serialize(cup);

            Console.WriteLine(jsonString);

            var dCup = Deserialize<Cup>(jsonString);

            Assert.That(dCup, Is.EqualTo(cup).Using(State.DefaultComparer));
        }

        [Test]
        public void EntitySerialization()
        {
            var entity = new TestEntity();
            var jsonString = Serialize(entity);

            // Console.WriteLine(jsonString);

            // var dEntity = Deserialize<TestEntity>(jsonString);

            // Assert.That(dEntity, Is.EqualTo(entity).Using(Entity.DefaultComparer));

            var fill = Capability.Get<TestCapabilityFill>();
            var drink = Capability.Get<TestCapabilityDrink>();

            fill.PerformAction(new List<Entity>() {entity});
            jsonString = Serialize(entity);

            Console.WriteLine(jsonString);

            var dEntity = Deserialize<TestEntity>(jsonString);

            Assert.That(dEntity, Is.EqualTo(entity).Using(Entity.DefaultComparer));
        }
    }
}