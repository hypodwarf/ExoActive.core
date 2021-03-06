using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ExoActive;
using JsonNetConverters;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
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

        [SetUp]
        public void Setup()
        {
            // Clear out the Manager before each test
            Manager.Entities = new ReadOnlyDictionary<Guid, Entity>(new Dictionary<Guid, Entity>());
        }

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
            cup.Fire(Cup.Trigger.Fill);
            var jsonString = Serialize(cup);

            Console.WriteLine(jsonString);

            var dCup = Deserialize<Cup>(jsonString);

            Assert.That(dCup, Is.EqualTo(cup).Using(EntityStateMachine.DefaultComparer));
        }
        
        [Test]
        public void StateTickEvent()
        {
            var cup = new DynamicCup();
            
            cup.Fire(DynamicCup.FillSome, 25);
            
            var jsonString = Serialize(cup);
            
            Console.WriteLine(jsonString);
            
            var dCup = Deserialize<DynamicCup>(jsonString);

            Assert.That(dCup, Is.EqualTo(cup).Using(EntityStateMachine.DefaultComparer));
            Assert.AreEqual(cup.Amount, dCup.Amount);
            
            TimeTicker.AddTicks(1);
            Assert.AreEqual(24, cup.Amount);
            
            Assert.AreEqual(cup.Amount, dCup.Amount);
        }

        [Test]
        public void EntitySerialization()
        {
            var entity = new TestEntity();
            var data = new CapabilityProcessData(new List<IEntity>() {entity}, null);

            Capability.PerformAction<TestCapabilityFill>(data);
            var jsonString = Serialize(entity);

            Console.WriteLine(jsonString);

            var dEntity = Deserialize<TestEntity>(jsonString);

            Assert.That(dEntity, Is.EqualTo(entity).Using(Entity.DefaultComparer));
        }

        [Test]
        public void ManagerSerialization()
        {
            var _ = Manager.New<TestEntity>();
            var jsonString = Serialize(Manager.Entities);
            
            Console.WriteLine(jsonString);

            Dictionary<Guid, Entity> dict = Deserialize<Dictionary<Guid, Entity>>(jsonString);

            Assert.True(Manager.Entities.Keys.SequenceEqual(dict.Keys));
            Assert.True(Manager.Entities.Values.SequenceEqual(dict.Values, Entity.DefaultComparer));
        }

        [Test]
        public void EntitySetSerialization()
        {
            var attr = new Attributes();
            attr.Add(AttributeGroupTests.types.T0, 1, "T-Zero");
            
            var es = new EntitySet {{Guid.NewGuid(), attr}};

            var jsonString = Serialize(es);
            
            Console.WriteLine(jsonString);
            var dEs = Deserialize<EntitySet>(jsonString);
            Assert.That(es.SequenceEqual(dEs));
        }
    }
}