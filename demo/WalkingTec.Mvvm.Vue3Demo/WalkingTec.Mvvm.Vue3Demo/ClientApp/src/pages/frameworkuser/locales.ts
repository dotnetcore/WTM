/**
 * 字段描述 枚举
 */
export enum EnumLocaleLabel {
    /** 用户ITCode */
    ITCode = "frameworkuser.ITCode",
    /** 密码 */
    Password = "frameworkuser.Password",
    /** 邮箱 */
    Email = "frameworkuser.Email",
    /** 用户名称 */
    Name = "frameworkuser.Name",
    /** 用户性别 */
    Sex = "frameworkuser.Sex",
    Sex_Male = "frameworkuser.Sex_Male",
    Sex_Female = "frameworkuser.Sex_Female",
    /** 用户头像 */
    Photo = "frameworkuser.PhotoId",
    /** 是否有效 */
    IsValid = "frameworkuser.IsValid",
    /** 角色 */
    RoleName = "frameworkuser.RoleName_view",
    /** 用户组 */
    GroupName = "frameworkuser.GroupName_view",
    /** 手机号 */
    CellPhone = "frameworkuser.CellPhone",
    /** 座机 */
    HomePhone = "frameworkuser.HomePhone",
    /** 住址 */
    Address = "frameworkuser.Address",
    /** 邮编 */
    ZipCode = "frameworkuser.ZipCode"
}
export default {
    en: {
        [EnumLocaleLabel.ITCode]: 'Account',
        [EnumLocaleLabel.Password]: 'Password',
        [EnumLocaleLabel.Email]: 'Email',
        [EnumLocaleLabel.Name]: 'Name',
        [EnumLocaleLabel.Sex]: 'Gender',
        [EnumLocaleLabel.Sex_Male]: 'Male',
        [EnumLocaleLabel.Sex_Female]: 'Female',
        [EnumLocaleLabel.Photo]: 'Photo',
        [EnumLocaleLabel.IsValid]: 'IsValid',
        [EnumLocaleLabel.RoleName]: 'RoleName',
        [EnumLocaleLabel.GroupName]: 'GroupName',
        [EnumLocaleLabel.CellPhone]: 'CellPhone',
        [EnumLocaleLabel.HomePhone]: 'HomePhone',
        [EnumLocaleLabel.Address]: 'Address',
        [EnumLocaleLabel.ZipCode]: 'ZipCode',
    },
    zh: {
        [EnumLocaleLabel.ITCode]: '账号',
        [EnumLocaleLabel.Password]: '密码',
        [EnumLocaleLabel.Email]: '邮箱',
        [EnumLocaleLabel.Name]: '姓名',
        [EnumLocaleLabel.Sex]: '性别',
        [EnumLocaleLabel.Sex_Male]: '男',
        [EnumLocaleLabel.Sex_Female]: '女',
        [EnumLocaleLabel.Photo]: '照片',
        [EnumLocaleLabel.IsValid]: '是否有效',
        [EnumLocaleLabel.RoleName]: '角色',
        [EnumLocaleLabel.GroupName]: '用户组',
        [EnumLocaleLabel.CellPhone]: '手机号',
        [EnumLocaleLabel.HomePhone]: '座机',
        [EnumLocaleLabel.Address]: '住址',
        [EnumLocaleLabel.ZipCode]: '邮编',
    }
}