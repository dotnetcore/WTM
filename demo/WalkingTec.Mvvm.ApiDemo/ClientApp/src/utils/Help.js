/**
 * @author 冷 (https://github.com/LengYXin)
 * @email lengyingxin8966@gmail.com
 * @create date 2018-09-12 18:53:01
 * @modify date 2018-09-12 18:53:01
 * @desc [description]
*/
var Help = /** @class */ (function () {
    function Help() {
    }
    Help.GUID = function () {
        return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
            var r = Math.random() * 16 | 0, v = c == 'x' ? r : (r & 0x3 | 0x8);
            return v.toString(16);
        });
    };
    return Help;
}());
export { Help };
//# sourceMappingURL=Help.js.map