using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.IO;
using System.Linq;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Models;

namespace WalkingTec.Mvvm.Core.Test.VM
{
    [TestClass]
    [TestCategory("Import")]
    public class BaseImportVMTest : CrudTestBase
    {
        private ImportSchoolVM _importVm;
        private ImportSchoolWithMajorsVM _importSubVm;

        public BaseImportVMTest()
        {
            InitSeed();
            _importVm = new ImportSchoolVM { Wtm = NewWtmContext() };
            _importVm.Wtm.ConfigInfo.Connections = new List<CS>
            {
                new CS { Key = "default", Value = Seed, DbType = DBTypeEnum.Memory, DbContext = nameof(DataContext) }
            };

            _importSubVm = new ImportSchoolWithMajorsVM { Wtm = NewWtmContext() };
            _importSubVm.Wtm.ConfigInfo.Connections = _importVm.Wtm.ConfigInfo.Connections;
        }

        #region 构造与基本功能

        [TestMethod] public void Construction_InitializesTemplate()
        {
            Assert.IsNotNull(_importVm.Template);
            Assert.IsInstanceOfType(_importVm.Template, typeof(ImportSchoolTemplateVM));
        }

        [TestMethod] public void Construction_InitializesErrorListVM()
        {
            Assert.IsNotNull(_importVm.ErrorListVM);
            Assert.IsNotNull(_importVm.ErrorListVM.EntityList);
        }

        [TestMethod] public void SetParms_PropagatesToTemplate()
        {
            _importVm.SetParms(new Dictionary<string, string> { { "k1", "v1" } });
            Assert.AreEqual("v1", _importVm.Template.Parms["k1"]);
        }

        [TestMethod] public void GenerateTemplate_ProducesValidXlsx()
        {
            var bytes = _importVm.GenerateTemplate(out var name);
            Assert.IsTrue(bytes.Length > 0);
            Assert.IsTrue(name.EndsWith(".xlsx"));
            using var ms = new MemoryStream(bytes);
            using var doc = SpreadsheetDocument.Open(ms, false);
            Assert.IsNotNull(doc.WorkbookPart);
        }

        #endregion

        #region 导入 → BatchSaveData → 数据库验证

        [TestMethod]
        public void ImportSingleRow_SavesToDatabase()
        {
            var excelBytes = GenExcel(typeof(ImportSchoolTemplateVM).Name,
                new[]{"学校编码","学校名称","备注","学校类型"},
                new[]{"111","测试学校","test remark","公立学校"});
            DoImport(_importVm, excelBytes);
            Assert.IsTrue(_importVm.BatchSaveData(), errMsg(_importVm));

            using var ctx = NewContext();
            var schools = ctx.Set<School>().ToList();
            Assert.AreEqual(1, schools.Count);
            Assert.AreEqual("111", schools[0].SchoolCode);
            Assert.AreEqual("测试学校", schools[0].SchoolName);
            Assert.AreEqual("test remark", schools[0].Remark);
            Assert.AreEqual(SchoolTypeEnum.PUB, schools[0].SchoolType);
            AssertCreatedAudit(schools[0], "user");
        }

        [TestMethod]
        public void ImportMultipleRows_AllSavedToDatabase()
        {
            var excelBytes = GenExcel(typeof(ImportSchoolTemplateVM).Name,
                new[]{"学校编码","学校名称","备注","学校类型"},
                new[]{"001","学校A","rA","公立学校","002","学校B","rB","私立学校","003","学校C","rC","公立学校"});
            DoImport(_importVm, excelBytes);
            Assert.IsTrue(_importVm.BatchSaveData(), errMsg(_importVm));

            using var ctx = NewContext();
            var schools = ctx.Set<School>().OrderBy(x => x.SchoolCode).ToList();
            Assert.AreEqual(3, schools.Count);
            Assert.AreEqual("001", schools[0].SchoolCode);
            Assert.AreEqual("学校A", schools[0].SchoolName);
            Assert.AreEqual("002", schools[1].SchoolCode);
            Assert.AreEqual("学校B", schools[1].SchoolName);
            Assert.AreEqual("003", schools[2].SchoolCode);
            Assert.AreEqual("学校C", schools[2].SchoolName);
        }

