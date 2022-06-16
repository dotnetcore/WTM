using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WalkingTec.Mvvm.Core.Implement;
using WalkingTec.Mvvm.Mvc;
using WalkingTec.Mvvm.Test.Mock;

namespace WalkingTec.Mvvm.Core.Test.VM
{
    [TestClass]
    public class BaseCRUDVMTestNoFk
    {
        private BaseCRUDVM<SchoolNoFK> _schoolvm = new BaseCRUDVM<SchoolNoFK>();
        private BaseCRUDVM<MajorNoFK> _majorvm = new BaseCRUDVM<MajorNoFK>();
        private BaseCRUDVM<StudentNoFK> _studentvm = new BaseCRUDVM<StudentNoFK>();
        private string _seed;

        public BaseCRUDVMTestNoFk()
        {
            _seed = Guid.NewGuid().ToString();

            _schoolvm.Wtm = MockWtmContext.CreateWtmContext(new DataContext(_seed, DBTypeEnum.Memory), "schooluser");
            _majorvm.Wtm = MockWtmContext.CreateWtmContext(new DataContext(_seed, DBTypeEnum.Memory), "majoruser");
            _studentvm.Wtm = MockWtmContext.CreateWtmContext(new DataContext(_seed, DBTypeEnum.Memory), "studentuser");

        }

        [TestMethod]
        [Description("单表添加")]
        [DataTestMethod]
        [DataRow("111", "test1", SchoolTypeEnum.PRI, "remark1")]
        [DataRow("222","test2", SchoolTypeEnum.PUB,"remark2")]
        public void SingleTableDoAdd(string code, string name, SchoolTypeEnum schooltype, string remark)
        {
            SchoolNoFK s = new SchoolNoFK
            {
                SchoolCode = code,
                SchoolName = name,
                SchoolType = schooltype,
                Remark = remark
            };
            _schoolvm.Entity = s;
            _schoolvm.DoAdd();

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                var rv = context.Set<SchoolNoFK>().ToList();
                Assert.AreEqual(1, rv.Count);
                Assert.AreEqual(code, rv[0].SchoolCode);
                Assert.AreEqual(name, rv[0].SchoolName);
                Assert.AreEqual(schooltype, rv[0].SchoolType);
                Assert.AreEqual(remark, rv[0].Remark);
                Assert.AreEqual("schooluser", rv[0].CreateBy);
                Assert.IsTrue(DateTime.Now.Subtract(rv[0].CreateTime.Value).Seconds < 10);
            }
            Assert.IsTrue(_schoolvm.MSD.Count == 0);
        }

