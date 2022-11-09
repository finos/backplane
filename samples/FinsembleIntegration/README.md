# Finsemble Backplane Bridge Service (Code Snippet)

Sample code snippet showcase setup of finsemble connection to backplane for sending receiving message

## Installation

1. Copy the folders under 'src\' to 'src\services\finsembleBackplaneBridge' in your finsemble project.
2. Go to config.json file in your finsemble project, and include the path to 'src\finsembleBackplaneBridge\config.json' in the "importConfig" key. For example:

```JSON
   "importConfig": [
	   ..,
	   "$applicationRoot/services/finsembleBackplaneBridge/config.json"
   ]
```

3. Install npm package '@finos/fdc3-backplane-client' from outputfolder at root. [details](../../docs/backplane-client-js.md)

- npm package can be found under package folder in root. Install this package.

4. Run build and start your finsemble project.

## License

Copyright (C) 2022 Backplane open source project

Distributed under the [Apache License, Version 2.0](http://www.apache.org/licenses/LICENSE-2.0).

SPDX-License-Identifier: [Apache-2.0](https://spdx.org/licenses/Apache-2.0)
