# Introduction
DataLink is a gateway for uniform access to arbitrary data sources. This highly configurable framework offers an agnostic API to route, filter, intercept and orchestrate CRUD operations using LINQ. The library provides inversion of control for data as opposed to service interfaces.

## Why?
Datalink was originally built to provide an abstraction layer to standardize access to technologies like EF Core, Cosmos DB, and MongoDB. Eventually, it was expanded to accommodate data provisioned through REST services, Redis and OData. Concerns such as querying, security, auditing and data transformation could now be universally handled regardless of the underlying provider.

## Features
 - *Routing:* Map types to providers such as ORMs, external services, caches or any other IQueryable source.
 - *Filtering:* Secure your data with composable query filters to modify IQueryables or expression trees.
 - *Interception:* Hook any part of the query pipeline with simple rule-based interceptors.
 - *Polymorphism:* Full support for automatic type coercion and interfaces for all operations.
 - *Transactions:* Automatic changetracking offers performant handling for atomic units of work.
 - *Aggregation:* Seamlessly query multiple data sources in parallel; the results are automatically aggregated.
 - *Asynchronous:* All applicable operations support async / await.

## How does it work?
// TODO

## Usage

#### Setup / routing
```C#
// Define an in-memory collection
var characters = new List<Character> {
    new () { Name = "Luke Skywalker" },
    new () { Name = "Han Solo" },
};

// Configure a hub to use the collection
var hub = DataHubConfiguration
    .Use(characters.AsQueryable())
    .CreateHub();

// Query by type, which will fetch from the mapped collection
var characters = hub.Get<Character>().ToList();
```

#### Type conversion
// TODO

#### Query filters
Use the strongly typed QueryFilter class to apply filters to queries with custom rules.
```C#
public class JediFilter : QueryFilter<Character> {
    protected override Expression<Func<Character, bool>> GetFilterExpression() {
        return character => character.IsJedi == true;
    }
}
```
Initialize the filters during configuration. All queries will automatically be augmented with the filter expression.
```C#
var hub = DataHubConfiguration
    .Use<CharacterRepository>()
    .Use<JediFilter>()
    .CreateHub();

// Only characters with the 'IsJedi' flag on planet 'Bespin' will be returned
var characters = hub.Get<Character>().Where(each => each.Planet == "Bespin").ToList();
```

#### Interception
You can use interceptors to inject behaviors into any part of the data pipeline.
```C#
public class HeroLogger : LoadInterceptor<Character> {
    private ILogger _logger;
    
    // Use an IoC container to inject logger
    public HeroLogger(ILogger logger) {
        _logger = logger;
    }

    protected override void OnLoad(Hero hero) {
        _logger.LogInformation("Loaded hero: " + hero.Name);
    }
}
```