export default (function (serverUrl) {
    if (serverUrl === void 0) { serverUrl = "@/store/system/frameworkuser"; }
    import(serverUrl)
        .then(function (res) { })
        .catch(function (err) {
        console.log("err", err);
    });
    return;
});
//# sourceMappingURL=import-stort.js.map