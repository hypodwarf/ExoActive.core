using System.Collections.Generic;
using ExoActive;
using NUnit.Framework;

namespace Tests
{
    public class RequirementTest
    {
        public static readonly IRequirement.Check CanFill = StateRequirement<Cup>.Create(Cup.Trigger.Fill, DataSelect.Actors);
        public static readonly IRequirement.Check CanDrink = StateRequirement<Cup>.Create(Cup.Trigger.Drink, DataSelect.Actors);

        public bool GTE(long value, long threshold)
        {
            return value >= threshold;
        }

        public bool LT(long value, long threshold)
        {
            return value < threshold;
        }

        [Test]
        public void TraitReqs()
        {
            var entity = new TestEntity();
            var data = new CapabilityProcessData(new List<IEntity>{entity}, new List<IEntity>());

            var happyCharReq = TraitRequirement.Create(TestEntity.EntityTraits.Happy, DataSelect.Actors);
            var notHappyCharReq = TraitRequirement.Create(TestEntity.EntityTraits.Happy, DataSelect.Actors, false);
            var sadCharReq = TraitRequirement.Create(TestEntity.EntityTraits.Sad, DataSelect.Actors);
            var notSadCharReq = TraitRequirement.Create(TestEntity.EntityTraits.Sad, DataSelect.Actors, false);
            Assert.True(happyCharReq(data));
            Assert.False(notHappyCharReq(data));
            Assert.False(sadCharReq(data));
            Assert.True(notSadCharReq(data));

            entity.Traits.Add(TestEntity.EntityTraits.Sad);
            Assert.True(happyCharReq(data));
            Assert.False(notHappyCharReq(data));
            Assert.True(sadCharReq(data));
            Assert.False(notSadCharReq(data));
        }

        [Test]
        public void AttributesReqs()
        {
            var entity = new TestEntity();
            var data = new CapabilityProcessData(new List<IEntity>{entity}, new List<IEntity>());

            var lowStrength = AttributeRequirement.Create(TestEntity.EntityAttributes.Strength, DataSelect.Actors, 10, LT);
            var goodStrength = AttributeRequirement.Create(TestEntity.EntityAttributes.Strength, DataSelect.Actors, 10, GTE);

            Assert.False(lowStrength(data));
            Assert.True(goodStrength(data));

            var attrMods = new Attributes();
            attrMods.Add(TestEntity.EntityAttributes.Strength, -5);

            entity.Attributes.Apply(attrMods);

            Assert.False(goodStrength(data));
            Assert.True(lowStrength(data));
        }

        [Test]
        public void StateReqs()
        {
            var entity = new TestEntity();
            var data = new CapabilityProcessData(new List<IEntity>{entity}, new List<IEntity>());
            entity.AddState(new Cup());

            Assert.True(CanFill(data));
            Assert.False(CanDrink(data));

            entity.GetState<Cup>().Fire(Cup.Trigger.Fill);

            Assert.True(CanFill(data));
            Assert.True(CanDrink(data));

            entity.GetState<Cup>().Fire(Cup.Trigger.Fill);

            Assert.False(CanFill(data));
            Assert.True(CanDrink(data));
        }
    }
}