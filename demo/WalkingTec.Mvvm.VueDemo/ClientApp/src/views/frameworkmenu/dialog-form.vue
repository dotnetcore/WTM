<template>
  <wtm-dialog-box componentClass="frameworkmenu-form" :ref="refName" :rules="rules" :is-show.sync="isShow" :model="formData" :status="status" @close="onClose" @open="onBindFormData" @onSubmit="onSubmitForm">
    <el-row>
      <wtm-form-item ref="Entity.IsInside" label="地址类型" prop="Entity.IsInside" :status="status" :span="24">
        <el-radio-group v-model="formData.Entity.IsInside">
          <el-radio :label="true">
            内部地址
          </el-radio>
          <el-radio :label="false">
            外部地址
          </el-radio>
        </el-radio-group>
        <template #editValue>
          {{ formData.Entity.IsInside==='true' ? "内部地址" : "外部地址" }}
        </template>
      </wtm-form-item>
    </el-row>
    <el-row v-show="formData.Entity.IsInside">
      <wtm-form-item label="模块名称">
        <el-select v-model="formData.SelectedModule" v-edit:[status]="moduleList" filterable placeholder="请选择" @change="onSelectedAction">
          <el-option v-for="item in moduleList" :key="item.Value" :label="item.Text" :value="item.Value" />
        </el-select>
      </wtm-form-item>
      <wtm-form-item label="动作名称">
        <el-select v-model="formData.SelectedActionIDs" v-edit:[status]="getActionsByModelData" multiple placeholder="请选择">
          <el-option v-for="item in getActionsByModelData" :key="item.Value" :label="item.Text" :value="item.Value" />
        </el-select>
      </wtm-form-item>
    </el-row>
    <el-row v-show="!formData.Entity.IsInside">
      <wtm-form-item ref="Entity.Url" label="Url" :span="24">
        <el-input v-model="formData.Entity.Url" v-edit:[status] />
      </wtm-form-item>
    </el-row>
    <wtm-form-item ref="Entity.PageName" label="页面名称">
      <el-input v-model="formData.Entity.PageName" v-edit:[status] placeholder="请输入内容" />
    </wtm-form-item>
    <wtm-form-item ref="Entity.ParentId" label="父目录">
      <el-select v-model="formData.Entity.ParentId" v-edit:[status]="getFoldersData" filterable placeholder="请选择">
        <el-option v-for="item in getFoldersData" :key="item.Value" :label="item.Text" :value="item.Value" />
      </el-select>
    </wtm-form-item>
    <wtm-form-item ref="Entity.FolderOnly" label="目录">
      <el-switch v-model="formData.Entity.FolderOnly" v-edit:[status] />
    </wtm-form-item>
    <wtm-form-item ref="Entity.ShowOnMenu" label="菜单显示">
      <el-switch v-model="formData.Entity.ShowOnMenu" v-edit:[status] />
    </wtm-form-item>
    <wtm-form-item ref="Entity.IsPublic" label="公开">
      <el-switch v-model="formData.Entity.IsPublic" v-edit:[status] />
    </wtm-form-item>
    <wtm-form-item ref="Entity.DisplayOrder" label="顺序" prop="Entity.DisplayOrder">
      <el-input v-model="formData.Entity.DisplayOrder" v-edit:[status] placeholder="请输入顺序" />
    </wtm-form-item>
    <wtm-form-item ref="Entity.ICon" label="图标" :status="status" :span="24">
      <el-select v-model="formData.Entity.ICon" v-edit:[status] filterable placeholder="请选择">
        <el-option v-for="(item, index) in iconList" :key="index" :label="item" :value="item">
          <span style="float: left">{{ item }}</span>
          <span style="float: right; font-size: 14px"><i :class="[item]"></i></span>
        </el-option>
      </el-select>
      <template #editValue>
        <i :class="[formData.Entity.ICon]"></i>
      </template>
    </wtm-form-item>
  </wtm-dialog-box>
</template>

<script lang='ts'>
import { Component, Vue, Watch } from "vue-property-decorator";
import { Action, State } from "vuex-class";
import mixinForm from "@/vue-custom/mixin/form-mixin";
import { RoutesModule } from "@/store/modules/routes";

