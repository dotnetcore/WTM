import config from "@/config/index";
import { contentType } from "@/config/enum";
/**
 * 接口
 */
// nsp详情
const nspsInfo = {
    url: config.nspApi + "/uds/in/nsp/v1/nsps/{nsp_uuid}?sig=%7Bsig%7D",
    method: "get"
};
// 留资二维码
const qrCodes = {
    url: config.serverHost + "/api/nsp/v1/qr_codes",
    method: "get",
    isBuffer: true
};
// 购车二维码
const qrCodesNoauth = {
    url:
        config.nspApi +
        "/api/v1/plf/iep/web-agg-svr/otd/nioapp/miniProgram/qrCode/noauth",
    method: "post",
    isBuffer: true,
    contentType: contentType.multipart
};
export default {
    nspsInfo,
    qrCodes,
    qrCodesNoauth
};
