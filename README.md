# DataLink
DataLink is a gateway for uniform access to arbitrary data sources. This highly configurable framework offers a simple abstraction layer to route, filter, intercept and orchestrate CRUD operations using LINQ.

## Features
 - *Routing:* Map types to providers such as ORMs, external services, caches or any other IQueryable source.
 - *Filtering:* Secure your data with composable query filters to modify IQueryables or expression trees.
 - *Interception:* Hook any part of the query pipeline with rule-based interceptors.
 - *Polymorphism:* Full support for interfaces / type coercion for all operations.
 - *Transactions:* Internal changetracking offers performant handling for atomic units of work.
 - *Aggregation:* Seamlessly query multiple data sources in parallel; the results are automatically aggregated.

## Examples
```C#
// Define an in-memory collection
var collection = new List<Person> {
    new () { Name = "Test1" },
    new () { Name = "Test2" },
};

// Configure a hub to use the collection
var hub = DataHubConfiguration.Default()
    .Use(collection.AsQueryable())
    .CreateHub();

// Query by type, which will fetch the persons from the collection
var results = hub.Get<Person>().ToList();
```