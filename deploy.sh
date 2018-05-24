# 问题1
#   Failed to load ▒▒, error: libunwind.so.8: cannot open shared object file: No such file or directory
#   Failed to bind to CoreCLR at '/websites/walkingtec.mvvm.mvc.admin/libcoreclr.so'
# yum install libunwind
# 问题2
# FailFast: Couldn't find a valid ICU package installed on the system. Set the configuration flag System.Globalization.Invariant to true  if you want to run with no globalization support.
#
#   at System.Environment.FailFast(System.String)
#   at System.Globalization.GlobalizationMode.GetGlobalizationInvariantMode()
#   at System.Globalization.GlobalizationMode..cctor()
#   at System.Globalization.CultureData.CreateCultureWithInvariantData()
#   at System.Globalization.CultureData.get_Invariant()
#   at System.Globalization.CultureData.GetCultureData(System.String, Boolean)
#   at System.Globalization.CultureInfo.InitializeFromName(System.String, Boolean)
#   at System.Globalization.CultureInfo.Init()
#   at System.Globalization.CultureInfo..cctor()
#   at System.StringComparer..cctor()
#   at System.AppDomainSetup.SetCompatibilitySwitches(System.Collections.Generic.IEnumerable`1<System.String>)
#   at System.AppDomain.PrepareDataForSetup(System.String, System.AppDomainSetup, System.Security.Policy.Evidence, System.Security.Polic y.Evidence, IntPtr, System.String, System.String[], System.String[])
# Aborted
# yum install icu

# clean
rm -rf ./.Publish
rm -rf walkingtec.mvvm.mvc.admin.tar.gz

# 编译并发布
# dotnet publish "./WalkingTec.Mvvm.Mvc.Admin/WalkingTec.Mvvm.Mvc.Admin.csproj" -c Release -r centos.7-x64 -f netcoreapp2.0 -o "../.Publish"
"C:/Program Files/dotnet/dotnet.exe" publish "./WalkingTec.Mvvm.Mvc.Admin/WalkingTec.Mvvm.Mvc.Admin.csproj" -c Release -r centos.7-x64 -f netcoreapp2.0 -o "../.Publish"

# 打包
cd .Publish/ && tar -zcf ../walkingtec.mvvm.mvc.admin.tar.gz *
cd ../

# 创建指定目录
ssh root@192.168.2.103 "mkdir -p /websites/walkingtec.mvvm.mvc.admin"
# 停止站点
# TODO

# 发布
# pscp -i ./id_rsa.ppk -r ./.Publish/* root@192.168.2.103:/websites/walkingtec.mvvm.mvc.admin
# scp -r ./.Publish/* root@192.168.2.103:/websites/walkingtec.mvvm.mvc.admin
scp -r ./walkingtec.mvvm.mvc.admin.tar.gz root@192.168.2.103:/websites/walkingtec.mvvm.mvc.admin.tar.gz

# 解压
ssh root@192.168.2.103 "tar -xzf /websites/walkingtec.mvvm.mvc.admin.tar.gz -C /websites/walkingtec.mvvm.mvc.admin"

# 授予执行权限
ssh root@192.168.2.103 "chmod 777 /websites/walkingtec.mvvm.mvc.admin/WalkingTec.Mvvm.Mvc.Admin"

# 启动站点
ssh root@192.168.2.103 "kill -9 $(pidof WalkingTec.Mvvm.Mvc.Admin)"
ssh root@192.168.2.103 "/websites/walkingtec.mvvm.mvc.admin/WalkingTec.Mvvm.Mvc.Admin"

# clean
rm -rf ./.Publish
rm -rf walkingtec.mvvm.mvc.admin.tar.gz


