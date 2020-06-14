using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WalkingTec.Mvvm.Core.Test.VM
{
    [TestClass]
    public class BaseCRUDVMTestWithOtherID
    {
        private BaseCRUDVM<SchoolWithOtherID> _schoolvm = new BaseCRUDVM<SchoolWithOtherID>();
        private BaseCRUDVM<MajorWithOtherID> _majorvm = new BaseCRUDVM<MajorWithOtherID>();
        private BaseCRUDVM<Student> _studentvm = new BaseCRUDVM<Student>();
        private string _seed;

        public BaseCRUDVMTestWithOtherID()
        {
            _seed = Guid.NewGuid().ToString();
            _schoolvm.DC = new DataContext(_seed, DBTypeEnum.Memory);
            _majorvm.DC = new DataContext(_seed, DBTypeEnum.Memory);
            _studentvm.DC = new DataContext(_seed, DBTypeEnum.Memory);

            _schoolvm.Session = new MockSession();
            _majorvm.Session = new MockSession();
            _studentvm.Session = new MockSession();

            _schoolvm.MSD = new MockMSD();
            _majorvm.MSD = new MockMSD();
            _studentvm.MSD = new MockMSD();
            
            _schoolvm.LoginUserInfo = new LoginUserInfo { ITCode = "schooluser" };
            _majorvm.LoginUserInfo = new LoginUserInfo { ITCode = "majoruser" };
            _studentvm.LoginUserInfo = new LoginUserInfo { ITCode = "studentuser" };

            Mock<IServiceProvider> mockService = new Mock<IServiceProvider>();
            mockService.Setup(x => x.GetService(typeof(GlobalData))).Returns(new GlobalData());
            mockService.Setup(x => x.GetService(typeof(Configs))).Returns(new Configs());
            GlobalServices.SetServiceProvider(mockService.Object);
        }

        [TestMethod]
        [Description("单表添加")]
        [DataTestMethod]
        [DataRow("111", "test1", SchoolTypeEnum.PRI, "remark1")]
        [DataRow("222","test2", SchoolTypeEnum.PUB,"remark2")]
        public void SingleTableDoAdd(string code, string name, SchoolTypeEnum schooltype, string remark)
        {
            SchoolWithOtherID s = new SchoolWithOtherID();
            s.SchoolCode = code;
            s.SchoolName = name;
            s.SchoolType = schooltype;
            s.Remark = remark;
            _schoolvm.Entity = s;
            _schoolvm.DoAdd();

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                var rv = context.Set<SchoolWithOtherID>().ToList();
                Assert.AreEqual(1, rv.Count());
                Assert.AreEqual(code, rv[0].SchoolCode);
                Assert.AreEqual(name, rv[0].SchoolName);
                Assert.AreEqual(schooltype, rv[0].SchoolType);
                Assert.AreEqual(remark, rv[0].Remark);
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
            SchoolWithOtherID s = new SchoolWithOtherID();
            s.SchoolCode = "000";
            s.SchoolName = "default";
            s.SchoolType = null;
            s.Remark = "default";
            _schoolvm.Entity = s;
            _schoolvm.DoAdd();

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                SchoolWithOtherID s2 = new SchoolWithOtherID();
                s2.SchoolCode = code;
                s2.SchoolName = name;
                s2.SchoolType = schooltype;
                s2.Remark = remark;
                s2.ID = s.ID;
                _schoolvm.DC = context;
                _schoolvm.Entity = s2;
                _schoolvm.DoEdit(true);
            }

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                var rv = context.Set<SchoolWithOtherID>().ToList();
                Assert.AreEqual(1, rv.Count());
                Assert.AreEqual(code, rv[0].SchoolCode);
                Assert.AreEqual(name, rv[0].SchoolName);
                Assert.AreEqual(schooltype, rv[0].SchoolType);
                Assert.AreEqual(remark, rv[0].Remark);
            }
        }


        [TestMethod]
        [Description("单表修改指定字段")]
        [DataTestMethod]
        [DataRow("111", "test1", SchoolTypeEnum.PRI, "remark1")]
        [DataRow("222", "test2", SchoolTypeEnum.PUB, "remark2")]
        public void SingleTableDoEditFields(string code, string name, SchoolTypeEnum schooltype, string remark)
        {
            SchoolWithOtherID s = new SchoolWithOtherID();
            s.SchoolCode = "000";
            s.SchoolName = "default";
            s.SchoolType = null;
            s.Remark = "default";
            _schoolvm.Entity = s;
            _schoolvm.DoAdd();

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                SchoolWithOtherID s2 = new SchoolWithOtherID();
                s2.SchoolCode = code;
                s2.SchoolName = name;
                s2.SchoolType = schooltype;
                s2.Remark = remark;
                s2.ID = s.ID;
                _schoolvm.DC = context;
                _schoolvm.Entity = s2;
                _schoolvm.FC.Add("Entity.SchoolName", name);
                _schoolvm.DoEdit();
            }

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                var rv = context.Set<SchoolWithOtherID>().ToList();
                Assert.AreEqual(1, rv.Count());
                Assert.AreEqual("000", rv[0].SchoolCode);
                Assert.AreEqual(name, rv[0].SchoolName);
                Assert.AreEqual(null, rv[0].SchoolType);
                Assert.AreEqual("default", rv[0].Remark);
            }
        }


        [TestMethod]
        [Description("单表删除")]
        public void SingleTableDelete()
        {
            SchoolWithOtherID s = new SchoolWithOtherID();
            s.SchoolCode = "000";
            s.SchoolName = "default";
            s.SchoolType = null;
            s.Remark = "default";
            _schoolvm.Entity = s;
            _schoolvm.DoAdd();

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                Assert.AreEqual(1, context.Set<SchoolWithOtherID>().Count());
                _schoolvm.DC = context;
                _schoolvm.Entity = new SchoolWithOtherID { ID = s.ID };
                _schoolvm.DoDelete();
            }

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                Assert.AreEqual(0, context.Set<SchoolWithOtherID>().Count());
            }
        }

        [TestMethod]
        [Description("一对多主表删除")]
        public void One2ManyTableDelete()
        {
            One2ManyDoAdd();

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                var id = context.Set<SchoolWithOtherID>().AsNoTracking().First().ID;
                _schoolvm.DC = context;
                _schoolvm.Entity = new SchoolWithOtherID { ID = id };
                _schoolvm.DoDelete();
            }

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                Assert.AreEqual(0, context.Set<SchoolWithOtherID>().Count());
            }
        }

        [TestMethod]
        [Description("多对多主表删除")]
        public void Many2ManyTableDelete()
        {
            Many2ManyDoAdd();

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                var id = context.Set<Student>().AsNoTracking().First().ID;
                _studentvm.DC = context;
                _studentvm.Entity = new Student { ID = id };
                _studentvm.DoRealDelete();
            }

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                Assert.AreEqual(1, context.Set<Student>().Count(), "主表是否正确删除");
                //Assert.AreEqual(1, context.Set<StudentMajor>().Count(), "子表是否正确删除");
            }
        }

        [TestMethod]
        [Description("一对多添加")]
        public void One2ManyDoAdd()
        {
            SchoolWithOtherID s = new SchoolWithOtherID();
            s.SchoolCode = "000";
            s.SchoolName = "school";
            s.SchoolType = SchoolTypeEnum.PRI;
            s.Majors = new List<MajorWithOtherID>();
            s.Majors.Add(new MajorWithOtherID
            {
                ID = "id1",
                MajorCode = "111",
                MajorName = "major1",
                MajorType = MajorTypeEnum.Optional
            });
            s.Majors.Add(new MajorWithOtherID
            {
                ID = "id2",
                MajorCode = "222",
                MajorName = "major2",
                MajorType = MajorTypeEnum.Required
            });
            _schoolvm.Entity = s;
            _schoolvm.DoAdd();

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                Assert.AreEqual(1, context.Set<SchoolWithOtherID>().Count());
                Assert.AreEqual(2, context.Set<MajorWithOtherID>().Count());
                var rv = context.Set<MajorWithOtherID>().ToList();
                Assert.AreEqual("id1", rv[0].ID);
                Assert.AreEqual("111", rv[0].MajorCode);
                Assert.AreEqual("major1", rv[0].MajorName);
                Assert.AreEqual(MajorTypeEnum.Optional, rv[0].MajorType);
                Assert.AreEqual("id2", rv[1].ID);
                Assert.AreEqual("222", rv[1].MajorCode);
                Assert.AreEqual("major2", rv[1].MajorName);
                Assert.AreEqual(MajorTypeEnum.Required, rv[1].MajorType);

                Assert.AreEqual("schooluser", rv[0].CreateBy);
                Assert.IsTrue(DateTime.Now.Subtract(rv[0].CreateTime.Value).Seconds < 10);
                Assert.AreEqual("schooluser", rv[1].CreateBy);
                Assert.IsTrue(DateTime.Now.Subtract(rv[1].CreateTime.Value).Seconds < 10);

            }
        }

        [TestMethod]
        [Description("多对多添加")]
        public void Many2ManyDoAdd()
        {
            MajorWithOtherID m1 = new MajorWithOtherID
            {
                ID = "id1",
                MajorCode = "111",
                MajorName = "major1",
                MajorType = MajorTypeEnum.Optional
            };
            MajorWithOtherID m2 = new MajorWithOtherID
            {
                ID = "id2",
                MajorCode = "222",
                MajorName = "major2",
                MajorType = MajorTypeEnum.Required
            };
            Student s1 = new Student
            {
                LoginName = "s1",
                Password = "aaa",
                Name = "student1"
            };
            Student s2 = new Student
            {
                LoginName = "s2",
                Password = "bbb",
                Name = "student2"
            };
            _majorvm.Entity = m1;
            _majorvm.DoAdd();
            _majorvm.Entity = m2;
            _majorvm.DoAdd();

            s1.StudentMajorWithOtherID = new List<StudentMajorWithOtherID>();
            s1.StudentMajorWithOtherID.Add(new StudentMajorWithOtherID { MajorId = m1.ID });
            s2.StudentMajorWithOtherID = new List<StudentMajorWithOtherID>();
            s2.StudentMajorWithOtherID.Add(new StudentMajorWithOtherID { MajorId = m2.ID });
            _studentvm.Entity = s1;
            _studentvm.DoAdd();
            _studentvm.Entity = s2;
            _studentvm.DoAdd();

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                Assert.AreEqual(2, context.Set<MajorWithOtherID>().Count());
                Assert.AreEqual(2, context.Set<Student>().Count());
                Assert.AreEqual(2, context.Set<StudentMajorWithOtherID>().Count());
                var rv = context.Set<StudentMajorWithOtherID>().ToList();
                Assert.AreEqual(s1.ID, rv[0].StudentId);
                Assert.AreEqual(m1.ID, rv[0].MajorId);
                Assert.AreEqual(s2.ID, rv[1].StudentId);
                Assert.AreEqual(m2.ID, rv[1].MajorId);
            }
        }


        [TestMethod]
        [Description("一对多修改")]
        public void One2ManyDoEdit()
        {
            One2ManyDoAdd();

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                var id = context.Set<SchoolWithOtherID>().Select(x=>x.ID).First();
                var mid = context.Set<MajorWithOtherID>().Select(x => x.ID).First();
                SchoolWithOtherID s = new SchoolWithOtherID { ID = id };
                s.Majors = new List<MajorWithOtherID>();
                s.Majors.Add(new MajorWithOtherID
                {
                    ID = "id3",
                    MajorCode = "333",
                    MajorName = "major3",
                    MajorType = MajorTypeEnum.Optional
                });
                s.Majors.Add(new MajorWithOtherID { ID = mid, MajorCode = "111update" });
                _schoolvm.Entity = s;
                _schoolvm.DC = context;
                _schoolvm.DoEdit(true);
            }

            using(var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                var id = context.Set<SchoolWithOtherID>().Select(x => x.ID).First();
                Assert.AreEqual(1, context.Set<SchoolWithOtherID>().Count());
                Assert.AreEqual(2, context.Set<MajorWithOtherID>().Where(x=>x.SchoolId == id).Count());
                var rv1 = context.Set<MajorWithOtherID>().Where(x=>x.MajorCode == "111update").SingleOrDefault();
                Assert.AreEqual("111update", rv1.MajorCode);
                Assert.AreEqual(null, rv1.MajorName);
                Assert.AreEqual(null, rv1.MajorType);
                var rv2 = context.Set<MajorWithOtherID>().Where(x => x.MajorCode == "333").SingleOrDefault();
                Assert.AreEqual("333", rv2.MajorCode);
                Assert.AreEqual("major3", rv2.MajorName);
                Assert.AreEqual(MajorTypeEnum.Optional, rv2.MajorType);

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
                var id = context.Set<SchoolWithOtherID>().Select(x => x.ID).First();
                var mid = context.Set<MajorWithOtherID>().Select(x => x.ID).First();
                SchoolWithOtherID s = new SchoolWithOtherID { ID = id };
                s.Majors = new List<MajorWithOtherID>();
                s.Majors.Add(new MajorWithOtherID
                {
                    ID = "id3",
                    MajorCode = "333",
                    MajorName = "major3",
                    MajorType = MajorTypeEnum.Optional
                });
                s.Majors.Add(new MajorWithOtherID { ID = mid, MajorCode = "111update" });
                _schoolvm.Entity = s;
                _schoolvm.DC = context;
                _schoolvm.FC = new Dictionary<string, object>();
                _schoolvm.FC.Add("Entity.Majors[0].MajorCode",null);
                _schoolvm.DoEdit();
            }

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                var id = context.Set<SchoolWithOtherID>().Select(x => x.ID).First();
                Assert.AreEqual(1, context.Set<SchoolWithOtherID>().Count());
                Assert.AreEqual(2, context.Set<MajorWithOtherID>().Where(x => x.SchoolId == id).Count());
                var rv1 = context.Set<MajorWithOtherID>().Where(x => x.MajorCode == "111update").SingleOrDefault();
                Assert.AreEqual("111update", rv1.MajorCode);
                Assert.AreEqual("major1", rv1.MajorName);
                Assert.AreEqual( MajorTypeEnum.Optional, rv1.MajorType);
                var rv2 = context.Set<MajorWithOtherID>().Where(x => x.MajorCode == "333").SingleOrDefault();
                Assert.AreEqual("333", rv2.MajorCode);
                Assert.AreEqual("major3", rv2.MajorName);
                Assert.AreEqual(MajorTypeEnum.Optional, rv2.MajorType);

                Assert.AreEqual("schooluser", rv1.UpdateBy);
                Assert.IsTrue(DateTime.Now.Subtract(rv1.UpdateTime.Value).Seconds < 10);
                Assert.AreEqual("schooluser", rv2.CreateBy);
                Assert.IsTrue(DateTime.Now.Subtract(rv2.CreateTime.Value).Seconds < 10);


            }
        }


        [TestMethod]
        [Description("多对多修改")]
        public void Many2ManyDoEdit()
        {
            MajorWithOtherID m1 = new MajorWithOtherID
            {
                ID = "id1",
                MajorCode = "111",
                MajorName = "major1",
                MajorType = MajorTypeEnum.Optional
            };
            MajorWithOtherID m2 = new MajorWithOtherID
            {
                ID = "id2",
                MajorCode = "222",
                MajorName = "major2",
                MajorType = MajorTypeEnum.Required
            };
            MajorWithOtherID m3 = new MajorWithOtherID
            {
                ID = "id3",
                MajorCode = "333",
                MajorName = "major3",
                MajorType = MajorTypeEnum.Required
            };
            Student s1 = new Student
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

            s1.StudentMajorWithOtherID = new List<StudentMajorWithOtherID>();
            s1.StudentMajorWithOtherID.Add(new StudentMajorWithOtherID { MajorId = m1.ID });
            s1.StudentMajorWithOtherID.Add(new StudentMajorWithOtherID { MajorId = m2.ID });
            _studentvm.Entity = s1;
            _studentvm.DoAdd();

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                s1.StudentMajorWithOtherID.RemoveAt(0);
                s1.StudentMajorWithOtherID.Add(new StudentMajorWithOtherID { MajorId = m3.ID });
                _studentvm.DC = context;
                _studentvm.Entity = s1;
                _studentvm.DoEdit();
            }

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                Assert.AreEqual(3, context.Set<MajorWithOtherID>().Count());
                Assert.AreEqual(1, context.Set<Student>().Count());
                Assert.AreEqual(2, context.Set<StudentMajorWithOtherID>().Count());
                var rv1 = context.Set<StudentMajorWithOtherID>().Where(x => x.MajorId == m2.ID).SingleOrDefault();
                var rv2 = context.Set<StudentMajorWithOtherID>().Where(x => x.MajorId == m3.ID).SingleOrDefault();
                Assert.AreEqual(s1.ID, rv1.StudentId);
                Assert.AreEqual(m2.ID, rv1.MajorId);
                Assert.AreEqual(s1.ID, rv2.StudentId);
                Assert.AreEqual(m3.ID, rv2.MajorId);
            }
        }


        [TestMethod]
        [Description("多对多修改清除关联")]
        public void Many2ManyDoEditClearRelation()
        {
            MajorWithOtherID m1 = new MajorWithOtherID
            {
                ID = "id1",
                MajorCode = "111",
                MajorName = "major1",
                MajorType = MajorTypeEnum.Optional
            };
            MajorWithOtherID m2 = new MajorWithOtherID
            {
                ID = "id2",
                MajorCode = "222",
                MajorName = "major2",
                MajorType = MajorTypeEnum.Required
            };
            MajorWithOtherID m3 = new MajorWithOtherID
            {
                ID = "id3",
                MajorCode = "333",
                MajorName = "major3",
                MajorType = MajorTypeEnum.Required
            };
            Student s1 = new Student
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

            s1.StudentMajorWithOtherID = new List<StudentMajorWithOtherID>();
            s1.StudentMajorWithOtherID.Add(new StudentMajorWithOtherID { MajorId = m1.ID });
            s1.StudentMajorWithOtherID.Add(new StudentMajorWithOtherID { MajorId = m2.ID });
            s1.StudentMajorWithOtherID.Add(new StudentMajorWithOtherID { MajorId = m3.ID });
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
                Assert.AreEqual(3, context.Set<StudentMajorWithOtherID>().Count());
            }

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                s1.StudentMajorWithOtherID = new List<StudentMajorWithOtherID>();
                _studentvm.DC = context;
                _studentvm.Entity = s1;
                _studentvm.FC = new Dictionary<string, object>();
                _studentvm.FC.Add("Entity.StudentMajorWithOtherID", null);
                _studentvm.DoEdit();
            }
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                Assert.AreEqual(0, context.Set<StudentMajorWithOtherID>().Count());
            }
        }


        [TestMethod]
        [Description("根据ID读取")]
        public void GetTest()
        {
            One2ManyDoAdd();
            _majorvm.SetInclude(x => x.School);
            string id;
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                id = context.Set<MajorWithOtherID>().Select(x => x.ID).FirstOrDefault();
            }
            _majorvm.SetEntityById(id);
            Assert.IsTrue(_majorvm.Entity.School.ID != 0);
        }

        [TestMethod]
        [Description("设置单一不可重复字段")]
        public void DuplicateTest()
        {

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                context.Set<MajorWithOtherID>().Add(new MajorWithOtherID { ID="id1", MajorCode = "111", MajorName = "222", MajorType = MajorTypeEnum.Optional });
                context.SaveChanges();
            }

            _majorvm = new MajorVM1WithOtherID();
            _majorvm.DC = new DataContext(_seed, DBTypeEnum.Memory);
            _majorvm.MSD = new MockMSD();
            _majorvm.Entity = new MajorWithOtherID {ID="id2", MajorCode = "111", MajorName = "not222", MajorType = MajorTypeEnum.Required };
            _majorvm.Validate();
            Assert.IsTrue(_majorvm.MSD["Entity.MajorCode"].Count > 0);
        }

        [TestMethod]
        [Description("设置多个不可重复字段")]
        public void DuplicateTest2()
        {
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                context.Set<MajorWithOtherID>().Add(new MajorWithOtherID {ID="id1", MajorCode = "111", MajorName = "222", MajorType = MajorTypeEnum.Optional });
                context.SaveChanges();
            }

            _majorvm = new MajorVM2WithOtherID();
            _majorvm.DC = new DataContext(_seed, DBTypeEnum.Memory);
            _majorvm.MSD = new MockMSD();
            _majorvm.Entity = new MajorWithOtherID {ID="id2", MajorCode = "111", MajorName = "222", MajorType = MajorTypeEnum.Required };
            _majorvm.Validate();
            Assert.IsTrue(_majorvm.MSD["Entity.MajorCode"].Count > 0);
            Assert.IsTrue(_majorvm.MSD["Entity.MajorName"].Count > 0);
        }


        class MajorVM1 : BaseCRUDVM<Major>
        {
            public override DuplicatedInfo<Major> SetDuplicatedCheck()
            {
                return CreateFieldsInfo(SimpleField(x => x.MajorCode));
            }
        }
        class MajorVM2 : BaseCRUDVM<Major>
        {
            public override DuplicatedInfo<Major> SetDuplicatedCheck()
            {
                return CreateFieldsInfo(SimpleField(x => x.MajorCode)).AddGroup(SimpleField(x=>x.MajorName));
            }
        }
        class MajorVM3 : BaseCRUDVM<OptMajor>
        {
            public override DuplicatedInfo<OptMajor> SetDuplicatedCheck()
            {
                return CreateFieldsInfo(SimpleField(x => x.MajorCode),SimpleField(x => x.SchoolId));
            }
        }


        class MajorVM1WithOtherID : BaseCRUDVM<MajorWithOtherID>
        {
            public override DuplicatedInfo<MajorWithOtherID> SetDuplicatedCheck()
            {
                return CreateFieldsInfo(SimpleField(x => x.MajorCode));
            }
        }
        class MajorVM2WithOtherID : BaseCRUDVM<MajorWithOtherID>
        {
            public override DuplicatedInfo<MajorWithOtherID> SetDuplicatedCheck()
            {
                return CreateFieldsInfo(SimpleField(x => x.MajorCode)).AddGroup(SimpleField(x => x.MajorName));
            }
        }

    }
}
