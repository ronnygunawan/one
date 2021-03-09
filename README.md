# Release Notes
### Version 1.0.3
Added `.AsOne()` extension method

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
var oneDecimal = One.Value(100.0m);
```
Use it in LINQ expression as a closure
```csharp
decimal total = from price in 199m.AsOne()
                let quantity = 2m
                let subtotal = price * quantity
                let tax = subtotal * 0.1m
                let discount = 10m
                select subtotal + tax - discount;
```

# Qlosure&lt;T&gt;: Closures Written in LINQ
You can write closures in LINQ by enumerating from a One&lt;T&gt; or a Qlosure&lt;T&gt;.

Start a qlosure
```csharp
from x in e.AsOne().ToQlosure() // ToQlosure is optional
```

Introduce a new range variable
```csharp
from x in e.AsOne()
let y = 10
```

Introduce a new range variable from another One&lt;T&gt;
```csharp
from x in e.AsOne()
from y in one // one is a One<int>, y is an Int32
```

Implicitly cast qlosure to its final value
```csharp
int result = from x in e.AsOne()
             let y = x * x
             select y;
```

# Auto Disposing Range Variables
All disposable range variables are automatically disposed before stepping into next statement.
```csharp
string ComputeSecretProof(string accessToken) {
    string secretProof = from appSecret in Configuration["AppSecret"].AsOne()
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
string json = from client in httpClient.AsOne().DoNotDisposeValue()
              let uri = new Uri(uriString)
              select client.GetString(uri);
// httpClient is not disposed and can still be reused
```

# Planned Features
1. Full NRT Support
2. Task Chaining
```csharp
Person person = await from response in httpClient.GetAsync(uri)
                      from json in response.Content.ReadAsStringAsync()
                      select JsonConvert.DeserializeObject<Person>(json);
```

Please submit an issue if you need to report bugs or request a feature.
