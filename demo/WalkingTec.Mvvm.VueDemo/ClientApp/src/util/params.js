export function getUrlByParamArr(url, paramObj) {
    if (paramObj instanceof Object && Object.keys(paramObj).length) {
        let params = "?";
        for (let key in paramObj) {
            params += `${key}=${paramObj[key]}&`;
        }
        params = params.slice(0, params.length - 2);
        return url + params;
    }
    return url;
}
