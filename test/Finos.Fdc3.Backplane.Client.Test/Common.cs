/**
	* SPDX-License-Identifier: Apache-2.0
	* Copyright 2021 FINOS FDC3 contributors - see NOTICE file
	*/

using AutoFixture;
using AutoFixture.AutoNSubstitute;


namespace Finos.Fdc3.Backplane.Client.Test
{
    public static class AutoFixture
    {
        public static Fixture Create()
        {
            Fixture fixture = new Fixture();
            fixture.Customize(new AutoNSubstituteCustomization());
            return fixture;
        }

    }
}