        #endregion

        #region 边界值 & 异常值

        [TestMethod]
        public void Import_MissingRequiredField_ValidationFails()
        {
            // SchoolCode 是 Required，传空字符串应在 BatchSaveData 的 TryValidateObject 中失败
            var excelBytes = GenExcel(typeof(ImportSchoolTemplateVM).Name,
                new[]{"学校编码","学校名称","备注","学校类型"},
                new[]{"","test","r","公立学校"});
            DoImport(_importVm, excelBytes);

            Assert.IsFalse(_importVm.BatchSaveData(), "缺少必填字段时 BatchSaveData 应返回 false");
            Assert.IsTrue(_importVm.ErrorListVM.EntityList.Count > 0, "应有验证错误");
        }

        [TestMethod]
        public void Import_InvalidFormat_ValidationFails()
        {
            // SchoolCode 要求 3 位数字 (Regex ^[0-9]{3,3}$)，"abc" 不符合
            var excelBytes = GenExcel(typeof(ImportSchoolTemplateVM).Name,
                new[]{"学校编码","学校名称","备注","学校类型"},
                new[]{"abc","test","r","公立学校"});
            DoImport(_importVm, excelBytes);

            Assert.IsFalse(_importVm.BatchSaveData(), "格式验证不通过应返回 false");
            Assert.IsTrue(_importVm.ErrorListVM.EntityList.Count > 0, "应有格式验证错误");
        }

        [TestMethod]
        public void Import_EmptyExcel_NoRowsNoErrors()
        {
            // 只传表头，不传数据行
            var excelBytes = GenExcel(typeof(ImportSchoolTemplateVM).Name,
                new[]{"学校编码","学校名称","备注","学校类型"},
                new string[0]);
            DoImport(_importVm, excelBytes);

            Assert.AreEqual(0, _importVm.EntityList.Count);
            Assert.AreEqual(0, _importVm.ErrorListVM.EntityList.Count);
        }

        #endregion

        #region 模板下载含已有数据

        [TestMethod]
        public void GenerateTemplate_WithExistingData_IncludesRows()
        {
            using (var ctx = NewContext())
            {
                ctx.Set<School>().Add(new School { SchoolCode = "111", SchoolName = "学校A", Remark = "备注A", SchoolType = SchoolTypeEnum.PUB });
                ctx.Set<School>().Add(new School { SchoolCode = "222", SchoolName = "学校B", Remark = "备注B", SchoolType = SchoolTypeEnum.PRI });
                ctx.SaveChanges();
            }

            var templateVm = new ImportSchoolTemplateWithDataVM { Wtm = NewWtmContext() };
            templateVm.Wtm.ConfigInfo.Connections = _importVm.Wtm.ConfigInfo.Connections;

            var bytes = templateVm.GenerateTemplate(out var name);
            Assert.IsTrue(bytes.Length > 0);
            Assert.IsTrue(name.EndsWith(".xlsx"));

            using var ms = new MemoryStream(bytes);
            using var doc = SpreadsheetDocument.Open(ms, false);
            var rows = doc.WorkbookPart!.WorksheetParts.First().Worksheet.GetFirstChild<SheetData>()!.Elements<Row>().ToList();
            Assert.AreEqual(3, rows.Count, "应有表头 + 2行数据");

            var row2Cells = rows[1].Elements<Cell>().ToList();
            var row3Cells = rows[2].Elements<Cell>().ToList();
            Assert.AreEqual("111", GetCellText(row2Cells[0], doc.WorkbookPart));
            Assert.AreEqual("学校A", GetCellText(row2Cells[1], doc.WorkbookPart));
            Assert.AreEqual("222", GetCellText(row3Cells[0], doc.WorkbookPart));
            Assert.AreEqual("学校B", GetCellText(row3Cells[1], doc.WorkbookPart));
        }

