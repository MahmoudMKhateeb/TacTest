namespace TACHYON.Dashboards.Shared.Dto
{
    public class ContainerReturnTrackerWidgetDataDto
    {

        /// <summary>
        /// Display the total number of containers that have not yet been returned, regardless of the deadline.
        ///(Example: If a container’s return was due yesterday or next week, it will still count towards this total.)
        /// </summary>
        public int TotalUnreturnedContainers  { get; set; }

        /// <summary>
        /// Show the count of containers that are overdue for return.
        /// </summary>
        public int DelayedUnreturnedContainers { get; set; }

        /// <summary>
        /// Indicate the number of containers that must be returned within the next X days.
        /// </summary>
        public int UpcomingUnreturnedContainers { get; set; }
    }
}