namespace Xunit;

[CollectionDefinition(nameof(NonParallelCollectionDefinition), DisableParallelization = true)]
public class NonParallelCollectionDefinition { }
