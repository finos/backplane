## Backplane 

A desktop service which act as a message bus between multiple desktop agents for sending/receiving FDC3 compliant data.
It serves two purposes:

- Enables other desktop agents running in containers like openfin, finsemble to exchange message using bridge.
- Enables applications running outside container to exchange FDC3 compliant message using client libraries.

There are client libraries to this runtime which expose API enabling interoperability across consuming applications.
Currently available client libs:

- C#: Fdc3.Backplane.Client [details](../Finos.Fdc3.Backplane.Client/Readme.md)
- JS: @finos/fdc3-backplane-client [details](../Finos.Fdc3.Backplane.Client.JS/README.md)

## Supported Platforms

Written in .NET 6.0 C# which supports multiple operating systems. See: https://github.com/dotnet/core/blob/main/release-notes/6.0/supported-os.md.

## Installation

- Download the artifacts zipped as part of release: Finos-Fdc3-Backplane*.zip.
- Unzip the downloaded zip folder.
- Run 'Finos.Fdc3.Backplane.exe'.

## Multihost support

Workflows can be spanned to multiple host machines by running instances of backplane on each machine and configuring the member nodes in config file. 

Look at MultiHostConfig section in [appsettings](./appsettings.json).


## License

Copyright (C) 2022 Backplane open source project

Distributed under the [Apache License, Version 2.0](http://www.apache.org/licenses/LICENSE-2.0).

SPDX-License-Identifier: [Apache-2.0](https://spdx.org/licenses/Apache-2.0)