        [TestMethod]
        [Description("单表修改全部字段")]
        [DataTestMethod]
        [DataRow("111", "test1", SchoolTypeEnum.PRI, "remark1")]
        [DataRow("222", "test2", SchoolTypeEnum.PUB, "remark2")]
        public void SingleTableDoEdit(string code, string name, SchoolTypeEnum schooltype, string remark)
        {
            SchoolNoFK s = new SchoolNoFK
            {
                SchoolCode = "000",
                SchoolName = "default",
                SchoolType = null,
                Remark = "default"
            };
            _schoolvm.Entity = s;
            _schoolvm.DoAdd();

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                SchoolNoFK s2 = new SchoolNoFK
                {
                    SchoolCode = code,
                    SchoolName = name,
                    SchoolType = schooltype,
                    Remark = remark,
                    ID = s.ID
                };
                _schoolvm.DC = context;
                _schoolvm.Entity = s2;
                _schoolvm.DoEdit(true);
            }

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                var rv = context.Set<SchoolNoFK>().ToList();
                Assert.AreEqual(1, rv.Count);
                Assert.AreEqual(code, rv[0].SchoolCode);
                Assert.AreEqual(name, rv[0].SchoolName);
                Assert.AreEqual(schooltype, rv[0].SchoolType);
                Assert.AreEqual(remark, rv[0].Remark);
                Assert.AreEqual("schooluser", rv[0].UpdateBy);
                Assert.IsTrue(DateTime.Now.Subtract(rv[0].UpdateTime.Value).Seconds < 10);
            }
        }


        [TestMethod]
        [Description("单表修改指定字段")]
        [DataTestMethod]
        [DataRow("111", "test1", SchoolTypeEnum.PRI, "remark1")]
        [DataRow("222", "test2", SchoolTypeEnum.PUB, "remark2")]
        public void SingleTableDoEditFields(string code, string name, SchoolTypeEnum schooltype, string remark)
        {
            SchoolNoFK s = new SchoolNoFK
            {
                SchoolCode = "000",
                SchoolName = "default",
                SchoolType = null,
                Remark = "default"
            };
            _schoolvm.Entity = s;
            _schoolvm.DoAdd();

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                SchoolNoFK s2 = new SchoolNoFK
                {
                    SchoolCode = code,
                    SchoolName = name,
                    SchoolType = schooltype,
                    Remark = remark,
                    ID = s.ID
                };
                _schoolvm.DC = context;
                _schoolvm.Entity = s2;
                _schoolvm.FC.Add("Entity.SchoolName", name);
                _schoolvm.DoEdit();
            }

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                var rv = context.Set<SchoolNoFK>().ToList();
                Assert.AreEqual(1, rv.Count);
                Assert.AreEqual("000", rv[0].SchoolCode);
                Assert.AreEqual(name, rv[0].SchoolName);
                Assert.AreEqual(null, rv[0].SchoolType);
                Assert.AreEqual("default", rv[0].Remark);
                Assert.AreEqual("schooluser", rv[0].UpdateBy);
                Assert.IsTrue(DateTime.Now.Subtract(rv[0].UpdateTime.Value).Seconds < 10);
            }
        }


        [TestMethod]
        [Description("单表删除")]
        public void SingleTableDelete()
        {
            SchoolNoFK s = new SchoolNoFK
            {
                SchoolCode = "000",
                SchoolName = "default",
                SchoolType = null,
                Remark = "default"
            };
            _schoolvm.Entity = s;
            _schoolvm.DoAdd();

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                Assert.AreEqual(1, context.Set<SchoolNoFK>().Count());
                _schoolvm.DC = context;
                _schoolvm.Entity = new SchoolNoFK { ID = s.ID };
                _schoolvm.DoDelete();
            }

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                Assert.AreEqual(0, context.Set<SchoolNoFK>().Count());
            }
        }

        [TestMethod]
        [Description("Persist单表删除")]
        public void SingleTablePersistDelete()
        {
            StudentNoFK s = new StudentNoFK
            {
                LoginName = "a",
                Password = "b",
                Name = "ab"
            };
            _studentvm.Entity = s;
            _studentvm.DoAdd();

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                Assert.AreEqual(1, context.Set<StudentNoFK>().Count());
                _studentvm.DC = context;
                _studentvm.Entity = context.Set<StudentNoFK>().Where(x=>x.ID == s.ID).FirstOrDefault();
                _studentvm.DoDelete();
            }

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                Assert.AreEqual(1, context.Set<StudentNoFK>().IgnoreQueryFilters().Count());
                var rv = context.Set<StudentNoFK>().IgnoreQueryFilters().ToList()[0];
                Assert.AreEqual(false, rv.IsValid);
                Assert.AreEqual("studentuser", rv.UpdateBy);
                Assert.IsTrue(DateTime.Now.Subtract(rv.UpdateTime.Value).Seconds < 10);
            }

        }

        [TestMethod]
        [Description("一对多主表删除")]
        public void One2ManyTableDelete()
        {
            One2ManyDoAdd();
            _schoolvm.SetInclude(x => x.Majors);
            Guid id;
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                 id = context.Set<SchoolNoFK>().AsNoTracking().First().ID;
            }
            _schoolvm.SetEntityById(id);
            _schoolvm.DoDelete();
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                Assert.AreEqual(0, context.Set<SchoolNoFK>().Count());
            }
        }

        [TestMethod]
        [Description("一对多子表删除")]
        public void One2ManySubDelete()
        {
            One2ManyDoAdd();
            _majorvm.SetInclude(x => x.School);
            Guid id;
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                 id = context.Set<MajorNoFK>().AsNoTracking().First().ID;
            }
            _majorvm.SetEntityById(id);
            _majorvm.DoDelete();

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                Assert.AreEqual(1, context.Set<MajorNoFK>().Count());
            }
        }



        [TestMethod]
        [Description("多对多主表删除")]
        public void Many2ManyTableDelete()
        {
            Many2ManyDoAdd();

            _studentvm.SetInclude(x => x.StudentMajor, x => x.StudentMajor[0].Major);
            Guid id;
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                 id = context.Set<StudentNoFK>().AsNoTracking().First().ID;
            }
            _studentvm.SetEntityById(id);
            _studentvm.DoRealDelete();

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                Assert.AreEqual(1, context.Set<StudentNoFK>().Count(), "主表是否正确删除");
                //Assert.AreEqual(1, context.Set<StudentMajor>().Count(), "子表是否正确删除");
            }
        }

        [TestMethod]
        [Description("Persist单表物理删除")]
        public void SingleTablePersistRealDelete()
        {
            StudentNoFK s = new StudentNoFK
            {
                LoginName = "a",
                Password = "b",
                Name = "ab"
            };
            _studentvm.Entity = s;
            _studentvm.DoAdd();

            _studentvm.SetInclude(x => x.StudentMajor, x => x.StudentMajor[0].Major);
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                Assert.AreEqual(1, context.Set<StudentNoFK>().Count());
            }
            _studentvm.SetEntityById(s.ID);
            _studentvm.DoRealDelete();

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                Assert.AreEqual(0, context.Set<StudentNoFK>().Count());
            }

        }

        [TestMethod]
        [Description("一对多添加")]
        public void One2ManyDoAdd()
        {
            SchoolNoFK s = new SchoolNoFK
            {
                SchoolCode = "000",
                SchoolName = "school",
                SchoolType = SchoolTypeEnum.PRI,
                Majors = new List<MajorNoFK>()
            };
            s.Majors.Add(new MajorNoFK
            {
                MajorCode = "111",
                MajorName = "major1",
                MajorType = MajorTypeEnum.Optional
            });
            s.Majors.Add(new MajorNoFK
            {
                MajorCode = "222",
                MajorName = "major2",
                MajorType = MajorTypeEnum.Required
            });
            _schoolvm.Entity = s;
            _schoolvm.DoAdd();

            using var context = new DataContext(_seed, DBTypeEnum.Memory);
            Assert.AreEqual(1, context.Set<SchoolNoFK>().Count());
            Assert.AreEqual(2, context.Set<MajorNoFK>().Count());
            var rv = context.Set<MajorNoFK>().ToList();
            Assert.AreEqual("111", rv[0].MajorCode);
            Assert.AreEqual("major1", rv[0].MajorName);
            Assert.AreEqual(MajorTypeEnum.Optional, rv[0].MajorType);
            Assert.AreEqual("222", rv[1].MajorCode);
            Assert.AreEqual("major2", rv[1].MajorName);
            Assert.AreEqual(MajorTypeEnum.Required, rv[1].MajorType);

            Assert.AreEqual("schooluser", context.Set<SchoolNoFK>().First().CreateBy);
            Assert.IsTrue(DateTime.Now.Subtract(context.Set<SchoolNoFK>().First().CreateTime.Value).Seconds < 10);
            Assert.AreEqual("schooluser", rv[0].CreateBy);
            Assert.IsTrue(DateTime.Now.Subtract(rv[0].CreateTime.Value).Seconds < 10);
            Assert.AreEqual("schooluser", rv[1].CreateBy);
            Assert.IsTrue(DateTime.Now.Subtract(rv[1].CreateTime.Value).Seconds < 10);
        }


        [TestMethod]
        [Description("多对多添加")]
        public void Many2ManyDoAdd()
        {
            MajorNoFK m1 = new MajorNoFK
            {
                MajorCode = "111",
                MajorName = "major1",
                MajorType = MajorTypeEnum.Optional
            };
            MajorNoFK m2 = new MajorNoFK
            {
                MajorCode = "222",
                MajorName = "major2",
                MajorType = MajorTypeEnum.Required
            };
            StudentNoFK s1 = new StudentNoFK
            {
                LoginName = "s1",
                Password = "aaa",
                Name = "student1"
            };
            StudentNoFK s2 = new StudentNoFK
            {
                LoginName = "s2",
                Password = "bbb",
                Name = "student2"
            };
            _majorvm.Entity = m1;
            _majorvm.DoAdd();
            _majorvm.Entity = m2;
            _majorvm.DoAdd();

            s1.StudentMajor = new List<StudentMajorNoFK>
            {
                new StudentMajorNoFK { MajorId = m1.MajorCode }
            };
            s2.StudentMajor = new List<StudentMajorNoFK>
            {
                new StudentMajorNoFK { MajorId = m2.MajorCode }
            };
            _studentvm.Entity = s1;
            _studentvm.DoAdd();
            _studentvm.Entity = s2;
            _studentvm.DoAdd();

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                Assert.AreEqual(2, context.Set<MajorNoFK>().Count());
                Assert.AreEqual(2, context.Set<StudentNoFK>().Count());
                Assert.AreEqual(2, context.Set<StudentMajorNoFK>().Count());
                var rv = context.Set<StudentMajorNoFK>().ToList();
                Assert.AreEqual(s1.LoginName, rv[0].StudentId);
                Assert.AreEqual(m1.MajorCode, rv[0].MajorId);
                Assert.AreEqual(s2.LoginName, rv[1].StudentId);
                Assert.AreEqual(m2.MajorCode, rv[1].MajorId);
            }
        }


        [TestMethod]
        [Description("一对多修改")]
        public void One2ManyDoEdit()
        {
            One2ManyDoAdd();

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                var old = context.Set<SchoolNoFK>().AsNoTracking().First();
                var mid = context.Set<MajorNoFK>().Select(x => x.ID).First();
                SchoolNoFK s = new SchoolNoFK { ID=old.ID, SchoolCode=old.SchoolCode };
                s.Majors = new List<MajorNoFK>
                {
                    new MajorNoFK
                    {
                        MajorCode = "333",
                        MajorName = "major3",
                        MajorType = MajorTypeEnum.Optional
                    },
                    new MajorNoFK { ID = mid, MajorCode = "111update" }
                };
                _schoolvm.Entity = s;
                _schoolvm.DC = context;
                _schoolvm.DoEdit(true);
            }

            using(var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                var id = context.Set<SchoolNoFK>().Select(x => x.SchoolCode).First();
                Assert.AreEqual(1, context.Set<SchoolNoFK>().Count());
                Assert.AreEqual(2, context.Set<MajorNoFK>().Where(x=>x.SchoolId == id).Count());
                var rv1 = context.Set<MajorNoFK>().Where(x=>x.MajorCode == "111").SingleOrDefault();
                Assert.AreEqual("111", rv1.MajorCode);
                Assert.AreEqual(null, rv1.MajorName);
                Assert.AreEqual(null, rv1.MajorType);
                var rv2 = context.Set<MajorNoFK>().Where(x => x.MajorCode == "333").SingleOrDefault();
                Assert.AreEqual("333", rv2.MajorCode);
                Assert.AreEqual("major3", rv2.MajorName);
                Assert.AreEqual(MajorTypeEnum.Optional, rv2.MajorType);

                Assert.AreEqual("schooluser", context.Set<SchoolNoFK>().First().UpdateBy);
                Assert.IsTrue(DateTime.Now.Subtract(context.Set<SchoolNoFK>().First().UpdateTime.Value).Seconds < 10);
                Assert.AreEqual("schooluser", rv1.UpdateBy);
                Assert.IsTrue(DateTime.Now.Subtract(rv1.UpdateTime.Value).Seconds < 10);
                Assert.AreEqual("schooluser", rv2.CreateBy);
                Assert.IsTrue(DateTime.Now.Subtract(rv2.CreateTime.Value).Seconds < 10);


            }
        }


        [TestMethod]
        [Description("一对多修改指定字段")]
        public void One2ManyDoEditFields()
        {
            One2ManyDoAdd();

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                var old = context.Set<SchoolNoFK>().AsNoTracking().First();
                var mid = context.Set<MajorNoFK>().Select(x => x.ID).First();
                SchoolNoFK s = new SchoolNoFK { ID = old.ID, SchoolCode = old.SchoolCode };
                s.Majors = new List<MajorNoFK>
                {
                    new MajorNoFK
                    {
                        MajorCode = "333",
                        MajorName = "major3",
                        MajorType = MajorTypeEnum.Optional
                    },
                    new MajorNoFK { ID = mid, MajorCode = "111update" }
                };
                _schoolvm.Entity = s;
                _schoolvm.DC = context;
                _schoolvm.FC = new Dictionary<string, object>
                {
                    { "Entity.Majors[0].MajorCode", null }
                };
                _schoolvm.DoEdit();
            }

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                var id = context.Set<SchoolNoFK>().Select(x => x.SchoolCode).First();
                Assert.AreEqual(1, context.Set<SchoolNoFK>().Count());
                Assert.AreEqual(2, context.Set<MajorNoFK>().Where(x => x.SchoolId == id).Count());
                var rv1 = context.Set<MajorNoFK>().Where(x => x.MajorCode == "111").SingleOrDefault();
                Assert.AreEqual("111", rv1.MajorCode);
                Assert.AreEqual("major1", rv1.MajorName);
                Assert.AreEqual( MajorTypeEnum.Optional, rv1.MajorType);
                var rv2 = context.Set<MajorNoFK>().Where(x => x.MajorCode == "333").SingleOrDefault();
                Assert.AreEqual("333", rv2.MajorCode);
                Assert.AreEqual("major3", rv2.MajorName);
                Assert.AreEqual(MajorTypeEnum.Optional, rv2.MajorType);

                Assert.AreEqual("schooluser", context.Set<SchoolNoFK>().First().UpdateBy);
                Assert.IsTrue(DateTime.Now.Subtract(context.Set<SchoolNoFK>().First().UpdateTime.Value).Seconds < 10);
                Assert.AreEqual("schooluser", rv1.UpdateBy);
                Assert.IsTrue(DateTime.Now.Subtract(rv1.UpdateTime.Value).Seconds < 10);
                Assert.AreEqual("schooluser", rv2.CreateBy);
                Assert.IsTrue(DateTime.Now.Subtract(rv2.CreateTime.Value).Seconds < 10);


            }
        }

        [TestMethod]
        [Description("一对多修改子表")]
        public void One2ManyDoEditSub()
        {
            One2ManyDoAdd();

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                var old = context.Set<SchoolNoFK>().AsNoTracking().First();
                var mid = context.Set<MajorNoFK>().Select(x => x.ID).First();

                MajorNoFK m = new MajorNoFK { ID = mid };
                m.MajorCode = "maaa";
                m.MajorName = "mbbb";
                m.School = new SchoolNoFK
                {
                    ID = old.ID,
                    SchoolName = "aaa",
                    SchoolCode = "bbb",
                    Remark = "ccc"
                };

                _majorvm.Entity = m;
                _majorvm.DC = context;
                _majorvm.FC.Add("Entity.MajorCode", null);
                _majorvm.FC.Add("Entity.MajorName", null);

                _schoolvm.Entity = m.School;
                _schoolvm.DC = context;
                _schoolvm.FC.Add("Entity.SchoolCode", null);
                _schoolvm.FC.Add("Entity.SchoolName", null);

                _schoolvm.DoEdit(false);
                _majorvm.DoEdit(false);
            }

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                var id = context.Set<SchoolNoFK>().Select(x => x.SchoolCode).First();
                Assert.AreEqual(1, context.Set<SchoolNoFK>().Count());
                Assert.AreEqual(2, context.Set<MajorNoFK>().Where(x => x.SchoolId == id).Count());
                var rv1 = context.Set<MajorNoFK>().Where(x => x.MajorName == "mbbb").SingleOrDefault();
                Assert.AreEqual("mbbb", rv1.MajorName);
                var rv2 = context.Set<SchoolNoFK>().Where(x => x.SchoolCode == "000").SingleOrDefault();
                Assert.AreEqual("aaa", rv2.SchoolName);

            }
        }




        [TestMethod]
        [Description("多对多修改")]
        public void Many2ManyDoEdit()
        {
            MajorNoFK m1 = new MajorNoFK
            {
                MajorCode = "111",
                MajorName = "major1",
                MajorType = MajorTypeEnum.Optional
            };
            MajorNoFK m2 = new MajorNoFK
            {
                MajorCode = "222",
                MajorName = "major2",
                MajorType = MajorTypeEnum.Required
            };
            MajorNoFK m3 = new MajorNoFK
            {
                MajorCode = "333",
                MajorName = "major3",
                MajorType = MajorTypeEnum.Required
            };
            StudentNoFK s1 = new StudentNoFK
            {
                LoginName = "s1",
                Password = "aaa",
                Name = "student1"
            };
            _majorvm.Entity = m1;
            _majorvm.DoAdd();
            _majorvm.Entity = m2;
            _majorvm.DoAdd();
            _majorvm.Entity = m3;
            _majorvm.DoAdd();

            s1.StudentMajor = new List<StudentMajorNoFK>();
            s1.StudentMajor.Add(new StudentMajorNoFK { MajorId = m1.MajorCode });
            s1.StudentMajor.Add(new StudentMajorNoFK { MajorId = m2.MajorCode });
            _studentvm.Entity = s1;
            _studentvm.DoAdd();

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                s1.StudentMajor.RemoveAt(0);
                s1.StudentMajor.Add(new StudentMajorNoFK { MajorId = m3.MajorCode });
                _studentvm.DC = context;
                _studentvm.Entity = s1;
                _studentvm.DoEdit();
            }

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                Assert.AreEqual(3, context.Set<MajorNoFK>().Count());
                Assert.AreEqual(1, context.Set<StudentNoFK>().Count());
                Assert.AreEqual(2, context.Set<StudentMajorNoFK>().Count());
                var rv1 = context.Set<StudentMajorNoFK>().Where(x => x.MajorId == m2.MajorCode).SingleOrDefault();
                var rv2 = context.Set<StudentMajorNoFK>().Where(x => x.MajorId == m3.MajorCode).SingleOrDefault();
                Assert.AreEqual(s1.LoginName, rv1.StudentId);
                Assert.AreEqual(m2.MajorCode, rv1.MajorId);
                Assert.AreEqual(s1.LoginName, rv2.StudentId);
                Assert.AreEqual(m3.MajorCode, rv2.MajorId);
            }
        }


        [TestMethod]
        [Description("多对多修改清除关联")]
        public void Many2ManyDoEditClearRelation()
        {
            MajorNoFK m1 = new MajorNoFK
            {
                MajorCode = "111",
                MajorName = "major1",
                MajorType = MajorTypeEnum.Optional
            };
            MajorNoFK m2 = new MajorNoFK
            {
                MajorCode = "222",
                MajorName = "major2",
                MajorType = MajorTypeEnum.Required
            };
            MajorNoFK m3 = new MajorNoFK
            {
                MajorCode = "333",
                MajorName = "major3",
                MajorType = MajorTypeEnum.Required
            };
            StudentNoFK s1 = new StudentNoFK
            {
                LoginName = "s1",
                Password = "aaa",
                Name = "student1"
            };
            _majorvm.Entity = m1;
            _majorvm.DoAdd();
            _majorvm.Entity = m2;
            _majorvm.DoAdd();
            _majorvm.Entity = m3;
            _majorvm.DoAdd();

            s1.StudentMajor = new List<StudentMajorNoFK>();
            s1.StudentMajor.Add(new StudentMajorNoFK { MajorId = m1.MajorCode });
            s1.StudentMajor.Add(new StudentMajorNoFK { MajorId = m2.MajorCode });
            s1.StudentMajor.Add(new StudentMajorNoFK { MajorId = m3.MajorCode });
            _studentvm.Entity = s1;
            _studentvm.DoAdd();

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                s1.StudentMajor = null;
                _studentvm.DC = context;
                _studentvm.Entity = s1;
                _studentvm.DoEdit();
            }

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                Assert.AreEqual(3, context.Set<StudentMajorNoFK>().Count());
            }

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                s1.StudentMajor = new List<StudentMajorNoFK>();
                _studentvm.DC = context;
                _studentvm.Entity = s1;
                _studentvm.FC = new Dictionary<string, object>();
                _studentvm.FC.Add("Entity.StudentMajor", null);
                _studentvm.DoEdit();
            }
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                Assert.AreEqual(0, context.Set<StudentMajorNoFK>().Count());
            }
        }


        [TestMethod]
        [Description("根据ID读取")]
        public void GetTest()
        {
            One2ManyDoAdd();
            _majorvm.SetInclude(x => x.School, x => x.StudentMajors[0].Student, x => x.MajorType, x => x.School.SchoolCode);
            Guid id;
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                id = context.Set<MajorNoFK>().Select(x => x.ID).FirstOrDefault();
            }
            _majorvm.SetEntityById(id);
            Assert.IsTrue(_majorvm.Entity.School.ID != Guid.Empty);
        }

        [TestMethod]
        [Description("根据ID读取")]
        public void GetTest2()
        {
            Many2ManyDoAdd();
            _majorvm.SetInclude(x => x.StudentMajors[0].Student);
            Guid id;
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                id = context.Set<MajorNoFK>().Select(x => x.ID).FirstOrDefault();
            }
            _majorvm.SetEntityById(id);
            Assert.IsTrue(_majorvm.Entity.StudentMajors[0].Student.Name != null);
        }


        [TestMethod]
        [Description("设置单一不可重复字段")]
        public void DuplicateTest()
        {

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                context.Set<MajorNoFK>().Add(new MajorNoFK { MajorCode = "111", MajorName = "222", MajorType = MajorTypeEnum.Optional });
                context.SaveChanges();
            }

            _majorvm = new MajorVM1
            {
                Wtm = MockWtmContext.CreateWtmContext(new DataContext(_seed, DBTypeEnum.Memory)),
                Entity = new MajorNoFK { MajorCode = "111", MajorName = "not222", MajorType = MajorTypeEnum.Required }
            };
            _majorvm.Validate();
            Assert.IsTrue(_majorvm.MSD["Entity.MajorCode"].Count > 0);
        }

        [TestMethod]
        [Description("设置多个不可重复字段")]
        public void DuplicateTest2()
        {
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                context.Set<MajorNoFK>().Add(new MajorNoFK { MajorCode = "111", MajorName = "222", MajorType = MajorTypeEnum.Optional });
                context.SaveChanges();
            }

            _majorvm = new MajorVM2
            {
                Wtm = MockWtmContext.CreateWtmContext(new DataContext(_seed, DBTypeEnum.Memory)),
                Entity = new MajorNoFK { MajorCode = "111", MajorName = "222", MajorType = MajorTypeEnum.Required }
            };
            _majorvm.Validate();
            Assert.IsTrue(_majorvm.MSD["Entity.MajorCode"].Count > 0);
            Assert.IsTrue(_majorvm.MSD["Entity.MajorName"].Count > 0);
        }


        class MajorVM1 : BaseCRUDVM<MajorNoFK>
        {
            public override DuplicatedInfo<MajorNoFK> SetDuplicatedCheck()
            {
                return CreateFieldsInfo(SimpleField(x => x.MajorCode));
            }
        }
        class MajorVM2 : BaseCRUDVM<MajorNoFK>
        {
            public override DuplicatedInfo<MajorNoFK> SetDuplicatedCheck()
            {
                return CreateFieldsInfo(SimpleField(x => x.MajorCode)).AddGroup(SimpleField(x=>x.MajorName));
            }
        }
    }
}
