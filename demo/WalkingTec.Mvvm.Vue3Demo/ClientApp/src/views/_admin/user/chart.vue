<template>
	<div class="card-fill layout-padding">
		<el-card shadow="hover" class="layout-padding-auto">
			<WtmSearcher v-model="searchData" @search="getChartData()">
				<el-row>
					<el-col :xs="24" :lg="12" class="mb20">
						<el-form-item ref="test1_FormItem" prop="test1" label="测试1">
							<el-input v-model="searchData.test1" clearable></el-input>
						</el-form-item>
					</el-col>
					<el-col :xs="24" :lg="12" class="mb20">
						<el-form-item ref="test2_FormItem" prop="test2" label="测试2">
							<el-input v-model="searchData.test2" clearable></el-input>
						</el-form-item>
					</el-col>
				</el-row>
			</WtmSearcher>
			<WtmChart ref="chart1" :request="() => `api/School/GetCharts?id=${searchData.test1}`" type="bar"
				style="height:400px;" />
		</el-card>
	</div>
</template>

<script setup lang="ts" name="测试图表">
import { defineAsyncComponent, reactive, onMounted, onUpdated, ref, nextTick } from 'vue';
import { ElMessageBox, ElMessage } from 'element-plus';
import frameworkuserApi from '/@/api/frameworkuser';
import { useThemeConfig } from '/@/stores/themeConfig';
import other from '/@/utils/other';
import { useI18n } from 'vue-i18n';

const chart1 = ref();
const searchData = ref({
	test1: 1,
	test2: 2
});

const getChartData = () => {
	chart1.value.doSearch(null, searchData.value);
};

</script>

<style scoped lang="scss"></style>
