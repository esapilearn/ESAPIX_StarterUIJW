using ESAPIX.Constraints;
using System;
using VMS.TPS.Common.Model.API;

namespace ESAPX_StarterUI.ViewModels
{
    public class CTDateConstraint : IConstraint
    {
        public string Name => "CT DATE CONSTRAINT";

        public string FullName => "CT < 60 days old";

        public ConstraintResult CanConstrain(PlanningItem pi)
        {
            if(pi.StructureSet.Image!=null) { return new ConstraintResult(this, ResultType.PASSED, ""); }
            else { return new ConstraintResult(this, ResultType.NOT_APPLICABLE, "Missing CT"); }
        }

        public ConstraintResult Constrain(PlanningItem pi)
        {
            var ctDate = pi.StructureSet.Image.CreationDateTime;
            return ConstrainCTDate(ctDate.Value);
        }

        public ConstraintResult ConstrainCTDate(System.DateTime ctDate)
        {
            var daysOld = (DateTime.Now - ctDate).TotalDays;
            var limit = DateTime.Now.AddDays(-60);
            if (ctDate > limit)
            {
                return new ConstraintResult(this, ResultType.PASSED, $"The CT is {daysOld} days old");
            }
            else
            {
                return new ConstraintResult(this, ResultType.ACTION_LEVEL_3, $"The CT is {daysOld} days old");
            }
        }
    }
}