        [TestMethod]
        public void GenerateTemplate_WithSubTableData_IncludesMasterDetailRows()
        {
            // 插入学校+专业数据
            using (var ctx = NewContext())
            {
                var s = new School { SchoolCode = "111", SchoolName = "主学校", Remark = "r", SchoolType = SchoolTypeEnum.PUB };
                s.Majors = new List<Major>
                {
                    new Major { MajorCode = "001", MajorName = "专业一", MajorType = MajorTypeEnum.Required },
                    new Major { MajorCode = "002", MajorName = "专业二", MajorType = MajorTypeEnum.Optional }
                };
                ctx.Set<School>().Add(s);
                ctx.SaveChanges();
            }

            // 使用主子表模板 + 带数据的 VM
            var templateSubVm = new ImportSchoolWithMajorsTemplateWithDataVM { Wtm = NewWtmContext() };
            templateSubVm.Wtm.ConfigInfo.Connections = _importVm.Wtm.ConfigInfo.Connections;

            var bytes = templateSubVm.GenerateTemplate(out var name);
            Assert.IsTrue(bytes.Length > 0);

            using var ms = new MemoryStream(bytes);
            using var doc = SpreadsheetDocument.Open(ms, false);
            var rows = doc.WorkbookPart!.WorksheetParts.First().Worksheet.GetFirstChild<SheetData>()!.Elements<Row>().ToList();
            // 表头 + 2行数据（主表1行 + 子表1延续行）
            Assert.AreEqual(3, rows.Count, "应有表头 + 主表行 + 子表延续行");
        }

        #endregion

        #region ComboBox / 下拉菜单

        [TestMethod]
        public void ComboBox_ValidValue_MapsCorrectly()
        {
            var excelBytes = GenExcel(typeof(ImportSchoolTemplateVM).Name,
                new[]{"学校编码","学校名称","备注","学校类型"},
                new[]{"111","test","r","私立学校"});
            DoImport(_importVm, excelBytes);
            Assert.IsTrue(_importVm.BatchSaveData(), errMsg(_importVm));
            Assert.AreEqual(SchoolTypeEnum.PRI, NewContext().Set<School>().First().SchoolType);
        }

        [TestMethod]
        public void ComboBox_InvalidValue_ReturnsError()
        {
            var excelBytes = GenExcel(typeof(ImportSchoolTemplateVM).Name,
                new[]{"学校编码","学校名称","备注","学校类型"},
                new[]{"111","test","r","不存在的类型"});
            DoImport(_importVm, excelBytes);

            Assert.IsTrue(_importVm.ErrorListVM.EntityList.Count > 0, "无效下拉值应报错");
            Assert.IsTrue(_importVm.ErrorListVM.EntityList.Any(e => e.Message != null && e.Message.Contains("ValueNotExist")),
                "错误信息应包含 ValueNotExist");
        }

        #endregion

        #region Enum 类型

        [TestMethod]
        public void EnumImport_DisplayName_SavesToDatabase()
        {
            var importEnumVm = new ImportSchoolEnumVM { Wtm = NewWtmContext() };
            importEnumVm.Wtm.ConfigInfo.Connections = _importVm.Wtm.ConfigInfo.Connections;
            var excelBytes = GenExcel(typeof(ImportSchoolEnumTemplateVM).Name,
                new[]{"学校编码","学校名称","备注","学校类型"},
                new[]{"111","test","r","公立学校"});
            DoImport(importEnumVm, excelBytes);

            Assert.IsTrue(importEnumVm.BatchSaveData(), errMsg(importEnumVm));
            Assert.AreEqual(SchoolTypeEnum.PUB, NewContext().Set<School>().First().SchoolType);
        }

        [TestMethod]
        public void EnumImport_InvalidValue_ReturnsError()
        {
            var importEnumVm = new ImportSchoolEnumVM { Wtm = NewWtmContext() };
            importEnumVm.Wtm.ConfigInfo.Connections = _importVm.Wtm.ConfigInfo.Connections;
            var excelBytes = GenExcel(typeof(ImportSchoolEnumTemplateVM).Name,
                new[]{"学校编码","学校名称","备注","学校类型"},
                new[]{"111","test","r","INVALID"});
            DoImport(importEnumVm, excelBytes);

            Assert.IsTrue(importEnumVm.ErrorListVM.EntityList.Count > 0, "无效枚举值应报错");
        }

