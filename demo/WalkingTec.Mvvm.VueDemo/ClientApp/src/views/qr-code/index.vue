<template>
    <div class="qrcode">
        <wrap :title="nspsInfo.name">
            <div class="code-box">
                <div class="code-item">
                    <img :src="buyUrl">
                    <span>购车二维码</span>
                    <a :href="buyHref" download="购车二维码">
                        <el-button type="primary">下载</el-button>
                    </a>
                </div>
                <div class="code-item">
                    <img :src="lzUrl">
                    <span>留咨二维码</span>
                    <a :href="lzHref" download="留资二维码">
                        <el-button type="primary">下载</el-button>
                    </a>
                </div>
            </div>
            <div v-if="!lzUrl" class="loading-box">
                <div class="el-loading-spinner">
                    <svg viewBox="25 25 50 50" class="circular">
                        <circle cx="50" cy="50" r="20" fill="none" class="path"></circle>
                    </svg>
                </div>
            </div>
        </wrap>
    </div>
</template>

<script lang='ts'>
import { Component, Vue } from "vue-property-decorator";
import store from "@/store/setting/qr-code";
import { Action, State } from "vuex-class";
import wrap from "@/components/layout/wrap.vue";
import { getBase64Str, getDownLoadUrl } from "@/util/string";
@Component({
    store,
    components: { wrap }
    })
export default class qrCode extends Vue {
    @Action
    getQrCodes;
    @Action
    postQrCodesNoauth;
    @Action
    getNspsInfo;
    @State
    nspsInfo;
    // 购车
    buyUrl: string = "";
    buyHref: string = "";
    // 留资
    lzUrl: string = "";
    lzHref: string = "";
    created() {
        this.getQrCodes().then(res => {
            if (res.result_code) {
            } else {
                this.lzUrl = getBase64Str(res);
                this.lzHref = getDownLoadUrl(this.lzUrl);
            }
        });
        const bizData = JSON.stringify({
            width: "1280",
            campaignSourceCode: "dU82G2OITK",
            page: "pages/AboutES8"
        });
        this.postQrCodesNoauth({
            bizData
        }).then(res => {
            this.buyUrl = getBase64Str(res);
            this.buyHref = getDownLoadUrl(this.buyUrl);
        });

        this.getNspsInfo();
    }
}
</script>
<style lang='less'>
@import "~@/assets/css/mixin.less";
@import "~@/assets/css/variable.less";
.qrcode {
    .code-box {
        .flexbox();
        margin-top: 46vw * @vwUnit;
        .code-item {
            .center(column);
            margin-right: 200vw * @vwUnit;
            width: 200vw * @vwUnit;
            & > img {
                width: 200vw * @vwUnit;
                height: 200vw * @vwUnit;
                margin-bottom: 32vw * @vwUnit;
            }
            & > span {
                font-size: 16px;
                font-weight: 400;
                color: #303133;
                margin-bottom: 16vw * @vwUnit;
            }
            & > a {
                margin-bottom: 70vw * @vwUnit;
            }
        }
    }
    .loading-box {
        position: relative;
        height: 300px;
    }
}
</style>
