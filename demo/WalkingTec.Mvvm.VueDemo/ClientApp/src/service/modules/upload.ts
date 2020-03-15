import config from "@/config/index";
// 上传
const upload = {
    url: config.headerApi + "/_file/upload",
    method: "post"
};

export { upload };
