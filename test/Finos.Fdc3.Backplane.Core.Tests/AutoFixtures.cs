/**
	* SPDX-License-Identifier: Apache-2.0
	* Copyright 2021 FINOS FDC3 contributors - see NOTICE file
	*/

using AutoFixture;
using AutoFixture.AutoNSubstitute;


namespace Finos.Fdc3.Backplane.Core.Tests
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
