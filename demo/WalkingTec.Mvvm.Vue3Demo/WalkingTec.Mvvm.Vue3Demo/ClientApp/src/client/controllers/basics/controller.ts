import { saveAs } from 'file-saver'
import { BindAll } from 'lodash-decorators'
import lodash from 'lodash';
import { action, observable } from "mobx"
import { AjaxBasics, Regulars } from "../../helpers"
import { BasicsOptions, EntitiesBasics } from "./entities"
import { Pagination, PaginationOptions } from "./pagination"
export interface ControllerBasicsOptions {
  /** 数据唯一标识 key */
  key?: string;
  /** 列表 */
  Pagination?: PaginationOptions;
  /** 实体详情 */
  Entities?: BasicsOptions;
}
@BindAll()

export class ControllerBasics<T = any> {
  constructor(protected $ajax: AjaxBasics, options?: ControllerBasicsOptions) {
    if (options) {
      this.onReset(options);
    }
  }
  /**
   * 配置
   * @type {ControllerBasicsOptions}
   * @memberof ControllerBasics
   */
  options: ControllerBasicsOptions = { key: 'id' };
  /**
    * 数据key
    * @readonly
    * @memberof Pagination
    */
  get key() {
    return this.options.key;
  }
  /**
    * 加载状态
    * @memberof EntitiesBasics
    */
  @observable
  loading = false;
  /**
   * 列表
   *
   * @memberof ControllerBasics
   */
  Pagination: Pagination<T>// = new Pagination(this.$ajax, this.options.Pagination)
  /**
   * 实体详情
   *
   * @memberof ControllerBasics
   */
  Entities: EntitiesBasics<T>// = new EntitiesBasics(this.$ajax, this.options.Entities)
  /**
  * 重置
  * @returns
  * @memberof Pagination
  */
  onReset(options?: ControllerBasicsOptions) {
    this.options = lodash.merge({}, this.options, options);
    this.Pagination = new Pagination(this.$ajax, this.options.Pagination);
    this.Entities = new EntitiesBasics(this.$ajax, this.options.Entities);
    this.onToggleLoading(false)
    return this;
  }
  /**
    * 切换加载状态
    * @private
    * @param {boolean} [loading=!this.loading]
    * @memberof Pagination
    */
  @action
  protected onToggleLoading(loading: boolean = !this.loading) {
    this.loading = loading;
  }
  /**
   * 添加
   */
  async onInstall(entities: T, msg: string) {
    try {
      this.onToggleLoading(true);
      const res = await this.$ajax.post('', entities)
      // this.Pagination.onCurrentChange(1, this.Pagination.oldBody)
      ControllerBasics.$msg(msg)
      this.onToggleLoading(false);
      return res
    } catch (error) {
      this.onToggleLoading(false);
      throw error
    }
  }
  /**
   * 修改更新
   * @param key 
   * @param value 
   */
  async onUpdate(entities: T, msg: string) {
    try {
      this.onToggleLoading(true);
      const res = await this.$ajax.put('', entities)
      this.Entities.onUpdate(old => {
        return lodash.assign(old, entities)
      })
      this.Pagination.onUpdate(entities, old => {
        return lodash.assign(old, entities)
      })
      ControllerBasics.$msg(msg)
      this.onToggleLoading(false);
    } catch (error) {
      this.onToggleLoading(false);
      throw error
    }
  }
  /**
   * 排序
   * @param entities 元素
   * @param overIndex 索引
   */
  async onOrderBy(entities: T, overIndex, msg: string) {
    try {
      this.onToggleLoading(true);
      this.Pagination.onOrderBy(entities, overIndex)
      await this.$ajax.put('', lodash.map(this.Pagination.dataSource, this.key))
      ControllerBasics.$msg(msg)
      this.onToggleLoading(false);
    } catch (error) {
      this.onToggleLoading(false);
      throw error
    }
  }
  /**
   * 根据 key 删除数据
   * @param {string} key
   * @returns {T[]}
   * @memberof Pagination
   */
  async onRemove(key: any, msg: string) {
    try {
      this.onToggleLoading(true);
      let ids = []
      if (lodash.isArray(key)) {
        ids = lodash.map(key, this.key)
      } else if (lodash.isObject(key)) {
        ids = [lodash.get(key, this.key)]
      } else {
        ids = [key]
      }
      await this.$ajax.delete('', { ids })
      this.Pagination.onRemove(key);
      ControllerBasics.$msg(msg)
    } catch (error) {
      console.error(error)
    }
    this.onToggleLoading(false);
  }
  /**
   * 导出
   * @param body 
   */
  async onExport(body) {
    // if (!this.options.export) {
    //   throw "没有配置导出路径"
    // }
    const res: any = await this.$ajax.request({ url: '', body, responseType: 'blob' }).toPromise();
    const disposition = res.xhr.getResponseHeader('content-disposition');
    Regulars.filename.test(disposition);
    saveAs(res.response, decodeURI(RegExp.$1) || `${Date.now()}.xls`);
  }
  /**
    * 获取 lodash Predicate 参数
    * @param key 
    */
  getPredicate(key: string | T): any {
    if (lodash.isObject(key)) {
      return { [this.key]: lodash.get(key, this.key) }
    }
    return { [this.key]: key }
  }
  /**
   * 消息提示
   * @param msg 
   * @param type 
   */
  static $msg = function (msg, type: "success" | "info" | "warning" | "error" | "loading" = "success") {
    console.log("LENG: ControllerBasics<T> -> msg", msg, type)
  }
}