/**
 * @author 冷 (https://github.com/LengYXin)
 * @email lengyingxin8966@gmail.com
 * @create date 2021-04-02 11:49:08
 * @modify date 2021-04-02 11:49:08
 * @desc 用户管理
 */
import lodash from 'lodash';
import { BindAll } from 'lodash-decorators';
import { UserController } from './user';
import { FilesController } from './files';
@BindAll()
export class SystemController {
    /** SystemController 唯一标识  */
    static Symbol = Symbol('SystemController');
    /**
     * 用户控制器
     * @memberof SystemController
     */
    UserController = new UserController();
    /**
     * 文件管理控制器
     * @memberof SystemController
     */
    FilesController = new FilesController();
    /**
     * 初始化
     */
    async onInit() {
        this.FilesController.onInit()
        await this.UserController.onInit()
    }
}
export const $System = new SystemController()