        #endregion

        #region 主子表导入

        [TestMethod]
        public void SubTable_SingleMasterMultipleChildren_AllSaved()
        {
            var headers = new[]{"学校编码","学校名称","备注","学校类型","专业编码","专业名称","专业类别"};
            var values = new[]
            {
                "111","主学校","r","公立学校","001","专业一","必修",
                "","","","","002","专业二","选修"
            };
            var excelBytes = GenExcel(typeof(ImportSchoolWithMajorsTemplateVM).Name, headers, values);
            DoImport(_importSubVm, excelBytes);

            Assert.AreEqual(0, _importSubVm.ErrorListVM.EntityList.Count,
                $"导入错误: {string.Join("; ", _importSubVm.ErrorListVM.EntityList.Select(e => $"[{e.Index}]{e.Message}"))}");
            Assert.AreEqual(1, _importSubVm.EntityList.Count);
            Assert.AreEqual(2, _importSubVm.EntityList[0].Majors.Count);

            Assert.IsTrue(_importSubVm.BatchSaveData(), errMsg(_importSubVm));

            using var ctx = NewContext();
            var school = ctx.Set<School>().Include(x => x.Majors).First();
            Assert.AreEqual("111", school.SchoolCode);
            Assert.AreEqual(2, school.Majors.Count);
            Assert.IsNotNull(school.Majors.FirstOrDefault(m => m.MajorCode == "001"));
            Assert.IsNotNull(school.Majors.FirstOrDefault(m => m.MajorCode == "002"));
        }

        [TestMethod]
        public void SubTable_MultipleMasters_AllSaved()
        {
            var headers = new[]{"学校编码","学校名称","备注","学校类型","专业编码","专业名称","专业类别"};
            var values = new[]
            {
                "001","学校A","rA","公立学校","011","专业A1","必修",
                "","","","","012","专业A2","选修",
                "002","学校B","rB","私立学校","021","专业B1","必修"
            };
            var excelBytes = GenExcel(typeof(ImportSchoolWithMajorsTemplateVM).Name, headers, values);
            DoImport(_importSubVm, excelBytes);

            Assert.AreEqual(0, _importSubVm.ErrorListVM.EntityList.Count,
                $"导入错误: {string.Join("; ", _importSubVm.ErrorListVM.EntityList.Select(e => $"[{e.Index}]{e.Message}"))}");
            Assert.AreEqual(2, _importSubVm.EntityList.Count);
            Assert.IsTrue(_importSubVm.BatchSaveData(), errMsg(_importSubVm));

            using var ctx = NewContext();
            var schools = ctx.Set<School>().Include(x => x.Majors).OrderBy(x => x.SchoolCode).ToList();
            Assert.AreEqual(2, schools.Count);
            Assert.AreEqual("001", schools[0].SchoolCode);
            Assert.AreEqual(2, schools[0].Majors.Count);
            Assert.AreEqual("002", schools[1].SchoolCode);
            Assert.AreEqual(1, schools[1].Majors.Count);
        }

        #endregion

        #region Bug 复现

        /// <summary>
        /// BaseImportVM.SetTemplateData 第325-328行：空行触发 return 而非 continue，
        /// 导致所有后续行被丢弃。例如：有3行有效数据，中间1行空白 → 只导入前面几行。
        /// </summary>
        [TestMethod]
        public void Import_EmptyRowInMiddle_DiscardsRemainingRows()
        {
            var headers = new[]{"学校编码","学校名称","备注","学校类型"};
            // 行1有效，行2空，行3有效 — 预期3行，Bug：只导入前2行（行3被丢弃）
            var values = new[]
            {
                "111","test1","r1","公立学校",
                "","","","",           // 空行
                "222","test2","r2","私立学校"
            };
            var excelBytes = GenExcel(typeof(ImportSchoolTemplateVM).Name, headers, values);
            DoImport(_importVm, excelBytes);

            // Bug: IsEmptyRowOpenXml 命中后 return（不是 continue），行3被丢弃
            // 修复后：1行空白不终止（需连续10行），所以跳过空行，行2被处理
            Assert.AreEqual(2, _importVm.EntityList.Count);
        }

