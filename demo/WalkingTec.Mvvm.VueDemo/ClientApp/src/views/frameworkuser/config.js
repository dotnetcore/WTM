export const SEARCH_DATA = {
  ITCode: "",
  Name: ""
};

export const TABLE_HEADER = [
  { key: "ITCode", sortable: true, label: "账号" },
  { key: "Name", sortable: true, label: "姓名" },
  { key: "Sex", sortable: true, label: "性别" },
  { key: "PhotoId", label: "照片", isSlot: true },
  { key: "IsValid", label: "是否生效", isSlot: true },
  { key: "RoleName_view", label: "角色" },
  { key: "GroupName_view", label: "用户组" },
  { key: "operate", label: "操作", isSlot: true, width: 130, fixed: "right" }
];
