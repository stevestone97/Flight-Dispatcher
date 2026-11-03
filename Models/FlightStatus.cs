namespace FlightDispatcherApp.Models
{
    /// <summary>
    /// Specifies the lifecycle state of a flight.
    /// </summary>
    public enum FlightStatus
    {
        /// <summary>
        /// The flight is registered and ready to depart.
        /// </summary>
        Ready,

        /// <summary>
        /// The flight has departed and is still airborne.
        /// </summary>
        InAir,

        /// <summary>
        /// The flight has completed its journey.
        /// </summary>
        Landed
    }
}
