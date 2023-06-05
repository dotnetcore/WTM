import WtmUploadImage from '/@/components/uploadImage/index.vue';
import WtmUploadFile from '/@/components/uploadFile/index.vue';
import WtmTable from '/@/components/table/index.vue';
import WtmSearcher from '/@/components/searchPanel/index.vue';
import WtmImportor from '/@/components/importor/index.vue';
import WtmChart from '/@/components/echart/index.vue';
import WtmButton from '/@/components/linkbutton/index.vue'
import WtmEditor from '/@/components/editor/index.vue'

declare module '@vue/runtime-core' {
    export interface GlobalComponents {
        WtmUploadImage: typeof WtmUploadImage;
        WtmUploadFile: typeof WtmUploadFile;
        WtmTable: typeof WtmTable;
        WtmSearcher: typeof WtmSearcher;        
        WtmImportor: typeof WtmImportor;
        WtmChart: typeof WtmChart;
        WtmChart: typeof WtmChart;
        WtmButton: typeof WtmButton;
        WtmEditor: typeof WtmEditor;
    }
  }
