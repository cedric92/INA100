using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INA.Model
{
    class ProgressBarControl
    {
        ViewModel.ViewModel _ViewModel;
        //stepvalue defines how much a single step in the progress bar will be
        private double stepValue=0;

        public ProgressBarControl(ViewModel.ViewModel vm)
        {
            this._ViewModel = vm;
        }

        //steps defines how many steps in the progressbar will be
        //for each file there should be two steps
        //e.g. : 4 files => steps = 8
        public void setProgressStatus(int steps)
        {
            // double: 100/ 3 = 33,3333... 
            // int: 100 / 3 = 33 -> error
            stepValue = 100 / Convert.ToDouble(steps);
            _ViewModel.ProgressStatus += stepValue;
        }

        public void setProgressStatus()
        {
            _ViewModel.ProgressStatus += stepValue;
        }
    }
}
