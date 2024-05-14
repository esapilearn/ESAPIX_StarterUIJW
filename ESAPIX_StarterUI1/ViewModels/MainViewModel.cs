using ESAPIX.Interfaces;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESAPIX.Common;
using VMS.TPS.Common.Model.API;
using Prism.Commands;
using System.Windows;
using ESAPIX.Extensions;
using ESAPIX.Constraints.DVH;
using System.Collections.ObjectModel;
using ESAPIX.Constraints;
using ESAPX_StarterUI.ViewModels.ESAPX_StarterUI.ViewModels;

namespace ESAPX_StarterUI.ViewModels
{
    public class MainViewModel : BindableBase
    {
        AppComThread VMS = AppComThread.Instance;

        public MainViewModel()
        {
            EvaluateCommand = new DelegateCommand(() =>
            {
                foreach (var pc in Constraints)
                {
                    var result = VMS.GetValue(sc =>
                    {
                        //Check if we can constrain first
                        var canConstrain = pc.Constraint.CanConstrain(sc.PlanSetup);
                        //If not..report why
                        if (!canConstrain.IsSuccess) { return canConstrain; }
                        else
                        {
                            //Can constrain - so do it
                            return pc.Constraint.Constrain(sc.PlanSetup);
                        }
                    });
                    //Update UI
                    pc.Result = result;
                }
            });


            CreateConstraints();
        }

        private void CreateConstraints()
        {
            Constraints.AddRange(new PlanConstraint[]
            {
                new PlanConstraint(ConstraintBuilder.Build("PTV45", "Max[%] <= 110")),
                new PlanConstraint(ConstraintBuilder.Build("Rectum", "V75Gy[%] <= 15")),
                new PlanConstraint(ConstraintBuilder.Build("Rectum", "V65Gy[%] <= 35")),
                new PlanConstraint(ConstraintBuilder.Build("Bladder", "V80Gy[%] <= 15")),
                //new PlanConstraint(new CTDateConstraint())
            });
        }


        public DelegateCommand EvaluateCommand { get; set; }
        public ObservableCollection<PlanConstraint> Constraints { get; private set; } = new ObservableCollection<PlanConstraint>();
    }



namespace ESAPX_StarterUI.ViewModels
    {
        public class PlanConstraint : BindableBase
        {
            public PlanConstraint(IConstraint con)
            {
                this.Constraint = con;
            }

            private IConstraint _constraint;
            public IConstraint Constraint
            {
                get { return _constraint; }
                set { SetProperty(ref _constraint, value); }
            }

            private ConstraintResult _result;
            public ConstraintResult Result
            {
                get { return _result; }
                set { SetProperty(ref _result, value); }
            }
        }
    }
}