        /// <summary>
        /// Bug: BaseImportVM.SetEntityData 重复覆盖时用 typeof(T).GetFields() 遍历所有字段，
        /// 非 ExcelPropety 字段（如普通 string）的 as 转换返回 null → excelProp.FieldName NPE。
        /// </summary>
        [TestMethod]
        public void Import_OverwriteWithNonExcelPropetyField_NPE()
        {
            // 模板包含一个非 ExcelPropety 的普通字段
            var templateWithExtra = new ImportSchoolTemplateWithNonExcelPropety { Wtm = NewWtmContext() };
            templateWithExtra.Wtm.ConfigInfo.Connections = _importVm.Wtm.ConfigInfo.Connections;
            var importVm2 = new ImportSchoolVM2 { Wtm = NewWtmContext(), Template = templateWithExtra };
            importVm2.Wtm.ConfigInfo.Connections = _importVm.Wtm.ConfigInfo.Connections;

            // 先插入一条已有记录
            var existing = new School { SchoolCode = "111", SchoolName = "old", Remark = "old", SchoolType = SchoolTypeEnum.PRI };
            importVm2.DC.AddEntity(existing);
            importVm2.DC.SaveChanges();

            // 导入覆盖（相同 SchoolCode）
            var excelBytes = GenExcel(typeof(ImportSchoolTemplateWithNonExcelPropety).Name,
                new[]{"学校编码","学校名称","备注","学校类型"},
                new[]{"111","new学校","newRemark","私立学校"});
            DoImport(importVm2, excelBytes);

            try
            {
                importVm2.BatchSaveData();
                // 如果没有抛异常，可能是 try-catch 吞掉了
            }
            catch (NullReferenceException)
            {
                Assert.Fail("BUG: 非 ExcelPropety 字段导致 NPE（typeof(T).GetFields() 包含 ExtraField）");
            }
        }

        #endregion

        #region 错误路径

        [TestMethod] public void Import_NoFile_ReturnsError()
        {
            _importVm.UploadFileId = null;
            _importVm.SetEntityList();
            Assert.IsTrue(_importVm.ErrorListVM.EntityList.Count > 0);
            Assert.AreEqual(0, _importVm.EntityList.Count);
        }

        [TestMethod] public void Import_WrongCols_ReturnsError()
        {
            var bytes = GenExcel(typeof(ImportSchoolTemplateVM).Name,
                new[]{"学校编码","学校名称","备注","学校类型","多余列"},
                new[]{"111","x","r","公立学校","extra"});
            DoImport(_importVm, bytes);
            Assert.IsTrue(_importVm.ErrorListVM.EntityList.Count > 0, "列数不匹配应报错");
            Assert.AreEqual(0, _importVm.EntityList.Count);
            Assert.AreEqual(0, NewContext().Set<School>().Count());
        }

        [TestMethod] public void Import_WrongTemplateType_ReturnsError()
        {
            var bytes = GenExcel("WrongType", new[]{"学校编码"}, new[]{"111"});
            DoImport(_importVm, bytes);
            Assert.IsTrue(_importVm.ErrorListVM.EntityList.Count > 0, "模板类型不匹配应报错");
        }

        [TestMethod] public void DuplicatedCheck_ReturnsNullByDefault()
        {
            Assert.IsNull(_importVm.SetDuplicatedCheck());
        }

        #endregion

        #region Helpers

        private void DoImport(BaseVM importVm, byte[] excelBytes)
        {
            var file = new FileAttachment
            {
                FileName = "test.xlsx", FileExt = "xlsx",
                FileData = excelBytes, Length = excelBytes.Length,
                UploadTime = DateTime.Now, SaveMode = "database"
            };
            importVm.DC.AddEntity(file);
            importVm.DC.SaveChanges();

            var uploadProp = importVm.GetType().GetProperty("UploadFileId");
            uploadProp!.SetValue(importVm, file.ID.ToString());
            var setEntityList = importVm.GetType().GetMethod("SetEntityList");
            setEntityList!.Invoke(importVm, null);
        }

