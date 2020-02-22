import config from "@/config/index";
const reqPath = config.headerApi + "/_framework";

// github info
const getGithubInfo = {
  url: reqPath + "/GetGithubInfo",
  method: "get"
};
export default { getGithubInfo };
