# Helmbet-RPS


- using [websocket-sharp](https://github.com/sta/websocket-sharp);

- using [SimpleBase](https://github.com/ssg/SimpleBase.git);

- using [Newtonsoft.Json](https://github.com/JamesNK/Newtonsoft.Json.git);

This is a BETA release of the HelmBet RPS application, powered by [Peerplays](https://peerplays.com) blockchain. 

Note: there is know issue with wss connection to the testnet, please use ws as of now, Issue will be fixed in next version.

Testnet details
Chain id: be6b79295e728406cbb7494bcb626e62ad278fa4018699cf8f75739f4c1a81fd

Node: wss://node.ppytest.com

- Register Account: http://paperwallet.ppytest.com 

- Please use 22 symbols as a password when registering a new account.


### Getting Started

- There is a NetworkManager gameObject available on the scene, that also includes Connection Managing that is monitoring socket-connections with the node. 

- Api Manager is a component required for access to the Peerplays blockchain API. 

- NodeManager required for adding, storing and selection of the node to be used for the application. It also contains default node connection. Last selected address is kept in device memory and is used by default next time application started. 
To restore this parameter in EditorMode: set ReserAtStart chekbox and press Play; in RunMode: call ConnectTo( host,  resultCallback ) or set property SelecteHost by new host. 

### Detect Chain ID

In the ChainConfig class there is a list of supported chainIDs. If you need to add new chain id or remove existing - you can edit this configuration.



###The MIT License###

Copyright (c) 2017 Pacatum, Inc., and contributors.

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the 'Software'), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED 'AS IS', WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
