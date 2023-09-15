# Event Aggregation
---

## EventAggregator

The `EventAggregator` acts as a messaging hub when used as a singleton. Various layers within an application can subscribe for messages published by other areas of the application; and all in a strongly-typed manner.

Consider a situation where you need to pass a reference to an object around to different parts of an application that have no direct knowledge of each other. The class below represents such an object:

```csharp
public sealed class SiteChanged 
{
  public ISiteDetails SiteDetails { get; }

  public SiteChanged(ISiteDetails siteDetails)
  {
    SiteDetails = siteDetails;
  }
}
```

### Publishing
Messages can be published using the synchronous `Publish()` or asynchronous `PublishAsync()` method, like so:

```csharp
// currentSite is an instance of ISiteDetails
// eventAggregator is an instance of EventAggregator
var siteChanged = new SiteChanged(currentSite);
await eventAggregator.PublishAsync(siteChanged);
```

### Subscribing
Use the `Subscribe()` method to be notified of new messages, like so:

```csharp
eventAggregator.Subscribe<SiteChanged>(HandleSiteChanged);
```

Where the `HandleSiteChanged()` method is declared like so:

```csharp
private void HandleSiteChanged(SiteChanged message)
{
  // required implementation here
}
```

#### Weak subscriptions
The `Subscribe()` method defaults to using weak references. This provides the ability for subscriptions to be automatically released if the subscriber is disposed of. If you require a strong referenced subscription, call the `Subscribe()` method like so:

```csharp
eventAggregator.Subscribe<SiteChanged>(HandleSiteChanged, false);
```

### Unsubscribing
When you no longer require to be notified of messages, such as during disposal, the events can be unsubscribed used the `Unsubscribe()` method, like so:

```csharp
eventAggregator.Unsubscribe<SiteChanged>(HandleSiteChanged);
```
