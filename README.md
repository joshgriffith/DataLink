# DataLink
DataLink is a .NET gateway for uniform access to arbitrary data sources. This highly configurable framework offers a simple abstraction layer to route, filter, intercept and orchestrate CRUD operations using LINQ.

## Features
--------
 - *Routing:* Easily map types and symbols to data providers. This allows you to seamlessly route specific types to ORMs, external services, caches or any other IQueryable source.
 - *Filtering:* Secure your data with composable query filters that allow you to modify IQueryables and expressions in transit.
 - *Interception:* Handle cross-cutting concerns by hooking any part of the query pipeline for full control over the operation or result set.
 - *Polymorphism:* Full support for interfaces / subtypes / base types for all query operations, even if the underlying provider or ORM lacks support.
 - *Transactional:* Built-in snapshot change-tracking provides a performant solution for managing transactions and committing changesets atomically.
 - *Aggregation:* Query multiple data sources in parallel; the results are automatically aggregated.

## Tutorials and examples