        private static string errMsg(BaseVM vm)
        {
            var errListProp = vm.GetType().GetProperty("ErrorListVM");
            var errList = errListProp?.GetValue(vm);
            var entityList = errList?.GetType().GetProperty("EntityList")?.GetValue(errList) as System.Collections.IEnumerable;
            if (entityList == null) return "";
            var msgs = entityList.Cast<object>().Select(e =>
            {
                var idx = e.GetType().GetProperty("Index")?.GetValue(e);
                var msg = e.GetType().GetProperty("Message")?.GetValue(e);
                return $"[{idx}]{msg}";
            });
            return $"BatchSaveData 失败: {string.Join("; ", msgs)}";
        }

        private static byte[] GenExcel(string typeName, string[] headers, string[] rowValues)
        {
            using var ms = new MemoryStream();
            using (var doc = SpreadsheetDocument.Create(ms, SpreadsheetDocumentType.Workbook))
            {
                var wbp = doc.AddWorkbookPart();
                wbp.Workbook = new Workbook();
                wbp.Workbook.AppendChild(new Sheets());
                var sp1 = wbp.AddNewPart<WorksheetPart>();
                var sd1 = new SheetData();
                sp1.Worksheet = new Worksheet(sd1);
                var hr = new Row { RowIndex = 1 };
                for (int i = 0; i < headers.Length; i++)
                    hr.AppendChild(new Cell { CellReference = OpenXmlHelper.GetCellReference(i, 0), DataType = CellValues.InlineString, InlineString = new InlineString(new Text(headers[i])) });
                sd1.AppendChild(hr);
                for (int r = 0; r < rowValues.Length; r += headers.Length)
                {
                    var dr = new Row { RowIndex = (uint)(r / headers.Length + 2) };
                    for (int c = 0; c < headers.Length && (r + c) < rowValues.Length; c++)
                        dr.AppendChild(new Cell { CellReference = OpenXmlHelper.GetCellReference(c, r / headers.Length + 1), DataType = CellValues.InlineString, InlineString = new InlineString(new Text(rowValues[r + c])) });
                    sd1.AppendChild(dr);
                }
                wbp.Workbook.GetFirstChild<Sheets>().AppendChild(new Sheet { Id = wbp.GetIdOfPart(sp1), SheetId = 1, Name = "Sheet1" });
                var sp2 = wbp.AddNewPart<WorksheetPart>();
                var sd2 = new SheetData();
                sp2.Worksheet = new Worksheet(sd2);
                var mr = new Row { RowIndex = 1 };
                mr.AppendChild(new Cell { CellReference = "C1", DataType = CellValues.InlineString, InlineString = new InlineString(new Text(typeName)) });
                sd2.AppendChild(mr);
                wbp.Workbook.GetFirstChild<Sheets>().AppendChild(new Sheet { Id = wbp.GetIdOfPart(sp2), SheetId = 2, Name = "Sheet2" });
                wbp.Workbook.Save();
            }
            return ms.ToArray();
        }

        private static string GetCellText(Cell cell, WorkbookPart wbp)
        {
            if (cell.DataType?.Value == CellValues.InlineString && cell.InlineString != null)
                return cell.InlineString.Text?.Text ?? "";
            if (cell.DataType?.Value == CellValues.SharedString && cell.CellValue != null
                && int.TryParse(cell.CellValue.Text, out var idx))
            {
                var sst = wbp.GetPartsOfType<SharedStringTablePart>().FirstOrDefault();
                if (sst != null)
                {
                    var items = sst.SharedStringTable.Elements<SharedStringItem>().ToList();
                    if (idx >= 0 && idx < items.Count) return items[idx].InnerText ?? "";
                }
            }
            return cell.CellValue?.Text ?? cell.InnerText ?? "";
        }

        #endregion
    }

