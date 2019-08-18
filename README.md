# What is One&lt;T&gt;?
One&lt;T&gt; is a generic collection that contains exactly one item. This kind of generic collection allows you to use LINQ expression to write closures and functional programming.

# Installation
Using Package Manager Console
```
Install-Package RG.System.Collections.Generic.One
```
Using dotnet CLI
```
dotnet add package RG.System.Collections.Generic.One
```

# Usage
Include using directive
```csharp
using System.Collections.Generic;
```
Create instance of One&lt;T&gt;
```csharp
// using constructor
var oneInt = new One<int>(1);

// using factory
var oneDecimal = One.Of(100.0m);

// using extension method
var oneString = "Hello World".ToOne();
```
Use it in LINQ expression as a closure
```csharp
decimal total = from price in 199m.ToOne()
                let quantity = 2m
                let subtotal = price * quantity
                let tax = subtotal * 0.1m
                let discount = 10m
                select subtotal + tax - discount;
```

# LINQ Support
When a LINQ expression enumerates from a One&lt;T&gt;, all following features are supported.

Starting a 'closure' LINQ expression
```csharp
from x in e.ToOne()
```
Introducing a new range variable
```csharp
from x in e.ToOne()
let y = 10
```
Introducing a new range variable from another One&lt;T&gt;
```csharp
from x in e.ToOne()
from y in one // one is a One<int>, z is an Int32
```
Returning result
```csharp
int result = from x in e.ToOne()
             let y = x * x
             select y;
```
Multiple awaits
```csharp
int result = from x in (await GetValue1Async()).ToOne()
             join y in (await GetValue2Async()).ToOne() on 1 equals 1
             select x * y;
```

# Auto Disposing Range Variables
All disposable range variables are automatically disposed on select.
```csharp
string ComputeSecretProof(string accessToken) {
    string secretProof = from appSecret in Configuration["AppSecret"].ToOne()
                         let appSecretBytes = Encoding.UTF8.GetBytes(appSecret)
                         let accessTokenBytes = Encoding.UTF8.GetBytes(accessToken)
                         let hmacssha256 = new HMACSHA256(appSecretBytes)
                         let proofBytes = hmacsha256.ComputeHash(accessTokenBytes)
                         select string.Join("", proofBytes.Select(b => b.ToString("x2")));
    // hmacsha256 already disposed at this point
    return secretProof;
}
                     
```

# Disabling Auto Dispose
```csharp
string json = await (Task<string>) from client in httpClient.ToOne(doNotDisposeValueOnDispose: true)
                                   let uri = new Uri(uriString)
                                   let jwtToken = ...
                                   ...
                                   select client.GetStringAsync(uri);
// httpClient is not disposed and can still be reused
```

# Planned Features
1. join .. into .. clause
2. async let

Please submit an issue if you need to report bugs or request a feature.
