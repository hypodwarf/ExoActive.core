using System.Linq;
using Example_Effect;
using ExoActive;
using NUnit.Framework;

namespace Tests.Example
{
    public class Effect
    {
        private void HealthCheck(IEntity[] team, int expectedHealth)
        {
            foreach (var actor in team)
            {
                Assert.AreEqual(expectedHealth, actor.Attributes.GetAttributeValue(HealthAttributes.Health));
            }
        }

        [Test]
        public void TestCapabilities()
        {
            
            IEntity[] team1 = {new Actor(), new Actor(), new Actor()};
            IEntity[] team2 = {new Actor(), new Actor(), new Actor()};
            
            HealthCheck(team1, 100);
            HealthCheck(team2, 100);
            
            Capability.PerformAction<Attack>(team1, team2);
            
            HealthCheck(team1, 100);
            HealthCheck(team2, 70);
            
            Capability.PerformAction<Heal>(team2, team2);
            
            HealthCheck(team1, 100);
            HealthCheck(team2, 100);
            
        }
    }
}