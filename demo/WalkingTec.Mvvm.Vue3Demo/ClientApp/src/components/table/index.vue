<template>
	<div ref="forPrint" class="table-container">
		<div v-if="props.config.isSub === true && props.config.isDisabled !== true" style="text-align: right;">
			<el-button type="success" class="ml10" @click="subCreate()">
				<i class="fa fa-plus"></i>
				{{ $t('message._system.common.vm.add') }}
			</el-button>
		</div>
		<el-table :default-expand-all="true" :data="props.config.isSub === true ? listvalue : state.data" :border="setBorder"
			v-bind="$attrs" row-key="ID" stripe style="width: 100%" v-loading="config.loading && props.config.isSub !== true"
			@selection-change="onSelectionChange" @sort-change="onSortChange">
			<el-table-column type="selection" :reserve-selection="true" width="30" v-if="config.isSelection" />
			<el-table-column type="index" :label="$t('message._system.table.no')" width="60" v-if="config.isSerialNo" />
			<el-table-column v-for="(item, index) in setHeader" :key="index"
				:show-overflow-tooltip="item.type !== 'combobox' && item.type !== 'switch' && item.type !== 'date' && item.type !== 'textbox'"
				:prop="item.key" :width="item.colWidth" :label="item.title" :sortable="item.sortable" :align="item.align">
                <template v-slot="scope">
                    <template v-if="item.formatter">
                        {{item.formatter(scope.row,scope.row[item.key])}}
                    </template>
                    <template v-else-if="item.type === 'image'">
                        <el-image :preview-teleported="true" v-if="scope.row[item.key + '__localurl__'] !== ''"
                                  :style="{ 'width': item.imageWidth ? item.imageWidth + 'px' : 'undefined', 'height': item.imageHeight ? item.imageHeight + 'px' : 'undefined', 'border-radius': '5px' }"
                                  :src="scope.row[item.key + '__localurl__']" :initial-index="scope.row[item.key + '__preview__']"
                                  :preview-src-list="state.picList" :title="$t('message._system.table.preview')" />
                    </template>
                    <template v-else-if="item.type === 'file' && props.config.isSub !== true">
                        <el-button text type="primary" @click="download(scope.row[item.key])">
                            {{
				                $t('message._system.common.vm.download')
                            }}
                        </el-button>
                    </template>
                    <template v-else-if="item.type === 'icon' && props.config.isSub !== true">
                        <i :class="scope.row[item.key]"></i>
                    </template>
                    <template v-else-if="item.type === 'switch' && props.config.isSub !== true">
                        <el-switch :model-value="(scope.row[item.key] && (scope.row[item.key] === true || scope.row[item.key] === 'true')) ? true : false" />
                    </template>
                    <template v-else-if="item.type === 'switch' && props.config.isSub === true">
                        <el-switch v-model="listvalue[scope.$index][item.key]"
                                   :disabled="item.isDisabled || props.config.isDisabled" />
                    </template>
                    <template v-else-if="item.type === 'textbox'">
                        <el-input v-model="listvalue[scope.$index][item.key]"
                                  :disabled="item.isDisabled || props.config.isDisabled"></el-input>
                    </template>
                    <template v-else-if="item.type === 'combobox'">
                        <el-select v-model="listvalue[scope.$index][item.key]"
                                   :disabled="item.isDisabled || props.config.isDisabled">
                            <el-option v-for="(op, opkey, opindex) in state.comboData[item.key]" :key="opindex" :value="opkey"
                                       :label="op"></el-option>
                        </el-select>
                    </template>
                    <template v-else-if="item.type === 'date'">
                        <el-date-picker v-if="item.dateType !== 'time'" v-model="listvalue[scope.$index][item.key]"
                                        :type="item.dateType ?? 'datetime'" :disabled="item.isDisabled || props.config.isDisabled" />
                        <el-time-picker v-else v-model="listvalue[scope.$index][item.key]"
                                        :disabled="item.isDisabled || props.config.isDisabled" />
                    </template>
                    <template v-else>
                        {{ scope.row[item.key] }}
                    </template>
                </template>
			</el-table-column>

			<slot name="operation" v-if="props.config.isOperate && props.config.isDisabled != true">
				<el-table-column v-if="props.config.isSub === true" :label="$t('message._system.common.vm.operate')"
					width="70">
					<template v-slot="cell">
						<el-button text type="primary" @click="subDelete(cell.$index)">{{
							$t('message._system.common.vm.delete') }}</el-button>
					</template>
				</el-table-column>
			</slot>
			<slot name="customColumn">
			</slot>
			<template #empty>
				<el-empty v-if="props.config.isSub !== true" :description="$t('message._system.table.noData')" />
			</template>
		</el-table>
		<div class="table-footer mt15" v-if="props.config.isSub !== true">
			<el-pagination v-if="config.total > 0 && config.hidePagination!=true" v-model:current-page="state.searcher.Page"
				v-model:page-size="state.searcher.Limit" :pager-count="5" :page-sizes="[20, 50, 100, 200]"
				:total="config.total" layout="total, sizes, prev, pager, next, jumper" background
				@size-change="onHandleSizeChange" @current-change="onHandleCurrentChange">
			</el-pagination>
			<div class="table-footer-tool">
				<el-popover v-if="config.hideSetting!=true" placement="top-end" trigger="click" transition="el-zoom-in-top" popper-class="table-tool-popper"
					:width="300" :persistent="false" @show="onSetTable">
					<template #reference>
						<i class="fa fa-gear">{{ $t('message._system.table.setting') }}</i>
					</template>
					<template #default>
						<div class="tool-box">
							<el-tooltip :content="$t('message._system.table.drag')" placement="top-start">
								<i class="ml11 fa fa-question" :size="17" color="#909399"></i>
							</el-tooltip>
							<el-checkbox v-model="state.checkListAll" :indeterminate="state.checkListIndeterminate"
								class="ml10 mr1" :label="$t('message._system.table.columns')" @change="onCheckAllChange" />
							<el-checkbox v-model="getConfig.isSerialNo" class="ml12 mr1"
								:label="$t('message._system.table.no')" />
							<el-checkbox v-model="getConfig.isSelection" class="ml12 mr1"
								:label="$t('message._system.table.multi')" />
						</div>
						<el-scrollbar>
							<div ref="toolSetRef" class="tool-sortable">
								<div class="tool-sortable-item" v-for="v in header" :key="v.key" :data-key="v.key">
									<i class="fa fa-arrows-alt handle cursor-pointer"></i>
									<el-checkbox v-model="v.isCheck" class="ml12 mr8" :label="v.title"
										@change="onCheckChange" />
								</div>
							</div>
						</el-scrollbar>
					</template>
				</el-popover>
				<i v-if="config.hidePrint!=true" class="fa fa-print" @click="doPrint">{{ $t('message._system.table.print') }}</i>
				<i v-if="config.hideRefresh!=true" class="fa fa-refresh" @click="doSearch(null, null, isTreeState)">{{ $t('message._system.table.refresh')
				}}</i>
			</div>
		</div>
	</div>
