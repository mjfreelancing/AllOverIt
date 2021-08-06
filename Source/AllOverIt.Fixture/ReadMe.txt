Inherit all of your fixture classes from FixtureBase and gain access to commonly used features, such as:

 * Create one or more randomly initialized primitives (or models)
 * Create a randomly initialized primitive that is guaranteed not to have a specified value
 * Get a random value between an upper and lower limit
 * And more....

 FixtureBase is merely a base class that simplifies access to commonly used methods built on top of AutoFixture.
 
 The base class provides access to AutoFixture via a protected property called 'Fixture'.