    #region Fixture: 基础学校导入（ComboBox 类型）

    public class ImportSchoolTemplateVM : BaseTemplateVM
    {
        [Display(Name = "学校编码")]
        public ExcelPropety SchoolCode = new ExcelPropety { FieldName = nameof(School.SchoolCode), ColumnName = "学校编码", DataType = ColumnDataType.Text, IsNullAble = false };

        [Display(Name = "学校名称")]
        public ExcelPropety SchoolName = new ExcelPropety { FieldName = nameof(School.SchoolName), ColumnName = "学校名称", DataType = ColumnDataType.Text, IsNullAble = false };

        [Display(Name = "备注")]
        public new ExcelPropety Remark = new ExcelPropety { FieldName = nameof(School.Remark), ColumnName = "备注", DataType = ColumnDataType.Text };

        [Display(Name = "学校类型")]
        public ExcelPropety SchoolType = new ExcelPropety
        {
            FieldName = nameof(School.SchoolType), ColumnName = "学校类型",
            DataType = ColumnDataType.ComboBox,
            ListItems = new List<ComboSelectListItem>
            {
                new ComboSelectListItem { Text = "公立学校", Value = SchoolTypeEnum.PUB.ToString() },
                new ComboSelectListItem { Text = "私立学校", Value = SchoolTypeEnum.PRI.ToString() }
            }
        };
    }

    public class ImportSchoolVM : BaseImportVM<ImportSchoolTemplateVM, School> { }

    #endregion

    #region Fixture: Enum 类型学校导入

    public class ImportSchoolEnumTemplateVM : BaseTemplateVM
    {
        [Display(Name = "学校编码")]
        public ExcelPropety SchoolCode = new ExcelPropety { FieldName = nameof(School.SchoolCode), ColumnName = "学校编码", DataType = ColumnDataType.Text, IsNullAble = false };

        [Display(Name = "学校名称")]
        public ExcelPropety SchoolName = new ExcelPropety { FieldName = nameof(School.SchoolName), ColumnName = "学校名称", DataType = ColumnDataType.Text, IsNullAble = false };

        [Display(Name = "备注")]
        public new ExcelPropety Remark = new ExcelPropety { FieldName = nameof(School.Remark), ColumnName = "备注", DataType = ColumnDataType.Text };

        [Display(Name = "学校类型")]
        public ExcelPropety SchoolType = new ExcelPropety
        {
            FieldName = nameof(School.SchoolType), ColumnName = "学校类型",
            DataType = ColumnDataType.Enum,
            EnumType = typeof(SchoolTypeEnum)
        };
    }

    public class ImportSchoolEnumVM : BaseImportVM<ImportSchoolEnumTemplateVM, School> { }

    #endregion

    #region Fixture: 主子表导入

    public class ImportSchoolWithMajorsTemplateVM : BaseTemplateVM
    {
        [Display(Name = "学校编码")]
        public ExcelPropety SchoolCode = new ExcelPropety { FieldName = nameof(School.SchoolCode), ColumnName = "学校编码", DataType = ColumnDataType.Text, IsNullAble = false };

        [Display(Name = "学校名称")]
        public ExcelPropety SchoolName = new ExcelPropety { FieldName = nameof(School.SchoolName), ColumnName = "学校名称", DataType = ColumnDataType.Text, IsNullAble = false };

        [Display(Name = "备注")]
        public new ExcelPropety Remark = new ExcelPropety { FieldName = nameof(School.Remark), ColumnName = "备注", DataType = ColumnDataType.Text };

        [Display(Name = "学校类型")]
        public ExcelPropety SchoolType = new ExcelPropety
        {
            FieldName = nameof(School.SchoolType), ColumnName = "学校类型",
            DataType = ColumnDataType.ComboBox,
            ListItems = new List<ComboSelectListItem>
            {
                new ComboSelectListItem { Text = "公立学校", Value = SchoolTypeEnum.PUB.ToString() },
                new ComboSelectListItem { Text = "私立学校", Value = SchoolTypeEnum.PRI.ToString() }
            }
        };

