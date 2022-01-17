using TACHYON.EntityLogs;

namespace TACHYON.Routs.RoutPoints.RoutPointSmartEnum
{

    #region PickUp Transuctions
    public class RoutPointPickUpStep1 : EntityLogTransaction
    {
        public RoutPointPickUpStep1(string displayName, int value) : base(displayName, value)
        {
        }
        public sealed override string Transaction { get => RoutPointConsts.PickUpStep1; }
    }
    public class RoutPointPickUpStep2 : EntityLogTransaction
    {
        public RoutPointPickUpStep2(string displayName, int value) : base(displayName, value)
        {
        }
        public sealed override string Transaction { get => RoutPointConsts.PickUpStep2; }
    }
    public class RoutPointPickUpStep3 : EntityLogTransaction
    {
        public RoutPointPickUpStep3(string displayName, int value) : base(displayName, value)
        {
        }
        public sealed override string Transaction { get => RoutPointConsts.PickUpStep3; }
    }
    public class RoutPointPickUpStep4 : EntityLogTransaction
    {
        public RoutPointPickUpStep4(string displayName, int value) : base(displayName, value)
        {
        }
        public sealed override string Transaction { get => RoutPointConsts.PickUpStep4; }
    }
    #endregion

    #region Drop Off Transuctions
    public class RoutPointDropOffStep1 : EntityLogTransaction
    {
        public RoutPointDropOffStep1(string displayName, int value) : base(displayName, value)
        {
        }
        public sealed override string Transaction { get => RoutPointConsts.DropOffStep1; }
    }
    public class RoutPointDropOffStep2 : EntityLogTransaction
    {
        public RoutPointDropOffStep2(string displayName, int value) : base(displayName, value)
        {
        }
        public sealed override string Transaction { get => RoutPointConsts.DropOffStep2; }
    }
    public class RoutPointDropOffStep3 : EntityLogTransaction
    {
        public RoutPointDropOffStep3(string displayName, int value) : base(displayName, value)
        {
        }
        public sealed override string Transaction { get => RoutPointConsts.DropOffStep3; }
    }
    public class RoutPointDropOffStep4 : EntityLogTransaction
    {
        public RoutPointDropOffStep4(string displayName, int value) : base(displayName, value)
        {
        }
        public sealed override string Transaction { get => RoutPointConsts.DropOffStep4; }
    }
    public class RoutPointDropOffStep5 : EntityLogTransaction
    {
        public RoutPointDropOffStep5(string displayName, int value) : base(displayName, value)
        {
        }
        public sealed override string Transaction { get => RoutPointConsts.DropOffStep5; }
    }
    public class RoutPointDropOffStep6 : EntityLogTransaction
    {
        public RoutPointDropOffStep6(string displayName, int value) : base(displayName, value)
        {
        }
        public sealed override string Transaction { get => RoutPointConsts.DropOffStep6; }
    }
    public class RoutPointDropOffStep7 : EntityLogTransaction
    {
        public RoutPointDropOffStep7(string displayName, int value) : base(displayName, value)
        {
        }
        public sealed override string Transaction { get => RoutPointConsts.DropOffStep7; }
    }

    public class RoutPointDropOffStep8 : EntityLogTransaction
    {
        public RoutPointDropOffStep8(string displayName, int value) : base(displayName, value)
        {
        }
        public sealed override string Transaction { get => RoutPointConsts.DropOffStep8; }
    }
    #endregion

    public class RoutPointAction1 : EntityLogTransaction
    {
        public RoutPointAction1(string displayName, int value) : base(displayName, value)
        {
        }
        public sealed override string Transaction { get => RoutPointConsts.Action1; }
    }
    public class RoutPointAction2 : EntityLogTransaction
    {
        public RoutPointAction2(string displayName, int value) : base(displayName, value)
        {
        }
        public sealed override string Transaction { get => RoutPointConsts.Action2; }
    }
    public class RoutPointAction3 : EntityLogTransaction
    {
        public RoutPointAction3(string displayName, int value) : base(displayName, value)
        {
        }
        public sealed override string Transaction { get => RoutPointConsts.Action3; }
    }
}