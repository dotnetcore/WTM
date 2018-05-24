## 问题1 libunwind
```bash
#   Failed to load ▒▒, error: libunwind.so.8: cannot open shared object file: No such file or directory
#   Failed to bind to CoreCLR at '/websites/walkingtec.mvvm.mvc.admin/libcoreclr.so'
yum -y install libunwind
```

## 问题2 icu
```bash
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
yum -y install icu
```

## 问题3 libgdiplus
```bash
yum -y install autoconf automake libtool
yum -y install freetype-devel fontconfig libXft-devel
yum -y install libjpeg-turbo-devel libpng-devel giflib-devel libtiff-devel libexif-devel
yum -y install glib2-devel cairo-devel

yum -y install git
mkdir -p /softwares
cd /softwares
git clone https://github.com/mono/libgdiplus
cd libgdiplus
./autogen.sh
make
make install
ln -s /usr/local/lib/libgdiplus.so /usr/lib64/gdiplus.dll

########################type init 错误。提示找不到libgdiplus组件##########
## 方案1
ln -s /usr/local/lib/libgdiplus.so /usr/lib64/libgdiplus.so
ln -s /usr/local/lib/libgdiplus.so /usr/libgdiplus.so
## 方案2
vi /etc/ld.so.conf
##将 /usr/local/lib 加入
# 使配置生效。
ldconfig
######################################################################

######生成出来的图片没有任何文字 DrawString not dislpay in image##########
# 复制 windowns fronts to /usr/share/fonts/chinese/TrueType/
######################################################################
```










```bash
```
