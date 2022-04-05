## Introduction
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
The **DataHub** class is simply a router that maps types to data sources. It provides an abstraction layer to access these data sources by type lookup. Finally, a middleware pipeline is injected into a custom IQueryProvider so that queries and result sets can be augmented.

### Setup
#### Start with a  configuration:
```C#
var configuration = DataHubConfiguration.Default();
```
#### Register a collection that we want to access:
```C#
var characters = new List<Character> {
    new () { Name = "Luke Skywalker" },
    new () { Name = "Han Solo" },
};

configuration.Use(characters);
```
#### Create the hub and then fetch the collection by type
```C#
var hub = configuration.CreateHub();
var characters = hub.Get<Character>().ToList();
```

### Query filters
Use the **QueryFilter** class to apply filters to queries with custom rules. This is useful for enforcing security or otherwise limiting the result set. All queries will automatically be augmented with the filter expression.
#### Filter by type
```C#
public class JediFilter : QueryFilter<Character> {
    protected override Expression<Func<Character, bool>> GetFilterExpression() {
        return character => character.IsJedi == true;
    }
}
```
#### Initialize the filters during configuration
```C#
var hub = DataHubConfiguration.Default()
    .Use(characters)
    .Use<JediFilter>()
    .CreateHub();

// Only characters with the 'IsJedi' flag on planet 'Bespin' will be returned
var characters = hub.Get<Character>().Where(each => each.Planet == "Bespin").ToList();
```

### Interception
You can use interceptors to inject behaviors into any part of the data pipeline.
#### Log when any entity is loaded
```C#
public class EntityLogger : LoadInterceptor<object> {
    protected override void OnLoad(object entity) {
        Console.Log("Loaded entity: " + entity);
    }
}
```

#### Log when specific entities types are loaded
```C#
public class CharacterLogger : LoadInterceptor<Character> {
    private ILogger _logger;
    
    // Use an IoC container to inject logger
    public CharacterLogger(ILogger logger) {
        _logger = logger;
    }

    protected override void OnLoad(Character character) {
        _logger.LogInformation("Loaded character: " + character.Name);
    }
}
```

#### Log when specific entities types are saved
```C#
public class CharacterSavedLogger : SaveInterceptor<Character> {
    protected override void OnBeforeSave(Character character) {
        Console.Log("Before saving character: " + character.Name);
    }

    protected override void OnAfterSave(Character character) {
        Console.Log("After saving character: " + character.Name);
    }
}
```

#### Log when any entity is deleted
```C#
public class EntityDeletionLogger : DeleteInterceptor<Character> {
    protected override void OnBeforeDelete(Character character) {
        Console.Log("Before deleting character: " + character.Name);
    }

    protected override void OnAfterDelete(Character character) {
        Console.Log("After deleting character: " + character.Name);
    }
}
```

### Change tracking
DataLink uses internal snapshots to keep track of when properties change. This allows you to create interception rules if you need special handling for certain types of modifications. These interceptors trigger before saving the entity if the property has changed.
#### Log when a property has changed
```C#
public class LocationChangeLogger : ChangeInterceptor<Character> {

    public LocationChangeLogger() {
        On(character => character.Location, OnLocationChanged);
    }

    private async Task OnLocationChanged(Character character, string newLocation) {
        Console.Log("Character location has changed to: " + newLocation);
    }
}
```

#### Log when an item has been added or removed from a list
```C#
public class InventoryLogger : ChangeInterceptor<Character> {

    public InventoryLogger() {
        OnAdd(character => character.Inventory, OnInventoryItemAdded);
        OnRemove(character => character.Inventory, OnInventoryItemRemoved);
    }

    private async Task OnInventoryItemAdded(Character character, Item item) {
        Console.Log("Added item to inventory: " + item.Name);
    }

    private async Task OnInventoryItemRemoved(Character character, Item item) {
        Console.Log("Removed item from inventory: " + item.Name);
    }
}
```

### Automatic type conversion
You can use interfaces, subclasses, or base classes for any of the above examples even if the underlying data provider does not natively support it. This allows you to enforce cross-cutting rules against data contracts.
```C#
public class Planet : ILocation {
    public bool IsHabitable { get; set; }
    public int Size { get; set; }
}

public class SpaceStation : ILocation {
    public string Faction { get; set; }
    public int Size { get; set; }
}
```

#### Query by interface
If multiple compatible types are matched, results will be automatically aggregated.
```C#
// The results include a combined set of both planets and space stations
var locations = hub.Get<ILocation>().ToList();
```
```C#
// Likewise, the sum will cumulate both planets and space stations
var locations = hub.Get<ILocation>().Sum();
```