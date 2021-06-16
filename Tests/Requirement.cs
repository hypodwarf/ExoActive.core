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
        public void CharacteristicReqs()
        {
            var obj = new TestObj();

            var happyCharReq = CharacteristicRequirement.Create(TestObj.ObjChar.Happy);
            var notHappyCharReq = CharacteristicRequirement.Create(TestObj.ObjChar.Happy, false);
            var sadCharReq = CharacteristicRequirement.Create(TestObj.ObjChar.Sad);
            var notSadCharReq = CharacteristicRequirement.Create(TestObj.ObjChar.Sad, false);
            Assert.True(happyCharReq(obj));
            Assert.False(notHappyCharReq(obj));
            Assert.False(sadCharReq(obj));
            Assert.True(notSadCharReq(obj));

            obj.Characteristics.Add(TestObj.ObjChar.Sad);
            Assert.True(happyCharReq(obj));
            Assert.False(notHappyCharReq(obj));
            Assert.True(sadCharReq(obj));
            Assert.False(notSadCharReq(obj));
        }

        [Test]
        public void AttributesReqs()
        {
            var obj = new TestObj();

            var lowStrength = AttributeRequirement.Create(TestObj.ObjAttr.Strength, 10, LT);
            var goodStrength = AttributeRequirement.Create(TestObj.ObjAttr.Strength, 10, GTE);

            Assert.False(lowStrength(obj));
            Assert.True(goodStrength(obj));

            var attrMods = new Attributes();
            attrMods.Add(TestObj.ObjAttr.Strength, -5);

            obj.Attributes.Apply(attrMods);

            Assert.False(goodStrength(obj));
            Assert.True(lowStrength(obj));
        }

        [Test]
        public void StateReqs()
        {
            var obj = new TestObj();
            obj.AddState(new Cup());

            Assert.True(CanFill(obj));
            Assert.False(CanDrink(obj));

            obj.GetState<Cup>().Fire(Cup.Trigger.Fill);

            Assert.True(CanFill(obj));
            Assert.True(CanDrink(obj));

            obj.GetState<Cup>().Fire(Cup.Trigger.Fill);

            Assert.False(CanFill(obj));
            Assert.True(CanDrink(obj));
        }
    }
}