/**
 * @author 冷 (https://github.com/LengYXin)
 * @email lengyingxin8966@gmail.com
 * @create date 2018-09-12 18:52:27
 * @modify date 2018-09-12 18:52:27
 * @desc [description] .
 */
import * as React from 'react';
import { message, notification, List, Row, Col, Button } from 'antd';
import { action, computed, observable, runInAction, toJS } from 'mobx';
import { Request } from 'utils/Request';
import RequestFiles from 'utils/RequestFiles';
import lodash from 'lodash';
import { Help } from 'utils/Help';
import globalConfig from 'global.config';

export default class Store {
  /** 配置 */
  Consfig
  /** 搜索 */
  Search
  /** 详情 */
  Details
  /** 编辑 */
  Edit
  /** 文件 */
  Files
  /** 其他 */
  Other
}
