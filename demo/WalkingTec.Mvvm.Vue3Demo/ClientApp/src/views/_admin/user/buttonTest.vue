<template>
    <div class="card-fill layout-padding">
        <el-card shadow="hover" class="layout-padding-auto">
            <div>
                <WtmButton button-text="普通按钮" type="primary" />
                <WtmButton button-text="带图标" type="danger" icon="fa fa-file" />
                <WtmButton button-text="带提示" icon="fa fa-file" confirm="确定么？" @click="click()" />
            </div>
            <br />
            <div>
                <WtmButton button-text="本页跳转" url="/system/frameworkgroup/index" target="self" />
                <WtmButton button-text="新窗口跳转" url="/system/frameworkgroup/index" target="newwindow" />
                <WtmButton button-text="弹窗" url="/system/frameworkgroup/index" dialog-title="弹框" target="dialog" />
            </div>
            <br />
            <el-row>
                <el-col :xs="24" :lg="3">
                    <WtmButton button-text="文字按钮" :is-text="true" />
                </el-col>
                <el-col :xs="24" :lg="3">
                    <WtmButton button-text="快捷方式" icon="fa fa-file" :is-short-cut="true" url="/system/frameworkgroup/index"
                        target="self" />
                    <WtmButton button-text="快捷方式2" icon="fa fa-file" :is-short-cut="true" url="/system/frameworkgroup/index"
                        target="self" />
                </el-col>
            </el-row>
        </el-card>
    </div>
</template>

<script setup lang="ts" >

const click = () => {
    window.alert('aaa')
}
</script> 