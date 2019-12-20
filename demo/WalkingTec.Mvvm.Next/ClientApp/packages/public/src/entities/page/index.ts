import { BindAll } from 'lodash-decorators';
import EntitiesBehavior, { IPageBehaviorOptions } from './behavior';
/**
 * 用户状态
 * @export
 * @class EntitiesUserStore
 * @extends {EntitiesBehavior}
 */
@BindAll()
export class EntitiesPageStore extends EntitiesBehavior {
    constructor(options: IPageBehaviorOptions) {
        super(options);
    }
}
// export default new EntitiesUserStore();