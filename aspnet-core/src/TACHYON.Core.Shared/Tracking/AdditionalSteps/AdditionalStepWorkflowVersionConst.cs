namespace TACHYON.Tracking.AdditionalSteps
{
    public static class AdditionalStepWorkflowVersionConst
    {
        /// Ports Movement - Import -> With/out Return Trip -> `First Trip`
        public const int PortsMovementImportFirstTripVersion = 1;
        /// Ports Movement - Import -> Without Return Trip -> `Return Trip`
        public const int PortsMovementImportReturnTripVersion = 2;
        /// Ports Movement - Export -> Two way Route with/out port shuttling -> `First Trip`
        public const int PortsMovementExportFirstTripVersion = 3;
        /// Ports Movement - Export -> Two way Route with/out port shuttling -> `Second Trip`
        public const int PortsMovementExportSecondTripVersion = 4;
        /// Ports Movement - Export -> Two way Route with port shuttling  -> `One Trip Only, Third Trip`
        public const int PortsMovementExportThirdTripVersion = 5;
        public const int PortsMovementExportOneWayFirstTripVersion = 6;
    }
}