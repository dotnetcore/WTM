<template>
    <div class="shopmange">
        <wrap title="NSP信息">
            <div class="nsp-info">
                <div class="nsp-info-item">
                    <span class="item-key">NSP名称</span>
                    <span class="item-value">{{nspsInfo.name}}</span>
                </div>
                <div class="nsp-info-item">
                    <span class="item-key">合作期限</span>
                    <span class="item-value">{{nspsInfo.contract_begin_time}} ~ {{nspsInfo.contract_end_time}}</span>
                </div>
                <div class="nsp-info-item">
                    <span class="item-key">归属NIO管辖大区</span>
                    <span class="item-value">{{nspsInfo.city_id}}</span>
                </div>
                <div class="nsp-info-item">
                    <span class="item-key">NIO驻店专属Fellow</span>
                    <span class="item-value">{{nspsInfo.fellow && nspsInfo.fellow.name}} ({{nspsInfo.fellow && nspsInfo.fellow.mobile}})</span>
                </div>
            </div>
        </wrap>
        <wrap title="门店信息" class="shopinfo">
            <div class="shop-info">
                <div class="shop-info-item">
                    <span class="item-key">门店电话</span>
                    <span class="item-value">
                        <edit :isEdit="isEdit" :val="formData.phone" :error="errorMsg.phoneError">
                            <el-input class="value-ipt" v-model="formData.phone" placeholder="请输入门店电话" @blur="onBlurPhone"></el-input>
                        </edit>
                    </span>
                </div>
                <div class="shop-info-item">
                    <span class="item-key">详细地址</span>
                    <span class="item-value">
                        <edit :isEdit="isEdit" :val="formData.address" :error="errorMsg.addressError">
                            <el-input class="value-ipt" v-model="formData.address" placeholder="请输入详细地址" @blur="onBlurAddress"></el-input>
                        </edit>
                    </span>
                </div>
                <div class="shop-info-item shop-imgbox">
                    <span class="item-key">门店图片</span>
                    <span class="item-value">
                        <edit :isEdit="isEdit" :val="formData.image_url" :error="errorMsg.imgError" isImg="true">
                            <upload-img @getUrl="onSetImageUrl" :url="formData.image_url"></upload-img>
                        </edit>
                    </span>
                </div>
                <div class="shop-info-item shop-imgbox">
                    <span class="item-key"></span>
                    <el-button v-show="!isEdit" type="primary" @click="isEdit = true">编辑</el-button>
                    <el-button v-show="isEdit" type="primary" @click="onSubmit">保存</el-button>
                    <el-button v-show="isEdit" @click="initData()">取消</el-button>
                </div>
            </div>
        </wrap>
    </div>
</template>

<script lang='ts'>
import { Component, Vue } from "vue-property-decorator";
import store from "@/store/setting/shop-manage";
import { Action, State } from "vuex-class";
import wrap from "@/components/layout/wrap.vue";
import uploadImg from "./upload-img.vue";
import edit from "./edit.vue";
import validator from "@/util/validator";

@Component({
    store,
    components: { wrap, "upload-img": uploadImg, edit }
})
export default class Test extends Vue {
    @Action
    getNspsInfo;
    @Action
    postNspsEdit;
    @State
    nspsInfo;
    // 表单
    formData = {
        phone: "",
        address: "",
        image_url: ""
    };
    // error 信息
    errorMsg = {
        phoneError: "",
        addressError: "",
        imgError: ""
    };
    isEdit: boolean = false;
    created() {
        this.getNspsInfo().then(res => {
            this.initData();
        });
    }
    // 初始化
    initData(nspsInfo?) {
        console.log(!nspsInfo, nspsInfo);
        if (!nspsInfo) {
            nspsInfo = this.nspsInfo;
        }
        this.formData.phone = nspsInfo.phone;
        this.formData.address = nspsInfo.address;
        this.formData.image_url = nspsInfo.image_url;
        this.isEdit = false;
    }
    // 上传图片
    onSetImageUrl(url) {
        this.formData.image_url = url;
        this.onVerImgUrl();
    }
    // 提交
    onSubmit() {
        this.onBlurPhone();
        this.onBlurAddress();
        this.onVerImgUrl();
        if (
            !this.errorMsg.phoneError &&
            !this.errorMsg.addressError &&
            !this.errorMsg.imgError
        ) {
            this.postNspsEdit({
                ...this.formData
            }).then(res => {
                if (res.result_code === "success") {
                    this.initData(res.data);
                } else {
                    this.$message.error(res.message);
                }
            });
        }
    }

    onBlurPhone() {
        this.errorMsg.phoneError = !validator.validateMobile(
            this.formData.phone
        )
            ? "请正确填写手机号码"
            : "";
    }
    onBlurAddress() {
        this.errorMsg.addressError = !this.formData.address.trim()
            ? "请正确详细地址"
            : "";
    }
    onVerImgUrl() {
        this.errorMsg.imgError = !this.formData.image_url
            ? "请上传门店图片"
            : "";
    }
}
</script>
<style lang='less'>
@import "~@/assets/css/variable.less";
.shopmange {
    font-size: 14px;
    .shopinfo {
        margin: 16vw * @vwUnit 0;
    }
    .nsp-info {
        margin-top: 16vw * @vwUnit;
        .nsp-info-item {
            font-weight: 400;
            margin-bottom: 16vw * @vwUnit;
            .item-key {
                color: #909399;
                display: inline-block;
                width: 154vw * @vwUnit;
            }
            .item-value {
                color: #303133;
            }
        }
    }
    .shop-info {
        margin-top: 24vw * @vwUnit;
        .shop-info-item {
            margin-bottom: 24vw * @vwUnit;
            &.shop-imgbox {
                display: flex;
            }
            .item-key {
                color: #909399;
                display: inline-block;
                width: 80vw * @vwUnit;
                line-height: 40vw * @vwUnit;
            }
            .item-value {
                .value-ipt {
                    width: 360vw * @vwUnit;
                    height: 40vw * @vwUnit;
                }
                .upload-demo {
                    display: inline-block;
                }
            }
        }
    }
}
</style>
