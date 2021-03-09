import { Message } from "element-ui";
import "reflect-metadata";
import { rules } from "./rules";
function validator(propertyKey, target, args) {
    // 遍历参数
    for (var index in args) {
        for (var _i = 0, rules_1 = rules; _i < rules_1.length; _i++) {
            var item = rules_1[_i];
            var data = Reflect.getMetadata(propertyKey, target, String(item.type) + index);
            if (!data) {
                continue;
            }
            if (args.hasOwnProperty(data.index)) {
                var value = args[data.index];
                if (!item.checkValue(value)) {
                    throw new Error(data.label + item.message);
                }
                break;
            }
        }
    }
}
export function validate() {
    return function (target, propertyKey, descriptor) {
        var fun = descriptor.value;
        descriptor.value = function () {
            try {
                validator(propertyKey, target, arguments);
                fun.apply(this, arguments);
            }
            catch (error) {
                Message.warning(error.message);
            }
        };
    };
}
export function defineMetadata(metaName, metadata, propertyKey, target) {
    Reflect.defineMetadata(propertyKey, metadata, target, String(metaName) + metadata.index);
}
//# sourceMappingURL=validate.js.map