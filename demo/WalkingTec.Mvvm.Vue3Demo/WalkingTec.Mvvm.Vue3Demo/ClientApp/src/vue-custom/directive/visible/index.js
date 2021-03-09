import { UserModule } from "@/store/modules/user";
import config from "@/config/index";
/**
 * 按钮 action权限
 * v-visible="{api path}"
 */
var visible = {
    inserted: function (el, _a, vnode) {
        var value = _a.value;
        if (config.development) {
            return;
        }
        var actions = UserModule.actionList.map(function (item) { return (!!item ? item.toUpperCase() : item); });
        if (_.isArray(value)) {
            var fslist = _.filter(value, function (item) { return !!item && actions.includes(item.toUpperCase()); });
            if (fslist.length < 1) {
                el.parentNode && el.parentNode.removeChild(el);
            }
        }
        else {
            if (value && !actions.includes(value.toUpperCase())) {
                el.parentNode && el.parentNode.removeChild(el);
            }
        }
    },
};
export default visible;
//# sourceMappingURL=index.js.map