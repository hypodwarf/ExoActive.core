using ExoActive;
using NUnit.Framework;

namespace Tests
{
    public class RequirementTest
    {
        public static readonly IRequirement.Check CanFill = StateRequirement<Cup>.Create(Cup.Trigger.Fill);
        public static readonly IRequirement.Check CanDrink = StateRequirement<Cup>.Create(Cup.Trigger.Drink);

        public bool GTE(int value, int threshold)
        {
            return value >= threshold;
        }

        public bool LT(int value, int threshold)
        {
            return value < threshold;
        }

        [Test]
        public void TraitReqs()
        {
            var entity = new TestEntity();

            var happyCharReq = TraitRequirement.Create(TestEntity.EntityTraits.Happy);
            var notHappyCharReq = TraitRequirement.Create(TestEntity.EntityTraits.Happy, false);
            var sadCharReq = TraitRequirement.Create(TestEntity.EntityTraits.Sad);
            var notSadCharReq = TraitRequirement.Create(TestEntity.EntityTraits.Sad, false);
            Assert.True(happyCharReq(entity));
            Assert.False(notHappyCharReq(entity));
            Assert.False(sadCharReq(entity));
            Assert.True(notSadCharReq(entity));

            entity.Traits.Add(TestEntity.EntityTraits.Sad);
            Assert.True(happyCharReq(entity));
            Assert.False(notHappyCharReq(entity));
            Assert.True(sadCharReq(entity));
            Assert.False(notSadCharReq(entity));
        }

        [Test]
        public void AttributesReqs()
        {
            var entity = new TestEntity();

            var lowStrength = AttributeRequirement.Create(TestEntity.EntityAttributes.Strength, 10, LT);
            var goodStrength = AttributeRequirement.Create(TestEntity.EntityAttributes.Strength, 10, GTE);

            Assert.False(lowStrength(entity));
            Assert.True(goodStrength(entity));

            var attrMods = new Attributes();
            attrMods.Add(TestEntity.EntityAttributes.Strength, -5);

            entity.Attributes.Apply(attrMods);

            Assert.False(goodStrength(entity));
            Assert.True(lowStrength(entity));
        }

        [Test]
        public void StateReqs()
        {
            var entity = new TestEntity();
            entity.AddState(new Cup());

            Assert.True(CanFill(entity));
            Assert.False(CanDrink(entity));

            entity.GetState<Cup>().Fire(Cup.Trigger.Fill);

            Assert.True(CanFill(entity));
            Assert.True(CanDrink(entity));

            entity.GetState<Cup>().Fire(Cup.Trigger.Fill);

            Assert.False(CanFill(entity));
            Assert.True(CanDrink(entity));
        }
    }
}