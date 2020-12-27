using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Demo.Models;


namespace WalkingTec.Mvvm.Demo.ViewModels.StudentVMs
{
    public partial class StudentVM : BaseCRUDVM<Student>
    {
        public List<ComboSelectListItem> AllStudentMajors { get; set; }
        [Display(Name = "专业")]
        public List<Guid> SelectedStudentMajorIDs { get; set; }

        public StudentVM()
        {
            SetInclude(x => x.StudentMajor);
        }

        public override DuplicatedInfo<Student> SetDuplicatedCheck()
        {
            var rv = CreateFieldsInfo(SimpleField(x => x.Name), SimpleField(x => x.IsValid));
            return rv;
        }

        protected override void InitVM()
        {
            AllStudentMajors = DC.Set<Major>().GetSelectListItems(LoginUserInfo?.DataPrivileges, null, y => y.MajorName);
            SelectedStudentMajorIDs = Entity.StudentMajor?.Select(x => x.MajorId).ToList();
        }

        public override void DoAdd()
        {
            Entity.StudentMajor = new List<StudentMajor>();
            if (SelectedStudentMajorIDs != null)
            {
                foreach (var id in SelectedStudentMajorIDs)
                {
                    Entity.StudentMajor.Add(new StudentMajor { MajorId = id });
                }
            }
           
            base.DoAdd();
        }

        public override void DoEdit(bool updateAllFields = false)
        {
            Entity.StudentMajor = new List<StudentMajor>();
            if(SelectedStudentMajorIDs != null )
            {
                SelectedStudentMajorIDs.ForEach(x => Entity.StudentMajor.Add(new StudentMajor { ID = Guid.NewGuid(), MajorId = x }));
            }

            base.DoEdit(updateAllFields);
        }

        public override void DoDelete()
        {
            base.DoDelete();
        }
    }
}
