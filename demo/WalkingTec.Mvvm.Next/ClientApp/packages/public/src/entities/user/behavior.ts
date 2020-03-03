import { action, runInAction, toJS } from 'mobx';
import { AjaxRequest } from "rxjs/ajax";
import { timer } from 'rxjs';
import Entities, { MenuDataItem } from './entities';
import Ajax, { Request } from '../../utils/request';
import lodash from 'lodash';
import { filter } from 'rxjs/operators';
// import { MenuDataItem } from '@ant-design/pro-layout';

/**
 * 对象 动作 行为 
 * @export
 * @class EntitiesUserBehavior
 * @extends {Entities}
 */
export default class EntitiesUserBehavior extends Entities {
    // Request = new Request();
   
    /**
     * 解析登录信息
     * @protected
     * @memberof EntitiesUserBehavior
     */
    @action
    protected onVerifyingLanding(UserInfo: any = {}) {
        // jwt
        if (UserInfo.access_token) {
            lodash.set(Request.headers, 'Authorization', `${UserInfo.token_type} ${UserInfo.access_token}`);
        } else {
            // this.Loading = false;
            this.Name = lodash.get(UserInfo, 'Name');
            this.Id = lodash.get(UserInfo, 'Id');
            this.ITCode = lodash.get(UserInfo, 'ITCode');
            // 动作
            this._Actions = lodash.get(UserInfo, 'Attributes.Actions', []);
            const PhotoId = lodash.get(UserInfo, 'PhotoId');
            this.Avatar = PhotoId ? `/api/_file/getFile/${PhotoId}` : 'https://zos.alipayobjects.com/rmsportal/ODTLcjxAfvqbxHnVXCYX.png';
            // 格式化 菜单 
            const Menus = lodash.get(UserInfo, 'Attributes.Menus', []).map(data => {
                return {
                    ...data,
                    key: data.Id,
                    path: data.Url || '',
                    name: data.Text,
                    icon: data.Icon || "pic-right",
                    // children: data.Children
                }
            });
            const onAnalysisMenus = async () => {
                await this.onAnalysisMenus(Menus);
                this.UserSubject.next(this);
            }
            lodash.defer(() => {
                onAnalysisMenus();
            })
        }

    }
    /**
     * 解析菜单 
     * @memberof EntitiesUserBehavior
     */
    async onAnalysisMenus(Menus) {
        runInAction(() => {
            this.Menus = Menus;
            this._MenuTrees = this.formatTree(Menus, null, []);
            this.Loading = false;
            this.OnlineState = true;
        })
    }
   
    /**
    * 递归 格式化 树
    * @param datalist 
    * @param ParentId 
    * @param children 
    */
    formatTree(datalist, ParentId, children: MenuDataItem[] = []) {
        lodash.filter(datalist, ['ParentId', ParentId]).map(data => {
            data = lodash.cloneDeep(data)
            data.children = this.formatTree(datalist, data.Id, data.children || []);
            children.push(data);
        });
        return children;
    }
}