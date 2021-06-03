using System;
using ExoActive;
using NUnit.Framework;
using Object = ExoActive.Object;

namespace Tests
{
    public class RequirementTest
    {
        public bool GTE(int value, int threshold) => value >= threshold;
        public bool LT(int value, int threshold) => value < threshold;
        
        [Flags]
        public enum ObjChar : ulong
        {
            Happy = 1UL << 0,
            Sad = 1UL << 1
        }

        public enum ObjAttr
        {
            Strength
        }

        public class TestObj : Object
        {
            public enum States
            {
                Cup
            }
            public TestObj()
            {
                characteristics.Add(ObjChar.Happy);
                attributes.Add(ObjAttr.Strength, 10);
                states.Add(States.Cup, new Cup());
            }
        }
        
        [Test]
        public void CharacteristicReqs()
        {
            var obj = new TestObj();
            
            var happyCharReq = new CharacteristicRequirement(ObjChar.Happy);
            var notHappyCharReq = new CharacteristicRequirement(ObjChar.Happy, false);
            var sadCharReq = new CharacteristicRequirement(ObjChar.Sad);
            var notSadCharReq = new CharacteristicRequirement(ObjChar.Sad, false);
            
            var objHappyReq = new Requirement(obj, happyCharReq);
            var objNotHappyReq = new Requirement(obj, notHappyCharReq);
            var objSadReq = new Requirement(obj, sadCharReq);
            var objNotSadReq = new Requirement(obj, notSadCharReq);
            
            Assert.True(objHappyReq);
            Assert.True(happyCharReq.Passes(obj));
            
            Assert.False(objNotHappyReq);
            Assert.False(notHappyCharReq.Passes(obj));

            Assert.False(objSadReq);
            Assert.False(sadCharReq.Passes(obj));

            Assert.True(objNotSadReq);
            Assert.True(notSadCharReq.Passes(obj));
            
            obj.Characteristics.Add(ObjChar.Sad);
            
            Assert.True(objHappyReq);
            Assert.True(happyCharReq.Passes(obj));
            
            Assert.False(objNotHappyReq);
            Assert.False(notHappyCharReq.Passes(obj));
            
            Assert.AreEqual(true, (bool)objSadReq);
            Assert.True(sadCharReq.Passes(obj));

            Assert.AreEqual(false, (bool)objNotSadReq);
            Assert.False(notSadCharReq.Passes(obj));
            
            Assert.True(objHappyReq && objSadReq);
        }

        [Test]
        public void AttributesReqs()
        {
            var obj = new TestObj();

            var lowStrength = new AttributeRequirement(ObjAttr.Strength, 10, LT);
            var goodStrength = new AttributeRequirement(ObjAttr.Strength, 10, GTE);
            var objLowStrength = new Requirement(obj, lowStrength);
            var objGoodStrength = new Requirement(obj, goodStrength);
            
            Assert.False(lowStrength.Passes(obj));
            Assert.False(objLowStrength);
            Assert.True(goodStrength.Passes(obj));
            Assert.True(objGoodStrength);

            var attrMods = new Attributes();
            attrMods.Add(ObjAttr.Strength, -5);

            obj.Attributes.Apply(attrMods);
            
            Assert.False(goodStrength.Passes(obj));
            Assert.AreEqual(false,(bool)objGoodStrength);
            Assert.True(lowStrength.Passes(obj));
            Assert.AreEqual(true, (bool)objLowStrength);
        }

        [Test]
        public void StateReqs()
        {
            var obj = new TestObj();

            var canFill = new StateRequirement(Cup.Trigger.Fill);
            var objCanFill = new Requirement(obj, canFill);
            var canDrink = new StateRequirement(Cup.Trigger.Drink);
            var objCanDrink = new Requirement(obj, canDrink);

            Assert.True(canFill.Passes(obj));
            Assert.True(objCanFill);
            Assert.False(canDrink.Passes(obj));
            Assert.False(objCanDrink);
            
            obj.State(TestObj.States.Cup).Fire(Cup.Trigger.Fill);
            
            Assert.True(canFill.Passes(obj));
            Assert.True(objCanFill);
            Assert.True(canDrink.Passes(obj));
            Assert.AreEqual(true, (bool)objCanDrink);
            
            obj.State(TestObj.States.Cup).Fire(Cup.Trigger.Fill);
            
            Assert.False(canFill.Passes(obj));
            Assert.AreEqual(false,(bool)objCanFill);
            Assert.True(canDrink.Passes(obj));
            Assert.True(objCanDrink);
            
        }
    }
}