</template>



<script setup lang="ts" name="netxTable">
    import { reactive, computed, nextTick, ref, getCurrentInstance, onMounted } from 'vue';
    import { ElMessage } from 'element-plus';
    import table2excel from 'js-table2excel';
    import Sortable from 'sortablejs';
    import { storeToRefs } from 'pinia';
    import { useThemeConfig } from '/@/stores/themeConfig';
    import fileapi from '/@/api/file'
    import '/@/theme/tableTool.scss';
    import { AxiosResponse } from 'axios';
    import { VueDevToolsIDs } from '@intlify/vue-devtools';
    import printJs from 'print-js';
    import other from '/@/utils/other';

    const emit = defineEmits(['update:modelValue']);
    // 定义父组件传过来的值
    const props = defineProps({
        // 列表内容
        data: {
            type: Array<any>,
            default: () => [],
        },
        /**
         * 表头内容,object数组，可设置
         * key:字符串类型，字段名称，
         * colWidth:字符串类型，列宽，
         * title：字符串类型，列头文字，
         * type：字符串类型，可填'text','switch','image','textbox,'combobox','date','icon','file'
         * align: 字符串，可填'left','right','center'，默认是left
         * isChecked:是否默认展示
         * isDisabled:列中控件是否显示为禁用状态
         * comboData:当type='combobox'时，指定下拉菜单的数据，可以为{value1:label1,value2:label2}这种格式，也可以为某个后台url
         * imageWidth:当type='image'时，指定图片宽度
         * imageHeight:当type='image时'，指定图片高度
         * dateType:当type='date'时，指定日期控件模式，可填写'date','month','year','week','datetime','time'
         * formatter:传递一个function(row,data)，row为该行的数据，data为该列绑定的数据，返回一个字符串用于显示
         */
        header: {
            type: Array<EmptyObjectType>,
            default: () => [],
        },
        /**
         * 配置项，可设置
         * total: 列表总数
         * loading: 布尔值，是否显示加载
         * isBorder: 布尔值，是否显示表格边框
         * isSerialNo: 布尔值，是否显示表格序号
         * isSelection: 布尔值，是否显示表格多选
         * isOperate: 布尔值，是否显示表格操作栏
         * isDisabled:布尔值，是否禁用
         * isSub:是否为子表控件
         * hidePagination:是否显示分页
         * hideSetting:是否显示设置按钮
         * hidePrint:是否显示打印按钮
         * hideRefresh:是否显示刷新按钮
         */
        config: {
            type: Object,
            default: () => { },
        },
        modelValue: {
            type: Array<EmptyObjectType>,
            default: () => [],
        }        
    });

    const listvalue = computed({
        get() {
            return props.modelValue;
        },
        set(value) {
            emit('update:modelValue', value)
        }
    })


    // 定义变量内容
    const ci = getCurrentInstance() as any;
    const toolSetRef = ref();
    const forPrint = ref();
    const storesThemeConfig = useThemeConfig();
    const { themeConfig } = storeToRefs(storesThemeConfig);
    const state = reactive({
        searcher: {
            Page: 1,
            Limit: 20,
            SortInfo: null as any
        },
        data: ref(props.data),
        picList: [] as string[],
        selectlist: [] as EmptyObjectType[],
        checkListAll: true,
        checkListIndeterminate: false,
        comboData:{},
    });
    let searchApi: Function = function (a: any = null, b: any = null) { };
    let isTreeState: any = null;
    // 设置边框显示/隐藏
    const setBorder = computed(() => {
        return Object.hasOwn(props.config, "isBorder") ? props.config.isBorder : false;
    });
    // 获取父组件 配置项（必传）
    const getConfig = computed(() => {
        return props.config;
    });
    // 设置 tool header 数据
    const setHeader = computed(() => {
        return props.header.filter((v) => v.isCheck);
    });
    // tool 列显示全选改变时
    const onCheckAllChange = <T>(val: T) => {
        if (val) props.header.forEach((v) => (v.isCheck = true));
        else props.header.forEach((v) => (v.isCheck = false));
        state.checkListIndeterminate = false;
    };
    // tool 列显示当前项改变时
    const onCheckChange = () => {
        const headers = props.header.filter((v) => v.isCheck).length;
        state.checkListAll = headers === props.header.length;
        state.checkListIndeterminate = headers > 0 && headers < props.header.length;
    };

    // 表格多选改变时，用于导出
    const onSelectionChange = (val: EmptyObjectType[]) => {
        state.selectlist = val;
    };

    // 排序改变时
    const onSortChange = (columninfo: any) => {
        if (columninfo.prop && columninfo.order) {
            state.searcher.SortInfo = {
                Property: columninfo.prop,
                Direction: columninfo.order === "descending" ? "Desc" : "Asc"
            }
        }
        else {
            state.searcher.SortInfo = null;
        }
        doSearch();
    };
    // 删除当前项
    const onDelRow = (row: EmptyObjectType) => {
        //emit('delRow', row);
    };
    // 分页改变
    const onHandleSizeChange = (val: number) => {
        state.searcher.Limit = val;
        doSearch();
    };
    // 分页改变
    const onHandleCurrentChange = (val: number) => {
        state.searcher.Page = val;
        doSearch();
    };

    onMounted(() => {
        const ch = props.header.filter((item) => item.type === 'combobox');
        const rv = {} as any;
        ch.forEach(element => {
            let cd = {} as any;
            if (element.comboData && typeof element.comboData == 'string') {
                other.getSelectList(element.comboData, [], false).then((data: any) => {
                    data.forEach((x: any) => {
                        cd[x.Value] = x.Text
                    });
                })
            }
            else {
                cd = element.comboData
            }
            const newdata = {};
            rv[element.key] = cd;
        });
        state.comboData = rv;
    });

    const doSearch = (api: any = null, para: any = null, isTree: any = null, parentKey: string = 'ParentId') => {

        if (para !== null) {
            Object.assign(state.searcher, para);
        }
        if (api != null) {
            searchApi = api;
        }
        if (isTree !== null) {
            isTreeState = isTree
        }

        let pro: Promise<AxiosResponse<any, any>> = searchApi(state.searcher);
        return pro.then(res => {
            const datatemp: any[] = [];
            const imageHeaders = props.header.filter((v) => v.isCheck && v.type === 'image');
            let index = 0;
            res.Data.forEach((element: EmptyObjectType<any>) => {
                imageHeaders.forEach((ih) => {
                    element[ih.key + "__localurl__"] = "";
                    if (element[ih.key]) {
                        element[ih.key + "__localurl__"] = '/api/_file/getfile/' + element[ih.key] + "?width=150&height=150";
                        element[ih.key + "__preview__"] = index++;
                        state.picList.push(element[ih.key + "__localurl__"]);
                    }
                })
                datatemp.push(element);
            });
            if (isTreeState !== true) {
                state.data = datatemp;
                props.config.total = res.Count;
                props.config.loading = false;
            }
            else {
                datatemp.forEach(element => {
                    element['children'] = datatemp.filter(x => x[parentKey] == element.ID);

                });
                state.data = datatemp.filter(x => !x[parentKey] || x[parentKey] == '')
                props.config.total = 0;
                props.config.loading = false;
            }
        }).catch(e => {
            props.config.loading = false;
        });
    }

    const setData = (data: any[], istree: any = false, parentKey: string = 'ParentId') => {
        if (istree !== true) {
            if (props.config.isSub) {
                listvalue.value = data;
            }
            else {
                state.data = data;
            }
            props.config.total = data.length;
            props.config.loading = false;
        }
        else {
            data.forEach(element => {
                element['children'] = data.filter(x => x[parentKey] == element.ID);
            });
            if (props.config.isSub) {
                listvalue.value = data.filter(x => !x[parentKey] || x[parentKey] == '');
            }
            else {
                state.data = data.filter(x => !x[parentKey] || x[parentKey] == '')
            }
            props.config.total = 0;
            props.config.loading = false;
        }
    }

    const doPrint = () => {
        var printdata = other.flatTree(state.data, setHeader.value[0].key)
        printJs({
            documentTitle: ' ',
            printable: printdata,
            type: 'json',
            properties: setHeader.value.map((item) => {
                return {
                    field: item.key,
                    displayName: item.title
                }
            }),
            gridHeaderStyle: `
			font-size:12px;
			border:0;
            border-top: 1px solid gray;
            border-left: 1px solid gray;
            border-right:1px solid gray;
            border-bottom:1px solid gray;
		`,
            gridStyle: `
			font-size:12px;
			border:0;
            border-top: 1px solid gray;
            border-left: 1px solid gray;
            border-right:1px solid gray;
            border-bottom:1px solid gray;
		`,
            style: `@page { size: landscape;} `,

        });
    }
    const getSelectedRows = () => {
        return state.selectlist;
    }


    // 设置
    const onSetTable = () => {
        nextTick(() => {
            const sortable = Sortable.create(toolSetRef.value, {
                handle: '.handle',
                dataIdAttr: 'data-key',
                animation: 150,
                onEnd: () => {
                    const headerList: EmptyObjectType[] = [];
                    sortable.toArray().forEach((val) => {
                        props.header.forEach((v) => {
                            if (v.key === val) headerList.push({ ...v });
                        });
                    });
                    props.header.length = 0;
                    headerList.forEach((val) => {
                        props.header.push(val);
                    })
                },
            });
        });
    };


    const subDelete = (index: any) => {

        listvalue.value.splice(index, 1);

    }

    const download = (data: any) => {

        fileapi().downloadFile(data);

    }

    const subCreate = () => {
        listvalue.value.push({});
    }
    // 暴露变量
    defineExpose({
        doSearch,
        getSelectedRows,
        setData,
        inheritAttrs: false
    });
</script>

<style scoped lang="scss">
.table-container {
	flex: 1;
	overflow: hidden;
	display: flex;
	flex-direction: column;

	.el-table {
		flex: 1;

	}

	.table-footer {
		display: flex;

		.table-footer-tool {
			flex: 1;
			display: flex;
			align-items: center;
			justify-content: flex-end;

			i {
				margin-right: 10px;
				cursor: pointer;
				color: var(--el-text-color-regular);

				&:last-of-type {
					margin-right: 0;
				}
			}
		}
	}
}</style>
