import config from "@/config/index";
/**
 * 接口
 */

// 列表
const resourcesList = {
    url: config.serverHost + "/api/nsp/v1/resources",
    method: "get"
};

export default {
    resourcesList
};
