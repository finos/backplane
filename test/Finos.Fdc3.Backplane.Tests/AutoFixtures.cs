using AutoFixture;
using AutoFixture.AutoNSubstitute;


namespace Finos.Fdc3.Backplane.Tests
{
    public static class AutoFixtures
    {
        public static Fixture Create()
        {
            Fixture fixture = new Fixture();
            fixture.Customize(new AutoNSubstituteCustomization() { ConfigureMembers = true });
            return fixture;
        }

    }
}
