namespace GraphqlSchema.Schema.Types
{
    internal enum MeterType
    {
        Production,             // produced by the panel/inverter
        Consumption,            // total consumption
        SelfConsumption,        // calculated (Production - FeedIn)
        FeedIn,                 // power fed back into the grid
        Purchased               // power taken from the grid
    }
}