        [Display(Name = "专业编码")]
        public ExcelPropety MajorCode = new ExcelPropety { FieldName = nameof(Major.MajorCode), ColumnName = "专业编码", DataType = ColumnDataType.Text, SubTableType = typeof(Major) };

        [Display(Name = "专业名称")]
        public ExcelPropety MajorName = new ExcelPropety { FieldName = nameof(Major.MajorName), ColumnName = "专业名称", DataType = ColumnDataType.Text, SubTableType = typeof(Major) };

        [Display(Name = "专业类别")]
        public ExcelPropety MajorType = new ExcelPropety
        {
            FieldName = nameof(Major.MajorType), ColumnName = "专业类别",
            DataType = ColumnDataType.ComboBox, SubTableType = typeof(Major),
            ListItems = new List<ComboSelectListItem>
            {
                new ComboSelectListItem { Text = "必修", Value = MajorTypeEnum.Required.ToString() },
                new ComboSelectListItem { Text = "选修", Value = MajorTypeEnum.Optional.ToString() }
            }
        };
    }

    public class ImportSchoolWithMajorsVM : BaseImportVM<ImportSchoolWithMajorsTemplateVM, School> { }

    /// <summary>
    /// 主子表模板 + 已有数据下载
    /// </summary>
    public class ImportSchoolWithMajorsTemplateWithDataVM : ImportSchoolWithMajorsTemplateVM
    {
        public override void SetTemplateDataValus()
        {
            var schools = Wtm?.DC?.Set<School>().Include(x => x.Majors).ToList();
            if (schools == null) return;

            foreach (var s in schools)
            {
                if (s.Majors != null && s.Majors.Count > 0)
                {
                    foreach (var m in s.Majors)
                    {
                        var row = TemplateDataTable.NewRow();
                        row[nameof(SchoolCode)] = s.SchoolCode;
                        row[nameof(SchoolName)] = s.SchoolName;
                        row[nameof(Remark)] = s.Remark;
                        row[nameof(SchoolType)] = s.SchoolType == SchoolTypeEnum.PUB ? "公立学校" : "私立学校";
                        row[nameof(MajorCode)] = m.MajorCode;
                        row[nameof(MajorName)] = m.MajorName;
                        row[nameof(MajorType)] = m.MajorType == MajorTypeEnum.Required ? "必修" : "选修";
                        TemplateDataTable.Rows.Add(row);
                    }
                }
                else
                {
                    var row = TemplateDataTable.NewRow();
                    row[nameof(SchoolCode)] = s.SchoolCode;
                    row[nameof(SchoolName)] = s.SchoolName;
                    row[nameof(Remark)] = s.Remark;
                    row[nameof(SchoolType)] = s.SchoolType == SchoolTypeEnum.PUB ? "公立学校" : "私立学校";
                    TemplateDataTable.Rows.Add(row);
                }
            }
        }
    }

    #endregion

    #region Fixture: 带数据库数据的模板下载

    public class ImportSchoolTemplateWithDataVM : ImportSchoolTemplateVM
    {
        public override void SetTemplateDataValus()
        {
            var schools = Wtm?.DC?.Set<School>().ToList();
            if (schools == null || schools.Count == 0) return;

            foreach (var s in schools)
            {
                var row = TemplateDataTable.NewRow();
                row[nameof(SchoolCode)] = s.SchoolCode;
                row[nameof(SchoolName)] = s.SchoolName;
                row[nameof(Remark)] = s.Remark;
                row[nameof(SchoolType)] = s.SchoolType == SchoolTypeEnum.PUB ? "公立学校" : "私立学校";
                TemplateDataTable.Rows.Add(row);
            }
        }
    }

    #region Fixture: Bug 复现 — 带非 ExcelPropety 字段的模板

    public class ImportSchoolTemplateWithNonExcelPropety : ImportSchoolTemplateVM
    {
        public string ExtraField = "not_an_excel_propety";
    }

    public class ImportSchoolVM2 : BaseImportVM<ImportSchoolTemplateWithNonExcelPropety, School> { }

    #endregion

    #endregion
}
