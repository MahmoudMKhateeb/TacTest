namespace TACHYON.Dashboards.Shared.Dto
{
    public class ContainerReturnTrackerWidgetDataDto
    {

        /// <summary>
        /// Display the total number of containers that have not yet been returned, regardless of the deadline.
        ///(Example: If a container’s return was due yesterday or next week, it will still count towards this total.)
        /// </summary>
        public int Total  { get; set; }

        /// <summary>
        /// Show the count of containers that are overdue for return.
        /// </summary>
        public int OverDue { get; set; }

        /// <summary>
        /// Indicate the number of containers that have return date less tan X days from now .
        /// </summary>
        public int LessThatXDaysRemaining { get; set; }

        /// <summary>
        /// Indicate the number of containers that have return date after X days from now .
        /// </summary>
         public int MoreThanXDaysRemaining { get; set; }
        
        /// <summary>
        /// Indicate the number of containers that have no container return date and not returned yet.
        /// </summary>
         public int WithoutReturnDate { get; set; }
    }
}