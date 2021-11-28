using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using TACHYON.Routs.RoutPoints;
using TACHYON.Routs.RoutPoints.RoutPointSmartEnum;
using TACHYON.SmartEnums;

namespace TACHYON.EntityLogs
{
    public abstract class EntityLogTransaction : SmartEnum
    {

        public static readonly EntityLogTransaction RoutPointStep1 = new RoutPointStep1(nameof(RoutPointStep1), 1);
        public static readonly EntityLogTransaction RoutPointStep2 = new RoutPointStep1(nameof(RoutPointStep2), 2);
        public static readonly EntityLogTransaction RoutPointStep3 = new RoutPointStep1(nameof(RoutPointStep3), 3);
        public static readonly EntityLogTransaction RoutPointStep4 = new RoutPointStep1(nameof(RoutPointStep4), 4);
        public static readonly EntityLogTransaction RoutPointStep5 = new RoutPointStep1(nameof(RoutPointStep5), 5);
        public static readonly EntityLogTransaction RoutPointStep6 = new RoutPointStep1(nameof(RoutPointStep6), 6);
        public static readonly EntityLogTransaction RoutPointStep7 = new RoutPointStep1(nameof(RoutPointStep7), 7);
        public static readonly EntityLogTransaction RoutPointStep8 = new RoutPointStep1(nameof(RoutPointStep8), 8);

        public static readonly EntityLogTransaction RoutPointAction1 = new RoutPointAction1(nameof(RoutPointAction1), 9);
        public static readonly EntityLogTransaction RoutPointAction2 = new RoutPointAction2(nameof(RoutPointAction2), 10);
        public static readonly EntityLogTransaction RoutPointAction3 = new RoutPointAction3(nameof(RoutPointAction3), 11);

        public static readonly EntityLogTransaction DefaultLogTransaction = new DefaultLogTransaction();


        public abstract string Transaction { get; }

        protected EntityLogTransaction(string displayName, int value) : base(displayName, value)
        {
        }
    }



    public class DefaultLogTransaction : EntityLogTransaction
    {
        public DefaultLogTransaction() : base(nameof(DefaultLogTransaction), -1)
        {
        }

        public override string Transaction { get => "NoReasonProvided"; } // Without Spaces To Use It As Localized String Key
    }
}