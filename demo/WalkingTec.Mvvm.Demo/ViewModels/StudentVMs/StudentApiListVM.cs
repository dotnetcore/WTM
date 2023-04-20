﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using WalkingTec.Mvvm.Demo.Models;


namespace WalkingTec.Mvvm.Demo.ViewModels.StudentVMs
{
    public partial class StudentApiListVM : BasePagedListVM<StudentApi_View, StudentApiSearcher>
    {

        protected override Task<IEnumerable<IGridColumn<StudentApi_View>>> InitGridHeader()
        {
            return new List<GridColumn<StudentApi_View>>{
                this.MakeGridHeader(x => x.ID),
                this.MakeGridHeader(x => x.Password),
                this.MakeGridHeader(x => x.Email),
                this.MakeGridHeader(x => x.Name),
                this.MakeGridHeader(x => x.Sex),
                this.MakeGridHeader(x => x.CellPhone),
                this.MakeGridHeader(x => x.Address),
                this.MakeGridHeader(x => x.ZipCode),
                this.MakeGridHeader(x => x.PhotoId).SetFormat(PhotoIdFormat),
                this.MakeGridHeader(x => x.IsValid),
                this.MakeGridHeader(x => x.EnRollDate),
                this.MakeGridHeader(x => x.MajorName_view),
                this.MakeGridHeaderAction(width: 200)
            };
        }
        private List<ColumnFormatInfo> PhotoIdFormat(StudentApi_View entity, object val)
        {
            return new List<ColumnFormatInfo>
            {
                ColumnFormatInfo.MakeDownloadButton(ButtonTypesEnum.Button,entity.PhotoId),
                ColumnFormatInfo.MakeViewButton(ButtonTypesEnum.Button,entity.PhotoId,640,480),
            };
        }


        public override Task<IOrderedQueryable<StudentApi_View>> GetSearchQuery()
        {
            var query = DC.Set<Student>()
                .CheckContain(Searcher.ID, x=>x.ID)
                .CheckContain(Searcher.Email, x=>x.Email)
                .CheckBetween(Searcher.EnRollDate?.GetStartTime(), Searcher.EnRollDate?.GetEndTime(), x => x.EnRollDate, includeMax: false)
                .Select(x => new StudentApi_View
                {
				    ID = x.ID,
                    Password = x.Password,
                    Email = x.Email,
                    Name = x.Name,
                    Sex = x.Sex,
                    CellPhone = x.CellPhone,
                    Address = x.Address,
                    ZipCode = x.ZipCode,
                    PhotoId = x.PhotoId,
                    IsValid = x.IsValid,
                    EnRollDate = x.EnRollDate,
                    MajorName_view = x.StudentMajor.Select(y=>y.Major.MajorName).ToSepratedString(null,","), 
                })
                .OrderBy(x => x.ID);
            return query;
        }

    }

    public class StudentApi_View : Student{
        [Display(Name = "专业名称")]
        public String MajorName_view { get; set; }

    }
}
