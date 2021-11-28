using TACHYON.EntityLogs;

namespace TACHYON.Routs.RoutPoints.RoutPointSmartEnum
{

    public class RoutPointStep1 : EntityLogTransaction
    {
        public RoutPointStep1(string displayName, int value) : base(displayName, value)
        {
        }

        public sealed override string Transaction { get => RoutPointConsts.Step1; }
    }

    public class RoutPointStep2 : EntityLogTransaction
    {
        public RoutPointStep2(string displayName, int value) : base(displayName, value)
        {
        }

        public sealed override string Transaction { get => RoutPointConsts.Step2; }
    }

    public class RoutPointStep3 : EntityLogTransaction
    {
        public RoutPointStep3(string displayName, int value) : base(displayName, value)
        {
        }

        public sealed override string Transaction { get => RoutPointConsts.Step3; }
    }

    public class RoutPointStep4 : EntityLogTransaction
    {
        public RoutPointStep4(string displayName, int value) : base(displayName, value)
        {
        }

        public sealed override string Transaction { get => RoutPointConsts.Step4; }
    }

    public class RoutPointStep5 : EntityLogTransaction
    {
        public RoutPointStep5(string displayName, int value) : base(displayName, value)
        {
        }

        public sealed override string Transaction { get => RoutPointConsts.Step5; }
    }

    public class RoutPointStep6 : EntityLogTransaction
    {
        public RoutPointStep6(string displayName, int value) : base(displayName, value)
        {
        }

        public sealed override string Transaction { get => RoutPointConsts.Step6; }
    }

    public class RoutPointStep7 : EntityLogTransaction
    {
        public RoutPointStep7(string displayName, int value) : base(displayName, value)
        {
        }

        public sealed override string Transaction { get => RoutPointConsts.Step7; }
    }

    public class RoutPointStep8 : EntityLogTransaction
    {
        public RoutPointStep8(string displayName, int value) : base(displayName, value)
        {
        }

        public sealed override string Transaction { get => RoutPointConsts.Step8; }
    }

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