/// <summary>
///     Configuration class which allows users to customize or filter a Waterfall.
/// </summary>
public class WaterfallConfiguration
{
    /// <summary>
    ///     Initializes a new instance of the WaterfallConfiguration class.
    /// </summary>
    /// <param name="ceiling">The ceiling value.</param>
    /// <param name="floor">The floor value.</param>
    private WaterfallConfiguration(double? ceiling, double? floor)
    {
        this.Ceiling = ceiling;
        this.Floor = floor;
    }

    /// <summary>
    ///     Gets the ceiling value.
    /// </summary>
    public double? Ceiling { get; }

    /// <summary>
    ///     Gets the floor value.
    /// </summary>
    public double? Floor { get; }

    /// <summary>
    ///     Gets a builder for creating instances of WaterfallConfiguration.
    /// </summary>
    /// <returns>The WaterfallConfigurationBuilder.</returns>
    public static WaterfallConfigurationBuilder Builder()
    {
        return new WaterfallConfigurationBuilder();
    }

    /// <summary>
    ///     Creates an empty instance of WaterfallConfiguration.
    /// </summary>
    /// <returns>The empty WaterfallConfiguration.</returns>
    public static WaterfallConfiguration Empty()
    {
        return new WaterfallConfiguration(double.NaN, double.NaN);
    }

    /// <summary>
    ///     Builder class which to create a WaterfallConfiguration.
    /// </summary>
    public class WaterfallConfigurationBuilder
    {
        private double? ceiling;
        private double? floor;

        internal WaterfallConfigurationBuilder()
        {
        }

        /// <summary>
        ///     Sets the ceiling value.
        /// </summary>
        /// <param name="ceiling">The ceiling value.</param>
        /// <returns>The WaterfallConfigurationBuilder.</returns>
        public WaterfallConfigurationBuilder SetCeiling(double ceiling)
        {
            this.ceiling = ceiling;
            return this;
        }

        /// <summary>
        ///     Sets the floor value.
        /// </summary>
        /// <param name="floor">The floor value.</param>
        /// <returns>The WaterfallConfigurationBuilder.</returns>
        public WaterfallConfigurationBuilder SetFloor(double floor)
        {
            this.floor = floor;
            return this;
        }

        /// <summary>
        ///     Builds an instance of WaterfallConfiguration based on the configured values.
        /// </summary>
        /// <returns>The created WaterfallConfiguration.</returns>
        public WaterfallConfiguration Build()
        {
            return new WaterfallConfiguration(ceiling, floor);
        }
    }
}