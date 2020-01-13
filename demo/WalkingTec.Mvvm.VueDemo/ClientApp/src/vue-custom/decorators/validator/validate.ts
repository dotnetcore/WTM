import { Message } from "element-ui";
import "reflect-metadata";
import { rules } from "./rules";

interface CustomMetaData {
    index: number; // 参数索引
    label: string; // 修饰参数关联的名字 如 name =》 用户名
}

function validator(propertyKey: string, target: any, args: any) {
    // 遍历参数
    for (let index in args) {
        for (let item of rules) {
            let data: CustomMetaData = Reflect.getMetadata(
                propertyKey,
                target,
                String(item.type) + index
            );
            if (!data) {
                continue;
            }
            if (args.hasOwnProperty(data.index)) {
                const value = args[data.index];
                if (!(item.checkValue as any)(value)) {
                    throw new Error(data.label + item.message);
                }
                break;
            }
        }
    }
}

export function validate() {
    return function(
        target: any,
        propertyKey: string,
        descriptor: PropertyDescriptor
    ) {
        const fun = descriptor.value;
        descriptor.value = function() {
            try {
                validator(propertyKey, target, arguments);
                fun.apply(this, arguments);
            } catch (error) {
                Message.warning(error.message);
            }
        };
    };
}

export function defineMetadata(
    metaName: symbol,
    metadata: CustomMetaData,
    propertyKey: string,
    target: any
) {
    Reflect.defineMetadata(
        propertyKey,
        metadata,
        target,
        String(metaName) + metadata.index
    );
}
