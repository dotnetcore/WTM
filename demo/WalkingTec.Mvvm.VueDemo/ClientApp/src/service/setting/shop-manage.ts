import config from "@/config/index";
/**
 * 接口
 */
// nsp详情
const nspsInfo = {
    url: config.nspApi + "/uds/in/nsp/v1/nsps/{nsp_uuid}?sig=%7Bsig%7D",
    method: "get"
};
// 编辑nsp
const nspsEdit = {
    url: config.nspApi + "/uds/in/nsp/v1/nsps/{nsp_uuid}",
    method: "post"
};
export default {
    nspsInfo,
    nspsEdit
};
