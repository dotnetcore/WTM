using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace WalkingTec.Mvvm.Core.Test.VM
{
    [TestClass]
    public class BasePaedListVMTest
    {
        private BasePagedListVM<Student,BaseSearcher> _studentListVM;
        private string _seed;

        public BasePaedListVMTest()
        {
            _seed = Guid.NewGuid().ToString();
            _studentListVM.DC = new DataContext(_seed, DBTypeEnum.Memory);
        }


    }
}
