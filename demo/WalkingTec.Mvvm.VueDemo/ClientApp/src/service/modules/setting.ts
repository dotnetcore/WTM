import config from "@/config/index";
import _request from "@/util/service";
const reqPath = config.headerApi;

// 租户
const trameworkTenants = {
  url: reqPath + "/_frameworktenant/GetFrameworkTenants",
  method: "get"
};
// 设置租户
const accountTenants = {
  url: reqPath + "/_account/SetTenant",
  method: "get"
};

const getFrameworkTenants = data => {
  return _request({ ...trameworkTenants, data });
};

const setTenants = data => {
  return _request({ ...accountTenants, data });
};

export default {
  trameworkTenants,
  accountTenants,
  getFrameworkTenants,
  setTenants
};