// 表单结构
const defaultFormData = {
    // 表单数据
    formData: {
        SelectedModule: "",
        SelectedActionIDs: [],
        Entity: {
            ID: "",
            ParentId: "",
            DomainId: "",
            PageName: "",
            ICon: "",
            DisplayOrder: 0,
            IsInside: true,
            ShowOnMenu: false,
            IsPublic: false,
            Url: "",
            FolderOnly: false
        }
    }
};

@Component({ mixins: [mixinForm(defaultFormData)] })
export default class Index extends Vue {
    @Action
    getActionsByModel;
    @Action
    getFolders;
    @State
    getActionsByModelData;
    @State
    getFoldersData;

    // 验证 ★★★★★
    rules = {
        "Entity.DisplayOrder": [
            {
                required: true,
                message: "请输入顺序",
                trigger: "blur"
            }
        ]
    };

    /**
     * 模版集合
     */
    get moduleList() {
        return RoutesModule.pageList;
    }
    // 空动作，清理列表
    @Watch("formData.SelectedModule")
    clearActions(newVal?: string) {
        if (!newVal && this.getActionsByModelData.length > 0) {
            this.getActionsByModel({
                ModelName: ""
            });
        }
    }
    iconList: Array<
        any
    > = "el-icon-platform-eleme,el-icon-eleme,el-icon-delete-solid,el-icon-delete,el-icon-s-tools,el-icon-setting,el-icon-user-solid,el-icon-user,el-icon-phone,el-icon-phone-outline,el-icon-more,el-icon-more-outline,el-icon-star-on,el-icon-star-off,el-icon-s-goods,el-icon-goods,el-icon-warning,el-icon-warning-outline,el-icon-question,el-icon-info,el-icon-remove,el-icon-circle-plus,el-icon-success,el-icon-error,el-icon-zoom-in,el-icon-zoom-out,el-icon-remove-outline,el-icon-circle-plus-outline,el-icon-circle-check,el-icon-circle-close,el-icon-s-help,el-icon-help,el-icon-minus,el-icon-plus,el-icon-check,el-icon-close,el-icon-picture,el-icon-picture-outline,el-icon-picture-outline-round,el-icon-upload,el-icon-upload2,el-icon-download,el-icon-camera-solid,el-icon-camera,el-icon-video-camera-solid,el-icon-video-camera,el-icon-message-solid,el-icon-bell,el-icon-s-cooperation,el-icon-s-order,el-icon-s-platform,el-icon-s-fold,el-icon-s-unfold,el-icon-s-operation,el-icon-s-promotion,el-icon-s-home,el-icon-s-release,el-icon-s-ticket,el-icon-s-management,el-icon-s-open,el-icon-s-shop,el-icon-s-marketing,el-icon-s-flag,el-icon-s-comment,el-icon-s-finance,el-icon-s-claim,el-icon-s-custom,el-icon-s-opportunity,el-icon-s-data,el-icon-s-check,el-icon-s-grid,el-icon-menu,el-icon-share,el-icon-d-caret,el-icon-caret-left,el-icon-caret-right,el-icon-caret-bottom,el-icon-caret-top,el-icon-bottom-left,el-icon-bottom-right,el-icon-back,el-icon-right,el-icon-bottom,el-icon-top,el-icon-top-left,el-icon-top-right,el-icon-arrow-left,el-icon-arrow-right,el-icon-arrow-down,el-icon-arrow-up,el-icon-d-arrow-left,el-icon-d-arrow-right,el-icon-video-pause,el-icon-video-play,el-icon-refresh,el-icon-refresh-right,el-icon-refresh-left,el-icon-finished,el-icon-sort,el-icon-sort-up,el-icon-sort-down,el-icon-rank,el-icon-loading,el-icon-view,el-icon-c-scale-to-original,el-icon-date,el-icon-edit,el-icon-edit-outline,el-icon-folder,el-icon-folder-opened,el-icon-folder-add,el-icon-folder-remove,el-icon-folder-delete,el-icon-folder-checked,el-icon-tickets,el-icon-document-remove,el-icon-document-delete,el-icon-document-copy,el-icon-document-checked,el-icon-document,el-icon-document-add,el-icon-printer,el-icon-paperclip,el-icon-takeaway-box,el-icon-search,el-icon-monitor,el-icon-attract,el-icon-mobile,el-icon-scissors,el-icon-umbrella,el-icon-headset,el-icon-brush,el-icon-mouse,el-icon-coordinate,el-icon-magic-stick,el-icon-reading,el-icon-data-line,el-icon-data-board,el-icon-pie-chart,el-icon-data-analysis,el-icon-collection-tag,el-icon-film,el-icon-suitcase,el-icon-suitcase-1,el-icon-receiving,el-icon-collection,el-icon-files,el-icon-notebook-1,el-icon-notebook-2,el-icon-toilet-paper,el-icon-office-building,el-icon-school,el-icon-table-lamp,el-icon-house,el-icon-no-smoking,el-icon-smoking,el-icon-shopping-cart-full,el-icon-shopping-cart-1,el-icon-shopping-cart-2,el-icon-shopping-bag-1,el-icon-shopping-bag-2,el-icon-sold-out,el-icon-sell,el-icon-present,el-icon-box,el-icon-bank-card,el-icon-money,el-icon-coin,el-icon-wallet,el-icon-discount,el-icon-price-tag,el-icon-news,el-icon-guide,el-icon-male,el-icon-female,el-icon-thumb,el-icon-cpu,el-icon-link,el-icon-connection,el-icon-open,el-icon-turn-off,el-icon-set-up,el-icon-chat-round,el-icon-chat-line-round,el-icon-chat-square,el-icon-chat-dot-round,el-icon-chat-dot-square,el-icon-chat-line-square,el-icon-message,el-icon-postcard,el-icon-position,el-icon-turn-off-microphone,el-icon-microphone,el-icon-close-notification,el-icon-bangzhu,el-icon-time,el-icon-odometer,el-icon-crop,el-icon-aim,el-icon-switch-button,el-icon-full-screen,el-icon-copy-document,el-icon-mic,el-icon-stopwatch,el-icon-medal-1,el-icon-medal,el-icon-trophy,el-icon-trophy-1,el-icon-first-aid-kit,el-icon-discover,el-icon-place,el-icon-location,el-icon-location-outline,el-icon-location-information,el-icon-add-location,el-icon-delete-location,el-icon-map-location,el-icon-alarm-clock,el-icon-timer,el-icon-watch-1,el-icon-watch,el-icon-lock,el-icon-unlock,el-icon-key,el-icon-service,el-icon-mobile-phone,el-icon-bicycle,el-icon-truck,el-icon-ship,el-icon-basketball,el-icon-football,el-icon-soccer,el-icon-baseball,el-icon-wind-power,el-icon-light-rain,el-icon-lightning,el-icon-heavy-rain,el-icon-sunrise,el-icon-sunrise-1,el-icon-sunset,el-icon-sunny,el-icon-cloudy,el-icon-partly-cloudy,el-icon-cloudy-and-sunny,el-icon-moon,el-icon-moon-night,el-icon-dish,el-icon-dish-1,el-icon-food,el-icon-chicken,el-icon-fork-spoon,el-icon-knife-fork,el-icon-burger,el-icon-tableware,el-icon-sugar,el-icon-dessert,el-icon-ice-cream,el-icon-hot-water,el-icon-water-cup,el-icon-coffee-cup,el-icon-cold-drink,el-icon-goblet,el-icon-goblet-full,el-icon-goblet-square,el-icon-goblet-square-full,el-icon-refrigerator,el-icon-grape,el-icon-watermelon,el-icon-cherry,el-icon-apple,el-icon-pear,el-icon-orange,el-icon-coffee,el-icon-ice-tea,el-icon-ice-drink,el-icon-milk-tea,el-icon-potato-strips,el-icon-lollipop,el-icon-ice-cream-square,el-icon-ice-cream-round".split(
        ","
    );
    created() {
        this.getFolders();
    }
    /**
     * 查询详情-after-调用
     */
    afterBindFormData(data) {
        _.set(this, `formData.SelectedModule`, data.SelectedModule);
        _.set(this, `formData.SelectedActionIDs`, data.SelectedActionIDs);
        this.onSelectedAction(false);
    }
    /**
     * 动作名称
     */
    onSelectedAction(isClear) {
        isClear && _.set(this, `formData.SelectedActionIDs`, []);
        this.getActionsByModel({
            ModelName: _.get(this, `formData.SelectedModule`) || ""
        });
    }
}
</script>
<style lang='less'>
</style>
