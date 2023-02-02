namespace TACHYON.Tracking
{
    public static class WorkflowVersionConst
    {
        public const int PickupPointWorkflowVersion = 0;
        public const int DropOffWithoutDeliveryNotePointWorkflowVersion = 1;
        public const int DropOffWithDeliveryNotePointWorkflowVersion = 2;
        
        
        public const int PickupHomeDeliveryWorkflowVersion = 3;
        /// Home delivery without pod and without receiver code.
        public const int DropOffHomeDeliveryWorkflowVersion = 4;
        /// Home delivery with pod and without receiver code.
        public const int DropOffHomeDeliveryWithPodWorkflowVersion = 5;
        /// Home delivery without pod and with receiver code.
        public const int DropOffHomeDeliveryWithReceiverCodeWorkflowVersion = 6;
        /// Home delivery with pod & receiver code
        public const int DropOffHomeDeliveryWithPodAndReceiverCodeWorkflowVersion = 7;
        /// ports movements drop off point workflow version
        public const int DropOffPortsMovementWorkflowVersion = 7;
        
        
    }
}