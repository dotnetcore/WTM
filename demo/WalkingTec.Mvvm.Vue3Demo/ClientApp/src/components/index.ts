import type { App } from 'vue';
import WtmUploadImage from '/@/components/uploadImage/index.vue';
import WtmUploadFile from '/@/components/uploadFile/index.vue';
import WtmTable from '/@/components/table/index.vue';
import WtmSearcher from '/@/components/searchPanel/index.vue';
import WtmImportor from '/@/components/importor/index.vue';
import WtmChart from '/@/components/echart/index.vue';
import WtmButton from '/@/components/linkbutton/index.vue'
import WtmEditor from '/@/components/editor/index.vue'

export default {
    install: (app:App) => {        
       app.component('WtmUploadImage',WtmUploadImage);
       app.component('WtmUploadFile',WtmUploadFile);
       app.component('WtmTable',WtmTable);
       app.component('WtmSearcher',WtmSearcher);
       app.component('WtmImportor',WtmImportor);
       app.component('WtmChart',WtmChart);
       app.component('WtmButton',WtmButton);
       app.component('WtmEditor',WtmEditor);
    }
  }